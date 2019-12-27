using System;
using System.ComponentModel;
using TheraEngine.Core.Attributes;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public interface IUIRotationComponent : IUIComponent
    {
        float RotationAngle { get; set; }
    }
    public class UIRotationComponent : UIComponent
    {
        [TSerialize(nameof(RotationAngle))]
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
                if (Set(ref _rotationAngle, value))
                    RecalcLocalTransform();
            }
        }
        protected override void OnResizeLayout(BoundingRectangleF parentBounds)
        {
            RecalcLocalTransform();
            OnResizeChildComponents(parentBounds);
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.CreateRotationZ(RotationAngle);
            inverseLocalTransform = Matrix4.CreateRotationZ(-RotationAngle);
        }
    }
}
