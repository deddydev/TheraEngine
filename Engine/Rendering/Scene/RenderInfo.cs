using System;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering
{
    public abstract class RenderInfo : TFileObject
    {
        /// <summary>
        /// Used by the engine for proper order of rendering.
        /// </summary> 
        [TSerialize]
        public ERenderPass RenderPass { get; set; } = ERenderPass.OpaqueForward;
        [TSerialize]
        public virtual bool VisibleByDefault { get; set; } = true;
        [TSerialize]
        public virtual bool Visible { get; set; } = true;
#if EDITOR
        [TSerialize]
        public virtual bool VisibleInEditorOnly { get; set; } = false;
#endif

        public DateTime LastRenderedTime { get; internal set; }
        public TimeSpan GetTimeSinceLastRender() => DateTime.Now - LastRenderedTime;

        internal void EnforceVisibility(bool visible) => Visible = visible;
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

        public RenderInfo2D(ERenderPass pass, int layerIndex, int orderInLayer)
        {
            RenderPass = pass;
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

        public void LinkScene(I2DRenderable r2d, Scene2D scene, bool forceVisible = false)
        {
            if (r2d?.RenderInfo == null || scene == null)
                return;

            r2d.RenderInfo.Scene = scene;
            r2d.RenderInfo.Owner = r2d;

            bool spawn = r2d.RenderInfo.VisibleByDefault || forceVisible;
#if EDITOR
            if (r2d.RenderInfo.VisibleInEditorOnly)
                spawn = spawn && Engine.EditorState.InEditMode;
#endif
            if (spawn)
                scene.Add(r2d);
        }

        public void UnlinkScene(I2DRenderable r2d, Scene2D scene)
        {
            if (r2d?.RenderInfo == null || scene == null || !r2d.RenderInfo.Visible)
                return;

            scene.Remove(r2d);
            r2d.RenderInfo.Scene = null;
            r2d.RenderInfo.Owner = null;
        }
    }
    public delegate float DelGetSortOrder(bool shadowPass);
    public class RenderInfo3D : RenderInfo
    {
        [TSerialize]
        public bool HiddenFromOwner { get; set; } = false;
        [TSerialize]
        public bool VisibleToOwnerOnly { get; set; } = false;
        [TSerialize(State = true, Config = false)]
        public override bool Visible
        {
            get => Scene != null && base.Visible;
            set
            {
                if (Visible == value)
                    return;
                base.Visible = value;
                if (Scene != null)
                {
                    if (value)
                        Scene.Add(Owner);
                    else
                        Scene.Remove(Owner);
                }
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

        [Browsable(false)]
        public int SceneID { get; internal set; } = -1;
        [Browsable(false)]
        public Scene3D Scene { get; internal set; }
        [Browsable(false)]
        public I3DRenderable Owner { get; internal set; }

        public RenderInfo3D()
        {
            RenderPass = ERenderPass.OpaqueDeferredLit;
        }
        public RenderInfo3D(ERenderPass pass, bool visibleByDefault = true, bool visibleInEditorOnly = false)
        {
            RenderPass = pass;
            VisibleByDefault = visibleByDefault;
            VisibleInEditorOnly = visibleInEditorOnly;
        }
        
        public override int GetHashCode() => SceneID;

        public void LinkScene(I3DRenderable r3d, Scene3D scene, bool forceVisible = false)
        {
            if (r3d?.RenderInfo == null || scene == null)
                return;

            r3d.RenderInfo.Scene = scene;
            r3d.RenderInfo.Owner = r3d;

            bool spawn = r3d.RenderInfo.VisibleByDefault || forceVisible;
#if EDITOR
            if (r3d.RenderInfo.VisibleInEditorOnly)
                spawn = spawn && Engine.EditorState.InEditMode;
#endif
            if (spawn)
                scene.Add(r3d);
        }

        public void UnlinkScene(I3DRenderable r3d, Scene3D scene)
        {
            if (r3d?.RenderInfo == null || scene == null || !r3d.RenderInfo.Visible)
                return;

            scene.Remove(r3d);
            r3d.RenderInfo.Scene = null;
            r3d.RenderInfo.Owner = null;
        }
    }
}
