using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Files;

namespace TheraEditor
{
    [FileClass("EDSET", "Editor Settings")]
    public class EditorSettings : FileObject
    {
        [Serialize]
        public EngineSettings Engine { get; set; } = new EngineSettings()
        {
            CapFPS = false,
            TargetFPS = 60.0f,
            CapUPS = false,
            TargetUPS = 30.0f,
        };
        public PropertyGridSettings PropertyGrid { get; set; } = new PropertyGridSettings()
        {

        };

        [FileClass("PGSET", "Property Grid Settings")]
        public class PropertyGridSettings : FileObject
        {
            public bool SplitCamelCase { get; set; } = true;
        }
    }
}
