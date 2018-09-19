﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Core.Maths.MachineLearning
{
    public class Neuron
    {
        public float Weight { get; set; }
        public float Value { get; set; }
        public float WeightedValue => Weight * Value;
        public static float ImportanceSum(List<Neuron> neurons, float bias)
            => TMath.Sigmoidf(neurons.Sum(x => x.WeightedValue) - bias);

        public void Initialize()
        {

        }
    }
}
