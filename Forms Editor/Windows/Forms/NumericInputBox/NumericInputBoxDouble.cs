using System;

namespace TheraEditor.Windows.Forms
{
    public class NumericInputBoxDouble : NumericInputBoxBase<Double>
    {
        public NumericInputBoxDouble()
        {
            SmallerIncrement = 0.1;
            SmallIncrement = 1.0;
            LargeIncrement = 15.0;
            LargerIncrement = 90.0;
        }

        protected override Double Clamp(Double value, Double min, Double max)
            => value.Clamp(min, max);
        protected override Double ClampMin(Double value, Double min)
            => value.ClampMin(min);
        protected override Double ClampMax(Double value, Double max)
            => value.ClampMax(max);
        protected override Double Round(Double value)
        {
            if (_enforcedDecimals == 0)
                value = Math.Round(value);
            else if (_enforcedDecimals > 0)
                value = Math.Round(value, _enforcedDecimals, _midPointRounding);
            return value;
        }
        protected override Double Increment(Double value, Double increment, bool negative)
            => value + (negative ? -increment : increment);
        protected override bool NumbersAreEqual(Double? value1, Double? value2)
        {
            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value.EqualTo(value2.Value);
        }
        protected override bool TryParse(string text, out Double value)
            => Double.TryParse(text, out value);
        public override Double MinimumValue => Double.MinValue;
        public override Double MaximumValue => Double.MaxValue;
        public override bool Integral => false;
        public override bool Signed => true;
        private int _enforcedDecimals = -1;
        private MidpointRounding _midPointRounding = MidpointRounding.AwayFromZero;
        public int AllowedDecimalPlaces
        {
            get => _enforcedDecimals;
            set
            {
                _enforcedDecimals = value.ClampMin(-1);
                Value = Value;
                UpdateTextWithValue();
            }
        }
        public MidpointRounding MidpointRoundingMethod
        {
            get => _midPointRounding;
            set
            {
                _midPointRounding = value;
                UpdateTextWithValue();
            }
        }
    }
}
