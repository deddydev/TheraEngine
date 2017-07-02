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
using System.Windows.Forms;
using System.ComponentModel;

namespace TheraEditor
{
    public class EditorHud : HudManager
    {
        public EditorHud(Vec2 bounds, Editor owner) : base(bounds)
        {
            _editor = owner;
        }

        private Editor _editor;
        private HighlightPoint _highlightPoint;
        RigidBody _pickedBody;
        Point2PointConstraint _currentConstraint;
        private float _hitDistance;
        private Vec3 _hitPoint;
        private float _toolSize = 2.0f;
        private SceneComponent _selectedComponent;

        [Browsable(false)]
        public Editor Editor => _editor;
        public SceneComponent HighlightedComponent
        {
            get => _highlightPoint.HighlightedComponent;
            set
            {
                if (value == null && HighlightedComponent != null)
                {
                    Engine.Scene.Remove(_highlightPoint);
                    RenderPanel.CapturedPanel.Invoke(new Action(() => RenderPanel.CapturedPanel.Cursor = Cursors.Default));
                }
                else if (value != null && HighlightedComponent == null)
                {
                    Engine.Scene.Add(_highlightPoint);
                    RenderPanel.CapturedPanel.Invoke(new Action(() => RenderPanel.CapturedPanel.Cursor = Cursors.Hand));
                }

                if (HighlightedComponent != null)
                {
                    EditorState state = HighlightedComponent.OwningActor.EditorState;
                    state.Highlighted = false;
                }
                _highlightPoint.HighlightedComponent = value;
                if (HighlightedComponent != null)
                {
                    //Debug.WriteLine(_highlightedComponent.OwningActor.Name);
                    EditorState state = HighlightedComponent.OwningActor.EditorState;
                    state.Highlighted = true;
                }
            }
        }


