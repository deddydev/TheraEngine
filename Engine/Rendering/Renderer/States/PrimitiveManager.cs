using CustomEngine.Rendering.Models.Materials;
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
        private VertexBuffer _indexBuffer, _matrixIndexBuffer, _matrixWeightBuffer;
        private Primitive _triangles;
        private Bone[] _utilizedBones;
        private Shader _vertexShader;
        
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
                    _indexBuffer = new VertexBuffer(_data._buffers.Count + 2, "FaceIndices", BufferTarget.ElementArrayBuffer);
                    _triangles = new Primitive(_data._faces.Count * 3, _data._facePoints.Count, PrimitiveType.Triangles);
                    switch (_triangles._elementType)
                    {
                        case DrawElementsType.UnsignedByte:
                            _indexBuffer.SetDataNumeric(_data.GetFaceIndices().Select(x => (byte)x).ToList());
                            break;
                        case DrawElementsType.UnsignedShort:
                            _indexBuffer.SetDataNumeric(_data.GetFaceIndices().Select(x => (ushort)x).ToList());
                            break;
                        case DrawElementsType.UnsignedInt:
                            _indexBuffer.SetDataNumeric(_data.GetFaceIndices());
                            break;
                    }
                }
            }
        }
        public void SkeletonChanged(Skeleton skeleton)
        {
            _matrixIndexBuffer?.Dispose();
            _matrixWeightBuffer?.Dispose();

            if (skeleton != null)
            {
                _utilizedBones = _data._utilizedBones.Select(x => skeleton.BoneCache[x]).ToArray();

                int infCount = _data._influences.Length;
                IVec4[] matrixIndices = new IVec4[infCount];
                Vec4[] matrixWeights = new Vec4[infCount];

                for (int i = 0; i < infCount; ++i)
                {
                    matrixIndices[i] = new IVec4();
                    matrixWeights[i] = new Vec4();
                    Influence inf = _data._influences[i];
                    for (int j = 0; j < 4; ++j)
                    {
                        BoneWeight b = inf.Weights[j];
                        if (b == null)
                        {
                            matrixIndices[i][j] = 0;
                            matrixWeights[i][j] = 0.0f;
                        }
                        else
                        {
                            matrixIndices[i][j] = _data._utilizedBones.IndexOf(b.Bone) + 1;
                            matrixWeights[i][j] = b.Weight;
                        }
                    }
                }

                _matrixIndexBuffer = new VertexBuffer(_data._buffers.Count, "MatrixIDs", BufferTarget.ArrayBuffer);
                _matrixWeightBuffer = new VertexBuffer(_data._buffers.Count + 1, "MatrixWeights", BufferTarget.ArrayBuffer);

                _matrixIndexBuffer.SetData(matrixIndices);
                _matrixWeightBuffer.SetData(matrixWeights);

                _vertexShader = Shader.WeightedVertexShader(_utilizedBones.Length);
            }
            else
            {
                _matrixIndexBuffer = null;
                _matrixWeightBuffer = null;
            }
        }
        private void SetBoneMatrixUniforms()
        {
            if (_utilizedBones == null)
                return;
            
            List<Matrix4> boneMatrices = new List<Matrix4>() { Matrix4.Identity };
            boneMatrices.AddRange(_utilizedBones.Select(b => b.VertexMatrix));
            Engine.Renderer.Uniform(Uniform.BoneMatricesName, boneMatrices.ToArray());
        }
        public unsafe void Render(Material material, Matrix4 transform)
        {
            if (_data == null)
                return;

            if (!_initialized)
                Generate();

            //TODO: set material and uniforms in render queue and then render ALL meshes that use it
            //order by depth FIRST though
            Engine.Renderer.UseMaterial(material.MaterialId);
            Engine.Renderer.SetCommonUniforms();
            material.SetUniforms();
            SetBoneMatrixUniforms();

            //This is a mesh-specific uniform
            Engine.Renderer.Uniform(Uniform.ModelMatrixName, transform);

            GL.BindVertexArray(BindingId);
            //GL.BindVertexBuffers(0, _data._buffers.Count, _bindingIds, new IntPtr[_data._buffers.Count], _data._buffers.Select(x => x.Stride).ToArray());
            _triangles.Render();
            GL.BindVertexArray(0);

            Engine.Renderer.UseMaterial(0);
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
            _indexBuffer.Generate();
            _indexBuffer.Bind();
            _indexBuffer.PushData();
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
        int _indexCount;
        public DrawElementsType _elementType;
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
