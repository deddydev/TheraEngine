using System;
using System.ComponentModel;
using TheraEngine.Core.Attributes;

namespace TheraEngine.Rendering.UI
{
    public class UIRotatableComponent : UIComponent
    {
        private float _rotationAngle = 0.0f;

        /// <summary>
        /// The rotation angle of the component in degrees.
        /// </summary>
        [TNumericPrefixSuffix("", "°")]
        [Category("Transform")]
        public float RotationAngle
        {
            get => _rotationAngle;
            set
            {
                _rotationAngle = value;
                RecalcLocalTransform();
            }
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform =
                Matrix4.CreateTranslation(LocalTranslationX, LocalTranslationY, 0.0f) *
                Matrix4.CreateRotationZ(RotationAngle) *
                Matrix4.CreateScale(ScaleX, ScaleY, 1.0f);

            inverseLocalTransform =
                Matrix4.CreateScale(1.0f / ScaleX, 1.0f / ScaleY, 1.0f) *
                Matrix4.CreateRotationZ(-RotationAngle) *
                Matrix4.CreateTranslation(-LocalTranslationX, -LocalTranslationY, 0.0f);
        }

        public override void PerformResize()
        {
            //TODO: calculate min max x and y by transforming quad corner points
            //_axisAlignedBounds.Translation = Vec3.TransformPosition(WorldPoint, GetInvActorTransform()).Xy;
            //_axisAlignedBounds.Bounds = Size;
            base.PerformResize();
            //RecalcLocalTransform();
        }
    }
}
