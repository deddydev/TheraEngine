using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [Flags]
    public enum EBinaryObjectFlags : byte
    {
        IsDefault       = 0b0000_0000,
        IsNotDefault    = 0b0000_0001,
        IsDerived       = 0b0000_0010,
    }
    public partial class TDeserializer : TBaseSerializer
    {
        /// <summary>
        /// Reads the file at <paramref name="filePath"/> as a binary file.
        /// </summary>
        /// <param name="filePath">The path of the file to write.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        /// <param name="encryptionPassword">If encrypted, this is the password to use to decrypt.</param>
        public async Task<TFileObject> DeserializeBinaryAsync(
            string filePath,
            IProgress<float> progress,
            CancellationToken cancel,
            string encryptionPassword)
        {
            Format = EProprietaryFileFormat.Binary;
            Type fileType = SerializationCommon.DetermineType(filePath);
            TFileObject rootFileObject = SerializationCommon.CreateObject(fileType) as TFileObject;
            Reader = new ReaderBinary(this, rootFileObject, filePath, progress, cancel, encryptionPassword);
            await Reader.ReadTree();

            Engine.PrintLine("Deserialized binary file at {0}", filePath);
            return Reader.RootFileObject;
        }
        public class ReaderBinary : AbstractReader<BinaryMemberTreeNode>
        {
            public Endian.EOrder Endian { get; }
            public bool Encrypted { get; }
            public bool Compressed { get; }
            Rfc2898DeriveBytes EncryptionDeriveBytes { get; }
            BinaryStringTableReader StringTable { get; set; }
            public int StringOffsetSize { get; private set; }

            public ReaderBinary(
                TDeserializer owner,
                TFileObject rootFileObject,
                string filePath,
                IProgress<float> progress,
                CancellationToken cancel,
                string encryptionPassword)
                : base(owner, rootFileObject, filePath, progress, cancel)
            {
                if (Encrypted)
                {
                    Random r = new Random();
                    byte[] salt = new byte[8];
                    r.NextBytes(salt);
                    EncryptionDeriveBytes = new Rfc2898DeriveBytes(encryptionPassword, salt, 1000);
                }
            }

            protected internal override async Task ReadTree()
            {
                await RootNode.CollectSerializedMembers();

                Memory.Endian.SerializerOrder = Endian;

                StringTable = new BinaryStringTableReader();

                int dataSize = RootNode.CalculatedSize;
                int stringSize = StringTable.TotalLength.Align(4);
                StringOffsetSize = StringTable.GetSmallestRepSize(stringSize);
                int totalSize = FileCommonHeader.Size + stringSize + dataSize;

                FileMap uncompMap = FileMap.FromFile(FilePath, FileMapProtect.Read);
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

                        StringTable = new BinaryStringTableReader(hdr->Strings);

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
                        using (CryptoStream cryptoStream = new CryptoStream(outStream, crypto.CreateDecryptor(key, iv), CryptoStreamMode.Write))
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
            private unsafe bool ReadObject(ref VoidPtr address, Type memberType, out BinaryMemberTreeNode node)
            {
                node = null;

                //object value = node.Object;
                //Type objType = node.ObjectType;
                //object defaultValue = node.DefaultConstructedObject;
                
                EBinaryObjectFlags flags = (EBinaryObjectFlags)address.ReadByte();
                if ((flags & EBinaryObjectFlags.IsDefault) != 0)
                    return false;
                else
                {
                    bool derived = (flags & EBinaryObjectFlags.IsDerived) != 0;
                    if (derived)
                    {
                        string assemblyTypeName = ReadString(ref address);
                        Type objType = Type.GetType(assemblyTypeName, false, false);
                        if (objType != null)
                            memberType = objType;
                    }
                }

                Type nulledType = Nullable.GetUnderlyingType(memberType);
                bool nullable = nulledType != null;
                if (nullable)
                    memberType = nulledType;

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
                            node.ParsableString = ReadString(ref address);
                        else
                            foreach (BinaryMemberTreeNode childNode in node.Children)
                                ReadObject(childNode, ref address);
                        break;
                }
            }
            private string ReadString(ref VoidPtr address)
            {
                string value;
                switch (StringOffsetSize)
                {
                    case 1:
                        value = StringTable[address.Byte - 1];
                        break;
                    case 2:
                        value = StringTable[address.UShort - 1];
                        break;
                    case 3:
                        value = StringTable[address.UInt24 - 1];
                        break;
                    default:
                    case 4:
                        value = StringTable[address.Int - 1];
                        break;
                }
                address += StringOffsetSize;
                return value;
            }
            private object ReadEnum(Type enumType, ref VoidPtr address)
            {
                object enumValue;
                TypeCode typeCode = Type.GetTypeCode(enumType);
                switch (typeCode)
                {
                    case TypeCode.SByte:
                        enumValue = Enum.ToObject(enumType, address.SByte);
                        address += sizeof(sbyte);
                        break;
                    case TypeCode.Byte:
                        enumValue = Enum.ToObject(enumType, address.Byte);
                        address += sizeof(byte);
                        break;
                    case TypeCode.Int16:
                        enumValue = Enum.ToObject(enumType, address.Short);
                        address += sizeof(short);
                        break;
                    case TypeCode.UInt16:
                        enumValue = Enum.ToObject(enumType, address.UShort);
                        address += sizeof(ushort);
                        break;
                    default:
                    case TypeCode.Int32:
                        enumValue = Enum.ToObject(enumType, address.Int);
                        address += sizeof(int);
                        break;
                    case TypeCode.UInt32:
                        enumValue = Enum.ToObject(enumType, address.UInt);
                        address += sizeof(uint);
                        break;
                    case TypeCode.Int64:
                        enumValue = Enum.ToObject(enumType, address.Long);
                        address += sizeof(long);
                        break;
                    case TypeCode.UInt64:
                        enumValue = Enum.ToObject(enumType, address.ULong);
                        address += sizeof(ulong);
                        break;
                }
                return enumValue;
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
                => new BinaryMemberTreeNode(parent, memberInfo);
            public override BinaryMemberTreeNode CreateNode(object root)
                => new BinaryMemberTreeNode(root);

            public unsafe sealed class BinaryStringTableReader
            {
                private Dictionary<int, string> _table;
                
                public Encoding Encoding { get; private set; }
                public int StringCount { get; private set; }
                public int StringLengthSize { get; private set; }

                public BinaryStringTableReader() { }

                public unsafe void ReadStrings(FileCommonHeader.StringTableHeader* strings)
                {
                    int encodingCodePage = strings->_encodingCodePage;
                    Encoding = Encoding.GetEncoding(encodingCodePage);

                    StringCount = strings->_stringCount;
                    StringLengthSize = strings->StringLengthSize;

                    _table = new Dictionary<int, string>();

                    int i = 0, offset = 0;
                    VoidPtr stringAddr = strings->Address;
                    int length = 0;
                    while (i++ < StringCount)
                    {
                        switch (StringLengthSize)
                        {
                            case 1: length = stringAddr.Byte; break;
                            case 2: length = stringAddr.UShort; break;
                            case 3: length = stringAddr.UInt24; break;
                            case 4: length = stringAddr.Int; break;
                        }
                        stringAddr += StringLengthSize;
                        string s = stringAddr.GetString(0, length, Encoding);
                        _table.Add(offset, s);
                        offset += StringLengthSize + length;
                    }
                }
                public string this[int offset]
                {
                    get
                    {
                        if (_table.ContainsKey(offset))
                            return _table[offset];
                        return null;
                    }
                }
            }
        }
    }
}