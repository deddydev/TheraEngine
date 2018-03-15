namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class ComparableFunc : ShaderMethod
    {
        public const string CategoryName = "Comparisons";
        
        public ComparableFunc() : base(ShaderVarType._bool) { }
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput A = new MatFuncValueInput("A", OutputArguments[0]);
            MatFuncValueInput B = new MatFuncValueInput("B", A);
            return new MatFuncValueInput[] { A, B };
        }
        protected override string GetOperation()
            => "{0} " + GetOperator() + " {1}";
        protected abstract string GetOperator();
    }
}
