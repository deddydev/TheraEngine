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
using static TheraEngine.Core.Files.Serialization.TDeserializer.ReaderBinary;

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
            object fileObject,
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
            Writer = new WriterBinary(
                this,
                fileObject,
                filePath,
                flags,
                progress,
                cancel,
                endian,
                encrypted,
                compressed,
                encryptionPassword,
                compressionProgress);
            await Writer.WriteObjectAsync();
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
            object fileObject,
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
            string filePath = TFileObject.GetFilePath(targetDirectoryPath, fileName, EProprietaryFileFormat.Binary, fileObject.GetType());
            Writer = new WriterBinary(
                this,
                fileObject,
                filePath, 
                flags,
                progress,
                cancel,
                endian,
                encrypted,
                compressed,
                encryptionPassword,
                compressionProgress);
            await Writer.WriteObjectAsync();
            Engine.PrintLine("Serialized binary file to {0}", filePath);
        }
        public class WriterBinary : AbstractWriter
        {
            public Endian.EOrder Endian { get; }
            public bool Encrypted { get; }
            public bool Compressed { get; }
            ICodeProgress CompressionProgress { get; }
            Rfc2898DeriveBytes EncryptionDeriveBytes { get; }
            BinaryStringTableWriter StringTable { get; set; }
            public int StringOffsetSize { get; private set; }
            public override EProprietaryFileFormatFlag Format => EProprietaryFileFormatFlag.Binary;

            public WriterBinary(
                TSerializer owner,
                object rootFileObject,
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
            private unsafe void WriteElement(SerializeElement node, ref VoidPtr address)
            {
                WriteStartElement(SerializationCommon.FixElementName(node.Name), ref address);
                int* attributeCount = (int*)address;
                *attributeCount = 0;
                address += 4;
                
                if (ReportProgress())
                {
                    WriteEndElement();
                    return;
                }

                if (node.IsDerivedType)
                {
                    WriteAttribute(SerializationCommon.TypeIdent, node.ObjectType.AssemblyQualifiedName, attributeCount);
                    if (ReportProgress())
                    {
                        WriteEndElement();
                        return;
                    }
                }

                List<SerializeAttribute> attributes = node.Attributes;
                List<SerializeElement> childElements = node.ChildElements;
                bool hasElementContent = node.GetElementContent(null, out object elementContent);

                foreach (SerializeAttribute attribute in attributes)
                {
                    if (attribute.GetObject(null, out object value))
                    {
                        WriteAttribute(attribute.Name, value, attributeCount);
                        if (ReportProgress())
                        {
                            WriteEndElement();
                            return;
                        }
                    }
                }

                if (hasElementContent)
                {
                    WriteObject(elementContent);
                    if (ReportProgress())
                    {
                        WriteEndElement();
                        return;
                    }
                }
                else
                    foreach (SerializeElement childNode in childElements)
                    {
                        WriteElement(childNode, ref address);
                        if (ReportProgress())
                        {
                            WriteEndElement();
                            return;
                        }
                    }

                WriteEndElement();
            }

            private void WriteObject(object elementContent) => throw new NotImplementedException();
            private void WriteStartElement(string v, ref VoidPtr address) => throw new NotImplementedException();
            private unsafe void WriteAttribute(string name, object value, int* attributeCount) => throw new NotImplementedException();
            private void WriteEndElement() => throw new NotImplementedException();

            protected override async Task WriteTreeAsync()
            {
                Memory.Endian.SerializeOrder = Endian;

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
                        hdr->Endian = Endian;
                        hdr->Encrypted = Encrypted;
                        hdr->Compressed = Compressed;

                        hdr->_stringTableLength = stringSize;
                        hdr->_sharedObjectsCount = SharedObjects.Count;

                        //SHA256Managed SHhash = new SHA256Managed();
                        //byte[] integrityHash = SHhash.ComputeHash(uncompMap.BaseStream);
                        //hdr->WriteHash(integrityHash);

                        StringTable.WriteTable(hdr->Strings);

                        VoidPtr offsets = hdr->Data;
                        VoidPtr data = offsets + SharedObjects.Count * sizeof(int);
                        int i = 0;
                        foreach (var pair in SharedObjects)
                        {
                            SharedObjectIndices.Add(pair.Key, i++);
                            offsets.WriteInt(data - offsets);
                            WriteObject(pair.Value, ref data);
                        }
                        WriteObject(RootNode, ref data);
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
            private unsafe void WriteObject(SerializeElement node, ref VoidPtr address)
            {
                object value = node.Object;
                Type objType = node.ObjectType;
                object defaultValue = node.DefaultObject;
                
                Type nulledType = Nullable.GetUnderlyingType(objType);
                bool nullable = nulledType != null;
                if (nullable)
                    objType = nulledType;

                if (value == defaultValue)
                {
                    address.WriteByte((byte)EBinaryObjectFlags.IsDefault);
                    return;
                }
                else
                {
                    EBinaryObjectFlags flags = EBinaryObjectFlags.IsNotDefault;

                    if (value is null)
                    {
                        flags |= EBinaryObjectFlags.IsNull;
                    }
                    else if (value is bool boolVal)
                    {
                        if (boolVal)
                            flags |= EBinaryObjectFlags.BooleanValue;
                    }
                    else if (node.IsDerivedType)
                    {
                        //TODO: generate assembly list and namespace folder tree in another section to save space
                        //This will do for now

                        flags |= EBinaryObjectFlags.IsDerived;
                        WriteString(node.ObjectType.AssemblyQualifiedName, ref address);
                    }

                    address.WriteByte((byte)flags);
                }

                if (value == null || value is bool)
                    return;

                //First, handle built-in primitive types
                switch (objType.Name)
                {
                    //case nameof(Boolean):   /*address.WriteByte((byte)((bool)value ? 1 : 0));*/ break;
                    case nameof(SByte):     address.WriteSByte((sbyte)value);               break;
                    case nameof(Byte):      address.WriteByte((byte)value);                 break;
                    case nameof(Char):      address.WriteChar((char)value);                 break;
                    case nameof(Int16):     address.WriteShort((short)value);               break;
                    case nameof(UInt16):    address.WriteUShort((ushort)value);             break;
                    case nameof(Int32):     address.WriteInt((int)value);                   break;
                    case nameof(UInt32):    address.WriteUInt((uint)value);                 break;
                    case nameof(Int64):     address.WriteLong((long)value);                 break;
                    case nameof(UInt64):    address.WriteULong((ulong)value);               break;
                    case nameof(Single):    address.WriteFloat((float)value);               break;
                    case nameof(Double):    address.WriteDouble((double)value);             break;
                    case nameof(Decimal):   address.WriteDecimal((decimal)value);           break;
                    case nameof(String):    WriteString(value?.ToString(), ref address);    break;
                    default:
                        if (objType.IsEnum)
                        {
                            WriteEnum(value, ref address);
                        }
                        if (objType.IsArray)
                        {
                            WriteArray(value, objType.GetElementType(), ref address);
                        }
                        else if (value is ISerializablePointer serPtr)
                        {
                            WriteSerializablePointer(ref address, serPtr, node);
                        }
                        else if (value is ISerializableByteArray serBytes)
                        {
                            WriteSerializableByteArray(ref address, serBytes, node);
                        }
                        else if (value is ISerializableString serStr)
                        {
                            WriteSerializableString(ref address, serStr, node);
                        }
                        else if (objType.IsValueType)
                        {
                            int size = Marshal.SizeOf(value);
                            Marshal.StructureToPtr(value, address, true);
                            address += size;
                        }
                        else if (value is TFileObject fobj && ShouldWriteFileObjectManually(objType))
                        {
                            WriteFileObjectManually(ref address, fobj, objType, node);
                        }
                        else
                            foreach (SerializeElement childNode in node.ChildElements)
                                WriteObject(childNode, ref address);
                        
                        break;
                }
            }

            private bool ShouldWriteFileObjectManually(Type objType)
            {
                FileExt ext = TFileObject.GetFileExtension(objType);
                if (ext == null)
                    return false;

                bool serConfig = ext.ManualBinConfigSerialize;
                bool serState = ext.ManualBinStateSerialize;

                return serConfig || serState;
            }
            private void WriteFileObjectManually(ref VoidPtr address, TFileObject fobj, Type objType, SerializeElement node)
            {
                int size = node.ManuallyCalculatedSize;
                address.WriteInt(size);
                fobj.ManualWriteBinary(address, size, StringTable, Flags);
                address += size;
            }
            internal int GetSizeObject(SerializeElement node)
            {
                if (node.Object is TObject tobj && SharedObjects.ContainsKey(tobj.Guid))
                    return 5; //flags byte + object index

                int size = 0;
                object value = node.Object;
                Type objType = node.ObjectType;
                object defaultValue = node.DefaultObject;

                if (objType != null)
                {
                    Type nulledType = Nullable.GetUnderlyingType(objType);
                    bool nullable = nulledType != null;
                    if (nullable)
                        objType = nulledType;
                }

                ++size; //flags byte

                if (value == defaultValue || value == null || value is bool)
                    return size;
                else if (node.IsDerivedType)
                {
                    StringTable.Add(node.ObjectType.AssemblyQualifiedName);
                    size += StringOffsetSize;
                }
                
                if (node.ChildElements.Count > 0)
                {
                    foreach (SerializeElement childNode in node.ChildElements)
                        size += GetSizeObject(childNode);

                    return size;
                }

                switch (objType.Name)
                {
                    case nameof(Boolean):   break; //Written to flags, no size
                    case nameof(SByte):
                    case nameof(Byte):      size += 1;                  break;
                    case nameof(Char):      size += sizeof(char);       break;
                    case nameof(Int16):     size += sizeof(short);      break;
                    case nameof(UInt16):    size += sizeof(ushort);     break;
                    case nameof(Int32):     size += sizeof(int);        break;
                    case nameof(UInt32):    size += sizeof(uint);       break;
                    case nameof(Int64):     size += sizeof(long);       break;
                    case nameof(UInt64):    size += sizeof(ulong);      break;
                    case nameof(Single):    size += sizeof(float);      break;
                    case nameof(Double):    size += sizeof(double);     break;
                    case nameof(Decimal):   size += sizeof(decimal);    break;
                    case nameof(String):
                        StringTable.Add((string)value);
                        size += StringOffsetSize;
                        break;
                    default:
                        if (objType.IsEnum)
                        {
                            size += GetSizeEnum(value);
                        }
                        else if (objType.IsArray)
                        {
                            size += GetSizeArray(value, objType.GetElementType());
                        }
                        else if (value is ISerializablePointer serPtr)
                        {
                            size += GetSizeSerializablePointer(serPtr, node);
                        }
                        else if (value is ISerializableByteArray serBytes)
                        {
                            size += GetSizeSerializableByteArray(serBytes, node);
                        }
                        else if (value is ISerializableString serStr)
                        {
                            size += GetSizeSerializableString(serStr, node);
                        }
                        else if (objType.IsValueType)
                        {
                            size += Marshal.SizeOf(value);
                        }
                        else if (value is TFileObject fobj && ShouldWriteFileObjectManually(objType))
                        {
                            size += GetSizeFileObjectManually(fobj, objType, node);
                        }
                        else
                        {

                        }
                        break;
                }

                return size;
            }

            private int GetSizeFileObjectManually(TFileObject fobj, Type objectType, SerializeElement node)
            {
                int size = sizeof(int); //length
                node.ManuallyCalculatedSize = fobj.ManualGetSizeBinary(StringTable, Flags);
                size += node.ManuallyCalculatedSize;
                return size;
            }
            private int GetSizeArray(object value, Type elementType)
            {
                Array array = value as Array;
                return 0;
            }
            private void WriteArray(object value, Type elementType, ref VoidPtr address)
            {
                Array array = value as Array;

                Bin8 flags = new Bin8(0);
                flags[0, 5] = (byte)(array.Rank - 1);
                //flags[5] = false;
                //flags[6] = false;
                //flags[7] = false;
                address.WriteByte(flags);

                for (int i = 0; i < array.Rank; ++i)
                    address.WriteInt(array.GetLength(i));

                //for (int i = 0; i < array.Rank; ++i)
                //{
                //    for (int x = 0; x < array.GetLength(i); ++x)
                //    {
                //        address.WriteInt(array.GetValue(i));
                //    }
                //}
            }

            private int GetSizeSerializablePointer(ISerializablePointer value, SerializeElement node)
            {
                return sizeof(int) + (node.ParsablePointerSize = value.GetSize());
            }
            private void WriteSerializablePointer(ref VoidPtr address, ISerializablePointer value, SerializeElement node)
            {
                value.WriteToPointer(address);
                address += node.ParsablePointerSize;
            }

            private int GetSizeSerializableByteArray(ISerializableByteArray value, SerializeElement node)
            {
                byte[] bytes = value.WriteToBytes();

                //We need to save this array instance for later
                //in case the value changes between now and writing
                node.ParsableBytes = bytes;

                return sizeof(int) + (bytes?.Length ?? 0);
            }
            private void WriteSerializableByteArray(ref VoidPtr address, ISerializableByteArray value, SerializeElement node)
            {
                byte[] bytes = node.ParsableBytes;
                address.WriteInt(bytes.Length);
                if (bytes != null)
                    foreach (byte b in bytes)
                        address.WriteByte(b);
            }

            private int GetSizeSerializableString(ISerializableString value, SerializeElement node)
            {
                string str = value.WriteToString();
                node.ParsableString = str;
                StringTable.Add(str);
                return StringOffsetSize;
            }
            private void WriteSerializableString(ref VoidPtr address, ISerializableString value, SerializeElement node)
            {
                WriteString(node.ParsableString, ref address);
            }

            private void WriteString(string value, ref VoidPtr address)
            {
                int offset = value == null ? 0 : StringTable[value] + 1;
                switch (StringOffsetSize)
                {
                    case 1: address.Byte = (byte)offset; break;
                    case 2: address.UShort = (ushort)offset; break;
                    case 3: address.UInt24 = (uint)offset; break;
                    default:
                    case 4: address.Int = offset; break;
                }
                address += StringOffsetSize;
            }
            private void WriteEnum(object value, ref VoidPtr address)
            {
                Enum enumValue = (Enum)value;
                TypeCode typeCode = enumValue.GetTypeCode();
                switch (typeCode)
                {
                    case TypeCode.SByte:    address.WriteSByte((sbyte)value);   break;
                    case TypeCode.Byte:     address.WriteByte((byte)value);     break;
                    case TypeCode.Int16:    address.WriteShort((short)value);   break;
                    case TypeCode.UInt16:   address.WriteUShort((ushort)value); break;
                    default:
                    case TypeCode.Int32:    address.WriteInt((int)value);       break;
                    case TypeCode.UInt32:   address.WriteUInt((uint)value);     break;
                    case TypeCode.Int64:    address.WriteLong((long)value);     break;
                    case TypeCode.UInt64:   address.WriteULong((ulong)value);   break;
                }
            }
            private int GetSizeEnum(object value)
            {
                Enum enumValue = (Enum)value;
                TypeCode typeCode = enumValue.GetTypeCode();
                switch (typeCode)
                {
                    case TypeCode.SByte:    return sizeof(sbyte);
                    case TypeCode.Byte:     return sizeof(byte);
                    case TypeCode.Int16:    return sizeof(short);
                    case TypeCode.UInt16:   return sizeof(ushort);
                    default:
                    case TypeCode.Int32:    return sizeof(int);
                    case TypeCode.UInt32:   return sizeof(uint);
                    case TypeCode.Int64:    return sizeof(long);
                    case TypeCode.UInt64:   return sizeof(ulong);
                }
            }
            
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

                public void Add(LocalizedString s)
                {
                    //if ((!string.IsNullOrEmpty(s)) && (!_table.ContainsKey(s)))
                    //{
                    //    _table.Add(s, 0);
                    //    int lengthSize = GetSmallestRepSize(s.Length);
                    //    StringLengthSize = Math.Max(StringLengthSize, lengthSize);
                    //    TotalLength += Encoding.GetMaxByteCount(s.Length);
                    //}
                }
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
