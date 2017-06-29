using TheraEngine;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.HUD;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Types;
using System;
using System.Diagnostics;
using TheraEngine.Files;
using BulletSharp;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System.Drawing;

namespace TheraEditor
{
    public class EditorHud : HudManager
    {
        public EditorHud(Vec2 bounds) : base(bounds) { }
        protected override void PreConstruct()
        {
            _highlightPoint = new HighlightPoint() { Hud = this };
            base.PreConstruct();
            //RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick);
        }
        public override void RegisterInput(InputInterface input)
        {
            //input.RegisterMouseScroll(OnScrolledInput, InputPauseType.TickOnlyWhenPaused);
            input.RegisterMouseMove(OnMouseMove, false, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, OnLeftClickSelect, InputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, InputPauseType.TickAlways);

            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, InputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnBackInput, InputPauseType.TickAlways);
        }
        protected override void OnMouseMove(float x, float y)
        {
            base.OnMouseMove(x, y);
            HighlightScene(false);
        }
        protected override void OnLeftClickSelect()
        {
            _highlightPoint.PickScene();
        }
        protected override void OnGamepadSelect()
        {
            _highlightPoint.PickScene();
        }

        public class HighlightPoint : I3DRenderable
        {
            private PrimitiveManager _circlePrimitive = new PrimitiveManager(Circle3D.WireframeMesh(1.0f, Vec3.Forward, Vec3.Zero, 30), Material.GetUnlitColorMaterial(Color.LimeGreen, false));
            private PrimitiveManager _normalPrimitive = new PrimitiveManager(Segment.Mesh(Vec3.Zero, Vec3.Forward), Material.GetUnlitColorMaterial(Color.LimeGreen, false));

            private EditorHud _hud;
            private SceneComponent _highlightedComponent;
            private bool _isRendering;
            private IOctreeNode _octreeNode;
            private Matrix4 _transform = Matrix4.Identity;

            public EditorHud Hud { get => _hud; set => _hud = value; }
            public bool HasTransparency => false;
            public Shape CullingVolume => null;

            public IOctreeNode OctreeNode { get => _octreeNode; set => _octreeNode = value; }
            public bool IsRendering { get => _isRendering; set => _isRendering = value; }

            public SceneComponent HighlightedComponent
            {
                get => _highlightedComponent;
                set
                {
                    if (value == null && _highlightedComponent != null)
                    {
                        Engine.Scene.Remove(this);
                        RenderPanel.CapturedPanel.Invoke(new Action(() => RenderPanel.CapturedPanel.Cursor = System.Windows.Forms.Cursors.Default));
                    }
                    else if (value != null && _highlightedComponent == null)
                    {
                        Engine.Scene.Add(this);
                        RenderPanel.CapturedPanel.Invoke(new Action(() => RenderPanel.CapturedPanel.Cursor = System.Windows.Forms.Cursors.Hand));
                    }

                    if (_highlightedComponent != null)
                    {
                        EditorState state = HighlightedComponent.OwningActor.EditorState;
                        state.Highlighted = false;
                    }
                    _highlightedComponent = value;
                    if (_highlightedComponent != null)
                    {
                        //Debug.WriteLine(_highlightedComponent.OwningActor.Name);
                        EditorState state = HighlightedComponent.OwningActor.EditorState;
                        state.Highlighted = true;
                    }
                }
            }
            
            public void Update(Viewport v, Vec2 viewportPoint)
            {
                if (EditorTransformTool3D.CurrentInstance != null)
                {
                    Ray cursor = v.GetWorldRay(viewportPoint);
                    if (EditorTransformTool3D.CurrentInstance.Highlight(cursor, v.Camera, false))
                    {
                        HighlightedComponent = EditorTransformTool3D.CurrentInstance.RootComponent;
                        return;
                    }
                }

                HighlightedComponent = v.PickScene(viewportPoint, true, true, out Vec3 hitNormal, out Vec3 hitPoint);
                _transform = Matrix4.CreateTranslation(hitPoint) * hitNormal.LookatAngles().GetMatrix() * Matrix4.CreateScale(_hud.OwningPawn.LocalPlayerController.Viewport.Camera.DistanceScale(hitPoint, 2.0f));
            }

            public void PickScene()
            {
                if (HighlightedComponent != null)
                {
                    if (HighlightedComponent.OwningActor is EditorTransformTool3D tool)
                    {

                    }
                    else if (HighlightedComponent is HudComponent hudComp)
                    {

                    }
                    else// if (comp != null)
                    {
                        if (HighlightedComponent is IPhysicsDrivable d && d.PhysicsDriver.SimulatingPhysics)
                        {
                            //hitDistance = cursor.StartPoint.DistanceToFast(hitPoint);
                            //RigidBody body = d.PhysicsDriver.CollisionObject;
                            //Generic6DofConstraint constraint = new Generic6DofConstraint(body, Vec3.Zero, new Vec3(0, 1, 0), true);
                            //World.AddConstraint(hinge);
                        }
                    }
                }
            }

            public void Render()
            {
                if (_highlightedComponent != null)
                {
                    _circlePrimitive.Render(_transform, Matrix3.Identity);
                    _normalPrimitive.Render(_transform, Matrix3.Identity);
                }
            }
        }

        private HighlightPoint _highlightPoint;
        
        private void HighlightScene(bool gamepad)
        {
            Viewport v = Engine.ActivePlayers[0].Viewport;
            if (v != null)
            {
                Vec2 viewportPoint = gamepad ? v.Center : v.AbsoluteToRelative(_cursorPos);
                _highlightPoint.Update(v, viewportPoint);
            }
        }
    }
}