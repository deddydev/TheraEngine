using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public interface IUITransformComponent : IUIComponent
    {
        Vec3 ScreenTranslation { get; }

        EventVec3 Translation { get; set; }
        EventVec3 Scale { get; set; }
    }
    public class UITransformComponent : UIComponent, IUITransformComponent
    {
        public UITransformComponent() : base() { }

        [TSerialize(nameof(Translation))]
        protected EventVec3 _translation = EventVec3.Zero;
        [TSerialize(nameof(Scale))]
        protected EventVec3 _scale = EventVec3.One;
        [TSerialize(nameof(Order))]
        protected ETransformOrder _order = ETransformOrder.TRS;

        protected EventVec2 _actualTranslation = new EventVec2();
        [Browsable(false)]
        public Vec2 ActualTranslation
        {
            get => _actualTranslation.Raw;
            set => _actualTranslation.Raw = value;
        }

        [Browsable(false)]
        [Category("Transform")]
        public Vec3 ScreenTranslation
        {
            get => Vec3.TransformPosition(WorldPoint, ActorRelativeMatrix);
            set => Translation.Xyz = Vec3.TransformPosition(value, InverseActorRelativeMatrix);
        }

        [Category("Transform")]
        public ETransformOrder Order
        {
            get => _order;
            set
            {
                if (Set(ref _order, value))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual EventVec3 Translation
        {
            get => _translation;
            set
            {
                if (Set(ref _translation, value,
                    () => _translation.Changed -= InvalidateLayout,
                    () => _translation.Changed += InvalidateLayout,
                    false))
                    InvalidateLayout();
            }
        }
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

        protected override void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            ActualTranslation = Translation.Xy;
            base.OnResizeLayout(parentRegion);
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Vec3 translation = new Vec3(ActualTranslation, Translation.Z);

            localTransform = Matrix4.TransformMatrix(
                Scale.Raw,
                Matrix4.Identity,
                translation,
                Order);

            inverseLocalTransform = Matrix4.TransformMatrix(
                1.0f / Scale.Raw, 
                Matrix4.Identity,
                -translation, 
                Transform.OppositeOrder(Order));
        }

        /// <summary>
        /// Scale and translate in/out to/from a specific point.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="worldScreenPoint"></param>
        /// <param name="minScale"></param>
        /// <param name="maxScale"></param>
        public void Zoom(float amount, Vec2 worldScreenPoint, Vec2? minScale, Vec2? maxScale)
        {
            if (Math.Abs(amount) < 0.0001f)
                return;

            Vec2 scale = _scale.Raw.Xy;
            Vec2 multiplier = Vec2.One / scale * amount;
            Vec2 newScale = scale - amount;

            if (minScale != null)
            {
                if (newScale.X < minScale.Value.X)
                    newScale.X = minScale.Value.X;

                if (newScale.Y < minScale.Value.Y)
                    newScale.Y = minScale.Value.Y;
            }

            if (maxScale != null)
            {
                if (newScale.X > maxScale.Value.X)
                    newScale.X = maxScale.Value.X;

                if (newScale.Y > maxScale.Value.Y)
                    newScale.Y = maxScale.Value.Y;
            }

            if (scale.DistanceTo(newScale) < 0.0001f)
                return;

            Vec2 newTranslation = ActualTranslation + (worldScreenPoint - WorldPoint.Xy) * multiplier;

            _translation.SetRawNoUpdate(new Vec3(newTranslation, _translation.Z));
            _scale.SetRawNoUpdate(new Vec3(newScale, _scale.Z));

            InvalidateLayout();

            RecalcLocalTransform();
        }
    }
}
