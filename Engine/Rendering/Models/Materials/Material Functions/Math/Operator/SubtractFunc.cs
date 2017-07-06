namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A - B",
                "Returns A - B.",
                "subtracted - subtraction")]
    public class SubtractFunc : OperatorFunc
    {
        public SubtractFunc() : base() { }
        protected override string GetOperator() => "-";
    }
}
