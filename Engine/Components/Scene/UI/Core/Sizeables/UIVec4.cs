using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.UI
{
    public class UIVec4 : TObject
    {
        public UIFloat X { get; } = new UIFloat();
        public UIFloat Y { get; } = new UIFloat();
        public UIFloat Z { get; } = new UIFloat();
        public UIFloat W { get; } = new UIFloat();

        public Vec4 Value
        {
            get => new Vec4(X.Value, Y.Value, Z.Value, W.Value);
            set
            {
                X.Value = value.X;
                Y.Value = value.Y;
                Z.Value = value.Z;
                W.Value = value.W;
            }
        }
    }
}
