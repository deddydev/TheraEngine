using CustomEngine.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using SevenZip.Compression.LZMA;
using SevenZip;
using System.Runtime.InteropServices;

namespace CustomEngine.Files.Serialization
{
    public static unsafe partial class CustomBinarySerializer
    {
        private class MemberTreeNode
        {
            public MemberTreeNode(object root)
                : this(root, new VarInfo(root.GetType())) { }
            public MemberTreeNode(object obj, VarInfo info)
            {
                Object = obj;
                Info = info;
                List<VarInfo> members = SerializationCommon.CollectSerializedMembers(info.VariableType);
                Members = members.Select(x => new MemberTreeNode(obj == null ? null : x.GetValue(obj), x)).ToList();
                CategorizedMembers = Members.Where(x => x.Info.Category != null).GroupBy(x => x.Info.Category).ToList();
                foreach (var grouping in CategorizedMembers)
                    foreach (MemberTreeNode p in grouping)
                        Members.Remove(p);
                if (Object is IList array)
                {
                    ArrayMembers = new MemberTreeNode[array.Count];
                    for (int i = 0; i < array.Count; ++i)
                        ArrayMembers[i] = new MemberTreeNode(array[i]);
                        
                }
            }

            public object Object;
            public VarInfo Info;
            public int CalculatedSize;
            public int FlagSize;
            public List<MemberTreeNode> Members;
            public List<IGrouping<string, MemberTreeNode>> CategorizedMembers;
            public MemberTreeNode[] ArrayMembers;
        }
        /// <summary>
        /// Writes the given object to the path as binary.
        /// </summary>
        public static void Serialize(
            object obj,
            string filePath,
            Endian.EOrder order,
            bool compressed,
            ICodeProgress compressionProgress)
        {
            Endian.Order = order;

            //Create serialization tree, as it will be accessed in two passes:
            //Getting the size of the tree, allocating the space, and then writing the tree's data
            MemberTreeNode root = new MemberTreeNode(obj);
            
            StringTable table = new StringTable();
            int dataSize = GetSizeObject(root, table);
            int stringSize = table.GetTotalSize();
            int totalSize = FileCommonHeader.Size + dataSize + stringSize;

            //TODO: encryption

            using (FileMap uncompMap = compressed ? FileMap.FromTempFile(totalSize) :
                FileMap.FromFile(filePath, FileMapProtect.ReadWrite, 0, totalSize))
            {
                FileCommonHeader* hdr = (FileCommonHeader*)uncompMap.Address;
                hdr->_fileLength = totalSize;
                hdr->_stringTableLength = stringSize;
                table.WriteTable(hdr);
                WriteObject(root, hdr->Data, table);

                if (compressed)
                    using (FileStream compStream = new FileStream(filePath,
                            FileMode.OpenOrCreate,
                            FileAccess.ReadWrite,
                            FileShare.ReadWrite,
                            8,
                            FileOptions.RandomAccess))
                    {
                        compStream.SetLength(totalSize);
                        new Encoder().Code(uncompMap.BaseStream, compStream, totalSize, totalSize, compressionProgress);
                        compStream.SetLength(compStream.Position);
                    }
            }
        }
        private static int GetSizeObject(MemberTreeNode node, StringTable table)
        {
            if (node.Object == null)
                return 0;

            int size = 0;
            int flagCount = 0;

            MethodInfo[] customMethods = node.Info.VariableType.GetMethods(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(x => x.GetCustomAttribute<CustomBinarySerializeSizeMethod>() != null).ToArray();

            foreach (MemberTreeNode p in node.Members)
            {
                MethodInfo customMethod = customMethods.FirstOrDefault(
                    x => string.Equals(p.Info.Name, x.GetCustomAttribute<CustomBinarySerializeSizeMethod>().Name));
                if (customMethod != null)
                    size += (int)customMethod.Invoke(node.Object, new object[] { table });
                else
                    size += GetSizeMember(p, table, ref flagCount);
            }
            foreach (var grouping in node.CategorizedMembers)
            {
                foreach (MemberTreeNode p in grouping)
                {
                    MethodInfo customMethod = customMethods.FirstOrDefault(
                        x => string.Equals(p.Info.Name, x.GetCustomAttribute<CustomBinarySerializeSizeMethod>().Name));
                    if (customMethod != null)
                        size += (int)customMethod.Invoke(node.Object, new object[] { table });
                    else
                        size += GetSizeMember(p, table, ref flagCount);
                }
            }
            
            size += flagCount.Align(8) / 8;
            return size.Align(4);
        }
        private static int GetSizeMember(MemberTreeNode node, StringTable table, ref int flagCount)
        {
            int size = 0;
            object value = node.Object;
            if (value == null)
                return 0;
            if (node.Object is bool)
                ++flagCount;
            size += GetSizeObject(node, table);
            return size;
        }
        private static void WriteObject(MemberTreeNode node, VoidPtr address, StringTable table)
        {
            if (node.Object == null)
                return;

            //Write flags at the start of the object data block
            int flagIndex = 0;
            VoidPtr flagsAddr = address;
            address.MovePointer(node.FlagSize);

            MethodInfo[] customMethods = node.Info.VariableType.GetMethods(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(x => x.GetCustomAttribute<CustomBinarySerializeMethod>() != null).ToArray();
            
            foreach (MemberTreeNode p in node.Members)
            {
                MethodInfo customMethod = customMethods.FirstOrDefault(
                    x => string.Equals(p.Info.Name, x.GetCustomAttribute<CustomBinarySerializeMethod>().Name));
                if (customMethod != null)
                    customMethod.Invoke(node.Object, new object[] { address, table });
                else
                    WriteMember(p, address, table, flagsAddr, ref flagIndex);
            }
            foreach (var grouping in node.CategorizedMembers)
            {
                foreach (MemberTreeNode p in grouping)
                {
                    MethodInfo customMethod = customMethods.FirstOrDefault(
                        x => string.Equals(p.Info.Name, x.GetCustomAttribute<CustomBinarySerializeMethod>().Name));
                    if (customMethod != null)
                        customMethod.Invoke(node.Object, new object[] { address, table });
                    else
                        WriteMember(p, address, table, flagsAddr, ref flagIndex);
                }
            }
        }
        private static void WriteMember(
            MemberTreeNode node,
            VoidPtr address,
            StringTable table,
            VoidPtr flagsAddr,
            ref int flagIndex)
        {
            object value = node.Object;
            if (value == null)
                return;

            Type t = node.Info.VariableType;

            if (TryInterfaces(node, address, table, flagsAddr, ref flagIndex, value, t))
                return;

            if (value is bool)
            {
                if (flagIndex == 8)
                {
                    flagsAddr.MovePointer();
                    flagIndex = 0;
                    flagsAddr.Byte = 0;
                }
                flagsAddr.Byte |= (byte)(1 << (7 - flagIndex++));
            }
            else if (t.IsEnum || value is string)
            {
                string s = value.ToString();
                address.WriteInt(table[s]);
            }
            else if (t.IsValueType)
            {
                List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(node.Info.VariableType);
                if (structFields.Count > 0)
                    WriteObject(node, address, table);
                else
                {
                    Marshal.StructureToPtr(value, address, true);
                    address.MovePointer(Marshal.SizeOf(value));
                }
            }
            else
                WriteObject(node, address, table);
        }

        private static bool TryInterfaces(
            MemberTreeNode node,
            VoidPtr address,
            StringTable table,
            VoidPtr flagsAddr,
            ref int flagIndex,
            object value,
            Type t)
        {
            if (value is IList array)
            {
                WriteIList(array, node, address, table, flagsAddr, ref flagIndex, value, t);
                return true;
            }
            return false;
        }

        private static void WriteIList(IList array, MemberTreeNode node, VoidPtr address, StringTable table, VoidPtr flagsAddr, ref int flagIndex, object value, Type t)
        {
            address.WriteInt(array.Count);
            if (array.Count > 0)
            {
                Type elementType = array[0].GetType();
                if (elementType.IsEnum || array[0] is string)
                {
                    for (int i = 0; i < array.Count; ++i)
                    {
                        string s = array[i].ToString();
                        address.WriteInt(table[s]);
                    }
                }
                else if (elementType.IsValueType)
                {
                    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(array[0].GetType());
                    //Struct has serialized members within it?
                    //Needs a full element
                    if (structFields.Count > 0)
                    {
                        VoidPtr arrayBase = address;
                        VoidPtr arrayValue = arrayBase;
                        address.MovePointer(array.Count * 4);
                        foreach (object o in array)
                        {
                            arrayValue.WriteInt(address - arrayBase);
                            WriteObject(new MemberTreeNode(o), address, table);
                        }
                    }
                    else
                    {
                        foreach (object o in array)
                        {
                            Marshal.StructureToPtr(o, address, true);
                            address.MovePointer(Marshal.SizeOf(o));
                        }
                    }
                }
                else
                {
                    foreach (object o in array)
                        WriteObject(new MemberTreeNode(o), address, table);
                }
            }
        }
    }
}
