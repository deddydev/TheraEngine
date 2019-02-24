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
                for (int i = 0; i < fields.Length; ++i)
                {
                    field = fields[i];
                    string name = GetFieldName(field, splitCamelCase);

                    tblEnumFlags.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    tblEnumFlags.RowCount++;

                    value = field.GetRawConstantValue();
                    TypeCode tc = Type.GetTypeCode(enumType);
                    object number = Convert.ChangeType(value, tc);

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
                        Text = number.ToString(),
                        Tag = number,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Dock = DockStyle.Left,
                    };
                    //Label bitValueHex = new Label()
                    //{
                    //    AutoSize = true,
                    //    Text = ((IFormattable)number).ToString("X", CultureInfo.InvariantCulture),
                    //    Tag = number,
                    //    TextAlign = ContentAlignment.MiddleLeft,
                    //    Margin = new Padding(0),
                    //    Padding = new Padding(0),
                    //    Dock = DockStyle.Left,
                    //};
                    //Label bitValueBin = new Label()
                    //{
                    //    AutoSize = true,
                    //    Text = ((IFormattable)number).ToString("G", CultureInfo.InvariantCulture),
                    //    Tag = number,
                    //    TextAlign = ContentAlignment.MiddleLeft,
                    //    Margin = new Padding(0),
                    //    Padding = new Padding(0),
                    //    Dock = DockStyle.Left,
                    //};

                    int r = tblEnumFlags.RowCount - 1;
                    tblEnumFlags.Controls.Add(bitSet, 0, r);
                    tblEnumFlags.Controls.Add(bitName, 1, r);
                    tblEnumFlags.Controls.Add(bitValueDec, 2, r);
                    //tblEnumFlags.Controls.Add(bitValueHex, 3, r);
                    //tblEnumFlags.Controls.Add(bitValueBin, 4, r);
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
}
