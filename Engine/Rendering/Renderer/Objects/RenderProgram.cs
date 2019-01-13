﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering
{
    public class RenderProgram : BaseRenderObject, IEnumerable<RenderShader>
    {
        public EProgramStageMask ShaderTypeMask { get; private set; } = EProgramStageMask.None;

        private List<RenderShader> _shaders;
        protected List<RenderShader> Shaders
        {
            get => _shaders;
            set
            {
                _shaders = value ?? new List<RenderShader>();

                if (_shaders.Any(x => x == null))
                    _shaders = _shaders.Where(x => x != null).ToList();

                ShaderTypeMask = EProgramStageMask.None;
                foreach (var shader in _shaders)
                {
                    shader.OwningProgram = this;
                    ShaderTypeMask |= (EProgramStageMask)(int)shader.ShaderMode;
                }

                //Force a recompilation.
                //TODO: recompile shaders without destroying program.
                //Need to attach shaders by id to the program and recompile.
                Destroy();
            }
        }
        
        private readonly ConcurrentDictionary<string, int> 
            _uniformCache = new ConcurrentDictionary<string, int>(),
            _attribCache = new ConcurrentDictionary<string, int>();

        public int GetUniformLocation(string name)
        {
            int bindingId = BindingId;
            if (bindingId == NullBindingId)
                return -1;
            return _uniformCache.GetOrAdd(name, n => Engine.Renderer.OnGetUniformLocation(bindingId, n));
        }
        public int GetAttributeLocation(string name)
        {
            int bindingId = BindingId;
            if (bindingId == NullBindingId)
                return -1;
            return _attribCache.GetOrAdd(name, n => Engine.Renderer.OnGetAttribLocation(bindingId, n));
        }

        public bool IsValid { get; private set; } = false;

        public int AddShader(GLSLScript shader)
        {
            if (shader == null)
                return -1;
            RenderShader rs = new RenderShader(shader) { OwningProgram = this };
            _shaders.Add(rs);
            ShaderTypeMask |= (EProgramStageMask)(int)shader.Type;
            Destroy();
            return _shaders.Count - 1;
        }
        public int AddShader(RenderShader shader)
        {
            if (shader == null)
                return -1;
            shader.OwningProgram = this;
            _shaders.Add(shader);
            ShaderTypeMask |= (EProgramStageMask)(int)shader.ShaderMode;
            Destroy();
            return _shaders.Count - 1;
        }
        public void RemoveShader(RenderShader shader)
        {
            if (shader == null || !_shaders.Contains(shader))
                return;
            shader.OwningProgram = null;
            _shaders.Remove(shader);
            ShaderTypeMask &= ~(EProgramStageMask)(int)shader.ShaderMode;
            Destroy();
        }

        public void SetShaders(params GLSLScript[] shaders)
            => Shaders = shaders.Select(x => new RenderShader(x)).ToList();
        public void SetShaders(IEnumerable<GLSLScript> shaders)
            => Shaders = shaders.Select(x => new RenderShader(x)).ToList();
        public void SetShaders(IEnumerable<RenderShader> shaders)
            => Shaders = shaders.ToList();
        public void SetShaders(params RenderShader[] shaders)
            => Shaders = shaders.ToList();

        public RenderProgram(params GLSLScript[] shaders)
            : base(EObjectType.Program) => SetShaders(shaders);
        public RenderProgram(IEnumerable<GLSLScript> shaders)
            : base(EObjectType.Program) => SetShaders(shaders);
        public RenderProgram(IEnumerable<RenderShader> shaders)
            : base(EObjectType.Program) => SetShaders(shaders);
        public RenderProgram(params RenderShader[] shaders)
            : base(EObjectType.Program) => SetShaders(shaders);

        public override void Destroy()
        {
            base.Destroy();

            IsValid = false;
            _uniformCache.Clear();
        }

        protected override int CreateObject()
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Func<int>)CreateObject, BaseRenderPanel.PanelType.Rendering))
                return CurrentBind.BindingId;

            _uniformCache.Clear();

            IsValid = true;

            int id = Engine.Renderer.GenerateProgram(Engine.Settings.AllowShaderPipelines);
            
            if (_shaders.Count == 0)
            {
                IsValid = false;
                return NullBindingId;
            }

            RenderShader shader;
            for (int i = 0; i < _shaders.Count; ++i)
            {
                shader = _shaders[i];

                if (!shader.IsActive)
                    shader.GenerateSafe();

                if (IsValid = IsValid && shader.IsCompiled)
                    Engine.Renderer.AttachShader(shader.BindingId, id);
                else
                    return NullBindingId;
            }

            bool valid = Engine.Renderer.LinkProgram(id, out string info);
            if (!(IsValid = IsValid && valid))
            {
                //if (info.Contains("Vertex info"))
                //{
                //    RenderShader s = _shaders.FirstOrDefault(x => x.File.Type == EShaderMode.Vertex);
                //    string source = s.GetSource(true);
                //    Engine.PrintLine(source);
                //}
                //else if (info.Contains("Geometry info"))
                //{
                //    RenderShader s = _shaders.FirstOrDefault(x => x.File.Type == EShaderMode.Geometry);
                //    string source = s.GetSource(true);
                //    Engine.PrintLine(source);
                //}
                //else if (info.Contains("Fragment info"))
                //{
                //    RenderShader s = _shaders.FirstOrDefault(x => x.File.Type == EShaderMode.Fragment);
                //    string source = s.GetSource(true);
                //    Engine.PrintLine(source);
                //}
                return NullBindingId;
            }

            //Destroy shader objects. We don't need them now.
            for (int i = 0; i < _shaders.Count; ++i)
            {
                shader = _shaders[i];
                Engine.Renderer.DetachShader(shader.BindingId, id);
                shader.Destroy();
            }
            
            return id;
        }
        
        public void Use() => Engine.Renderer.UseProgram(BindingId);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params IUniformable4Int[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params IUniformable4Float[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params IUniformable3Int[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params IUniformable3Float[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params IUniformable2Int[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params IUniformable2Float[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params IUniformable1Int[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params IUniformable1Float[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params int[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params float[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params uint[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, params double[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, Matrix4 p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, Matrix4[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, Matrix3 p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by engineUniform.
        /// The engineUniform is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(EEngineUniform engineUniform, Matrix3[] p)
            => Uniform(Models.Materials.Uniform.GetLocation(this, engineUniform), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params IUniformable4Int[] p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params IUniformable4Float[] p)
            => Uniform(GetUniformLocation(name), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params IUniformable3Int[] p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params IUniformable3Float[] p)
            => Uniform(GetUniformLocation(name), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params IUniformable2Int[] p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params IUniformable2Float[] p)
            => Uniform(GetUniformLocation(name), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params IUniformable1Int[] p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params IUniformable1Float[] p)
            => Uniform(GetUniformLocation(name), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params int[] p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params float[] p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params uint[] p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, params double[] p)
            => Uniform(GetUniformLocation(name), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, Matrix4 p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, Matrix4[] p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, Matrix3 p)
            => Uniform(GetUniformLocation(name), p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by name.
        /// The name is cached so that retrieving the uniform location is only required once.
        /// </summary>
        public void Uniform(string name, Matrix3[] p)
            => Uniform(GetUniformLocation(name), p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable4Int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable4Float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable4Double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable4UInt[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable4Bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable3Int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable3Float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable3Double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable3UInt[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable3Bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable2Int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable2Float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable2Double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable2UInt[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable2Bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable1Int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable1Float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable1Double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable1UInt[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params IUniformable1Bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params uint[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, Matrix4 p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params Matrix4[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, Matrix3 p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        /// <summary>
        /// Passes a uniform value into the shaders of this program by location.
        /// </summary>
        public void Uniform(int location, params Matrix3[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        /// <summary>
        /// Passes a texture sampler into the fragment shader of this program by name.
        /// The name is cached so that retrieving the sampler's location is only required once.
        /// </summary>
        public void Sampler(string name, BaseRenderTexture tref, int textureUnit)
        {
            Engine.Renderer.SetActiveTexture(textureUnit);
            Uniform(name, textureUnit);
            tref?.Bind();
        }
        /// <summary>
        /// Passes a texture sampler value into the fragment shader of this program by name.
        /// The name is cached so that retrieving the sampler's location is only required once.
        /// </summary>
        public void Sampler(int location, BaseRenderTexture tref, int textureUnit)
        {
            Engine.Renderer.SetActiveTexture(textureUnit);
            Uniform(location, textureUnit);
            tref?.Bind();
        }

        public IEnumerator<RenderShader> GetEnumerator()
            => ((IEnumerable<RenderShader>)_shaders).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<RenderShader>)_shaders).GetEnumerator();
    }
}
