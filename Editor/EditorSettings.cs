using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TheraEngine;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEditor
{
    [TFileDef("Editor Settings")]
    public class EditorSettings : TSettings
    {
        private GlobalFileRef<PropertyGridSettings> _propertyGridRef = new GlobalFileRef<PropertyGridSettings>();
        private GlobalFileRef<ControlSettings> _controlSettingsRef = new GlobalFileRef<ControlSettings>();
        
        [Browsable(false)]
        public PropertyGridSettings PropertyGrid
        {
            get => _propertyGridRef.File;
            set => _propertyGridRef.File = value;
        }
        [Browsable(false)]
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
        public PathReference DockConfigPath { get; set; }

        [TSerialize]
        [Browsable(false)]
        public List<string> RecentlyOpenedProjectPaths { get; set; }

        [Category("Debug")]
        [TSerialize]
        public bool RenderCameraFrustums { get; set; }
        [Category("Debug")]
        [TSerialize]
        public bool RenderSkeletons { get; set; }
        [Category("Debug")]
        [TSerialize]
        public bool RenderQuadtree { get; set; }
        [Category("Debug")]
        [TSerialize]
        public bool RenderOctree { get; set; }
        [Category("Debug")]
        [TSerialize]
        public bool RenderSplines { get; set; }
        [Category("Debug")]
        [TSerialize]
        public bool RenderCullingVolumes { get; set; }
        [Category("Debug")]
        [TSerialize]
        public bool RenderLights { get; set; }
        [Category("Debug")]
        [TSerialize]
        public bool RenderPhysicsWorld { get; set; }

        public enum EOverwrite
        {
            Ask,
            Always,
            Never,
        }
        [Category("Installation")]
        [TSerialize]
        public EOverwrite OverwriteCurrentInstall { get; set; } = EOverwrite.Ask;
        
        [Serializable]
        [TFileDef("Property Grid Settings")]
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
            public bool ShowTypeNames { get; set; } = true;
            [TSerialize]
            public bool DisplayMethods { get; set; } = false;
            [TSerialize]
            public bool DisplayEvents { get; set; } = false;

            public PropertyGridSettings()
            {
                SplitCamelCase = true;
                UpdateRateInSeconds = 0.2f;
                IgnoreLoneSubCategories = true;
            }
        }

        [Serializable]
        [TFileDef("Control Settings")]
        public class ControlSettings : TSettings
        {
            public ControlSettings()
            {

            }
        }

        public EditorSettings()
        {
            DockConfigPath = "DockPanel.config";
            PropertyGridRef = new PropertyGridSettings();
            ControlSettingsRef = new ControlSettings();
        }

        public string GetFullDockConfigPath()
        {
            string path = DockConfigPath?.Path;
            if (!string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(FilePath))
                return Path.Combine(Path.GetDirectoryName(FilePath), path);
            return path;
        }
    }
}
