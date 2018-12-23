using System;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.Worlds;

namespace TheraEditor
{
    /// <summary>
    /// Stores all information on the state of the project within the editor.
    /// </summary>
    [TFileExt("state")]
    [TFileDef("Project State")]
    public class ProjectState : TFileObject
    {
        [TSerialize]
        public DateTime TimeCreated { get; internal set; }
        [TSerialize]
        public DateTime TimeSpentWorking { get; internal set; }
        
        private TWorld _currentWorld;
        private Map _currentMap;
        private TransformActor _currentActor;

        public TWorld CurrentWorld
        {
            get => _currentWorld;
            set => _currentWorld = value;
        }
        public Map CurrentMap
        {
            get => _currentMap;
            set => _currentMap = value;
        }
        public TransformActor CurrentActor
        {
            get => _currentActor;
            set => _currentActor = value;
        }
    }
}