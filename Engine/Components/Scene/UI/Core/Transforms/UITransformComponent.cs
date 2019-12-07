using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.UI
{
    public interface IUITransformComponent : IUIComponent
    {
        Vec2 ScreenTranslation { get; }

        Vec2 LocalTranslation { get; set; }
        float LocalTranslationX { get; set; }
        float LocalTranslationY { get; set; }

        Vec2 Scale { get; set; }
        float ScaleX { get; set; }
        float ScaleY { get; set; }
    }
    public class UITransformComponent : UIComponent, IUITranslationComponent
    {
        public UITransformComponent() : base() { }

        [TSerialize]
        public ETransformOrder Order { get; set; }

        #region Translation

        protected Vec2 _translation = Vec2.Zero;

        [Browsable(false)]
        [Category("Transform")]
        public Vec2 ScreenTranslation
        {
            get => Vec3.TransformPosition(WorldPoint, GetComponentTransform()).Xy;
            set => LocalTranslation = Vec3.TransformPosition(value, GetInvComponentTransform()).Xy;
        }

        [Category("Transform")]
        public virtual Vec2 LocalTranslation
        {
            get => _translation;
            set
            {
                _translation = value;
                //RecalcLocalTransform();
                Resize();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationX
        {
            get => _translation.X;
            set
            {
                _translation.X = value;
                //RecalcLocalTransform();
                Resize();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationY
        {
            get => _translation.Y;
            set
            {
                _translation.Y = value;
                //RecalcLocalTransform();
                Resize();
            }
        }
        #endregion

        #region Scale

        protected Vec2 _scale = Vec2.One;

        [Category("Transform")]
        public virtual Vec2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                RecalcLocalTransform();
                //PerformResize();
            }
        }
        [Browsable(false)]
        [Category("Transform")]
        public virtual float ScaleX
        {
            get => _scale.X;
            set
            {
                _scale.X = value;
                RecalcLocalTransform();
                //PerformResize();
            }
        }
        [Browsable(false)]
        [Category("Transform")]
        public virtual float ScaleY
        {
            get => _scale.Y;
            set
            {
                _scale.Y = value;
                RecalcLocalTransform();
                //PerformResize();
            }
        }
        #endregion

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.TransformMatrix(Scale, Matrix4.Identity, LocalTranslation, Order);
            inverseLocalTransform = Matrix4.TransformMatrix(1.0f / ScaleX, Matrix4.Identity, -LocalTranslation, Transform.OppositeOrder(Order));
        }

        public void Zoom(float amount, Vec2 worldScreenPoint, Vec2? minScale, Vec2? maxScale)
        {
            if (amount == 0.0f)
                return;

            Vec2 multiplier = Vec2.One / _scale * amount;
            Vec2 newScale = _scale - amount;

            bool xClamped = false;
            bool yClamped = false;
            if (minScale != null)
            {
                if (newScale.X < minScale.Value.X)
                {
                    newScale.X = minScale.Value.X;
                    xClamped = true;
                }
                if (newScale.Y < minScale.Value.Y)
                {
                    newScale.Y = minScale.Value.Y;
                    yClamped = true;
                }
            }
            if (maxScale != null)
            {
                if (newScale.X > maxScale.Value.X)
                {
                    newScale.X = maxScale.Value.X;
                    xClamped = true;
                }
                if (newScale.Y > maxScale.Value.Y)
                {
                    newScale.Y = maxScale.Value.Y;
                    yClamped = true;
                }
            }

            if (!xClamped || !yClamped)
            {
                _translation = (_translation + (worldScreenPoint - WorldPoint.Xy) * multiplier);
                _scale = newScale;
            }

            Resize();

            //_cameraToWorldSpaceMatrix = _cameraToWorldSpaceMatrix * Matrix4.CreateScale(scale);
            //_worldToCameraSpaceMatrix = Matrix4.CreateScale(-scale) * _worldToCameraSpaceMatrix;
            //UpdateTransformedFrustum();
            //OnTransformChanged();
        }
    }
}
