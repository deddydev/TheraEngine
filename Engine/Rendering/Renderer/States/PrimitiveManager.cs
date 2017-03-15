using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections;
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
        private Dictionary<int, int> _boneRemap;
        private volatile HashSet<int> _modifiedVertexIndices = new HashSet<int>();
        private volatile HashSet<int> _modifiedBoneIndices = new HashSet<int>();
        private PrimitiveBufferInfo _bufferInfo = new PrimitiveBufferInfo();
        private Material _material;
        internal CPUSkinInfo _cpuSkinInfo;
        private Bone _singleBind;
        internal bool _processingSkinning = false;

        public PrimitiveManager() 
            : base(GenType.VertexArray) { }
        public PrimitiveManager(PrimitiveData data, Material material)
            : base(GenType.VertexArray)
        {
            Data = data;
            _material = material;
        }

        public PrimitiveData Data
        {
            get => _data;
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
            get => _material;
            set => _program?.SetMaterial(_material = value);
        }

        public MeshProgram Program => _program;
        public HashSet<int> ModifiedVertexIndices => _modifiedVertexIndices;
        public HashSet<int> ModifiedBoneIndices => _modifiedBoneIndices;

        private void UpdateBoneInfo(bool set)
        {
            if (Data._influences != null && _cpuSkinInfo != null)
            {
                for (int i = 0; i < _cpuSkinInfo._influences.Length; ++i)
                {
                    var inf = _cpuSkinInfo._influences[i];
                    for (int j = 0; j < inf._weightCount; ++j)
                    {
                        Bone b = inf._bones[j];
                        if (set)
                        {
                            if (!b._influencedInfluences.Contains(inf))
                                b._influencedInfluences.Add(inf);
                        }
                        else
                        {
                            if (b._influencedInfluences.Contains(inf))
                                b._influencedInfluences.Remove(inf);
                        }
                    }
                }
                for (int i = 0; i < Data._facePoints.Count; ++i)
                {
                    var inf = _cpuSkinInfo._influences[Data._facePoints[i]._influenceIndex];
                    FacePoint point = Data._facePoints[i];
                    for (int j = 0; j < inf._weightCount; ++j)
                    {
                        Bone b = inf._bones[j];
                        if (set)
                        {
                            List<int> list;

                            //if (!b._influencedVertices.ContainsKey(BindingId))
                            //    b._influencedVertices.Add(BindingId, list = new List<int>());
                            //else
                                list = b._influencedVertices[BindingId];

                            if (!list.Contains(point.VertexIndex))
                                list.Add(point.VertexIndex);
                        }
                        else
                        {
                            //if (b._influencedVertices.ContainsKey(BindingId))
                            //{
                                List<int> list = b._influencedVertices[BindingId];
                                if (list.Contains(point.VertexIndex))
                                    list.Remove(point.VertexIndex);
                                //if (list.Count == 0)
                                //    b._influencedVertices.Remove(BindingId);
                            //}
                        }
                    }
                }
            }
        }
        public void SkeletonChanged(Skeleton skeleton)
        {
            UpdateBoneInfo(false);
            _modifiedBoneIndices.Clear();
            _data[BufferType.MatrixIds]?.Dispose();
            _data[BufferType.MatrixWeights]?.Dispose();
            if (_utilizedBones != null)
                foreach (Bone b in _utilizedBones)
                    b.RemovePrimitiveManager(this);
            _boneRemap = null;
            if (skeleton != null && _data._influences != null)
            {
                if (_data._utilizedBones == null || _data._utilizedBones.Length == 1)
                {
                    _singleBind = skeleton.BoneNameCache[_data._singleBindBone];
                    _cpuSkinInfo = null;
                    _bufferInfo._boneCount = 1;
                }
                else
                {
                    _utilizedBones = _data._utilizedBones.Select(x => skeleton.BoneNameCache[x]).ToArray();
                    _boneRemap = new Dictionary<int, int>();
                    for (int i = 0; i < _utilizedBones.Length; ++i)
                    {
                        Bone b = _utilizedBones[i];
                        _modifiedBoneIndices.Add(b._index);
                        b.AddPrimitiveManager(this);
                        _boneRemap.Add(b._index, i);
                    }

                    int infCount = _data._influences.Length;
                    if (Engine.Settings.UseIntegerWeightingIds || !Engine.Settings.SkinOnGPU)
                    {
                        if (Engine.Settings.SkinOnGPU)
                        {
                            _cpuSkinInfo = null;
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
                            _data.AddBufferNumeric(matrixIndices.SelectMany(x => new byte[] { (byte)x.X, (byte)x.Y, (byte)x.Z, (byte)x.W }).ToList(), new VertexAttribInfo(BufferType.MatrixIds), false, true);
                            _data.AddBuffer(matrixWeights.ToList(), new VertexAttribInfo(BufferType.MatrixWeights));
                        }
                        else
                            _cpuSkinInfo = new CPUSkinInfo(_data, skeleton);
                    }
                    else
                    {
                        Vec4[] matrixIndices = new Vec4[infCount];
                        Vec4[] matrixWeights = new Vec4[infCount];

                        for (int i = 0; i < infCount; ++i)
                        {
                            matrixIndices[i] = new Vec4();
                            matrixWeights[i] = new Vec4();
                            Influence inf = _data._influences[i];
                            for (int j = 0; j < Influence.MaxWeightCount; ++j)
                            {
                                BoneWeight b = inf.Weights[j];
                                if (b == null)
                                {
                                    matrixIndices[i][j] = 0.0f;
                                    matrixWeights[i][j] = 0.0f;
                                }
                                else
                                {
                                    matrixIndices[i][j] = _data._utilizedBones.IndexOf(b.Bone) + 1;
                                    matrixWeights[i][j] = b.Weight;
                                }
                            }
                        }
                        _cpuSkinInfo = null;
                        _data.AddBuffer(matrixIndices.ToList(), new VertexAttribInfo(BufferType.MatrixIds), false, false);
                        _data.AddBuffer(matrixWeights.ToList(), new VertexAttribInfo(BufferType.MatrixWeights));
                    }
                    _bufferInfo._boneCount = _utilizedBones.Length;
                }
            }
            else
            {
                _bufferInfo._boneCount = 0;
                _utilizedBones = null;
            }
            UpdateBoneInfo(true);
        }
        private void SetSkinningUniforms()
        {
            if (!_bufferInfo.IsWeighted)
                return;

            _processingSkinning = true;
            if (Engine.Settings.SkinOnGPU)
            {
                Engine.Renderer.Uniform(Uniform.BoneMatricesName + "[0]", Matrix4.Identity);
                Engine.Renderer.Uniform(Uniform.BoneMatricesITName + "[0]", Matrix3.Identity);

                //Update modified bone matrix uniforms
                var enumerator = _modifiedBoneIndices.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    int index = enumerator.Current;
                    int remappedIndex = _boneRemap[index];
                    Bone b = _utilizedBones[remappedIndex];
                    //Increase index to account for identity matrix at index 0
                    ++remappedIndex;
                    Engine.Renderer.Uniform(Uniform.BoneMatricesName + "[" + remappedIndex + "]", b.VertexMatrix);
                    Engine.Renderer.Uniform(Uniform.BoneMatricesITName + "[" + remappedIndex + "]", b.VertexMatrixIT.GetRotationMatrix3());
                }
                //Engine.Renderer.Uniform(Uniform.MorphWeightsName, _morphWeights);
                //_modifiedBoneIndices.Clear();
            }
            else
                _cpuSkinInfo.UpdatePNBT(_modifiedVertexIndices.GetEnumerator());
            _processingSkinning = false;
        }

        /// <summary>
        /// Retrieves the linked material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T Parameter<T>(int index) where T : GLVar
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
