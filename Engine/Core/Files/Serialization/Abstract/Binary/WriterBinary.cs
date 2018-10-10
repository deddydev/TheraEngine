using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer
    {
        public class WriterBinary : AbstractWriter
        {
            Endian.EOrder Order { get; }
            bool Encrypted { get; }
            bool Compressed { get; }
            string EncryptionPassword { get; }
            ICodeProgress CompressionProgress { get; }

            public WriterBinary(
                TSerializer owner,
                TFileObject rootFileObject,
                string filePath,
                ESerializeFlags flags,
                IProgress<float> progress,
                CancellationToken cancel,
                Endian.EOrder order,
                bool encrypted,
                bool compressed,
                string encryptionPassword,
                ICodeProgress compressionProgress)
                : base(owner, rootFileObject, filePath, flags, progress, cancel)
            {
                Order = order;
                Encrypted = encrypted;
                Compressed = compressed;
                EncryptionPassword = encryptionPassword;
                CompressionProgress = compressionProgress;
            }

            public static void MakeKeyAndIV(
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

            public override unsafe async Task WriteTree()
            {
                Endian.Order = Order;

                BinaryStringTable table = new BinaryStringTable();
                int dataSize = RootNode.GetSize(table);
                int stringSize = table.GetTotalSize();
                int totalSize = FileCommonHeader.Size + stringSize + dataSize;

                using (FileMap uncompMap = Compressed ? FileMap.FromTempFile(totalSize) :
                    FileMap.FromFile(FilePath, FileMapProtect.ReadWrite, 0, totalSize))
                {
                    FileCommonHeader* hdr = (FileCommonHeader*)uncompMap.Address;
                    hdr->_fileLength = totalSize;
                    hdr->_stringTableLength = stringSize;
                    hdr->Endian = Order;
                    hdr->Encrypted = Encrypted;
                    hdr->Compressed = Compressed;

                    table.WriteTable(hdr);

                    VoidPtr addr = hdr->Data;

                    //Write the root node to the main address.
                    //This will write all child nodes within this node as well.
                    RootNode.ObjectWriter.Write(ref addr, table);

                    SHA256Managed SHhash = new SHA256Managed();
                    byte[] integrityHash = SHhash.ComputeHash(uncompMap.BaseStream);
                    for (int i = 0; i < 0x20; ++i)
                        hdr->_hash[i] = integrityHash[i];
                    uncompMap.BaseStream.Position = 0;

                    FileStream outStream;

                    if (Compressed)
                    {
                        outStream = new FileStream(FilePath,
                            FileMode.OpenOrCreate,
                            FileAccess.ReadWrite,
                            FileShare.ReadWrite,
                            8,
                            FileOptions.RandomAccess);
                        outStream.SetLength(totalSize);
                        outStream.Position = 0;
                        SevenZip.Compression.LZMA.Encoder e = new SevenZip.Compression.LZMA.Encoder();
                        e.Code(uncompMap.BaseStream, outStream, totalSize, totalSize, CompressionProgress);
                        outStream.SetLength(outStream.Position);
                    }
                    else
                        outStream = uncompMap.BaseStream;

                    if (Encrypted)
                    {
                        SymmetricAlgorithm crypto = new RijndaelManaged();

                        Random r = new Random();
                        byte[] salt = new byte[8];
                        r.NextBytes(salt);

                        MakeKeyAndIV(EncryptionPassword, salt, crypto.KeySize, crypto.BlockSize, out byte[] key, out byte[] iv);

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

                    if (Compressed)
                        outStream.Dispose();
                }
            }
            public override Task Finish()
            {
                throw new NotImplementedException();
            }
            public override Task WriteAttributeStringAsync(string name, string value)
            {
                throw new NotImplementedException();
            }
            public override Task WriteElementStringAsync(string name, string value)
            {
                throw new NotImplementedException();
            }
            public override Task WriteEndElementAsync()
            {
                throw new NotImplementedException();
            }
            public override Task WriteStartElementAsync(string name)
            {
                throw new NotImplementedException();
            }
            protected override void OnReportProgress()
            {
                throw new NotImplementedException();
            }
            protected override Task WriteAsync(MemberTreeNode node)
            {
                throw new NotImplementedException();
            }
            public override int GetSize(MethodInfo[] customMethods, ref int flagCount, BinaryStringTable table)
            {
                Type t = Info.VariableType;
                if (t.IsValueType)
                    return Marshal.SizeOf(t);

                int size = 0;

                foreach (MemberTreeNode p in Children)
                    size += GetSizeMember(p, customMethods, ref flagCount, table);

                foreach (var grouping in CategorizedChildren)
                    foreach (MemberTreeNode p in grouping)
                        size += GetSizeMember(p, customMethods, ref flagCount, table);

                return size;
            }
            public override int GetSizeMember(MemberTreeNode node, MethodInfo[] customMethods, ref int flagCount, BinaryStringTable table)
            {
                object value = node.Object;

                MethodInfo customMethod = customMethods.FirstOrDefault(x => string.Equals(node.MemberInfo.Name, x.GetCustomAttribute<CustomBinarySerializeSizeMethod>().Name));

                if (customMethod != null)
                    return (int)customMethod.Invoke(value, new object[] { table });

                if (TryGetSize(node, table, out int size))
                    return size;

                Type t = node.MemberInfo.VariableType;

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
                        size += node.GetSize(table);
                    else
                        size += Marshal.SizeOf(value);
                }
                else
                    size += node.GetSize(table);

                return size;
            }
            public override bool Write(ref VoidPtr address, BinaryStringTable table)
            {
                throw new NotImplementedException();
            }
            internal protected override MemberTreeNode CreateNode(object obj, VarInfo memberInfo)
            {
                return new BinaryMemberTreeNode(obj, memberInfo, this);
            }
            internal protected override MemberTreeNode CreateNode(object rootObject)
            {
                return new BinaryMemberTreeNode(rootObject, this);
            }
            public unsafe class BinaryStringTable
            {
                SortedList<string, int> _table = new SortedList<string, int>(StringComparer.Ordinal);

                public void Add(string s)
                {
                    if ((!string.IsNullOrEmpty(s)) && (!_table.ContainsKey(s)))
                        _table.Add(s, 0);
                }
                public void AddRange(IEnumerable<string> values)
                {
                    foreach (string s in values)
                        Add(s);
                }
                public int GetTotalSize()
                {
                    int len = 0;
                    foreach (string s in _table.Keys)
                        len += s.Length + 1;
                    return len.Align(4);
                }
                public void Clear() => _table.Clear();
                public int this[string s]
                {
                    get
                    {
                        if ((!string.IsNullOrEmpty(s)) && (_table.ContainsKey(s)))
                            return _table[s];
                        return _table.Values[0];
                    }
                }
                public void WriteTable(FileCommonHeader* address)
                {
                    VoidPtr baseAddress = address->Strings;
                    VoidPtr currentAddress = baseAddress;
                    for (int i = 0; i < _table.Count; i++)
                    {
                        string s = _table.Keys[i];
                        _table[s] = currentAddress - baseAddress;
                        s.Write(ref currentAddress);
                    }
                }
            }
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
        }
    }
}
