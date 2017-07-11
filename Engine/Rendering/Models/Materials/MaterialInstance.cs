namespace TheraEngine.Rendering.Models.Materials
{
    public class MaterialInstance
    {
        Material _material;
        ShaderVar[] _parameters;

        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public ShaderVar[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
    }
}
