using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Memory;

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
    public class DataBuffer : BaseRenderObject, IDisposable, ISerializablePointer
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

        [TSerialize("Index", NodeType = ENodeType.Attribute)]
        internal int _bufferIndex;
        [TSerialize("BindLocation", NodeType = ENodeType.Attribute)]
        internal int _location;
        [TSerialize("ComponentType", NodeType = ENodeType.Attribute)]
        internal ComponentType _componentType;
        [TSerialize("Normalize", NodeType = ENodeType.Attribute)]
        internal bool _normalize;
        [TSerialize("Integral", NodeType = ENodeType.Attribute)]
        internal bool _integral = false;
        [TSerialize("ComponentCount", NodeType = ENodeType.Attribute)]
        internal int _componentCount;
        [TSerialize("ElementCount", NodeType = ENodeType.Attribute)]
        internal int _elementCount;
        [TSerialize("Type", NodeType = ENodeType.Attribute)]
        internal EBufferType _type = EBufferType.Other;

        [TSerialize("Data", IsElementString = true)]
        internal DataSource _data;

        [CustomSerializeMethod("Data")]
        private object CustomDataSerialize()
        {
            Endian.EOrder prevOrder = Endian.SerializeOrder;
            Endian.SerializeOrder = Endian.EOrder.Little;
            int count = _elementCount * _componentCount;
            object array = null;
            VoidPtr address = _data.Address;
            switch (_componentType)
            {
                case ComponentType.SByte:
                    {
                        sbyte[] values = new sbyte[count];
                        for (int i = 0; i < count; ++i)
                            values[i] = address.ReadSByte();
                        array = values;
                    }
                    break;
                case ComponentType.Byte:
                    {
                        byte[] values = new byte[count];
                        for (int i = 0; i < count; ++i)
                            values[i] = address.ReadByte();
                        array = values;
                    }
                    break;
                case ComponentType.Short:
                    {
                        short[] values = new short[count];
                        for (int i = 0; i < count; ++i)
                            values[i] = address.ReadShort();
                        array = values;
                    }
                    break;
                case ComponentType.UShort:
                    {
                        ushort[] values = new ushort[count];
                        for (int i = 0; i < count; ++i)
                            values[i] = address.ReadUShort();
                        array = values;
                    }
                    break;
                case ComponentType.Int:
                    {
                        int[] values = new int[count];
                        for (int i = 0; i < count; ++i)
                            values[i] = address.ReadInt();
                        array = values;
                    }
                    break;
                case ComponentType.UInt:
                    {
                        uint[] values = new uint[count];
                        for (int i = 0; i < count; ++i)
                            values[i] = address.ReadUInt();
                        array = values;
                    }
                    break;
                case ComponentType.Float:
                    {
                        float[] values = new float[count];
                        for (int i = 0; i < count; ++i)
                            values[i] = address.ReadFloat();
                        array = values;
                    }
                    break;
                case ComponentType.Double:
                    {
                        double[] values = new double[count];
                        for (int i = 0; i < count; ++i)
                            values[i] = address.ReadDouble();
                        array = values;
                    }
                    break;
            }
            Endian.SerializeOrder = prevOrder;
            return array;
        }
        [CustomDeserializeMethod("Data")]
        private void CustomDataDeserialize(SerializeElementContent node)
        {
            Endian.EOrder prevOrder = Endian.SerializeOrder;
            Endian.SerializeOrder = Endian.EOrder.Little;
            int count = _elementCount * _componentCount;
            VoidPtr address;
            switch (_componentType)
            {
                case ComponentType.SByte:
                    {
                        node.GetObjectAs(out sbyte[] values);
                        _data = DataSource.Allocate(count * sizeof(sbyte));
                        address = _data.Address;
                        for (int k = 0; k < count; ++k)
                            address.WriteSByte(values[k]);
                    }
                    break;
                case ComponentType.Byte:
                    {
                        node.GetObjectAs(out byte[] values);
                        _data = DataSource.Allocate(count * sizeof(byte));
                        address = _data.Address;
                        for (int k = 0; k < count; ++k)
                            address.WriteByte(values[k]);
                    }
                    break;
                case ComponentType.Short:
                    {
                        node.GetObjectAs(out short[] values);
                        _data = DataSource.Allocate(count * sizeof(short));
                        address = _data.Address;
                        for (int k = 0; k < count; ++k)
                            address.WriteShort(values[k]);
                    }
                    break;
                case ComponentType.UShort:
                    {
                        node.GetObjectAs(out ushort[] values);
                        _data = DataSource.Allocate(count * sizeof(ushort));
                        address = _data.Address;
                        for (int k = 0; k < count; ++k)
                            address.WriteUShort(values[k]);
                    }
                    break;
                case ComponentType.Int:
                    {
                        node.GetObjectAs(out int[] values);
                        _data = DataSource.Allocate(count * sizeof(int));
                        address = _data.Address;
                        for (int k = 0; k < count; ++k)
                            address.WriteInt(values[k]);
                    }
                    break;
                case ComponentType.UInt:
                    {
                        node.GetObjectAs(out uint[] values);
                        _data = DataSource.Allocate(count * sizeof(uint));
                        address = _data.Address;
                        for (int k = 0; k < count; ++k)
                            address.WriteUInt(values[k]);
                    }
                    break;
                case ComponentType.Float:
                    {
                        node.GetObjectAs(out float[] values);
                        _data = DataSource.Allocate(count * sizeof(float));
                        address = _data.Address;
                        for (int k = 0; k < count; ++k)
                            address.WriteFloat(values[k]);
                    }
                    break;
                case ComponentType.Double:
                    {
                        node.GetObjectAs(out double[] values);
                        _data = DataSource.Allocate(count * sizeof(double));
                        address = _data.Address;
                        for (int k = 0; k < count; ++k)
                            address.WriteDouble(values[k]);
                    }
                    break;
            }
            Endian.SerializeOrder = prevOrder;
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

        public int GetSize()
        {
            return DataLength;
        }
        public void WriteToPointer(VoidPtr address)
        {

        }
        public void ReadFromPointer(VoidPtr address, int size)
        {

        }
    }
}
