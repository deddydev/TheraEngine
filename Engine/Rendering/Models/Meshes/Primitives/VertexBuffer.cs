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

        public bool IsPositionsBuffer() { return _name == PositionsName; }
        public bool IsNormalsBuffer() { return _name == NormalsName; }
        public bool IsTexCoordBuffer() { return _name.StartsWith(TexCoordName); }
        public bool IsColorBuffer() { return _name.StartsWith(ColorName); }

        public VertexBuffer(int index, string name, int elementCount, ComponentType componentType, int componentCount, bool normalize)
        {
            _index = index;
            _componentType = componentType;
            _componentCount = componentCount;
            _elementCount = elementCount;
            _normalize = normalize;
            _name = name;

            _data = DataSource.Allocate(DataLength);
        }
        public VertexBuffer(int index, string name)
        {
            _index = index;
            _name = name;
        }

        public Remapper SetData<T>(IList<T> list) where T : IBufferable
        {
            Remapper remapper = new Remapper();
            remapper.Remap(list, null);

            _elementCount = remapper.ImplementationLength;

            IBufferable d = default(T);
            _componentType = d.ComponentType;
            _componentCount = d.ComponentCount;
            _normalize = d.Normalize;

            _data = DataSource.Allocate(DataLength);

            int stride = Stride;
            for (int i = 0; i < remapper.ImplementationLength; ++i)
                list[remapper.ImplementationTable[i]].Write(_data.Address[i, stride]);

            return remapper;
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

        public string Name { get { return _name; } set { _name = value; } }
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
        public void Dispose()
        {
            _data.Dispose();
            _data = null;
        }
        ~VertexBuffer() { Dispose(); }
    }
}
