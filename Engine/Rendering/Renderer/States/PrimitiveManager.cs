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

        private MeshProgram _program;
        private PrimitiveData _data;
        //private PrimitiveData _skinningData;
        private VertexBuffer _indexBuffer;
        private Primitive _triangles;
        private Bone[] _utilizedBones;
        //private Shader _vertexShader;
        //private PrimitiveBufferInfo _bufferInfo;

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
                    _indexBuffer = new VertexBuffer("FaceIndices", BufferTarget.ElementArrayBuffer);
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
        //public void SkeletonChanged(Skeleton skeleton)
        //{
        //    _skinningData?.Dispose();
        //    if (skeleton != null)
        //    {
        //        _utilizedBones = _data._utilizedBones.Select(x => skeleton.BoneCache[x]).ToArray();

        //        int infCount = _data._influences.Length;
        //        IVec4[] matrixIndices = new IVec4[infCount];
        //        Vec4[] matrixWeights = new Vec4[infCount];

        //        for (int i = 0; i < infCount; ++i)
        //        {
        //            matrixIndices[i] = new IVec4();
        //            matrixWeights[i] = new Vec4();
        //            Influence inf = _data._influences[i];
        //            for (int j = 0; j < 4; ++j)
        //            {
        //                BoneWeight b = inf.Weights[j];
        //                if (b == null)
        //                {
        //                    matrixIndices[i][j] = 0;
        //                    matrixWeights[i][j] = 0.0f;
        //                }
        //                else
        //                {
        //                    matrixIndices[i][j] = _data._utilizedBones.IndexOf(b.Bone) + 1;
        //                    matrixWeights[i][j] = b.Weight;
        //                }
        //            }
        //        }

        //        _skinningData.AddBuffer(matrixIndices.ToList(), new VertexAttribInfo(BufferType.MatrixIds), false, BufferTarget.ArrayBuffer);
        //        _skinningData.AddBuffer(matrixWeights.ToList(), new VertexAttribInfo(BufferType.MatrixWeights), false, BufferTarget.ArrayBuffer);

        //        _bufferInfo._boneCount = _utilizedBones.Length;
        //    }
        //    else
        //    {
        //        _skinningData = null;
        //        _bufferInfo._boneCount = 0;
        //    }
        //}
        //private void SetBoneMatrixUniforms()
        //{
        //    if (_utilizedBones == null)
        //        return;
            
        //    List<Matrix4> positionMatrices = new List<Matrix4>() { Matrix4.Identity };
        //    List<Matrix3> normalMatrices = new List<Matrix3>() { Matrix3.Identity };

        //    foreach (Bone b in _utilizedBones)
        //    {
        //        positionMatrices.Add(b.VertexMatrix);
        //        normalMatrices.Add(b.VertexMatrix.GetRotationMatrix3());
        //    }
            
        //    Engine.Renderer.Uniform(Uniform.PositionMatricesName, positionMatrices.ToArray());
        //    Engine.Renderer.Uniform(Uniform.NormalMatricesName, normalMatrices.ToArray());
        //}
        public unsafe void Render(Matrix4 transform)
        {
            if (_data == null)
                return;

            if (!_initialized)
                Generate();

            //TODO: set material and uniforms in render queue and then render ALL meshes that use it
            //order by depth FIRST though
            Engine.Renderer.UseProgram(_program.BindingId);

            Engine.Renderer.SetCommonUniforms();
            //SetBoneMatrixUniforms();
            //This is a mesh-specific uniform
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ModelMatrix), transform);

            GL.BindVertexArray(BindingId);
            _triangles.Render();
            GL.BindVertexArray(0);

            Engine.Renderer.UseProgram(NullBindingId);
        }
        protected override void OnGenerated()
        {
            _initialized = true;
            
            GL.BindVertexArray(BindingId);
            _bindingIds = _data.GenerateBuffers();
            _indexBuffer.Generate();
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
    public class MeshProgram : BaseRenderState
    {
        public Shader[] _shaders;

        public MeshProgram() : base(GenType.Program) { }

        public void SetShaders(params Shader[] shaders) { _shaders = shaders; }
        public void Compile() { Generate(); }
        protected override int CreateObject()
        {
            int[] ids = _shaders.Select(x => x.Compile()).ToArray();
            int id = Engine.Renderer.GenerateProgram(ids);
            return id;
        }
        protected override void OnGenerated()
        {

        }
        protected override void OnDeleted()
        {

        }
        public void SetUniforms()
        {

        }
    }
}
