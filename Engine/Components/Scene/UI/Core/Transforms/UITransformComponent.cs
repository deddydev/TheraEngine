using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

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
        private EventVec3 _translation = EventVec3.Zero;
        [TSerialize(nameof(Scale))]
        private EventVec3 _scale = EventVec3.One;
        [TSerialize(nameof(Order))]
        private ETransformOrder _order = ETransformOrder.TRS;

        [Browsable(false)]
        [Category("Transform")]
        public Vec3 ScreenTranslation
        {
            get => Vec3.TransformPosition(WorldPoint, ActorRelativeTransform);
            set => Translation.Xyz = Vec3.TransformPosition(value, InverseActorRelativeTransform);
        }

        [Category("Transform")]
        public ETransformOrder Order
        {
            get => _order;
            set
            {
                if (Set(ref _order, value))
                    RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public virtual EventVec3 Translation
        {
            get => _translation;
            set
            {
                if (Set(ref _translation, value,
                    () => _translation.Changed -= RecalcLocalTransform,
                    () => _translation.Changed += RecalcLocalTransform,
                    false))
                    RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public virtual EventVec3 Scale
        {
            get => _scale;
            set
            {
                if (Set(ref _scale, value,
                    () => _scale.Changed -= RecalcLocalTransform,
                    () => _scale.Changed += RecalcLocalTransform,
                    false))
                    RecalcLocalTransform();
            }
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.TransformMatrix(Scale.Raw, Matrix4.Identity, Translation, Order);
            inverseLocalTransform = Matrix4.TransformMatrix(1.0f / Scale.Raw, Matrix4.Identity, -Translation, Transform.OppositeOrder(Order));
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

            Vec2 newTranslation = _translation.Raw.Xy + (worldScreenPoint - WorldPoint.Xy) * multiplier;

            _translation.SetRawNoUpdate(new Vec3(newTranslation, _translation.Z));
            _scale.SetRawNoUpdate(new Vec3(newScale, _scale.Z));

            RecalcLocalTransform();
            ResizeLayout();
        }
    }
}
