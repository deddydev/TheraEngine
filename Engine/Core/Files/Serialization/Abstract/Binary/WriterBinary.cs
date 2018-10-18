using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer
    {
        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into binary.</param>
        /// <param name="filePath">The path of the file to write.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        /// <param name="endian">The direction to write multi-byte values in.</param>
        /// <param name="encrypted">If true, encrypts the file. The data cannot be decrypted without the password.</param>
        /// <param name="compressed">If true, compresses the file. This will make the file size as small as possible.</param>
        /// <param name="encryptionPassword">If encrypted, this is the password to use to encrypt and decrypt.</param>
        /// <param name="compressionProgress">Handler for compression updates.</param>
        public async Task SerializeBinaryAsync(
            TFileObject fileObject,
            string filePath,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel,
            Endian.EOrder endian,
            bool encrypted,
            bool compressed,
            string encryptionPassword,
            ICodeProgress compressionProgress)
        {
            Format = EProprietaryFileFormat.Binary;
            Writer = new WriterBinary(this, fileObject, filePath, flags, progress, cancel, endian, encrypted, compressed, encryptionPassword, compressionProgress);
            await Writer.WriteTree();

            Engine.PrintLine("Serialized binary file to {0}", filePath);
        }
        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into binary.</param>
        /// <param name="targetDirectoryPath">The path to a directory to write the file in.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        /// <param name="endian">The direction to write multi-byte values in.</param>
        /// <param name="encrypted">If true, encrypts the file. The data cannot be decrypted without the password.</param>
        /// <param name="compressed">If true, compresses the file. This will make the file size as small as possible.</param>
        /// <param name="encryptionPassword">If encrypted, this is the password to use to encrypt and decrypt.</param>
        /// <param name="compressionProgress">Handler for compression updates.</param>
        public async Task SerializeBinaryAsync(
            TFileObject fileObject,
            string targetDirectoryPath,
            string fileName,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel,
            Endian.EOrder endian,
            bool encrypted,
            bool compressed,
            string encryptionPassword,
            ICodeProgress compressionProgress)
        {
            string filePath = TFileObject.GetFilePath(targetDirectoryPath, fileName, Format = EProprietaryFileFormat.Binary, fileObject.GetType());
            Writer = new WriterBinary(this, fileObject, filePath, flags, progress, cancel, endian, encrypted, compressed, encryptionPassword, compressionProgress);
            await Writer.WriteTree();

            Engine.PrintLine("Serialized binary file to {0}", filePath);
        }
        public class WriterBinary : AbstractWriter<BinaryMemberTreeNode>
        {
            public Endian.EOrder Endian { get; }
            public bool Encrypted { get; }
            public bool Compressed { get; }
            ICodeProgress CompressionProgress { get; }
            Rfc2898DeriveBytes EncryptionDeriveBytes { get; }
            BinaryStringTableWriter StringTable { get; set; }
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
                StringTable = new BinaryStringTableWriter();

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

                Memory.Endian.SerializerOrder = Endian;

                int dataSize = RootNode.CalculatedSize;
                int stringSize = StringTable.TotalLength.Align(4);
                StringOffsetSize = StringTable.GetSmallestRepSize(stringSize);
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
                        hdr->WriteMagic();
                        hdr->_stringTableLength = stringSize;
                        hdr->Endian = Endian;
                        hdr->Encrypted = Encrypted;
                        hdr->Compressed = Compressed;
                        
                        //SHA256Managed SHhash = new SHA256Managed();
                        //byte[] integrityHash = SHhash.ComputeHash(uncompMap.BaseStream);
                        //hdr->WriteHash(integrityHash);

                        StringTable.WriteTable(hdr->Strings);

                        VoidPtr addr = hdr->Data;
                        WriteObject(RootNode, ref addr);
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
                object value = node.Object;
                Type objType = node.ObjectType;
                object defaultValue = node.DefaultConstructedObject;
                
                Type nulledType = Nullable.GetUnderlyingType(objType);
                bool nullable = nulledType != null;
                if (nullable)
                    objType = nulledType;

                if (value == defaultValue)
                {
                    address.Byte = (byte)EBinaryObjectFlags.IsDefault;
                    address += 1;
                    return;
                }
                else
                {
                    EBinaryObjectFlags flags = EBinaryObjectFlags.IsNotDefault;

                    //TODO: generate assembly list and namespace folder tree in another section to save space
                    //This will do for now
                    if (node.IsDerivedType)
                    {
                        flags |= EBinaryObjectFlags.IsDerived;
                        WriteString(node.ObjectType.AssemblyQualifiedName, ref address);
                    }

                    address.Byte = (byte)flags;
                    address += 1;
                }
                
                //First, handle built-in primitive types
                switch (objType.Name)
                {
                    case nameof(Boolean):
                        address.Byte = (byte)((bool)value ? 1 : 0);
                        address += 1;
                        break;
                    case nameof(SByte):
                        address.SByte = (sbyte)value;
                        address += 1;
                        break;
                    case nameof(Byte):
                        address.Byte = (byte)value;
                        address += 1;
                        break;
                    case nameof(Char):
                        address.Char = (char)value;
                        address += sizeof(char);
                        break;
                    case nameof(Int16):
                        address.Short = (short)value;
                        address += sizeof(short);
                        break;
                    case nameof(UInt16):
                        address.UShort = (ushort)value;
                        address += sizeof(ushort);
                        break;
                    case nameof(Int32):
                        address.Int = (int)value;
                        address += sizeof(int);
                        break;
                    case nameof(UInt32):
                        address.UInt = (uint)value;
                        address += sizeof(uint);
                        break;
                    case nameof(Int64):
                        address.Long = (long)value;
                        address += sizeof(long);
                        break;
                    case nameof(UInt64):
                        address.ULong = (ulong)value;
                        address += sizeof(ulong);
                        break;
                    case nameof(Single):
                        address.Float = (float)value;
                        address += sizeof(float);
                        break;
                    case nameof(Double):
                        address.Double = (double)value;
                        address += sizeof(double);
                        break;
                    case nameof(Decimal):
                        address.Decimal = (decimal)value;
                        address += sizeof(decimal);
                        break;
                    case nameof(String):
                        WriteString(value?.ToString(), ref address);
                        break;
                    default: //Now do more specific work
                        //Write manually?
                        if (value is TFileObject fobj)
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
                        else if (objType.IsEnum)
                            WriteEnum(value, ref address);
                        else if (objType.IsValueType && node.Children.Count == 0)
                        {
                            int size = Marshal.SizeOf(value);
                            Marshal.StructureToPtr(value, address, true);
                            address += size;
                        }
                        else if (objType.GetInterface(nameof(IByteArrayParsable)) != null)
                        {
                            byte[] bytes = node.ParsableBytes;
                            address.Int = bytes.Length;
                            address += sizeof(int);
                            if (bytes != null)
                                foreach (byte b in bytes)
                                {
                                    address.Byte = b;
                                    address += 1;
                                }
                        }
                        else if (objType.GetInterface(nameof(IStringParsable)) != null)
                            WriteString(node.ParsableString, ref address);
                        else
                            foreach (BinaryMemberTreeNode childNode in node.Children)
                                WriteObject(childNode, ref address);
                        break;
                }
            }
            internal int GetSizeObject(BinaryMemberTreeNode node)
            {
                int size = 0;
                object value = node.Object;
                Type objType = node.ObjectType;
                object defaultValue = node.DefaultConstructedObject;
                
                Type nulledType = Nullable.GetUnderlyingType(objType);
                bool nullable = nulledType != null;
                if (nullable)
                    objType = nulledType;

                ++size;
                if (value == defaultValue)
                    return size;
                else if (node.IsDerivedType)
                {
                    StringTable.Add(node.ObjectType.AssemblyQualifiedName);
                    size += StringOffsetSize;
                }
                
                switch (objType.Name)
                {
                    case nameof(Boolean):
                    case nameof(SByte):
                    case nameof(Byte): size += 1; break;
                    case nameof(Char): size += sizeof(char); break;
                    case nameof(Int16): size += sizeof(short); break;
                    case nameof(UInt16): size += sizeof(ushort); break;
                    case nameof(Int32): size += sizeof(int); break;
                    case nameof(UInt32): size += sizeof(uint); break;
                    case nameof(Int64): size += sizeof(long); break;
                    case nameof(UInt64): size += sizeof(ulong); break;
                    case nameof(Single): size += sizeof(float); break;
                    case nameof(Double): size += sizeof(double); break;
                    case nameof(Decimal): size += sizeof(decimal); break;
                    case nameof(String):
                        StringTable.Add((string)value);
                        size += StringOffsetSize;
                        break;
                    default:
                        if (value is TFileObject fobj)
                        {
                            //Write manually?
                            FileExt ext = TFileObject.GetFileExtension(node.ObjectType);
                            bool serConfig = ext.ManualBinConfigSerialize && Flags.HasFlag(ESerializeFlags.SerializeConfig);
                            bool serState = ext.ManualBinStateSerialize && Flags.HasFlag(ESerializeFlags.SerializeState);
                            if (serConfig || serState)
                            {
                                int manualSize = fobj.ManualGetSizeBinary(StringTable, Flags);
                                size += manualSize;
                                return size;
                            }
                        }
                        else if (objType.IsEnum)
                            size += GetSizeEnum(value);
                        else if (objType.IsValueType && node.Children.Count == 0)
                            size += Marshal.SizeOf(value);
                        else if (objType.GetInterface(nameof(IByteArrayParsable)) != null)
                        {
                            byte[] bytes = ((IByteArrayParsable)value).WriteToBytes();
                            node.ParsableBytes = bytes;
                            size += 4; //Length
                            if (bytes != null)
                                size += bytes.Length;
                        }
                        else if (objType.GetInterface(nameof(IStringParsable)) != null)
                        {
                            string str = ((IStringParsable)value).WriteToString();
                            node.ParsableString = str;
                            StringTable.Add(str);
                            size += StringOffsetSize;
                        }
                        else
                            foreach (BinaryMemberTreeNode childNode in node.Children)
                                size += GetSizeObject(childNode);
                        break;
                }

                return size;
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
                        address += sizeof(sbyte);
                        break;
                    case TypeCode.Byte:
                        address.Byte = (byte)value;
                        address += sizeof(byte);
                        break;
                    case TypeCode.Int16:
                        address.Short = (short)value;
                        address += sizeof(short);
                        break;
                    case TypeCode.UInt16:
                        address.UShort = (ushort)value;
                        address += sizeof(ushort);
                        break;
                    default:
                    case TypeCode.Int32:
                        address.Int = (int)value;
                        address += sizeof(int);
                        break;
                    case TypeCode.UInt32:
                        address.UInt = (uint)value;
                        address += sizeof(uint);
                        break;
                    case TypeCode.Int64:
                        address.Long = (long)value;
                        address += sizeof(long);
                        break;
                    case TypeCode.UInt64:
                        address.ULong = (ulong)value;
                        address += sizeof(ulong);
                        break;
                }
            }
            private int GetSizeEnum(object value)
            {
                Enum enumValue = (Enum)value;
                TypeCode typeCode = enumValue.GetTypeCode();
                switch (typeCode)
                {
                    case TypeCode.SByte: return sizeof(sbyte);
                    case TypeCode.Byte: return sizeof(byte);
                    case TypeCode.Int16: return sizeof(short);
                    case TypeCode.UInt16: return sizeof(ushort);
                    default:
                    case TypeCode.Int32: return sizeof(int);
                    case TypeCode.UInt32: return sizeof(uint);
                    case TypeCode.Int64: return sizeof(long);
                    case TypeCode.UInt64: return sizeof(ulong);
                }
            }
            //private void WriteBoolean(bool value, byte[] flagBytes, ref int flagIndex)
            //{
            //    int bitIndex = flagIndex & 7;
            //    int byteIndex = flagIndex >> 3;

            //    if (bitIndex == 0)
            //        flagBytes[byteIndex] = 0;

            //    int data = 1 << (7 - bitIndex);
            //    if (value)
            //        flagBytes[byteIndex] |= (byte)data;
            //    else
            //        flagBytes[byteIndex] &= (byte)~data;

            //    ++flagIndex;
            //}

            public override BinaryMemberTreeNode CreateNode(BinaryMemberTreeNode parent, MemberInfo memberInfo)
                => new BinaryMemberTreeNode(parent, memberInfo, this);
            public override BinaryMemberTreeNode CreateNode(object root)
                => new BinaryMemberTreeNode(root, this);

            public unsafe sealed class BinaryStringTableWriter
            {
                public BinaryStringTableWriter()
                {
                    _table = new SortedList<string, int>(StringComparer.Ordinal);
                    Encoding = Encoding.Unicode;
                }
                public BinaryStringTableWriter(StringComparer comparer)
                {
                    _table = new SortedList<string, int>(comparer);
                    Encoding = Encoding.Unicode;
                }
                public BinaryStringTableWriter(Encoding encoding)
                {
                    _table = new SortedList<string, int>(StringComparer.Ordinal);
                    Encoding = encoding;
                }
                public BinaryStringTableWriter(StringComparer comparer, Encoding encoding)
                {
                    _table = new SortedList<string, int>(comparer);
                    Encoding = encoding;
                }

                private SortedList<string, int> _table;

                public Encoding Encoding { get; private set; } = null;
                public int TotalLength { get; private set; } = 0;
                public int StringLengthSize { get; private set; }

                public void Add(string s)
                {
                    if ((!string.IsNullOrEmpty(s)) && (!_table.ContainsKey(s)))
                    {
                        _table.Add(s, 0);
                        int lengthSize = GetSmallestRepSize(s.Length);
                        StringLengthSize = Math.Max(StringLengthSize, lengthSize);
                        TotalLength += Encoding.GetMaxByteCount(s.Length);
                    }
                }
                public void AddRange(IEnumerable<string> values)
                {
                    foreach (string s in values)
                        Add(s);
                }
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
                public void AddingFinished()
                {
                    TotalLength += FileCommonHeader.StringTableHeader.Size + StringLengthSize * _table.Count;
                }
                public void WriteTable(VoidPtr baseAddress)
                {
                    FileCommonHeader.StringTableHeader* hdr = (FileCommonHeader.StringTableHeader*)baseAddress;
                    baseAddress += FileCommonHeader.StringTableHeader.Size;
                    VoidPtr currentAddress = baseAddress;
                    for (int i = 0; i < _table.Count; i++)
                    {
                        string s = _table.Keys[i];
                        _table[s] = currentAddress - baseAddress;
                        int length = s.Write(currentAddress + StringLengthSize, Encoding);
                        switch (StringLengthSize)
                        {
                            case 1:
                                currentAddress.Byte = (byte)length;
                                break;
                            case 2:
                                currentAddress.UShort = (ushort)length;
                                break;
                            case 3:
                                currentAddress.UInt24 = (uint)length;
                                break;
                            case 4:
                                currentAddress.Int = length;
                                break;
                        }
                        currentAddress += StringLengthSize + length;
                    }
                }
                public int GetSmallestRepSize(int length)
                {
                    int len = length;
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
        }
    }
}
