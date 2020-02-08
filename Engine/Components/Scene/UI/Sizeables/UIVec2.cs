using System;

namespace TheraEngine.Rendering.UI
{
    public class UIVec2 : TObject
    {
        public static UIVec2 Zero { get; } = new UIVec2(0.0f, 0.0f);
        public static UIVec2 One { get; } = new UIVec2(1.0f, 1.0f);

        public UIVec2()
        {
            XProperty = new UIFloat();
            YProperty = new UIFloat();
        }
        public UIVec2(UIFloat x, UIFloat y)
        {
            XProperty = x;
            YProperty = y;
        }
        public UIVec2(UIFloat xy)
        {
            XProperty = xy;
            YProperty = xy;
        }

        public UIFloat XProperty { get; } = new UIFloat();
        public UIFloat YProperty { get; } = new UIFloat();

        public Vec2 Xy
        {
            get => new Vec2(XProperty.Value, YProperty.Value);
            set
            {
                XProperty.Value = value.X;
                YProperty.Value = value.Y;
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
    }
}
