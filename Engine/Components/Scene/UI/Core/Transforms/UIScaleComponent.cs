using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.UI
{
    public interface IUIScaleComponent : IUIComponent
    {
        EventVec3 Scale { get; set; }
    }
    public class UIScaleComponent : UIComponent, IUIScaleComponent
    {
        public UIScaleComponent() : base() { }

        [TSerialize(nameof(Scale))]
        protected EventVec3 _scale = EventVec3.One;

        [Category("Transform")]
        public virtual EventVec3 Scale
        {
            get => _scale;
            set
            {
                Set(ref _scale, value ?? new EventVec3(),
                    () => _scale.Changed -= RecalcLocalTransform,
                    () => _scale.Changed += RecalcLocalTransform,
                    false);

                RecalcLocalTransform();
            }
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Scale.AsScaleMatrix();
            inverseLocalTransform = Scale.AsInverseScaleMatrix();
        }
    }
}
