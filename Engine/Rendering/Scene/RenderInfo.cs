using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering
{
    public abstract class RenderInfo
    {
        public virtual bool Visible { get; set; } = true;
#if EDITOR
        public virtual bool VisibleInEditorOnly { get; set; } = false;
#endif
    }
    public class RenderInfo2D : RenderInfo
    {
        /// <summary>
        /// Used by the engine for proper order of rendering.
        /// </summary> 
        public ERenderPass RenderPass;
        /// <summary>
        /// Used to render objects in the same pass in a certain order.
        /// Smaller value means rendered sooner, zero (exactly) means it doesn't matter.
        /// </summary>
        public int LayerIndex;
        public int IndexWithinLayer;

        public RenderInfo2D(ERenderPass pass, int layerIndex, int orderInLayer)
        {
            RenderPass = pass;
            LayerIndex = layerIndex;
            IndexWithinLayer = orderInLayer;
        }

        public DateTime LastRenderedTime { get; internal set; }

        public TimeSpan GetTimeSinceLastRender() => DateTime.Now - LastRenderedTime;

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
    }
    public delegate float DelGetSortOrder(bool shadowPass);
    public class RenderInfo3D : RenderInfo
    {
        public bool HiddenFromOwner { get; set; } = false;
        public bool VisibleToOwnerOnly { get; set; } = false;

        public override bool Visible
        {
            get => base.Visible;
            set
            {
                if (Visible == value)
                    return;
                base.Visible = value;
                if (Scene != null)
                {
                    if (Visible)
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
        public bool ReceivesShadows { get; set; } = true;
        [TSerialize]
        public bool CastsShadows { get; set; } = true;
        [TSerialize]
        public ERenderPass RenderPass { get; set; } = ERenderPass.OpaqueDeferredLit;

        [Browsable(false)]
        public DateTime LastRenderedTime { get; internal set; }

        [Browsable(false)]
        public int SceneID { get; internal set; } = -1;
        [Browsable(false)]
        public Scene3D Scene { get; internal set; }
        public I3DRenderable Owner { get; internal set; }

        public TimeSpan GetTimeSinceLastRender() => DateTime.Now - LastRenderedTime;

        //public DelGetSortOrder RenderOrderFunc;

        public RenderInfo3D()
        {

        }
        public RenderInfo3D(ERenderPass pass, bool castsShadows = true, bool receivesShadows = true)
        {
            RenderPass = pass;
            //RenderOrderFunc = renderOrderFunc;
            CastsShadows = castsShadows;
            ReceivesShadows = receivesShadows;
        }

        //internal float GetRenderOrder(bool shadowPass)
        //{
        //    return RenderOrderFunc == null ? 0.0f : RenderOrderFunc(shadowPass);
        //}

        public override int GetHashCode() => SceneID;
    }
}
