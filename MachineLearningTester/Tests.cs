using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TheraEngine.Core.Maths.MachineLearning;
using TheraEngine.Tests;

namespace MachineLearningTester
{
    public static class Tests
    {
        public static event DelCostChanged CostChanged;
        private enum ELogicalOpType
        {
            OR,
            AND,
            XOR,
            NOR,
            XNOR,
            NAND,
        }
        public static async Task RunAll()
        {
            double rate = 0.8;
            double maxError = 0.0009;
            double momentum = 0.2;
            int digits = 0;

            //Task.WaitAll(
            await TrainLogicOp(ELogicalOpType.OR, rate, momentum, maxError, digits);
            await TrainLogicOp(ELogicalOpType.AND, rate, momentum, maxError, digits);
            await TrainLogicOp(ELogicalOpType.XOR, rate, momentum, maxError, digits);
            await TrainLogicOp(ELogicalOpType.NOR, rate, momentum, maxError, digits);
            await TrainLogicOp(ELogicalOpType.XNOR, rate, momentum, maxError, digits);
            await TrainLogicOp(ELogicalOpType.NAND, rate, momentum, maxError, digits);
        }

        private static async Task TrainLogicOp(
            ELogicalOpType type,
            double learningRate,
            double momentum,
            double maxError,
            int outputRoundingFracDigits)
        {
            Tuple<double[], double[]>[] inputOutput = GetBinOp(type);

            Console.WriteLine($"Training {type.ToString()} network.");

            Network nw = new Network(2,
                new CF_DiffSquared(),
                NeuronLayer.Dense(new AF_Sine(), 2, true),
                NeuronLayer.Dense(new AF_Logistic(), 1, true));

            Stopwatch timer = new Stopwatch();

            nw.CostChanged += CostChangedMethod;
            timer.Start();

            await Task.Run(() => nw.Train(maxError, EErrorTrainingType.Total, learningRate, momentum, inputOutput));

            timer.Stop();
            nw.CostChanged -= CostChangedMethod;

            Console.WriteLine($"{nw.TotalIterationsTrained} iterations finished in {timer.Elapsed.TotalSeconds} seconds.");

            //double totalError;

            double[] zz = nw.Calculate(inputOutput[0].Item1); //nw.CalculateError(inputOutput[0].Item1, inputOutput[0].Item2, out totalError);
            Console.WriteLine("[0, 0]: " + zz.ToStringList(", ", o => o.Round(outputRoundingFracDigits).ToString()));

            double[] oz = nw.Calculate(inputOutput[1].Item1); //nw.CalculateError(inputOutput[1].Item1, inputOutput[1].Item2, out totalError);
            Console.WriteLine("[1, 0]: " + oz.ToStringList(", ", o => o.Round(outputRoundingFracDigits).ToString()));

            double[] zo = nw.Calculate(inputOutput[2].Item1); //nw.CalculateError(inputOutput[2].Item1, inputOutput[2].Item2, out totalError);
            Console.WriteLine("[0, 1]: " + zo.ToStringList(", ", o => o.Round(outputRoundingFracDigits).ToString()));

            double[] oo = nw.Calculate(inputOutput[3].Item1); //nw.CalculateError(inputOutput[3].Item1, inputOutput[3].Item2, out totalError);
            Console.WriteLine("[1, 1]: " + oo.ToStringList(", ", o => o.Round(outputRoundingFracDigits).ToString()));

            nw.ToCSV(TestDefaults.DesktopPath + type.ToString() + ".csv");
        }

        private static void CostChangedMethod(double oldCost, double newCost, int iteration)
        {
            CostChanged?.Invoke(oldCost, newCost, iteration);
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
