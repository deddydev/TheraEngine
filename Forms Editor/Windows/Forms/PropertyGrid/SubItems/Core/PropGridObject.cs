﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(object))]
    public partial class PropGridObject : PropGridItem
    {
        public PropGridObject() => InitializeComponent();
        
        private const string MiscName = "Miscellaneous";
        private Dictionary<string, PropGridCategory> _categories = new Dictionary<string, PropGridCategory>();

        object _object;
        protected override void UpdateDisplayInternal()
        {
            _object = GetValue();

            if (checkBox1.Checked = _object == null && pnlProps.Visible == true)
                pnlProps.Visible = false;
            
            string typeName = DataType.GetFriendlyName();
            lblObjectTypeName.Text = IListOwner != null ? _object.ToString() + " [" + typeName + "]" : typeName;
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
                        if (mainControlType == null && TheraPropertyGrid.SubItemControlTypes.ContainsKey(subType))
                        {
                            mainControlType = TheraPropertyGrid.SubItemControlTypes[subType];
                            if (!controlTypes.Contains(mainControlType))
                                controlTypes.PushFront(mainControlType);
                        }
                        Type[] interfaces = subType.GetInterfaces();
                        foreach (Type i in interfaces)
                            if (TheraPropertyGrid.SubItemControlTypes.ContainsKey(i))
                            {
                                Type controlType = TheraPropertyGrid.SubItemControlTypes[i];
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
                    CreateControl(controlTypes, prop, obj, attribs);
                }
            }

            if (Editor.Settings.File.PropertyGrid.File.IgnoreLoneSubCategories && _categories.Count == 1)
                _categories.Values.ToArray()[0].CategoryName = null;

            pnlProps.ResumeLayout(true);
        }

        private void CreateControl(Deque<Type> controlTypes, PropertyInfo prop, object obj, object[] attribs)
        {
            var controls = controlTypes.Select(x =>
            {
                var control = Activator.CreateInstance(x) as PropGridItem;
                control.SetProperty(prop, obj);
                control.Dock = DockStyle.Fill;
                control.Visible = true;
                control.Show();
                return control;
            }).ToList();

            var category = attribs.FirstOrDefault(x => x is CategoryAttribute) as CategoryAttribute;
            string catName = category == null ? MiscName : category.Category;
            if (_categories.ContainsKey(catName))
                _categories[catName].AddProperty(controls, attribs, false);
            else
            {
                PropGridCategory catCtrl = new PropGridCategory()
                {
                    CategoryName = catName,
                    Dock = DockStyle.Top,
                };
                catCtrl.AddProperty(controls, attribs, false);
                pnlProps.Controls.Add(catCtrl);
                _categories.Add(catName, catCtrl);
            }
            //}
            //else
            //    Engine.PrintLine("Unable to find control for " + subType);
        }

        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            lblObjectTypeName.BackColor = Color.FromArgb(14, 18, 34);
        }

        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            lblObjectTypeName.BackColor = Color.Transparent;
        }

        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            pnlProps.Visible = !pnlProps.Visible;
            Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(pnlProps);
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

        protected override void SetControlsEnabled(bool enabled)
        {
            checkBox1.Enabled = enabled;
        }
    }
}
