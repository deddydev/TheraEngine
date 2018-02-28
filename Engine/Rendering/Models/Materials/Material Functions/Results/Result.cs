namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class ResultFunc : MaterialFunction
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

        public ResultFunc() : base() { }
        public ResultFunc(bool inline) : base(false) { }
        
        protected override string GetOperation()
        {
            return FragmentShaderGenerator.OutputColorName + " = {0}";
        }
    }
}