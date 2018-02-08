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
    public partial class TexRefControl : UserControl
    {
        public TexRefControl()
        {
            InitializeComponent();
        }

        private BaseTexRef _texRef;
        public BaseTexRef TexRef
        {
            get => _texRef;
            set
            {
                _texRef = value;
                if (_texRef != null)
                {

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
