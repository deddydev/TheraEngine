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
    public class BaseNumericInputBox : TextBox { }
    public abstract class NumericInputBoxBase<T> : BaseNumericInputBox
        where T : struct, IFormattable, IComparable, IConvertible
    {
        public delegate void BoxValueChanged(NumericInputBoxBase<T> box, T? previous, T? current);
        public event BoxValueChanged ValueChanged;

        public NumericInputBoxBase()
        {
            UpdateTextWithValue();
        }
        
        public T? _previousValue = null;
        public T? _currentValue = null;
        private bool _nullable = false;
        private T _defaultValue;
        
        public T LargeIncrement { get; set; }
        public T LargerIncrement { get; set; }
        public T SmallIncrement { get; set; }
        public T SmallerIncrement { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T? Value
        {
            get => _currentValue;
            set
            {
                T? newValue = value == null ?
                    (Nullable ? null : (T?)DefaultValue) :
                    Round(Clamp(value.Value, MinimumValue, MaximumValue));

                if (!NumbersAreEqual(_currentValue, newValue))
                {
                    _previousValue = _currentValue;
                    _currentValue = newValue;
                    ValueChanged?.Invoke(this, _previousValue, _currentValue);
                }

                UpdateTextWithValue();
            }
        }

        protected abstract T Clamp(T value, T min, T max);
        protected abstract T ClampMin(T value, T min);
        protected abstract T Round(T value);
        protected abstract T ClampMax(T value, T max);
        protected abstract T Increment(T value, T increment, bool negative);
        protected abstract bool NumbersAreEqual(T? value1, T? value2);
        protected abstract bool TryParse(string text, out T value);

        public abstract T MinimumValue { get; }
        public abstract T MaximumValue { get; }
        public abstract bool Integral { get; }
        public abstract bool Signed { get; }

        //private int _enforcedDecimals = -1;
        //private MidpointRounding _midPointRounding = MidpointRounding.AwayFromZero;
        //public int AllowedDecimalPlaces
        //{
        //    get => _enforcedDecimals;
        //    set
        //    {
        //        _enforcedDecimals = value.ClampMin(-1);
        //        UpdateTextWithValue();
        //    }
        //}
        //public MidpointRounding MidpointRoundingMethod
        //{
        //    get => _midPointRounding;
        //    set
        //    {
        //        _midPointRounding = value;
        //        UpdateTextWithValue();
        //    }
        //}
        private string _prefix = "", _suffix = "";
        public string NumberPrefix
        {
            get => _prefix;
            set
            {
                if (_prefix != null && !string.IsNullOrWhiteSpace(Text) && Text.StartsWith(_prefix))
                    Text = Text.Substring(_prefix.Length);
                _prefix = value;
                if (_prefix != null && !string.IsNullOrWhiteSpace(Text))
                    Text = _prefix + Text;
            }
        }
        public string NumberSuffix
        {
            get => _suffix;
            set
            {
                if (_suffix != null && !string.IsNullOrWhiteSpace(Text) && Text.EndsWith(_suffix))
                    Text = Text.Substring(0, Text.Length - _suffix.Length);
                _suffix = value;
                if (_suffix != null && !string.IsNullOrWhiteSpace(Text))
                    Text = Text + _suffix;
            }
        }
        public bool Nullable
        {
            get => _nullable;
            set
            {
                _nullable = value;
                if (!_nullable && _currentValue == null)
                    Value = DefaultValue;
            }
        }
        public T DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Text = GetTextValue();
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            //Submit text input
            GetValueFromText();
            if (_prefix != null && !Text.StartsWith(_prefix))
                Text = _prefix + Text;
            if (_suffix != null && !Text.EndsWith(_suffix))
                Text = Text + _suffix;
            base.OnLostFocus(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!Focused)
                return;
            GetValueFromText();
            if (_currentValue != null)
            {
                _currentValue = Clamp(Increment(_currentValue.Value, SmallIncrement, e.Delta < 0), MinimumValue, MaximumValue);
                UpdateTextWithValue();
            }
            base.OnMouseWheel(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Back:
                case Keys.Left:
                case Keys.Right:
                    break;
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    if (Signed && Text.IndexOf('-') >= SelectionStart)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Down:
                case Keys.Up:
                    {
                        GetValueFromText();
                        if (_currentValue == null)
                            return;
                        T increment = e.KeyCode == Keys.Down || e.KeyCode == Keys.Up ? (e.Shift ? SmallerIncrement : SmallIncrement) : (e.Shift ? LargerIncrement : LargeIncrement);
                        Value = Increment(_currentValue.Value, increment, e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Down);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.Subtract:
                case Keys.OemMinus:
                    if (!Signed || SelectionStart != 0 || Text.IndexOf('-') != -1)
                        e.SuppressKeyPress = true;
                    break;

                case Keys.Decimal:
                case Keys.OemPeriod:
                    if (Integral || Text.IndexOf('.') != -1)
                        e.SuppressKeyPress = true;
                    break;

                case Keys.Escape:
                    //Reset text with current, unchanged value
                    UpdateTextWithValue();
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Enter:
                    GetValueFromText();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.X:
                    if (e.Control)
                    {
                        Text = "";
                        GetValueFromText();
                    }
                    break;
                case Keys.V:
                case Keys.C:
                    if (!e.Control)
                        goto default;
                    break;

                default:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
            base.OnKeyDown(e);
        }

        protected void UpdateTextWithValue()
            => Text = _currentValue == null ? "" : Focused ? _currentValue.Value.ToString() : (_prefix + _currentValue.Value.ToString() + _suffix);

        protected void GetValueFromText()
        {
            string textValue = GetTextValue();
            if (_currentValue != null && _currentValue.Value.ToString() == textValue)
                return;

            Value = (!string.IsNullOrWhiteSpace(textValue) && TryParse(textValue, out T newValue2)) ? (T?)newValue2 : null;
        }

        protected string GetTextValue()
        {
            string textValue = Text;
            if (textValue.StartsWith(_prefix))
                textValue = textValue.Substring(_prefix.Length, textValue.Length - _prefix.Length);
            if (textValue.EndsWith(_suffix))
                textValue = textValue.Substring(0, textValue.Length - _suffix.Length);
            return textValue;
        }
    }
}
