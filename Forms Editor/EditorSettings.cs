﻿using System;
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
        [TSerialize]
        public SingleFileRef<EngineSettings> Engine { get; set; }
        [TSerialize]
        public SingleFileRef<PropertyGridSettings> PropertyGrid { get; set; }
        [TSerialize]
        public SingleFileRef<ControlSettings> Controls { get; set; }

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
