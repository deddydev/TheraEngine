using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public enum EDrawElementType
    {
        Byte    = 0,
        UShort  = 2,
        UInt    = 4,
    }
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
        internal VertexBuffer _indexBuffer;
        public EDrawElementType _elementType;
        private Bone[] _utilizedBones;
        private PrimitiveBufferInfo _bufferInfo = new PrimitiveBufferInfo();
        private Material _material;
        private CPUSkinInfo _cpuSkinInfo;

        public PrimitiveManager() : base(GenType.VertexArray) { }
        public PrimitiveManager(PrimitiveData data, Material material) : base(GenType.VertexArray)
        {
            Data = data;
            _material = material;
        }

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
                    _indexBuffer = new VertexBuffer("FaceIndices", EBufferTarget.DrawIndices, true);
                    //TODO: primitive restart will use MaxValue for restart id
                    if (_data._facePoints.Count < byte.MaxValue)
                    {
                        _elementType = EDrawElementType.Byte;
                        _indexBuffer.SetDataNumeric(_data.GetIndices().Select(x => (byte)x).ToList());
                    }
                    else if (_data._facePoints.Count < short.MaxValue)
                    {
                        _elementType = EDrawElementType.UShort;
                        _indexBuffer.SetDataNumeric(_data.GetIndices().Select(x => (ushort)x).ToList());
                    }
                    else
                    {
                        _elementType = EDrawElementType.UInt;
                        _indexBuffer.SetDataNumeric(_data.GetIndices());
                    }
                }
            }
        }
        public Material Material
        {
            get { return _material; }
            set
            {
                _material = value;
                _program?.SetMaterial(_material);
            }
        }

        public MeshProgram Program { get { return _program; } }

        public void SkeletonChanged(Skeleton skeleton)
        {
            _data[BufferType.MatrixIds]?.Dispose();
            _data[BufferType.MatrixWeights]?.Dispose();
            if (skeleton != null && _data._influences != null)
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
                    for (int j = 0; j < Influence.MaxWeightCount; ++j)
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

                _positionMatrices = new Matrix4[_utilizedBones.Length + 1];
                _normalMatrices = new Matrix3[_utilizedBones.Length + 1];
                _positionMatrices[0] = Matrix4.Identity;
                _normalMatrices[0] = Matrix3.Identity;

                RegenerateSkinningMatries();

                if (Engine._engineSettings.File.SkinOnGPU)
                {
                    _cpuSkinInfo = null;
                    _data.AddBuffer(matrixIndices.ToList(), new VertexAttribInfo(BufferType.MatrixIds), false, true);
                    _data.AddBuffer(matrixWeights.ToList(), new VertexAttribInfo(BufferType.MatrixWeights));
                }
                else
                {
                    _cpuSkinInfo = new CPUSkinInfo(_data, matrixWeights, matrixIndices);
                }
                _bufferInfo._boneCount = _utilizedBones.Length;
            }
            else
            {
                _bufferInfo._boneCount = 0;
                _utilizedBones = null;
            }
        }
        private void RegenerateSkinningMatries()
        {
            for (int i = 1; i < _utilizedBones.Length + 1; ++i)
            {
                Bone b = _utilizedBones[i - 1];
                _positionMatrices[i] = b.VertexMatrix;
                _normalMatrices[i] = b.VertexMatrixIT.GetRotationMatrix3();
            }
        }
        Matrix4[] _positionMatrices;
        Matrix3[] _normalMatrices;
        private void SetSkinningUniforms()
        {
            if (!_bufferInfo.IsWeighted)
                return;

            if (Engine._engineSettings.File.SkinOnGPU)
            {
                Engine.Renderer.Uniform(Uniform.BoneMatricesName, _positionMatrices);
                Engine.Renderer.Uniform(Uniform.BoneMatricesITName, _normalMatrices);
                //Engine.Renderer.Uniform(Uniform.MorphWeightsName, _morphWeights);
            }
            else
            {
                //RegenerateSkinningMatries();
                //_cpuSkinInfo.UpdatePNBT(_positionMatrices, _normalMatrices);
            }
        }

        /// <summary>
        /// Retrieves the linked material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T GetParameter<T>(int index) where T : GLVar
        {
            if (Program == null)
                Generate();
            if (index >= 0 && index < Program.Parameters.Length)
                return Program.Parameters[index] as T;
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// Retrieves the linked material's uniform parameter with the given name.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T GetParameter<T>(string name) where T : GLVar
        {
            if (Program == null)
                Generate();
            return Program.Parameters.FirstOrDefault(x => x.Name == name) as T;
        }

        public unsafe void Render(Matrix4 modelMatrix)
        {
            //TODO: don't invert, transpose, and get rotation matrix here
            Render(modelMatrix, modelMatrix.GetRotationMatrix3());//modelMatrix.Inverted().Transposed().GetRotationMatrix3());
        }
        public unsafe void Render(Matrix4 modelMatrix, Matrix3 normalMatrix)
        {
            if (_data == null)
                return;

            if (!IsActive)
                Generate();

            if (_program == null)
                return;

            Engine.Renderer.UseProgram(_program);
            Engine.Renderer.Cull(_data.Culling);
            
            SetSkinningUniforms();
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ModelMatrix), modelMatrix);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.NormalMatrix), normalMatrix);
            
            Engine.Renderer.RenderPrimitiveManager(this, false);
            Engine.Renderer.UseProgram(null);
        }
        protected override void OnGenerated()
        {
            _program = new MeshProgram(_material, _bufferInfo);
            _program.Generate();

            Engine.Renderer.BindPrimitiveManager(this);
            _bindingIds = _data.GenerateBuffers(BindingId);
            _indexBuffer._vaoId = BindingId;
            _indexBuffer.Generate();
            Engine.Renderer.LinkRenderIndices(this, _indexBuffer);
            Engine.Renderer.BindPrimitiveManager(null);
        }
        protected override void PostDeleted()
        {
            _data.Dispose();
            _indexBuffer.Dispose();
            _program.Destroy();
        }
    }
}
