using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine
{
    public enum ShadingStyle
    {
        Forward = 0,
        Deferred = 1,
    }
    [FileClass("ESET", "Engine Settings")]
    public class EngineSettings : FileObject
    {
        [Category("Performance")]
        [Serialize]
        public ShadingStyle ShadingStyle { get; set; }
        [Category("Performance")]
        [Serialize]
        public bool SkinOnGPU { get; set; }
        [Category("Performance")]
        [Serialize]
        public bool UseIntegerWeightingIds { get; set; }
        [Category("Performance")]
        [Serialize]
        public bool AllowShaderPipelines { get; set; }

        [Category("Debug")]
        [Serialize]
        public bool RenderCameraFrustums { get; set; }
        [Category("Debug")]
        [Serialize]
        public bool RenderSkeletons { get; set; }
        [Category("Debug")]
        [Serialize]
        public bool RenderQuadtree { get; set; }
        [Category("Debug")]
        [Serialize]
        public bool RenderOctree { get; set; }
        [Category("Debug")]
        [Serialize]
        public bool RenderSplines { get; set; }
        [Category("Debug")]
        [Serialize]
        public bool RenderCullingVolumes { get; set; }
        [Category("Debug")]
        [Serialize]
        public bool RenderLights { get; set; }

        /// <summary>
        /// Determines if the render rate should be capped at a specific frequency. If not, will run as fast as possible (though there is no point going any faster than the monitor can update).
        /// </summary>
        [Description("Determines if the render rate should be capped at a specific frequency. If not, will run as fast as possible (though there is no point going any faster than the monitor can update).")]
        [Category("Frames Per Second")]
        [Serialize("Capped", OverrideXmlCategory = "FramesPerSecond"/*, IsXmlAttribute = true*/)]
        public bool CapFPS { get; set; }
        /// <summary>
        /// How many frames are expected to be rendered per second.
        /// </summary>
        [Description("How many frames are expected to be rendered per second.")]
        [Category("Frames Per Second")]
        [Serialize("Target", OverrideXmlCategory = "FramesPerSecond", SerializeIf = "CapFPS")]
        public float TargetFPS { get; set; }

        /// <summary>
        /// Determines if the update rate should be capped at a specific frequency. If not, will run as fast as possible.
        /// </summary>
        [Description("Determines if the update rate should be capped at a specific frequency. If not, will run as fast as possible.")]
        [Category("Updates Per Second")]
        [Serialize("Capped", OverrideXmlCategory = "UpdatesPerSecond"/*, IsXmlAttribute = true*/)]
        public bool CapUPS { get; set; }
        /// <summary>
        /// How many internal engine tick update calls are expected to be made per second. This is not the same as the render frequency.
        /// </summary>
        [Description("How many internal engine tick update calls are made per second. This is not the same as the render frequency.")]
        [Category("Updates Per Second")]
        [Serialize("Target", OverrideXmlCategory = "UpdatesPerSecond", SerializeIf = "CapUPS")]
        public float TargetUPS { get; set; }

        /// <summary>
        /// How many seconds the user has to hold a button for it to register as a hold event.
        /// </summary>
        [Description("How many seconds the user has to hold a button for it to register as a hold event.")]
        [Category("Input")]
        [Serialize]
        public float HoldInputDelay { get; set; }
        /// <summary>
        /// How many seconds the user has between pressing the same button twice for it to register as a double click event.
        /// </summary>
        [Description("How many seconds the user has between pressing the same button twice for it to register as a double click event.")]
        [Category("Input")]
        [Serialize]
        public float DoubleClickInputDelay { get; set; }

        public EngineSettings()
        {
            ShadingStyle = ShadingStyle.Deferred;
            SkinOnGPU = false;
            UseIntegerWeightingIds = true;
            AllowShaderPipelines = true;
#if DEBUG
            RenderOctree = false;
            RenderQuadtree = true;
            RenderSkeletons = false;
            RenderCameraFrustums = false;
            RenderSplines = true;
            RenderCullingVolumes = false;
            RenderLights = true;
#else
            RenderOctree = false;
            RenderQuadtree = false;
            RenderSkeletons = false;
            RenderCameraFrustums = false;
            RenderSplines = false;
            RenderCullingVolumes = false;
#endif
            CapFPS = false;
            TargetFPS = 60.0f;
            CapUPS = false;
            TargetUPS = 30.0f;
        }
    }
}
