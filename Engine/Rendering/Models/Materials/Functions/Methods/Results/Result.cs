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

        protected override string GetOperation()
        {
            return ShaderGenerator.OutputColorName + " = {0}";
        }
        protected override string GetGlobalVarDec()
        {
            return "layout(location = 0) out vec4 OutColor;";
            return base.GetGlobalVarDec();
        }

        public ShaderFile[] GenerateShaders() => ShaderGenerator.GenerateShaders(this);
    }
}