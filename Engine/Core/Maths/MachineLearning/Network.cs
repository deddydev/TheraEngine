namespace TheraEngine.Core.Maths.MachineLearning
{
    public class Network
    {
        public NeuronSet First { get; set; }
        public NeuronSet Last { get; set; }
        public double LearningRate { get; set; }

        public NeuronSet Input { get; set; }
        public NeuronSet ExpectedOutput { get; set; }

        public void Initialize()
        {
            First?.Initialize();
        }
    }
}
