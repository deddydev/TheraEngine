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
    public class NumericInputBoxUInt64 : NumericInputBoxBase<UInt64>
    {
        protected override UInt64 Clamp(UInt64 value, UInt64 min, UInt64 max)
            => value.Clamp(min, max);
        protected override UInt64 ClampMin(UInt64 value, UInt64 min)
            => value.ClampMin(min);
        protected override UInt64 ClampMax(UInt64 value, UInt64 max)
            => value.ClampMax(max);
        protected override UInt64 Round(UInt64 value)
            => value;
        protected override UInt64 Increment(UInt64 value, UInt64 increment, bool negative)
            => value + (UInt64)(negative ? -(long)increment : (long)increment);
        protected override bool NumbersAreEqual(UInt64? value1, UInt64? value2)
        {
            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value == value2.Value;
        }
        protected override bool TryParse(string text, out UInt64 value)
            => UInt64.TryParse(text, out value);
        public override UInt64 MinimumValue => UInt64.MinValue;
        public override UInt64 MaximumValue => UInt64.MaxValue;
        public override bool Integral => true;
        public override bool Signed => false;
    }
}
