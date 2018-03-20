namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class ResultFunc : ShaderMethod
    {
        private TMaterial _material;
        public TMaterial Material
        {
            get => _material;
            set
            {
                _material = value;
            }
        }

        public ResultFunc() : base(false)
        {
            HasGlobalVarDec = true;
            ReturnsInline = false;
        }

        public bool Generate(out ShaderFile[] shaderFiles, out ShaderVar[] shaderVars) 
            => MaterialGenerator.Generate(this, out shaderFiles, out shaderVars);
    }
}