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
    [File3rdParty(
        "fs",   "vs",   "gs",   "tcs",  "tes",  "cs",
        "frag", "vert", "geom", "tesc", "tese", "comp")]
    [FileExt("shader")]
    [FileDef("Shader")]
    public class ShaderFile : TFileObject
    {
        public event Action SourceChanged;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public ShaderMode Type { get; set; }
        public GlobalFileRef<TextFile>[] Sources => _sources;
        
        [TSerialize(nameof(Sources))]
        private GlobalFileRef<TextFile>[] _sources;

        #region Constructors
        public ShaderFile() { }
        public ShaderFile(ShaderMode type)
        {
            Type = type;
        }
        public ShaderFile(ShaderMode type, string source)
        {
            Type = type;
            _sources = new GlobalFileRef<TextFile>[] { TextFile.FromText(source) };
        }
        public ShaderFile(ShaderMode type, params string[] sources)
        {
            Type = type;
            _sources = sources.Select(x => new GlobalFileRef<TextFile>(x)).ToArray();
        }
        public ShaderFile(ShaderMode type, params TextFile[] files)
        {
            Type = type;
            _sources = files.Select(x => new GlobalFileRef<TextFile>(x)).ToArray();
        }
        public ShaderFile(ShaderMode type, params GlobalFileRef<TextFile>[] references)
        {
            Type = type;
            _sources = references;
        }
        #endregion

        public void SetSource(string source)
        {
            _sources = new GlobalFileRef<TextFile>[] { TextFile.FromText(source) };
            OnSourceChanged();
        }
        public void SetSources(string[] sources)
        {
            _sources = sources.Select(x => new GlobalFileRef<TextFile>(TextFile.FromText(x))).ToArray();
            OnSourceChanged();
        }

        private void OnSourceChanged() => SourceChanged?.Invoke();
    }
}
