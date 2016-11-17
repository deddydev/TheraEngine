using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Worlds.Actors.Components;
using System;
using CustomEngine.Rendering.Cameras;
using System.Linq;

namespace CustomEngine.Worlds.Actors
{
    public class CameraPawn : Pawn
    {
        float _linearSpeed = 0.1f, _linearRight = 0.0f, _linearForward = 0.0f, _linearUp = 0.0f;
        float _radialSpeed = 0.1f, _pitch = 0.0f, _yaw = 0.0f, _roll = 0.0f;
        bool _rotating = false;
        bool _translating = false;
        
        CameraComponent CameraComponent { get { return RootComponent as CameraComponent; } }
        protected override void SetDefaults()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input);
            base.SetDefaults();
        }
        protected override SceneComponent SetupComponents()
        {
            return new CameraComponent(false);
        }
        protected override void RegisterInput(InputInterface input)
        {
            input.RegisterMouseScroll(OnScrolled);
            input.RegisterMouseMove(OnMouseMove, MouseMoveType.Relative);
            input.RegisterButtonPressed(EMouseButton.RightClick, OnRightClick);
            input.RegisterButtonPressed(EMouseButton.LeftClick, OnLeftClick);
            input.RegisterButtonPressed(EKey.A, OnAPressed);
            input.RegisterButtonPressed(EKey.W, OnWPressed);
            input.RegisterButtonPressed(EKey.S, OnSPressed);
            input.RegisterButtonPressed(EKey.D, OnDPressed);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickX, OnRightStickX, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickY, OnRightStickY, false);
            input.RegisterButtonPressed(GamePadButton.RightBumper, OnRBPressed);
            input.RegisterButtonPressed(GamePadButton.LeftBumper, OnLBPressed);
        }
        private void OnLBPressed(bool pressed) { _linearUp += pressed ? -1.0f : 1.0f; }
        private void OnRBPressed(bool pressed) { _linearUp += pressed ? 1.0f : -1.0f; }
        private void OnLeftStickX(float value) { _linearRight = value; }
        private void OnLeftStickY(float value) { _linearForward = value; }
        private void OnRightStickX(float value) { _yaw = value; }
        private void OnRightStickY(float value) { _pitch = value; }
        public void OnScrolled(bool up)
        {

        }
        public void OnRightClick(bool pressed)
        {

        }
        public void OnLeftClick(bool pressed)
        {
            
        }
        public void OnAPressed(bool pressed)
        {

        }
        public void OnWPressed(bool pressed)
        {

        }
        public void OnSPressed(bool pressed)
        {

        }
        public void OnDPressed(bool pressed)
        {

        }
        public void OnMouseMove(float x, float y)
        {

        }
        internal override void Tick(float delta)
        {
            Transform.TranslateRelative(new Vec3(_linearRight, _linearUp, _linearForward) * _linearSpeed);
            Quaternion yaw = Quaternion.FromAxisAngle(Vec3.Up, _yaw * _radialSpeed);
            Quaternion pitch = Quaternion.FromAxisAngle(Vec3.Right, _pitch * _radialSpeed);
            Quaternion roll = Quaternion.FromAxisAngle(Vec3.Forward, _roll * _radialSpeed);
            Transform.RotateInPlace(roll * pitch * yaw);
        }
    }
}
