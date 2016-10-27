using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class VertexBuffer : IDisposable
    {
        public const string PositionsName = "Positions";
        public const string NormalsName = "Normals";
        public const string TexCoordName = "TexCoord";
        public const string ColorName = "Color";

        public enum BufferUsage
        {
            StreamDraw = 0,
            StreamRead = 1,
            StreamCopy = 2,
            StaticDraw = 4,
            StaticRead = 5,
            StaticCopy = 6,
            DynamicDraw = 8,
            DynamicRead = 9,
            DynamicCopy = 10
        }
        public enum ComponentType
        {
            SByte = 0,
            Byte = 1,
            Short = 2,
            UShort = 3,
            Int = 4,
            UInt = 5,
            Float = 6,
            Double = 7
        }
        private ComponentType _componentType;
        private int _componentCount;
        private int _elementCount;
        private bool _normalize;

        private DataSource _data;

        private bool _isDirty;
        private int _index;
        private int _bindingId;
        private string _name;
        private BufferTarget _target;

        public VertexBuffer(
            int index,
            string name,
            BufferTarget target,
            int elementCount,
            ComponentType componentType,
            int componentCount,
            bool normalize)
        {
            _index = index;
            _name = name;
            _target = target;

            _componentType = componentType;
            _componentCount = componentCount;
            _elementCount = elementCount;
            _normalize = normalize;

            _data = DataSource.Allocate(DataLength);
        }
        public VertexBuffer(int index, string name, BufferTarget target)
        {
            _index = index;
            _name = name;
            _target = target;
        }

        public int BindingId { get { return _bindingId; } set { _bindingId = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public bool HasChanged { get { return _isDirty; } set { _isDirty = value; } }

        public VoidPtr Data { get { return _data.Address; } }
        public int DataLength { get { return _elementCount * Stride; } }
        public int Stride { get { return _componentCount * ElementSize; } }
        private int ElementSize
        {
            get
            {
                switch (_componentType)
                {
                    case ComponentType.SByte:
                    case ComponentType.Byte:
                        return 1;
                    case ComponentType.Short:
                    case ComponentType.UShort:
                        return 2;
                    case ComponentType.Int:
                    case ComponentType.UInt:
                    case ComponentType.Float:
                        return 4;
                    case ComponentType.Double:
                        return 8;
                }
                return -1;
            }
        }

        public bool IsPositionsBuffer() { return _name == PositionsName; }
        public bool IsNormalsBuffer() { return _name == NormalsName; }
        public bool IsTexCoordBuffer() { return _name.StartsWith(TexCoordName); }
        public bool IsColorBuffer() { return _name.StartsWith(ColorName); }
        public void Initialize()
        {
            Generate();
            Bind();
            BindVertexAttrib();
            PushData();
        }
        /// <summary>
        /// Determines how the currently bound buffer should be read.
        /// Only call during initialization, unless the buffer is changed.
        /// </summary>
        public void BindVertexAttrib()
        {
            GL.EnableVertexAttribArray(_index);
            //VertexAttribPointer is deprecated
            //GL.VertexAttribPointer(_index, _componentCount, VertexAttribPointerType.Byte + (int)_componentType, _normalize, 0, 0);
            GL.VertexAttribFormat(_index, _componentCount, VertexAttribType.Byte + (int)_componentType, _normalize, 0);
            GL.VertexAttribBinding(_index, _index);
        }
        /// <summary>
        /// Creates this buffer. Only call during initialization.
        /// </summary>
        public void Generate() { _bindingId = GL.GenBuffer(); }
        private void DestroyBinding() { GL.DeleteBuffer(_bindingId); }
        /// <summary>
        /// Binds this buffer for modification.
        /// </summary>
        public void Bind() { GL.BindBuffer(_target, _bindingId); }
        /// <summary>
        /// Pushes this buffer's data to the currently bound buffer 
        /// Call Bind() before this to set the current buffer.
        /// </summary>
        public void PushData(BufferUsage usage = BufferUsage.StaticDraw)
        {
            GL.BufferData(_target, (IntPtr)_data.Length, _data.Address, BufferUsageHint.StreamDraw + (int)usage);
        }
        public unsafe Remapper SetDataNumeric<T>(IList<T> list, bool remap = true) where T : struct
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

            _componentCount = 1;
            _normalize = false;
            _isDirty = true;
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(list, null);
                _elementCount = remapper.ImplementationLength;
                int stride = Stride;
                int elementSize = ElementSize;
                for (int i = 0; i < remapper.ImplementationLength; ++i)
                {
                    VoidPtr addr = _data.Address[i, stride];
                    T value = list[remapper.ImplementationTable[i]];
                    System.Runtime.InteropServices.Marshal.StructureToPtr(value, addr, true);
                }
                return remapper;
            }
            else
            {
                _elementCount = list.Count;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                int elementSize = ElementSize;
                for (int i = 0; i < list.Count; ++i)
                {
                    VoidPtr addr = _data.Address[i, stride];
                    T value = list[i];
                    System.Runtime.InteropServices.Marshal.StructureToPtr(value, addr, true);
                }
                return null;
            }
        }
        public Remapper SetData<T>(IList<T> list, bool remap = true) where T : IBufferable
        {
            IBufferable d = default(T);
            _componentType = d.ComponentType;
            _componentCount = d.ComponentCount;
            _normalize = d.Normalize;

            _isDirty = true;

            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(list, null);
                _elementCount = remapper.ImplementationLength;
                int stride = Stride;
                for (int i = 0; i < remapper.ImplementationLength; ++i)
                    list[remapper.ImplementationTable[i]].Write(_data.Address[i, stride]);
                return remapper;
            }
            else
            {
                _elementCount = list.Count;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                for (int i = 0; i < list.Count; ++i)
                    list[i].Write(_data.Address[i, stride]);
                return null;
            }
        }
        public void Dispose()
        {
            DestroyBinding();
            _bindingId = 0;
            _data.Dispose();
            _data = null;
        }
        ~VertexBuffer() { Dispose(); }

        public static implicit operator VoidPtr(VertexBuffer b) { return b.Data; }
    }
}
