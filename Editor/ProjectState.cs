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
    [Serializable]
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

        public World CurrentWorld { get; set; }
    }
}