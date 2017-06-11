using TheraEngine.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials
{
    public class MaterialInstance
    {
        Material _material;
        GLVar[] _parameters;

        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public GLVar[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
    }
}
