using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.GenericFile), nameof(Resources.GenericFile))]
    public class SkeletalModelWrapper : FileWrapper<SkeletalModel>
    {
        public SkeletalModelWrapper() : base() { }

        private ModelEditorForm _form;

        public override async void Edit()
        {
            if (_form is null || _form.Disposing || _form.IsDisposed)
            {
                var loadTask = FileRef.GetInstanceAsync();

                _form = new ModelEditorForm();
                _form.CloseInvoked += _form_CloseInvoked;
                _form.Show();

                var file = await loadTask;
                _form.SetModel(file);
            }
            else
                _form.Focus();
        }

        private void _form_CloseInvoked()
        {
            _form.CloseInvoked -= _form_CloseInvoked;
            _form = null;
        }
    }
}