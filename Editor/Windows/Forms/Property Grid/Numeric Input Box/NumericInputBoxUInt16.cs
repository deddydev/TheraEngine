using Extensions;
using System;
using TheraEngine.Core.Tools;

namespace TheraEditor.Windows.Forms
{
    public class NumericInputBoxUInt16 : NumericInputBoxBase<UInt16>
    {
        protected override UInt16 Clamp(UInt16 value, UInt16 min, UInt16 max)
            => value.Clamp(min, max);
        protected override UInt16 ClampMin(UInt16 value, UInt16 min)
            => value.ClampMin(min);
        protected override UInt16 ClampMax(UInt16 value, UInt16 max)
            => value.ClampMax(max);
        protected override UInt16 Round(UInt16 value)
            => value;
        protected override UInt16 Increment(UInt16 value, UInt16 increment, bool negative)
            => (UInt16)(value + (negative ? -increment : increment));
        protected override bool NumbersAreEqual(UInt16? value1, UInt16? value2)
        {
            bool null1 = value1 is null;
            bool null2 = value2 is null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value == value2.Value;
        }
        protected override bool TryParse(string text, out UInt16 value)
        {
            try
            {
                value = ExpressionParser.Evaluate<UInt16>(text, null);
                return true;
            }
            catch
            {
                value = DefaultValue;
                return false;
            }

            //return UInt16.TryParse(text, out value);
        }
        public override UInt16 MinimumValue { get; set; } = UInt16.MinValue;
        public override UInt16 MaximumValue { get; set; } = UInt16.MaxValue;
        public override bool Integral => true;
        public override bool Signed => false;
    }
}
