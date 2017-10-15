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
    public class NumericInputBox : TextBox
    {
        public event EventHandler ValueChanged;

        public NumericInputBox()
        {
            UpdateText();
        }
        
        public decimal _previousValue = 0.0m;
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal Value
        {
            get
            {
                if (_currentValue == null)
                    ValidateTextInput();
                return _currentValue.Value;
            }
            set
            {
                _currentValue = value.Clamp(_minValue, _maxValue);
                UpdateText();
            }
        }
        public decimal MinimumValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                UpdateText();
            }
        }
        public decimal MaximumValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                UpdateText();
            }
        }
        public bool Integral
        {
            get => _integral;
            set
            {
                _integral = value;
                UpdateText();
            }
        }
        public bool Signed
        {
            get => _signed;
            set
            {
                _signed = value;
                UpdateText();
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            ValidateTextInput();
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
                            ValidateTextInput();
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        if (e.Shift)
                        {
                            Text = (val - _largerIncrement).Clamp(_minValue, _maxValue).ToString();
                            ValidateTextInput();
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
                            ValidateTextInput();
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        if (e.Shift)
                        {
                            Text = (val + _largerIncrement).Clamp(_minValue, _maxValue).ToString();
                            ValidateTextInput();
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
                        ValidateTextInput();
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Down:
                    if (decimal.TryParse(Text, out val))
                    {
                        if (e.Shift || _integral)
                            Text = (val - _fineIncrement).Clamp(_minValue, _maxValue).ToString();
                        else
                            Text = (val - _finerIncrement).Clamp(_minValue, _maxValue).ToString();
                        ValidateTextInput();
                    }
                    e.Handled = true;
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
                    UpdateText();
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Enter:
                    ValidateTextInput();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.X:
                    if (e.Control)
                    {
                        if (decimal.TryParse(Text, out val))
                        {
                            val = float.NaN;
                            Text = val.ToString();
                            ValidateTextInput();
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

        private void UpdateText()
        {
            if (_currentValue == float.NaN)
                Text = "";
            else
                Text = _currentValue.ToString();
        }

        private void ValidateTextInput()
        {
            decimal val = _currentValue ?? 0.0m;

            if (_currentValue != null && val.ToString() == Text)
                return;

            if (Text == "")
                val = decimal.NaN;
            else if (!_integral)
            {
                float.TryParse(Text, out val);
                val = val.Clamp(_minValue, _maxValue);
            }
            else
            {
                int.TryParse(Text, out val2);
                //val2 = val2.Clamp(Convert.ToInt32(_minValue.Clamp((float)int.MinValue, (float)int.MaxValue)), Convert.ToInt32(_maxValue.Clamp((float)int.MinValue, (float)int.MaxValue)));
            }

            if (!_integral)
            {
                if (_currentValue != val)
                {
                    _previousValue = _currentValue ?? 0.0f;
                    _currentValue = val;
                    ValueChanged?.Invoke(this, null);
                }
            }
            else
            {
                if (_currentValue != val2)
                {
                    _previousValue = _currentValue ?? 0.0f;
                    _currentValue = val2;
                    ValueChanged?.Invoke(this, null);
                }
            }

            UpdateText();
        }
    }
}
