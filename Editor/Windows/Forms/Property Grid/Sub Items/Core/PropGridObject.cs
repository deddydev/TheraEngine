﻿using AppDomainToolkit;
using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Reflection;
using WeifenLuo.WinFormsUI.Docking;
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
        protected TypeProxy _currentType;
        public TypeProxy CurrentType => _currentType ?? DataType;

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

        protected override bool UpdateDisplayInternal(object value)
        {
            //Value is boxed as object, so this doesn't work
            //if (pnlProps.Visible && !ReferenceEquals(value, _object))
            //    LoadProperties(value);
            
            _object = value;
            TypeProxy type = _object?.GetTypeProxy();
            bool nullType = type is null;
            if (!nullType)
            {
                if (type != CurrentType)
                {
                    _currentType = type;
                    UpdateMouseDown();
                }
            }
            else if (!(_currentType is null))
            {
                _currentType = null;
                UpdateMouseDown();
            }

            var settings = Editor.GetSettings();
            if (settings.PropertyGrid.ShowTypeNames && 
                !(MemberInfo is PropGridMemberInfoIList ilist && 
                ilist.ListElementType == CurrentType))
            {
                string typeName = (value?.GetTypeProxy() ?? DataType)?.GetFriendlyName() ?? string.Empty;
                lblObjectTypeName.Text = "[" + typeName + "] " + (value is null ? "null" : value.ToString());
            }
            else
            {
                lblObjectTypeName.Text = value is null ? "null" : value.ToString();
            }

            if (chkNull.Checked = _object is null)
            {
                pnlProps.Visible = false;
                lblObjectTypeName.Enabled = false;
            }
            else
            {
                pnlProps.Visible = AlwaysVisible;
                lblObjectTypeName.Enabled = true;
            }
            return false;
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
        private void LoadProperties(bool notNull)
        {
            var propGridSettings = Editor.GetSettings().PropertyGrid;
            TheraPropertyGrid.LoadPropertiesToPanel(
                ParentCategory?.PropertyGrid,
                pnlProps, _categories,
                notNull ? _object : null,
                this, DataChangeHandler, true,
                propGridSettings.DisplayMethods, propGridSettings.DisplayEvents);
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            if (_object != null && !AlwaysVisible)
                lblObjectTypeName.BackColor = chkNull.BackColor = Color.FromArgb(105, 140, 170);
        }

        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            if (_object != null && !AlwaysVisible)
                lblObjectTypeName.BackColor = chkNull.BackColor = Color.Transparent;
        }
        
        internal protected override void SetReferenceHolder(PropGridMemberInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
            if (parentInfo?.DataType != null)
            {
                if (parentInfo.DataType.IsValueType)
                {
                    chkNull.Visible = false;
                    pnlHeader.BackColor = Color.FromArgb(105, 110, 140);
                }
                else
                {
                    chkNull.Visible = true;
                    pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
                }
            }
            UpdateMouseDown();
        }

        public void Expand() => pnlProps.Visible = true;
        public void Collapse() => pnlProps.Visible = AlwaysVisible;
        public void Toggle() => pnlProps.Visible = AlwaysVisible ? true : !pnlProps.Visible;
        public ControlCollection ChildControls => pnlProps.Controls;

        public bool EditInPlace { get; set; } = false;
        public bool AlwaysVisible { get; set; } = false;

        private void UpdateMouseDown()
        {
            _mouseDown = MouseDownProperties;

            TypeProxy type = CurrentType;
            if (type != null && !type.IsValueType)
            {
                TypeProxy compareType;
                TypeProxy bestType = null;
                EngineDomainProxyEditor proxy = Engine.DomainProxy as EngineDomainProxyEditor;
                if (proxy.FullEditorTypes is null)
                    return;

                while (type != null && type != typeof(object))
                {
                    string name = type.GetFriendlyName();
                    if (proxy.FullEditorTypes.ContainsKey(type))
                    {
                        bestType = type;
                        break;
                    }

                    foreach (Type intfType in type.GetInterfaces())
                    {
                        if (proxy.FullEditorTypes.ContainsKey(intfType))
                        {
                            compareType = intfType;
                            if (bestType is null || compareType.IsAssignableTo(bestType))
                                bestType = compareType;
                        }
                    }
                    if (bestType != null)
                        break;
                    type = type.BaseType;
                }
                if (bestType != null)
                {
                    _editorType = proxy.FullEditorTypes[bestType];
                    _mouseDown = MouseDownEditor;
                    return;
                }
            }
        }

        private TypeProxy _editorType;
        private Action _mouseDown;
        private void MouseDownEditor()
        {
            object value = GetValue();
            Form form = Activator.CreateInstance((Type)_editorType, value) as Form;
            if (form is DockContent newFormDockable && !(form is TheraForm))
            {
                DockContent propGridForm = ParentCategory.PropertyGrid.FindForm() as DockContent;
                DockPanel targetDockPanel = propGridForm?.DockPanel ?? Editor.Instance.DockPanel;
                newFormDockable.Show(targetDockPanel, DockState.Document);
            }
            else
                form?.ShowDialog(this);
        }
        private void MouseDownProperties()
        {
            if (EditInPlace)
                pnlProps.Visible = AlwaysVisible ? true : !pnlProps.Visible;
            else
                ParentCategory?.PropertyGrid?.SetObject(_object, MemberInfo.MemberAccessor);
        }
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            if (_object != null)
                _mouseDown();
        }
        
        private void pnlProps_VisibleChanged(object sender, EventArgs e)
        {
            //TODO: execute on game domain
            RemoteAction.Invoke(AppDomainHelper.GameAppDomain, () =>
            {
                if (pnlProps.Visible)
                {
                    if (pnlProps.Controls.Count == 0)
                        LoadProperties(true);
                }
                else
                    LoadProperties(false);
            });
        }
        
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;

            if (chkNull.Checked)
            {
                UpdateValue(null, true);
                if ((GetValue() is null))
                    return;

                Engine.Out("Unable to set this property to null.");
            }
            
            object o = Editor.DomainProxy.UserCreateInstanceOf(DataType, true);

            UpdateValue(o, true);
        }
    }
}
