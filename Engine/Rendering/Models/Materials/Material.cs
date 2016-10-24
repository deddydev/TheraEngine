using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Material
    {
        private List<MaterialProperty> _properties = new List<MaterialProperty>();
        public ResultBasicFunc _resultCommand;
        private bool _cullFront = false, _cullBack = true;

        public Shader _vertexShader, _fragmentShader;
        
        public void Compile()
        {
            _vertexShader.Compile(_resultCommand);
            _fragmentShader.Compile(_resultCommand);
        }
    }
}
