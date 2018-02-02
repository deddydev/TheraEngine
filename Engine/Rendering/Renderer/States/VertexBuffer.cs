﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using System.ComponentModel;
using System.IO;

namespace TheraEngine.Rendering.Models
{
    public enum EBufferTarget
    {
        DataArray,
        DrawIndices,
    }
    public enum BufferType
    {
        Position        = 0, //VertexBuffer.MaxMorphs + 1
        Normal          = 1, //VertexBuffer.MaxMorphs + 1
        Binormal        = 2, //VertexBuffer.MaxMorphs + 1
        Tangent         = 3, //VertexBuffer.MaxMorphs + 1
        MatrixIds       = 4, //VertexBuffer.MaxMorphs + 1
        MatrixWeights   = 5, //VertexBuffer.MaxMorphs + 1
        Color           = 6, //VertexBuffer.MaxColors
        TexCoord        = 7, //VertexBuffer.MaxTexCoords
        Other           = 8, //VertexBuffer.MaxOtherBuffers
    }
    public class VertexAttribInfo
    {
        public VertexAttribInfo(BufferType type, int index = 0)
        {
            _type = type;
            _index = index.Clamp(0, GetMaxBuffersForType(type) - 1);
        }

        public BufferType _type;
        public int _index;

        public static int GetMaxBuffersForType(BufferType type)
        {
            switch (type)
            {
                case BufferType.Color:
                    return VertexShaderDesc.MaxColors;
                case BufferType.TexCoord:
                    return VertexShaderDesc.MaxTexCoords;
                case BufferType.Other:
                    return VertexShaderDesc.MaxOtherBuffers;
                default:
                    return VertexShaderDesc.MaxMorphs + 1;
            }
        }
        public static string GetAttribName(BufferType type, int index) => type.ToString() + index.ToString();
        public static int GetLocation(BufferType type, int index)
        {
            int location = 0;
            for (BufferType i = 0; i < type; ++i)
                location += GetMaxBuffersForType(i);
            return location + index;
        }
        public string GetAttribName() => GetAttribName(_type, _index);
        public int GetLocation() => GetLocation(_type, _index);
    }
    public enum BufferUsage
    {
        StreamDraw  = 0,
        StreamRead  = 1,
        StreamCopy  = 2,
        StaticDraw  = 4,
        StaticRead  = 5,
        StaticCopy  = 6,
        DynamicDraw = 8,
        DynamicRead = 9,
        DynamicCopy = 10
    }
    public class VertexBuffer : BaseRenderState, IDisposable
    {
        public enum ComponentType
        {
            SByte   = 0,
            Byte    = 1,
            Short   = 2,
            UShort  = 3,
            Int     = 4,
            UInt    = 5,
            Float   = 6,
            Double  = 10
        }

        internal BufferUsage _usage = BufferUsage.StaticDraw;

        [TSerialize("ComponentType", Order = 0, XmlNodeType = EXmlNodeType.Attribute)]
        internal ComponentType _componentType;
        [TSerialize("Normalize", Order = 1, XmlNodeType = EXmlNodeType.Attribute)]
        internal bool _normalize;
        [TSerialize("Integral", Order = 2, XmlNodeType = EXmlNodeType.Attribute)]
        internal bool _integral = false;
        [TSerialize("Data", Order = 2, IsXmlElementString = true)]
        internal DataSource _data;
        internal int _bufferIndex;
        internal int _location, _vaoId = 0;
        [TSerialize("ComponentCount", XmlNodeType = EXmlNodeType.Attribute)]
        internal int _componentCount;
        [TSerialize("ElementCount", XmlNodeType = EXmlNodeType.Attribute)]
        internal int _elementCount;
        internal EBufferTarget _target;
        [TSerialize("Type", XmlNodeType = EXmlNodeType.Attribute)]
        internal BufferType _type = BufferType.Other;

