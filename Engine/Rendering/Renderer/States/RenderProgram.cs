using TheraEngine.Rendering.Models.Materials;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace TheraEngine.Rendering
{
    public class RenderProgram : BaseRenderState, IEnumerable<RenderShader>
    {
        private List<RenderShader> _shaders;
        protected List<RenderShader> Shaders
        {
            get => _shaders;
            set
            {
                _shaders = value;

                if (_shaders.Any(x => x == null))
                    _shaders = _shaders.Where(x => x != null).ToList();

                //Force a recompilation.
                //TODO: recompile shaders without destroying program.
                //Need to attach shaders by id to the program and recompile.
                Destroy();
            }
        }
        public bool IsValid { get; private set; } = false;

        public int AddShader(ShaderFile shader)
        {
            if (shader == null)
                return -1;
            _shaders.Add(new RenderShader(shader));
            Destroy();
            return _shaders.Count - 1;
        }
        public int AddShader(RenderShader shader)
        {
            if (shader == null)
                return -1;
            _shaders.Add(shader);
            Destroy();
            return _shaders.Count - 1;
        }
        public void RemoveShader(RenderShader shader)
        {
            if (shader == null)
                return;
            _shaders.Remove(shader);
            Destroy();
        }

        public void SetShaders(params ShaderFile[] shaders)
            => Shaders = shaders.Select(x => new RenderShader(x)).ToList();
        public void SetShaders(IEnumerable<ShaderFile> shaders)
            => Shaders = shaders.Select(x => new RenderShader(x)).ToList();
        public void SetShaders(IEnumerable<RenderShader> shaders)
            => Shaders = shaders.ToList();
        public void SetShaders(params RenderShader[] shaders)
            => Shaders = shaders.ToList();

        public RenderProgram(params ShaderFile[] shaders)
            : base(EObjectType.Program) => SetShaders(shaders);
        public RenderProgram(IEnumerable<ShaderFile> shaders)
            : base(EObjectType.Program) => SetShaders(shaders);
        public RenderProgram(IEnumerable<RenderShader> shaders)
            : base(EObjectType.Program) => SetShaders(shaders);
        public RenderProgram(params RenderShader[] shaders)
            : base(EObjectType.Program) => SetShaders(shaders);

        public override void Destroy()
        {
            base.Destroy();
            IsValid = false;
        }

        protected override int CreateObject()
        {
            IsValid = true;

            int id = Engine.Renderer.GenerateProgram(Engine.Settings.AllowShaderPipelines);

            //Generate shader objects
            RenderShader shader;
            for (int i = 0; i < _shaders.Count; ++i)
            {
                shader = _shaders[i];
                shader.Generate();
                if (IsValid = IsValid && shader.IsCompiled)
                    Engine.Renderer.AttachShader(shader.BindingId, id);
                else
                    return id;
            }

            bool valid = Engine.Renderer.LinkProgram(id, out string info);
            if (!(IsValid = IsValid && valid))
            {
                if (info.Contains("Vertex"))
                {
                    RenderShader s = _shaders.FirstOrDefault(x => x.File.Type == ShaderMode.Vertex);
                    string source = s.GetSource(true, true);
                    Engine.PrintLine(source);
                }
                else if (info.Contains("Geometry"))
                {
                    RenderShader s = _shaders.FirstOrDefault(x => x.File.Type == ShaderMode.Geometry);
                    string source = s.GetSource(true, true);
                    Engine.PrintLine(source);
                }
                else if (info.Contains("Fragment"))
                {
                    RenderShader s = _shaders.FirstOrDefault(x => x.File.Type == ShaderMode.Fragment);
                    string source = s.GetSource(true, true);
                    Engine.PrintLine(source);
                }
                return id;
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
