using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.UI
{
    public class UIVec3 : TObject
    {
        public static UIVec3 Zero { get; } = new UIVec3(0.0f, 0.0f, 0.0f);
        public static UIVec3 One { get; } = new UIVec3(1.0f, 1.0f, 1.0f);

        public UIVec3()
        {
            XProperty = new UIFloat();
            YProperty = new UIFloat();
            ZProperty = new UIFloat();
        }
        public UIVec3(UIFloat x, UIFloat y, UIFloat z)
        {
            XProperty = x;
            YProperty = y;
            ZProperty = z;
        }
        public UIVec3(UIFloat xyz)
        {
            XProperty = xyz;
            YProperty = xyz;
            ZProperty = xyz;
        }

        public UIFloat XProperty { get; } = new UIFloat();
        public UIFloat YProperty { get; } = new UIFloat();
        public UIFloat ZProperty { get; } = new UIFloat();

        public Vec3 Xyz 
        {
            get => new Vec3(XProperty.Value, YProperty.Value, ZProperty.Value);
            set
            {
                XProperty.Value = value.X;
                YProperty.Value = value.Y;
                ZProperty.Value = value.Z;
            }
        }
        public Vec2 Xy
        {
            get => new Vec2(XProperty.Value, YProperty.Value);
            set
            {
                XProperty.Value = value.X;
                YProperty.Value = value.Y;
            }
        }
        public Vec2 Yz
        {
            get => new Vec2(YProperty.Value, ZProperty.Value);
            set
            {
                YProperty.Value = value.X;
                ZProperty.Value = value.Y;
            }
        }
        public float X
        {
            get => XProperty.Value;
            set => XProperty.Value = value;
        }
        public float Y
        {
            get => YProperty.Value;
            set => YProperty.Value = value;
        }
        public float Z
        {
            get => ZProperty.Value;
            set => ZProperty.Value = value;
        }
    }
}
