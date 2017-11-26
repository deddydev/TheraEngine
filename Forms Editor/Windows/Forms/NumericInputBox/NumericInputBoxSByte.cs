using System;

namespace TheraEditor.Windows.Forms
{
    public class NumericInputBoxSByte : NumericInputBoxBase<SByte>
    {
        public NumericInputBoxSByte()
        {
            SmallerIncrement = 1;
            SmallIncrement = 2;
            LargeIncrement = 5;
            LargerIncrement = 10;
        }

        protected override SByte Clamp(SByte value, SByte min, SByte max)
            => value.Clamp(min, max);
        protected override SByte ClampMin(SByte value, SByte min)
            => value.ClampMin(min);
        protected override SByte ClampMax(SByte value, SByte max)
            => value.ClampMax(max);
        protected override SByte Round(SByte value)
            => value;
        protected override SByte Increment(SByte value, SByte increment, bool negative)
            => (sbyte)(value + (negative ? -increment : increment));
        protected override bool NumbersAreEqual(SByte? value1, SByte? value2)
        {
            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value == value2.Value;
        }
        protected override bool TryParse(string text, out SByte value)
            => SByte.TryParse(text, out value);
        public override SByte MinimumValue { get; set; } = SByte.MinValue;
        public override SByte MaximumValue { get; set; } = SByte.MaxValue;
        public override bool Integral => true;
        public override bool Signed => true;
    }
}
