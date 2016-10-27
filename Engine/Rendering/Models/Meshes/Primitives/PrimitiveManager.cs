using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    /// <summary>
    /// Used to render raw primitive data.
    /// </summary>
    public class PrimitiveManager
    {
        PrimitiveData _data;

        public int[] _bindingIds;
        public IntPtr[] _offsets;
        public int[] _strides;

        VertexBuffer _indexBuffer;
        private Primitive _triangles;

        private int _vaoId;

        public PrimitiveManager() { }

        public void SetPrimitiveData(PrimitiveData data)
        {
            if (_data != null)
                Destroy();
            if ((_data = data) != null)
            {
                _bindingIds = _data._buffers.Select(x => x.BindingId).ToArray();
                Initialize();
            }
        }

        public void Initialize()
        {
            _vaoId = GL.GenVertexArray();
            GL.BindVertexArray(_vaoId);

            _data.Initialize();

            _triangles = new Primitive(
                _data._faces.Count * 3, 
                _data._facePoints.Count, 
                PrimitiveType.Triangles);

            _indexBuffer = new VertexBuffer(_data._buffers.Count, "IndexBuffer", BufferTarget.ElementArrayBuffer);
            _indexBuffer.SetData(_data.GetFaceIndices(), false);

            GL.BindVertexArray(0);
        }

        public void Render(int programId)
        {
            if (_data == null)
                return;

            GL.UseProgram(programId);
            GL.BindVertexArray(_vaoId);

            GL.BindVertexBuffers(0, _data._buffers.Count, _bindingIds, new IntPtr[_data._buffers.Count], _data._buffers.Select(x => x.Stride).ToArray());
            _triangles.Render();

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        public void Destroy()
        {
            _triangles = null;
            _data.Dispose();
            _indexBuffer.Dispose();
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

    public class Primitive
    {
        int _indexCount;//, _indexOffset;
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
            GL.DrawElements(_type, _indexCount, _elementType, 0);
        }
    }
}
