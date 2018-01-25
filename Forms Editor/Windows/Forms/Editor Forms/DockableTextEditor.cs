using System.Drawing;
using TheraEngine.Scripting;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableTextEditor : DockContent
    {
        public DockableTextEditor()
        {
            InitializeComponent();
            //TextBox.SetWhitespaceBackColor(true, Color.FromArgb(82, 83, 90));
            //TextBox.SetWhitespaceForeColor(true, Color.FromArgb(224, 224, 224));
        }

        private PythonScript _script = null;
        public PythonScript Script
        {
            get => _script;
            set
            {
                _script = value;
                //TextBox.Text = _script == null ? string.Empty : _script.Text;
            }
        }
    }
}
