﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override bool ReadOnly
        {
            get => base.ReadOnly;
            set
            {
                base.ReadOnly = value;
                if (pnlProps.Visible)
                    foreach (PropGridCategory cat in _categories.Values)
                        cat.ReadOnly = value;
            }
        }

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

        private async void LoadProperties(bool notNull)
        {
            var propGridSettings = Editor.GetSettings().PropertyGrid;
            await TheraPropertyGrid.LoadPropertiesToPanel(
                pnlProps, _categories,
                notNull ? _object : null,
                notNull ? (Func<object>)(() => _object) : (() => null), 
                DataChangeHandler, ReadOnly, 
                true, propGridSettings.DisplayMethods, propGridSettings.DisplayEvents);
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
            //Form f = FindForm();
            //Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(pnlProps);
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
                    LoadProperties(true);
            }
            else
            {
                LoadProperties(false);
            }
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true, this), true);
        }
    }
}
