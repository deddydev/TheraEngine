using TheraEngine;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.HUD;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Types;
using System;
using BulletSharp;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using TheraEngine.Worlds;
using TheraEngine.Core.Shapes;

namespace TheraEditor.Windows.Forms
{
    public class EditorHud : HudManager
    {
        public EditorHud(Vec2 bounds) : base(bounds)
        {

        }
        
        private HighlightPoint _highlightPoint;
        RigidBody _pickedBody;
        Point2PointConstraint _currentConstraint;
        private float _hitDistance;
        private Vec3 _hitPoint;
        private float _toolSize = 2.0f;
        private SceneComponent _selectedComponent, _dragComponent;
        private bool _mouseDown;
        
        public SceneComponent HighlightedComponent
        {
            get => _highlightPoint.HighlightedComponent;
            set
            {
                if (value == HighlightedComponent)
                    return;

                if (value == null && HighlightedComponent != null)
                {
                    Engine.Scene.Remove(_highlightPoint);
                    RenderPanel.CheckedInvoke(new Action(() => 
                    {
                        if (RenderPanel.CapturedPanel != null)
                        {
                            RenderPanel.CapturedPanel.Cursor = Cursors.Default;
                        }
                    }),
                    RenderPanel.PanelType.Captured);
                }
                else if (value != null && HighlightedComponent == null)
                {
                    Engine.Scene.Add(_highlightPoint);
                    RenderPanel.CheckedInvoke(new Action(() =>
                    {
                        if (RenderPanel.CapturedPanel != null)
                        {
                            RenderPanel.CapturedPanel.Cursor = Cursors.Hand;
                        }
                    }),
                    RenderPanel.PanelType.Captured);
                }

                if (HighlightedComponent != null)
                {
                    EditorState state = HighlightedComponent.OwningActor.EditorState;
                    state.Highlighted = false;
                }
                _highlightPoint.HighlightedComponent = value;
                if (HighlightedComponent != null)
                {
                    //Engine.DebugPrint(_highlightedComponent.OwningActor.Name);
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
            //input.RegisterMouseMove(OnMouseMove, false, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, OnMouseDown, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Released, OnMouseUp, InputPauseType.TickAlways);

            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, InputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnBackInput, InputPauseType.TickAlways);
            
            input.RegisterButtonEvent(EKey.Number1, ButtonInputType.Pressed, SetTranslationMode, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Number2, ButtonInputType.Pressed, SetRotationMode, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Number3, ButtonInputType.Pressed, SetScaleMode, InputPauseType.TickAlways);
        }
        TransformType _transformType = TransformType.Translate;
        private void ToggleTransformMode()
        {
            if (_transformType == TransformType.Translate)
                _transformType = TransformType.Scale;
            else
                _transformType++;
        }
        private void SetTranslationMode() => SetMode(TransformType.Translate);
        private void SetRotationMode() => SetMode(TransformType.Rotate);
        private void SetScaleMode() => SetMode(TransformType.Scale);
        public void SetMode(TransformType type)
        {
            _transformType = type;
            if (EditorTransformTool3D.Instance != null)
                EditorTransformTool3D.Instance.TransformMode = _transformType;
        }

        protected override void OnMouseMove(float x, float y)
        {
            base.OnMouseMove(x, y);
        }

        protected void OnMouseDown()
        {
            if (!_mouseDown)
                MouseDown();
        }
        protected void OnMouseUp()
        {
            if (_mouseDown)
                MouseUp();
        }
        protected void OnGamepadSelect()
        {
            if (_mouseDown)
                MouseUp();
            else
                MouseDown();
        }
        public override void OnSpawnedPostComponentSetup(World world)
        {
            base.OnSpawnedPostComponentSetup(world);
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, HighlightScene);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, HighlightScene);
        }
        private void HighlightScene(float delta)
        {
            HighlightScene(false);
        }
        private void HighlightScene(bool gamepad)
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport;
            if (v != null && RenderPanel.HoveredPanel != null)
            {
                Vec2 viewportPoint = /*gamepad ? v.Center : */v.AbsoluteToRelative(CursorPosition);
                HighlightScene(v, viewportPoint);
            }
        }
        public void HighlightScene(Viewport v, Vec2 viewportPoint)
        {
            if (_dragComponent != null)
            {
                Vec3 hitNormal;
                SceneComponent comp;

                float prevHitDist = _hitDistance;
                IPhysicsDrivable p = _dragComponent as IPhysicsDrivable;
                if (p != null)
                    comp = v.PickScene(viewportPoint, true, true, out hitNormal, out _hitPoint, out _hitDistance, p.PhysicsDriver.CollisionObject);
                else
                    comp = v.PickScene(viewportPoint, true, true, out hitNormal, out _hitPoint, out _hitDistance);

                float upDist = 0.0f;
                Vec3 forwardCameraVector = v.Camera.GetForwardVector();
                if (comp == null)
                {
                    hitNormal = Vec3.Up;// v.Camera.GetUpVector();
                    _hitDistance = prevHitDist;
                    _hitPoint = v.Camera.WorldPoint + forwardCameraVector * _hitDistance;
                }
                else if (p != null)
                {
                    
                }

                Vec3 
                    right = forwardCameraVector ^ hitNormal, 
                    up = hitNormal,
                    forward = right ^ up,
                    translation = _hitPoint;

                right.NormalizeFast();
                up.NormalizeFast();
                forward.NormalizeFast();

                translation += up * upDist;

                _dragComponent.WorldMatrix = Matrix4.CreateSpacialTransform(translation, right, up, forward);
            }
            else if (_selectedComponent != null)
            {
                Ray cursor = v.GetWorldRay(viewportPoint);
                if (EditorTransformTool3D.Instance != null)
                {
                    if (EditorTransformTool3D.Instance.MouseMove(cursor, v.Camera, _mouseDown))
                    {
                        return;
                    }
                }
                else if (_currentConstraint != null)
                    _currentConstraint.PivotInB = cursor.StartPoint + cursor.Direction * _hitDistance;
            }
            else
            {
                if (EditorTransformTool3D.Instance != null)
                {
                    Ray cursor = v.GetWorldRay(viewportPoint);
                    if (EditorTransformTool3D.Instance.MouseMove(cursor, v.Camera, _mouseDown))
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
            _mouseDown = false;
            if (_currentConstraint != null)
            {
                Engine.World.PhysicsScene.RemoveConstraint(_currentConstraint);
                _currentConstraint.Dispose();
                _currentConstraint = null;
                _pickedBody.ForceActivationState(ActivationState.ActiveTag);
                _pickedBody = null;
            }
            _selectedComponent = null;
            _dragComponent = null;
            if (HighlightedComponent != null)
            {
                Engine.Scene.Add(_highlightPoint);
            }
        }
        public bool UseTransformTool { get; set; } = false;
        public SceneComponent DragComponent { get => _dragComponent; set => _dragComponent = value; }

        public void MouseDown()
        {
            _mouseDown = true;
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
                        _pickedBody.ForceActivationState(ActivationState.DisableDeactivation);

                        Vec3 localPivot = Vector3.TransformCoordinate(_hitPoint, Matrix.Invert(_pickedBody.CenterOfMassTransform));
                        Point2PointConstraint p2p = new Point2PointConstraint(_pickedBody, localPivot);
                        p2p.Setting.ImpulseClamp = 60;
                        p2p.Setting.Tau = 0.1f;

                        _currentConstraint = p2p;
                        Engine.World.PhysicsScene.AddConstraint(_currentConstraint);
                    }
                    else
                    {
                        if (UseTransformTool)
                            EditorTransformTool3D.GetInstance(_selectedComponent, _transformType);
                        else
                        {
                            _dragComponent = _selectedComponent;
                            _hitDistance = 20.0f;
                        }
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
            private RenderInfo3D _renderInfo = new RenderInfo3D(ERenderPassType3D.OnTopForward, null, false, false);
            public RenderInfo3D RenderInfo => _renderInfo;
            public Shape CullingVolume => null;
            public IOctreeNode OctreeNode { get; set; }

            private Material _material;

            public const int CirclePrecision = 20;
            public static readonly Color Color = Color.LimeGreen;

            private PrimitiveManager _circlePrimitive;
            private PrimitiveManager _normalPrimitive;

            public HighlightPoint()
            {
                _material = Material.GetUnlitColorMaterialForward(Color);
                _material.RenderParams.DepthTest.Enabled = false;
                _normalPrimitive = new PrimitiveManager(Segment.Mesh(Vec3.Zero, Vec3.Forward), _material);
                _circlePrimitive = new PrimitiveManager(Circle3D.WireframeMesh(1.0f, Vec3.Forward, Vec3.Zero, CirclePrecision), _material);
            }
            
            public SceneComponent HighlightedComponent { get; set; }
            public Matrix4 Transform { get; set; } = Matrix4.Identity;
            
            public void Render()
            {
                if (HighlightedComponent != null && HighlightedComponent != EditorTransformTool3D.Instance?.RootComponent)
                {
                    _circlePrimitive.Render(Transform, Matrix3.Identity);
                    _normalPrimitive.Render(Transform, Matrix3.Identity);
                }
            }
        }
    }
}