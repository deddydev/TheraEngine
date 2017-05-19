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
using System.Security.Cryptography;

namespace CustomEngine.Files.Serialization
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FileCommonHeader
    {
        public const int Size = 0x2C;

        public byte _encrypted;
        public byte _compressed;
        public bushort _endian;
        public bint _stringTableLength;
        public bint _fileLength;
        public fixed byte _hash[0x20];

        public bool Encrypted
        {
            get => _encrypted != 0;
            set => _encrypted = (byte)(value ? 1 : 0);
        }
        public bool Compressed
        {
            get => _compressed != 0;
            set => _compressed = (byte)(value ? 1 : 0);
        }
        public Endian.EOrder Endian
        {
            get => (Endian.EOrder)(ushort)_endian;
            set => _endian = (ushort)value;
        }
        public VoidPtr Strings => Address + Size;
        public VoidPtr Data => Strings + _stringTableLength;
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
    public static unsafe partial class CustomBinarySerializer
    {
        const string PASSWORD = "test_password";
        static readonly byte[] SALT = { 0x02, 0x47, 0x62, 0x32, 0x05, 0x25, 0x39, 0x84 };
        
        /// <summary>
        /// Writes the given object to the path as binary.
        /// </summary>
        public static void Serialize(
            object obj,
            string filePath,
            Endian.EOrder order,
            bool encrypted,
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

            using (FileMap uncompMap = compressed ? FileMap.FromTempFile(totalSize) :
                FileMap.FromFile(filePath, FileMapProtect.ReadWrite, 0, totalSize))
            {
                FileCommonHeader* hdr = (FileCommonHeader*)uncompMap.Address;
                hdr->_fileLength = totalSize;
                hdr->_stringTableLength = stringSize;
                hdr->Endian = order;
                hdr->Encrypted = encrypted;
                hdr->Compressed = compressed;
                table.WriteTable(hdr);
                VoidPtr addr = hdr->Data;
                WriteObject(root, ref addr, table);

                SHA256Managed SHhash = new SHA256Managed();
                byte[] hash = SHhash.ComputeHash(uncompMap.BaseStream);
                for (int i = 0; i < 0x20; ++i)
                    hdr->_hash[i] = hash[i];

                FileStream outStream;
                if (compressed)
                {
                    outStream = new FileStream(filePath,
                            FileMode.OpenOrCreate,
                            FileAccess.ReadWrite,
                            FileShare.ReadWrite,
                            8,
                            FileOptions.RandomAccess);
                    outStream.SetLength(totalSize);
                    outStream.Position = 2;
                    new Encoder().Code(uncompMap.BaseStream, outStream, totalSize, totalSize, compressionProgress);
                    outStream.SetLength(outStream.Position);
                }
                else
                    outStream = uncompMap.BaseStream;

                if (encrypted)
                {
                    SymmetricAlgorithm crypto = new RijndaelManaged();

                    MakeKeyAndIV(PASSWORD, SALT, crypto.KeySize, crypto.BlockSize, out byte[] key, out byte[] iv);

                    outStream.Position = 2;
                    int blockSize = 0x1000;
                    int bytesRead = 0;
                    long tempPos;
                    byte[] buffer = new byte[blockSize];
                    using (CryptoStream cryptoStream = new CryptoStream(outStream, crypto.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                    {
                        while (true)
                        {
                            tempPos = outStream.Position;
                            bytesRead = outStream.Read(buffer, 0, blockSize);
                            if (bytesRead <= 0)
                                break;
                            outStream.Position = tempPos;
                            cryptoStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                if (compressed)
                    outStream.Dispose();
            }
        }
        private static void MakeKeyAndIV(
            string password, byte[] salt,
            int key_size_bits, int block_size_bits,
            out byte[] key, out byte[] iv)
        {
            Rfc2898DeriveBytes derive_bytes =
                new Rfc2898DeriveBytes(password, salt, 1000);

            key = derive_bytes.GetBytes(key_size_bits / 8);
            iv = derive_bytes.GetBytes(block_size_bits / 8);
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
            
            size += (node.FlagSize = flagCount.Align(8) / 8);
            return node.CalculatedSize = size.Align(4);
        }
        private static void WriteObject(MemberTreeNode node, ref VoidPtr address, StringTable table)
        {
            if (node.Object == null)
                return;

            //Write flags at the start of the object data block
            int flagIndex = 0;
            VoidPtr flagsAddr = address;
            address += node.FlagSize;

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
                    WriteMember(p, ref address, table, ref flagsAddr, ref flagIndex);
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
                        WriteMember(p, ref address, table, ref flagsAddr, ref flagIndex);
                }
            }
        }
        private static int GetSizeMember(MemberTreeNode node, StringTable table, ref int flagCount)
        {
            object value = node.Object;

            if (TryGetSizeInterface(node, table, out int size))
                return size;

            Type t = node.Info.VariableType;
            
            if (t == typeof(bool))
                ++flagCount;
            else if (t == typeof(string))
            {
                if (value != null)
                    table.Add(value.ToString());
                size += 4;
            }
            else if (t.IsEnum)
            {
                //table.Add(value.ToString());
                size += 4;
            }
            else if (t.IsValueType)
            {
                if (node.Members.Count > 0)
                    size += GetSizeObject(node, table);
                else
                    size += Marshal.SizeOf(value);
            }
            else
                size += GetSizeObject(node, table);

            return size;
        }
        private static void WriteMember(
            MemberTreeNode node,
            ref VoidPtr address,
            StringTable table,
            ref VoidPtr flagsAddr,
            ref int flagIndex)
        {
            object value = node.Object;

            Type t = node.Info.VariableType;

            if (TryWriteInterface(node, ref address, table))
                return;

            if (t == typeof(bool))
            {
                if (flagIndex == 0)
                    flagsAddr.Byte = 0;
                else if (flagIndex == 8)
                {
                    flagsAddr += 1;
                    flagIndex = 0;
                }
                flagsAddr.Byte |= (byte)(1 << (7 - flagIndex++));
            }
            else if (t == typeof(string))
            {
                address.Int = value == null ? -1 : table[value.ToString()];
                address += 4;
            }
            else if (t.IsEnum)
            {
                //string s = value.ToString();
                //address.WriteInt(table[s]);
                address.Int = (int)value;
                address += 4;
            }
            else if (t.IsValueType)
            {
                if (node.Members.Count > 0)
                    WriteObject(node, ref address, table);
                else
                {
                    Marshal.StructureToPtr(value, address, true);
                    address += (Marshal.SizeOf(value));
                }
            }
            else
                WriteObject(node, ref address, table);
        }

        private static bool TryWriteInterface(MemberTreeNode node, ref VoidPtr address, StringTable table)
        {
            if (node.Interface == InterfaceType.IList)
            {
                WriteIList(node, ref address, table);
                return true;
            }
            return false;
        }
        private static bool TryGetSizeInterface(MemberTreeNode node, StringTable table, out int size)
        {
            if (node.Interface == InterfaceType.IList)
            {
                size = GetSizeIList(node, table);
                return true;
            }
            size = 0;
            return false;
        }
        private static int GetSizeIList(MemberTreeNode node, StringTable table)
        {
            MemberTreeNode[] array = node.IListMembers;
            if (array.Length > 0)
            {
                Type elementType = array[0].Object.GetType();
                if (elementType.IsEnum || array[0].Object is string)
                    return 4 + array.Length * 4;
                else if (elementType.IsValueType)
                {
                    int size = 4;
                    //Struct has serialized members within it?
                    //Needs a full element
                    if (array[0].Members.Count > 0)
                    {
                        size += array.Length * 4;
                        foreach (MemberTreeNode o in array)
                            size += GetSizeObject(o, table);
                    }
                    else
                        foreach (MemberTreeNode o in array)
                            size += Marshal.SizeOf(o.Object);
                    return size;
                }
                else
                {
                    int size = 4;
                    foreach (MemberTreeNode o in array)
                        size += GetSizeObject(o, table);
                    return size;
                }
            }
            return 4;
        }
        private static void WriteIList(MemberTreeNode node, ref VoidPtr address, StringTable table)
        {
            MemberTreeNode[] array = node.IListMembers;
            address.Int = array.Length;
            address += 4;
            if (array.Length > 0)
            {
                Type elementType = array[0].Object.GetType();
                if (elementType.IsEnum || array[0].Object is string)
                {
                    for (int i = 0; i < array.Length; ++i)
                    {
                        string s = array[i].ToString();
                        address.Int = table[s];
                        address += 4;
                    }
                }
                else if (elementType.IsValueType)
                {
                    //Struct has serialized members within it?
                    //Needs a full element
                    if (array[0].Members.Count > 0)
                    {
                        VoidPtr arrayBase = address;
                        VoidPtr arrayValue = arrayBase;
                        address += array.Length * 4;
                        foreach (MemberTreeNode o in array)
                        {
                            arrayValue.Int = address - arrayBase;
                            arrayValue += 4;
                            WriteObject(o, ref address, table);
                        }
                    }
                    else
                    {
                        foreach (MemberTreeNode o in array)
                        {
                            Marshal.StructureToPtr(o.Object, address, true);
                            address += Marshal.SizeOf(o.Object);
                        }
                    }
                }
                else
                {
                    foreach (MemberTreeNode o in array)
                        WriteObject(o, ref address, table);
                }
            }
        }
    }
}
