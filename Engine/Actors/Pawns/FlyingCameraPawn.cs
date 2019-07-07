using Extensions;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Actors.Types.Pawns
{
    public class FlyingCameraPawn : FlyingCameraPawnBase<TRComponent>
    {
        public FlyingCameraPawn() : base() { }
        public FlyingCameraPawn(ELocalPlayerIndex possessor) : base(possessor) { }
        protected override TRComponent OnConstructRoot()
        {
            TRComponent root = new TRComponent();
            CameraComp = new CameraComponent(new PerspectiveCamera());
            root.ChildComponents.Add(CameraComp);
            return root;
        }
        protected override void OnScrolled(bool up) 
            => RootComponent.TranslateRelative(0.0f, 0.0f, up ? ScrollSpeed : -ScrollSpeed);
        protected override void MouseMove(float x, float y)
        {
            if (Rotating)
            {
                float pitch = y * MouseRotateSpeed;
                float yaw = -x * MouseRotateSpeed;
                RootComponent.Rotation.AddRotations(pitch, yaw, 0.0f);
            }
            else if (Translating)
            {
                RootComponent.TranslateRelative(-x * MouseTranslateSpeed, -y * MouseTranslateSpeed, 0.0f);
            }
        }
        protected override void Tick(float delta)
        {
            bool translate = !(_linearRight.IsZero() && _linearUp.IsZero() && _linearForward.IsZero());
            bool rotate = !(_pitch.IsZero() && _yaw.IsZero());
            if (translate)
                RootComponent.TranslateRelative(new Vec3(_linearRight, _linearUp, -_linearForward) * delta);
            if (rotate)
                RootComponent.Rotation.AddRotations(_pitch * delta, _yaw * delta, 0.0f);
        }
    }
}
