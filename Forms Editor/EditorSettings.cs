using System.ComponentModel;
using TheraEngine;
using TheraEngine.Files;

namespace TheraEditor
{
    [FileDef("Editor Settings")]
    public class EditorSettings : TSettings
    {
        [TSerialize]
        public GlobalFileRef<EngineSettings> Engine { get; set; }
        [TSerialize]
        public GlobalFileRef<PropertyGridSettings> PropertyGrid { get; set; }
        [TSerialize]
        public GlobalFileRef<ControlSettings> Controls { get; set; }
        
        [FileDef("Property Grid Settings")]
        public class PropertyGridSettings : TSettings
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

        [FileDef("Control Settings")]
        public class ControlSettings : TSettings
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
