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
