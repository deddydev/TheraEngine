using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MaterialInstance
    {
        private Material _material;
        private List<GLVar> _parameters = new List<GLVar>();

        public MaterialInstance(Material material)
        {
            _material = material;
        }
        public void SetUniforms()
        {
            _parameters.ForEach(x => x.SetUniform());
        }
    }
}
