using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer
    {
        public class WriterBinary : AbstractWriter<BinaryMemberTreeNode>
        {
            public Endian.EOrder Endian { get; }
            public bool Encrypted { get; }
            public bool Compressed { get; }
            ICodeProgress CompressionProgress { get; }
            Rfc2898DeriveBytes EncryptionDeriveBytes { get; }
            BinaryStringTable StringTable { get; set; }
            public int StringOffsetSize { get; private set; }

            public WriterBinary(
                TSerializer owner,
                TFileObject rootFileObject,
                string filePath,
                ESerializeFlags flags,
                IProgress<float> progress,
                CancellationToken cancel,
                Endian.EOrder endian,
                bool encrypted,
                bool compressed,
                string encryptionPassword,
                ICodeProgress compressionProgress)
                : base(owner, rootFileObject, filePath, flags, progress, cancel)
            {
                Endian = endian;
                Encrypted = encrypted;
                Compressed = compressed;
                CompressionProgress = compressionProgress;
                StringTable = new BinaryStringTable();

                if (Encrypted)
                {
                    Random r = new Random();
                    byte[] salt = new byte[8];
                    r.NextBytes(salt);
                    EncryptionDeriveBytes = new Rfc2898DeriveBytes(encryptionPassword, salt, 1000);
                }
            }
            
            protected internal override async Task WriteTree()
            {
                await RootNode.CollectSerializedMembers();
                BinaryMemberTreeNode rootNode = (BinaryMemberTreeNode)RootNode;

                Memory.Endian.Order = Endian;

                int dataSize = rootNode.CalculatedSize;
                int stringSize = (int)StringTable.TotalLength.Align(4);
                StringOffsetSize = StringTable.GetOffsetSize();
                int totalSize = FileCommonHeader.Size + stringSize + dataSize;

                FileMap uncompMap;
                if (Compressed)
                    uncompMap = FileMap.FromTempFile(totalSize);
                else
                    uncompMap = FileMap.FromFile(FilePath, FileMapProtect.ReadWrite, 0, totalSize);

                using (uncompMap)
                {
                    unsafe
                    {
                        FileCommonHeader* hdr = (FileCommonHeader*)uncompMap.Address;
                        hdr->_stringTableLength = stringSize;
                        hdr->Endian = Endian;
                        hdr->Encrypted = Encrypted;
                        hdr->Compressed = Compressed;

                        //SHA256Managed SHhash = new SHA256Managed();
                        //byte[] integrityHash = SHhash.ComputeHash(uncompMap.BaseStream);
                        //hdr->WriteHash(integrityHash);

                        StringTable.WriteTable(hdr->Strings);

                        VoidPtr addr = hdr->Data;

                        //Write the root node to the main address.
                        //This will write all child nodes within this node as well.
                        WriteObject(rootNode, ref addr);
                    }

                    uncompMap.BaseStream.Position = 0;

                    FileStream outStream;

                    //Compres first, then encrypt
                    //This is because compression works best on the original patterned data

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
                        byte[] key = EncryptionDeriveBytes.GetBytes(crypto.KeySize / 8);
                        byte[] iv = EncryptionDeriveBytes.GetBytes(crypto.BlockSize / 8);

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
            private unsafe void WriteObject(BinaryMemberTreeNode node, ref VoidPtr address)
            {
                if (node.Object == null)
                    return;

                if (node.Object is TFileObject fobj)
                {
                    FileExt ext = TFileObject.GetFileExtension(node.ObjectType);
                    bool serConfig = ext.ManualBinConfigSerialize && Flags.HasFlag(ESerializeFlags.SerializeConfig);
                    bool serState = ext.ManualBinStateSerialize && Flags.HasFlag(ESerializeFlags.SerializeState);
                    if (serConfig || serState)
                    {
                        int size = node.CalculatedSize;
                        fobj.ManualWriteBinary(address, size, StringTable, Flags);
                        address += size;
                        return;
                    }
                }

                //Write flags at the start of the object data block
                int flagIndex = 0;
                byte[] flagBytes = new byte[node.FlagSize];
                
                foreach (BinaryMemberTreeNode childNode in node.ChildMembers)
                    WriteMember(childNode, ref address, flagBytes, ref flagIndex);
                
                byte* flagData = (byte*)address;
                foreach (byte b in flagBytes)
                    *flagData++ = b;
            }
            private void WriteMember(
                BinaryMemberTreeNode node,
                ref VoidPtr address,
                byte[] flagBytes,
                ref int flagIndex)
            {
                object value = node.Object;
                Type objType = node.ObjectType;
                switch (objType.Name)
                {
                    case "Boolean":
                        WriteBoolean((bool)value, flagBytes, ref flagIndex);
                        break;
                    case "SByte":
                        break;
                    case "Byte":
                        break;
                    case "Char":
                        break;
                    case "Int16":
                        break;
                    case "UInt16":
                        break;
                    case "Int32":
                        break;
                    case "UInt32":
                        break;
                    case "Int64":
                        break;
                    case "UInt64":
                        break;
                    case "Single":
                        break;
                    case "Double":
                        break;
                    case "Decimal":
                        break;
                    case "String":
                        WriteString(value.ToString(), ref address);
                        break;
                    default:
                        if (objType.IsEnum)
                            WriteEnum(value, ref address);
                        else if (objType.IsValueType)
                        {
                            if (node.ChildMembers.Count > 0)
                                WriteObject(node, ref address);
                            else
                            {
                                int size = Marshal.SizeOf(value);
                                Marshal.StructureToPtr(value, address, true);
                                address += size;
                            }
                        }
                        else
                            WriteObject(node, ref address);
                        break;
                }
            }

            private void WriteString(string value, ref VoidPtr address)
            {
                int offset = StringTable[value] + 1;
                switch (StringOffsetSize)
                {
                    case 1:
                        address.Byte = (byte)offset;
                        break;
                    case 2:
                        address.UShort = (ushort)offset;
                        break;
                    case 3:
                        address.UInt24 = (uint)offset;
                        break;
                    default:
                    case 4:
                        address.Int = offset;
                        break;
                }
                address += StringOffsetSize;
            }

            private void WriteEnum(object value, ref VoidPtr address)
            {
                Enum enumValue = (Enum)value;
                TypeCode typeCode = enumValue.GetTypeCode();
                switch (typeCode)
                {
                    case TypeCode.SByte:
                        address.SByte = (sbyte)value;
                        address += 1;
                        break;
                    case TypeCode.Byte:
                        address.Byte = (byte)value;
                        address += 1;
                        break;
                    case TypeCode.Int16:
                        address.Short = (short)value;
                        address += 2;
                        break;
                    case TypeCode.UInt16:
                        address.UShort = (ushort)value;
                        address += 2;
                        break;
                    case TypeCode.Int32:
                        address.Int = (int)value;
                        address += 4;
                        break;
                    case TypeCode.UInt32:
                        address.UInt = (uint)value;
                        address += 4;
                        break;
                    case TypeCode.Int64:
                        address.Long = (long)value;
                        address += 8;
                        break;
                    case TypeCode.UInt64:
                        address.ULong = (ulong)value;
                        address += 8;
                        break;
                }
            }

            private void WriteBoolean(bool value, byte[] flagBytes, ref int flagIndex)
            {
                int bitIndex = flagIndex & 7;
                int byteIndex = flagIndex >> 3;

                if (bitIndex == 0)
                    flagBytes[byteIndex] = 0;

                int data = 1 << (7 - bitIndex);
                if (value)
                    flagBytes[byteIndex] |= (byte)data;
                else
                    flagBytes[byteIndex] &= (byte)~data;

                ++flagIndex;
            }

            private int GetSizeMember(MemberTreeNode node, ref int flagCount)
            {
                object value = node.Object;

                Type t = node.ObjectType;

                if (t == typeof(bool))
                    ++flagCount;
                else if (t == typeof(string))
                {
                    if (value != null)
                        StringTable.Add(value.ToString());
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

            public override BinaryMemberTreeNode CreateNode(BinaryMemberTreeNode parent, MemberInfo memberInfo)
                => new BinaryMemberTreeNode(parent, memberInfo, this);
            public override MemberTreeNode CreateNode(object root)
                => new BinaryMemberTreeNode(root, this);

            public unsafe class BinaryStringTable
            {
                private SortedList<string, int> _table = new SortedList<string, int>(StringComparer.Ordinal);

                public int TotalLength { get; private set; } = 0;

                public void Add(string s)
                {
                    if ((!string.IsNullOrEmpty(s)) && (!_table.ContainsKey(s)))
                    {
                        _table.Add(s, 0);
                        TotalLength += s.Length + 1;
                    }
                }
                public void AddRange(IEnumerable<string> values)
                {
                    foreach (string s in values)
                        Add(s);
                }
                //public int GetTotalSize()
                //{
                //    int len = 0;
                //    foreach (string s in _table.Keys)
                //        len += s.Length + 1;
                //    return len.Align(4);
                //}
                public void Clear()
                {
                    _table.Clear();
                    TotalLength = 0;
                }
                public int this[string s]
                {
                    get
                    {
                        if ((!string.IsNullOrEmpty(s)) && (_table.ContainsKey(s)))
                            return _table[s];
                        return -1;
                    }
                }
                public void WriteTable(VoidPtr baseAddress)
                {
                    VoidPtr currentAddress = baseAddress;
                    for (int i = 0; i < _table.Count; i++)
                    {
                        string s = _table.Keys[i];
                        _table[s] = currentAddress - baseAddress;
                        s.Write(ref currentAddress, true);
                    }
                }
                public int GetOffsetSize()
                {
                    int len = TotalLength.Align(4);
                    if (len < 0xFF)
                        return 1;
                    if (len < 0xFF_FF)
                        return 2;
                    if (len < 0xFF_FF_FF)
                        return 3;
                    //if (len < int.MaxValue)
                        return 4;
                    //if (len < 0xFF_FF_FF_FF_FFul)
                    //    return 5;
                    //if (len < 0xFF_FF_FF_FF_FF_FFul)
                    //    return 6;
                    //if (len < 0xFF_FF_FF_FF_FF_FF_FFul)
                    //    return 7;
                    ////if (len < 0xFF_FF_FF_FF_FF_FF_FF_FFul)
                    //return 8;
                }
            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct FileCommonHeader
            {
                public const int Size = 0x10;
                public const string Magic = "THRA";

                public bint _magic;
                public Bin8 _flags;
                public byte _pad1;
                public bushort _pad2;
                public bint _stringTableLength;
                public bint _typeNameStringOffset;
                //public fixed byte _hash[0x20];

                public void WriteMagic() => Magic.Write(_magic.Address, false);
                //public void WriteHash(byte[] integrityHash)
                //{
                //    for (int i = 0; i < 0x20; ++i)
                //        _hash[i] = integrityHash[i];
                //}

                public bool Encrypted
                {
                    get => _flags[0];
                    set => _flags[0] = value;
                }
                public bool Compressed
                {
                    get => _flags[1];
                    set => _flags[1] = value;
                }
                public Endian.EOrder Endian
                {
                    get => (Endian.EOrder)_flags[2, 2];
                    set => _flags[2, 2] = (byte)value;
                }
                public VoidPtr Strings => Address + Size;
                public VoidPtr Data => Strings + _stringTableLength;
                public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
            }
        }
    }
}
