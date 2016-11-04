using CustomEngine.Rendering.Models.Materials;
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
    public enum EQualifier
    {
        _uniform,
        _out,
        _in
    }
    public class Shader
    {
        public event EventHandler Compiled;

        public bool NeedsCompile { get { return _sourceChanged; } }

        private bool _sourceChanged = false;
        private ShaderMode _type;
        private string _source;
        private List<GLVar> _uniforms = new List<GLVar>();

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
            _source = resultFunction.Generate(_type);
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
        public static Shader TestVertexShader()
        {
            string source = @"

in vec4 Position;
in vec3 Normal;
in vec2 TexCoord;
uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjMatrix;

void main()
{
    
}
";
            return new Shader(ShaderMode.Vertex, source);
        }
        public static Shader TestFragmentShader()
        {
            string source = @"

layout (location = 0) out vec4 diffuseColor;

void main()
{
    diffuseColor = vec4(1.0, 0.0, 0.0, 1.0);
}
";
            return new Shader(ShaderMode.Fragment, source);
        }
    }
}
