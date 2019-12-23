using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Worlds;

namespace TheraEditor
{
    public class EditorWorldManager : WorldManager
    {
        public EditorWorldManager()
        {
            Engine.PreWorldChanged += PreWorldChanged;
            Engine.PostWorldChanged += PostWorldChanged;
        }

        private void PreWorldChanged()
        {
            World = null;
        }
        private void PostWorldChanged()
        {
            World = Engine.World;
        }
    }
}
