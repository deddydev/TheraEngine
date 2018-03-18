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

                //Force a recompilation.
                //TODO: recompile shaders without destroying program.
                //Need to attach shaders by id to the program and recompile.
                Destroy();
            }
        }

        public int AddShader(ShaderFile shader)
        {
            _shaders.Add(new RenderShader(shader));
            Destroy();
            return _shaders.Count - 1;
        }
        public int AddShader(RenderShader shader)
        {
            _shaders.Add(shader);
            Destroy();
            return _shaders.Count - 1;
        }
        public void RemoveShader(RenderShader shader)
        {
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

        protected override int CreateObject()
        {
            int[] ids = _shaders.Where(x => x != null).Select(x => x.BindingId).ToArray();
            return Engine.Renderer.GenerateProgram(ids, Engine.Settings.AllowShaderPipelines);
        }

        public void Use() => Engine.Renderer.UseProgram(BindingId);

        public IEnumerator<RenderShader> GetEnumerator()
            => ((IEnumerable<RenderShader>)_shaders).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<RenderShader>)_shaders).GetEnumerator();
    }
}
