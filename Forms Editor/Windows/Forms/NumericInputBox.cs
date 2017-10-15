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
    public delegate void BoxValueChanged(decimal? previous, decimal? current);
    public class NumericInputBox : TextBox
    {
        public event BoxValueChanged ValueChanged;

        public NumericInputBox()
        {
            
            CurrentValueChanged();
        }
        
        public decimal? _previousValue = null;
        public decimal? _currentValue = null;
        public decimal
            _minValue = decimal.MinValue,
            _maxValue = decimal.MaxValue,
            _largeIncrement = 90m,
            _fineIncrement = 1m,
            _largerIncrement = 180m,
            _finerIncrement = 0.1m;
        public bool _integral = false;
        public bool _signed = true;
        public int _enforcedDecimals = -1;
        public MidpointRounding _midPointRounding = MidpointRounding.AwayFromZero;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal Value
        {
            get
            {
                if (_currentValue == null)
                    ValidateText();
                return _currentValue.Value;
            }
            set
            {
                _currentValue = value.Clamp(_minValue, _maxValue);
                CurrentValueChanged();
            }
        }
        public decimal MinimumValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                CurrentValueChanged();
            }
        }
        public decimal MaximumValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                CurrentValueChanged();
            }
        }
        public bool Integral
        {
            get => _integral;
            set
            {
                _integral = value;
                CurrentValueChanged();
            }
        }
        public bool Signed
        {
            get => _signed;
            set
            {
                _signed = value;
                CurrentValueChanged();
            }
        }
        public int AllowedDecimalPlaces
        {
            get => _enforcedDecimals;
            set
            {
                _enforcedDecimals = value;
                CurrentValueChanged();
            }
        }
        public MidpointRounding MidpointRoundingMethod
        {
            get => _midPointRounding;
            set
            {
                _midPointRounding = value;
                CurrentValueChanged();
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            ValidateText();
            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            decimal val;
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

                case Keys.Left:
                    if (decimal.TryParse(Text, out val))
                    {
                        if (e.Control)
                        {
                            Text = (val - _largeIncrement).Clamp(_minValue, _maxValue).ToString();
                            ValidateText();
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        else if (e.Shift)
                        {
                            Text = (val - _largerIncrement).Clamp(_minValue, _maxValue).ToString();
                            ValidateText();
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                    }
                    break;

                case Keys.Right:
                    if (decimal.TryParse(Text, out val))
                    {
                        if (e.Control)
                        {
                            Text = (val + _largeIncrement).Clamp(_minValue, _maxValue).ToString();
                            ValidateText();
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        else if (e.Shift)
                        {
                            Text = (val + _largerIncrement).Clamp(_minValue, _maxValue).ToString();
                            ValidateText();
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                    }
                    break;

                case Keys.Up:
                    if (decimal.TryParse(Text, out val))
                    {
                        if (e.Shift || _integral)
                            Text = (val + _fineIncrement).Clamp(_minValue, _maxValue).ToString();
                        else
                            Text = (val + _finerIncrement).Clamp(_minValue, _maxValue).ToString();
                        ValidateText();
                        e.Handled = true;
                    }
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Down:
                    if (decimal.TryParse(Text, out val))
                    {
                        if (e.Shift || _integral)
                            Text = (val - _fineIncrement).Clamp(_minValue, _maxValue).ToString();
                        else
                            Text = (val - _finerIncrement).Clamp(_minValue, _maxValue).ToString();
                        ValidateText();
                        e.Handled = true;
                    }
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Subtract:
                case Keys.OemMinus:
                    if (!_signed || SelectionStart != 0 || Text.IndexOf('-') != -1)
                        e.SuppressKeyPress = true;
                    break;

                case Keys.Decimal:
                case Keys.OemPeriod:
                    if (Text.IndexOf('.') != -1)
                        e.SuppressKeyPress = true;
                    break;

                case Keys.Escape:
                    CurrentValueChanged();
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Enter:
                    ValidateText();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.X:
                    if (e.Control)
                    {
                        if (decimal.TryParse(Text, out val))
                        {
                            Text = "";
                            ValidateText();
                        }
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

        private void CurrentValueChanged()
            => Text = _currentValue == null ? "" : _currentValue.ToString();
        
        private void ValidateText()
        {
            //No change?
            if (_currentValue != null && _currentValue.Value.ToString() == Text)
                return;

            decimal? newValue;
            if (string.IsNullOrWhiteSpace(Text))
                newValue = null;
            else
            {
                if (decimal.TryParse(Text, out decimal newValue2))
                {
                    decimal min = _signed ? _minValue : _minValue.ClampMin(0m);
                    newValue2 = newValue2.Clamp(min, _maxValue);
                    if (_integral)
                        newValue2 = Math.Round(newValue2);
                    newValue = newValue2;
                }
                else
                    newValue = null;
            }

            if (_currentValue != newValue)
            {
                _previousValue = _currentValue;
                _currentValue = newValue;
                ValueChanged?.Invoke(this, null);
            }

            CurrentValueChanged();
        }
    }
}
