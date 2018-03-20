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
        }

        public bool Generate() => MaterialGenerator.Generate(
            this, out ShaderFile[] shaderFiles, out ShaderVar[] shaderVars);
    }
}