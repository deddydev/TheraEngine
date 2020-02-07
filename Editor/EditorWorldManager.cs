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
            TargetWorld = null;
        }
        private void PostWorldChanged()
        {
            TargetWorld = Engine.World;
        }
    }
}
