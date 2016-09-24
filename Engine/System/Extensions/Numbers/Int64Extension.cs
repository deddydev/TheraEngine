using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Int64Extension
    {
        public static Int64 Reverse(this Int64 value)
        {
            return
                ((value >> 56) & 0xFF) | ((value & 0xFF) << 56) |
                ((value >> 40) & 0xFF00) | ((value & 0xFF00) << 40) |
                ((value >> 24) & 0xFF0000) | ((value & 0xFF0000) << 24) |
                ((value >> 8) & 0xFF000000) | ((value & 0xFF000000) << 8);
        }
        public static Int64 Align(this Int64 value, Int64 align)
        {
            if (align <= 1) return value;
            Int64 temp = value % align;
            if (temp != 0) value += align - temp;
            return value;
        }
        public static Int64 Clamp(this Int64 value, Int64 min, Int64 max)
        {
            return value < min ? min : value > max ? max : value;
        }
        public static Int64 ClampMin(this Int64 value, Int64 min)
        {
            return value <= min ? min : value;
        }
        public static Int64 ClampMax(this Int64 value, Int64 max)
        {
            return value >= max ? max : value;
        }
        public static Int64 RoundDownToEven(this Int64 value)
        {
            return value - (value % 2);
        }
        public static Int64 RoundUpToEven(this Int64 value)
        {
            return value + (value % 2);
        }
    }
}
