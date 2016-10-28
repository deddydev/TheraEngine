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
    public class Shader : BaseRenderState
    {
        bool _sourceChanged = false;
        public ShaderMode _type;
        public string _source;

        public bool NeedsCompile { get { return _sourceChanged; } }

        public Shader(ShaderMode type) : base(GenType.Shader)
        {
            _type = type;
        }
        public Shader(ShaderMode type, string source) : base(GenType.Shader)
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

        public event EventHandler Compiled;
        public int Compile()
        {
            if (!_sourceChanged)
                return 0;

            _sourceChanged = false;
            int id = Engine.Renderer.GenerateShader(this);

            return id;
        }

        public List<MaterialParameter> CollectUniformCommands(MaterialFunction cmd)
        {
            foreach (GLVar arg in cmd.InputArguments)
            {
                
            }
            return null;
        }
    }
}
