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
        private class ArgumentInfo
        {
            public Type Type { get; set; }
            public int ColumnIndex { get; set; }
            public int RowIndex { get; set; }
            public object Value { get; set; }
        }
        public enum EObjectCreatorMode
        {
            Object,
            Array,
            Boolean,
            Char,
            String,
            Int8,
            UInt8,
            Int16,
            UInt16,
            Int32,
            UInt32,
            Int64,
            UInt64,
            Decimal,
            Single,
            Double
        }

        public ObjectCreator() : base()
        {
            InitializeComponent();
            numArrayLength.MinimumValue = 0;
            toolStripTypeSelection.Renderer = new TheraToolstripRenderer();
        }

        private bool _updating = false;
        internal Type[] _genericTypeArgs = null;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Text = "Object Creator";
            if (ClassType != null)
                Text += " - " + ClassType.GetFriendlyName();
        }

        /// <summary>
        /// Returns true if the dialog needs to be shown.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allowDerivedTypes"></param>
        /// <returns></returns>
        public bool Initialize(Type type, bool allowDerivedTypes)
        {
            ConstructedObject = null;

            if (type.IsGenericTypeDefinition)
                throw new InvalidOperationException("Cannot create an instance of a generic type definition. Pass in the type created with each generic parameter set.");

            if (type.IsGenericParameter)
                throw new InvalidOperationException("Cannot create an instance of a generic parameter. Pass in the type created from the parameter constraints.");

            if (type.IsGenericType)
            {
                _genericTypeArgs = type.GenericTypeArguments;
                type = type.GetGenericTypeDefinition();
            }

            IsNullable = type == typeof(Nullable<>);
            if (IsNullable)
            {
                type = _genericTypeArgs[0];
                if (type.IsGenericType)
                {
                    _genericTypeArgs = type.GenericTypeArguments;
                    type = type.GetGenericTypeDefinition();
                }
                else
                    _genericTypeArgs = null;
            }

            if (type.IsArray)
            {
                Mode = EObjectCreatorMode.Array;
                type = type.GetElementType();
            }
            else
            {
                switch (type.Name)
                {
                    default:
                        Mode = EObjectCreatorMode.Object;
                        break;
                    case "Char":
                        Mode = EObjectCreatorMode.Char;
                        break;
                    case "String":
                        //Mode = EObjectCreatorMode.String;
                        ConstructedObject = string.Empty;
                        return false;
                        //break;
                    case "Boolean":
                        Mode = EObjectCreatorMode.Boolean;
                        break;
                    case "SByte":
                        Mode = EObjectCreatorMode.Int8;
                        numericInputBoxSByte.Nullable = chkNull.Visible;
                        break;
                    case "Byte":
                        Mode = EObjectCreatorMode.UInt8;
                        numericInputBoxByte.Nullable = chkNull.Visible;
                        break;
                    case "Int16":
                        Mode = EObjectCreatorMode.Int16;
                        numericInputBoxInt16.Nullable = chkNull.Visible;
                        break;
                    case "UInt16":
                        Mode = EObjectCreatorMode.UInt16;
                        numericInputBoxUInt16.Nullable = chkNull.Visible;
                        break;
                    case "Int32":
                        Mode = EObjectCreatorMode.Int32;
                        numericInputBoxInt32.Nullable = chkNull.Visible;
                        break;
                    case "UInt32":
                        Mode = EObjectCreatorMode.UInt32;
                        numericInputBoxUInt32.Nullable = chkNull.Visible;
                        break;
                    case "Int64":
                        Mode = EObjectCreatorMode.Int64;
                        numericInputBoxInt64.Nullable = chkNull.Visible;
                        break;
                    case "UInt64":
                        Mode = EObjectCreatorMode.UInt64;
                        numericInputBoxUInt64.Nullable = chkNull.Visible;
                        break;
                    case "Single":
                        Mode = EObjectCreatorMode.Single;
                        numericInputBoxSingle.Nullable = chkNull.Visible;
                        break;
                    case "Double":
                        Mode = EObjectCreatorMode.Double;
                        numericInputBoxDouble.Nullable = chkNull.Visible;
                        break;
                    case "Decimal":
                        Mode = EObjectCreatorMode.Decimal;
                        numericInputBoxDecimal.Nullable = chkNull.Visible;
                        break;
                }
            }

            if (Mode == EObjectCreatorMode.Object || 
                Mode == EObjectCreatorMode.Array)
            {
                if (allowDerivedTypes)
                {
                    Type[] types = Program.PopulateTreeView(treeView1, OnTypeSelected, x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
                    if (types.Length > 1)
                    {
                        treeView1.Visible = true;
                        treeView1.ExpandAll();
                        return true;
                    }
                    else if (types.Length == 1)
                        type = types[0];
                    //else
                    //    return false;
                }

                if (type.IsAbstract || type.IsInterface)
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
                                if (type.IsGenericTypeDefinition)
                                    type = type.MakeGenericType(_genericTypeArgs);
                                ConstructedObject = Activator.CreateInstance(type);
                                return false;
                            }
                        }
                        else
                        {
                            if (type.IsEnum || type.IsPrimitive)
                            {
                                ConstructedObject = type.GetDefaultValue();
                                return false;
                            }
                            else
                            {
                                var staticConstructors = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.ReturnType == type && !x.IsSpecialName).ToArray();
                                if (staticConstructors.Length == 0)
                                {
                                    Engine.LogWarning($"Can't create type {type.GetFriendlyName()}; has no public constructors.");
                                    return false;
                                }
                                else if (staticConstructors.Length == 1)
                                {
                                    MethodInfo method = staticConstructors[0];
                                    if (method.GetParameters().Length == 0)
                                    {
                                        if (type.IsGenericTypeDefinition)
                                            type = type.MakeGenericType(_genericTypeArgs);
                                        ConstructedObject = method.Invoke(null, null);
                                        return false;
                                    }
                                }
                            }
                        }
                    }

                    SetTargetType(type);
                    treeView1.Visible = false;
                }
            }
            else
            {
                SetTargetType(type);
                treeView1.Visible = false;
            }
            
            return true;
        }

        private void OnTypeSelected(object sender, EventArgs e)
        {
            TreeNode item = sender as TreeNode;
            SetTargetType(item?.Tag as Type);
        }

        //private void ConstructorSelector_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (_updating)
        //        return;
        //    CheckBox c = (CheckBox)sender;
        //    ConstructorIndex = (int)c.Tag;
        //    _updating = true;
        //    if (c.Checked)
        //    {
        //        for (int i = 0; i < FinalArguments.Length; ++i)
        //        {
        //            if (i == ConstructorIndex)
        //                continue;
        //            int rowIndex = i << 1;
        //            CheckBox box = (CheckBox)tblConstructors.GetControlFromPosition(0, rowIndex);
        //            box.Checked = false;
        //        }
        //    }
        //    else
        //        c.Checked = true;
        //    _updating = false;
        //}

        public bool IsNullable { get; private set; }
        public int ConstructorIndex { get; private set; } = -1;
        public Type ClassType { get; private set; }
        public ConstructorInfo[] PublicInstanceConstructors { get; private set; }
        public MethodInfo[] PublicStaticConstructors { get; private set; }
        public object[][] FinalArguments;
        public ParameterInfo[][] Parameters;
        public object ConstructedObject { get; private set; } = null;
        private EObjectCreatorMode _mode = EObjectCreatorMode.Object;
        private EObjectCreatorMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                if (_mode == EObjectCreatorMode.Object)
                {
                    treeView1.Visible = true;
                    pnlArrayLength.Visible = false;
                    toolStripDropDownButton1.Visible = true;
                    tblConstructors.Visible = true;
                    richTextBox1.Visible = false;
                    toolStripDropDownButton1.Text = "Select an object type...";
                    tblConstructors.Controls.Clear();

                    numericInputBoxByte.Visible =
                    numericInputBoxSByte.Visible =
                    numericInputBoxSingle.Visible =
                    numericInputBoxDouble.Visible =
                    numericInputBoxDecimal.Visible =
                    numericInputBoxInt16.Visible =
                    numericInputBoxUInt16.Visible =
                    numericInputBoxInt32.Visible =
                    numericInputBoxUInt32.Visible =
                    numericInputBoxInt64.Visible =
                    numericInputBoxUInt64.Visible =
                    richTextBox1.Visible = 
                    chkBoolean.Visible =
                    false;
                }
                else if (_mode == EObjectCreatorMode.Array)
                {
                    treeView1.Visible = false;
                    pnlArrayLength.Visible = true;
                    toolStripDropDownButton1.Visible = true;
                    tblConstructors.Visible = true;
                    richTextBox1.Visible = false;
                    cboConstructor.Visible = false;
                    toolStripDropDownButton1.Text = "Select an element type...";

                    numericInputBoxByte.Visible =
                    numericInputBoxSByte.Visible =
                    numericInputBoxSingle.Visible =
                    numericInputBoxDouble.Visible =
                    numericInputBoxDecimal.Visible =
                    numericInputBoxInt16.Visible =
                    numericInputBoxUInt16.Visible =
                    numericInputBoxInt32.Visible =
                    numericInputBoxUInt32.Visible =
                    numericInputBoxInt64.Visible =
                    numericInputBoxUInt64.Visible =
                    richTextBox1.Visible =
                    chkBoolean.Visible = 
                    false;

                    tblConstructors.Controls.Clear();

                    tblConstructors.ColumnStyles.Clear();
                    tblConstructors.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                    tblConstructors.ColumnCount = 1;

                    tblConstructors.RowStyles.Clear();
                    tblConstructors.RowCount = numArrayLength.Value.Value;

                    for (int i = 0; i < tblConstructors.RowCount; ++i)
                        tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }
                else
                {
                    treeView1.Visible =
                    pnlArrayLength.Visible =
                    toolStripDropDownButton1.Visible =
                    cboConstructor.Visible =
                    tblConstructors.Visible = false;

                    toolStripDropDownButton1.Text = string.Empty;

                    numericInputBoxByte.Visible     = _mode == EObjectCreatorMode.UInt8;
                    numericInputBoxSByte.Visible    = _mode == EObjectCreatorMode.Int8;
                    numericInputBoxSingle.Visible   = _mode == EObjectCreatorMode.Single;
                    numericInputBoxDouble.Visible   = _mode == EObjectCreatorMode.Double;
                    numericInputBoxDecimal.Visible  = _mode == EObjectCreatorMode.Decimal;
                    numericInputBoxInt16.Visible    = _mode == EObjectCreatorMode.Int16;
                    numericInputBoxUInt16.Visible   = _mode == EObjectCreatorMode.UInt16;
                    numericInputBoxInt32.Visible    = _mode == EObjectCreatorMode.Int32;
                    numericInputBoxUInt32.Visible   = _mode == EObjectCreatorMode.UInt32;
                    numericInputBoxInt64.Visible    = _mode == EObjectCreatorMode.Int64;
                    numericInputBoxUInt64.Visible   = _mode == EObjectCreatorMode.UInt64;
                    richTextBox1.Visible            = _mode == EObjectCreatorMode.String || _mode == EObjectCreatorMode.Char;
                    chkBoolean.Visible              = _mode == EObjectCreatorMode.Boolean;
                }
            }
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            if (chkNull.Checked)
            {
                ConstructedObject = null;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            if (ClassType.ContainsGenericParameters)
            {
                if (_genericTypeArgs != null)
                    ClassType = ClassType.MakeGenericType(_genericTypeArgs);
                else
                {
                    using (GenericsSelector selector = new GenericsSelector(ClassType))
                    {
                        DialogResult result = selector.ShowDialog(this);
                        if (result == DialogResult.OK)
                            ClassType = selector.FinalClassType;
                        else
                            return;
                    }
                }
            }

            switch (Mode)
            {
                case EObjectCreatorMode.Array:
                    ConstructedObject = Array.CreateInstance(ClassType, FinalArguments.GetLength(0));
                    FinalArguments.Select(x => x[0]).ToArray().CopyTo((Array)ConstructedObject, 0);
                    break;
                case EObjectCreatorMode.String:
                    ConstructedObject = richTextBox1.Text;
                    break;
                case EObjectCreatorMode.Char:
                    ConstructedObject = richTextBox1.Text.Length == 0 ? 0 : richTextBox1.Text[0];
                    break;
                case EObjectCreatorMode.Int8:
                    ConstructedObject = numericInputBoxSByte.Value;
                    break;
                case EObjectCreatorMode.UInt8:
                    ConstructedObject = numericInputBoxByte.Value;
                    break;
                case EObjectCreatorMode.Int16:
                    ConstructedObject = numericInputBoxInt16.Value;
                    break;
                case EObjectCreatorMode.UInt16:
                    ConstructedObject = numericInputBoxUInt16.Value;
                    break;
                case EObjectCreatorMode.Int32:
                    ConstructedObject = numericInputBoxInt32.Value;
                    break;
                case EObjectCreatorMode.UInt32:
                    ConstructedObject = numericInputBoxUInt32.Value;
                    break;
                case EObjectCreatorMode.Int64:
                    ConstructedObject = numericInputBoxInt64.Value;
                    break;
                case EObjectCreatorMode.UInt64:
                    ConstructedObject = numericInputBoxUInt64.Value;
                    break;
                case EObjectCreatorMode.Single:
                    ConstructedObject = numericInputBoxSingle.Value;
                    break;
                case EObjectCreatorMode.Double:
                    ConstructedObject = numericInputBoxDouble.Value;
                    break;
                case EObjectCreatorMode.Decimal:
                    ConstructedObject = numericInputBoxDecimal.Value;
                    break;
                case EObjectCreatorMode.Boolean:
                    ConstructedObject = chkBoolean.Checked;
                    break;
                case EObjectCreatorMode.Object:
                    object[] paramData = FinalArguments.IndexInArrayRange(ConstructorIndex) ? FinalArguments[ConstructorIndex] : null;
                    if (ConstructorIndex < PublicInstanceConstructors.Length)
                    {
                        if (paramData == null || paramData.Length == 0)
                        {
                            bool hasParameterlessConstructor = ClassType.GetConstructors().FirstOrDefault(x => x.GetParameters().Length == 0) != null;
                            if (hasParameterlessConstructor)
                                ConstructedObject = Activator.CreateInstance(ClassType);
                            else
                            {
                                ConstructedObject = null;
                                Engine.PrintLine($"Unable to create {ClassType.GetFriendlyName()}; no valid constructor available.");
                            }
                        }
                        else
                        {
                            var parameters = FinalArguments[ConstructorIndex];
                            ConstructorInfo constructorInfo = ClassType.GetConstructors()[ConstructorIndex];
                            ConstructedObject = constructorInfo.Invoke(parameters);
                        }
                    }
                    else
                    {
                        int index = ConstructorIndex - PublicInstanceConstructors.Length;
                        ConstructedObject = PublicStaticConstructors[index].Invoke(null, paramData);
                    }
                    break;
            }

            if (IsNullable)
            {
                ClassType = typeof(Nullable<>).MakeGenericType(ClassType);
                ConstructedObject = Activator.CreateInstance(ClassType, ConstructedObject);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void SetTargetType(Type type)
        {
            if (type == null)
                return;

            ClassType = type;

            bool notNull = ClassType != null;

            cboConstructor.Visible = 
                notNull && 
                Mode == EObjectCreatorMode.Object && 
                ClassType.GetConstructors().Length > 1;
            tblConstructors.Controls.Clear();
            toolStripDropDownButton1.Text = type?.GetFriendlyName();
            chkNull.Visible = (IsNullable || !type.IsValueType) && notNull;
            btnOkay.Enabled = notNull;

            if (Mode == EObjectCreatorMode.Array)
            {
                treeView1.Visible = true;
                FinalArguments = new object[numArrayLength.Value.Value][].FillWith(new object[1] { type.GetDefaultValue() });
                for (int i = 0; i < numArrayLength.Value.Value; ++i)
                    tblConstructors.Controls.Add(CreateControl(type, 0, i));
            }
            else
            {
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

                cboConstructor.Items.Clear();
                for (int index = 0; index < PublicInstanceConstructors.Length; ++index)
                    cboConstructor.Items.Add(PublicInstanceConstructors[index].GetFriendlyName());
                for (int index = 0; index < PublicStaticConstructors.Length; ++index)
                    cboConstructor.Items.Add(PublicStaticConstructors[index].GetFriendlyName());
                if (cboConstructor.Items.Count > 0)
                    cboConstructor.SelectedIndex = 0;
            }
        }

        private void cboConstructor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cboConstructor.SelectedIndex;
            if (index < PublicInstanceConstructors.Length)
            {
                ConstructorInfo c = PublicInstanceConstructors[index];
                DisplayConstructorMethod(ClassType.Name.Split('`')[0], c.GetParameters(), index);
            }
            else
            {
                index -= PublicInstanceConstructors.Length;
                if (index < PublicStaticConstructors.Length)
                {
                    MethodInfo m = PublicStaticConstructors[index];
                    DisplayConstructorMethod(m.Name, m.GetParameters(), PublicInstanceConstructors.Length + index);
                }
            }
            ConstructorIndex = index;
        }

        private void DisplayConstructorMethod(string funcName, ParameterInfo[] parameters, int index)
        {
            tblConstructors.Controls.Clear();
            tblConstructors.RowStyles.Clear();
            tblConstructors.RowCount = 0;
            tblConstructors.ColumnStyles.Clear();
            tblConstructors.ColumnCount = 0;
            
            tblConstructors.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblConstructors.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblConstructors.ColumnCount = 2;

            Parameters[index] = parameters;
            FinalArguments[index] = new object[parameters.Length];

            //Update row count if the number of parameters exceeds current count
            int rows = Math.Max(parameters.Length + 1, tblConstructors.RowCount);
            if (rows > tblConstructors.RowCount)
            {
                int total = rows - tblConstructors.ColumnCount;
                for (int i = 0; i < total; ++i)
                {
                    tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    tblConstructors.RowCount = tblConstructors.RowStyles.Count;
                }
            }

            //CheckBox constructorSelector = new CheckBox()
            //{
            //    Text = funcName,
            //    Dock = DockStyle.Left,
            //    AutoSize = true,
            //    Tag = index,
            //    Checked = index == 0,
            //    ForeColor = Color.FromArgb(200, 200, 220),
            //    BackColor = Color.Transparent,
            //};
            
            //constructorSelector.CheckedChanged += ConstructorSelector_CheckedChanged;
            //tblConstructors.Controls.Add(constructorSelector, 0, tblConstructors.RowCount - 2);
            
            for (int paramIndex = 0; paramIndex < parameters.Length; ++paramIndex)
            {
                ParameterInfo p = parameters[paramIndex];
                Type t = p.ParameterType;

                FinalArguments[index][paramIndex] = t.GetDefaultValue();

                string typeName = t.GetFriendlyName();
                string varName = p.Name;
                string finalText = typeName + " " + varName;

                Label paramLabel = new Label()
                {
                    TextAlign = ContentAlignment.MiddleRight,
                    Text = finalText,
                    Dock = DockStyle.Right,
                    AutoSize = true,
                    Padding = new Padding(0),
                    Margin = new Padding(5),
                    BackColor = Color.FromArgb(20, 20, 20),
                    ForeColor = Color.FromArgb(224, 224, 224),
                };
                tblConstructors.Controls.Add(paramLabel, 0, paramIndex);

                Control paramTool = CreateControl(t, p.Position, index);
                if (paramTool != null)
                    tblConstructors.Controls.Add(paramTool, 1, paramIndex);
            }
        }

        private void ResizeArray(NumericInputBoxBase<int> box, int? previous, int? current)
        {
            if (current == null)
                return;

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
                    if (ClassType != null)
                    {
                        var control = CreateControl(ClassType, 0, i);
                        if (control != null)
                            tblConstructors.Controls.Add(control, 0, i);
                    }
                }
            }
            else
            {
                for (int i = Math.Min(tblConstructors.RowStyles.Count - 1, countBefore - 1); i >= array.Length; --i)
                {
                    tblConstructors.Controls.Remove(tblConstructors.GetControlFromPosition(0, i));
                    tblConstructors.Controls.Remove(tblConstructors.GetControlFromPosition(1, i));
                    tblConstructors.RowStyles.RemoveAt(i);
                }
            }
        }
        public Control CreateControl(Type type, int columnIndex, int rowIndex)
        {
            Control paramTool = null;
            bool nullable = false;
            if (type == null)
                return null;
            if (type.IsGenericParameter)
            {
                TypeInfo info = ClassType.GetTypeInfo();
                int argIndex = Array.FindIndex(info.GenericTypeParameters, x => x == type);
                if (_genericTypeArgs.IndexInArrayRange(argIndex))
                    type = _genericTypeArgs[argIndex];
            }
            ArgumentInfo arg = new ArgumentInfo()
            {
                Type = type,
                ColumnIndex = columnIndex,
                RowIndex = rowIndex,
                Value = FinalArguments[rowIndex][columnIndex],
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

                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = 
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = current;
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
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = s.Text;
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
                        };
                        objectSelectionLabel.MouseEnter += (sender, e) =>
                        {
                            Label s = (Label)sender;
                            s.BackColor = Color.FromArgb(s.BackColor.R + 20, s.BackColor.G + 20, s.BackColor.B + 20);
                        };
                        objectSelectionLabel.MouseLeave += (sender, e) =>
                        {
                            Label s = (Label)sender;
                            s.BackColor = Color.FromArgb(s.BackColor.R - 20, s.BackColor.G - 20, s.BackColor.B - 20);
                        };
                        objectSelectionLabel.MouseClick += (sender, e) =>
                        {
                            Label s = (Label)sender;
                            ArgumentInfo argInfo = (ArgumentInfo)s.Tag;
                            Type argType = argInfo.Type;
                            if (argType.IsGenericParameter)
                            {
                                TypeInfo info = IntrospectionExtensions.GetTypeInfo(ClassType);
                                int argIndex = Array.FindIndex(info.GenericTypeParameters, x => x == argType);
                                if (_genericTypeArgs.IndexInArrayRange(argIndex))
                                    argType = _genericTypeArgs[argIndex];
                            }
                            object o = Editor.UserCreateInstanceOf(argType, true, this);

                            if (o == null && argInfo.Type.IsValueType)
                                o = argInfo.Type.GetDefaultValue();

                            argInfo.Value = o;
                            s.Text = o == null ? "null" : o.ToString();
                            FinalArguments[argInfo.RowIndex][argInfo.ColumnIndex] = o;
                        };
                        paramTool = objectSelectionLabel;
                    }
                    break;
            }
            if (paramTool != null)
            {
                paramTool.Padding = new Padding(5);
                paramTool.Margin = new Padding(0);
                paramTool.Dock = DockStyle.Fill;
                paramTool.AutoSize = false;
                paramTool.BackColor = Color.FromArgb(20, 20, 20);
                paramTool.ForeColor = Color.FromArgb(224, 224, 224);
            }
            return paramTool;
        }

        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Enabled = !chkNull.Checked;
        }

        private void numericInputBoxByte1_ValueChanged(NumericInputBoxBase<byte> box, byte? previous, byte? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxSByte1_ValueChanged(NumericInputBoxBase<sbyte> box, sbyte? previous, sbyte? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxInt161_ValueChanged(NumericInputBoxBase<short> box, short? previous, short? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxUInt161_ValueChanged(NumericInputBoxBase<ushort> box, ushort? previous, ushort? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxInt321_ValueChanged(NumericInputBoxBase<int> box, int? previous, int? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxUInt321_ValueChanged(NumericInputBoxBase<uint> box, uint? previous, uint? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxInt641_ValueChanged(NumericInputBoxBase<long> box, long? previous, long? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxUInt641_ValueChanged(NumericInputBoxBase<ulong> box, ulong? previous, ulong? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxSingle1_ValueChanged(NumericInputBoxBase<float> box, float? previous, float? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxDouble1_ValueChanged(NumericInputBoxBase<double> box, double? previous, double? current)
        {
            chkNull.Checked = current == null;
        }

        private void numericInputBoxDecimal1_ValueChanged(NumericInputBoxBase<decimal> box, decimal? previous, decimal? current)
        {
            chkNull.Checked = current == null;
        }
    }
}
