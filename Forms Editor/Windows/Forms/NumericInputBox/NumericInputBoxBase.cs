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
    public abstract class NumericInputBoxBase<T> : TextBox where T : struct, IFormattable, IComparable, IConvertible
    {
        public delegate void BoxValueChanged(T? previous, T? current);
        public event BoxValueChanged ValueChanged;

        public NumericInputBoxBase()
        {
            UpdateTextWithValue();
        }
        
        public T? _previousValue = null;
        public T? _currentValue = null;
        public T
            _minValue,
            _maxValue,
            _largeIncrement,
            _smallIncrement,
            _largerIncrement,
            _smallerIncrement;
        private bool _nullable = false;
        private T _defaultValue;

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T? Value
        {
            get => _currentValue;
            set
            {
                _previousValue = _currentValue;
                if (value == null)
                    _currentValue = Nullable ? null : (T?)_defaultValue;
                else
                    _currentValue = Round(Clamp(value.Value, _minValue, _maxValue));

                UpdateTextWithValue();
            }
        }

        protected abstract T Clamp(T value, T min, T max);
        protected abstract T ClampMin(T value, T min);
        protected abstract T Round(T value);
        protected abstract T ClampMax(T value, T max);
        protected abstract T Increment(T value, T increment, bool negative);
        protected abstract bool NumbersAreEqual(T? value1, T? value2);
        
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
        public bool Nullable
        {
            get => _nullable;
            set
            {
                _nullable = value;
                UpdateTextWithValue();
            }
        }
        public T DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            //Submit text input
            GetValueFromText();
            base.OnLostFocus(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!Focused)
                return;
            GetValueFromText();
            if (_currentValue != null)
            {
                _currentValue = Clamp(Increment(_currentValue.Value,_smallIncrement, e.Delta < 0), _minValue, _maxValue);
                UpdateTextWithValue();
            }
            base.OnMouseWheel(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
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
                case Keys.Back:
                    break;
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Down:
                case Keys.Up:
                    {
                        GetValueFromText();
                        if (_currentValue == null)
                            return;
                        T increment = e.KeyCode == Keys.Down || e.KeyCode == Keys.Up ? (e.Shift ? _smallerIncrement : _smallIncrement) : (e.Shift ? _largerIncrement : _largeIncrement);
                        _currentValue = Clamp(Increment(_currentValue.Value, increment, e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Down), _minValue, _maxValue);
                        UpdateTextWithValue();
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
            => Text = _currentValue == null ? "" : _currentValue.Value.ToString();

        protected abstract bool TryParse(string text, out T value);

        protected void GetValueFromText()
        {
            //No change?
            if (_currentValue != null && _currentValue.Value.ToString() == Text)
                return;

            T? newValue = !string.IsNullOrWhiteSpace(Text) && TryParse(Text, out T newValue2) ? (T?)Round(Clamp(newValue2, _minValue, _maxValue)) : (Nullable ? null : (T?)DefaultValue);
            
            if (!NumbersAreEqual(_currentValue, newValue))
            {
                _previousValue = _currentValue;
                _currentValue = newValue;
                ValueChanged?.Invoke(_previousValue, _currentValue);
            }

            UpdateTextWithValue();
        }
    }
}
