using Extensions;
using System;
using TheraEngine.Core.Tools;

namespace TheraEditor.Windows.Forms
{
    public class NumericInputBoxSingle : NumericInputBoxBase<Single>
    {
        public NumericInputBoxSingle()
        {
            SmallerIncrement = 0.1f;
            SmallIncrement = 1.0f;
            LargeIncrement = 15.0f;
            LargerIncrement = 90.0f;
        }

        protected override Single Clamp(Single value, Single min, Single max)
            => value.Clamp(min, max);
        protected override Single ClampMin(Single value, Single min)
            => value.ClampMin(min);
        protected override Single ClampMax(Single value, Single max)
            => value.ClampMax(max);
        protected override Single Round(Single value)
        {
            if (_enforcedDecimals == 0)
                value = (Single)Math.Round(value);
            else if (_enforcedDecimals > 0)
                value = (Single)Math.Round(value, _enforcedDecimals, _midPointRounding);
            return value;
        }
        protected override Single Increment(Single value, Single increment, bool negative)
            => value + (negative ? -increment : increment);
        protected override bool NumbersAreEqual(Single? value1, Single? value2)
        {
            bool null1 = value1 is null;
            bool null2 = value2 is null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value.EqualTo(value2.Value);
        }
        protected override bool TryParse(string text, out Single value)
        {
            try
            {
                value = ExpressionParser.Evaluate<Single>(text, null);
                return true;
            }
            catch
            {
                value = DefaultValue;
                return false;
            }

            //return Single.TryParse(text, out value);
        }
        public override Single MinimumValue { get; set; } = Single.MinValue;
        public override Single MaximumValue { get; set; } = Single.MaxValue;
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
