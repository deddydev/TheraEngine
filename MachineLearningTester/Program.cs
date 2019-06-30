using System;
using System.Diagnostics;
using TheraEngine.Core.Maths.MachineLearning;

namespace MachineLearningTester
{
    class Program
    {
        static void Main(string[] args)
        {
            TestXOR();
        }

        private static void TestXOR()
        {
            Network nw = new Network(2,
                new CF_DiffSquared(),
                new NeuronLayer(new AF_ReLU(), 4),
                new NeuronLayer(new AF_Sigmoid(), 1));

            Stopwatch timer = new Stopwatch();
            timer.Start();
            nw.Train(2500, 0.2, new double[] { 0.0, 0.0 }, 0.0);
            nw.Train(2500, 0.2, new double[] { 0.0, 1.0 }, 1.0);
            nw.Train(2500, 0.2, new double[] { 1.0, 0.0 }, 1.0);
            nw.Train(2500, 0.2, new double[] { 1.0, 1.0 }, 0.0);
            timer.Stop();
            Console.WriteLine($"10000 iterations finished in {timer.Elapsed.TotalSeconds} seconds.");

            double[] zz = nw.Execute(0.0, 0.0);
            Console.WriteLine("0, 0: " + zz.ToStringList(", "));

            double[] oz = nw.Execute(1.0, 0.0);
            Console.WriteLine("1, 0: " + oz.ToStringList(", "));

            double[] zo = nw.Execute(0.0, 1.0);
            Console.WriteLine("0, 1: " + zo.ToStringList(", "));

            double[] oo = nw.Execute(1.0, 1.0);
            Console.WriteLine("1, 1: " + oo.ToStringList(", "));
        }
    }
}
