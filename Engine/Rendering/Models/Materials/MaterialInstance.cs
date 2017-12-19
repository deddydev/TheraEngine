using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public class MaterialInstance
    {
        [TSerialize]
        GlobalFileRef<TMaterial> _material = new GlobalFileRef<TMaterial>();

        [TSerialize]
        ShaderVar[] _parameters;

        public GlobalFileRef<TMaterial> Material => _material;
        public ShaderVar[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
    }
}
