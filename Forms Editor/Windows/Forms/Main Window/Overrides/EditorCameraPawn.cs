using System;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core.Shapes;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Physics;
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
            input.RegisterButtonPressed(EMouseButton.LeftClick, OnLeftClick, EInputPauseType.TickAlways);
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
            else if (HasHit)
                RootComponent.Translation.Raw = Segment.PointAtLineDistance(RootComponent.WorldPoint, HitPoint, up ? -ScrollSpeed : ScrollSpeed);
            else
                RootComponent.TranslateRelative(0.0f, 0.0f, up ? ScrollSpeed : -ScrollSpeed);
        }

        private bool _alt = false;
        private bool _shift = false;
        private bool _leftClickDown = false;

        public bool HasHit { get; private set; } = false;
        public Vec3 HitPoint { get; private set; }
        public Vec3 HitNormal { get; private set; }
        public float HitDistance { get; private set; }
        public Vec3 HitScreenPoint { get; private set; }
        public TCollisionObject HitObject { get; private set; }

        private EditorHud EditorHud => HUD as EditorHud;

        private bool DragZooming => _ctrl && _leftClickDown;

        protected override void MouseMove(float x, float y)
        {
            if (Rotating)
            {
                float pitch = y * MouseRotateSpeed;
                float yaw = -x * MouseRotateSpeed;

                if (EditorHud.SelectedComponent != null)
                    RootComponent.ArcBallRotate(pitch, yaw, EditorHud.SelectedComponent.WorldPoint);
                else if (HasHit)
                    RootComponent.ArcBallRotate(pitch, yaw, HitPoint);
                else
                    RootComponent.Rotation.AddRotations(pitch, yaw, 0.0f);
            }
            else if (Translating)
            {
                if (HasHit)
                {
                    //This fixes stationary jitter caused by float imprecision
                    //when recalculating the same hit point every update
                    if (x == 0.0f && y == 0.0f)
                        return;

                    Vec3 oldPoint = HitPoint;
                    Vec3 screenPoint = Camera.WorldToScreen(HitPoint);
                    screenPoint.X += -x;
                    screenPoint.Y += -y;
                    HitScreenPoint = screenPoint;
                    Vec3 hitPoint = Camera.ScreenToWorld(HitScreenPoint);
                    Vec3 diff = hitPoint - oldPoint;
                    RootComponent.Translation += diff;
                }
                else
                    RootComponent.TranslateRelative(-x * MouseTranslateSpeed, -y * MouseTranslateSpeed, 0.0f);
            }
            else if (DragZooming)
            {
                bool forward = y < 0.0f;
                if (HasHit)
                    RootComponent.Translation.Raw = Segment.PointAtLineDistance(RootComponent.WorldPoint, HitPoint, forward ? -ScrollSpeed : ScrollSpeed);
                else
                    RootComponent.TranslateRelative(0.0f, 0.0f, forward ? -ScrollSpeed : ScrollSpeed);
            }
        }
        protected override void Tick(float delta)
        {
            bool moving = Translating || Rotating;
            CursorManager.WrapCursorWithinClip = moving;
            if (!moving)
            {
                Viewport v = LocalPlayerController?.Viewport;
                if (v != null)
                {
                    Vec2 viewportPoint = HUD.CursorPosition(v);
                    Segment s = v.GetWorldSegment(viewportPoint);
                    RayTraceClosest c = new RayTraceClosest(s.StartPoint, s.EndPoint, 0, 0xFFFF);
                    if (HasHit = c.Trace())
                    {
                        HitPoint = c.HitPointWorld;
                        HitNormal = c.HitNormalWorld;
                        HitDistance = HitPoint.DistanceToFast(s.StartPoint);
                        HitObject = c.CollisionObject;
                    }
                }
            }

            bool translate = !(_linearRight.IsZero() && _linearUp.IsZero() && _linearForward.IsZero());
            bool rotate = !(_pitch.IsZero() && _yaw.IsZero());
            if (translate)
                RootComponent.TranslateRelative(new Vec3(_linearRight, _linearUp, -_linearForward) * delta);
            if (rotate)
                RootComponent.Rotation.AddRotations(_pitch * delta, _yaw * delta, 0.0f);
        }
    }
}
