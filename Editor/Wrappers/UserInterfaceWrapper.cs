using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models;
using TheraEditor.Properties;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(IUserInterface), nameof(Resources.GenericFile))]
    public class UserInterfaceWrapper : FileWrapper<IUserInterface>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static UserInterfaceWrapper()
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
        
        public UserInterfaceWrapper() : base() { }
    }
}