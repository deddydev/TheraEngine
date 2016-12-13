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
        public DrawElementsType _elementType;
        public PrimitiveType _type;
        //private Bone[] _utilizedBones;
        //private Shader _vertexShader;
        //private PrimitiveBufferInfo _bufferInfo;
        private Material _material;
        private Bone _singleBind;

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
                    _type = PrimitiveType.Triangles;

                    if (_data._facePoints.Count <= byte.MaxValue)
                    {
                        _elementType = DrawElementsType.UnsignedByte;
                        _indexBuffer.SetDataNumeric(_data.GetFaceIndices().Select(x => (byte)x).ToList());
                    }
                    else if (_data._facePoints.Count <= short.MaxValue)
                    {
                        _elementType = DrawElementsType.UnsignedShort;
                        _indexBuffer.SetDataNumeric(_data.GetFaceIndices().Select(x => (ushort)x).ToList());
                    }
                    else
                    {
                        _elementType = DrawElementsType.UnsignedInt;
                        _indexBuffer.SetDataNumeric(_data.GetFaceIndices());
                    }
                }
            }
        }
        public void SetMaterial(Material material)
        {
            _material = material;
            if (_program != null)
                _program.SetMaterial(material);
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
            Engine.Renderer.UseProgram(_program);
            
            //SetBoneMatrixUniforms();
            //This is a mesh-specific uniform
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ModelMatrix), transform);

            GL.BindVertexArray(BindingId);
            GL.DrawElements(_type, _indexBuffer.ElementCount, _elementType, 0);
            GL.BindVertexArray(0);

            Engine.Renderer.UseProgram(null);
        }
        protected override void OnGenerated()
        {
            _initialized = true;

            _program = new MeshProgram(_material);
            _program.Generate();

            GL.BindVertexArray(BindingId);
            _bindingIds = _data.GenerateBuffers();
            _indexBuffer.Generate();
            GL.BindVertexArray(0);
        }
        protected override void OnDeleted()
        {
            _initialized = false;
            _data.Dispose();
            _indexBuffer.Dispose();
            _program.Destroy();
        }
    }
}
