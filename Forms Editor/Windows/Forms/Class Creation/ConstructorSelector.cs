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
                Type[] allowed = Program.FindPublicTypes(x => !x.IsAbstract && type.IsAssignableFrom(x));
                if (allowed.Length == 0)
                    return false;

                cboTypes.Items.AddRange(allowed);
            }
            else if (type.IsAbstract)
                return false;
            else
                cboTypes.Items.Add(type);
            
            cboTypes.SelectedIndex = 0;
            
            return true;
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
            _updating = true;
            if (c.Checked)
            {
                int row = (int)c.Tag;
                for (int i = 0; i < tblConstructors.RowCount; ++i)
                {
                    
                }
            }
            else
                c.Checked = true;
            _updating = false;
        }

        private void ObjectLabel_MouseLeave(object sender, EventArgs e)
        {
            Label s = (Label)sender;
            s.BackColor = Color.FromArgb(50, 55, 70);
        }

        private void ObjectLabel_MouseEnter(object sender, EventArgs e)
        {
            Label s = (Label)sender;
            s.BackColor = Color.FromArgb(70, 75, 90);
        }

        private void ObjectLabel_MouseClick(object sender, MouseEventArgs e)
        {
            Label s = (Label)sender;
            object o = Editor.UserCreateInstanceOf((Type)s.Tag, true);
            FinalArguments[(int)s.Tag] = o;
        }

        public Type ClassType { get; private set; }
        public ConstructorInfo[] PublicInstanceConstructors { get; private set; }
        public object[] FinalArguments;
        public object ConstructedObject { get; private set; } = null;
        
        private void btnOkay_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            ConstructedObject = FinalArguments.Length == 0 ? Activator.CreateInstance(ClassType) : Activator.CreateInstance(ClassType, FinalArguments);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private Control CreateControl(Type t, ParameterInfo p)
        {
            Control paramTool = null;
            switch (t.Name)
            {
                case "Boolean":
                    paramTool = new CheckBox()
                    {

                    };
                    break;
                case "SByte":
                    paramTool = new NumericInputBoxSByte()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "Byte":
                    paramTool = new NumericInputBoxByte()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "Char":
                    paramTool = new TextBox()
                    {

                    };
                    break;
                case "Int16":
                    paramTool = new NumericInputBoxInt16()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "UInt16":
                    paramTool = new NumericInputBoxUInt16()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "Int32":
                    paramTool = new NumericInputBoxInt32()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "UInt32":
                    paramTool = new NumericInputBoxUInt32()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "Int64":
                    paramTool = new NumericInputBoxInt64()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "UInt64":
                    paramTool = new NumericInputBoxUInt64()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "Single":
                    paramTool = new NumericInputBoxSingle()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "Double":
                    paramTool = new NumericInputBoxDouble()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "Decimal":
                    paramTool = new NumericInputBoxDecimal()
                    {
                        Nullable = t.IsByRef,
                    };
                    break;
                case "String":
                    paramTool = new TextBox()
                    {

                    };
                    break;
                default:
                    Label defaultLabel = new Label()
                    {
                        Text = t.IsValueType ? "Unselected" : "null",
                        Dock = DockStyle.Left,
                        AutoSize = true,
                        BackColor = Color.FromArgb(50, 55, 70),
                        ForeColor = Color.FromArgb(200, 200, 220),
                    };
                    defaultLabel.MouseEnter += ObjectLabel_MouseEnter;
                    defaultLabel.MouseLeave += ObjectLabel_MouseLeave;
                    defaultLabel.MouseClick += ObjectLabel_MouseClick;
                    break;
            }
            return paramTool;
        }

        private void cboTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            tblConstructors.RowStyles.Clear();
            tblConstructors.RowCount = 0;
            tblConstructors.ColumnStyles.Clear();
            tblConstructors.ColumnCount = 0;

            Type type = (Type)cboTypes.SelectedItem;
            PublicInstanceConstructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (PublicInstanceConstructors.Length == 1)
            {
                var parameters = PublicInstanceConstructors[0].GetParameters();
                if (parameters.Length == 0)
                {
                    FinalArguments = new object[0];
                    return;
                }
            }

            foreach (ConstructorInfo c in PublicInstanceConstructors)
            {
                //First row: text representation
                tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                //Second row: input object editors
                tblConstructors.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                //Update row count
                tblConstructors.RowCount = tblConstructors.RowStyles.Count;

                ParameterInfo[] parameters = c.GetParameters();

                //Update column count if the number of parameters exceeds current count
                int columns = Math.Max(parameters.Length + 2, tblConstructors.ColumnCount);
                if (columns > tblConstructors.ColumnCount)
                {
                    for (int i = 0; i < columns - tblConstructors.ColumnCount; ++i)
                    {
                        tblConstructors.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                        tblConstructors.ColumnCount = tblConstructors.ColumnStyles.Count;
                    }
                }

                CheckBox constructorSelector = new CheckBox()
                {
                    Text = "",
                    Dock = DockStyle.Left,
                    AutoSize = true,
                    Tag = tblConstructors.RowCount - 2,
                };
                constructorSelector.CheckedChanged += ConstructorSelector_CheckedChanged;
                tblConstructors.Controls.Add(constructorSelector, 0, tblConstructors.RowCount - 1);

                Label constructorLabel = new Label()
                {
                    Text = c.Name,
                    Dock = DockStyle.Left,
                    AutoSize = true,
                    BackColor = Color.FromArgb(50, 55, 70),
                    ForeColor = Color.FromArgb(200, 200, 220),
                };
                tblConstructors.Controls.Add(constructorLabel, 1, tblConstructors.RowCount - 1);

                for (int i = 0; i < parameters.Length; ++i)
                {
                    ParameterInfo p = parameters[i];
                    Type t = p.ParameterType;
                    string typeName = t.GetFriendlyName();
                    string varName = p.Name;
                    string finalText = typeName + " " + varName;
                    if (i != parameters.Length - 1)
                        finalText += ", ";
                    Label paramLabel = new Label()
                    {
                        Text = finalText,
                        Dock = DockStyle.Left,
                        AutoSize = true,
                    };
                    Control paramTool = CreateControl(t, p);

                    tblConstructors.Controls.Add(paramLabel, i + 2, tblConstructors.RowCount - 2);
                    if (paramTool != null)
                        tblConstructors.Controls.Add(paramTool, i + 2, tblConstructors.RowCount - 1);
                }
            }
        }
    }
}
