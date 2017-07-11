﻿using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;

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
        Material Material { get; set; }
        EDrawElementType ElementType { get; }

        void SkeletonChanged(Skeleton skeleton);
        T2 Parameter<T2>(int index) where T2 : ShaderVar;
        T2 Parameter<T2>(string name) where T2 : ShaderVar;
        void Render();
        void Render(Matrix4 modelMatrix);
        void Render(Matrix4 modelMatrix, Matrix3 normalMatrix);
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
        private Material _material;
        private ProgramPipeline _pipeline; //Used to connect the material shader(s) to the vertex shader
        private RenderProgram _vertexProgram; //The autogenerated vertex shader program
        private VertexShaderDesc _bufferInfo; //The buffers utilized by the vertex buffers - used to generate vertex shader

        private VertexBuffer _indexBuffer;
        private EDrawElementType _elementType;
        private Shader _vertexShader;

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
        public PrimitiveManager(PrimitiveData data, Material material) : this()
        {
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

                    _vertexShader = VertexShaderGenerator.Generate(_bufferInfo, false, false, false);
                    if (_vertexProgram != null)
                    {
                        if (_vertexProgram.IsActive)
                        {
                            _vertexProgram.Destroy();
                            _vertexProgram = new RenderProgram(_bufferInfo, _vertexShader);
                            _vertexProgram.Generate();
                            _pipeline.Clear(EProgramStageMask.VertexShaderBit);
                            _pipeline.Add(EProgramStageMask.VertexShaderBit, _vertexProgram);
                        }
                        else
                            _vertexProgram = null;
                    }

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
        public Material Material
        {
            get => _material;
            set
            {
                _material = value;
                EProgramStageMask mask = EProgramStageMask.FragmentShaderBit | EProgramStageMask.GeometryShaderBit | EProgramStageMask.TessControlShaderBit | EProgramStageMask.TessEvaluationShaderBit;
                _pipeline.Clear(mask);
                _pipeline.Add(mask, _material.Program);
            }
        }

        public RenderProgram VertexProgram
        {
            get
            {
                if (!IsActive)
                    Generate();
                return _vertexProgram;
            }
        }

        public ThreadSafeHashSet<int> ModifiedVertexIndices => _modifiedVertexIndices;
        public ThreadSafeHashSet<int> ModifiedBoneIndices => _modifiedBoneIndices;

        public VertexBuffer IndexBuffer => _indexBuffer;
        public EDrawElementType ElementType => _elementType;

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
                                list = b._influencedVertices[BindingId];

                            if (!list.Contains(point.VertexIndex))
                                list.Add(point.VertexIndex);
                        }
                        else
                        {
                            //if (b._influencedVertices.ContainsKey(BindingId))
                            //{
                            ThreadSafeList<int> list = b._influencedVertices[BindingId];
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
                    if (!string.IsNullOrEmpty(_data.SingleBindBone) && skeleton.BoneNameCache.ContainsKey(_data.SingleBindBone))
                        _singleBind = skeleton.BoneNameCache[_data.SingleBindBone];
                    else
                        _singleBind = null;

                    _cpuSkinInfo = null;
                    //_bufferInfo._boneCount = 1;
                }
                else if (_data._influences != null)
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
                    //_bufferInfo._boneCount = _utilizedBones.Length;
                }
            }
            else
            {
                //_bufferInfo._boneCount = 0;
                _utilizedBones = null;
            }
            UpdateBoneInfo(true);
            //_vertexShaderProgram?.MakeVertexShader(_bufferInfo);
        }
        private void SetSkinningUniforms()
        {
            if (!_bufferInfo.IsWeighted)
                return;

            _processingSkinning = true;
            if (Engine.Settings.SkinOnGPU)
            {
                Engine.Renderer.ProgramUniform(_vertexProgram.BindingId, Uniform.BoneMatricesName + "[0]", Matrix4.Identity);
                Engine.Renderer.ProgramUniform(_vertexProgram.BindingId, Uniform.BoneMatricesITName + "[0]", Matrix4.Identity);

                //Update modified bone matrix uniforms
                foreach (int index in _modifiedBoneIndices)
                {
                    int remappedIndex = _boneRemap[index];
                    Bone b = _utilizedBones[remappedIndex];
                    //Increase index to account for identity matrix at index 0
                    ++remappedIndex;
                    Engine.Renderer.ProgramUniform(_vertexProgram.BindingId, Uniform.BoneMatricesName + "[" + remappedIndex + "]", b.VertexMatrix);
                    Engine.Renderer.ProgramUniform(_vertexProgram.BindingId, Uniform.BoneMatricesITName + "[" + remappedIndex + "]", b.NormalMatrix);
                }
                //Engine.Renderer.Uniform(Uniform.MorphWeightsName, _morphWeights);
                _modifiedBoneIndices.Clear();
            }
            else
                _cpuSkinInfo.UpdatePNBT(_modifiedVertexIndices);
            _processingSkinning = false;
        }

        /// <summary>
        /// Retrieves the linked material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(int index) where T2 : ShaderVar
        {
            if (VertexProgram == null)
                Generate();
            if (index >= 0 && index < Material.Parameters.Length)
                return Material.Parameters[index] as T2;
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// Retrieves the linked material's uniform parameter with the given name.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(string name) where T2 : ShaderVar
        {
            if (VertexProgram == null)
                Generate();
            return Material.Parameters.FirstOrDefault(x => x.Name == name) as T2;
        }
        public void Render()
        {
            Render(Matrix4.Identity, Matrix3.Identity);
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

            if (_vertexProgram == null)
                return;

            if (_singleBind != null)
            {
                modelMatrix = modelMatrix * _singleBind.FrameMatrix;
                normalMatrix = normalMatrix * _singleBind.FrameMatrix.GetRotationMatrix3();
            }

            _pipeline.Bind();

            AbstractRenderer.CurrentCamera.SetUniforms(_vertexProgram.BindingId);
            AbstractRenderer.CurrentCamera.SetUniforms(_material.Program.BindingId);

            if (Engine.Settings.ShadingStyle == ShadingStyle.Forward)
                Engine.Scene.Lights.SetUniforms(_material.Program.BindingId);

            _material.SetUniforms();
            Engine.Renderer.Cull(_data.Culling);

            OnSettingUniforms();
            
            SetSkinningUniforms();
            Engine.Renderer.ProgramUniform(_vertexProgram.BindingId, Uniform.GetLocation(_vertexProgram.BindingId, ECommonUniform.ModelMatrix), modelMatrix);
            Engine.Renderer.ProgramUniform(_vertexProgram.BindingId, Uniform.GetLocation(_vertexProgram.BindingId, ECommonUniform.NormalMatrix), normalMatrix);

            Engine.Renderer.RenderPrimitiveManager(this, false);
            //Engine.Renderer.UseProgram(null);
        }
        private void OnSettingUniforms() => SettingUniforms?.Invoke();
        protected override void OnGenerated()
        {
            //Create vertex shader program here
            _vertexProgram = new RenderProgram(_bufferInfo, _vertexShader);
            _vertexProgram.Generate();

            _pipeline.Clear(EProgramStageMask.VertexShaderBit);
            _pipeline.Add(EProgramStageMask.VertexShaderBit, _vertexProgram);

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
            _vertexProgram.Destroy();
        }
    }
}
