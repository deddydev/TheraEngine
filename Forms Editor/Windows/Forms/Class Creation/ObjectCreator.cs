using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheraEngine;

namespace TheraEditor.Windows.Forms
{
    public partial class ObjectCreator : TheraForm
    {
        internal Type[] _preSelectedGenericTypeArgs = null;
        
        /// <summary>
        /// Returns true if the dialog needs to be shown.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allowDerivedTypes"></param>
        /// <returns></returns>
        public bool Initialize(Type type, bool allowDerivedTypes)
        {
            if (type.IsGenericTypeDefinition)
                throw new InvalidOperationException("Cannot create an instance of a generic type definition. Pass in the type created with each generic parameter set.");

            if (type.IsGenericParameter)
                throw new InvalidOperationException("Cannot create an instance of a generic parameter. Pass in the type created from the parameter constraints.");

            ArrayMode = type.IsArray;

            if (ArrayMode)
                type = type.GetElementType();

            if (type.IsGenericType)
            {
                _preSelectedGenericTypeArgs = type.GenericTypeArguments;
                //type = type.GetGenericTypeDefinition();
            }
            
            if (allowDerivedTypes)
            {
                Type[] types = Program.PopulateMenuDropDown(toolStripDropDownButton1, OnTypeSelected, x => type.IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface);
                if (types.Length == 1)
                {
                    ConstructorInfo[] constructors = type.GetConstructors();
                    if (constructors.Length <= 1)
                    {
                        if (constructors.Length == 1)
                        {
                            if (constructors[0].GetParameters().Length == 0)
                            {
                                ConstructedObject = Activator.CreateInstance(type);
                                return false;
                            }
                        }
                        else
                        {
                            Engine.LogWarning($"Can't create type {type.GetFriendlyName()}; has no public constructors.");
                            return false;
                        }
                    }

                    SetTargetType(types[0]);
                    toolStripTypeSelection.Visible = false;
                }
                else
                    toolStripTypeSelection.Visible = true;
            }
            else if (type.IsAbstract || type.IsInterface)
                return false;
            else
            {
                ConstructorInfo[] constructors = type.GetConstructors();
                if (constructors.Length <= 1)
                {
                    if (constructors.Length == 1)
                    {
                        if (constructors[0].GetParameters().Length == 0)
                        {
                            ConstructedObject = Activator.CreateInstance(type);
                            return false;
                        }
                    }
                    else
                    {
                        Engine.LogWarning($"Can't create type {type.GetFriendlyName()}; has no public constructors.");
                        return false;
                    }
                }
                
                SetTargetType(type);
                toolStripTypeSelection.Visible = false;
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
            numericInputBoxSingle1.MinimumValue = 0;
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
            public int ColumnIndex { get; set; }
            public int RowIndex { get; set; }
            public object Value { get; set; }
        }
        
        public int ConstructorIndex { get; private set; }
        public Type ClassType { get; private set; }
        public ConstructorInfo[] PublicInstanceConstructors { get; private set; }
        public MethodInfo[] PublicStaticConstructors { get; private set; }
        public object[][] FinalArguments;
        public ParameterInfo[][] Parameters;
        public object ConstructedObject { get; private set; } = null;
        private bool _arrayMode = false;
        private bool ArrayMode
        {
            get => _arrayMode;
            set
            {
                _arrayMode = value;
                toolStripTypeSelection.Visible = !_arrayMode;
                pnlArrayLength.Visible = _arrayMode;
                toolStripDropDownButton1.Text = _arrayMode ? "Select an element type..." : "Select an object type...";

                tblConstructors.ColumnStyles.Clear();
                tblConstructors.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tblConstructors.ColumnCount = 1;

                tblConstructors.RowStyles.Clear();
                tblConstructors.RowCount = numericInputBoxSingle1.Value.Value;
                for (int i = 0; i < tblConstructors.RowCount; ++i)
                    tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            if (ClassType.ContainsGenericParameters)
            {
                if (_preSelectedGenericTypeArgs != null)
                    ClassType = ClassType.MakeGenericType(_preSelectedGenericTypeArgs);
                else
                {
                    using (GenericsSelector selector = new GenericsSelector(ClassType))
                    {
                        DialogResult result = selector.ShowDialog();
                        if (result == DialogResult.OK)
                            ClassType = selector.FinalClassType;
                        else
                            return;
                    }
                }
            }

            DialogResult = DialogResult.OK;

            if (ArrayMode)
            {
                ConstructedObject = Array.CreateInstance(ClassType, FinalArguments.GetLength(0));
                FinalArguments.Select(x => x[0]).ToArray().CopyTo((Array)ConstructedObject, 0);
            }
            else
            {
                object[] paramData = FinalArguments[ConstructorIndex];
                if (ConstructorIndex < PublicInstanceConstructors.Length)
                {
                    if (paramData.Length == 0)
                        ConstructedObject = Activator.CreateInstance(ClassType);
                    else
                    {
                        ConstructorInfo constructor = ClassType.GetConstructor(Parameters[ConstructorIndex].Select(x => x.ParameterType).ToArray());
                        ConstructedObject = constructor.Invoke(paramData);
                    }
                }
                else
                {
                    int index = ConstructorIndex - PublicInstanceConstructors.Length;
                    ConstructedObject = PublicStaticConstructors[index].Invoke(null, paramData);
                }
            }
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void SetTargetType(Type type)
        {
            tblConstructors.Controls.Clear();
            ClassType = type;

            toolStripDropDownButton1.Text = type.GetFriendlyName();

            if (ArrayMode)
            {
                toolStripTypeSelection.Visible = true;
                FinalArguments = new object[numericInputBoxSingle1.Value.Value][].FillWith(new object[1] { type.GetDefaultValue() });
                for (int i = 0; i < numericInputBoxSingle1.Value.Value; ++i)
                    tblConstructors.Controls.Add(CreateControl(type, 0, i, FinalArguments));
            }
            else
            {
                tblConstructors.RowStyles.Clear();
                tblConstructors.RowCount = 0;
                tblConstructors.ColumnStyles.Clear();
                tblConstructors.ColumnCount = 0;

                PublicInstanceConstructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                PublicStaticConstructors = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.ReturnType == type && !x.IsSpecialName).ToArray();

                //if (PublicInstanceConstructors.Length == 1)
                //{
                //    var parameters = PublicInstanceConstructors[0].GetParameters();
                //    if (parameters.Length == 0)
                //    {
                //        FinalArguments = new object[0][];
                //        return;
                //    }
                //}

                int count = PublicInstanceConstructors.Length + PublicStaticConstructors.Length;
                Parameters = new ParameterInfo[count][];
                FinalArguments = new object[count][];

                string constructorName = ClassType.Name.Split('`')[0];
                for (int index = 0; index < PublicInstanceConstructors.Length; ++index)
                {
                    ConstructorInfo c = PublicInstanceConstructors[index];
                    DisplayConstructorMethod(constructorName, c.GetParameters(), index);
                }
                for (int index = 0; index < PublicStaticConstructors.Length; ++index)
                {
                    MethodInfo m = PublicStaticConstructors[index];
                    DisplayConstructorMethod(m.Name, m.GetParameters(), index + PublicInstanceConstructors.Length);
                }
            }
        }

        private void DisplayConstructorMethod(string funcName, ParameterInfo[] parameters, int index)
        {
            //First row: text representation
            tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblConstructors.RowCount = tblConstructors.RowStyles.Count;
            //Second row: input object editors
            tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblConstructors.RowCount = tblConstructors.RowStyles.Count;

            Parameters[index] = parameters;
            FinalArguments[index] = new object[parameters.Length];

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
                Text = funcName,
                Dock = DockStyle.Left,
                AutoSize = true,
                Tag = index,
                Checked = index == 0,
            };

            ConstructorIndex = 0;

            constructorSelector.CheckedChanged += ConstructorSelector_CheckedChanged;
            tblConstructors.Controls.Add(constructorSelector, 0, tblConstructors.RowCount - 2);
            
            for (int paramIndex = 0; paramIndex < parameters.Length; ++paramIndex)
            {
                ParameterInfo p = parameters[paramIndex];
                Type t = p.ParameterType;

                FinalArguments[index][paramIndex] = t.GetDefaultValue();

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

                Control paramTool = CreateControl(t, p.Position, index, FinalArguments);
                if (paramTool != null)
                    tblConstructors.Controls.Add(paramTool, paramIndex + 1, tblConstructors.RowCount - 1);
            }
        }

        private void ResizeArray(NumericInputBoxBase<int> box, int? previous, int? current)
        {
            object[][] array = FinalArguments;
            int countBefore;
            if (array == null)
            {
                array = new object[current.Value][];
                if (current.Value == 0)
                    return;
                countBefore = 0;
            }
            else
            {
                if (current.Value == array.Length || current.Value < 0)
                    return;
                countBefore = array.Length;
                Array.Resize(ref array, current.Value);
            }
            FinalArguments = array;
            tblConstructors.RowCount = array.Length;
            if (countBefore < array.Length)
            {
                object def = ClassType.GetDefaultValue();
                for (int i = countBefore; i < array.Length; ++i)
                {
                    tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    FinalArguments[i] = new object[1] { def };
                    tblConstructors.Controls.Add(CreateControl(ClassType, 0, i, FinalArguments), 0, i);
                }
            }
            else
            {
                for (int i = array.Length; i < countBefore; ++i)
                {
                    tblConstructors.RowStyles.RemoveAt(array.Length);
                    tblConstructors.Controls.Remove(tblConstructors.GetControlFromPosition(0, i));
                }
            }
        }
        public Control CreateControl(Type type, int columnIndex, int rowIndex, object[][] finalArguments)
        {
            Control paramTool = null;
            bool nullable = false;
            if (type.IsGenericParameter)
            {
                TypeInfo info = IntrospectionExtensions.GetTypeInfo(ClassType);
                int argIndex = Array.FindIndex(info.GenericTypeParameters, x => x == type);
                if (_preSelectedGenericTypeArgs.IndexInArrayRange(argIndex))
                    type = _preSelectedGenericTypeArgs[argIndex];
            }
            ArgumentInfo arg = new ArgumentInfo()
            {
                Type = type,
                ColumnIndex = columnIndex,
                RowIndex = rowIndex,
                Value = finalArguments[rowIndex][columnIndex],
            };
            Type temp = Nullable.GetUnderlyingType(type);
            if (nullable = temp != null)
                type = temp;
            switch (type.Name)
            {
                case "Boolean":
                    {
                        CheckBox box = new CheckBox()
                        {
                            Tag = arg,
                            ThreeState = nullable,
                            CheckState = (arg.Value == null ? CheckState.Indeterminate : ((bool)arg.Value == true ? CheckState.Checked : CheckState.Unchecked)),
                        };
                        box.CheckedChanged += (sender, e) =>
                        {
                            CheckBox checkBox = (CheckBox)sender;
                            ArgumentInfo argInfo = (ArgumentInfo)checkBox.Tag;

                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = 
                            argInfo.Value = (checkBox.CheckState == CheckState.Checked ? true : (checkBox.CheckState == CheckState.Indeterminate ? (bool?)null : false));
                        };
                        paramTool = box;
                    }
                    break;
                case "SByte":
                    {
                        NumericInputBoxSByte box = new NumericInputBoxSByte()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (sbyte?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Byte":
                    {
                        NumericInputBoxByte box = new NumericInputBoxByte()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (byte?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Char":
                    {
                        //TODO: make textbox handle single char input
                        TextBox box = new TextBox()
                        {
                            Tag = arg,
                            Text = ((char)arg.Value).ToString(),
                        };
                        paramTool = box;
                    }
                    break;
                case "Int16":
                    {
                        NumericInputBoxInt16 box = new NumericInputBoxInt16()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (short?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "UInt16":
                    {
                        NumericInputBoxUInt16 box = new NumericInputBoxUInt16()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (ushort?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Int32":
                    {
                        NumericInputBoxInt32 box = new NumericInputBoxInt32()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (int?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "UInt32":
                    {
                        NumericInputBoxUInt32 box = new NumericInputBoxUInt32()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (uint?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Int64":
                    {
                        NumericInputBoxInt64 box = new NumericInputBoxInt64()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (long?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "UInt64":
                    {
                        NumericInputBoxUInt64 box = new NumericInputBoxUInt64()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (ulong?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Single":
                    {
                        NumericInputBoxSingle box = new NumericInputBoxSingle()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (float?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Double":
                    {
                        NumericInputBoxDouble box = new NumericInputBoxDouble()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (double?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "Decimal":
                    {
                        NumericInputBoxDecimal box = new NumericInputBoxDecimal()
                        {
                            Nullable = nullable,
                            Tag = arg,
                            Value = (decimal?)arg.Value,
                        };
                        box.ValueChanged += (b, prev, current) =>
                        {
                            ArgumentInfo argInfo = (ArgumentInfo)b.Tag;
                            argInfo.Value = current;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
                        };
                        paramTool = box;
                    }
                    break;
                case "String":
                    {
                        TextBox box = new TextBox()
                        {
                            Tag = arg,
                            Text = arg.Value as string,
                        };
                        box.TextChanged += (sender, e) =>
                        {
                            TextBox s = (TextBox)sender;
                            ArgumentInfo argInfo = (ArgumentInfo)s.Tag;
                            argInfo.Value = s.Text;
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = s.Text;
                        };
                        paramTool = box;
                    }
                    break;
                default:
                    {
                        Label objectSelectionLabel = new Label()
                        {
                            Text = arg.Value == null ? "null" : arg.Value.ToString(),
                            Tag = arg,
                            BackColor = Color.FromArgb(70, 75, 90),
                        };
                        objectSelectionLabel.MouseEnter += (sender, e) =>
                        {
                            Label s = (Label)sender;
                            s.BackColor = Color.FromArgb(50, 55, 70);
                        };
                        objectSelectionLabel.MouseLeave += (sender, e) =>
                        {
                            Label s = (Label)sender;
                            s.BackColor = Color.FromArgb(70, 75, 90);
                        };
                        objectSelectionLabel.MouseClick += (sender, e) =>
                        {
                            Label s = (Label)sender;
                            ArgumentInfo argInfo = (ArgumentInfo)s.Tag;
                            Type t = argInfo.Type;
                            if (t.IsGenericParameter)
                            {
                                TypeInfo info = IntrospectionExtensions.GetTypeInfo(ClassType);
                                int argIndex = Array.FindIndex(info.GenericTypeParameters, x => x == t);
                                if (_preSelectedGenericTypeArgs.IndexInArrayRange(argIndex))
                                    t = _preSelectedGenericTypeArgs[argIndex];
                            }
                            object o = Editor.UserCreateInstanceOf(t, true);

                            if (o == null && argInfo.Type.IsValueType)
                                o = argInfo.Type.GetDefaultValue();

                            argInfo.Value = o;
                            s.Text = o == null ? "null" : o.ToString();
                            finalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = o;
                        };
                        paramTool = objectSelectionLabel;
                    }
                    break;
            }
            if (paramTool != null)
            {
                paramTool.Padding = new Padding(0);
                paramTool.Margin = new Padding(0);
                paramTool.Dock = DockStyle.Top;
                paramTool.AutoSize = true;
                paramTool.BackColor = Color.FromArgb(50, 55, 70);
                paramTool.ForeColor = Color.FromArgb(200, 200, 220);
            }
            return paramTool;
        }
        
        //public static ObjectCreator Current = null;
        //protected override void OnActivated(EventArgs e)
        //{
        //    Current = this;
        //    base.OnActivated(e);
        //}
        //protected override void OnDeactivate(EventArgs e)
        //{
        //    Current = null;
        //    base.OnDeactivate(e);
        //}
    }
}
