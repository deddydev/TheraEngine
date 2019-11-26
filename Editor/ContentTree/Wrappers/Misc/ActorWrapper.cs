using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Actors;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.GenericFile), nameof(Resources.GenericFile))]
    public class ActorWrapper : FileWrapper<BaseActor>
    {
        public ActorWrapper() : base() { }
        
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
                _form.SetActor(file);
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