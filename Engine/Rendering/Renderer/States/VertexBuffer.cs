using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Position        = 0,
        Normal          = 1,
        Binormal        = 2,
        Tangent         = 3,
        Color           = 4,
        TexCoord        = 5,
        MatrixIds       = 6,
        MatrixWeights   = 7,
        Barycentric     = 8,
        Other           = 9,
    }
    public class VertexAttribInfo
    {
        public VertexAttribInfo(BufferType type, int index = 0)
        {
            _type = type;
            _index = index.Clamp(0, VertexBuffer.MaxBufferCountPerType);
        }

        public BufferType _type;
        public int _index;

        public string GetAttribName() => _type.ToString() + _index;
        public int GetLocation()
        {
            if (_type < BufferType.Barycentric)
                return (int)_type * VertexBuffer.MaxBufferCountPerType + _index;
            else if (_type == BufferType.Barycentric)
                return (int)BufferType.Barycentric * VertexBuffer.MaxBufferCountPerType;
            else
                return (int)BufferType.Other * VertexBuffer.MaxBufferCountPerType + _index;
        }
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
        public static readonly int MaxBufferCountPerType = 1;
        public static readonly int BufferTypeCount = 8;
        
        //TransformedPosition, TransformedNormal, TransformedBinormal, TransformedTangent
        //TransformedTexCoord, TransformedColor
        public static readonly int SkinningBufferCount = 6; 
        public static readonly int MaxBufferCount = MaxBufferCountPerType * BufferTypeCount + SkinningBufferCount;
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

        [Serialize("ComponentType", IsXmlAttribute = true)]
        internal ComponentType _componentType;
        [Serialize("Normalize", IsXmlAttribute = true)]
        internal bool _normalize;
        internal BufferUsage _usage = BufferUsage.StaticDraw;
        [Serialize("Data")]
        internal DataSource _data;
        [Serialize("Integral", IsXmlAttribute = true)]
        internal bool _integral = false;
        internal int _index;
        internal int _location, _vaoId = 0;
        [Serialize("ComponentCount", IsXmlAttribute = true)]
        internal int _componentCount;
        [Serialize("ElementCount", IsXmlAttribute = true)]
        internal int _elementCount;
        internal EBufferTarget _target;
        [Serialize("Type", IsXmlAttribute = true)]
        internal BufferType _type = BufferType.Other;

        [CustomXMLSerializeMethod("Data")]
        private unsafe bool CustomDataSerialize(XmlWriter writer)
        {
            string s = "";
            switch (_componentType)
            {
                case ComponentType.SByte:
                    sbyte* ptr1 = (sbyte*)_data.Address;
                    for (int i = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j)
                            s += (*ptr1++).ToString() + " ";
                    break;
                case ComponentType.Byte:
                    byte* ptr2 = (byte*)_data.Address;
                    for (int i = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j)
                            s += (*ptr2++).ToString() + " ";
                    break;
                case ComponentType.Short:
                    short* ptr3 = (short*)_data.Address;
                    for (int i = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j)
                            s += (*ptr3++).ToString() + " ";
                    break;
                case ComponentType.UShort:
                    ushort* ptr4 = (ushort*)_data.Address;
                    for (int i = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j)
                            s += (*ptr4++).ToString() + " ";
                    break;
                case ComponentType.Int:
                    int* ptr5 = (int*)_data.Address;
                    for (int i = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j)
                            s += (*ptr5++).ToString() + " ";
                    break;
                case ComponentType.UInt:
                    uint* ptr6 = (uint*)_data.Address;
                    for (int i = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j)
                            s += (*ptr6++).ToString() + " ";
                    break;
                case ComponentType.Float:
                    float* ptr7 = (float*)_data.Address;
                    for (int i = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j)
                            s += (*ptr7++).ToString() + " ";
                    break;
                case ComponentType.Double:
                    double* ptr8 = (double*)_data.Address;
                    for (int i = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j)
                            s += (*ptr8++).ToString() + " ";
                    break;
            }
            writer.WriteString(s);
            return true;
        }
        [CustomXMLDeserializeMethod("Data")]
        private unsafe bool CustomDataDeserialize(XMLReader reader)
        {
            string[] s = reader.ReadElementString().Split(' ');
            switch (_componentType)
            {
                case ComponentType.SByte:
                    sbyte* ptr1 = (sbyte*)_data.Address;
                    for (int i = 0, k = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j, ++k)
                            *ptr1++ = sbyte.Parse(s[k]);
                    break;
                case ComponentType.Byte:
                    byte* ptr2 = (byte*)_data.Address;
                    for (int i = 0, k = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j, ++k)
                            *ptr2++ = byte.Parse(s[k]);
                    break;
                case ComponentType.Short:
                    short* ptr3 = (short*)_data.Address;
                    for (int i = 0, k = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j, ++k)
                            *ptr3++ = short.Parse(s[k]);
                    break;
                case ComponentType.UShort:
                    ushort* ptr4 = (ushort*)_data.Address;
                    for (int i = 0, k = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j, ++k)
                            *ptr4++ = ushort.Parse(s[k]);
                    break;
                case ComponentType.Int:
                    int* ptr5 = (int*)_data.Address;
                    for (int i = 0, k = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j, ++k)
                            *ptr5++ = int.Parse(s[k]);
                    break;
                case ComponentType.UInt:
                    uint* ptr6 = (uint*)_data.Address;
                    for (int i = 0, k = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j, ++k)
                            *ptr6++ = uint.Parse(s[k]);
                    break;
                case ComponentType.Float:
                    float* ptr7 = (float*)_data.Address;
                    for (int i = 0, k = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j, ++k)
                            *ptr7++ = float.Parse(s[k]);
                    break;
                case ComponentType.Double:
                    double* ptr8 = (double*)_data.Address;
                    for (int i = 0, k = 0; i < _elementCount; ++i)
                        for (int j = 0; j < _componentCount; ++j, ++k)
                            *ptr8++ = double.Parse(s[k]);
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
            get => _index;
            set => _index = value;
        }
        public BufferType BufferType
        {
            get => _type;
            set => _type = value;
        }

        public VertexBuffer(
            int index,
            VertexAttribInfo info,
            EBufferTarget target,
            int elementCount,
            ComponentType componentType,
            int componentCount,
            bool normalize,
            bool integral) : base(GenType.Buffer)
        {
            _index = index;
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
            bool integral) : base(GenType.Buffer)
        {
            _index = -1;
            _location = location;
            _target = target;
            _name = name;
            _integral = integral;
        }
        public VertexBuffer(
            string name,
            EBufferTarget target,
            bool integral) : base(GenType.Buffer)
        {
            _index = -1;
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
           bool integral) : base(GenType.Buffer)
        {
            _index = index;
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

            //Console.WriteLine("\nSetting numeric vertex data for buffer " + Index + " - " + Name);

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
                    //Console.Write(value.ToString() + " ");
                    Marshal.StructureToPtr(value, addr, true);
                }
                //Console.WriteLine();
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
                    //Console.Write(value.ToString() + " ");
                    Marshal.StructureToPtr(value, addr, true);
                }
                //Console.WriteLine("\n");
                return null;
            }
        }
        public Remapper SetData<T>(IList<T> list, bool remap = false) where T : IBufferable
        {
            //Console.WriteLine("\nSetting vertex data for buffer " + Index + " - " + Name);

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
                    //Console.WriteLine(b.ToString());
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
                    //Console.WriteLine(list[i].ToString());
                    list[i].Write(_data.Address[i, stride]);
                }
                return null;
            }
        }
        public Remapper GetData<T>(out T[] array, bool remap = true) where T : IBufferable
        {
            //Console.WriteLine("\nGetting vertex data from buffer " + Index + " - " + Name);
            
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
        public void Dispose()
        {
            Destroy();
            Console.WriteLine("Disposing of " + BufferType + " buffer");
            if (_data != null)
            {
                _data.Dispose();
                _data = null;
            }
            _vaoId = 0;
        }
        ~VertexBuffer() { Dispose(); }
        public static implicit operator VoidPtr(VertexBuffer b) { return b.Address; }
        public override string ToString() { return _name; }
    }
}