        [CustomXMLSerializeMethod("Data")]
        private unsafe bool CustomDataSerialize(XmlWriter writer)
        {
            int count = _elementCount * _componentCount;
            switch (_componentType)
            {
                case ComponentType.SByte:
                    sbyte* ptr1 = (sbyte*)_data.Address;
                    for (int i = 0; i < count; ++i)
                        writer.WriteString((*ptr1++).ToString() + " ");
                    break;
                case ComponentType.Byte:
                    byte* ptr2 = (byte*)_data.Address;
                    for (int i = 0; i < count; ++i)
                        writer.WriteString((*ptr2++).ToString() + " ");
                    break;
                case ComponentType.Short:
                    short* ptr3 = (short*)_data.Address;
                    for (int i = 0; i < count; ++i)
                        writer.WriteString((*ptr3++).ToString() + " ");
                    break;
                case ComponentType.UShort:
                    ushort* ptr4 = (ushort*)_data.Address;
                    for (int i = 0; i < count; ++i)
                        writer.WriteString((*ptr4++).ToString() + " ");
                    break;
                case ComponentType.Int:
                    int* ptr5 = (int*)_data.Address;
                    for (int i = 0; i < count; ++i)
                        writer.WriteString((*ptr5++).ToString() + " ");
                    break;
                case ComponentType.UInt:
                    uint* ptr6 = (uint*)_data.Address;
                    for (int i = 0; i < count; ++i)
                        writer.WriteString((*ptr6++).ToString() + " ");
                    break;
                case ComponentType.Float:
                    float* ptr7 = (float*)_data.Address;
                    for (int i = 0; i < count; ++i)
                        writer.WriteString((*ptr7++).ToString() + " ");
                    break;
                case ComponentType.Double:
                    double* ptr8 = (double*)_data.Address;
                    for (int i = 0; i < count; ++i)
                        writer.WriteString((*ptr8++).ToString() + " ");
                    break;
            }
            return true;
        }
        [CustomXMLDeserializeMethod("Data")]
        private unsafe bool CustomDataDeserialize(XMLReader reader)
        {
            int count = _elementCount * _componentCount;
            switch (_componentType)
            {
                case ComponentType.SByte:
                    _data = DataSource.Allocate(count * sizeof(sbyte));
                    sbyte* ptr1 = (sbyte*)_data.Address;
                    for (int k = 0; k < count && reader.ReadValue(ptr1++); ++k) ;
                    break;
                case ComponentType.Byte:
                    _data = DataSource.Allocate(count * sizeof(byte));
                    byte* ptr2 = (byte*)_data.Address;
                    for (int k = 0; k < count && reader.ReadValue(ptr2++); ++k) ;
                    break;
                case ComponentType.Short:
                    _data = DataSource.Allocate(count * sizeof(short));
                    short* ptr3 = (short*)_data.Address;
                    for (int k = 0; k < count && reader.ReadValue(ptr3++); ++k) ;
                    break;
                case ComponentType.UShort:
                    _data = DataSource.Allocate(count * sizeof(ushort));
                    ushort* ptr4 = (ushort*)_data.Address;
                    for (int k = 0; k < count && reader.ReadValue(ptr4++); ++k) ;
                    break;
                case ComponentType.Int:
                    _data = DataSource.Allocate(count * sizeof(int));
                    int* ptr5 = (int*)_data.Address;
                    for (int k = 0; k < count && reader.ReadValue(ptr5++); ++k) ;
                    break;
                case ComponentType.UInt:
                    _data = DataSource.Allocate(count * sizeof(uint));
                    uint* ptr6 = (uint*)_data.Address;
                    for (int k = 0; k < count && reader.ReadValue(ptr6++); ++k) ;
                    break;
                case ComponentType.Float:
                    _data = DataSource.Allocate(count * sizeof(float));
                    float* ptr7 = (float*)_data.Address;
                    for (int k = 0; k < count && reader.ReadValue(ptr7++); ++k) ;
                    break;
                case ComponentType.Double:
                    _data = DataSource.Allocate(count * sizeof(double));
                    double* ptr8 = (double*)_data.Address;
                    for (int k = 0; k < count && reader.ReadValue(ptr8++); ++k) ;
                    break;
            }
            return true;
        }

