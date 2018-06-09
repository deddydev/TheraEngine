using TheraEngine.Rendering.Models.Materials;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System;

namespace TheraEngine.Rendering
{
    public class RenderProgram : BaseRenderState, IEnumerable<RenderShader>
    {
        internal static ConcurrentDictionary<int, RenderProgram> LivePrograms = new ConcurrentDictionary<int, RenderProgram>();

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

        private ConcurrentDictionary<int, ConcurrentDictionary<string, int>> _uniformCache =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();

        public int GetCachedUniformLocation(string name)
        {
            int bindingId = BindingId;
            if (_uniformCache.TryGetValue(bindingId, out ConcurrentDictionary<string, int> progDic))
                return progDic.GetOrAdd(name, n => Engine.Renderer.OnGetUniformLocation(bindingId, n));
            else
            {
                progDic = new ConcurrentDictionary<string, int>();
                int loc = Engine.Renderer.OnGetUniformLocation(bindingId, name);
                if (!progDic.TryAdd(name, loc) || !_uniformCache.TryAdd(bindingId, progDic))
                    throw new Exception();
                return loc;
            }
        }

        public bool IsValid { get; private set; } = false;

        public int AddShader(GLSLShaderFile shader)
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

        public void SetShaders(params GLSLShaderFile[] shaders)
            => Shaders = shaders.Select(x => new RenderShader(x)).ToList();
        public void SetShaders(IEnumerable<GLSLShaderFile> shaders)
            => Shaders = shaders.Select(x => new RenderShader(x)).ToList();
        public void SetShaders(IEnumerable<RenderShader> shaders)
            => Shaders = shaders.ToList();
        public void SetShaders(params RenderShader[] shaders)
            => Shaders = shaders.ToList();

        public RenderProgram(params GLSLShaderFile[] shaders)
            : base(EObjectType.Program) => SetShaders(shaders);
        public RenderProgram(IEnumerable<GLSLShaderFile> shaders)
            : base(EObjectType.Program) => SetShaders(shaders);
        public RenderProgram(IEnumerable<RenderShader> shaders)
            : base(EObjectType.Program) => SetShaders(shaders);
        public RenderProgram(params RenderShader[] shaders)
            : base(EObjectType.Program) => SetShaders(shaders);

        public override void Destroy()
        {
            if (IsActive)
                LivePrograms.TryRemove(BindingId, out RenderProgram prog);

            base.Destroy();

            IsValid = false;
            _uniformCache.Clear();
        }

        protected override int CreateObject()
        {
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
                    shader.Generate();

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

            LivePrograms.AddOrUpdate(id, this, (key, oldProg) => this);

            return id;
        }
        
        public void Use()
        {
            Engine.Renderer.UseProgram(BindingId);
        }

        public IEnumerator<RenderShader> GetEnumerator()
            => ((IEnumerable<RenderShader>)_shaders).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<RenderShader>)_shaders).GetEnumerator();
    }
}
