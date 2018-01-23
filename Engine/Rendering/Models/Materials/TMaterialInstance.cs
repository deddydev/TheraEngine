using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public class TMaterialInstance
    {
        [TSerialize]
        GlobalFileRef<TMaterial> _material = new GlobalFileRef<TMaterial>();

        [TSerialize]
        ShaderVar[] _parameters;

        public GlobalFileRef<TMaterial> Material
        {
            get => _material;
            set => _material = value ?? new GlobalFileRef<TMaterial>();
        }
        public ShaderVar[] Parameters
        {
            get => _parameters;
            set => _parameters = value;
        }
    }
}
