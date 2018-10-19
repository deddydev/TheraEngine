using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        IsNull          = 0b0000_0100,
        IsSharedObject  = 0b0000_1000,
        BooleanValue    = 0b1000_0000,
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
            public Endian.EOrder Endian { get; private set; }
            public bool Encrypted { get; private set; }
            public bool Compressed { get; private set; }
            Rfc2898DeriveBytes EncryptionDeriveBytes { get; set; }
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
                //await RootNode.CollectSerializedMembers();

                //Memory.Endian.SerializeOrder = Endian;

                //StringTable = new BinaryStringTableReader();

                //int dataSize = RootNode.CalculatedSize;
                //int stringSize = StringTable.TotalLength.Align(4);
                //StringOffsetSize = StringTable.GetSmallestRepSize(stringSize);
                //int totalSize = FileCommonHeader.Size + stringSize + dataSize;
                
                using (FileMap uncompMap = FileMap.FromFile(FilePath, FileMapProtect.Read))
                {
                    unsafe
                    {
                        FileCommonHeader* hdr = (FileCommonHeader*)uncompMap.Address;
                        Endian = hdr->Endian;
                        Encrypted = hdr->Encrypted;
                        Compressed = hdr->Compressed;

                        VoidPtr addr = hdr->Data;
                        RootNode = ReadObject(ref addr, null);
                    }

                    //uncompMap.BaseStream.Position = 0;

                    //FileStream outStream;

                    ////Compres first, then encrypt
                    ////This is because compression works best on the original patterned data

                    //if (Compressed)
                    //{
                    //    outStream = new FileStream(FilePath,
                    //        FileMode.OpenOrCreate,
                    //        FileAccess.ReadWrite,
                    //        FileShare.ReadWrite,
                    //        8,
                    //        FileOptions.RandomAccess);
                    //    outStream.SetLength(totalSize);
                    //    outStream.Position = 0;
                    //    SevenZip.Compression.LZMA.Encoder e = new SevenZip.Compression.LZMA.Encoder();
                    //    e.Code(uncompMap.BaseStream, outStream, totalSize, totalSize, CompressionProgress);
                    //    outStream.SetLength(outStream.Position);
                    //}
                    //else
                    //    outStream = uncompMap.BaseStream;

                    //if (Encrypted)
                    //{
                    //    SymmetricAlgorithm crypto = new RijndaelManaged();
                    //    byte[] key = EncryptionDeriveBytes.GetBytes(crypto.KeySize / 8);
                    //    byte[] iv = EncryptionDeriveBytes.GetBytes(crypto.BlockSize / 8);

                    //    outStream.Position = 0;
                    //    int blockSize = 0x1000;
                    //    int bytesRead = 0;
                    //    long tempPos;
                    //    byte[] buffer = new byte[blockSize];
                    //    using (CryptoStream cryptoStream = new CryptoStream(outStream, crypto.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                    //    {
                    //        while (true)
                    //        {
                    //            tempPos = outStream.Position;
                    //            bytesRead = outStream.Read(buffer, 0, blockSize);
                    //            if (bytesRead <= 0)
                    //                break;
                    //            outStream.Position = tempPos;
                    //            cryptoStream.Write(buffer, 0, bytesRead);
                    //        }
                    //    }
                    //}

                    //if (Compressed)
                    //    outStream.Dispose();
                }
            }
            private unsafe BinaryMemberTreeNode ReadObject(ref VoidPtr address, Type memberType)
            {
                object obj = null;
                Type objType = memberType;

                EBinaryObjectFlags flags = (EBinaryObjectFlags)address.ReadByte();
                if ((flags & EBinaryObjectFlags.IsDefault) != 0)
                {
                    obj = objType.GetDefaultValue();
                }
                else if ((flags & EBinaryObjectFlags.IsNull) != 0)
                {
                    obj = null;
                }
                else
                {
                    bool derived = (flags & EBinaryObjectFlags.IsDerived) != 0;
                    if (derived)
                    {
                        string assemblyTypeName = ReadString(ref address);
                        objType = Type.GetType(assemblyTypeName, false, false) ?? memberType;
                    }

                    Type nulledType = Nullable.GetUnderlyingType(objType);
                    bool nullable = nulledType != null;
                    if (nullable)
                        objType = nulledType;
                    
                    //First, handle built-in primitive types
                    switch (objType.Name)
                    {
                        case nameof(Boolean):   /*obj = address.ReadByte() != 0;*/  break;
                        case nameof(SByte):     obj = address.ReadSByte();          break;
                        case nameof(Byte):      obj = address.ReadByte();           break;
                        case nameof(Char):      obj = address.ReadChar();           break;
                        case nameof(Int16):     obj = address.ReadShort();          break;
                        case nameof(UInt16):    obj = address.ReadUShort();         break;
                        case nameof(Int32):     obj = address.ReadInt();            break;
                        case nameof(UInt32):    obj = address.ReadUInt();           break;
                        case nameof(Int64):     obj = address.ReadLong();           break;
                        case nameof(UInt64):    obj = address.ReadULong();          break;
                        case nameof(Single):    obj = address.ReadFloat();          break;
                        case nameof(Double):    obj = address.ReadDouble();         break;
                        case nameof(Decimal):   obj = address.ReadDecimal();        break;
                        case nameof(String):    obj = ReadString(ref address);      break;
                        default:
                            if (objType.IsEnum)
                            {
                                obj = ReadEnum(objType, ref address);
                            }
                            else if (objType.IsArray)
                            {
                                obj = ReadArray(objType, ref address);
                            }
                            else if (objType.GetInterface(nameof(ISerializablePointer)) != null)
                            {
                                obj = ReadSerializablePointer(objType, ref address);
                            }
                            else if (objType.GetInterface(nameof(ISerializableByteArray)) != null)
                            {
                                obj = ReadSerializableByteArray(objType, ref address);
                            }
                            else if (objType.GetInterface(nameof(ISerializableString)) != null)
                            {
                                obj = ReadSerializableString(objType, ref address);
                            }
                            else if (objType.IsValueType)
                            {
                                obj = ReadStruct(objType, ref address);
                            }
                            else if (typeof(TFileObject).IsAssignableFrom(objType) && ShouldReadFileObjectManually(objType))
                            {
                                obj = ReadFileObjectManually(objType, ref address);
                            }
                            else
                            {
                                obj = null;
                            }

                            break;
                    }
                }

                return new BinaryMemberTreeNode(obj) { MemberType = memberType, };
            }

            private bool ShouldReadFileObjectManually(Type objType)
            {
                FileExt ext = TFileObject.GetFileExtension(objType);
                if (ext == null)
                    return false;

                bool serConfig = ext.ManualBinConfigSerialize;
                bool serState = ext.ManualBinStateSerialize;

                return serConfig || serState;
            }
            private object ReadFileObjectManually(Type objType, ref VoidPtr address)
            {
                int size = address.ReadInt();
                object obj = SerializationCommon.CreateObject(objType);

                TFileObject fileObj = (TFileObject)obj;
                fileObj.ManualReadBinary(address, size, StringTable);

                address += size;

                return obj;
            }
            private object ReadStruct(Type objType, ref VoidPtr address)
            {
                int size = address.ReadInt();
                object obj = SerializationCommon.CreateObject(objType);

                Marshal.PtrToStructure(address, obj);

                address += size;

                return obj;
            }
            private object ReadSerializableByteArray(Type objType, ref VoidPtr address)
            {
                int size = address.ReadInt();
                ISerializableByteArray serBytes = (ISerializableByteArray)SerializationCommon.CreateObject(objType);

                byte[] bytes = new byte[size];
                for (int i = 0; i < size; ++i)
                    bytes[i] = address.ReadByte();
                serBytes.ReadFromBytes(bytes);

                return serBytes;
            }
            private object ReadSerializablePointer(Type objType, ref VoidPtr address)
            {
                int size = address.ReadInt();

                ISerializablePointer serPtr = (ISerializablePointer)SerializationCommon.CreateObject(objType);
                serPtr.ReadFromPointer(address, size);

                address += size;

                return serPtr;
            }
            private object ReadSerializableString(Type objType, ref VoidPtr address)
            {
                ISerializableString serStr = (ISerializableString)SerializationCommon.CreateObject(objType);

                string value = ReadString(ref address);
                serStr.ReadFromString(value);

                return serStr;
            }

            private string ReadString(ref VoidPtr address)
            {
                string value;
                switch (StringOffsetSize)
                {
                    case 1: value = StringTable[address.Byte   - 1]; break;
                    case 2: value = StringTable[address.UShort - 1]; break;
                    case 3: value = StringTable[address.UInt24 - 1]; break;
                    default:
                    case 4: value = StringTable[address.Int    - 1]; break;
                }
                address.Offset(StringOffsetSize);
                return value;
            }
            private object ReadEnum(Type enumType, ref VoidPtr address)
            {
                TypeCode typeCode = Type.GetTypeCode(enumType);
                switch (typeCode)
                {
                    case TypeCode.SByte:    return Enum.ToObject(enumType, address.ReadSByte());
                    case TypeCode.Byte:     return Enum.ToObject(enumType, address.ReadByte());
                    case TypeCode.Int16:    return Enum.ToObject(enumType, address.ReadShort());
                    case TypeCode.UInt16:   return Enum.ToObject(enumType, address.ReadUShort());
                    default:
                    case TypeCode.Int32:    return Enum.ToObject(enumType, address.ReadInt());
                    case TypeCode.UInt32:   return Enum.ToObject(enumType, address.ReadUInt());
                    case TypeCode.Int64:    return Enum.ToObject(enumType, address.ReadLong());
                    case TypeCode.UInt64:   return Enum.ToObject(enumType, address.ReadULong());
                }
            }
            private object ReadArray(Type type, ref VoidPtr address)
            {
                return null;
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
                    StringCount = (int)strings->_stringCount;
                    StringLengthSize = strings->StringLengthSize;

                    _table = new Dictionary<int, string>();

                    int i = 0, offset = 0;
                    VoidPtr stringAddr = strings->Address;
                    int length = 0;
                    while (i++ < StringCount/* && offset.Align(4) < dataLength*/)
                    {
                        switch (StringLengthSize)
                        {
                            case 1: length = stringAddr.Byte;   break;
                            case 2: length = stringAddr.UShort; break;
                            case 3: length = stringAddr.UInt24; break;
                            case 4: length = stringAddr.Int;    break;
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