using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models;
using TheraEditor.Properties;
using TheraEngine.Actors;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(IActor), nameof(Resources.GenericFile))]
    public class ActorWrapper : FileWrapper<IActor>
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
        
        public ActorWrapper() : base() { }

        public override async void EditResource()
        {
            ModelEditorForm d = new ModelEditorForm();
            d.Show();
            var actor = await ResourceRef.GetInstanceAsync();
            d.SetActor(actor);
        }
    }
}