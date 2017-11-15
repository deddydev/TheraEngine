using System;

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
