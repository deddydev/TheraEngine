using TheraEngine.Files;
using TheraEngine.Worlds;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices;
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
        
        private SingleFileRef<World> _transitionWorld;
        private SingleFileRef<World> _openingWorld;
        private string _gamePath;

        [Category("Performance")]
        [Serialize]
        public ShadingStyle ShadingStyle { get => _shadingStyle; set => _shadingStyle = value; }
        [Category("Performance")]
        [Serialize]
        public bool SkinOnGPU { get => _skinOnGPU; set => _skinOnGPU = value; }
        [Category("Performance")]
        [Serialize]
        public bool UseIntegerWeightingIds { get => _useIntegerWeightingIds; set => _useIntegerWeightingIds = value; }
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

        [Category("Frames Per Second")]
        [Serialize("Capped", OverrideXmlCategory = "FramesPerSecond"/*, IsXmlAttribute = true*/)]
        public bool CapFPS { get => _capFPS; set => _capFPS = value; }
        [Category("Frames Per Second")]
        [Serialize("Target", OverrideXmlCategory = "FramesPerSecond", SerializeIf = "CapFPS")]
        public float TargetFPS { get => _targetFPS; set => _targetFPS = value; }

        [Category("Updates Per Second")]
        [Serialize("Capped", OverrideXmlCategory = "UpdatesPerSecond"/*, IsXmlAttribute = true*/)]
        public bool CapUPS { get => _capUPS; set => _capUPS = value; }
        [Category("Updates Per Second")]
        [Serialize("Target", OverrideXmlCategory = "UpdatesPerSecond", SerializeIf = "CapUPS")]
        public float TargetUPS { get => _targetUPS; set => _targetUPS = value; }

        [Category("Game")]
        [Serialize]
        public SingleFileRef<World> TransitionWorld { get => _transitionWorld; set => _transitionWorld = value; }
        [Category("Game")]
        [Serialize]
        public SingleFileRef<World> OpeningWorld { get => _openingWorld; set => _openingWorld = value; }
        [Category("Game")]
        [Serialize]
        public string GamePath { get => _gamePath; set => _gamePath = value; }

        public EngineSettings()
        {
            ShadingStyle = ShadingStyle.Deferred;
            SkinOnGPU = false;
            UseIntegerWeightingIds = false;
            RenderOctree = true;
            RenderQuadtree = true;
            RenderSkeletons = false;
            RenderCameraFrustums = false;
            RenderSplines = true;
            RenderCullingVolumes = false;
            CapFPS = false;
            TargetFPS = 60.0f;
            CapUPS = false;
            TargetUPS = 30.0f;
            GamePath = Engine.StartupPath;
            OpeningWorld = new SingleFileRef<World>("OpeningWorld.xworld");
            TransitionWorld = new SingleFileRef<World>("TransitionWorld.xworld");
        }
    }
}
