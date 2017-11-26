using TheraEngine.Files;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace TheraEngine
{
    public enum ShadingStyle
    {
        Forward = 0,
        Deferred = 1,
    }
    [FileClass("ENSET", "Engine Settings")]
    public class EngineSettings : FileObject
    {
        [Category("Performance")]
        [TSerialize]
        public ShadingStyle ShadingStyle3D { get; set; }
        [Category("Performance")]
        [TSerialize]
        public bool SkinOnGPU { get; set; }
        [Category("Performance")]
        [TSerialize]
        public bool UseIntegerWeightingIds { get; set; }
        [Category("Performance")]
        [TSerialize]
        public bool AllowShaderPipelines { get; set; }

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

        /// <summary>
        /// Determines if the render rate should be capped at a specific frequency. If not, will run as fast as possible (though there is no point going any faster than the monitor can update).
        /// </summary>
        [Description("Determines if the render rate should be capped at a specific frequency. If not, will run as fast as possible (though there is no point going any faster than the monitor can update).")]
        [Category("Frames Per Second")]
        [DisplayName("Capped")]
        [TSerialize("Capped", OverrideXmlCategory = "FramesPerSecond"/*, XmlNodeType = EXmlNodeType.Attribute*/)]
        public bool CapFPS { get; set; }
        /// <summary>
        /// How many frames are expected to be rendered per second.
        /// </summary>
        [Description("How many frames are expected to be rendered per second.")]
        [Category("Frames Per Second")]
        [DisplayName("Target")]
        [TSerialize("Target", OverrideXmlCategory = "FramesPerSecond", Condition = "CapFPS")]
        public float TargetFPS { get; set; }

        /// <summary>
        /// Determines if the update rate should be capped at a specific frequency. If not, will run as fast as possible.
        /// </summary>
        [Description("Determines if the update rate should be capped at a specific frequency. If not, will run as fast as possible.")]
        [Category("Updates Per Second")]
        [DisplayName("Capped")]
        [TSerialize("Capped", OverrideXmlCategory = "UpdatesPerSecond"/*, XmlNodeType = EXmlNodeType.Attribute*/)]
        public bool CapUPS { get; set; }
        /// <summary>
        /// How many internal engine tick update calls are expected to be made per second. This is not the same as the render frequency.
        /// </summary>
        [Description("How many internal engine tick update calls are made per second. This is not the same as the render frequency.")]
        [Category("Updates Per Second")]
        [DisplayName("Target")]
        [TSerialize("Target", OverrideXmlCategory = "UpdatesPerSecond", Condition = "CapUPS")]
        public float TargetUPS { get; set; }

        /// <summary>
        /// How many seconds the user has to hold a button for it to register as a hold event.
        /// </summary>
        [Description("How many seconds the user has to hold a button for it to register as a hold event.")]
        [Category("Input")]
        [TSerialize]
        public float HoldInputDelay { get; set; }
        /// <summary>
        /// How many seconds the user has between pressing the same button twice for it to register as a double click event.
        /// </summary>
        [Description("How many seconds the user has between pressing the same button twice for it to register as a double click event.")]
        [Category("Input")]
        [TSerialize]
        public float DoubleClickInputDelay { get; set; }

        /// <summary>
        /// The path to the folder containing premade engine shaders.
        /// </summary>
        [Description("The path to the folder containing premade engine shaders.")]
        [Category("Paths")]
        [TSerialize]
        public string ShadersFolder { get; set; }

        public EngineSettings()
        {
            ShadingStyle3D = ShadingStyle.Deferred;
            SkinOnGPU = false;
            UseIntegerWeightingIds = true;
            AllowShaderPipelines = true;
#if DEBUG
            RenderPhysicsWorld = false;
            RenderOctree = false;
            RenderQuadtree = true;
            RenderSkeletons = false;
            RenderCameraFrustums = false;
            RenderSplines = false;
            RenderCullingVolumes = false;
            RenderLights = false;
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

            ShadersFolder = Path.Combine(Application.StartupPath, "..\\..\\..\\Engine\\Shaders");
        }
    }
}
