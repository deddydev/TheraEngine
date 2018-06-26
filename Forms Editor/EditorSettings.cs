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
        private GlobalFileRef<EngineSettings> _engineSettingsRef = new GlobalFileRef<EngineSettings>();
        private GlobalFileRef<PropertyGridSettings> _propertyGridRef = new GlobalFileRef<PropertyGridSettings>();
        private GlobalFileRef<ControlSettings> _controlSettingsRef = new GlobalFileRef<ControlSettings>();
        
        public EngineSettings EngineSettings
        {
            get => _engineSettingsRef.File;
            set => _engineSettingsRef.File = value;
        }
        public PropertyGridSettings PropertyGrid
        {
            get => _propertyGridRef.File;
            set => _propertyGridRef.File = value;
        }
        public ControlSettings Controls
        {
            get => _controlSettingsRef.File;
            set => _controlSettingsRef.File = value;
        }

        [TSerialize]
        public GlobalFileRef<EngineSettings> EngineSettingsRef
        {
            get => _engineSettingsRef;
            set => _engineSettingsRef = value ?? new GlobalFileRef<EngineSettings>();
        }
        [TSerialize]
        public GlobalFileRef<PropertyGridSettings> PropertyGridRef
        {
            get => _propertyGridRef;
            set => _propertyGridRef = value ?? new GlobalFileRef<PropertyGridSettings>();
        }
        [TSerialize]
        public GlobalFileRef<ControlSettings> ControlSettingsRef
        {
            get => _controlSettingsRef;
            set => _controlSettingsRef = value ?? new GlobalFileRef<ControlSettings>();
        }

        [TSerialize]
        public string DockConfigPath { get; set; }

        [TSerialize("RecentlyOpenedProjectPaths")]
        private List<string> _recentlyOpenedProjectPaths = new List<string>();
        public List<string> RecentlyOpenedProjectPaths => _recentlyOpenedProjectPaths;

        [FileDef("Property Grid Settings")]
        public class PropertyGridSettings : TSettings
        {
            private List<string> _collapsedCategories = new List<string>();
            private List<string> _expandedCategories = new List<string>();

            [TSerialize]
            public bool SplitCamelCase { get; set; }
            [TSerialize]
            public float UpdateRateInSeconds { get; set; }
            [TSerialize]
            public bool IgnoreLoneSubCategories { get; set; }
            [TSerialize]
            public List<string> CollapsedCategories
            {
                get => _collapsedCategories;
                set => _collapsedCategories = value ?? new List<string>();
            }
            [TSerialize]
            public List<string> ExpandedCategories
            {
                get => _expandedCategories;
                set => _expandedCategories = value ?? new List<string>();
            }

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
            DockConfigPath = Path.DirectorySeparatorChar + "DockPanel.config";
            EngineSettingsRef = new EngineSettings()
            {
                CapFPS = true,
                TargetFPS = 60.0f,
                CapUPS = false,
                TargetUPS = 90.0f,
            };
            PropertyGridRef = new PropertyGridSettings();
            ControlSettingsRef = new ControlSettings();
        }

        public string GetFullDockConfigPath()
        {
            if (!string.IsNullOrWhiteSpace(DockConfigPath) &&
                DockConfigPath[0] == Path.DirectorySeparatorChar)
            {
                if (!string.IsNullOrWhiteSpace(FilePath))
                    return Path.Combine(Path.GetDirectoryName(FilePath), DockConfigPath);
            }
            return DockConfigPath;
        }
    }
}
