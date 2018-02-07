using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;
using TheraEditor.Windows.Forms.PropertyGrid;
using System.Reflection;

namespace TheraEditor.Windows.Forms
{
    public partial class MaterialControl : UserControl
    {
        public MaterialControl()
        {
            InitializeComponent();
        }

        private TMaterial _material;
        public TMaterial Material
        {
            get => _material;
            set
            {
                _material = value;
                if (_material != null)
                {
                    foreach (ShaderVar v in _material.Parameters)
                    {
                        Type t = ShaderVar.AssemblyTypeAssociations[v.TypeName];
                        var ct = TheraPropertyGrid.GetControlTypes(t);
                        TheraPropertyGrid.InstantiatePropertyEditors(ct, v.GetType().GetProperty("Value"), v, Control_PropertyObjectChanged);
                    }
                }
            }
        }

        private void Control_PropertyObjectChanged(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo)
        {
            //btnSave.Visible = true;
            //Editor.Instance.UndoManager.AddChange(TargetObject.EditorState, oldValue, newValue, propertyOwner, propertyInfo);
        }
    }
}
