using System;

namespace TheraEditor.Windows.Forms
{
    public class NumericInputBoxInt64 : NumericInputBoxBase<Int64>
    {
        protected override Int64 Clamp(Int64 value, Int64 min, Int64 max)
            => value.Clamp(min, max);
        protected override Int64 ClampMin(Int64 value, Int64 min)
            => value.ClampMin(min);
        protected override Int64 ClampMax(Int64 value, Int64 max)
            => value.ClampMax(max);
        protected override Int64 Round(Int64 value)
            => value;
        protected override Int64 Increment(Int64 value, Int64 increment, bool negative)
            => value + (negative ? -increment : increment);
        protected override bool NumbersAreEqual(Int64? value1, Int64? value2)
        {
            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value == value2.Value;
        }
        protected override bool TryParse(string text, out Int64 value)
            => Int64.TryParse(text, out value);
        public override Int64 MinimumValue => Int64.MinValue;
        public override Int64 MaximumValue => Int64.MaxValue;
        public override bool Integral => true;
        public override bool Signed => true;
    }
}
