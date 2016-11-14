﻿using CustomEngine.Rendering.Models.Materials;
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
    public class PrimitiveManager : BaseRenderState
    {
        public int[] _bindingIds;
        public IntPtr[] _offsets;
        public int[] _strides;

        private PrimitiveData _data;
        private VertexBuffer _indexBuffer;
        private Primitive _triangles;
        
        private bool _initialized = false;

        public PrimitiveManager() : base(GenType.VertexArray) { }

        public PrimitiveData Data
        {
            get { return _data; }
            set
            {
                Destroy();
                if (_indexBuffer != null)
                {
                    _indexBuffer.Dispose();
                    _indexBuffer = null;
                }
                _data = value;
                if (_data != null)
                {
                    _indexBuffer = new VertexBuffer(_data._buffers.Count, "IndexBuffer", BufferTarget.ElementArrayBuffer);
                    _indexBuffer.SetDataNumeric(_data.GetFaceIndices(), false);
                }
            }
        }

        public void Render(Material material, Matrix4 transform)
        {
            if (_data == null)
                return;

            if (!_initialized)
                Generate();

            Engine.Renderer.UseMaterial(material.BindingId);

            Engine.Renderer.SetCommonUniforms();
            Engine.Renderer.Uniform(Uniform.ModelMatrixUniform, transform);
            material.SetUniforms();
            
            GL.BindVertexArray(BindingId);

            GL.BindVertexBuffers(0, _data._buffers.Count, _bindingIds, new IntPtr[_data._buffers.Count], _data._buffers.Select(x => x.Stride).ToArray());
            _triangles.Render();

            GL.BindVertexArray(0);

            Engine.Renderer.UseMaterial(0);

            ErrorCode code = GL.GetError();
            if (code != ErrorCode.NoError)
                Console.WriteLine(code);
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

        protected override void OnGenerated()
        {
            _initialized = true;
            
            GL.BindVertexArray(BindingId);

            _bindingIds = _data.Initialize();
            _triangles = new Primitive(
                _data._faces.Count * 3,
                _data._facePoints.Count,
                PrimitiveType.Triangles);

            _indexBuffer.Initialize();

            GL.BindVertexArray(0);
        }
        protected override void OnDeleted()
        {
            _initialized = false;
            _triangles = null;
            _data.Dispose();
            _indexBuffer.Dispose();
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
            return 1 << (((_elementType - DrawElementsType.UnsignedByte) - 1) >> 1);
        }

        public unsafe void Render()
        {
            GL.DrawElements(_type, _indexCount, _elementType, 0);
        }
    }
}
