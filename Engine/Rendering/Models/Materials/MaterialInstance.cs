using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public class MaterialInstance
    {
        [TSerialize]
        SingleFileRef<Material> _material = new SingleFileRef<Material>();

        [TSerialize]
        ShaderVar[] _parameters;

        public SingleFileRef<Material> Material => _material;
        public ShaderVar[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
    }
}
