﻿using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
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
    public interface IPrimitiveManager
    {
        int BindingId { get; }
        HashSet<int> ModifiedBoneIndicesRendering { get; }
        HashSet<int> ModifiedVertexIndicesRendering { get; }
        HashSet<int> ModifiedBoneIndicesUpdating { get; }
        HashSet<int> ModifiedVertexIndicesUpdating { get; }
        PrimitiveData Data { get; set; }
        DataBuffer IndexBuffer { get; }
        TMaterial Material { get; set; }
        EDrawElementType ElementType { get; }

        void SwapModifiedBuffers();
        void SkeletonChanged(ISkeleton skeleton);
        T2 Parameter<T2>(int index) where T2 : ShaderVar;
        T2 Parameter<T2>(string name) where T2 : ShaderVar;
        void Render();
        void Render(int instances);
        void Render(Matrix4 modelMatrix, int instances = 1);
        void Render(Matrix4 modelMatrix, TMaterial material, int instances = 1);
        void Render(Matrix4 modelMatrix, Matrix3 normalMatrix, int instances = 1);
        void Render(Matrix4 modelMatrix, Matrix3 normalMatrix, TMaterial material, int instances = 1);
    }
    public delegate void DelPrimManagerSettingUniforms(RenderProgram vertexProgram, RenderProgram materialProgram);
    /// <summary>
    /// Used to render raw primitive data.
    /// </summary>
    public class PrimitiveManager : BaseRenderObject, IPrimitiveManager
    {
        /// <summary>
        /// Subscribe to this event to send your own uniforms to the material.
        /// </summary>
        public event DelPrimManagerSettingUniforms SettingUniforms;

        //Vertex buffer information
        private int[] _bindingIds;
        private IntPtr[] _offsets;
        private int[] _strides;
        private PrimitiveData _data;
        private ProgramPipeline _pipeline; //Used to connect the material shader(s) to the vertex shader
        private RenderProgram _vertexProgram; //The autogenerated vertex shader program
        internal GLSLScript _vertexShader;

        //Skeleton information
        private IBone _singleBind;
        private IBone[] _utilizedBones;
        private bool _remake = false;
        private Dictionary<int, int> _boneRemap;
        private bool _allowRender = true;
        private CPUSkinInfo _cpuSkinInfo; //Only used in CPU skinning mode
        //private bool _processingSkinning = false;

        public PrimitiveManager() : base(EObjectType.VertexArray) { }
        public PrimitiveManager(PrimitiveData data, TMaterial material) : this()
        {
            if (Engine.Settings.AllowShaderPipelines)
                _pipeline = new ProgramPipeline();
            Material = material;
            Data = data;
        }

        /// <summary>
        /// Determines how to use the results of the <see cref="ConditionalRenderQuery"/>.
        /// </summary>
        public EConditionalRenderType ConditionalRenderType { get; set; } = EConditionalRenderType.QueryNoWait;
        /// <summary>
        /// A render query that is used to determine if this mesh should be rendered or not.
        /// </summary>
        public RenderQuery ConditionalRenderQuery { get; set; } = null;

        public VertexShaderDesc BufferInfo { get; set; }
        public int Instances { get; set; } = 1;

        //TODO: move vertex buffer allocations out of PrimitiveData and into this class, so the original PrimitiveData can be disposed of.
        public PrimitiveData Data
        {
            get => _data;
            set
            {
                Destroy();
                if (IndexBuffer != null)
                {
                    IndexBuffer.Dispose();
                    IndexBuffer = null;
                }
                _data = value;
                if (_data != null)
                {
                    BufferInfo = _data.BufferInfo;
                    _data.BufferInfoChanged += OnBufferInfoChanged;
                    BufferInfo.Changed += OnBufferInfoChanged;
                    OnBufferInfoChanged();
                    
                    IndexBuffer = new DataBuffer("FaceIndices", EBufferTarget.ElementArrayBuffer, true);
                    //TODO: primitive restart will use MaxValue for restart id
                    if (_data._facePoints.Count < byte.MaxValue)
                    {
                        ElementType = EDrawElementType.Byte;
                        IndexBuffer.SetDataNumeric(_data.GetIndices().Select(x => (byte)x).ToList());
                    }
                    else if (_data._facePoints.Count < short.MaxValue)
                    {
                        ElementType = EDrawElementType.UShort;
                        IndexBuffer.SetDataNumeric(_data.GetIndices().Select(x => (ushort)x).ToList());
                    }
                    else
                    {
                        ElementType = EDrawElementType.UInt;
                        IndexBuffer.SetDataNumeric(_data.GetIndices());
                    }
                }
                else
                    BufferInfo = null;
            }
        }

        private VertexShaderGenerator VtxShaderGen { get; } = new VertexShaderGenerator();
        private void OnBufferInfoChanged()
        {
            _vertexShader = VtxShaderGen.Generate(BufferInfo, Material, false, false, false);
            if (Engine.Settings.AllowShaderPipelines)
            {
                if (_vertexProgram != null && _vertexProgram.IsActive)
                    _vertexProgram.Destroy();
                
                _vertexProgram = new RenderProgram(_vertexShader);
                //_vertexProgram.Generate();
            }
        }

        public TMaterial Material { get; set; }
        public TMaterial GetRenderMaterial()
        {
            if (Material?.Program != null && Material.Program.IsValid)
                return Material;
            else
                return TMaterial.InvalidMaterial;
        }

        public RenderProgram VertexProgram
        {
            get
            {
                if (!Engine.Settings.AllowShaderPipelines)
                    return null;

                if (!IsActive)
                    Generate();

                return _vertexProgram;
            }
        }

        [Browsable(false)]
        public DataBuffer IndexBuffer { get; private set; }
        public EDrawElementType ElementType { get; private set; }
        [Browsable(false)]
        public RenderProgram VertexFragProgram { get; private set; }

        /// <summary>
        /// All vertices that have changed and are ready for render.
        /// Only used by CPU skinning.
        /// </summary>
        [Browsable(false)]
        public HashSet<int> ModifiedVertexIndicesRendering => _modifiedVertexIndicesRendering;
        /// <summary>
        /// All bones that have changed and are ready for render.
        /// Only used by GPU skinning.
        /// </summary>
        [Browsable(false)]
        public HashSet<int> ModifiedBoneIndicesRendering => _modifiedBoneIndicesRendering;
        /// <summary>
        /// All vertices that are currently changing.
        /// Only used by CPU skinning.
        /// </summary>
        [Browsable(false)]
        public HashSet<int> ModifiedVertexIndicesUpdating => _modifiedVertexIndicesUpdating;
        /// <summary>
        /// All bones that are currently changing.
        /// Only used by GPU skinning.
        /// </summary>
        [Browsable(false)]
        public HashSet<int> ModifiedBoneIndicesUpdating => _modifiedBoneIndicesUpdating;

        private HashSet<int> _modifiedVertexIndicesRendering = new HashSet<int>();
        private HashSet<int> _modifiedBoneIndicesRendering = new HashSet<int>();
        private HashSet<int> _modifiedVertexIndicesUpdating = new HashSet<int>();
        private HashSet<int> _modifiedBoneIndicesUpdating = new HashSet<int>();

        public void SwapModifiedBuffers()
        {
            THelpers.Swap(ref _modifiedVertexIndicesRendering, ref _modifiedVertexIndicesUpdating);
            THelpers.Swap(ref _modifiedBoneIndicesRendering, ref _modifiedBoneIndicesUpdating);
            _modifiedBoneIndicesUpdating.Clear();
            _modifiedVertexIndicesUpdating.Clear();
        }

        private void UpdateBoneInfo(bool set)
        {
            _allowRender = set;
            if (Data._influences != null && _cpuSkinInfo != null)
            {
                for (int i = 0; i < _cpuSkinInfo._influences.Length; ++i)
                {
                    var inf = _cpuSkinInfo._influences[i];
                    for (int j = 0; j < inf._weightCount; ++j)
                    {
                        IBone b = inf._bones[j];
                        if (set)
                        {
                            if (!b.InfluencedInfluences.Contains(inf))
                                b.InfluencedInfluences.Add(inf);
                        }
                        else
                        {
                            if (b.InfluencedInfluences.Contains(inf))
                                b.InfluencedInfluences.Remove(inf);
                        }
                    }
                }
                for (int i = 0; i < Data._facePoints.Count; ++i)
                {
                    var inf = _cpuSkinInfo._influences[Data._facePoints[i].InfluenceIndex];
                    FacePoint point = Data._facePoints[i];
                    for (int j = 0; j < inf._weightCount; ++j)
                    {
                        IBone b = inf._bones[j];
                        if (set)
                        {
                            List<int> list;

                            //if (!b._influencedVertices.ContainsKey(BindingId))
                            //    b._influencedVertices.Add(BindingId, list = new List<int>());
                            //else
                                list = b.InfluencedVertices[BindingId].Item2;

                            if (!list.Contains(point.Index))
                                list.Add(point.Index);
                        }
                        else
                        {
                            //if (b._influencedVertices.ContainsKey(BindingId))
                            //{
                            List<int> list = b.InfluencedVertices[BindingId].Item2;
                                if (list.Contains(point.Index))
                                    list.Remove(point.Index);
                                //if (list.Count == 0)
                                //    b._influencedVertices.Remove(BindingId);
                            //}
                        }
                    }
                }
            }
        }
        public void SkeletonChanged(ISkeleton skeleton)
        {
            UpdateBoneInfo(false);
            
            _modifiedBoneIndicesUpdating.Clear();
            var matrixIdsBuffer = _data[EBufferType.MatrixIds];
            if (matrixIdsBuffer != null)
            {
                matrixIdsBuffer.Destroy();
                _data.Buffers.RemoveAt(_data.Buffers.Count - 1);
            }
            var matrixWeightsBuffer = _data[EBufferType.MatrixIds];
            if (matrixWeightsBuffer != null)
            {
                matrixWeightsBuffer.Destroy();
                _data.Buffers.RemoveAt(_data.Buffers.Count - 1);
            }
            _boneMatrixBuffer?.Dispose();
            _boneMatrixBuffer = null;

            if (_utilizedBones != null)
                foreach (IBone b in _utilizedBones)
                {
                    b.RemovePrimitiveManager(this);
                    b.Renamed -= B_Renamed;
                }

            if (_singleBind != null)
            {
                _singleBind.Renamed -= _singleBind_Renamed;
                _singleBind = null;
            }

            _boneRemap = null;
            if (skeleton != null)
            {
                if (_data._utilizedBones is null || _data._utilizedBones.Length <= 1)
                {
                    if (!string.IsNullOrEmpty(_data.SingleBindBone) &&
                        skeleton.BoneNameCache.ContainsKey(_data.SingleBindBone))
                    {
                        _singleBind = skeleton.BoneNameCache[_data.SingleBindBone];
                        _singleBind.Renamed += _singleBind_Renamed;
                    }

                    _cpuSkinInfo = null;
                }
                else if (_data._influences != null && _data._influences.Length > 0)
                {
                    int facePointCount = _data._facePoints.Count;
                    if (Engine.Settings.UseIntegerWeightingIds || !Engine.Settings.SkinOnGPU)
                    {
                        if (Engine.Settings.SkinOnGPU)
                        {
                            _cpuSkinInfo = null;
                            IVec4[] matrixIndices = new IVec4[facePointCount];
                            Vec4[] matrixWeights = new Vec4[facePointCount];

                            for (int i = 0; i < facePointCount; ++i)
                            {
                                matrixIndices[i] = new IVec4();
                                matrixWeights[i] = new Vec4();
                                InfluenceDef inf = _data._influences[_data._facePoints[i].InfluenceIndex];
                                for (int j = 0; j < InfluenceDef.MaxWeightCount; ++j)
                                {
                                    BoneWeight weight = inf.Weights[j];
                                    int index;
                                    if (weight is null || (index = _data._utilizedBones.IndexOf(weight.Bone)) < 0)
                                    {
                                        matrixIndices[i][j] = 0;
                                        matrixWeights[i][j] = 0.0f;
                                    }
                                    else
                                    {
                                        matrixIndices[i][j] = index + 1;
                                        matrixWeights[i][j] = weight.Weight;
                                    }
                                }
                            }

                            _data.AddBuffer(matrixIndices, new VertexAttribInfo(EBufferType.MatrixIds), false, true);
                            _data.AddBuffer(matrixWeights, new VertexAttribInfo(EBufferType.MatrixWeights));

                            _remake = true;
                            //Destroy();
                            //Generate();
                        }
                        else
                            _cpuSkinInfo = new CPUSkinInfo(_data, skeleton);
                    }
                    else
                    {
                        Vec4[] matrixIndices = new Vec4[facePointCount];
                        Vec4[] matrixWeights = new Vec4[facePointCount];

                        for (int i = 0; i < facePointCount; ++i)
                        {
                            matrixIndices[i] = new Vec4();
                            matrixWeights[i] = new Vec4();
                            InfluenceDef inf = _data._influences[_data.FacePoints[i].InfluenceIndex];
                            for (int j = 0; j < InfluenceDef.MaxWeightCount; ++j)
                            {
                                BoneWeight weight = inf.Weights[j];
                                int index;
                                if (weight is null || (index = _data.UtilizedBones.IndexOf(weight.Bone)) < 0)
                                {
                                    matrixIndices[i][j] = 0.0f;
                                    matrixWeights[i][j] = 0.0f;
                                }
                                else
                                {
                                    matrixIndices[i][j] = index + 1;
                                    matrixWeights[i][j] = weight.Weight;
                                }
                            }
                        }
                        _cpuSkinInfo = null;

                        _data.AddBuffer(matrixIndices, new VertexAttribInfo(EBufferType.MatrixIds), false, false);
                        _data.AddBuffer(matrixWeights, new VertexAttribInfo(EBufferType.MatrixWeights));

                        _remake = true;
                        //Destroy();
                        //Generate();
                    }

                    _utilizedBones = _data._utilizedBones.Select(x => skeleton.BoneNameCache[x]).ToArray();
                    foreach (Bone b in _utilizedBones)
                        b.Renamed += B_Renamed;
                    _boneMatrixBuffer = new DataBuffer("BoneMatrices", EBufferTarget.UniformBuffer, false)
                    {
                        MapData = false,
                        Usage = EBufferUsage.DynamicDraw,
                    };
                    List<Matrix4> matrices = _utilizedBones.Select(x => x.VertexMatrix).ToList();
                    matrices.Insert(0, Matrix4.Identity);
                    _boneMatrixBuffer.SetData(matrices, false);
                    _remake = true;
                    _boneRemap = new Dictionary<int, int>();
                    for (int i = 0; i < _utilizedBones.Length; ++i)
                    {
                        IBone b = _utilizedBones[i];

                        _modifiedBoneIndicesUpdating.Add(b.Index);

                        //b.AddPrimitiveManager(this);
                        _boneRemap.Add(b.Index, i);
                    }
                }
            }
            else
            {
                _utilizedBones = null;
            }
            UpdateBoneInfo(true);
            SwapModifiedBuffers();
        }

        private void B_Renamed(TObject node, string oldName)
        {
            if (node is Bone b)
            {
                int index = _utilizedBones.IndexOf(b);
                if (_utilizedBones.IndexInRange(index))
                    _utilizedBones[index] = b;
            }
        }

        private void _singleBind_Renamed(TObject node, string oldName)
        {
            _data.SingleBindBone = node.Name;
        }

        private DataBuffer _boneMatrixBuffer;
        private void SetSkinningUniforms(RenderProgram program)
        {
            if (!BufferInfo.IsWeighted)
                return;
            
            if (Engine.Settings.SkinOnGPU)
            {
                if (_boneMatrixBuffer != null)
                {
                    if (_modifiedBoneIndicesRendering.Count > 0)
                    {
                        Matrix4 vtxMtx;
                        int boneIndex;
                        foreach (int i in _modifiedBoneIndicesRendering)
                        {
                            boneIndex = _boneRemap[i];
                            vtxMtx = _utilizedBones[boneIndex].VertexMatrix;

                            //Increment the bone index for all subsequent data calculations 
                            //to account for the identity matrix at index 0
                            ++boneIndex;

                            _boneMatrixBuffer.Set(boneIndex, vtxMtx);
                        }
                        //Engine.Renderer.Uniform(Uniform.MorphWeightsName, _morphWeights);
                    }

                    _boneMatrixBuffer.SetBlockName(program, "Bones");
                    _boneMatrixBuffer.PushSubData(0, _boneMatrixBuffer.DataLength);
                }
            }
            else
            {
                _cpuSkinInfo?.UpdatePNBT(_modifiedVertexIndicesRendering);
            }
        }
        /// <summary>
        /// Retrieves the linked material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(int index) where T2 : ShaderVar
        {
            if (index >= 0 && index < Material.Parameters.Length)
                return Material.Parameters[index] as T2;
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// Retrieves the linked material's uniform parameter with the given name.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(string name) where T2 : ShaderVar
            => Material.Parameters.FirstOrDefault(x => x.Name == name) as T2;
        private Matrix4 _lastRenderedModelMatrix = Matrix4.Identity;

        internal TMaterial GetRenderMaterial(TMaterial localOverrideMat)
            => Engine.Renderer.MeshMaterialOverride ?? localOverrideMat ?? Material;

        public void Render() => Render(1);
        public void Render(int instances) 
            => Render(Matrix4.Identity, Matrix3.Identity, instances);
        public void Render(Matrix4 modelMatrix, int instances = 1) 
            => Render(modelMatrix, modelMatrix.Inverted().Transposed().GetRotationMatrix3(), instances);
        public void Render(Matrix4 modelMatrix, TMaterial materialOverride, int instances = 1)
            => Render(modelMatrix, modelMatrix.Inverted().Transposed().GetRotationMatrix3(), materialOverride, instances);
        public void Render(Matrix4 modelMatrix, Matrix3 normalMatrix, int instances = 1)
            => Render(modelMatrix, normalMatrix, null, instances);
        public void Render(Matrix4 modelMatrix, Matrix3 normalMatrix, TMaterial materialOverride, int instances = 1)
        {
            if (_data is null || !_allowRender)
                return;

            if (_remake)
            {
                Destroy();
                for (int i = 0; i < _utilizedBones.Length; ++i)
                {
                    IBone b = _utilizedBones[i];
                    b.AddPrimitiveManager(this);
                }
                if (_boneMatrixBuffer != null)
                {
                    _boneMatrixBuffer.Destroy();
                    _boneMatrixBuffer.Generate();
                    _boneMatrixBuffer.PushData();
                }
                _remake = false;
            }

            if (!IsActive)
                Generate();
            
            if (_singleBind != null)
            {
                modelMatrix = modelMatrix * _singleBind.VertexMatrix;
                normalMatrix = normalMatrix * _singleBind.VertexMatrix.Inverted().Transposed().GetRotationMatrix3();
            }

            TMaterial mat = GetRenderMaterial(materialOverride);
            if (mat is null)
                return;

            RenderProgram vtxProg, matProg;
            if (Engine.Settings.AllowShaderPipelines)
            {
                matProg = mat.Program;

                _pipeline.Bind();
                _pipeline.Clear(EProgramStageMask.AllShaderBits);
                _pipeline.Set(mat.Program.ShaderTypeMask, matProg.BindingId);

                //If the program doesn't override the vertex shader, use the default one for this mesh
                if (!mat.Program.ShaderTypeMask.HasFlag(EProgramStageMask.VertexShaderBit))
                {
                    vtxProg = _vertexProgram;
                    _pipeline.Set(EProgramStageMask.VertexShaderBit, vtxProg.BindingId);
                }
                else
                    vtxProg = matProg;
            }
            else
            {
                if (VertexFragProgram is null)
                {
                    VertexFragProgram = new RenderProgram(Material.FragmentShaders[0], _vertexShader);
                    //_vertexFragProgram.Generate();
                }
                vtxProg = matProg = VertexFragProgram;
                VertexFragProgram.Use();
            }
            
            SetSkinningUniforms(vtxProg);
            
            vtxProg.Uniform(Uniform.GetLocation(vtxProg, EEngineUniform.ModelMatrix), modelMatrix);
            //Engine.Renderer.Uniform(vtxId, Uniform.GetLocation(vtxId, ECommonUniform.PrevModelMatrix), _lastRenderedModelMatrix);
            vtxProg.Uniform(Uniform.GetLocation(vtxProg, EEngineUniform.NormalMatrix), normalMatrix);

            if (Engine.Renderer.CurrentCamera != null)
            {
                vtxProg.Uniform(Uniform.GetLocation(vtxProg,
                    EEngineUniform.WorldToCameraSpaceMatrix),
                    Engine.Renderer.CurrentCamera.WorldToCameraSpaceMatrix);

                vtxProg.Uniform(Uniform.GetLocation(vtxProg,
                    EEngineUniform.ProjMatrix),
                    Engine.Renderer.CurrentCamera.ProjectionMatrix);
            }
            else
            {
                //No camera? Everything will be rendered in world space instead of camera space.
                //This is used by point lights to render depth cubemaps, for example.

                vtxProg.Uniform(Uniform.GetLocation(vtxProg, 
                    EEngineUniform.WorldToCameraSpaceMatrix), 
                    Matrix4.Identity);

                vtxProg.Uniform(Uniform.GetLocation(vtxProg,
                    EEngineUniform.ProjMatrix),             
                    Matrix4.Identity);
            }

            mat.SetUniforms(matProg);

            OnSettingUniforms(vtxProg, matProg);
            
            Engine.Renderer.RenderPrimitiveManager(this, false, Instances);
            _lastRenderedModelMatrix = modelMatrix;
        }
        private void OnSettingUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
            => SettingUniforms?.Invoke(vertexProgram, materialProgram);
        protected override void PostGenerated()
        {
            //Create vertex shader program here
            if (Engine.Settings.AllowShaderPipelines)
            {
                _vertexProgram = new RenderProgram(_vertexShader);
                //_vertexProgram.Generate();
            }
            else
            {
                if (VertexFragProgram is null)
                {
                    VertexFragProgram = new RenderProgram(Material.FragmentShaders[0], _vertexShader);
                    //_vertexFragProgram.Generate();
                }
            }
            
            Engine.Renderer.BindPrimitiveManager(this);
            _bindingIds = _data.GenerateBuffers(BindingId);
            IndexBuffer._vaoId = BindingId;
            IndexBuffer.Generate();
            Engine.Renderer.LinkRenderIndices(this, IndexBuffer);
            Engine.Renderer.BindPrimitiveManager(null);
        }
        protected override void PostDeleted()
        {
            _pipeline?.Destroy();
            _vertexProgram?.Destroy();
            VertexFragProgram?.Destroy();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Destroy();
                }
                
                _data.Dispose();
                IndexBuffer.Dispose();
                _vertexProgram = null;
                VertexFragProgram = null;

                _disposedValue = true;
            }
        }
    }
}
