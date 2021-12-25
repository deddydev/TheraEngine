using Extensions;
using System;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths;
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
            root.ChildSockets.Add(CameraComp = new CameraComponent(new PerspectiveCamera()));
            return root;
        }
        protected override void OnScrolled(bool up) 
            => RootComponent.Transform.TranslateRelative(0.0f, 0.0f, up ? ScrollSpeed : -ScrollSpeed);
        protected override void MouseMove(float x, float y)
        {
            if (Rotating)
            {
                AddYawPitch(
                    -x * MouseRotateSpeed,
                    -y * MouseRotateSpeed);
            }
            else if (Translating)
            {
                RootComponent.Transform.TranslateRelative(
                    -x * MouseTranslateSpeed,
                    -y * MouseTranslateSpeed,
                    0.0f);
            }
        }
        public void Pivot(float pitch, float yaw, float distance)
            => ArcBallRotate(pitch, yaw, RootComponent.Transform.Translation + RootComponent.Transform.Matrix.Value.ForwardVec * distance);
        public void ArcBallRotate(float pitch, float yaw, Vec3 focusPoint)
        {
            //"Arcball" rotation
            //All rotation is done within local component space

            RootComponent.Transform.Translation.Value = TMath.ArcballTranslation(
                pitch, yaw, focusPoint,
                RootComponent.Transform.Translation.Value, 
                RootComponent.Transform.Matrix.Value.RightVec);

            AddYawPitch(yaw, pitch);
        }
        protected override void Tick(float delta)
        {
            IncrementRotation();

            if (!(_incRight.IsZero() && _incUp.IsZero() && _incForward.IsZero()))
                RootComponent.Transform.TranslateRelative(new Vec3(_incRight, _incUp, -_incForward) * delta);
        }

        private void IncrementRotation()
        {
            if (!_incPitch.IsZero())
            {
                if (!_incYaw.IsZero())
                    AddYawPitch(_incYaw, _incPitch);
                else
                    Pitch += _incPitch;
            }
            else if (!_incYaw.IsZero())
                Yaw += _incYaw;
        }

        protected override void YawPitchUpdated()
        {
            RootComponent.Transform.Rotation.Value = Quat.Euler(Pitch, Yaw, 0.0f);
        }
    }
}
