using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TheraEngine;
using TheraEngine.Files;

namespace TheraEditor
{
    [FileDef("Editor Settings")]
    public class EditorSettings : TSettings
    {
        [TSerialize]
        public GlobalFileRef<EngineSettings> EngineDefaults { get; set; }
        [TSerialize]
        public GlobalFileRef<PropertyGridSettings> PropertyGrid { get; set; }
        [TSerialize]
        public GlobalFileRef<ControlSettings> Controls { get; set; }
        [TSerialize]
        public string DockConfigPath { get; set; }

        [TSerialize("RecentlyOpenedProjectPaths")]
        private List<string> _recentlyOpenedProjectPaths = new List<string>();
        public List<string> RecentlyOpenedProjectPaths => _recentlyOpenedProjectPaths;

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
            EngineDefaults = new EngineSettings()
            {
                CapFPS = true,
                TargetFPS = 60.0f,
                CapUPS = false,
                TargetUPS = 90.0f,
            };
            PropertyGrid = new PropertyGridSettings();
            Controls = new ControlSettings();
        }

        public string GetFullDockConfigPath()
        {
            if (!string.IsNullOrWhiteSpace(DockConfigPath) &&
                DockConfigPath[0] == '\\' &&
                !string.IsNullOrWhiteSpace(FilePath))
                return Path.Combine(Path.GetDirectoryName(FilePath), DockConfigPath);
            return DockConfigPath;
        }
    }
}
