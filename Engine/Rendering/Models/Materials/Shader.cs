using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum ShaderMode
    {
        Fragment,           // https://www.opengl.org/wiki/Fragment_Shader
        Vertex,             // https://www.opengl.org/wiki/Vertex_Shader
        Geometry,           // https://www.opengl.org/wiki/Geometry_Shader
        TessEvaluation,     // https://www.opengl.org/wiki/Tessellation_Evaluation_Shader
        TessControl,        // https://www.opengl.org/wiki/Tessellation_Control_Shader
        Compute             // https://www.opengl.org/wiki/Compute_Shader
    }
    public class Shader
    {
        public event EventHandler Compiled;

        public bool NeedsCompile => _sourceChanged;
        public ShaderMode ShaderType => _type;

        private bool _sourceChanged = false;
        [TSerialize]
        private ShaderMode _type;
        [TSerialize("Sources")]
        internal TextFileRef[] _sources;

        public Shader(ShaderMode type)
        {
            _type = type;
        }
        public Shader(ShaderMode type, string source)
        {
            _type = type;
            _sources = new string[] { source };
            _sourceChanged = true;
        }
        public Shader(ShaderMode type, params string[] sources)
        {
            _type = type;
            _sources = sources;
            _sourceChanged = true;
        }
        public void SetSource(string source)
        {
            _sources = new string[] { source };
            _sourceChanged = true;
        }
        public void SetSources(string[] sources)
        {
            _sources = sources;
            _sourceChanged = true;
        }
        internal int Compile()
        {
            _sourceChanged = false;

            Engine.Renderer.SetShaderMode(_type);
            int id = Engine.Renderer.GenerateShader(_sources);

            Compiled?.Invoke(this, null);

            return id;
        }
    }
}
