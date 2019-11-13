using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Actors;

namespace TheraEditor.Wrappers
{
    [TreeFileType]
    public class ActorWrapper : FileWrapper<BaseActor>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static ActorWrapper()
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
            ActorWrapper w = GetInstance<ActorWrapper>();
        }
        #endregion

        public override async void Edit()
        {
            ModelEditorForm d = new ModelEditorForm();
            d.Show();

            BaseActor actor = await ResourceRef.GetInstanceAsync();
            d.SetActor(actor);
        }
    }
}