namespace CustomEngine.Rendering.Renderer
{
    public enum ShaderType
    {
        Fragment,
        Vertex,
    }
    public class Shader
    {
        private string _code;
        private int _handle, _programHandle;
        private ShaderType _type;
        public Shader(ShaderType type)
        {
            _type = type;
        }
        public void SetCode(string code)
        {
            _code = code;
        }
        public void Compile() { }

    }
}
