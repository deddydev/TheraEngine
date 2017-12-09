using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public class MaterialInstance
    {
        [TSerialize]
        SingleFileRef<TMaterial> _material = new SingleFileRef<TMaterial>();

        [TSerialize]
        ShaderVar[] _parameters;

        public SingleFileRef<TMaterial> Material => _material;
        public ShaderVar[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
    }
}
