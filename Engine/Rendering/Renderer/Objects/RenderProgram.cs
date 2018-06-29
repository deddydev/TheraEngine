using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.Models.Materials;

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
        
        private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, int>> 
            _uniformCache = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>(),
            _attribCache = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();

        public int GetUniformLocation(string name)
        {
            int bindingId = BindingId;
            if (bindingId == NullBindingId)
                return -1;
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
        public int GetAttributeLocation(string name)
        {
            int bindingId = BindingId;
            if (_attribCache.TryGetValue(bindingId, out ConcurrentDictionary<string, int> progDic))
                return progDic.GetOrAdd(name, n => Engine.Renderer.OnGetAttribLocation(bindingId, n));
            else
            {
                progDic = new ConcurrentDictionary<string, int>();
                int loc = Engine.Renderer.OnGetAttribLocation(bindingId, name);
                if (!progDic.TryAdd(name, loc) || !_attribCache.TryAdd(bindingId, progDic))
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
            
            return id;
        }
        
        public void Use()
        {
            Engine.Renderer.UseProgram(BindingId);
        }

        public void Uniform(string name, params IUniformable4Int[] p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params IUniformable4Float[] p)
            => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, params IUniformable3Int[] p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params IUniformable3Float[] p)
            => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, params IUniformable2Int[] p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params IUniformable2Float[] p)
            => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, params IUniformable1Int[] p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params IUniformable1Float[] p)
            => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, params int[] p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params float[] p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params uint[] p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params double[] p)
            => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, Matrix4 p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, Matrix4[] p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, Matrix3 p)
            => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, Matrix3[] p)
            => Uniform(GetUniformLocation(name), p);

        public void Uniform(int location, params IUniformable4Int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable4Float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable4Double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable4UInt[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable4Bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        
        public void Uniform(int location, params IUniformable3Int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable3Float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable3Double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable3UInt[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable3Bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        public void Uniform(int location, params IUniformable2Int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable2Float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable2Double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable2UInt[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable2Bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        public void Uniform(int location, params IUniformable1Int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable1Float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable1Double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable1UInt[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params IUniformable1Bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        public void Uniform(int location, params int[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params float[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params double[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params uint[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params bool[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        public void Uniform(int location, Matrix4 p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params Matrix4[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, Matrix3 p)
            => Engine.Renderer.Uniform(BindingId, location, p);
        public void Uniform(int location, params Matrix3[] p)
            => Engine.Renderer.Uniform(BindingId, location, p);

        public IEnumerator<RenderShader> GetEnumerator()
            => ((IEnumerable<RenderShader>)_shaders).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<RenderShader>)_shaders).GetEnumerator();
    }
}
