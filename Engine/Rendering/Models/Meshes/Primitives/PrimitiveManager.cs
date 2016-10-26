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

        private bool[] _dirty;
        private DataSource _indexBuffer;
        private Primitive _primitives;

        private int _vaoId, _indexBufferId, _programId, _instanceCount;

        public void SetPrimitiveData(PrimitiveData data)
        {
            if (_data != null)
            {
                _data.Dispose();
            }
            _data = data;
        }

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
