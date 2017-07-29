using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    internal class MinMaxEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        IWindowsFormsEditorService _service = null;

        public override object EditValue(
            ITypeDescriptorContext context,
            IServiceProvider provider,
            object value)
        {
            if (provider != null)
                _service =
                    provider.GetService(
                    typeof(IWindowsFormsEditorService))
                    as IWindowsFormsEditorService;

            if (_service != null)
            {
                MinMaxControl control = new MinMaxControl(0.0f, 1.0f, (float)value);
                control.Closed += ControlClosed;
                
                _service.DropDownControl(control);

                if (control.DialogResult == DialogResult.OK)
                {
                    value = control.Value;
                }
                _service = null;
            }

            return value;
        }

        void ControlClosed(object sender, EventArgs e)
        {
            if (_service != null)
                _service.CloseDropDown();
        }
    }
}
