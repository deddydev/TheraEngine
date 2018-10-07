using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SevenZip.Compression.LZMA;
using SevenZip;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FileCommonHeader
    {
        public const int Size = 0x30;

        public byte _encrypted;
        public byte _compressed;
        public bushort _endian;
        public bint _stringTableLength;
        public bint _fileLength;
        public bint _typeNameStringOffset;
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
        public static bool SerializeRaw(
            object obj,
            string filePath,
            Endian.EOrder order,
            out byte[] integrityHash,
            ESerializeFlags flags)
        {
            return Serialize(
                obj,
                filePath,
                order,
                false,
                false,
                null, 
                out byte[] encryptionSalt,
                out integrityHash, 
                null,
                flags);
        }
        public static bool SerializeCompressed(
            object obj,
            string filePath,
            Endian.EOrder order,
            out byte[] integrityHash,
            ICodeProgress compressionProgress,
            ESerializeFlags flags)
        {
            return Serialize(
                obj,
                filePath,
                order,
                false,
                true,
                null,
                out byte[] encryptionSalt,
                out integrityHash,
                compressionProgress,
                flags);
        }
        public static bool SerializeEncrypted(
            object obj,
            string filePath,
            Endian.EOrder order,
            out byte[] integrityHash,
            out byte[] encryptionSalt,
            string encryptionPassword,
            ESerializeFlags flags)
        {
            return Serialize(
                obj,
                filePath,
                order,
                true,
                false,
                encryptionPassword,
                out encryptionSalt,
                out integrityHash,
                null, 
                flags);
        }

        /// <summary>
        /// Writes the given object to the path as binary.
        /// </summary>
        public static bool Serialize(
            object obj,
            string filePath,
            Endian.EOrder order,
            bool encrypted,
            bool compressed,
            string encryptionPassword,
            out byte[] encryptionSalt,
            out byte[] integrityHash,
            ICodeProgress compressionProgress,
            ESerializeFlags flags)
        {
            encryptionSalt = new byte[8];
            Endian.Order = order;
            try
            {
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
                    integrityHash = SHhash.ComputeHash(uncompMap.BaseStream);
                    for (int i = 0; i < 0x20; ++i)
                        hdr->_hash[i] = integrityHash[i];
                    uncompMap.BaseStream.Position = 0;

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
                        outStream.Position = 0;
                        new Encoder().Code(uncompMap.BaseStream, outStream, totalSize, totalSize, compressionProgress);
                        outStream.SetLength(outStream.Position);
                    }
                    else
                        outStream = uncompMap.BaseStream;

                    if (encrypted)
                    {
                        SymmetricAlgorithm crypto = new RijndaelManaged();

                        new Random().NextBytes(encryptionSalt);

                        MakeKeyAndIV(encryptionPassword, encryptionSalt, crypto.KeySize, crypto.BlockSize, out byte[] key, out byte[] iv);

                        outStream.Position = 0;
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
                return true;
            }
            catch (Exception ex)
            {
                integrityHash = new byte[0x20];
                Engine.PrintLine("Error serializing binary file to " + filePath + ".\n\nException:\n" + ex.ToString());
                return false;
            }
        }
        private static void MakeKeyAndIV(
            string password, 
            byte[] salt,
            int keySizeBits,
            int blockSizeBits,
            out byte[] key,
            out byte[] iv)
        {
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt, 1000);

            key = deriveBytes.GetBytes(keySizeBits / 8);
            iv = deriveBytes.GetBytes(blockSizeBits / 8);
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
            byte[] flagBytes = new byte[node.FlagSize];

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
                    WriteMember(p, ref address, table, flagBytes, ref flagIndex);
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
                        WriteMember(p, ref address, table, flagBytes, ref flagIndex);
                }
            }

            byte* flagData = (byte*)address;
            foreach (byte b in flagBytes)
                *flagData++ = b;
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
            byte[] flagBytes,
            ref int flagIndex)
        {
            object value = node.Object;

            Type t = node.Info.VariableType;

            if (TryWriteInterface(node, ref address, table))
                return;

            if (t == typeof(bool))
            {
                int bitIndex = flagIndex & 7;
                int byteIndex = flagIndex >> 3;

                if (bitIndex == 0)
                    flagBytes[byteIndex] = 0;
                
                int data = 1 << (7 - bitIndex);
                if ((bool)value)
                    flagBytes[byteIndex] |= (byte)data;
                else
                    flagBytes[byteIndex] &= (byte)~data;

                ++flagIndex;
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
