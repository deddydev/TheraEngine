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
            
            UpdateTextWithValue();
        }
        
        public decimal? _previousValue = null;
        public decimal? _currentValue = null;
        public decimal
            _minValue = decimal.MinValue,
            _maxValue = decimal.MaxValue,
            _largeIncrement = 90m,
            _smallIncrement = 1m,
            _largerIncrement = 180m,
            _smallerIncrement = 0.1m;
        private bool _integral = false, _signed = true, _nullable = false;
        private int _enforcedDecimals = -1;
        private MidpointRounding _midPointRounding = MidpointRounding.AwayFromZero;
        private decimal _defaultValue = 0m;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal? Value
        {
            get => _currentValue;
            set
            {
                _previousValue = _currentValue;
                if (value == null) 
                    _currentValue = Nullable ? null : (decimal?)_defaultValue;
                else
                    _currentValue = value.Value.Clamp(_minValue, _maxValue);
                
                UpdateTextWithValue();
            }
        }
        public decimal MinimumValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                UpdateTextWithValue();
            }
        }
        public decimal MaximumValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                UpdateTextWithValue();
            }
        }
        public bool Integral
        {
            get => _integral;
            set
            {
                _integral = value;
                UpdateTextWithValue();
            }
        }
        public bool Signed
        {
            get => _signed;
            set
            {
                _signed = value;
                UpdateTextWithValue();
            }
        }
        public int AllowedDecimalPlaces
        {
            get => _enforcedDecimals;
            set
            {
                _enforcedDecimals = value.ClampMin(-1);
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
        public bool Nullable
        {
            get => _nullable;
            set
            {
                _nullable = value;
                UpdateTextWithValue();
            }
        }
        public decimal DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                UpdateTextWithValue();
            }
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
                _currentValue = (_currentValue.Value + (e.Delta < 0 ? -_smallIncrement : _smallIncrement)).Clamp(_minValue, _maxValue);
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

                case Keys.Left:
                    {
                        if (e.Control || e.Shift)
                        {
                            decimal increment = e.Control ? _largeIncrement : _largerIncrement;
                            GetValueFromText();
                            if (_currentValue == null)
                                return;
                            _currentValue = (_currentValue.Value - _largeIncrement).Clamp(_minValue, _maxValue);
                            UpdateTextWithValue();
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        break;
                    }
                case Keys.Right:
                    {
                        if (e.Control || e.Shift)
                        {
                            decimal increment = e.Control ? _largeIncrement : _largerIncrement;
                            GetValueFromText();
                            if (_currentValue == null)
                                return;
                            _currentValue = (_currentValue.Value + _largeIncrement).Clamp(_minValue, _maxValue);
                            UpdateTextWithValue();
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        break;
                    }
                case Keys.Up:
                    {
                        GetValueFromText();
                        if (_currentValue == null)
                            return;
                        decimal increment = e.Shift || _integral ? _smallIncrement : _smallerIncrement;
                        _currentValue = (_currentValue.Value + increment).Clamp(_minValue, _maxValue);
                        UpdateTextWithValue();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    }
                case Keys.Down:
                    {
                        GetValueFromText();
                        if (_currentValue == null)
                            return;
                        decimal increment = e.Shift || _integral ? _smallIncrement : _smallerIncrement;
                        _currentValue = (_currentValue.Value - increment).Clamp(_minValue, _maxValue);
                        UpdateTextWithValue();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    }
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
                    //if (e.Control)
                    //{
                    //    if (decimal.TryParse(Text, out decimal val))
                    //    {
                    //        Text = "";
                    //        GetValueFromText();
                    //    }
                    //}
                    //break;
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

        private void UpdateTextWithValue()
            => Text = _currentValue == null ? "" : _currentValue.ToString();
        
        private void GetValueFromText()
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
                    if (_integral || _enforcedDecimals == 0)
                        newValue2 = Math.Round(newValue2);
                    else if (_enforcedDecimals > 0)
                        newValue2 = Math.Round(newValue2, _enforcedDecimals, _midPointRounding);
                    newValue = newValue2;
                }
                else
                    newValue = null;
            }

            if (_currentValue != newValue)
            {
                _previousValue = _currentValue;
                _currentValue = newValue;
                ValueChanged?.Invoke(_previousValue, _currentValue);
            }

            UpdateTextWithValue();
        }

        public void SetValue(sbyte value)
        {
            MinimumValue = sbyte.MinValue;
            MaximumValue = sbyte.MaxValue;
            Integral = true;
            Signed = true;
            Value = value;
        }
        public void SetValue(byte value)
        {
            MinimumValue = byte.MinValue;
            MaximumValue = byte.MaxValue;
            Integral = true;
            Signed = false;
            Value = value;
        }
        public void SetValue(short value)
        {
            MinimumValue = short.MinValue;
            MaximumValue = short.MaxValue;
            Integral = true;
            Signed = true;
            Value = value;
        }
        public void SetValue(ushort value)
        {
            MinimumValue = ushort.MinValue;
            MaximumValue = ushort.MaxValue;
            Integral = true;
            Signed = false;
            Value = value;
        }
        public void SetValue(int value)
        {
            MinimumValue = int.MinValue;
            MaximumValue = int.MaxValue;
            Integral = true;
            Signed = true;
            Value = value;
        }
        public void SetValue(uint value)
        {
            MinimumValue = uint.MinValue;
            MaximumValue = uint.MaxValue;
            Integral = true;
            Signed = false;
            Value = value;
        }
        public void SetValue(long value)
        {
            MinimumValue = long.MinValue;
            MaximumValue = long.MaxValue;
            Integral = true;
            Signed = true;
            Value = value;
        }
        public void SetValue(ulong value)
        {
            MinimumValue = ulong.MinValue;
            MaximumValue = ulong.MaxValue;
            Integral = true;
            Signed = false;
            Value = value;
        }
        public void SetValue(float value)
        {
            MinimumValue = decimal.MinValue;//Convert.ToDecimal(float.MinValue);
            MaximumValue = decimal.MaxValue;//Convert.ToDecimal(float.MaxValue);
            Integral = false;
            Signed = true;
            Value = Convert.ToDecimal(value);
        }
        public void SetValue(double value)
        {
            MinimumValue = decimal.MinValue; //Convert.ToDecimal(double.MinValue);
            MaximumValue = decimal.MaxValue; //Convert.ToDecimal(double.MaxValue);
            Integral = false;
            Signed = true;
            Value = Convert.ToDecimal(value);
        }
        public void SetValue(decimal value)
        {
            MinimumValue = decimal.MinValue;
            MaximumValue = decimal.MaxValue;
            Integral = false;
            Signed = true;
            Value = value;
        }
        public sbyte GetSByte()
        {
            if (Value == null)
                return 0;
            return Convert.ToSByte(Value.Value);
        }
        public byte GetByte()
        {
            if (Value == null)
                return 0;
            return Convert.ToByte(Value.Value);
        }
        public short GetShort()
        {
            if (Value == null)
                return 0;
            return Convert.ToInt16(Value.Value);
        }
        public ushort GetUShort()
        {
            if (Value == null)
                return 0;
            return Convert.ToUInt16(Value.Value);
        }
        public int GetInt()
        {
            if (Value == null)
                return 0;
            return Convert.ToInt32(Value.Value);
        }
        public uint GetUInt()
        {
            if (Value == null)
                return 0U;
            return Convert.ToUInt32(Value.Value);
        }
        public long GetLong()
        {
            if (Value == null)
                return 0L;
            return Convert.ToInt64(Value.Value);
        }
        public ulong GetULong()
        {
            if (Value == null)
                return 0UL;
            return Convert.ToUInt64(Value.Value);
        }
        public float GetFloat()
        {
            if (Value == null)
                return 0.0F;
            return Convert.ToSingle(Value.Value);
        }
        public double GetDouble()
        {
            if (Value == null)
                return 0.0;
            return Convert.ToDouble(Value.Value);
        }
    }
}
