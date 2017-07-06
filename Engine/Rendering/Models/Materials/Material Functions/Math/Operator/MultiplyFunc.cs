namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A * B",
                "Returns A * B.",
                "multiply multiplied multiplication *")]
    public class MultiplyFunc : OperatorFunc
    {
        public MultiplyFunc() : base() { }
        protected override string GetOperator() => "*";
    }
}
