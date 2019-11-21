using TheraEditor.Windows.Forms;
using TheraEngine.Actors;

namespace TheraEditor.Wrappers
{
    [TreeFileType]
    public class ActorWrapper : FileWrapper<BaseActor>
    {
        public override async void Edit()
        {
            BaseActor actor = await FileRef.GetInstanceAsync();
            Editor.Instance.DisplayForm<ModelEditorForm>(actor);
        }
    }
}