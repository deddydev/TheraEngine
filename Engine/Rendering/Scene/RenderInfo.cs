using System;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering
{
    public abstract class RenderInfo : TFileObject
    {
        [TSerialize]
        public virtual bool VisibleByDefault { get; set; } = true;
        [TSerialize(State = true, Config = false)]
        public virtual bool Visible { get; set; } = true;
#if EDITOR
        [TSerialize]
        public virtual bool VisibleInEditorOnly { get; set; } = false;
        public EEditorVisibility EditorVisibilityMode { get; set; } = EEditorVisibility.Unchanged;
        public enum EEditorVisibility
        {
            Unchanged,
            VisibleAlways,
            VisibleOnlyWhenSelected,
        }
#endif
        [Browsable(false)]
        public DateTime LastRenderedTime { get; internal set; }
        public TimeSpan GetTimeSinceLastRender() => DateTime.Now - LastRenderedTime;
    }
    public class RenderInfo2D : RenderInfo
    {
        /// <summary>
        /// Used to render objects in the same pass in a certain order.
        /// Smaller value means rendered sooner, zero (exactly) means it doesn't matter.
        /// </summary>
        [TSerialize]
        public int LayerIndex { get; set; }
        [TSerialize]
        public int IndexWithinLayer { get; set; }
        [Browsable(false)]
        public I2DRenderable Owner { get; internal set; }
        [Browsable(false)]
        public Scene2D Scene { get; internal set; }
        /// <summary>
        /// The axis-aligned bounding box for this UI component.
        /// </summary>
        [Browsable(false)]
        public BoundingRectangleF AxisAlignedRegion;
        [Browsable(false)]
        public IQuadtreeNode QuadtreeNode { get; set; }

        public RenderInfo2D(int layerIndex, int orderInLayer)
        {
            LayerIndex = layerIndex;
            IndexWithinLayer = orderInLayer;
        }

        public bool DeeperThan(RenderInfo2D other)
        {
            if (other == null)
                return true;

            if (LayerIndex > other.LayerIndex)
                return true;
            else if (LayerIndex == other.LayerIndex && IndexWithinLayer > other.IndexWithinLayer)
                return true;
            
            return false;
        }

        public void LinkScene(I2DRenderable r2D, Scene2D scene, bool forceVisible = false)
        {
            if (r2D == null || scene == null)
                return;

            Scene = scene;
            Owner = r2D;

            Visible = r2D.RenderInfo.VisibleByDefault || forceVisible;
#if EDITOR
            if (VisibleInEditorOnly)
                Visible = Visible && Engine.EditorState.InEditMode;
#endif
            if (Visible)
                Scene.Add(Owner);
        }

        public void UnlinkScene()
        {
            if (Owner == null || Scene == null)
                return;

            Scene.Remove(Owner);
            Scene = null;
            Owner = null;
        }
    }
    public delegate float DelGetSortOrder(bool shadowPass);
    public sealed class RenderInfo3D : RenderInfo
    {
        [TSerialize]
        public bool HiddenFromOwner { get; set; } = false;
        [TSerialize]
        public bool VisibleToOwnerOnly { get; set; } = false;
        /// <summary>
        /// The shape the rendering octree will use to determine occlusion and offscreen culling (visibility).
        /// </summary>
        [TSerialize]
        public TShape CullingVolume { get; set; }
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
                if (Visible == value)
                    return;

                base.Visible = value;

                if (Scene == null)
                    return;

                if (value)
                    Scene.Add(Owner);
                else
                    Scene.Remove(Owner);
            }
        }

        /// <summary>
        /// Used to render objects in the same pass in a certain order.
        /// Smaller value means rendered sooner, zero (exactly) means it doesn't matter.
        /// </summary>
        //[Browsable(false)]
        //public float RenderOrder => RenderOrderFunc == null ? 0.0f : RenderOrderFunc();
        [TSerialize]
        public bool ReceivesShadows { get; set; } = false;
        [TSerialize]
        public bool CastsShadows { get; set; } = false;
        [TSerialize]
        public bool VisibleInIBLCapture { get; set; } = true;
        
        [Browsable(false)]
        public int SceneID { get; internal set; } = -1;
        [Browsable(false)]
        public Scene3D Scene { get; internal set; }
        [Browsable(false)]
        public I3DRenderable Owner { get; internal set; }

        public RenderInfo3D() { }
        public RenderInfo3D(bool visibleByDefault = true, bool visibleInEditorOnly = false)
        {
            VisibleByDefault = visibleByDefault;
            VisibleInEditorOnly = visibleInEditorOnly;
        }
        
        public override int GetHashCode() => SceneID;

        public void LinkScene(I3DRenderable r3d, Scene3D scene, bool forceVisible = false)
        {
            if (r3d == null || scene == null)
                return;

            Scene = scene;
            Owner = r3d;

            Visible = VisibleByDefault || forceVisible;
#if EDITOR
            if (VisibleInEditorOnly)
                Visible = Visible && Engine.EditorState.InEditMode;
#endif
            if (Visible)
                Scene.Add(Owner);

            CullingVolume?.RenderInfo?.LinkScene(CullingVolume, scene);
        }

        public void UnlinkScene()
        {
            if (Owner == null || Scene == null)
                return;

            Scene.Remove(Owner);
            Scene = null;
            Owner = null;

            CullingVolume?.RenderInfo?.UnlinkScene();
        }
    }
}
