using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using TheraEngine;

namespace System.Windows.Forms
{
    public class DragRangeAttribute : Attribute
    {
        public float Minimum { get; set; }
        public float Maximum { get; set; }
        public DragRangeAttribute(float min, float max)
        {
            Minimum = min;
            Maximum = max;
        }
    }
    internal class FloatDragEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        IWindowsFormsEditorService _service = null;
        PropertyDescriptor _property;
        object _owner;
        
        public override object EditValue(
            ITypeDescriptorContext context,
            IServiceProvider provider,
            object value)
        {
            if (provider != null)
                _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            if (_service != null)
            {
                _property = context.PropertyDescriptor;
                if (_property.Attributes[typeof(DragRangeAttribute)] is DragRangeAttribute attrib)
                {
                    _owner = context.Instance;

                    MinMaxControl control = new MinMaxControl(attrib.Minimum, attrib.Maximum, (float)value);
                    control.ValueChanged += Control_ValueChanged;
                    control.Closed += ControlClosed;
                    
                    _service.DropDownControl(control);
                    value = control.Value;
                    
                    control.ValueChanged -= Control_ValueChanged;
                    control.Closed -= ControlClosed;
                    _owner = null;
                }
                _property = null;
                _service = null;
            }

            return value;
        }

        private void Control_ValueChanged(float value)
        {
            _property.SetValue(_owner, value);
            RenderPanel.GamePanel.Invalidate();
        }

        void ControlClosed(object sender, EventArgs e)
        {
            if (_service != null)
                _service.CloseDropDown();
        }
    }
}
