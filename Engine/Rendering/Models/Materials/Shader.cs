using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering.Models.Materials.MaterialCommands;

namespace CustomEngine.Rendering.Models.Materials
{
    public enum ShaderType
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
        public ShaderType _type;
        public string _source;

        public Shader(ShaderType type)
        {
            _type = type;
        }
        public Shader(ShaderType type, string source)
        {
            _type = type;
            _source = source;
        }

        public void SetSource(string source)
        {
            _source = source;
        }

        internal void Compile(ResultBasicFunc _resultCommand)
        {
            
        }

        public List<MaterialParameter> CollectUniformCommands(MaterialFunction cmd)
        {
            foreach (GLVar arg in cmd.InputArguments)
            {
                
            }
        }
    }
}