        public bool Integral
        {
            get => _integral;
            set => _integral = value;
        }
        public int Index
        {
            get => _bufferIndex;
            set => _bufferIndex = value;
        }
        public BufferType BufferType
        {
            get => _type;
            set => _type = value;
        }

        public VertexBuffer() : base(EObjectType.Buffer) { }
        public VertexBuffer(
            int index,
            VertexAttribInfo info,
            EBufferTarget target,
            int elementCount,
            ComponentType componentType,
            int componentCount,
            bool normalize,
            bool integral) : base(EObjectType.Buffer)
        {
            _bufferIndex = index;
            _target = target;
            _location = info.GetLocation();
            _name = info.GetAttribName();
            _type = info._type;

            _componentType = componentType;
            _componentCount = componentCount;
            _elementCount = elementCount;
            _normalize = normalize;
            _integral = integral;

            _data = DataSource.Allocate(DataLength);
        }
        public VertexBuffer(
            string name,
            int location,
            EBufferTarget target,
            bool integral) : base(EObjectType.Buffer)
        {
            _bufferIndex = -1;
            _location = location;
            _target = target;
            _name = name;
            _integral = integral;
        }
        public VertexBuffer(
            string name,
            EBufferTarget target,
            bool integral) : base(EObjectType.Buffer)
        {
            _bufferIndex = -1;
            _location = -1;
            _target = target;
            _name = name;
            _integral = integral;
        }
        //public VertexBuffer(
        //    int index,
        //    string name,
        //    int location,
        //    BufferTarget target) : base(GenType.Buffer)
        //{
        //    _index = index;
        //    _location = location;
        //    _target = target;
        //    _name = name;
        //}
        public VertexBuffer(
           int index,
           VertexAttribInfo info,
           EBufferTarget target,
           bool integral) : base(EObjectType.Buffer)
        {
            _bufferIndex = index;
            _target = target;
            _location = info.GetLocation();
            _name = info.GetAttribName();
            _type = info._type;
            _integral = integral;
        }

