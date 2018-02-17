using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering
{
    public class RenderInfo2D
    {
        /// <summary>
        /// Used by the engine for proper order of rendering.
        /// </summary> 
        public ERenderPass2D RenderPass;
        /// <summary>
        /// Used to render objects in the same pass in a certain order.
        /// Smaller value means rendered sooner, zero (exactly) means it doesn't matter.
        /// </summary>
        public int LayerIndex;
        public int OrderInLayer;

        public RenderInfo2D(ERenderPass2D pass, int layerIndex, int orderInLayer)
        {
            RenderPass = pass;
            LayerIndex = layerIndex;
            OrderInLayer = orderInLayer;
        }

        public DateTime LastRenderedTime { get; internal set; }

        public TimeSpan GetTimeSinceLastRender() => DateTime.Now - LastRenderedTime;
    }
    public delegate float DelGetSortOrder(bool shadowPass);
    public class RenderInfo3D
    {
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
        public ERenderPass3D RenderPass { get; set; } = ERenderPass3D.OpaqueDeferredLit;

        [Browsable(false)]
        public DateTime LastRenderedTime { get; internal set; }
        public TimeSpan GetTimeSinceLastRender() => DateTime.Now - LastRenderedTime;

        public DelGetSortOrder RenderOrderFunc;

        public RenderInfo3D()
        {

        }
        public RenderInfo3D(ERenderPass3D pass, DelGetSortOrder renderOrderFunc, bool castsShadows = true, bool receivesShadows = true)
        {
            RenderPass = pass;
            RenderOrderFunc = renderOrderFunc;
            CastsShadows = castsShadows;
            ReceivesShadows = receivesShadows;
        }

        internal float GetRenderOrder(bool shadowPass)
        {
            return RenderOrderFunc == null ? 0.0f : RenderOrderFunc(shadowPass);
        }
    }
}
