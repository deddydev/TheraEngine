using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public partial class ObjectCreator : TheraForm
    {
        public static ObjectCreator Current = null;
        protected override void OnActivated(EventArgs e)
        {
            Current = this;
            base.OnActivated(e);
        }
        protected override void OnDeactivate(EventArgs e)
        {
            Current = null;
            base.OnDeactivate(e);
        }
        /// <summary>
        /// Returns true if the dialog needs to be shown.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allowDerivedTypes"></param>
        /// <returns></returns>
        public bool Initialize(Type type, bool allowDerivedTypes)
        {
            ClassType = type;

            if (allowDerivedTypes)
            {
                Program.PopulateMenuDropDown(toolStripDropDownButton1, OnTypeSelected, x => !x.IsAbstract && type.IsAssignableFrom(x));
                //Type[] allowed = Program.FindPublicTypes();
                //if (allowed.Length == 0)
                //    return false;

                //cboTypes.Items.AddRange(allowed);
            }
            else if (type.IsAbstract)
                return false;
            else
            {
                string typeName = type.GetFriendlyName();
                ToolStripDropDownButton btn = new ToolStripDropDownButton(typeName)
                {
                    AutoSize = false,
                    ShowDropDownArrow = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Tag = type,
                };
                Size s = TextRenderer.MeasureText(typeName, btn.Font);
                btn.Width = s.Width;
                btn.Height = s.Height + 10;
                btn.Click += OnTypeSelected;
                toolStripDropDownButton1.DropDownItems.Add(btn);
            }
            
            return true;
        }

        private void OnTypeSelected(object sender, EventArgs e)
        {
            ToolStripDropDownItem item = sender as ToolStripDropDownItem;
            SetTargetType(item?.Tag as Type);
        }

        public ObjectCreator() : base()
        {
            InitializeComponent();
        }

        private bool _updating = false;
        private void ConstructorSelector_CheckedChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            CheckBox c = (CheckBox)sender;
            ConstructorIndex = (int)c.Tag;
            _updating = true;
            if (c.Checked)
            {
                for (int i = 0; i < FinalArguments.Length; ++i)
                {
                    if (i == ConstructorIndex)
                        continue;
                    int rowIndex = i << 1;
                    CheckBox box = (CheckBox)tblConstructors.GetControlFromPosition(0, rowIndex);
                    box.Checked = false;
                }
            }
            else
                c.Checked = true;
            _updating = false;
        }

        private class ArgumentInfo
        {
            public Type Type { get; set; }
            public int Index { get; set; }
            public int RowIndex { get; set; }
            public object Value { get; set; }
        }

        public int ConstructorIndex { get; private set; }
        public Type ClassType { get; private set; }
        public ConstructorInfo[] PublicInstanceConstructors { get; private set; }
        public object[][] FinalArguments;
        public object ConstructedObject { get; private set; } = null;
        
        private void btnOkay_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            ConstructedObject = FinalArguments[ConstructorIndex].Length == 0 ? Activator.CreateInstance(ClassType) : Activator.CreateInstance(ClassType, FinalArguments[ConstructorIndex]);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void SetTargetType(Type type)
        {
            tblConstructors.RowStyles.Clear();
            tblConstructors.RowCount = 0;
            tblConstructors.ColumnStyles.Clear();
            tblConstructors.ColumnCount = 0;
            tblConstructors.Controls.Clear();
            ClassType = type;

            toolStripDropDownButton1.Text = type.GetFriendlyName();

            PublicInstanceConstructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            //if (PublicInstanceConstructors.Length == 1)
            //{
            //    var parameters = PublicInstanceConstructors[0].GetParameters();
            //    if (parameters.Length == 0)
            //    {
            //        FinalArguments = new object[0][];
            //        return;
            //    }
            //}

            FinalArguments = new object[PublicInstanceConstructors.Length][];

            for (int constructorIndex = 0; constructorIndex < PublicInstanceConstructors.Length; ++constructorIndex)
            {
                ConstructorInfo c = PublicInstanceConstructors[constructorIndex];

                //First row: text representation
                tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tblConstructors.RowCount = tblConstructors.RowStyles.Count;
                //Second row: input object editors
                tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tblConstructors.RowCount = tblConstructors.RowStyles.Count;

                ParameterInfo[] parameters = c.GetParameters();
                FinalArguments[constructorIndex] = new object[parameters.Length];

                //Update column count if the number of parameters exceeds current count
                int columns = Math.Max(parameters.Length + 1, tblConstructors.ColumnCount);
                if (columns > tblConstructors.ColumnCount)
                {
                    int total = columns - tblConstructors.ColumnCount;
                    for (int i = 0; i < total; ++i)
                    {
                        tblConstructors.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                        tblConstructors.ColumnCount = tblConstructors.ColumnStyles.Count;
                    }
                }

                CheckBox constructorSelector = new CheckBox()
                {
                    Text = ClassType.Name.Split('`')[0],
                    Dock = DockStyle.Left,
                    AutoSize = true,
                    Tag = constructorIndex,
                    Checked = constructorIndex == 0,
                };

                ConstructorIndex = 0;
                
                constructorSelector.CheckedChanged += ConstructorSelector_CheckedChanged;
                tblConstructors.Controls.Add(constructorSelector, 0, tblConstructors.RowCount - 2);

                //Label constructorLabel = new Label()
                //{
                //    Text = ClassType.Name.Split('`')[0],
                //    Dock = DockStyle.Left,
                //    AutoSize = true,
                //    BackColor = Color.FromArgb(50, 55, 70),
                //    ForeColor = Color.FromArgb(200, 200, 220),
                //    Padding = new Padding(0),
                //    Margin = new Padding(0),
                //};
                //tblConstructors.Controls.Add(constructorLabel, 1, tblConstructors.RowCount - 2);

                for (int paramIndex = 0; paramIndex < parameters.Length; ++paramIndex)
                {
                    ParameterInfo p = parameters[paramIndex];
                    Type t = p.ParameterType;

                    FinalArguments[constructorIndex][paramIndex] = t.GetDefaultValue();

                    string typeName = t.GetFriendlyName();
                    string varName = p.Name;
                    string finalText = typeName + " " + varName;
                    if (paramIndex != parameters.Length - 1)
                        finalText += ", ";

                    Label paramLabel = new Label()
                    {
                        Text = finalText,
                        Dock = DockStyle.Left,
                        AutoSize = true,
                        Padding = new Padding(0),
                        Margin = new Padding(0),
                        BackColor = Color.FromArgb(50, 55, 70),
                        ForeColor = Color.FromArgb(200, 200, 220),
                    };
                    tblConstructors.Controls.Add(paramLabel, paramIndex + 1, tblConstructors.RowCount - 2);

                    Control paramTool = CreateControl(t, p, constructorIndex, FinalArguments);
                    if (paramTool != null)
                        tblConstructors.Controls.Add(paramTool, paramIndex + 1, tblConstructors.RowCount - 1);
                }
            }
        }
        public static Control CreateControl(Type t, ParameterInfo p, int rowIndex, object[][] finalArguments)
        {
            Control paramTool = null;
            ArgumentInfo arg = new ArgumentInfo()
            {
                Type = t,
                Index = p.Position,
                RowIndex = rowIndex,
                Value = t.GetDefaultValue(),
            };
            Type temp = Nullable.GetUnderlyingType(t);
            if (temp != null)
                t = temp;
            switch (t.Name)
            {
                case "Boolean":
                    {
                        CheckBox box = new CheckBox()
                        {
                            Tag = arg,
                        };
                        box.CheckedChanged += (sender, e) =>
                        {
                            CheckBox checkBox = (CheckBox)sender;
                            ArgumentInfo argInfo = (ArgumentInfo)checkBox.Tag;
                            argInfo.Value = checkBox.Checked;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = checkBox.Checked;
                        };
                        paramTool = box;
                    }
                    break;
                case "SByte":
                    {
                        NumericInputBoxSByte box = new NumericInputBoxSByte()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Byte":
                    {
                        NumericInputBoxByte box = new NumericInputBoxByte()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Char":
                    paramTool = new TextBox()
                    {
                        Tag = arg
                    };
                    break;
                case "Int16":
                    {
                        NumericInputBoxInt16 box = new NumericInputBoxInt16()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "UInt16":
                    {
                        NumericInputBoxUInt16 box = new NumericInputBoxUInt16()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Int32":
                    {
                        NumericInputBoxInt32 box = new NumericInputBoxInt32()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "UInt32":
                    {
                        NumericInputBoxUInt32 box = new NumericInputBoxUInt32()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Int64":
                    {
                        NumericInputBoxInt64 box = new NumericInputBoxInt64()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "UInt64":
                    {
                        NumericInputBoxUInt64 box = new NumericInputBoxUInt64()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Single":
                    {
                        NumericInputBoxSingle box = new NumericInputBoxSingle()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Double":
                    {
                        NumericInputBoxDouble box = new NumericInputBoxDouble()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Decimal":
                    {
                        NumericInputBoxDecimal box = new NumericInputBoxDecimal()
                        {
                            Nullable = temp != null,
                            Tag = arg
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "String":
                    {
                        TextBox box = new TextBox()
                        {
                            Tag = arg,
                        };
                        box.TextChanged += (sender, e) =>
                        {
                            TextBox s = (TextBox)sender;
                            ArgumentInfo argInfo = (ArgumentInfo)s.Tag;
                            argInfo.Value = s.Text;
                            finalArguments[argInfo.RowIndex][argInfo.Index] = s.Text;
                        };
                        paramTool = box;
                    }
                    break;
                default:
                    {
                        Label defaultLabel = new Label()
                        {
                            Text = arg.Value == null ? "null" : arg.Value.ToString(),
                            Tag = arg
                        };
                        defaultLabel.MouseEnter += (sender, e) =>
                        {
                            Label s = (Label)sender;
                            s.BackColor = Color.FromArgb(50, 55, 70);
                        };
                        defaultLabel.MouseLeave += (sender, e) =>
                        {
                            Label s = (Label)sender;
                            s.BackColor = Color.FromArgb(70, 75, 90);
                        };
                        defaultLabel.MouseClick += (sender, e) =>
                        {
                            Label s = (Label)sender;
                            ArgumentInfo argInfo = (ArgumentInfo)s.Tag;

                            object o = Editor.UserCreateInstanceOf(argInfo.Type, true);

                            if (o == null && argInfo.Type.IsValueType)
                                o = argInfo.Type.GetDefaultValue();

                            argInfo.Value = o;
                            s.Text = o == null ? "null" : o.ToString();
                            finalArguments[argInfo.RowIndex][argInfo.Index] = o;
                        };
                        paramTool = defaultLabel;
                    }
                    break;
            }
            if (paramTool != null)
            {
                paramTool.Padding = new Padding(0);
                paramTool.Margin = new Padding(3);
                paramTool.Dock = DockStyle.Top;
                paramTool.AutoSize = true;
                paramTool.BackColor = Color.FromArgb(50, 55, 70);
                paramTool.ForeColor = Color.FromArgb(200, 200, 220);
            }
            return paramTool;
        }
    }
}
