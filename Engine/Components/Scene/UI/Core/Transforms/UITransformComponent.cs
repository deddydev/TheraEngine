using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
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
        public UITransformComponent() : base()
        {
            _translation = EventVec3.Zero;
            _translation.Changed += UpdateMatrix;

            _scale = EventVec3.One;
            _scale.Changed += UpdateMatrix;
        }

        protected EventVec3 _translation;
        protected EventVec3 _scale;
        protected EventVec2 _actualTranslation = new EventVec2();
        protected ETransformOrder _order = ETransformOrder.TRS;

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
            get => LocalToScreen(_worldMatrix.Translation);
            set => _worldMatrix.Translation = ScreenToLocal(value);
        }

        [TSerialize]
        [Category("Transform")]
        public ETransformOrder Order
        {
            get => _order;
            set
            {
                if (Set(ref _order, value))
                    UpdateMatrix();
            }
        }
        [TSerialize]
        [Category("Transform")]
        public virtual EventVec3 Translation
        {
            get => _translation;
            set
            {
                if (Set(ref _translation, value ?? new EventVec3(),
                    () => _translation.Changed -= UpdateMatrix,
                    () => _translation.Changed += UpdateMatrix,
                    false))
                    UpdateMatrix();
            }
        }
        [TSerialize]
        [Category("Transform")]
        public virtual EventVec3 Scale
        {
            get => _scale;
            set
            {
                if (Set(ref _scale, value ?? EventVec3.One,
                    () => _scale.Changed -= UpdateMatrix,
                    () => _scale.Changed += UpdateMatrix,
                    false))
                    UpdateMatrix();
            }
        }

        protected void UpdateMatrix()
        {
            InvalidateLayout();
            //ActualTranslation.Raw = Translation.Xy;
            //RecalcLocalTransform();
        }

        protected virtual void OnResizeActual(BoundingRectangleF parentBounds)
        {
            ActualTranslation.Raw = Translation.Xy;
        }
        protected override void OnResizeLayout(BoundingRectangleF parentBounds)
        {
            OnResizeActual(parentBounds);
            RecalcLocalTransform(true, false);
            var bounds = new BoundingRectangleF(0.0f, parentBounds.Extents);
            OnResizeChildComponents(bounds);
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Vec3 translation = new Vec3(ActualTranslation.Raw, Translation.Z);
            Vec3 scale = Scale?.Value ?? Vec3.One;

            localTransform = Matrix4.TransformMatrix(
                scale,
                Matrix4.Identity,
                translation,
                Order);

            inverseLocalTransform = Matrix4.TransformMatrix(
                1.0f / scale, 
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

            Vec2 scale = _scale.Value.Xy;
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

            Vec2 newTranslation = ActualTranslation.Raw + (worldScreenPoint - WorldPoint.Xy) * multiplier;

            _translation.Value = new Vec3(newTranslation, _translation.Z);
            _scale.Value = new Vec3(newScale, _scale.Z);

            UpdateMatrix();
            //InvalidateLayout();
        }
    }
}
