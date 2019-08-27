using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.GenericFile), nameof(Resources.GenericFile))]
    public class SkeletalModelWrapper : FileWrapper<SkeletalModel>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static SkeletalModelWrapper()
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
            SkeletalModelWrapper w = GetInstance<SkeletalModelWrapper>();
        }
        #endregion
        
        public SkeletalModelWrapper() : base() { }

        private ModelEditorForm _form;

        public override async void EditResource()
        {
            if (_form == null || _form.Disposing || _form.IsDisposed)
            {
                var modelTask = ResourceRef.GetInstanceAsync();

                _form = new ModelEditorForm();
                _form.FormClosed += _form_FormClosed;
                _form.Show();

                var model = await modelTask;
                _form.SetModel(model);
            }
            else
                _form.Focus();
        }
        private void _form_FormClosed(object sender, FormClosedEventArgs e)
        {
            _form.FormClosed -= _form_FormClosed;
            _form = null;
        }
    }
}