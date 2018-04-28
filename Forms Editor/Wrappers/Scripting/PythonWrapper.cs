using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Scripting;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(PythonScript), nameof(Resources.PythonIcon))]
    public class PythonWrapper : FileWrapper<PythonScript>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static PythonWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            PythonWrapper w = GetInstance<PythonWrapper>();
        }
        #endregion
        
        public PythonWrapper() : base() { }

        public override void EditResource()
        {
            DockableTextEditor m = new DockableTextEditor();
            m.Show(Editor.Instance.DockPanel, DockState.Document);
            m.InitText(ResourceRef.File.Text, Path.GetFileName(ResourceRef.ReferencePathAbsolute), ETextEditorMode.Python);
            m.Saved += M_Saved;
        }

        private void M_Saved(DockableTextEditor obj)
        {
            ResourceRef.File.Text = obj.GetText();
            Editor.Instance.ContentTree.WatchProjectDirectory = false;
            ResourceRef.ExportReference();
            Editor.Instance.ContentTree.WatchProjectDirectory = true;
        }
    }
}