        public VoidPtr Address => _data.Address;
        public int ComponentCount => _componentCount;
        public int ElementCount => _elementCount;
        public int DataLength => _elementCount * Stride;
        public int Stride => _componentCount * ComponentSize;
        private int ComponentSize
        {
            get
            {
                switch (_componentType)
                {
                    case ComponentType.SByte: return sizeof(sbyte);
                    case ComponentType.Byte: return sizeof(byte);
                    case ComponentType.Short: return sizeof(short);
                    case ComponentType.UShort: return sizeof(ushort);
                    case ComponentType.Int: return sizeof(int);
                    case ComponentType.UInt: return sizeof(uint);
                    case ComponentType.Float: return sizeof(float);
                    case ComponentType.Double: return sizeof(double);
                }
                return -1;
            }
        }
        protected override void OnGenerated()
        {
            Engine.Renderer.InitializeBuffer(this);
        }
        internal const bool MapData = true;
        private void PushData()
        {
            Engine.Renderer.PushBufferData(this);
        }
        public T Get<T>(int offset) where T : struct
        {
            T value = default(T);
            Marshal.PtrToStructure(_data.Address + offset, value);
            return value;
        }
        public void Set<T>(int offset, T value) where T : struct
        {
            Marshal.StructureToPtr(value, _data.Address + offset, true);
        }
        public unsafe Remapper SetDataNumeric<T>(IList<T> list, bool remap = false) where T : struct
        {
            if (typeof(T) == typeof(sbyte))
                _componentType = ComponentType.SByte;
            else if (typeof(T) == typeof(byte))
                _componentType = ComponentType.Byte;
            else if (typeof(T) == typeof(short))
                _componentType = ComponentType.Short;
            else if (typeof(T) == typeof(ushort))
                _componentType = ComponentType.UShort;
            else if (typeof(T) == typeof(int))
                _componentType = ComponentType.Int;
            else if (typeof(T) == typeof(uint))
                _componentType = ComponentType.UInt;
            else if (typeof(T) == typeof(float))
                _componentType = ComponentType.Float;
            else if (typeof(T) == typeof(double))
                _componentType = ComponentType.Double;
            else
                throw new InvalidOperationException("Not a proper numeric data type.");

            //Engine.DebugPrint("\nSetting numeric vertex data for buffer " + Index + " - " + Name);

            _componentCount = 1;
            _normalize = false;
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(list, null);
                _elementCount = remapper.ImplementationLength;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                int elementSize = ComponentSize;
                for (int i = 0; i < remapper.ImplementationLength; ++i)
                {
                    VoidPtr addr = _data.Address[i, stride];
                    T value = list[remapper.ImplementationTable[i]];
                    //Debug.Write(value.ToString() + " ");
                    Marshal.StructureToPtr(value, addr, true);
                }
                //Engine.DebugPrint();
                return remapper;
            }
            else
            {
                _elementCount = list.Count;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                int elementSize = ComponentSize;
                for (int i = 0; i < list.Count; ++i)
                {
                    VoidPtr addr = _data.Address[i, stride];
                    T value = list[i];
                    //Debug.Write(value.ToString() + " ");
                    Marshal.StructureToPtr(value, addr, true);
                }
                //Engine.DebugPrint("\n");
                return null;
            }
        }
        public Remapper SetData<T>(IList<T> list, bool remap = false) where T : IBufferable
        {
            //Engine.DebugPrint("\nSetting vertex data for buffer " + Index + " - " + Name);

            IBufferable d = default(T);
            _componentType = d.ComponentType;
            _componentCount = d.ComponentCount;
            _normalize = d.Normalize;

            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(list, null);
                _elementCount = remapper.ImplementationLength;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                for (int i = 0; i < remapper.ImplementationLength; ++i)
                {
                    IBufferable b = list[remapper.ImplementationTable[i]];
                    //Engine.DebugPrint(b.ToString());
                    b.Write(_data.Address[i, stride]);
                }
                return remapper;
            }
            else
            {
                _elementCount = list.Count;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                for (int i = 0; i < list.Count; ++i)
                {
                    //Engine.DebugPrint(list[i].ToString());
                    list[i].Write(_data.Address[i, stride]);
                }
                return null;
            }
        }
        public Remapper GetData<T>(out T[] array, bool remap = true) where T : IBufferable
        {
            //Engine.DebugPrint("\nGetting vertex data from buffer " + Index + " - " + Name);

            IBufferable d = default(T);
            _componentType = d.ComponentType;
            _componentCount = d.ComponentCount;
            _normalize = d.Normalize;

            int stride = Stride;
            array = new T[_elementCount];
            for (int i = 0; i < _elementCount; ++i)
            {
                T value = default(T);
                value.Read(_data.Address[i, stride]);
                array[i] = value;
            }

            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(array);
                return remapper;
            }
            return null;
        }
        protected override void PreDeleted()
        {
            if (MapData)
                Engine.Renderer.UnmapBufferData(this);
        }

        ~VertexBuffer() { Dispose(false); }
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Destroy();
                }

                //Engine.DebugPrint("Disposing of " + BufferType + " buffer");
                if (_data != null)
                {
                    _data.Dispose();
                    _data = null;
                }
                _vaoId = 0;
                _disposedValue = true;
            }
        }

        public static implicit operator VoidPtr(VertexBuffer b) => b.Address;
        public override string ToString() => _name;
    }
}
