using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Core.Reflection.Attributes
{
    public class DragRangeAttribute : Attribute
    {
        public float Minimum { get; set; }
        public float Maximum { get; set; }
        public DragRangeAttribute(float min, float max)
        {
            Minimum = min;
            Maximum = max;
        }
    }
}
