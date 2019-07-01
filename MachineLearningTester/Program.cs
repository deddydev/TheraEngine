using System;
using System.Diagnostics;
using TheraEngine.Core.Maths.MachineLearning;

namespace MachineLearningTester
{
    class Program
    {
        static void Main(string[] args)
        {
            double rate = 0.2;
            int iters = 10000000;

            TrainLogicOp(ELogicalOpType.OR, rate, iters);
            TrainLogicOp(ELogicalOpType.AND, rate, iters);
            TrainLogicOp(ELogicalOpType.XOR, rate, iters);
            TrainLogicOp(ELogicalOpType.NOR, rate, iters);
            TrainLogicOp(ELogicalOpType.XNOR, rate, iters);
            TrainLogicOp(ELogicalOpType.NAND, rate, iters);
        }

        private enum ELogicalOpType
        {
            OR,
            AND,
            XOR,
            NOR,
            XNOR,
            NAND,
        }

        private static void TrainLogicOp(ELogicalOpType type, double learningRate, int iterations)
        {
            Tuple<double[], double[]>[] inputOutput = GetBinOp(type);

            Console.WriteLine($"Training {type.ToString()} network.");

            Network nw = new Network(2,
                new CF_DiffSquared(),
                new NeuronLayer(new AF_Sigmoid(), 2, true),
                new NeuronLayer(new AF_Sigmoid(), 1, true));
            //nw.CostChanged += CostChanged;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            nw.Train(iterations, learningRate, inputOutput);

            timer.Stop();

            Console.WriteLine($"{nw.TotalIterationsTrained} iterations finished in {timer.Elapsed.TotalSeconds} seconds.");

            double totalError;

            double[] zz = nw.CalculateError(inputOutput[0].Item1, inputOutput[0].Item2, out totalError);
            Console.WriteLine("0, 0: " + zz.ToStringList(", ", o => o.Round(2).ToString()));

            double[] oz = nw.CalculateError(inputOutput[1].Item1, inputOutput[1].Item2, out totalError);
            Console.WriteLine("1, 0: " + oz.ToStringList(", ", o => o.Round(2).ToString()));

            double[] zo = nw.CalculateError(inputOutput[2].Item1, inputOutput[2].Item2, out totalError);
            Console.WriteLine("0, 1: " + zo.ToStringList(", ", o => o.Round(2).ToString()));

            double[] oo = nw.CalculateError(inputOutput[3].Item1, inputOutput[3].Item2, out totalError);
            Console.WriteLine("1, 1: " + oo.ToStringList(", ", o => o.Round(2).ToString()));
        }

        private static void CostChanged(double oldCost, double newCost)
        {
            Console.WriteLine(newCost);
        }

        private static Tuple<double[], double[]>[] GetBinOp(ELogicalOpType type)
        {
            Tuple<double[], double[]>[] inputOutput = new Tuple<double[], double[]>[4];
            switch (type)
            {
                case ELogicalOpType.XOR:
                    inputOutput[0] = new Tuple<double[], double[]>(new double[] { 0.0, 0.0 }, new double[] { 0.0 });
                    inputOutput[1] = new Tuple<double[], double[]>(new double[] { 0.0, 1.0 }, new double[] { 1.0 });
                    inputOutput[2] = new Tuple<double[], double[]>(new double[] { 1.0, 0.0 }, new double[] { 1.0 });
                    inputOutput[3] = new Tuple<double[], double[]>(new double[] { 1.0, 1.0 }, new double[] { 0.0 });
                    break;
                case ELogicalOpType.XNOR:
                    inputOutput[0] = new Tuple<double[], double[]>(new double[] { 0.0, 0.0 }, new double[] { 1.0 });
                    inputOutput[1] = new Tuple<double[], double[]>(new double[] { 0.0, 1.0 }, new double[] { 0.0 });
                    inputOutput[2] = new Tuple<double[], double[]>(new double[] { 1.0, 0.0 }, new double[] { 0.0 });
                    inputOutput[3] = new Tuple<double[], double[]>(new double[] { 1.0, 1.0 }, new double[] { 1.0 });
                    break;
                case ELogicalOpType.AND:
                    inputOutput[0] = new Tuple<double[], double[]>(new double[] { 0.0, 0.0 }, new double[] { 0.0 });
                    inputOutput[1] = new Tuple<double[], double[]>(new double[] { 0.0, 1.0 }, new double[] { 0.0 });
                    inputOutput[2] = new Tuple<double[], double[]>(new double[] { 1.0, 0.0 }, new double[] { 0.0 });
                    inputOutput[3] = new Tuple<double[], double[]>(new double[] { 1.0, 1.0 }, new double[] { 1.0 });
                    break;
                case ELogicalOpType.NOR:
                    inputOutput[0] = new Tuple<double[], double[]>(new double[] { 0.0, 0.0 }, new double[] { 1.0 });
                    inputOutput[1] = new Tuple<double[], double[]>(new double[] { 0.0, 1.0 }, new double[] { 0.0 });
                    inputOutput[2] = new Tuple<double[], double[]>(new double[] { 1.0, 0.0 }, new double[] { 0.0 });
                    inputOutput[3] = new Tuple<double[], double[]>(new double[] { 1.0, 1.0 }, new double[] { 0.0 });
                    break;
                case ELogicalOpType.OR:
                    inputOutput[0] = new Tuple<double[], double[]>(new double[] { 0.0, 0.0 }, new double[] { 0.0 });
                    inputOutput[1] = new Tuple<double[], double[]>(new double[] { 0.0, 1.0 }, new double[] { 1.0 });
                    inputOutput[2] = new Tuple<double[], double[]>(new double[] { 1.0, 0.0 }, new double[] { 1.0 });
                    inputOutput[3] = new Tuple<double[], double[]>(new double[] { 1.0, 1.0 }, new double[] { 1.0 });
                    break;
                case ELogicalOpType.NAND:
                    inputOutput[0] = new Tuple<double[], double[]>(new double[] { 0.0, 0.0 }, new double[] { 1.0 });
                    inputOutput[1] = new Tuple<double[], double[]>(new double[] { 0.0, 1.0 }, new double[] { 1.0 });
                    inputOutput[2] = new Tuple<double[], double[]>(new double[] { 1.0, 0.0 }, new double[] { 1.0 });
                    inputOutput[3] = new Tuple<double[], double[]>(new double[] { 1.0, 1.0 }, new double[] { 0.0 });
                    break;
            }
            return inputOutput;
        }
    }
}
