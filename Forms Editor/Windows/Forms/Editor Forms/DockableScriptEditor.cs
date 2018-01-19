using System.Drawing;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Scripting;
using TheraEngine.Worlds.Actors;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableScriptEditor : DockContent
    {
        public DockableScriptEditor()
        {
            InitializeComponent();
            TextBox.SetWhitespaceBackColor(true, Color.FromArgb(82, 83, 90));
            TextBox.SetWhitespaceForeColor(true, Color.FromArgb(224, 224, 224));
        }

        private PythonScript _script = null;
        public PythonScript Script
        {
            get => _script;
            set
            {
                _script = value;
                TextBox.Text = _script == null ? string.Empty : _script.Text;
            }
        }
    }
}
