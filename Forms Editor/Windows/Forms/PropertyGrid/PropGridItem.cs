using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using TheraEngine;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridItem : UserControl
    {
        public PropertyInfo Property { get; set; }
        public object PropertyOwner { get; set; }
        public Label Label { get; set; }

        protected bool _updating = false;

        public PropGridItem() => InitializeComponent();
        public object GetPropertyValue()
        {
            try
            {
                if (!Property.CanRead)
                    return null;
                return Property.GetValue(PropertyOwner);
            }
            catch (Exception ex)
            {
                Engine.PrintLine(ex.ToString());
                return ex;
            }
        }
        
        public void UpdatePropertyValue(object newValue)
        {
            if (Property.CanWrite && !_updating)
                Property.SetValue(PropertyOwner, newValue);
        }
        internal void SetProperty(PropertyInfo propertyInfo, object propertyOwner)
        {
            Property = propertyInfo;
            PropertyOwner = propertyOwner;
            UpdateDisplay();
        }
        internal void SetLabel(Label label)
        {
            Label = label;
            OnLabelSet();
        }
        public void UpdateDisplay()
        {
            _updating = true;
            UpdateDisplayInternal();
            _updating = false;
        }
        protected virtual void UpdateDisplayInternal() { }
        protected virtual void OnLabelSet() { }
    }
}
