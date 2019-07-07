using Extensions;
using System;
using TheraEngine.Core.Tools;

namespace TheraEditor.Windows.Forms
{
    public class NumericInputBoxDecimal : NumericInputBoxBase<Decimal>
    {
        protected override Decimal Clamp(Decimal value, Decimal min, Decimal max)
            => value.Clamp(min, max);
        protected override Decimal ClampMin(Decimal value, Decimal min)
            => value.ClampMin(min);
        protected override Decimal ClampMax(Decimal value, Decimal max)
            => value.ClampMax(max);
        protected override Decimal Round(Decimal value)
        {
            if (_enforcedDecimals == 0)
                value = Math.Round(value);
            else if (_enforcedDecimals > 0)
                value = Math.Round(value, _enforcedDecimals, _midPointRounding);
            return value;
        }
        protected override Decimal Increment(Decimal value, Decimal increment, bool negative)
            => value + (negative ? -increment : increment);
        protected override bool NumbersAreEqual(Decimal? value1, Decimal? value2)
        {
            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value == value2.Value;
        }
        protected override bool TryParse(string text, out Decimal value)
        {
            try
            {
                value = ExpressionParser.Evaluate<Decimal>(text, null);
                return true;
            }
            catch
            {
                value = DefaultValue;
                return false;
            }

            //return Decimal.TryParse(text, out value);
        }
        public override Decimal MinimumValue { get; set; } = Decimal.MinValue;
        public override Decimal MaximumValue { get; set; } = Decimal.MaxValue;
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
