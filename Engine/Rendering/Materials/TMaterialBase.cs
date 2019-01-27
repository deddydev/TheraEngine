using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public delegate void DelSettingUniforms(RenderProgram program);
    public abstract class TMaterialBase : TFileObject
    {
        public event DelSettingUniforms SettingUniforms;
        public event Action ParametersChanged;
        public event Action TexturesChanged;

        protected float _secondsLive = 0.0f;
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
                if (_program != null && !_program.IsActive)
                    _program.GenerateSafe();
                return _program;
            }
            protected set
            {
                if (_program != null)
                    _program.Generated -= _program_Generated;
                _program = value;
                if (_program != null)
                    _program.Generated += _program_Generated;
            }
        }

        private void _program_Generated()
        {
            _secondsLive = 0.0f;

            //return;
            //if (Program.IsValid)
            //{
            //    const string uniform = "uniform";
            //    const string sampler = "sampler";
            //    const string layout = "layout";
            //    const string binding = "binding";

            //    List<ShaderVar> vars = new List<ShaderVar>();
            //    List<BaseTexRef> tex = new List<BaseTexRef>();

            //    foreach (RenderShader shader in Program)
            //    {
            //        string text = shader.SourceText;
            //        int mainIndex = text.IndexOf("main(");
            //        if (mainIndex > 0)
            //            text = text.Substring(0, mainIndex);

            //        int prevSemicolonIndex = -1;
            //        int[] uniformIndices = text.FindAllOccurrences(0, uniform);
            //        foreach (int unifIndex in uniformIndices)
            //        {
            //            int startIndex = unifIndex + uniform.Length;
            //            int semicolonIndex = text.FindFirst(startIndex, ';');

            //            prevSemicolonIndex = semicolonIndex;

            //            string uniformLineText = text.Substring(startIndex, semicolonIndex - startIndex).Trim().ReplaceWhitespace(" ");

            //            string[] parts = uniformLineText.Split(' ');
            //            string type = parts[0];
            //            string name = parts[1];

            //            int samplerIndex = type.IndexOf(sampler);
            //            if (samplerIndex >= 0)
            //            {
            //                //This is a texture uniform

            //                //int layoutIndex = prevSemicolonIndex < 0 ?
            //                //    text.FindFirstReverse(layout) :
            //                //    text.Substring(prevSemicolonIndex + 1, unifIndex).FindFirstReverse(layout);

            //                //int bindingIndex = -1;
            //                //if (layoutIndex >= 0)
            //                //{
            //                //    int open = text.FindFirst(layoutIndex + layout.Length, '(') + 1;
            //                //    int close = text.FindFirst(open, ')');
            //                //    string layoutSection = text.Substring(open, close - open);
            //                //    int bindingStrIndex = layoutSection.IndexOf(binding);
            //                //    int start = layoutSection.FindFirst(bindingStrIndex + binding.Length, '=') + 1;
            //                //    int commaIndex = layoutSection.FindFirst(start, ',');
            //                //    int end = commaIndex < 0 ? close : commaIndex;
            //                //    string bindingValue = layoutSection.Substring(start, end - start);
            //                //    bindingIndex = int.Parse(bindingValue.Trim());
            //                //}
            //                //else
            //                //{
            //                //    bindingIndex = 
            //                //}
                            
            //                string type2 = type.Substring(samplerIndex + 7);
            //                switch (type2)
            //                {
            //                    case "1D":

            //                        break;
            //                    case "2D":

            //                        break;
            //                    case "3D":

            //                        break;
            //                    case "Cube":

            //                        break;
            //                    case "2DRect":

            //                        break;
            //                    case "1DArray":

            //                        break;
            //                    case "2DArray":

            //                        break;
            //                    case "CubeArray":

            //                        break;
            //                    case "Buffer":

            //                        break;
            //                    case "2DMS":

            //                        break;
            //                    case "2DMSArray":

            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                int equalIndex = uniformLineText.FindFirst(0, '=');
            //                if (equalIndex >= 0)
            //                {

            //                }

            //                if (Enum.TryParse("_" + type, out EShaderVarType result))
            //                {
            //                    Type shaderType = ShaderVar.ShaderTypeAssociations[result];
            //                    ShaderVar match = Parameters.FirstOrDefault(x =>
            //                        String.Equals(x.Name, name, StringComparison.InvariantCulture) &&
            //                        x.TypeName == result);

            //                    if (match != null && match.GetType() == shaderType)
            //                    {
            //                        vars.Add(match);
            //                    }
            //                    else
            //                    {
            //                        object value;
            //                        int arrayIndexStart = name.IndexOf("[");
            //                        if (arrayIndexStart > 0)
            //                        {
            //                            int arrayIndexEnd = name.IndexOf("]");
            //                            int arrayCount = int.Parse(name.Substring(arrayIndexStart + 1, arrayIndexEnd - arrayIndexStart - 1));

            //                            Type genericVarType = typeof(ShaderArray<>);
            //                            Type genericValType = typeof(ShaderArrayValueHandler<>);

            //                            Type valueType = genericValType.MakeGenericType(shaderType);
            //                            shaderType = genericVarType.MakeGenericType(shaderType);

            //                            value = Activator.CreateInstance(valueType, arrayCount);
            //                            name = name.Substring(0, arrayIndexStart);

            //                            if (match == null)
            //                                value = shaderType.GetDefaultValue();
            //                            else
            //                                value = match.GenericValue;
            //                        }
            //                        else if (match == null)
            //                            value = shaderType.GetDefaultValue();
            //                        else
            //                            value = match.GenericValue;

            //                        //int defaultValue, string name, IShaderVarOwner owner
            //                        ShaderVar var = (ShaderVar)Activator.CreateInstance(shaderType, value, name, null);
            //                        vars.Add(var);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    //Textures = tex.ToArray();
            //    //Parameters = vars.ToArray();
            //}
        }

        [TSerialize]
        public ShaderVar[] Parameters
        {
            get => _parameters;
            set
            {
                _parameters = value;
                ParametersChanged?.Invoke();
            }
        }

        [TSerialize(Order = 1)]
        public BaseTexRef[] Textures
        {
            get => _textures;
            set
            {
                _textures = value;
                TexturesChanged?.Invoke();
            }
        }

        /// <summary>
        /// Retrieves the material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the fragment shader.
        /// </summary>
        public T2 Parameter<T2>(int index) where T2 : ShaderVar
        {
            if (Parameters.IndexInArrayRange(index))
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
        public void SetUniforms(RenderProgram program)
        {
            if (program == null)
                program = Program;

            //Apply special rendering parameters
            if (RenderParams != null)
                Engine.Renderer.ApplyRenderParams(RenderParams);

            //Set variable uniforms
            foreach (ShaderVar v in _parameters)
                v.SetProgramUniform(Program);

            //Set texture uniforms
            SetTextureUniforms(program);

            OnSetUniforms(program);

            SettingUniforms?.Invoke(program);
        }
        protected virtual void OnSetUniforms(RenderProgram program) { }

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
        public void SetTextureUniforms(RenderProgram program)
        {
            for (int i = 0; i < Textures.Length; ++i)
            {
                BaseTexRef tref = Textures[i];
                if (tref == null)
                    continue;
                program.Sampler(tref.ResolveSamplerName(i), tref.RenderTextureGeneric, i);
            }
        }
        public void SetTextureUniform(RenderProgram program, int textureIndex, string samplerNameOverride = null)
        {
            if (Textures.IndexInArrayRange(textureIndex))
            {
                BaseTexRef tref = Textures[textureIndex];
                if (tref == null)
                    return;
                program.Sampler(tref.ResolveSamplerName(textureIndex, samplerNameOverride), tref.RenderTextureGeneric, textureIndex);
            }
        }
    }
}
