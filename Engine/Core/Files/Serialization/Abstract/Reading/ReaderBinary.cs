﻿using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class Deserializer : BaseSerializationIO
    {
        public class ReaderBinary : AbstractReader
        {
            public Endian.EOrder Endian { get; private set; }
            public bool Encrypted { get; private set; }
            public bool Compressed { get; private set; }
            Rfc2898DeriveBytes EncryptionDeriveBytes { get; set; }
            BinaryStringTableReader StringTable { get; set; }
            public int StringOffsetSize { get; private set; }

            public override EProprietaryFileFormatFlag Format => EProprietaryFileFormatFlag.Binary;

            public ReaderBinary(
                Deserializer owner,
                string filePath,
                TypeProxy fileType,
                Stream stream,
                IProgress<float> progress,
                CancellationToken cancel,
                string encryptionPassword)
                : base(owner, filePath, fileType, stream, progress, cancel)
            {
                if (Encrypted)
                {
                    Random r = new Random();
                    byte[] salt = new byte[8];
                    r.NextBytes(salt);
                    EncryptionDeriveBytes = new Rfc2898DeriveBytes(encryptionPassword, salt, 1000);
                }
            }

            protected override async Task ReadTreeAsync()
            {
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
                }
            }
            private unsafe SerializeElement ReadObject(ref VoidPtr address, Type memberType)
            {
                object obj = null;
                Type objType = memberType;
                //if (objType is null)
                //{
                //    Engine.LogWarning($"{nameof(objType)} cannot be null.");
                //    return null;
                //}

                EBinaryObjectFlags flags = (EBinaryObjectFlags)address.ReadByte();
                
                if ((flags & EBinaryObjectFlags.IsNull) != 0)
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

                    if ((flags & EBinaryObjectFlags.IsDefault) != 0)
                    {
                        obj = objType.GetDefaultValue();
                    }
                    else
                    {
                        //First, handle built-in primitive types
                        switch (objType.Name)
                        {
                            case nameof(Boolean):   /*obj = address.ReadByte() != 0;*/  break;
                            case nameof(SByte): obj = address.ReadSByte(); break;
                            case nameof(Byte): obj = address.ReadByte(); break;
                            case nameof(Char): obj = address.ReadChar(); break;
                            case nameof(Int16): obj = address.ReadShort(); break;
                            case nameof(UInt16): obj = address.ReadUShort(); break;
                            case nameof(Int32): obj = address.ReadInt(); break;
                            case nameof(UInt32): obj = address.ReadUInt(); break;
                            case nameof(Int64): obj = address.ReadLong(); break;
                            case nameof(UInt64): obj = address.ReadULong(); break;
                            case nameof(Single): obj = address.ReadFloat(); break;
                            case nameof(Double): obj = address.ReadDouble(); break;
                            case nameof(Decimal): obj = address.ReadDecimal(); break;
                            case nameof(String): obj = ReadString(ref address); break;
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
                                else if (typeof(IFileObject).IsAssignableFrom(objType) && ShouldReadFileObjectManually(objType))
                                {
                                    obj = ReadFileObjectManually(objType, ref address);
                                }
                                else
                                {
                                    SerializeElement elem = new SerializeElement(null, new TSerializeMemberInfo(memberType, null));
                                    var objSer = BaseObjectSerializer.GetSerializerFor(objType, false, true);
                                    objSer.TreeNode = elem;
                                    objSer.DeserializeTreeFromBinary(ref address, this);
                                    objSer.DeserializeTreeToObjectAsync();
                                    obj = null;
                                    return elem;
                                }

                                break;
                        }
                    }
                }

                return new SerializeElement(obj, new TSerializeMemberInfo(memberType, null));
            }

            private bool ShouldReadFileObjectManually(Type objType)
            {
                if (TFileObject.GetFileExtension(objType) is null)
                    return false;

                bool serConfig = TFileObject.GetFileExtension(objType).ManualBinConfigSerialize;
                bool serState = TFileObject.GetFileExtension(objType).ManualBinStateSerialize;

                return serConfig || serState;
            }
            private object ReadFileObjectManually(Type objType, ref VoidPtr address)
            {
                int size = address.ReadInt();
                object obj = SerializationCommon.CreateInstance(objType);

                IFileObject fileObj = (IFileObject)obj;
                fileObj.ManualReadBinary(address, size, StringTable);

                address += size;

                return obj;
            }
            private object ReadStruct(Type objType, ref VoidPtr address)
            {
                int size = address.ReadInt();
                object obj = SerializationCommon.CreateInstance(objType);

                Marshal.PtrToStructure(address, obj);

                address += size;

                return obj;
            }
            private object ReadSerializableByteArray(Type objType, ref VoidPtr address)
            {
                int size = address.ReadInt();
                ISerializableByteArray serBytes = (ISerializableByteArray)SerializationCommon.CreateInstance(objType);

                byte[] bytes = new byte[size];
                for (int i = 0; i < size; ++i)
                    bytes[i] = address.ReadByte();
                serBytes.ReadFromBytes(bytes);

                return serBytes;
            }
            private object ReadSerializablePointer(Type objType, ref VoidPtr address)
            {
                int size = address.ReadInt();

                ISerializablePointer serPtr = (ISerializablePointer)SerializationCommon.CreateInstance(objType);
                serPtr.ReadFromPointer(address, size);

                address += size;

                return serPtr;
            }
            private object ReadSerializableString(Type objType, ref VoidPtr address)
            {
                ISerializableString serStr = (ISerializableString)SerializationCommon.CreateInstance(objType);

                string value = ReadString(ref address);
                serStr.ReadFromString(value);

                return serStr;
            }

            private string ReadString(ref VoidPtr address)
            {
                var value = StringOffsetSize switch
                {
                    1 => StringTable[address.Byte - 1],
                    2 => StringTable[address.UShort - 1],
                    3 => StringTable[address.UInt24 - 1],
                    _ => StringTable[address.Int - 1],
                };
                address.Offset(StringOffsetSize);
                return value;
            }
            private object ReadEnum(Type enumType, ref VoidPtr address)
            {
                TypeCode typeCode = Type.GetTypeCode(enumType);
                return typeCode switch
                {
                    TypeCode.SByte => Enum.ToObject(enumType, address.ReadSByte()),
                    TypeCode.Byte => Enum.ToObject(enumType, address.ReadByte()),
                    TypeCode.Int16 => Enum.ToObject(enumType, address.ReadShort()),
                    TypeCode.UInt16 => Enum.ToObject(enumType, address.ReadUShort()),
                    TypeCode.UInt32 => Enum.ToObject(enumType, address.ReadUInt()),
                    TypeCode.Int64 => Enum.ToObject(enumType, address.ReadLong()),
                    TypeCode.UInt64 => Enum.ToObject(enumType, address.ReadULong()),
                    _ => Enum.ToObject(enumType, address.ReadInt()),
                };
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
            [Flags]
            public enum EBinaryObjectFlags : byte
            {
                /// <summary>
                /// This object is what is set to when the main constructor is run.
                /// </summary>
                IsDefault = 0b0000_0000,
                /// <summary>
                /// This object is different from what it is set to when the main constructor is run.
                /// </summary>
                IsNotDefault = 0b0000_0001,
                /// <summary>
                /// This object inherits from the type of the member it is assigned to.
                /// This is always true if the member type is an interface.
                /// Otherwise the object is exactly the same type or null.
                /// </summary>
                IsDerived = 0b0000_0010,
                /// <summary>
                /// This object is null.
                /// Not used by value types.
                /// </summary>
                IsNull = 0b0000_0100,
                /// <summary>
                /// This object is a reference type that is referenced by multiple other objects in the same file.
                /// </summary>
                IsSharedObject = 0b0000_1000,
                /// <summary>
                /// This element contains other elements of the same category.
                /// </summary>
                IsGrouping = 0b0001_0000,
                /// <summary>
                /// Unused bit.
                /// </summary>
                Unused2 = 0b0010_0000,
                /// <summary>
                /// Unused bit.
                /// </summary>
                Unused3 = 0b0100_0000,
                /// <summary>
                /// If this object's type is boolean, its value is stored here.
                /// Not used for non-boolean objects.
                /// </summary>
                BooleanValue = 0b1000_0000,
            }
        }
    }
}