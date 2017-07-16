using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine
{
    public enum ShadingStyle
    {
        Forward = 0,
        Deferred = 1,
        DeferredOpaqueForwardTransparent,
    }
    [FileClass("ESET", "Engine Settings")]
    public class EngineSettings : FileObject
    {
        private ShadingStyle _shadingStyle;
        private bool _skinOnGPU;
        private bool _useIntegerWeightingIds;
        private bool _allowShaderPipelines;
        
        private bool _renderCameraFrustums;
        private bool _renderSkeletons;
        private bool _renderQuadtree;
        private bool _renderOctree;
        private bool _renderSplines;
        private bool _renderCullingVolumes;
        
        private bool _capFPS;
        private float _targetFPS;
        
        private bool _capUPS;
        private float _targetUPS;

        private float _doubleClickInputDelay;
        private float _holdInputDelay;

        [Category("Performance")]
        [Serialize]
        public ShadingStyle ShadingStyle { get => _shadingStyle; set => _shadingStyle = value; }
        [Category("Performance")]
        [Serialize]
        public bool SkinOnGPU { get => _skinOnGPU; set => _skinOnGPU = value; }
        [Category("Performance")]
        [Serialize]
        public bool UseIntegerWeightingIds { get => _useIntegerWeightingIds; set => _useIntegerWeightingIds = value; }
        [Category("Performance")]
        [Serialize]
        public bool AllowShaderPipelines { get => _allowShaderPipelines; set => _allowShaderPipelines = value; }
        [Category("Debug")]
        [Serialize]
        public bool RenderCameraFrustums { get => _renderCameraFrustums; set => _renderCameraFrustums = value; }
        [Category("Debug")]
        [Serialize]
        public bool RenderSkeletons { get => _renderSkeletons; set => _renderSkeletons = value; }
        [Category("Debug")]
        [Serialize]
        public bool RenderQuadtree { get => _renderQuadtree; set => _renderQuadtree = value; }
        [Category("Debug")]
        [Serialize]
        public bool RenderOctree { get => _renderOctree; set => _renderOctree = value; }
        [Category("Debug")]
        [Serialize]
        public bool RenderSplines { get => _renderSplines; set => _renderSplines = value; }
        [Category("Debug")]
        [Serialize]
        public bool RenderCullingVolumes { get => _renderCullingVolumes; set => _renderCullingVolumes = value; }

        /// <summary>
        /// Determines if the render rate should be capped at a specific frequency. If not, will run as fast as possible (though there is no point going any faster than the monitor can update).
        /// </summary>
        [Description("Determines if the render rate should be capped at a specific frequency. If not, will run as fast as possible (though there is no point going any faster than the monitor can update).")]
        [Category("Frames Per Second")]
        [Serialize("Capped", OverrideXmlCategory = "FramesPerSecond"/*, IsXmlAttribute = true*/)]
        public bool CapFPS { get => _capFPS; set => _capFPS = value; }
        /// <summary>
        /// How many frames are expected to be rendered per second.
        /// </summary>
        [Description("How many frames are expected to be rendered per second.")]
        [Category("Frames Per Second")]
        [Serialize("Target", OverrideXmlCategory = "FramesPerSecond", SerializeIf = "CapFPS")]
        public float TargetFPS { get => _targetFPS; set => _targetFPS = value; }

        /// <summary>
        /// Determines if the update rate should be capped at a specific frequency. If not, will run as fast as possible.
        /// </summary>
        [Description("Determines if the update rate should be capped at a specific frequency. If not, will run as fast as possible.")]
        [Category("Updates Per Second")]
        [Serialize("Capped", OverrideXmlCategory = "UpdatesPerSecond"/*, IsXmlAttribute = true*/)]
        public bool CapUPS { get => _capUPS; set => _capUPS = value; }
        /// <summary>
        /// How many internal engine tick update calls are expected to be made per second. This is not the same as the render frequency.
        /// </summary>
        [Description("How many internal engine tick update calls are made per second. This is not the same as the render frequency.")]
        [Category("Updates Per Second")]
        [Serialize("Target", OverrideXmlCategory = "UpdatesPerSecond", SerializeIf = "CapUPS")]
        public float TargetUPS { get => _targetUPS; set => _targetUPS = value; }

        /// <summary>
        /// How many seconds the user has to hold a button for it to register as a hold event.
        /// </summary>
        [Description("How many seconds the user has to hold a button for it to register as a hold event.")]
        [Category("Input")]
        [Serialize]
        public float HoldInputDelay { get => _holdInputDelay; set => _holdInputDelay = value; }
        /// <summary>
        /// How many seconds the user has between pressing the same button twice for it to register as a double click event.
        /// </summary>
        [Description("How many seconds the user has between pressing the same button twice for it to register as a double click event.")]
        [Category("Input")]
        [Serialize]
        public float DoubleClickInputDelay { get => _doubleClickInputDelay; set => _doubleClickInputDelay = value; }
        public bool RenderLights { get; internal set; }

        public EngineSettings()
        {
            ShadingStyle = ShadingStyle.Deferred;
            SkinOnGPU = false;
            UseIntegerWeightingIds = false;
            AllowShaderPipelines = true;
#if DEBUG
            RenderOctree = false;
            RenderQuadtree = true;
            RenderSkeletons = false;
            RenderCameraFrustums = false;
            RenderSplines = true;
            RenderCullingVolumes = false;
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
            TargetUPS = 90.0f;
        }
    }
}
