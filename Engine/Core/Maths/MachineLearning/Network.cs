using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Core.Maths.MachineLearning
{
    public class Network
    {
        private NeuronSet _first;
        
        public NeuronSet First { get => _first; set => _first = value; }

        public void Initialize()
        {
            First?.Initialize();
        }
    }
}
