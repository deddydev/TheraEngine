using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Input.Devices;

namespace TheraEditor
{
    [FileClass("SET", "Editor Settings")]
    public class EditorSettings : FileObject
    {
        [Serialize]
        public SingleFileRef<EngineSettings> Engine { get; set; }
        [Serialize]
        public SingleFileRef<PropertyGridSettings> PropertyGrid { get; set; }
        [Serialize]
        public SingleFileRef<ControlSettings> Controls { get; set; }

        [FileClass("SET", "Property Grid Settings")]
        public class PropertyGridSettings : FileObject
        {
            [Serialize]
            public bool SplitCamelCase { get; set; }
            [Serialize]
            public float UpdateRateInSeconds { get; set; }
            [Serialize]
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
