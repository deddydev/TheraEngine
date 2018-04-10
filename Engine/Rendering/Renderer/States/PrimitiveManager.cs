﻿using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;

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
        ThreadSafeHashSet<int> ModifiedBoneIndices { get; }
        ThreadSafeHashSet<int> ModifiedVertexIndices { get; }
        PrimitiveData Data { get; set; }
        VertexBuffer IndexBuffer { get; }
        TMaterial Material { get; set; }
        EDrawElementType ElementType { get; }

        void SkeletonChanged(Skeleton skeleton);
        T2 Parameter<T2>(int index) where T2 : ShaderVar;
        T2 Parameter<T2>(string name) where T2 : ShaderVar;
        void Render();
        void Render(Matrix4 modelMatrix);
        void Render(Matrix4 modelMatrix, Matrix3 normalMatrix);
        void Render(Matrix4 modelMatrix, Matrix3 normalMatrix, TMaterial material);
    }
    /// <summary>
    /// Used to render raw primitive data.
    /// </summary>
    public class PrimitiveManager : BaseRenderState, IPrimitiveManager
    {
        public event Action SettingUniforms;

        //Vertex buffer information
        private int[] _bindingIds;
        private IntPtr[] _offsets;
        private int[] _strides;
        private PrimitiveData _data;

        //Shader program information
        private TMaterial _material;
        private ProgramPipeline _pipeline; //Used to connect the material shader(s) to the vertex shader
        private RenderProgram _vertexProgram; //The autogenerated vertex shader program
        private RenderProgram _vertexFragProgram;
        private VertexShaderDesc _bufferInfo; //The buffers utilized by the vertex buffers - used to generate vertex shader

        private VertexBuffer _indexBuffer;
        private EDrawElementType _elementType;
        internal ShaderFile _vertexShader;

        //Skeleton information
        private Bone _singleBind;
        private Bone[] _utilizedBones;
        private Dictionary<int, int> _boneRemap;

        //Realtime skinning information
        private volatile ThreadSafeHashSet<int> _modifiedVertexIndices = new ThreadSafeHashSet<int>();
        private volatile ThreadSafeHashSet<int> _modifiedBoneIndices = new ThreadSafeHashSet<int>();
        private CPUSkinInfo _cpuSkinInfo; //Only used in CPU skinning mode
        private bool _processingSkinning = false;

        public PrimitiveManager() : base(EObjectType.VertexArray) { }
        public PrimitiveManager(PrimitiveData data, TMaterial material) : this()
        {
            if (Engine.Settings.AllowShaderPipelines)
                _pipeline = new ProgramPipeline();
            _material = material;
            Data = data;
        }

        //TODO: move vertex buffer allocations out of PrimitiveData and into this class, so the original PrimitiveData can be disposed of.
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
                    _bufferInfo = _data.BufferInfo;
                    _data.BufferInfoChanged += _data_BufferInfoChanged;
                    _data_BufferInfoChanged();
                    
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
                else
                    _bufferInfo = null;
            }
        }

        private void _data_BufferInfoChanged()
        {
            _vertexShader = new VertexShaderGenerator().Generate(_bufferInfo, false, false, false, _material);
            if (Engine.Settings.AllowShaderPipelines && _vertexProgram != null)
            {
                if (_vertexProgram.IsActive)
                {
                    _vertexProgram.Destroy();
                    _vertexProgram = new RenderProgram(_vertexShader);
                    _vertexProgram.Generate();
                }
                else
                    _vertexProgram = null;
            }
        }

        public TMaterial Material
        {
            get
            {
                if (_material?.Program != null && _material.Program.IsValid)
                    return _material;
                else
                    return TMaterial.InvalidMaterial;
            }
            set => _material = value;
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

        public ThreadSafeHashSet<int> ModifiedVertexIndices => _modifiedVertexIndices;
        public ThreadSafeHashSet<int> ModifiedBoneIndices => _modifiedBoneIndices;

        public VertexBuffer IndexBuffer => _indexBuffer;
        public EDrawElementType ElementType => _elementType;

        public RenderProgram VertexFragProgram => _vertexFragProgram;

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
                            ThreadSafeList<int> list;

                            //if (!b._influencedVertices.ContainsKey(BindingId))
                            //    b._influencedVertices.Add(BindingId, list = new List<int>());
                            //else
                                list = b._influencedVertices[BindingId].Item2;

                            if (!list.Contains(point.VertexIndex))
                                list.Add(point.VertexIndex);
                        }
                        else
                        {
                            //if (b._influencedVertices.ContainsKey(BindingId))
                            //{
                            ThreadSafeList<int> list = b._influencedVertices[BindingId].Item2;
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
            if (skeleton != null)
            {
                if (_data._utilizedBones == null || _data._utilizedBones.Length == 1)
                {
                    if (!string.IsNullOrEmpty(_data.SingleBindBone) &&
                        skeleton.BoneNameCache.ContainsKey(_data.SingleBindBone))
                        _singleBind = skeleton.BoneNameCache[_data.SingleBindBone];
                    else
                        _singleBind = null;

                    _cpuSkinInfo = null;
                }
                else if (_data._influences != null)
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
                                InfluenceDef inf = _data._influences[_data._facePoints[i]._influenceIndex];
                                for (int j = 0; j < InfluenceDef.MaxWeightCount; ++j)
                                {
                                    BoneWeight weight = inf.Weights[j];
                                    int index;
                                    if (weight == null || (index = _data._utilizedBones.IndexOf(weight.Bone)) < 0)
                                    {
                                        matrixIndices[i][j] = 0;
                                        matrixWeights[i][j] = 0.0f;
                                    }
                                    else
                                    {
                                        matrixIndices[i][j] = index;
                                        matrixWeights[i][j] = weight.Weight;
                                    }
                                }
                            }

                            _data.AddBuffer(matrixIndices, new VertexAttribInfo(BufferType.MatrixIds), false, true);
                            _data.AddBuffer(matrixWeights, new VertexAttribInfo(BufferType.MatrixWeights));

                            Destroy();
                            Generate();
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
                            InfluenceDef inf = _data._influences[_data._facePoints[i]._influenceIndex];
                            for (int j = 0; j < InfluenceDef.MaxWeightCount; ++j)
                            {
                                BoneWeight weight = inf.Weights[j];
                                int index;
                                if (weight == null || (index = _data._utilizedBones.IndexOf(weight.Bone)) < 0)
                                {
                                    matrixIndices[i][j] = 0.0f;
                                    matrixWeights[i][j] = 0.0f;
                                }
                                else
                                {
                                    matrixIndices[i][j] = index;
                                    matrixWeights[i][j] = weight.Weight;
                                }
                            }
                        }
                        _cpuSkinInfo = null;

                        _data.AddBuffer(matrixIndices, new VertexAttribInfo(BufferType.MatrixIds), false, false);
                        _data.AddBuffer(matrixWeights, new VertexAttribInfo(BufferType.MatrixWeights));

                        Destroy();
                        Generate();
                    }

                    _utilizedBones = _data._utilizedBones.Select(x => skeleton.BoneNameCache[x]).ToArray();
                    _boneRemap = new Dictionary<int, int>();
                    for (int i = 0; i < _utilizedBones.Length; ++i)
                    {
                        Bone b = _utilizedBones[i];

                        _modifiedBoneIndices.Add(b._index);

                        b.AddPrimitiveManager(this);
                        _boneRemap.Add(b._index, i);
                    }
                }
            }
            else
            {
                _utilizedBones = null;
            }
            UpdateBoneInfo(true);
        }
        private void SetSkinningUniforms(int programBindingId)
        {
            if (!_bufferInfo.IsWeighted)
                return;

            _processingSkinning = true;
            if (Engine.Settings.SkinOnGPU)
            {
                //Engine.Renderer.ProgramUniform(programBindingId, Uniform.BonePosMtxName + "[0]", Matrix4.Identity);
                //Engine.Renderer.ProgramUniform(programBindingId, Uniform.BoneNrmMtxName + "[0]", Matrix4.Identity);

                var pos = _modifiedBoneIndices.Select(x => _utilizedBones[_boneRemap[x]].VertexMatrix).ToArray();
                var nrm = _modifiedBoneIndices.Select(x => _utilizedBones[_boneRemap[x]].NormalMatrix).ToArray();
                Engine.Renderer.Uniform(programBindingId, Uniform.BonePosMtxName, pos);
                Engine.Renderer.Uniform(programBindingId, Uniform.BoneNrmMtxName, nrm);

                //Update modified bone matrix uniforms
                //foreach (int index in _modifiedBoneIndices)                
                //{
                //    int remappedIndex = _boneRemap[index];
                //    Bone b = _utilizedBones[remappedIndex];

                //    //Increase index to account for identity matrix at index 0
                //    ++remappedIndex;

                //    Engine.Renderer.ProgramUniform(programBindingId, Uniform.BonePosMtxName + "[" + remappedIndex + "]", b.VertexMatrix);
                //    Engine.Renderer.ProgramUniform(programBindingId, Uniform.BoneNrmMtxName + "[" + remappedIndex + "]", b.NormalMatrix);
                //}

                //Engine.Renderer.Uniform(Uniform.MorphWeightsName, _morphWeights);
                _modifiedBoneIndices.Clear();
            }
            else
            {
                _cpuSkinInfo?.UpdatePNBT(_modifiedVertexIndices);
                _modifiedVertexIndices.Clear();
            }
            _processingSkinning = false;
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
        public void Render()
            => Render(Matrix4.Identity, Matrix3.Identity);
        
        
        private Matrix4 _lastRenderedModelMatrix = Matrix4.Identity;

        internal TMaterial GetRenderMaterial(TMaterial localOverrideMat)
            => Engine.Renderer.MaterialOverride ?? localOverrideMat ?? Material;

        public unsafe void Render(Matrix4 modelMatrix) => Render(modelMatrix, modelMatrix.Inverted().Transposed().GetRotationMatrix3());
        public unsafe void Render(Matrix4 modelMatrix, TMaterial material) => Render(modelMatrix, modelMatrix.Inverted().Transposed().GetRotationMatrix3(), material);
        public unsafe void Render(Matrix4 modelMatrix, Matrix3 normalMatrix) => Render(modelMatrix, normalMatrix, null);
        public unsafe void Render(Matrix4 modelMatrix, Matrix3 normalMatrix, TMaterial material)
        {
            if (_data == null)
                return;
            
            if (!IsActive)
                Generate();
            
            if (_singleBind != null)
            {
                modelMatrix = modelMatrix * _singleBind.VertexMatrix;
                normalMatrix = normalMatrix * _singleBind.NormalMatrix.GetRotationMatrix3();
            }

            TMaterial mat = GetRenderMaterial(material);

            int vtxId, fragId;
            if (Engine.Settings.AllowShaderPipelines)
            {
                _pipeline.Bind();
                //Engine.PrintLine("{0} bound", _pipeline.ToString());
                _pipeline.Set(EProgramStageMask.FragmentShaderBit | EProgramStageMask.GeometryShaderBit, fragId = mat.Program.BindingId);
                _pipeline.Set(EProgramStageMask.VertexShaderBit, vtxId = _vertexProgram.BindingId);
            }
            else
            {
                vtxId = fragId = VertexFragProgram.BindingId;
                VertexFragProgram.Use();
            }
            
            SetSkinningUniforms(vtxId);
            
            Engine.Renderer.Uniform(vtxId, Uniform.GetLocation(vtxId, EEngineUniform.ModelMatrix), modelMatrix);
            //Engine.Renderer.Uniform(vtxId, Uniform.GetLocation(vtxId, ECommonUniform.PrevModelMatrix), _lastRenderedModelMatrix);
            Engine.Renderer.Uniform(vtxId, Uniform.GetLocation(vtxId, EEngineUniform.NormalMatrix), normalMatrix);

            if (AbstractRenderer.CurrentCamera != null)
            {
                Engine.Renderer.Uniform(vtxId, Uniform.GetLocation(vtxId, EEngineUniform.WorldToCameraSpaceMatrix),
                    AbstractRenderer.CurrentCamera.WorldToCameraSpaceMatrix);
                Engine.Renderer.Uniform(vtxId, Uniform.GetLocation(vtxId, EEngineUniform.ProjMatrix),
                    AbstractRenderer.CurrentCamera.ProjectionMatrix);
            }
            else
            {
                //No camera? Everything will be rendered in world space instead of camera space.
                //This is used by point lights to render depth cubemaps, for example.
                Engine.Renderer.Uniform(vtxId, Uniform.GetLocation(vtxId, EEngineUniform.WorldToCameraSpaceMatrix),
                    Matrix4.Identity);
                Engine.Renderer.Uniform(vtxId, Uniform.GetLocation(vtxId, EEngineUniform.ProjMatrix),
                    Matrix4.Identity);
            }

            mat.SetUniforms(fragId);

            OnSettingUniforms();
            
            Engine.Renderer.RenderPrimitiveManager(this, false);
            _lastRenderedModelMatrix = modelMatrix;
        }
        private void OnSettingUniforms() => SettingUniforms?.Invoke();
        protected override void PostGenerated()
        {
            //Create vertex shader program here
            if (Engine.Settings.AllowShaderPipelines)
            {
                _vertexProgram = new RenderProgram(_vertexShader);
                _vertexProgram.Generate();
            }
            else
            {
                if (_vertexFragProgram == null)
                {
                    _vertexFragProgram = new RenderProgram(Material.FragmentShaders[0], _vertexShader);
                    _vertexFragProgram.Generate();
                }
            }
            
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
            _vertexProgram?.Destroy();
            _vertexProgram = null;
            _vertexFragProgram?.Destroy();
            _vertexFragProgram = null;
        }
    }
}
