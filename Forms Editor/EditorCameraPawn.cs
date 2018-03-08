using System;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core.Shapes;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Rendering;

namespace TheraEditor.Actors.Types.Pawns
{
    public class EditorCameraPawn : FlyingCameraPawn
    {
        public EditorCameraPawn() : base() { }
        public EditorCameraPawn(LocalPlayerIndex possessor) : base(possessor) { }

        //public EditorHud HUD => LocalPlayerController.Viewport.HUD as EditorHud;

        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
            input.RegisterButtonPressed(EMouseButton.RightClick, OnLeftClick, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.AltLeft, OnAlt, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.AltRight, OnAlt, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.ShiftLeft, OnShift, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.ShiftRight, OnShift, EInputPauseType.TickAlways);
        }

        protected virtual void OnLeftClick(bool pressed)
        {
            _leftClickDown = pressed;
        }

        private void OnAlt(bool pressed) => _alt = pressed;
        private void OnShift(bool pressed) => _shift = pressed;
        protected override void OnScrolled(bool up)
        {
            if (_alt || _shift)
                return;

            if (_ctrl)
                Engine.TimeDilation *= up ? 0.8f : 1.2f;
            else
                RootComponent.TranslateRelative(0.0f, 0.0f, up ? ScrollSpeed : -ScrollSpeed);
        }
        
        protected override void OnRightClick(bool pressed)
        {
            base.OnRightClick(pressed);

            Viewport v = LocalPlayerController.Viewport;
            Vec2 viewportPoint = HUD.CursorPosition(v);
            Segment s = v.GetWorldSegment(viewportPoint);
            RayTraceClosest c = new RayTraceClosest(s.StartPoint, s.EndPoint, 0, 0xFFFF);
            if (_hasHit = c.Trace())
                _hitPoint = c.HitPointWorld;
        }

        private bool _alt = false;
        private bool _shift = false;
        private bool _leftClickDown = false;
        private bool _hasHit = false;
        private Vec3 _hitPoint;
        private Vec3 _screenPoint;

        private EditorHud EditorHud => HUD as EditorHud;

        public override void MouseMove(float x, float y)
        {
            if (Rotating)
            {
                float pitch = y * MouseRotateSpeed;
                float yaw = -x * MouseRotateSpeed;

                if (EditorHud.SelectedComponent != null)
                    RootComponent.Pivot(pitch, yaw, EditorHud.SelectedComponent.WorldPoint);
                else if (_hasHit)
                    RootComponent.Pivot(pitch, yaw, _hitPoint);
                else
                    RootComponent.DesiredRotation.AddRotations(pitch, yaw, 0.0f);
            }
            else if (Translating)
            {
                if (_hasHit)
                {
                    Vec3 oldPoint = _hitPoint;
                    _screenPoint = Camera.WorldToScreen(_hitPoint);
                    _screenPoint.X += -x;
                    _screenPoint.Y += -y;
                    Vec3 hitPoint = Camera.ScreenToWorld(_screenPoint);
                    Vec3 diff = hitPoint - oldPoint;
                    RootComponent.DesiredTranslation += diff;
                }
                else
                    RootComponent.TranslateRelative(-x * MouseTranslateSpeed, -y * MouseTranslateSpeed, 0.0f);
            }
        }
        public override void OnSpawnedPostComponentSetup()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
            base.OnSpawnedPostComponentSetup();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
            base.OnDespawned();
        }
        private void Tick(float delta)
        {
            bool translate = !(_linearRight.IsZero() && _linearUp.IsZero() && _linearForward.IsZero());
            bool rotate = !(_pitch.IsZero() && _yaw.IsZero());
            if (translate)
                RootComponent.TranslateRelative(new Vec3(_linearRight, _linearUp, -_linearForward) * delta);
            if (rotate)
                RootComponent.DesiredRotation.AddRotations(_pitch * delta, _yaw * delta, 0.0f);
        }
    }
}
