using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public interface IUITranslationComponent : IUIComponent
    {
        Vec3 ScreenTranslation { get; }
        EventVec3 Translation { get; set; }
    }
    public class UITranslationComponent : UIComponent, IUITranslationComponent
    {
        public UITranslationComponent() : base()
        {
            Translation = EventVec3.Zero;
        }

        protected EventVec3 _translation;
        protected EventVec2 _actualTranslation = new EventVec2();

        //[Browsable(false)]
        [Category("Transform")]
        public EventVec2 ActualTranslation
        {
            get => _actualTranslation;
            set => Set(ref _actualTranslation, value ?? new EventVec2());
        }

        [Browsable(false)]
        [Category("Transform")]
        public Vec3 ScreenTranslation
        {
            get => Vec3.TransformPosition(WorldPoint, ActorRelativeMatrix);
            set => Translation.Xyz = Vec3.TransformPosition(value, InverseActorRelativeMatrix);
        }

        [TSerialize]
        [Category("Transform")]
        public virtual EventVec3 Translation
        {
            get => _translation;
            set
            {
                if (Set(ref _translation, value ?? new EventVec3(),
                    () => _translation.Changed -= InvalidateLayout,
                    () => _translation.Changed += InvalidateLayout,
                    false))
                    InvalidateLayout();
            }
        }
        protected virtual void OnResizeActual(BoundingRectangleF parentBounds)
        {
            ActualTranslation.Value = Translation.Xy;
        }
        protected override void OnResizeLayout(BoundingRectangleF parentBounds)
        {
            OnResizeActual(parentBounds);
            RecalcLocalTransform();
            var bounds = new BoundingRectangleF(ActualTranslation.Value, parentBounds.Extents);
            OnResizeChildComponents(bounds);
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.CreateTranslation(Translation);
            inverseLocalTransform = Matrix4.CreateTranslation(-Translation);
        }
    }
}
