using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public interface IRenderInfo3D : IRenderInfo
    {
        event DelCullingVolumeChanged CullingVolumeChanged;

        bool HiddenFromOwner { get; set; }
        bool VisibleToOwnerOnly { get; set; }
        TShape CullingVolume { get; set; }
        IOctreeNode OctreeNode { get; set; }
        bool ReceivesShadows { get; set; }
        bool CastsShadows { get; set; }
        bool VisibleInIBLCapture { get; set; }
        IScene3D Scene { get; set; }
        I3DRenderable Owner { get; set; }

        //TODO: better link / unlink mechanism
        //preferably something like World.RegisterRenderableItem(IRenderable item) on component spawn
        void LinkScene(I3DRenderable r3d, IScene3D scene, bool forceVisible = false);
        void UnlinkScene();

        bool AllowRender(IVolume cullingVolume, RenderPasses passes, ICamera camera, bool shadowPass);
    }
    public class RenderInfo3D : RenderInfo, IRenderInfo3D
    {
        private TShape _cullingVolume;
        public event DelCullingVolumeChanged CullingVolumeChanged;

        [TSerialize]
        public bool HiddenFromOwner { get; set; } = false;
        [TSerialize]
        public bool VisibleToOwnerOnly { get; set; } = false;
        /// <summary>
        /// The shape the rendering octree will use to determine occlusion and offscreen culling (visibility).
        /// </summary>
        //[BrowsableIf("")]
        [TSerialize]
        public TShape CullingVolume
        {
            get => _cullingVolume;
            set
            {
                TShape old = _cullingVolume;
                _cullingVolume?.RenderInfo?.UnlinkScene();
                _cullingVolume = value;
                var r3D = _cullingVolume?.RenderInfo;
                if (r3D != null)
                {
                    r3D.CastsShadows = false;
                    r3D.LinkScene(Owner, Scene);
                }
                CullingVolumeChanged?.Invoke(old, _cullingVolume);
            }
        }
        /// <summary>
        /// The octree bounding box this object is currently located in.
        /// </summary>   
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        public override bool Visible
        {
            get => Scene != null && base.Visible;
            set
            {
                if (base.Visible == value)
                    return;

                base.Visible = value;

                if (Scene is null)
                    return;

                if (value)
                    Scene.Renderables.Add(Owner);
                else
                    Scene.Renderables.Remove(Owner);
            }
        }

        /// <summary>
        /// Used to render objects in the same pass in a certain order.
        /// Smaller value means rendered sooner, zero (exactly) means it doesn't matter.
        /// </summary>
        //[Browsable(false)]
        //public float RenderOrder => RenderOrderFunc is null ? 0.0f : RenderOrderFunc();
        [TSerialize]
        public bool ReceivesShadows { get; set; } = true;
        [TSerialize]
        public bool CastsShadows { get; set; } = true;
        [TSerialize]
        public bool VisibleInIBLCapture { get; set; } = true;
        
        [Browsable(false)]
        public IScene3D Scene { get; set; }
        [Browsable(false)]
        public I3DRenderable Owner { get; set; }

        public RenderInfo3D() { }
        public RenderInfo3D(bool visibleByDefault = true, bool visibleInEditorOnly = false)
        {
            VisibleByDefault = visibleByDefault;
#if EDITOR
            VisibleInEditorOnly = visibleInEditorOnly;
#endif
        }
        
        public override int GetHashCode() => SceneID;

        public void LinkScene(I3DRenderable r3d, IScene3D scene, bool forceVisible = false)
        {
            if (r3d is null || scene is null)
                return;

            Scene = null;
            Visible = false;

            Scene = scene;
            Owner = r3d;

            bool visible = VisibleByDefault || forceVisible;
#if EDITOR
            if (VisibleInEditorOnly)
                visible = visible && Engine.EditorState.InEditMode;
#endif
            Visible = visible;

            CullingVolume?.RenderInfo?.LinkScene(CullingVolume, scene);
        }

        public void UnlinkScene()
        {
            if (Owner is null || Scene is null)
                return;

            Scene.Renderables.Remove(Owner);
            Scene = null;
            Owner = null;

            CullingVolume?.RenderInfo?.UnlinkScene();
        }

        public virtual bool AllowRender(
            IVolume cullingVolume,
            RenderPasses passes,
            ICamera camera,
            bool shadowPass)
        {
#if EDITOR
            if ((Owner is CameraComponent ccomp && ccomp.Camera == camera) ||
                (Owner is ICamera cam && cam == camera))
                return false;

            bool editMode = Engine.EditorState.InEditMode;
            if (!editMode && VisibleInEditorOnly)
                return false;
#endif
            return Visible &&
            (!shadowPass || CastsShadows) &&
            (CullingVolume == null || cullingVolume.Contains(CullingVolume) != EContainment.Disjoint);
        }
    }
}
