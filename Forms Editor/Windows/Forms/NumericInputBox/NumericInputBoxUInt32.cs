using System;
using TheraEngine.Core.Tools;

namespace TheraEditor.Windows.Forms
{
    public class NumericInputBoxUInt32 : NumericInputBoxBase<UInt32>
    {
        protected override UInt32 Clamp(UInt32 value, UInt32 min, UInt32 max)
            => value.Clamp(min, max);
        protected override UInt32 ClampMin(UInt32 value, UInt32 min)
            => value.ClampMin(min);
        protected override UInt32 ClampMax(UInt32 value, UInt32 max)
            => value.ClampMax(max);
        protected override UInt32 Round(UInt32 value)
            => value;
        protected override UInt32 Increment(UInt32 value, UInt32 increment, bool negative)
            => (UInt32)(value + (negative ? -increment : increment));
        protected override bool NumbersAreEqual(UInt32? value1, UInt32? value2)
        {
            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value == value2.Value;
        }
        protected override bool TryParse(string text, out UInt32 value)
        {
            try
            {
                value = ExpressionParser.Evaluate<UInt32>(text, null);
                return true;
            }
            catch
            {
                value = DefaultValue;
                return false;
            }

            //return UInt32.TryParse(text, out value);
        }
        public override UInt32 MinimumValue { get; set; } = UInt32.MinValue;
        public override UInt32 MaximumValue { get; set; } = UInt32.MaxValue;
        public override bool Integral => true;
        public override bool Signed => false;
    }
}
