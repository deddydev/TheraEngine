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
using System.Collections;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridItem : UserControl
    {
        public Type ValueType { get; set; }
        public PropertyInfo Property { get; set; }
        public object PropertyOwner { get; set; }
        public Label Label { get; set; }
        public int IListIndex { get; set; }
        public IList IListOwner { get; set; }

        protected bool _updating = false;

        public PropGridItem() => InitializeComponent();
        public object GetValue()
        {
            try
            {
                if (IListOwner != null)
                    return IListOwner[IListIndex];

                if (Property == null)
                    throw new InvalidOperationException();

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

        public void UpdateValue(object newValue)
        {
            if (IListOwner != null)
                IListOwner[IListIndex] = newValue;

            if (Property == null)
                throw new InvalidOperationException();

            if (Property.CanWrite && !_updating)
            {
                Property.SetValue(PropertyOwner, newValue);

                //Update the display in case the property's set method modifies the submitted data
                UpdateDisplay();
            }
        }
        internal void SetIListOwner(IList list, Type elementType, int index)
        {
            IListOwner = list;
            IListIndex = index;
            ValueType = elementType;
            SetControlsEnabled(!list.IsReadOnly);
        }
        internal void SetProperty(PropertyInfo propertyInfo, object propertyOwner)
        {
            Property = propertyInfo;
            PropertyOwner = propertyOwner;
            ValueType = Property.PropertyType;
            SetControlsEnabled(Property.CanWrite);
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
        protected virtual void SetControlsEnabled(bool enabled) { Enabled = enabled; }
        protected virtual void UpdateDisplayInternal() { }
        protected virtual void OnLabelSet() { }
    }
}
