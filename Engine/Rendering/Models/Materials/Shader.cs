using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
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

        public bool NeedsCompile { get { return _sourceChanged; } }

        bool _sourceChanged = false;
        public ShaderMode _type;
        public string _source;

        public Shader(ShaderMode type)
        {
            _type = type;
        }
        public Shader(ShaderMode type, string source)
        {
            _type = type;
            _source = source;
            _sourceChanged = true;
        }
        public void SetSource(string source)
        {
            _source = source;
            _sourceChanged = true;
        }
        public void Generate(ResultBasicFunc resultFunction)
        {
            _source = "";
        }
        public int Compile()
        {
            if (!_sourceChanged)
                return 0;

            _sourceChanged = false;

            Engine.Renderer.SetShaderMode(_type);
            int id = Engine.Renderer.GenerateShader(_source);

            Compiled?.Invoke(this, null);

            return id;
        }
    }
}
