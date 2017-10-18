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
    public class NumericInputBoxByte : NumericInputBoxBase<Byte>
    {
        public NumericInputBoxByte()
        {
            SmallerIncrement = 1;
            SmallIncrement = 2;
            LargeIncrement = 5;
            LargerIncrement = 10;
        }

        protected override Byte Clamp(Byte value, Byte min, Byte max)
            => value.Clamp(min, max);
        protected override Byte ClampMin(Byte value, Byte min)
            => value.ClampMin(min);
        protected override Byte ClampMax(Byte value, Byte max)
            => value.ClampMax(max);
        protected override Byte Round(Byte value)
            => value;
        protected override Byte Increment(Byte value, Byte increment, bool negative)
            => (Byte)(value + (negative ? -increment : increment));
        protected override bool NumbersAreEqual(Byte? value1, Byte? value2)
        {
            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 && null2)
                return true;
            if (null1 || null2)
                return false;
            return value1.Value == value2.Value;
        }
        protected override bool TryParse(string text, out Byte value)
            => Byte.TryParse(text, out value);
        public override Byte MinimumValue => Byte.MinValue;
        public override Byte MaximumValue => Byte.MaxValue;
        public override bool Integral => true;
        public override bool Signed => false;
    }
}
