using System.ComponentModel;
using TheraEngine;
using TheraEngine.Files;

namespace TheraEditor
{
    [FileClass("SET", "Editor Settings")]
    public class EditorSettings : FileObject
    {
        [TSerialize]
        public GlobalFileRef<EngineSettings> Engine { get; set; }
        [TSerialize]
        public GlobalFileRef<PropertyGridSettings> PropertyGrid { get; set; }
        [TSerialize]
        public GlobalFileRef<ControlSettings> Controls { get; set; }

        [FileClass("SET", "Property Grid Settings")]
        public class PropertyGridSettings : FileObject
        {
            [TSerialize]
            public bool SplitCamelCase { get; set; }
            [TSerialize]
            public float UpdateRateInSeconds { get; set; }
            [TSerialize]
            public bool IgnoreLoneSubCategories { get; set; }

            public PropertyGridSettings()
            {
                SplitCamelCase = true;
                UpdateRateInSeconds = 0.2f;
                IgnoreLoneSubCategories = true;
            }
        }
        [FileClass("SET", "Control Settings")]
        public class ControlSettings : FileObject
        {
            public ControlSettings()
            {

            }
        }

        public EditorSettings()
        {
            Engine = new EngineSettings()
            {
                CapFPS = true,
                TargetFPS = 15.0f,
                CapUPS = false,
                TargetUPS = 30.0f,
            };
            PropertyGrid = new PropertyGridSettings();
            Controls = new ControlSettings();
        }
    }
}
