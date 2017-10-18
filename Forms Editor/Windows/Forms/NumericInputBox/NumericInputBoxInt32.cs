using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public class NumericInputBoxInt32 : NumericInputBoxBase<Int32>
    {
        protected override Int32 Clamp(Int32 value, Int32 min, Int32 max)
            => value.Clamp(min, max);
        protected override Int32 ClampMin(Int32 value, Int32 min)
            => value.ClampMin(min);
        protected override Int32 ClampMax(Int32 value, Int32 max)
            => value.ClampMax(max);
        protected override Int32 Round(Int32 value)
            => value;
        protected override Int32 Increment(Int32 value, Int32 increment, bool negative)
            => value + (negative ? -increment : increment);
        protected override bool NumbersAreEqual(Int32? value1, Int32? value2)
        {
            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value == value2.Value;
        }
        protected override bool TryParse(string text, out Int32 value)
            => Int32.TryParse(text, out value);
        public override Int32 MinimumValue => Int32.MinValue;
        public override Int32 MaximumValue => Int32.MaxValue;
        public override bool Integral => true;
        public override bool Signed => true;
    }
}
