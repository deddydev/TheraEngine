using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public class UIRotatableComponent : UIComponent
    {
        private float _rotationAngle = 0.0f;

        /// <summary>
        /// The rotation angle of the component in degrees.
        /// </summary>
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
            //step 1: set identity matrix
            //step 2: translate into position (bottom left corner)
            //step 3: rotate in position
            //step 4: translate backward, relative to the rotation, by the local rotation origin to center on the rotation point
            //step 5: scale the component

            Vec2 localOriginTrans = LocalOriginTranslation;
            localTransform =
                Matrix4.CreateTranslation(LocalTranslationX, LocalTranslationY, 0.0f) *
                Matrix4.CreateTranslation(localOriginTrans.X, localOriginTrans.Y, 0.0f) *
                (Matrix4.CreateTranslation(-localOriginTrans.X, -localOriginTrans.Y, 0.0f) *
                Matrix4.CreateRotationZ(RotationAngle) *
                Matrix4.CreateScale(ScaleX, ScaleY, 1.0f));

            inverseLocalTransform =
                Matrix4.CreateTranslation(-localOriginTrans.X, -localOriginTrans.Y, 0.0f) *
                Matrix4.CreateScale(1.0f / ScaleX, 1.0f / ScaleY, 1.0f) *
                Matrix4.CreateRotationZ(-RotationAngle) *
                Matrix4.CreateTranslation(localOriginTrans.X, localOriginTrans.Y, 0.0f) *
                Matrix4.CreateTranslation(-LocalTranslationX, -LocalTranslationY, 0.0f);
        }

        protected override void OnResized()
        {
            //TODO: calculate min max x and y by transforming quad corner points
            //_axisAlignedBounds.Translation = Vec3.TransformPosition(WorldPoint, GetInvActorTransform()).Xy;
            //_axisAlignedBounds.Bounds = Size;
            RecalcLocalTransform();
        }
    }
}
