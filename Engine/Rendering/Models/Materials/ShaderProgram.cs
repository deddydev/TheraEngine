using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class ShaderProgram : BaseRenderState
    {
        bool _shaderChanged = false;
        List<Shader> _shaders;

        public bool NeedsRemake { get { return _shaderChanged; } }

        public ShaderProgram(params Shader[] shaders) : base(GenType.Program)
        {
            _shaders = shaders.ToList();
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
            return Engine.Renderer.GenerateShader(this);
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
