using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheraEngine;
using static System.Windows.Forms.Control;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public interface ICollapsible
    {
        void Expand();
        void Collapse();
        void Toggle();
        ControlCollection ChildControls { get; }
    }
    [PropGridControlFor(typeof(object))]
    public partial class PropGridObject : PropGridItem, ICollapsible
    {
        public PropGridObject() => InitializeComponent();
        
        private const string MiscName = "Miscellaneous";
        private Dictionary<string, PropGridCategory> _categories = new Dictionary<string, PropGridCategory>();
        protected object _object;

        protected override void UpdateDisplayInternal(object value)
        {
            //Value is boxed as object, so this doesn't work
            //if (pnlProps.Visible && !ReferenceEquals(value, _object))
            //    LoadProperties(value);

            _object = value;

            if (Editor.GetSettings().PropertyGrid.ShowTypeNames)
            {
                string typeName = (value?.GetType() ?? DataType).GetFriendlyName();
                lblObjectTypeName.Text = "[" + typeName + "] " + (value == null ? "null" : value.ToString());
            }
            else
            {
                lblObjectTypeName.Text = value == null ? "null" : value.ToString();
            }

            if (chkNull.Checked = _object == null)
            {
                pnlProps.Visible = false;
                lblObjectTypeName.Enabled = false;
            }
            else
            {
                lblObjectTypeName.Enabled = true;
            }
        }

        protected override void DestroyHandle()
        {
            foreach (Control control in pnlProps.Controls)
                control.Dispose();
            pnlProps.Controls.Clear();
            foreach (var category in _categories.Values)
                category.DestroyProperties();
            _categories.Clear();
            base.DestroyHandle();
        }

        private void LoadProperties(object obj)
        {
            pnlProps.SuspendLayout();
            foreach (Control control in pnlProps.Controls)
                control.Dispose();
            pnlProps.Controls.Clear();
            foreach (var category in _categories.Values)
                category.DestroyProperties();
            _categories.Clear();

            if (obj != null)
            {
                Type targetObjectType = obj.GetType();
                PropertyInfo[] props = targetObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo prop in props)
                {
                    var indexParams = prop.GetIndexParameters();
                    if (indexParams != null && indexParams.Length > 0)
                        continue;

                    Type subType = prop.PropertyType;
                    var attribs = prop.GetCustomAttributes(true);
                    if (attribs.FirstOrDefault(x => x is BrowsableAttribute) is BrowsableAttribute browsable && !browsable.Browsable)
                        continue;

                    Type mainControlType = null;
                    Deque<Type> controlTypes = new Deque<Type>();
                    while (subType != null)
                    {
                        if (mainControlType == null && TheraPropertyGrid.InPlaceEditorTypes.ContainsKey(subType))
                        {
                            mainControlType = TheraPropertyGrid.InPlaceEditorTypes[subType];
                            if (!controlTypes.Contains(mainControlType))
                                controlTypes.PushFront(mainControlType);
                        }
                        Type[] interfaces = subType.GetInterfaces();
                        foreach (Type i in interfaces)
                            if (TheraPropertyGrid.InPlaceEditorTypes.ContainsKey(i))
                            {
                                Type controlType = TheraPropertyGrid.InPlaceEditorTypes[i];
                                if (!controlTypes.Contains(controlType))
                                    controlTypes.PushBack(controlType);
                            }

                        subType = subType.BaseType;
                    }

                    if (controlTypes.Count == 0)
                    {
                        Engine.PrintLine("Unable to find control for " + prop.PropertyType.GetFriendlyName());
                        controlTypes.PushBack(typeof(PropGridText));
                    }

                    TheraPropertyGrid.CreateControls(
                        controlTypes, new PropGridItemRefPropertyInfo(() => _object, prop), pnlProps, _categories, attribs, false, DataChangeHandler);
                }
            }

            if (Editor.GetSettings().PropertyGridRef.File.IgnoreLoneSubCategories && _categories.Count == 1)
                _categories.Values.ToArray()[0].CategoryName = null;

            pnlProps.ResumeLayout(true);
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            if (_object != null)
                lblObjectTypeName.BackColor = chkNull.BackColor = Color.FromArgb(105, 140, 170);
        }

        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            if (_object != null)
                lblObjectTypeName.BackColor = chkNull.BackColor = Color.Transparent;
        }
        
        internal protected override void SetReferenceHolder(PropGridItemRefInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
            UpdateMouseDown();
        }

        public void Expand() => pnlProps.Visible = true;
        public void Collapse() => pnlProps.Visible = false;
        public void Toggle() => pnlProps.Visible = !pnlProps.Visible;
        public ControlCollection ChildControls => pnlProps.Controls;

        private void UpdateMouseDown()
        {
            _mouseDown = MouseDownProperties;
            if (!DataType.IsValueType)
            {
                Type t = DataType;
                while (t != null && t != typeof(object))
                {
                    if (TheraPropertyGrid.FullEditorTypes.ContainsKey(t))
                    {
                        _editorType = TheraPropertyGrid.FullEditorTypes[t];
                        _mouseDown = MouseDownEditor;
                        return;
                    }
                    foreach (Type intfType in t.GetInterfaces())
                    {
                        if (TheraPropertyGrid.FullEditorTypes.ContainsKey(intfType))
                        {
                            _editorType = TheraPropertyGrid.FullEditorTypes[intfType];
                            _mouseDown = MouseDownEditor;
                            return;
                        }
                    }
                    t = t.BaseType;
                }
            }
        }

        private Type _editorType;
        private Action _mouseDown;
        private void MouseDownEditor()
        {
            using (Form f = Activator.CreateInstance(_editorType, GetValue()) as Form)
                f?.ShowDialog(this);
        }
        private void MouseDownProperties()
        {
            pnlProps.Visible = !pnlProps.Visible;
            Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(pnlProps);
        }
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            if (_object != null)
                _mouseDown();
        }
        
        private void pnlProps_VisibleChanged(object sender, EventArgs e)
        {
            if (pnlProps.Visible)
            {
                if (pnlProps.Controls.Count == 0)
                    LoadProperties(_object);
            }
            else
            {
                LoadProperties(null);
            }
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true, this), true);
        }
    }
}
