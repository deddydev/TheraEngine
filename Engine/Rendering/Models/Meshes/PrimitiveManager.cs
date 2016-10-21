using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class PrimitiveManager
    {
        PrimitiveData _data;
        
        private bool[] _dirty;
        private DataSource _indexBuffer;
        private Primitive _primitives;

        private int _vaoId, _indexBufferId, _programId, _instanceCount;

        public void Initialize()
        {
            _vaoId = GL.GenVertexArray();
            GL.BindVertexArray(_vaoId);

            _data.Initialize();

            _indexBufferId = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferId);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            GL.UseProgram(_programId);
            GL.BindVertexArray(_vaoId);

            _primitives.Render();

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        public void Destroy()
        {
            _data.Destroy();
            GL.DeleteBuffer(_indexBufferId);
            GL.DeleteVertexArray(_vaoId);
        }

        public VoidPtr PreModifyVertices()
        {
            //Map buffers
            return 0;
        }
        public void PostModifyVertices()
        {
            //Unmap buffers
        }
    }

    public class VertexBuffer
    {
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

        public VertexBuffer(int index, int elementCount, ComponentType componentType, int componentCount, bool normalize)
        {
            _index = index;
            _componentType = componentType;
            _componentCount = componentCount;
            _elementCount = elementCount;
            _normalize = normalize;

            _data = DataSource.Allocate(DataLength);
        }

        private ComponentType _componentType;
        private int _componentCount;
        private int _elementCount;
        private bool _normalize;

        private DataSource _data;

        private bool _isDirty;
        private int _index;
        private int _bindingId;

        public bool HasChanged { get { return _isDirty; } set { _isDirty = value; } }
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
        public int Stride { get { return _componentCount * ElementSize; } }
        public int DataLength { get { return _elementCount * Stride; } }

        public VoidPtr Data { get { return _data.Address; } }

        public static implicit operator VoidPtr(VertexBuffer b) { return b.Data; }

        public void Initialize()
        {
            Generate();
            Bind();
            BindVertexAttrib();
            BindShaderLocation();
            PushData();
        }

        public void BindShaderLocation()
        {
            GL.VertexAttribFormat(_index, _componentCount, VertexAttribType.Byte + (int)_componentType, _normalize, 0);
            GL.VertexAttribBinding(_index, _index);
        }

        public void BindVertexAttrib()
        {
            GL.EnableVertexAttribArray(_index);
            GL.VertexAttribPointer(_index, _componentCount, VertexAttribPointerType.Byte + (int)_componentType, _normalize, 0, 0);
        }
        public void Generate() { _bindingId = GL.GenBuffer(); }
        public void Destroy() { GL.DeleteBuffer(_bindingId); }
        public void Bind() { GL.BindBuffer(BufferTarget.ArrayBuffer, _bindingId); }
        public void PushData()
        {
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)_data.Length, _data.Address, BufferUsageHint.StaticDraw);
        }
    }

    public class PrimitiveData
    {
        public List<FacePoint> _facePoints = new List<FacePoint>();
        public List<Triangle> _faces = new List<Triangle>();
        public Dictionary<string, VertexBuffer> _buffers = new Dictionary<string, VertexBuffer>();
        
        public VertexBuffer this[string name]
        {
            get { return _buffers.ContainsKey(name) ? _buffers[name] : null; }
            set
            {
                if (_buffers.ContainsKey(name))
                    _buffers[name] = value;
                else
                    _buffers.Add(name, value);
            }
        }
        public void Initialize()
        {
            foreach (VertexBuffer b in _buffers.Values)
                b?.Initialize();
        }
        public void Destroy()
        {
            foreach (VertexBuffer b in _buffers.Values)
                b?.Destroy();
        }
    }

    public class Primitive
    {
        int _indexCount, _indexOffset;
        //DataSource _indexBuffer;
        DrawElementsType _elementType;
        PrimitiveType _type;

        public Primitive(int elements, int cachedVertexCount, PrimitiveType type)
        {
            _type = type;

            if (cachedVertexCount <= byte.MaxValue)
                _elementType = DrawElementsType.UnsignedByte;
            else if (cachedVertexCount <= short.MaxValue)
                _elementType = DrawElementsType.UnsignedShort;
            else
                _elementType = DrawElementsType.UnsignedInt;

            //_indexBuffer = new DataSource(elements * GetElementSize());
            _indexCount = elements;
        }

        public int GetElementSize()
        {
            return 1 << (((_elementType - DrawElementsType.UnsignedByte) - 1) / 2);
        }

        public unsafe void Render()
        {
            GL.DrawElements(_type, _indexCount, _elementType, _indexOffset);
        }
    }
}
