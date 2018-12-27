using System;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection.Attributes.Serialization;
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
        public ProjectState()
        {
            TimeCreated = DateTime.Now;
        }

        [TSerialize]
        public DateTime TimeCreated { get; internal set; }
        [TSerialize]
        public TimeSpan TotalTimeSpentWorking { get; private set; }

        public DateTime TimeOpened { get; private set; }
        public TimeSpan SessionElapsedTime => DateTime.Now - TimeOpened;

        [TPreDeserialize]
        private void PreDeserialize()
        {
            TimeOpened = DateTime.Now;
        }
        [TPreSerialize]
        private void PreSerialize()
        {
            TotalTimeSpentWorking += SessionElapsedTime;
        }

        private TWorld _currentWorld;

        public TWorld CurrentWorld
        {
            get => _currentWorld;
            set => _currentWorld = value;
        }
    }
}