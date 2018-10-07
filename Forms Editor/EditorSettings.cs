using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TheraEngine;
using TheraEngine.Core.Files;

namespace TheraEditor
{
    [FileDef("Editor Settings")]
    public class EditorSettings : TSettings
    {
        private GlobalFileRef<PropertyGridSettings> _propertyGridRef = new GlobalFileRef<PropertyGridSettings>();
        private GlobalFileRef<ControlSettings> _controlSettingsRef = new GlobalFileRef<ControlSettings>();
        
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

        [TSerialize(nameof(RecentlyOpenedProjectPaths))]
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
            [TSerialize]
            public bool ShowTypeNames { get; set; } = false;

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
