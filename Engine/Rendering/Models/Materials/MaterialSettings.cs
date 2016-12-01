using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MaterialSettings
    {
        private List<MaterialProperty> _properties = new List<MaterialProperty>();
        private VertexAttribInfo[] _attrib;

        public bool CullFront { get { return _cullFront; } set { _cullFront = true; } }
        public bool CullBack { get { return _cullBack; } set { _cullBack = true; } }
        
        public MaterialSettings()
        {

        }
    }
}
