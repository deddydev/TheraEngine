using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Enum))]
    public partial class PropGridEnum : PropGridItem
    {
        public PropGridEnum()
        {
            InitializeComponent();
            cboEnumNames.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            cboEnumNames.GotFocus += comboBox1_GotFocus;
            cboEnumNames.LostFocus += comboBox1_LostFocus;
            _containsBit = new Dictionary<TypeCode, DelContainsBit>()
            {
                { TypeCode.Byte,    UInt8ContainsBit  },
                { TypeCode.SByte,   Int8ContainsBit   },
                { TypeCode.Int16,   Int16ContainsBit  },
                { TypeCode.UInt16,  UInt16ContainsBit },
                { TypeCode.Int32,   Int32ContainsBit  },
                { TypeCode.UInt32,  UInt32ContainsBit },
                { TypeCode.Int64,   Int64ContainsBit  },
                { TypeCode.UInt64,  UInt64ContainsBit },
            };
            _convertBit = new Dictionary<TypeCode, DelConvertBit>()
            {
                { TypeCode.Byte,    UInt8ConvertBit  },
                { TypeCode.SByte,   Int8ConvertBit   },
                { TypeCode.Int16,   Int16ConvertBit  },
                { TypeCode.UInt16,  UInt16ConvertBit },
                { TypeCode.Int32,   Int32ConvertBit  },
                { TypeCode.UInt32,  UInt32ConvertBit },
                { TypeCode.Int64,   Int64ConvertBit  },
                { TypeCode.UInt64,  UInt64ConvertBit },
            };
        }

        private void comboBox1_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void comboBox1_GotFocus(object sender, EventArgs e) => IsEditing = true;

        private string _value = string.Empty;
        private FieldInfo[] _fields;

        protected override bool UpdateDisplayInternal(object value)
        {
            bool editable = IsEditable();
            switch (value)
            {
                case Enum e:
                {
                    _fields = DataType.GetFields(BindingFlags.Static | BindingFlags.Public);
                
                    string temp = value.ToString();
                    //if (string.Equals(_value, temp, StringComparison.InvariantCulture))
                    //    return;
                    _value = temp;
                
                    bool flags = DataType.GetCustomAttributes(false).FirstOrDefault(x => x is FlagsAttribute) != null;
                    tblEnumFlags.Visible = flags;
                    cboEnumNames.Visible = !flags;
                    if (flags)
                    {
                        //Type valueType = Enum.GetUnderlyingType(DataType);
                        TypeCode t = e.GetTypeCode();
                        DelContainsBit contains = _containsBit[t];
                        object totalValue = Convert.ChangeType(e, t);
                        object constValue, number;
                        for (int i = 0; i < _fields.Length; ++i)
                        {
                            constValue = _fields[i].GetRawConstantValue();
                            number = Convert.ChangeType(constValue, t);
                            if (tblEnumFlags.GetControlFromPosition(0, i) is CheckBox box)
                                box.Checked = contains(totalValue, number);
                        }
                        //panel1.Height = _fields.Length * 21;
                        tblEnumFlags.Enabled = editable;
                    }
                    else
                    {
                        int selectedIndex = -1;
                        string name;
                        for (int i = 0; i < _fields.Length; ++i)
                        {
                            name = _fields[i].Name;
                            if (string.Equals(name, _value, StringComparison.InvariantCulture))
                                selectedIndex = i;
                        }
                        if (cboEnumNames.Items.Count != _fields.Length)
                        {
                            cboEnumNames.Items.Clear();
                            bool splitCamelCase = Editor.GetSettings().PropertyGridRef.File.SplitCamelCase;
                            for (int i = 0; i < _fields.Length; ++i)
                                cboEnumNames.Items.Add(GetFieldName(_fields[i], splitCamelCase));
                        }
                        cboEnumNames.SelectedIndex = selectedIndex;
                        cboEnumNames.Enabled = editable;
                    }

                    break;
                }
                case Exception _:
                    cboEnumNames.Visible = true;
                    tblEnumFlags.Visible = false;
                    cboEnumNames.Text = value.ToString();
                    break;
                default:
                    cboEnumNames.Visible = false;
                    tblEnumFlags.Visible = false;
                    break;
            }
            return false;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboEnumNames.SelectedIndex < 0)
                return;
            object value = _fields[cboEnumNames.SelectedIndex].GetRawConstantValue();
            UpdateValue(value, true);
        }
        
        private bool UInt8ContainsBit(object total, object bits) 
            => ((byte)total & (byte)bits) == (byte)bits;
        private bool Int8ContainsBit(object total, object bits)
            => ((sbyte)total & (sbyte)bits) == (sbyte)bits;
        private bool UInt16ContainsBit(object total, object bits)
            => ((ushort)total & (ushort)bits) == (ushort)bits;
        private bool Int16ContainsBit(object total, object bits)
            => ((short)total & (short)bits) == (short)bits;
        private bool UInt32ContainsBit(object total, object bits)
            => ((uint)total & (uint)bits) == (uint)bits;
        private bool Int32ContainsBit(object total, object bits)
            => ((int)total & (int)bits) == (int)bits;
        private bool UInt64ContainsBit(object total, object bits)
            => ((ulong)total & (ulong)bits) == (ulong)bits;
        private bool Int64ContainsBit(object total, object bits)
            => ((long)total & (long)bits) == (long)bits;

        private object UInt8ConvertBit(object total, object bits, bool add)
            => add ? ((byte)total | (byte)bits) : ((byte)total & ~(byte)bits);
        private object Int8ConvertBit(object total, object bits, bool add)
            => add ? ((sbyte)total | (sbyte)bits) : ((sbyte)total & ~(sbyte)bits);
        private object UInt16ConvertBit(object total, object bits, bool add)
            => add ? ((ushort)total | (ushort)bits) : ((ushort)total & ~(ushort)bits);
        private object Int16ConvertBit(object total, object bits, bool add)
            => add ? ((short)total | (short)bits) : ((short)total & ~(short)bits);
        private object UInt32ConvertBit(object total, object bits, bool add)
            => add ? ((uint)total | (uint)bits) : ((uint)total & ~(uint)bits);
        private object Int32ConvertBit(object total, object bits, bool add)
            => add ? ((int)total | (int)bits) : ((int)total & ~(int)bits);
        private object UInt64ConvertBit(object total, object bits, bool add)
            => add ? ((ulong)total | (ulong)bits) : ((ulong)total & ~(ulong)bits);
        private object Int64ConvertBit(object total, object bits, bool add)
            => add ? ((long)total | (long)bits) : ((long)total & ~(long)bits);

        private delegate bool DelContainsBit(object total, object bits);
        private delegate object DelConvertBit(object total, object bits, bool add);

        private Dictionary<TypeCode, DelContainsBit> _containsBit;
        private Dictionary<TypeCode, DelConvertBit> _convertBit;

        private void BitSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            TypeCode t = Type.GetTypeCode(DataType);
            object totalValue = Convert.ChangeType(GetValue(), t);
            object newTotal = _convertBit[t](totalValue, box.Tag, box.Checked);
            UpdateValue(newTotal, true);
        }

        protected internal override void SetReferenceHolder(PropGridMemberInfo parentInfo)
        {
            UpdateControls(parentInfo?.DataType);
            base.SetReferenceHolder(parentInfo);
        }

        private void UpdateControls(Type enumType)
        {
            if (enumType == null)
                return;
            
            bool splitCamelCase = Editor.GetSettings().PropertyGridRef.File.SplitCamelCase;
            FieldInfo field;
            FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            bool flags = enumType.GetCustomAttributes(false).FirstOrDefault(x => x is FlagsAttribute) != null;
            if (flags)
            {
                panel1.Visible = true;
                cboEnumNames.Visible = false;
                tblEnumFlags.RowStyles.Clear();
                tblEnumFlags.RowCount = 0;
                object value;

                int maxDec = 0;
                int maxHex = 0;
                int maxBin = 0;
                (object number, string name, string dec, string hex, string bin)[] values = new (object number, string name, string dec, string hex, string bin)[fields.Length];
                for (int i = 0; i < fields.Length; ++i)
                {
                    field = fields[i];
                    string name = GetFieldName(field, splitCamelCase);

                    value = field.GetRawConstantValue();
                    TypeCode tc = Type.GetTypeCode(enumType);
                    object number = Convert.ChangeType(value, tc);

                    string decStr = number.ToString();
                    string hexStr = ((IFormattable)number).ToString("X", CultureInfo.InvariantCulture);
                    string binStr = string.Format(new BinaryFormatter(), "{0:B}", number);

                    maxDec = Math.Max(decStr.Length, maxDec);
                    maxHex = Math.Max(hexStr.Length, maxHex);
                    maxBin = Math.Max(binStr.Length, maxBin);

                    values[i] = (number, name, decStr, hexStr, binStr);
                }

                for (int i = 0; i < values.Length; ++i)
                {
                    (object number, string name, string dec, string hex, string bin) = values[i];

                    tblEnumFlags.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    tblEnumFlags.RowCount++;
                    
                    CheckBox bitSet = new CheckBox()
                    {
                        AutoSize = true,
                        Checked = false,
                        Tag = number,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Dock = DockStyle.Left,
                    };
                    bitSet.CheckedChanged += BitSet_CheckedChanged;
                    Label bitName = new Label()
                    {
                        AutoSize = true,
                        Text = name,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Dock = DockStyle.Fill,
                    };
                    Label bitValueDec = new Label()
                    {
                        AutoSize = true,
                        Text = dec.PadLeft(maxDec, '0'),
                        Tag = number,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Dock = DockStyle.Left,
                    };
                    Label bitValueHex = new Label()
                    {
                        AutoSize = true,
                        Text = "0x" + hex.PadLeft(maxHex, '0'),
                        Tag = number,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Dock = DockStyle.Left,
                    };
                    Label bitValueBin = new Label()
                    {
                        AutoSize = true,
                        Text = "0b" + bin.PadLeft(maxBin, '0'),
                        Tag = number,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Dock = DockStyle.Left,
                    };

                    int row = tblEnumFlags.RowCount - 1;
                    tblEnumFlags.Controls.Add(bitSet, 0, row);
                    tblEnumFlags.Controls.Add(bitName, 1, row);
                    tblEnumFlags.Controls.Add(bitValueDec, 2, row);
                    tblEnumFlags.Controls.Add(bitValueHex, 3, row);
                    tblEnumFlags.Controls.Add(bitValueBin, 4, row);
                }
            }
            else
            {
                panel1.Visible = false;
                cboEnumNames.Visible = true;
                cboEnumNames.Items.Clear();
                for (int i = 0; i < fields.Length; ++i)
                    cboEnumNames.Items.Add(GetFieldName(fields[i], splitCamelCase));
            }
        }
        private string GetFieldName(FieldInfo field, bool splitCamelCase)
        {
            DisplayNameAttribute attrib = field.GetCustomAttribute<DisplayNameAttribute>();
            string name = attrib?.DisplayName ?? field.Name;
            if (splitCamelCase)
                name = name.SplitCamelCase();
            return name;
        }
    }

    public class BinaryFormatter : IFormatProvider, ICustomFormatter
    {
        // IFormatProvider.GetFormat implementation.
        public object GetFormat(Type formatType)
        {
            // Determine whether custom formatting object is requested.
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        // Format number in binary (B), octal (O), or hexadecimal (H).
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Handle format string.
            int baseNumber;
            // Handle null or empty format string, string with precision specifier.
            string thisFmt = String.Empty;
            // Extract first character of format string (precision specifiers
            // are not supported).
            if (!String.IsNullOrEmpty(format))
                thisFmt = format.Length > 1 ? format.Substring(0, 1) : format;


            // Get a byte array representing the numeric value.
            byte[] bytes;
            if (arg is sbyte)
            {
                string byteString = ((sbyte)arg).ToString("X2");
                bytes = new byte[1] { Byte.Parse(byteString, System.Globalization.NumberStyles.HexNumber) };
            }
            else if (arg is byte)
            {
                bytes = new byte[1] { (byte)arg };
            }
            else if (arg is short)
            {
                bytes = BitConverter.GetBytes((short)arg);
            }
            else if (arg is int)
            {
                bytes = BitConverter.GetBytes((int)arg);
            }
            else if (arg is long)
            {
                bytes = BitConverter.GetBytes((long)arg);
            }
            else if (arg is ushort)
            {
                bytes = BitConverter.GetBytes((ushort)arg);
            }
            else if (arg is uint)
            {
                bytes = BitConverter.GetBytes((uint)arg);
            }
            else if (arg is ulong)
            {
                bytes = BitConverter.GetBytes((ulong)arg);
            }
            else
            {
                try
                {
                    return HandleOtherFormats(format, arg);
                }
                catch (FormatException e)
                {
                    throw new FormatException(String.Format("The format of '{0}' is invalid.", format), e);
                }
            }

            switch (thisFmt.ToUpper())
            {
                // Binary formatting.
                case "B":
                    baseNumber = 2;
                    break;
                case "O":
                    baseNumber = 8;
                    break;
                case "H":
                    baseNumber = 16;
                    break;
                // Handle unsupported format strings.
                default:
                    try
                    {
                        return HandleOtherFormats(format, arg);
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException(String.Format("The format of '{0}' is invalid.", format), e);
                    }
            }

            // Return a formatted string.
            string numericString = String.Empty;
            int upper = bytes.GetUpperBound(0);
            int lower = bytes.GetLowerBound(0);
            for (int ctr = upper; ctr >= lower; --ctr)
            {
                string byteString = Convert.ToString(bytes[ctr], baseNumber);
                if (baseNumber == 2)
                    byteString = new String('0', 8 - byteString.Length) + byteString;
                else if (baseNumber == 8)
                    byteString = new String('0', 4 - byteString.Length) + byteString;
                // Base is 16.
                else
                    byteString = new String('0', 2 - byteString.Length) + byteString;

                numericString += byteString;
            }
            int trimIndex = 0;
            for (; trimIndex < numericString.Length - 1; ++trimIndex)
                if (numericString[trimIndex] != '0')
                    break;
            return numericString.Substring(trimIndex).Trim();
        }

        private string HandleOtherFormats(string format, object arg)
        {
            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
            else if (arg != null)
                return arg.ToString();
            else
                return String.Empty;
        }
    }
}
