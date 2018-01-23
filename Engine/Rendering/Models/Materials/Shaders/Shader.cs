using System;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Files;
using TheraEngine.Files;

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
    [FileExt("shader")]
    [FileDef("Shader")]
    public class Shader : FileObject
    {
        public event EventHandler Compiled;

        public bool NeedsCompile => _sourceChanged;
        public ShaderMode ShaderType => _type;

        private bool _sourceChanged = false;
        [TSerialize("Type", XmlNodeType = EXmlNodeType.Attribute)]
        private ShaderMode _type;
        [TSerialize("Sources")]
        private GlobalFileRef<TextFile>[] _sources;

        public Shader() { }
        public Shader(ShaderMode type)
        {
            _type = type;
        }
        public Shader(ShaderMode type, string source)
        {
            _type = type;
            _sources = new GlobalFileRef<TextFile>[] { TextFile.FromText(source) };
            _sourceChanged = true;
        }
        public Shader(ShaderMode type, params string[] sources)
        {
            _type = type;
            _sources = sources.Select(x => new GlobalFileRef<TextFile>(x)).ToArray();
            _sourceChanged = true;
        }
        public Shader(ShaderMode type, params TextFile[] files)
        {
            _type = type;
            _sources = files.Select(x => new GlobalFileRef<TextFile>(x)).ToArray();
            _sourceChanged = true;
        }
        public Shader(ShaderMode type, params GlobalFileRef<TextFile>[] references)
        {
            _type = type;
            _sources = references;
            _sourceChanged = true;
        }
        public void SetSource(string source)
        {
            _sources = new GlobalFileRef<TextFile>[] { TextFile.FromText(source) };
            _sourceChanged = true;
        }
        public void SetSources(string[] sources)
        {
            _sources = sources.Select(x => new GlobalFileRef<TextFile>(TextFile.FromText(x))).ToArray();
            _sourceChanged = true;
        }
        internal int Compile()
        {
            _sourceChanged = false;

            Engine.Renderer.SetShaderMode(_type);
            int id = Engine.Renderer.GenerateShader(_sources.Select(x => x.File?.Text).ToArray());

            Compiled?.Invoke(this, null);

            return id;
        }
    }
}
