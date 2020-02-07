using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public interface IUIScaleComponent : IUIComponent
    {
        EventVec3 Scale { get; set; }
    }
    public class UIScaleComponent : UIComponent, IUIScaleComponent
    {
        public UIScaleComponent() : base()
        {
            Scale = EventVec3.One;
        }

        protected EventVec3 _scale;

        [TSerialize]
        [Category("Transform")]
        public virtual EventVec3 Scale
        {
            get => _scale;
            set
            {
                if (Set(ref _scale, value,
                    () => _scale.Changed -= InvalidateLayout,
                    () => _scale.Changed += InvalidateLayout,
                    false))
                    InvalidateLayout();
            }
        }
        protected override void OnResizeLayout(BoundingRectangleF parentBounds)
        {
            RecalcLocalTransform();
            OnResizeChildComponents(parentBounds);
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Scale.AsScaleMatrix();
            inverseLocalTransform = Scale.AsInverseScaleMatrix();
        }
    }
}
