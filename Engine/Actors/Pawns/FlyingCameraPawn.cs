using Extensions;
using System;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Actors.Types.Pawns
{
    public class FlyingCameraPawn : FlyingCameraPawnBase<TransformComponent>
    {
        public FlyingCameraPawn() : base() { }
        public FlyingCameraPawn(ELocalPlayerIndex possessor) : base(possessor) { }
        protected override TransformComponent OnConstructRoot()
        {
            TransformComponent root = new TransformComponent();
            CameraComp = new CameraComponent(new PerspectiveCamera());
            root.ChildSockets.Add(CameraComp);
            return root;
        }
        protected override void OnScrolled(bool up) 
            => RootComponent.Transform.TranslateRelative(0.0f, 0.0f, up ? ScrollSpeed : -ScrollSpeed);
        protected override void MouseMove(float x, float y)
        {
            if (Rotating)
            {
                float pitch = y * MouseRotateSpeed;
                float yaw = -x * MouseRotateSpeed;
                RootComponent.Transform.Rotation.Value *= Quat.Euler(pitch, yaw, 0.0f);
            }
            else if (Translating)
            {
                RootComponent.Transform.TranslateRelative(-x * MouseTranslateSpeed, -y * MouseTranslateSpeed, 0.0f);
            }
        }
        protected override void Tick(float delta)
        {
            bool translate = !(_linearRight.IsZero() && _linearUp.IsZero() && _linearForward.IsZero());
            bool rotate = !(_pitch.IsZero() && _yaw.IsZero());
            if (translate)
                RootComponent.Transform.TranslateRelative(new Vec3(_linearRight, _linearUp, -_linearForward) * delta);
            if (rotate)
                RootComponent.Transform.Rotation.Value *= Quat.Euler(_pitch * delta, _yaw * delta, 0.0f);
        }
    }
}
