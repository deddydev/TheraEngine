using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Core.Maths.MachineLearning
{
    public class NeuronSet
    {
        private NeuronSet _previous;
        private NeuronSet _next;
        private List<Neuron> _neurons;

        public NeuronSet Next { get => _next; set => _next = value; }
        public NeuronSet Previous { get => _previous; set => _previous = value; }
        public List<Neuron> Neurons { get => _neurons; set => _neurons = value; }

        public void Initialize()
        {
            foreach (Neuron n in Neurons)
                n.Initialize();
            
            Next?.Initialize();
        }
    }
}