        protected override void PreConstruct()
        {
            _highlightPoint = new HighlightPoint();
            base.PreConstruct();
            //RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick);
        }
        public override void RegisterInput(InputInterface input)
        {
            //input.RegisterMouseScroll(OnScrolledInput, InputPauseType.TickOnlyWhenPaused);
            input.RegisterMouseMove(OnMouseMove, false, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, OnMouseDown, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Released, OnMouseUp, InputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, InputPauseType.TickAlways);

            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, InputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnBackInput, InputPauseType.TickAlways);
        }
        protected override void OnMouseMove(float x, float y)
        {
            base.OnMouseMove(x, y);
            HighlightScene(false);
        }
        protected void OnMouseDown()
        {
            MouseDown();
        }
        protected void OnMouseUp()
        {
            MouseUp();
        }
        protected void OnGamepadSelect()
        {
            MouseDown();
        }
        private void HighlightScene(bool gamepad)
        {
            Viewport v = Engine.ActivePlayers[0].Viewport;
            if (v != null)
            {
                Vec2 viewportPoint = gamepad ? v.Center : v.AbsoluteToRelative(_cursorPos);
                MouseMove(v, viewportPoint);
            }
        }
        public void MouseMove(Viewport v, Vec2 viewportPoint)
        {
            if (_selectedComponent != null)
            {
                Ray cursor = v.GetWorldRay(viewportPoint);
                if (_currentConstraint != null)
                    _currentConstraint.PivotInB = cursor.StartPoint + cursor.Direction * _hitDistance;
            }
            else
            {
                if (EditorTransformTool3D.Instance != null)
                {
                    Ray cursor = v.GetWorldRay(viewportPoint);
                    if (EditorTransformTool3D.Instance.Highlight(cursor, v.Camera, false))
                    {
                        HighlightedComponent = EditorTransformTool3D.Instance.RootComponent;
                        return;
                    }
                }

                SceneComponent comp = v.PickScene(viewportPoint, true, true, out Vec3 hitNormal, out _hitPoint, out _hitDistance);
                _highlightPoint.Transform = Matrix4.CreateTranslation(_hitPoint) * hitNormal.LookatAngles().GetMatrix() * Matrix4.CreateScale(OwningPawn.LocalPlayerController.Viewport.Camera.DistanceScale(_hitPoint, _toolSize));
                HighlightedComponent = comp;
            }
        }
        public void MouseUp()
        {
            if (_currentConstraint != null)
            {
                Engine.World.PhysicsScene.RemoveConstraint(_currentConstraint);
                _currentConstraint.Dispose();
                _currentConstraint = null;
                _pickedBody.ForceActivationState(ActivationState.ActiveTag);
                _pickedBody = null;
            }
            _selectedComponent = null;
            if (HighlightedComponent != null)
            {
                Engine.Scene.Add(_highlightPoint);
            }
        }
        public void MouseDown()
        {
            _selectedComponent = HighlightedComponent;

            if (_selectedComponent != null)
            {
                Engine.Scene.Remove(_highlightPoint);
                if (_selectedComponent.OwningActor is EditorTransformTool3D tool)
                {

                }
                else if (_selectedComponent is HudComponent hudComp)
                {

                }
                else// if (comp != null)
                {
                    if (_selectedComponent is IPhysicsDrivable d && d.PhysicsDriver.SimulatingPhysics)
                    {
                        _pickedBody = d.PhysicsDriver.CollisionObject;
                        _pickedBody.ForceActivationState(ActivationState.ActiveTag);

                        Vec3 localPivot = Vector3.TransformCoordinate(_hitPoint, Matrix.Invert(_pickedBody.CenterOfMassTransform));
                        Point2PointConstraint p2p = new Point2PointConstraint(_pickedBody, localPivot);
                        p2p.Setting.ImpulseClamp = 60;
                        p2p.Setting.Tau = 0.1f;

                        _currentConstraint = p2p;
                        Engine.World.PhysicsScene.AddConstraint(_currentConstraint);
                    }
                    else
                    {
                        EditorTransformTool3D.GetCurrentInstance(_selectedComponent);
                    }
                    TreeNode t = _selectedComponent.OwningActor.EditorState.TreeNode;
                    if (t != null)
                    {
                        if (t.TreeView.InvokeRequired)
                            t.TreeView.Invoke(new Action(() => t.TreeView.SelectedNode = t));
                        else
                            t.TreeView.SelectedNode = t;
                    }
                }
            }
            else
            {
                EditorTransformTool3D.DestroyInstance();
            }
        }
        public class HighlightPoint : I3DRenderable
        {
            public const int CirclePrecision = 30;
            public static readonly Color Color = Color.LimeGreen;

            private PrimitiveManager _circlePrimitive = new PrimitiveManager(Circle3D.WireframeMesh(1.0f, Vec3.Forward, Vec3.Zero, CirclePrecision), Material.GetUnlitColorMaterial(Color, false));
            private PrimitiveManager _normalPrimitive = new PrimitiveManager(Segment.Mesh(Vec3.Zero, Vec3.Forward), Material.GetUnlitColorMaterial(Color, false));

            private Matrix4 _transform = Matrix4.Identity;
            private SceneComponent _highlightedComponent;
            private bool _isRendering;
            private IOctreeNode _octreeNode;

            public bool HasTransparency => false;
            public Shape CullingVolume => null;
            public IOctreeNode OctreeNode { get => _octreeNode; set => _octreeNode = value; }
            public bool IsRendering { get => _isRendering; set => _isRendering = value; }
            public SceneComponent HighlightedComponent { get => _highlightedComponent; set => _highlightedComponent = value; }
            public Matrix4 Transform { get => _transform; set => _transform = value; }

            public void Render()
            {
                if (HighlightedComponent != null)
                {
                    _circlePrimitive.Render(Transform, Matrix3.Identity);
                    _normalPrimitive.Render(Transform, Matrix3.Identity);
                }
            }
        }
    }
}