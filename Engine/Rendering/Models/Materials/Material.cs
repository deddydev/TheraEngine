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
        private bool _cullFront = false, _cullBack = true;
    }
}
