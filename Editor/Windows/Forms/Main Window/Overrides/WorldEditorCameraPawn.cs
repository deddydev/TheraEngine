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

        //private TransformComponent _camera = null;
        //public TransformComponent TargetCamera
        //{
        //    get => _camera ?? RootComponent;
        //    set => _camera = value;
        //}
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

            TransformComponent comp = RootComponent;
            if (_ctrl)
                Engine.TimeDilation *= up ? 0.8f : 1.2f;
            else
            {
                float amount = up ? ScrollSpeed : -ScrollSpeed;
                if (HasHit)
                    comp.Transform.Translation.Value = Segment.PointAtLineDistance(comp.WorldPoint, HitPoint, -amount);
                else
                    comp.Transform.TranslateRelative(0.0f, 0.0f, amount);
            }

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
            TransformComponent comp = RootComponent;
            if (Rotating)
            {
                float pitch = y * MouseRotateSpeed;
                float yaw = -x * MouseRotateSpeed;

                var selComp = EditorHud?.SelectedComponent;
                if (selComp != null)
                    ArcBallRotate(pitch, yaw, selComp.WorldPoint);
                else if (HasHit)
                    ArcBallRotate(pitch, yaw, HitPoint);
                else
                    AddYawPitch(yaw, pitch);
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
                    
                    comp.Translation += Camera.ScreenToWorld(Camera.WorldToScreen(HitPoint) - new Vec3(x, y, 0.0f)) - HitPoint;
                }
                else
                    comp.Transform.TranslateRelative(-x * MouseTranslateSpeed, -y * MouseTranslateSpeed, 0.0f);
            }
            else if (DragZooming)
            {
                float scrollSpeed = y < 0.0f ? -ScrollSpeed : ScrollSpeed;
                if (HasHit)
                    comp.Translation = Segment.PointAtLineDistance(RootComponent.WorldPoint, HitPoint, scrollSpeed);
                else
                    comp.Transform.TranslateRelative(0.0f, 0.0f, scrollSpeed);
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

            base.Tick(delta);
        }
    }
}
