using TheraEngine.Files;
using TheraEngine.Worlds;
using System.ComponentModel;
using TheraEngine.Actors;

namespace TheraEditor
{
    /// <summary>
    /// Stores all information on the state of the project within the editor.
    /// </summary>
    [FileExt("state")]
    [FileDef("Project State")]
    public class ProjectState : TFileObject
    {
        private World _currentWorld;
        private Map _currentMap;
        private Actor _currentActor;

        public World CurrentWorld
        {
            get => _currentWorld;
            set => _currentWorld = value;
        }
        public Map CurrentMap
        {
            get => _currentMap;
            set => _currentMap = value;
        }
        public Actor CurrentActor
        {
            get => _currentActor;
            set => _currentActor = value;
        }
    }
}