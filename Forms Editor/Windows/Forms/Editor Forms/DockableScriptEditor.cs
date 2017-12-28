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
