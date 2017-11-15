using System;

namespace TheraEditor.Windows.Forms
{
    public class NumericInputBoxInt16 : NumericInputBoxBase<Int16>
    {
        protected override Int16 Clamp(Int16 value, Int16 min, Int16 max)
            => value.Clamp(min, max);
        protected override Int16 ClampMin(Int16 value, Int16 min)
            => value.ClampMin(min);
        protected override Int16 ClampMax(Int16 value, Int16 max)
            => value.ClampMax(max);
        protected override Int16 Round(Int16 value)
            => value;
        protected override Int16 Increment(Int16 value, Int16 increment, bool negative)
            => (Int16)(value + (negative ? -increment : increment));
        protected override bool NumbersAreEqual(Int16? value1, Int16? value2)
        {
            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value == value2.Value;
        }
        protected override bool TryParse(string text, out Int16 value)
            => Int16.TryParse(text, out value);
        public override Int16 MinimumValue => Int16.MinValue;
        public override Int16 MaximumValue => Int16.MaxValue;
        public override bool Integral => true;
        public override bool Signed => true;
    }
}
