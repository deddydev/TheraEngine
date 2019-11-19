using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Actors;

namespace TheraEditor.Wrappers
{
    [TreeFileType]
    public class ActorWrapper : FileWrapper<BaseActor>
    {
        public override async void Edit()
        {
            ModelEditorForm d = new ModelEditorForm();
            d.Show();

            BaseActor actor = await FileRef.GetInstanceAsync();
            d.SetActor(actor);
        }
    }
}