using Extensions;
using System;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.UI;

namespace TheraEditor.Actors.Types.Pawns
{
    public class WorldEditorCameraPawn : FlyingCameraPawn
    {
        public WorldEditorCameraPawn() : base() { }
        public WorldEditorCameraPawn(ELocalPlayerIndex possessor) : base(possessor) { }

        private ICameraTransformable _camera = null;
        public ICameraTransformable TargetCamera
        {
            get => _camera ?? RootComponent;
            set => _camera = value;
        }
        public UIViewportComponent TargetViewportComponent { get; set; } = null;

        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);

            input.RegisterButtonPressed(EMouseButton.LeftClick, OnLeftClick, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.AltLeft, OnAlt, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.AltRight, OnAlt, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ShiftLeft, OnShift, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ShiftRight, OnShift, EInputPauseType.TickAlways);
        }

        protected virtual void OnLeftClick(bool pressed)
            => _leftClickDown = pressed;
        private void OnAlt(bool pressed) 
            => _alt = pressed;
        private void OnShift(bool pressed)
            => _shift = pressed;

        protected override void OnScrolled(bool up)
        {
            if (_alt || _shift)
                return;

            ICameraTransformable comp = TargetCamera;
            if (_ctrl)
                Engine.TimeDilation *= up ? 0.8f : 1.2f;
            else if (HasHit)
                comp.Translation.Value = Segment.PointAtLineDistance(comp.WorldPoint, HitPoint, up ? -ScrollSpeed : ScrollSpeed);
            else
                comp.TranslateRelative(0.0f, 0.0f, up ? ScrollSpeed : -ScrollSpeed);

            //if (!Moving)
            //{
            //    Viewport v = LocalPlayerController?.Viewport;
            //    EditorHud.ViewChanged(v, Viewport.CursorPosition(v));
            //}
        }

        private bool _alt = false;
        private bool _shift = false;
        private bool _leftClickDown = false;

        public bool HasHit => EditorHud?.HighlightedComponent != null;
        public Vec3 HitPoint => EditorHud?.HitPoint ?? Vec3.Zero;
        public Vec3 HitNormal => EditorHud?.HitNormal ?? Vec3.Zero;
        public float HitDistance => EditorHud?.HitDistance ?? 0.0f;
        public Vec3 HitScreenPoint { get; private set; }
        private EditorUI3D EditorHud => HUD?.File as EditorUI3D;

        private bool DragZooming => _ctrl && _leftClickDown;

        protected override void MouseMove(float x, float y)
        {
            //if (TargetViewportComponent != null && Moving)
            //{
            //    Vec2 result = TargetViewportComponent.ScreenToLocal(new Vec2(x, y), true);
            //    x = result.X;
            //    y = result.Y;
            //}
            ICameraTransformable comp = TargetCamera;
            if (Rotating)
            {
                float pitch = y * MouseRotateSpeed;
                float yaw = -x * MouseRotateSpeed;

                var selComp = EditorHud?.SelectedComponent;
                if (selComp != null)
                    comp.ArcBallRotate(pitch, yaw, selComp.WorldPoint);
                else if (HasHit)
                    comp.ArcBallRotate(pitch, yaw, HitPoint);
                else
                    comp.Rotation.AddRotations(pitch, yaw, 0.0f);
            }
            else if (Translating)
            {
                if (HasHit && Camera != null)
                {
                    //This fixes stationary jitter caused by float imprecision
                    //when recalculating the same hit point every update
                    if (Math.Abs(x) < 0.00001f && 
                        Math.Abs(y) < 0.00001f)
                        return;
                    
                    Vec3 oldPoint = HitPoint;
                    Vec3 screenPoint = Camera.WorldToScreen(HitPoint);
                    screenPoint.X += -x;
                    screenPoint.Y += -y;
                    HitScreenPoint = screenPoint;
                    Vec3 hitPoint = Camera.ScreenToWorld(HitScreenPoint);
                    Vec3 diff = hitPoint - oldPoint;
                    comp.Translation += diff;
                }
                else
                    comp.TranslateRelative(-x * MouseTranslateSpeed, -y * MouseTranslateSpeed, 0.0f);
            }
            else if (DragZooming)
            {
                bool forward = y < 0.0f;
                if (HasHit)
                    comp.Translation.Value = Segment.PointAtLineDistance(comp.WorldPoint, HitPoint, forward ? -ScrollSpeed : ScrollSpeed);
                else
                    comp.TranslateRelative(0.0f, 0.0f, forward ? -ScrollSpeed : ScrollSpeed);
            }
        }
        protected override void Tick(float delta)
        {
            bool moving = Moving;
            CursorManager.GlobalWrapCursorWithinClip = moving;

            //if (!moving)
            //{
            //    Viewport v = LocalPlayerController?.Viewport;
            //    if (v != null)
            //    {
            //        Vec2 viewportPoint = HUD.CursorPosition(v);
            //        Segment s = v.GetWorldSegment(viewportPoint);
            //        RayTraceClosest c = new RayTraceClosest(s.StartPoint, s.EndPoint, 0, 0xFFFF);
            //        if (HasHit = c.Trace())
            //        {
            //            HitPoint = c.HitPointWorld;
            //            HitNormal = c.HitNormalWorld;
            //            HitDistance = HitPoint.DistanceToFast(s.StartPoint);
            //            HitObject = c.CollisionObject;
            //        }
            //    }
            //}

            ICameraTransformable comp = TargetCamera;
            bool translate = !(_linearRight.IsZero() && _linearUp.IsZero() && _linearForward.IsZero());
            bool rotate = !(_pitch.IsZero() && _yaw.IsZero());
            if (translate)
                comp.TranslateRelative(new Vec3(_linearRight, _linearUp, -_linearForward) * delta);
            if (rotate)
                comp.Rotation.AddRotations(_pitch * delta, _yaw * delta, 0.0f);
        }
    }
}
