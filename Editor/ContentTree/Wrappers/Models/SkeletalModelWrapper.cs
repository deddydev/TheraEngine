using System.ComponentModel;
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
                var modelTask = FileRef.GetInstanceAsync();

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