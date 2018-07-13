using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using System.ComponentModel;
using System.IO;
using TheraEngine.Core.Memory;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models
{
    public enum EBufferTarget
    {
        ArrayBuffer = 34962,
        ElementArrayBuffer = 34963,
        PixelPackBuffer = 35051,
        PixelUnpackBuffer = 35052,
        UniformBuffer = 35345,
        TextureBuffer = 35882,
        TransformFeedbackBuffer = 35982,
        CopyReadBuffer = 36662,
        CopyWriteBuffer = 36663,
        DrawIndirectBuffer = 36671,
        ShaderStorageBuffer = 37074,
        DispatchIndirectBuffer = 37102,
        QueryBuffer = 37266,
        AtomicCounterBuffer = 37568
    }
    public enum EBufferType
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
        public VertexAttribInfo(EBufferType type, int index = 0)
        {
            _type = type;
            _index = index.Clamp(0, GetMaxBuffersForType(type) - 1);
        }

        public EBufferType _type;
        public int _index;

        public static int GetMaxBuffersForType(EBufferType type)
        {
            switch (type)
            {
                case EBufferType.Color:
                    return VertexShaderDesc.MaxColors;
                case EBufferType.TexCoord:
                    return VertexShaderDesc.MaxTexCoords;
                case EBufferType.Other:
                    return VertexShaderDesc.MaxOtherBuffers;
                default:
                    return VertexShaderDesc.MaxMorphs + 1;
            }
        }
        public static string GetAttribName(EBufferType type, int index) => type.ToString() + index.ToString();
        public static int GetLocation(EBufferType type, int index)
        {
            int location = 0;
            for (EBufferType i = 0; i < type; ++i)
                location += GetMaxBuffersForType(i);
            return location + index;
        }
        public string GetAttribName() => GetAttribName(_type, _index);
        public int GetLocation() => GetLocation(_type, _index);
    }
    public enum EBufferUsage
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
    public class DataBuffer : BaseRenderObject, IDisposable
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
            Double  = 10,
        }

        public bool MapData { get; set; } = false;
        public EBufferTarget Target { get; private set; } = EBufferTarget.ArrayBuffer;
        public EBufferUsage Usage { get; set; } = EBufferUsage.StaticDraw;
        internal int _vaoId = 0;

        [TSerialize("Index", XmlNodeType = EXmlNodeType.Attribute)]
        internal int _bufferIndex;
        [TSerialize("BindLocation", XmlNodeType = EXmlNodeType.Attribute)]
        internal int _location;
        [TSerialize("ComponentType", XmlNodeType = EXmlNodeType.Attribute)]
        internal ComponentType _componentType;
        [TSerialize("Normalize", XmlNodeType = EXmlNodeType.Attribute)]
        internal bool _normalize;
        [TSerialize("Integral", XmlNodeType = EXmlNodeType.Attribute)]
        internal bool _integral = false;
        [TSerialize("ComponentCount", XmlNodeType = EXmlNodeType.Attribute)]
        internal int _componentCount;
        [TSerialize("ElementCount", XmlNodeType = EXmlNodeType.Attribute)]
        internal int _elementCount;
        [TSerialize("Type", XmlNodeType = EXmlNodeType.Attribute)]
        internal EBufferType _type = EBufferType.Other;

        [TSerialize("Data", IsXmlElementString = true)]
        internal DataSource _data;

        [CustomXMLSerializeMethod("Data")]
        private unsafe bool CustomDataSerialize(XmlWriter writer, ESerializeFlags flags)
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
        public EBufferType BufferType
        {
            get => _type;
            set => _type = value;
        }

        public DataBuffer() : base(EObjectType.Buffer) { }
        public DataBuffer(
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
            Target = target;
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
        public DataBuffer(
            string name,
            int location,
            EBufferTarget target,
            bool integral) : base(EObjectType.Buffer)
        {
            _bufferIndex = -1;
            _location = location;
            Target = target;
            _name = name;
            _integral = integral;
        }
        public DataBuffer(
            string name,
            EBufferTarget target,
            bool integral) : base(EObjectType.Buffer)
        {
            _bufferIndex = -1;
            _location = -1;
            Target = target;
            _name = name;
            _integral = integral;
        }
        public DataBuffer(
           int index,
           VertexAttribInfo info,
           EBufferTarget target,
           bool integral) : base(EObjectType.Buffer)
        {
            _bufferIndex = index;
            Target = target;
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

        public bool IsMapped { get; internal set; } = false;

        protected override void PostGenerated() => Engine.Renderer.InitializeBuffer(this);
        public void PushData() => Engine.Renderer.PushBufferData(this);
        public void PushSubData(int offset, int length)
            => Engine.Renderer.PushBufferSubData(this, offset, length);
        public void PushSubData()
            => Engine.Renderer.PushBufferSubData(this, 0, DataLength);

        /// <summary>
        /// Reads the struct value at the given offset into the buffer.
        /// Offset is in bytes; NOT relative to the size of the struct.
        /// </summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="offset">The offset into the buffer, in bytes.</param>
        /// <returns>The T value at the given offset.</returns>
        public T Get<T>(int offset) where T : struct
            => (T)Marshal.PtrToStructure(_data.Address + offset, typeof(T));

        /// <summary>
        /// Writes the struct value into the buffer at the given offset.
        /// Offset is in bytes; NOT relative to the size of the struct.
        /// This will not update the data in GPU memory. To do that, call PushData or PushSubData after this call.
        /// </summary>
        /// <typeparam name="T">The type of value to write.</typeparam>
        /// <param name="offset">The offset into the buffer, in bytes.</param>
        /// <param name="value">The value to write.</param>
        public void Set<T>(int offset, T value) where T : struct
            => Marshal.StructureToPtr(value, _data.Address + offset, true);
        
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

        public void SetBlockName(RenderProgram program, string blockName)
        {
            int blockIndex = Engine.Renderer.GetUniformBlockIndex(program.BindingId, blockName);
            SetBlockIndex(blockIndex);
        }
        public void SetBlockIndex(int blockIndex)
        {
            Engine.Renderer.BindBufferBase((EBufferRangeTarget)(int)Target, blockIndex, BindingId);
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
                T value = default;
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

        ~DataBuffer() { Dispose(false); }
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

        public static implicit operator VoidPtr(DataBuffer b) => b.Address;
        public override string ToString() => _name;
    }
}
