using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEngine.Actors.Types.Pawns;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.GenericFile))]
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