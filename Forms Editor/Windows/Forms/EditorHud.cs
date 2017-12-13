using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.UI;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Components;
using TheraEngine.Worlds.Actors.Types;
using TheraEngine.Worlds.Actors.Types.Pawns;

namespace TheraEditor.Windows.Forms
{
    public class EditorHud : UIManager
    {
        public EditorHud(Vec2 bounds) : base(bounds)
        {

        }
        
        private HighlightPoint _highlightPoint;
        RigidBody _pickedBody;
        Point2PointConstraint _currentConstraint;
        private float _hitDistance;
        private Vec3 _hitPoint;
        private float _toolSize = 1.2f;
        private SceneComponent _selectedComponent, _dragComponent;
        private bool _mouseDown;

        public bool UseTransformTool { get; set; } = true;
        public SceneComponent DragComponent { get => _dragComponent; set => _dragComponent = value; }
        public SceneComponent SelectedComponent
        {
            get => _selectedComponent;
            set
            {
                if (_selectedComponent == value)
                    return;

                PreSelectedComponentChanged();
                _selectedComponent = value;
                PostSelectedComponentChanged();
            }
        }
        public SceneComponent HighlightedComponent
        {
            get => _highlightPoint.HighlightedComponent;
            set
            {
                if (HighlightedComponent == value)
                    return;

                if (value == null && HighlightedComponent != null)
                {
                    Engine.Scene.Remove(_highlightPoint);
                    BaseRenderPanel.CheckedInvoke(new Action(() => 
                    {
                        if (BaseRenderPanel.CapturedPanel != null)
                        {
                            BaseRenderPanel.CapturedPanel.Cursor = Cursors.Default;
                        }
                    }),
                    BaseRenderPanel.PanelType.Captured);
                }
                else if (value != null && HighlightedComponent == null)
                {
                    Engine.Scene.Add(_highlightPoint);
                    BaseRenderPanel.CheckedInvoke(new Action(() =>
                    {
                        if (BaseRenderPanel.CapturedPanel != null)
                        {
                            BaseRenderPanel.CapturedPanel.Cursor = Cursors.Hand;
                        }
                    }),
                    BaseRenderPanel.PanelType.Captured);
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
        protected override UIDockableComponent OnConstruct()
        {
            return base.OnConstruct();
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
            input.RegisterButtonEvent(EKey.Number4, ButtonInputType.Pressed, SetDragDropMode, InputPauseType.TickAlways);
        }
        TransformType _transformType = TransformType.Translate;
        private void ToggleTransformMode()
        {
            if (_transformType == TransformType.DragDrop)
                _transformType = TransformType.Scale;
            else
                _transformType++;
        }
        private void SetTranslationMode() => SetMode(TransformType.Translate);
        private void SetRotationMode() => SetMode(TransformType.Rotate);
        private void SetScaleMode() => SetMode(TransformType.Scale);
        private void SetDragDropMode() => SetMode(TransformType.DragDrop);
        public void SetMode(TransformType type)
        {
            _transformType = type;
            if (UseTransformTool = _transformType != TransformType.DragDrop)
            {
                if (EditorTransformTool3D.Instance != null)
                    EditorTransformTool3D.Instance.TransformMode = _transformType;
            }
            else
            {
                if (EditorTransformTool3D.Instance != null)
                    EditorTransformTool3D.DestroyInstance();
            }
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
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, MouseMove);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, MouseMove);
        }
        private void MouseMove(float delta)
        {
            MouseMove(false);
        }
        private void MouseMove(bool gamepad)
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport;
            if (v != null && BaseRenderPanel.HoveredPanel != null)
            {
                Vec2 viewportPoint = /*gamepad ? v.Center : */v.AbsoluteToRelative(CursorPosition);
                MouseMove(v, viewportPoint);
            }
        }
        public void MouseMove(Viewport v, Vec2 viewportPoint)
        {
            if (EditorTransformTool3D.Instance != null && EditorTransformTool3D.Instance.IsSpawned)
            {
                Ray cursor = v.GetWorldRay(viewportPoint);
                if (EditorTransformTool3D.Instance.MouseMove(cursor, v.Camera, _mouseDown))
                {
                    if (!_mouseDown)
                        HighlightedComponent = EditorTransformTool3D.Instance.RootComponent;

                    return;
                }
                else
                {
                    SceneComponent comp = v.PickScene(viewportPoint, true, true, out Vec3 hitNormal, out _hitPoint, out _hitDistance);
                    _highlightPoint.Transform = Matrix4.CreateTranslation(_hitPoint) * hitNormal.LookatAngles().GetMatrix() * Matrix4.CreateScale(OwningPawn.LocalPlayerController.Viewport.Camera.DistanceScale(_hitPoint, _toolSize));
                    HighlightedComponent = comp;
                }
            }
            else if (_currentConstraint != null)
            {
                Ray cursor = v.GetWorldRay(viewportPoint);
                _currentConstraint.PivotInB = cursor.StartPoint + cursor.Direction * _hitDistance;
            }
            else if (_dragComponent != null)
            {
                float prevHitDist = _hitDistance;
                IPhysicsDrivable p = _dragComponent as IPhysicsDrivable;
                SceneComponent comp = v.PickScene(viewportPoint, true, true, out Vec3 hitNormal, out _hitPoint, out _hitDistance, p != null ? new RigidBody[] { p.PhysicsDriver.CollisionObject } : new RigidBody[0]);

                float upDist = 0.0f;
                if (comp == null)
                {
                    hitNormal = Vec3.Up;// v.Camera.GetUpVector();
                    _hitDistance = prevHitDist;
                    Vec3 forwardCameraVector = v.Camera.GetForwardVector();
                    _hitPoint = v.Camera.WorldPoint + forwardCameraVector * _hitDistance;
                }
                else if (p != null)
                {

                }

                Vec3 rightCameraVector = v.Camera.GetRightVector();
                Vec3
                    forward = rightCameraVector ^ hitNormal,
                    up = hitNormal,
                    right = up ^ forward,
                    translation = _hitPoint;

                right.NormalizeFast();
                up.NormalizeFast();
                forward.NormalizeFast();

                translation += up * upDist;

                _dragComponent.WorldMatrix = Matrix4.CreateSpacialTransform(translation, right, up, forward);
            }
            else if (_selectedComponent == null)
            {
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
                Engine.Scene.Add(_highlightPoint);
        }
        private void PreSelectedComponentChanged()
        {

        }
        private void PostSelectedComponentChanged()
        {
            if (_selectedComponent != null)
            {
                Engine.Scene.Remove(_highlightPoint);
                if (_selectedComponent.OwningActor is EditorTransformTool3D tool)
                {

                }
                else if (_selectedComponent is UIComponent hudComp)
                {

                }
                else// if (comp != null)
                {
                    if (_selectedComponent is IPhysicsDrivable d && d.PhysicsDriver != null && d.PhysicsDriver.SimulatingPhysics)
                    {
                        _dragComponent = null;
                        EditorTransformTool3D.DestroyInstance();

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
                            EditorTransformTool3D.DestroyInstance();
                            _dragComponent = _selectedComponent;
                            _hitDistance = 20.0f;
                        }
                    }

                    TreeNode t = _selectedComponent.OwningActor.EditorState.TreeNode;
                    if (t != null)
                    {
                        if (t.TreeView.InvokeRequired)
                            t.TreeView.Invoke((Action)(() => t.TreeView.SelectedNode = t));
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
        public void MouseDown()
        {
            _mouseDown = true;
            SelectedComponent = HighlightedComponent;
        }
        public class HighlightPoint : I3DRenderable
        {
            private RenderInfo3D _renderInfo = new RenderInfo3D(ERenderPass3D.OnTopForward, null, false, false);

            public const int CirclePrecision = 20;
            public static readonly Color Color = Color.LimeGreen;

            public RenderInfo3D RenderInfo => _renderInfo;
            public Shape CullingVolume => null;
            public IOctreeNode OctreeNode { get; set; }
            public SceneComponent HighlightedComponent { get; set; }
            public Matrix4 Transform { get; set; } = Matrix4.Identity;

            private TMaterial _material;
            private PrimitiveManager _circlePrimitive;
            private PrimitiveManager _normalPrimitive;

            public HighlightPoint()
            {
                _material = TMaterial.GetUnlitColorMaterialForward(Color);
                _material.RenderParams.File.DepthTest.Enabled = false;
                _normalPrimitive = new PrimitiveManager(Segment.Mesh(Vec3.Zero, Vec3.Forward), _material);
                _circlePrimitive = new PrimitiveManager(Circle3D.WireframeMesh(1.0f, Vec3.Forward, Vec3.Zero, CirclePrecision), _material);
            }
            
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