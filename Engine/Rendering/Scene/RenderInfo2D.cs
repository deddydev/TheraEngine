using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering
{
    public interface IRenderInfo2D : IRenderInfo
    {
        int LayerIndex { get; set; }
        int IndexWithinLayer { get; set; }

        BoundingRectangleF AxisAlignedRegion { get; set; }
        IQuadtreeNode QuadtreeNode { get; set; }
        I2DRenderable Owner { get; set; }
        IScene2D Scene { get; set; }

        bool DeeperThan(IRenderInfo2D other);
        void LinkScene(I2DRenderable r2D, IScene2D scene, bool forceVisible = false);
        void UnlinkScene();
    }
    public class RenderInfo2D : RenderInfo, IRenderInfo2D
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
        public I2DRenderable Owner { get; set; }
        [Browsable(false)]
        public IScene2D Scene { get; set; }
        public bool IsAttachedToScene => Scene != null;
        
        /// <summary>
        /// The axis-aligned bounding box for this UI component.
        /// </summary>
        [Browsable(false)]
        public BoundingRectangleF AxisAlignedRegion { get; set; }

        [Browsable(false)]
        public IQuadtreeNode QuadtreeNode { get; set; }

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
                    Scene.RenderTree.Add(Owner);
                else
                    Scene.RenderTree.Remove(Owner);
            }
        }
        
        public RenderInfo2D(int layerIndex, int orderInLayer)
        {
            LayerIndex = layerIndex;
            IndexWithinLayer = orderInLayer;
        }

        public bool DeeperThan(IRenderInfo2D other)
        {
            if (other is null)
                return true;

            if (LayerIndex > other.LayerIndex)
                return true;
            else if (LayerIndex == other.LayerIndex && IndexWithinLayer > other.IndexWithinLayer)
                return true;
            
            return false;
        }

        public void LinkScene(I2DRenderable r2D, IScene2D scene, bool forceVisible = false)
        {
            if (r2D is null || scene is null)
                return;

            Scene = null;
            Visible = false;

            Scene = scene;
            Owner = r2D;

            bool visible = VisibleByDefault || forceVisible;
#if EDITOR
            if (VisibleInEditorOnly)
                visible = visible && Engine.EditorState.InEditMode;
#endif
            Visible = visible;

            //AxisAlignedRegion?.RenderInfo?.LinkScene(AxisAlignedRegion, scene);
        }

        public void UnlinkScene()
        {
            if (Owner is null || Scene is null)
                return;

            Scene.RenderTree.Remove(Owner);
            Scene = null;
            Owner = null;
        }
    }
}
