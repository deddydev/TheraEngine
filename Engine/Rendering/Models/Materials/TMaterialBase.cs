using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Files;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.Models.Materials
{
    public delegate void DelSettingUniforms(int programBindingId);
    public abstract class TMaterialBase : TFileObject
    {
        public event DelSettingUniforms SettingUniforms;
        public event DelSettingUniforms SettingTextureUniforms;
        
        protected ShaderVar[] _parameters;
        protected BaseTexRef[] _textures;
        protected RenderProgram _program;

        private List<PrimitiveManager> _references = new List<PrimitiveManager>();

        private LocalFileRef<RenderingParameters> _renderParamsRef = new RenderingParameters();

        [Browsable(false)]
        public RenderingParameters RenderParams
        {
            get => RenderParamsRef.File;
            set => RenderParamsRef.File = value;
        }

        [TSerialize]
        public LocalFileRef<RenderingParameters> RenderParamsRef
        {
            get => _renderParamsRef;
            set => _renderParamsRef = value ?? new LocalFileRef<RenderingParameters>();
        }

        public int UniqueID => Program.BindingId;
        public RenderProgram Program
        {
            get
            {
                if (_program != null && (!_program.IsActive || !_program.IsValid))
                    _program.Generate();
                return _program;
            }
            protected set
            {
                _program = value;
            }
        }

        [TSerialize]
        public ShaderVar[] Parameters
        {
            get => _parameters;
            set => _parameters = value;
        }

        [TSerialize(Order = 1)]
        public BaseTexRef[] Textures
        {
            get => _textures;
            set => _textures = value;
        }

        /// <summary>
        /// Retrieves the material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the fragment shader.
        /// </summary>
        public T2 Parameter<T2>(int index) where T2 : ShaderVar
        {
            if (index >= 0 && index < Parameters.Length)
                return Parameters[index] as T2;
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// Retrieves the material's uniform parameter with the given name.
        /// Use this to set uniform values to be passed to the fragment shader.
        /// </summary>
        public T2 Parameter<T2>(string name) where T2 : ShaderVar
            => Parameters.FirstOrDefault(x => x.Name == name) as T2;

        internal void AddReference(PrimitiveManager user)
        {
            //if (_references.Count == 0)
            //    _uniqueID = Engine.Scene.AddActiveMaterial(this);
            _references.Add(user);
        }
        internal void RemoveReference(PrimitiveManager user)
        {
            _references.Add(user);
            //if (_references.Count == 0)
            //{
            //    Engine.Scene.RemoveActiveMaterial(this);
            //    _uniqueID = -1;
            //}
        }
        public void SetUniforms(int programBindingId)
        {
            if (programBindingId <= 0)
                programBindingId = Program.BindingId;

            //Apply special rendering parameters
            if (RenderParams != null)
                Engine.Renderer.ApplyRenderParams(RenderParams);

            //Set variable uniforms
            foreach (ShaderVar v in _parameters)
                v.SetProgramUniform(Program.BindingId);

            //Set texture uniforms
            SetTextureUniforms(programBindingId);

            OnSetUniforms(programBindingId);

            SettingUniforms?.Invoke(programBindingId);
        }
        protected virtual void OnSetUniforms(int programBindingId) { }

        public EDrawBuffersAttachment[] CollectFBOAttachments()
        {
            if (_textures != null && _textures.Length > 0)
            {
                List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
                foreach (BaseTexRef tref in _textures)
                {
                    if (!tref.FrameBufferAttachment.HasValue)
                        continue;
                    switch (tref.FrameBufferAttachment.Value)
                    {
                        case EFramebufferAttachment.Color:
                        case EFramebufferAttachment.Depth:
                        case EFramebufferAttachment.DepthAttachment:
                        case EFramebufferAttachment.DepthStencilAttachment:
                        case EFramebufferAttachment.Stencil:
                        case EFramebufferAttachment.StencilAttachment:
                            continue;
                    }
                    fboAttachments.Add((EDrawBuffersAttachment)(int)tref.FrameBufferAttachment.Value);
                }
                return fboAttachments.ToArray();
            }
            return new EDrawBuffersAttachment[0];
        }

        public void GenerateTextures(bool loadSynchronously)
        {
            if (_textures != null)
            {
                foreach (var t in _textures)
                //await Task.Run(() => Parallel.ForEach(_textures, t =>
                {
                    t.GetRenderTextureGeneric(loadSynchronously).PushData();
                }
                //));
            }
        }
        /// <summary>
        /// Resizes the gbuffer's textures.
        /// Note that they will still fully cover the screen regardless of 
        /// if their dimensions match or not.
        /// </summary>
        public void Resize2DTextures(int width, int height)
        {
            //Update each texture's dimensions
            foreach (BaseTexRef t in Textures)
                if (t is TexRef2D t2d)
                    t2d.Resize(width, height);
        }
        private void SetTextureUniforms(int programBindingId)
        {
            for (int i = 0; i < Textures.Length; ++i)
                SetTextureUniform(Textures[i].GetRenderTextureGeneric(true), i, "Texture" + i, programBindingId);
        }
        public static void SetTextureUniform(BaseRenderTexture tref, int textureUnit, string varName, int programBindingId)
        {
            if (tref == null)
                return;

            Engine.Renderer.SetActiveTexture(textureUnit);
            Engine.Renderer.Uniform(programBindingId, varName, textureUnit);
            tref.Bind();
        }
    }
}
