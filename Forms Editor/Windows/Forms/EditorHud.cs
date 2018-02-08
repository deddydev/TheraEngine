using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.UI;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds;
using TheraEngine.Components;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Physics;
using TheraEngine.Editor;
using TheraEngine.Rendering.Cameras;

namespace TheraEditor.Windows.Forms
{
    public class EditorHud : UIManager
    {
        public EditorHud(Vec2 bounds) : base(bounds)
        {

        }
        
        private HighlightPoint _highlightPoint;
        TRigidBody _pickedBody;
        TPointPointConstraint _currentConstraint;
        private float _draggingTestDistance;
        private Vec3 _hitPoint;
        private float _toolSize = 1.2f;
        private SceneComponent _selectedComponent, _dragComponent;
        private bool MouseDown { get; set; }//=> OwningPawn.LocalPlayerController.Input.Mouse.LeftClick.IsPressed;
        
        public bool UseTransformTool { get; set; } = true;
        public SceneComponent DragComponent
        {
            get => _dragComponent;
            set => _dragComponent = value;
        }
        public SceneComponent SelectedComponent => _selectedComponent;
        public void SetSelectedComponent(bool selectedByViewport, SceneComponent comp)
        {
            if (_selectedComponent == comp)
                return;

            PreSelectedComponentChanged(selectedByViewport);
            _selectedComponent = comp;
            PostSelectedComponentChanged(selectedByViewport);
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
                    if (TransformTool3D.Instance == null || (TransformTool3D.Instance != null && HighlightedComponent != TransformTool3D.Instance.RootComponent))
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
                    if (TransformTool3D.Instance == null || (TransformTool3D.Instance != null && value != TransformTool3D.Instance.RootComponent))
                        OwningWorld.Scene.Add(_highlightPoint);
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
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, OnMouseDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Released, OnMouseUp, EInputPauseType.TickAlways);

            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnBackInput, EInputPauseType.TickAlways);
            
            input.RegisterButtonEvent(EKey.Number1, ButtonInputType.Pressed, SetTranslationMode, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Number2, ButtonInputType.Pressed, SetRotationMode, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Number3, ButtonInputType.Pressed, SetScaleMode, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Number4, ButtonInputType.Pressed, SetDragDropMode, EInputPauseType.TickAlways);
            
            input.RegisterButtonEvent(EKey.Number5, ButtonInputType.Pressed, SetWorldSpace, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Number6, ButtonInputType.Pressed, SetParentSpace, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Number7, ButtonInputType.Pressed, SetLocalSpace, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Number8, ButtonInputType.Pressed, SetScreenSpace, EInputPauseType.TickAlways);

            //void SetAlt(bool set) => _modifierKeys = _modifierKeys.SetBit(0, set);
            //void SetCtrl(bool set) => _modifierKeys = _modifierKeys.SetBit(1, set);
            //void SetShift(bool set) => _modifierKeys = _modifierKeys.SetBit(2, set);

            //input.RegisterButtonPressed(EKey.AltLeft, SetAlt, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.AltRight, SetAlt, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.LAlt, SetAlt, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.RAlt, SetAlt, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.ControlLeft, SetCtrl, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.ControlRight, SetCtrl, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.LControl, SetCtrl, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.RControl, SetCtrl, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.ShiftLeft, SetShift, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.ShiftRight, SetShift, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.LShift, SetShift, EInputPauseType.TickAlways);
            //input.RegisterButtonPressed(EKey.RShift, SetShift, EInputPauseType.TickAlways);

            input.RegisterMouseScroll(OnMouseScroll, EInputPauseType.TickAlways);
        }
        //private byte _modifierKeys;
        //public byte ModifierKeys => _modifierKeys;
        //public const byte AltKey = 0b001;
        //public const byte CtrlKey = 0b010;
        //public const byte ShiftKey = 0b100;
        //public bool OnlyAltPressed => _modifierKeys == AltKey;
        //public bool OnlyCtrlPressed => _modifierKeys == CtrlKey;
        //public bool OnlyShiftPressed => _modifierKeys == ShiftKey;
        //public bool AtLeastAltPressed => (_modifierKeys & AltKey) != 0;
        //public bool AtLeastCtrlPressed => (_modifierKeys & CtrlKey) != 0;
        //public bool AtLeastShiftPressed => (_modifierKeys & ShiftKey) != 0;

        private float _draggingUniformScale = 1.0f;
        private float _spawnRotation = 0.0f;
        private void OnMouseScroll(bool up)
        {
            if (_dragComponent != null)
            {
                if (Control.ModifierKeys == Keys.Alt)
                    _draggingTestDistance *= up ? 1.2f : 0.8f;
                if (Control.ModifierKeys == Keys.Shift)
                    _draggingUniformScale *= up ? 1.2f : 0.8f;
                if (Control.ModifierKeys == Keys.Control)
                    _spawnRotation += up ? 5.0f : -5.0f;
            }
        }
        private void SetWorldSpace()
        {
            TransformTool3D.Instance.TransformSpace = ESpace.World;
        }
        private void SetLocalSpace()
        {
            TransformTool3D.Instance.TransformSpace = ESpace.Local;
        }
        private void SetScreenSpace()
        {
            TransformTool3D.Instance.TransformSpace = ESpace.Screen;
        }
        private void SetParentSpace()
        {
            TransformTool3D.Instance.TransformSpace = ESpace.Parent;
        }

        TransformType _transformType = TransformType.Translate;
        private void ToggleTransformMode()
        {
            if (_transformType == TransformType.DragDrop)
                _transformType = TransformType.Scale;
            else
                ++_transformType;
        }
        private void SetTranslationMode() => TransformMode = TransformType.Translate;
        private void SetRotationMode() => TransformMode = TransformType.Rotate;
        private void SetScaleMode() => TransformMode = TransformType.Scale;
        private void SetDragDropMode() => TransformMode = TransformType.DragDrop;

        public TransformType TransformMode
        {
            get => _transformType;
            set
            {
                _transformType = value;
                if (UseTransformTool = _transformType != TransformType.DragDrop)
                    TransformTool3D.Instance.TransformMode = _transformType;
                else
                    TransformTool3D.DestroyInstance();
            }
        }
        
        protected void OnMouseDown()
        {
            if (!MouseDown)
                DoMouseDown();
        }
        protected void OnMouseUp()
        {
            if (MouseDown)
                DoMouseUp();
        }
        protected void OnGamepadSelect()
        {
            if (MouseDown)
                DoMouseUp();
            else
                DoMouseDown();
        }
        public override void OnSpawnedPostComponentSetup()
        {
            base.OnSpawnedPostComponentSetup();
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
            if (TransformTool3D.Instance.IsSpawned)
            {
                Ray cursor = v.GetWorldRay(viewportPoint);
                if (TransformTool3D.Instance.MouseMove(cursor, v.Camera, MouseDown))
                {
                    if (!MouseDown)
                        HighlightedComponent = TransformTool3D.Instance.RootComponent;
                }
                else
                {
                    SceneComponent comp = v.PickScene(viewportPoint, true, true, out Vec3 hitNormal, out _hitPoint, out _draggingTestDistance);
                    _highlightPoint.Transform = Matrix4.CreateTranslation(_hitPoint) * hitNormal.LookatAngles().GetMatrix() * Matrix4.CreateScale(OwningPawn.LocalPlayerController.Viewport.Camera.DistanceScale(_hitPoint, _toolSize));
                    HighlightedComponent = comp;
                }
            }
            else if (_currentConstraint != null)
            {
                Ray cursor = v.GetWorldRay(viewportPoint);
                _currentConstraint.PivotInB = cursor.StartPoint + cursor.Direction * _draggingTestDistance;
            }
            else if (_dragComponent != null)
            {
                float prevHitDist = _draggingTestDistance;
                IRigidCollidable p = _dragComponent as IRigidCollidable;
                SceneComponent comp = v.PickScene(viewportPoint, true, true, out Vec3 hitNormal, out _hitPoint, out _draggingTestDistance, p != null ? new TRigidBody[] { p.RigidBodyCollision } : new TRigidBody[0]);

                float upDist = 0.0f;
                if (comp == null)
                {
                    _draggingTestDistance = prevHitDist;
                    hitNormal = Vec3.Up;// v.Camera.GetUpVector();
                    float depth = TMath.DistanceToDepth(_draggingTestDistance, v.Camera.NearZ, v.Camera.FarZ);
                    _hitPoint = v.ScreenToWorld(v.ToInternalResCoords(viewportPoint), depth);
                    //Vec3 forwardCameraVector = v.Camera.GetForwardVector();
                    //_hitPoint = v.Camera.WorldPoint + forwardCameraVector * _hitDistance;
                }
                else if (p != null)
                {

                }

                Vec3 rightCameraVector = v.Camera.RightVector;
                Quat rotation = Quat.FromAxisAngle(hitNormal, _spawnRotation);
                rightCameraVector = rotation * rightCameraVector;
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
                SceneComponent comp = v.PickScene(viewportPoint, true, true, out Vec3 hitNormal, out _hitPoint, out _draggingTestDistance);
                _highlightPoint.Transform = Matrix4.CreateTranslation(_hitPoint) * hitNormal.LookatAngles().GetMatrix() * Matrix4.CreateScale(OwningPawn.LocalPlayerController.Viewport.Camera.DistanceScale(_hitPoint, _toolSize));
                HighlightedComponent = comp;
            }
        }
        public void DoMouseUp()
        {
            MouseDown = false;
            if (_currentConstraint != null)
            {
                OwningWorld.PhysicsWorld.RemoveConstraint(_currentConstraint);
                //_currentConstraint.Dispose();
                _currentConstraint = null;
                _pickedBody.ForceActivationState(EBodyActivationState.Active);
                _pickedBody = null;
            }

            //_selectedComponent = null;
            _dragComponent = null;
            if (HighlightedComponent != null)
                OwningWorld.Scene.Add(_highlightPoint);
        }
        private void PreSelectedComponentChanged(bool selectedByViewport)
        {

        }
        private void PostSelectedComponentChanged(bool selectedByViewport)
        {
            if (OwningWorld == null)
                return;
            
            if (_selectedComponent != null)
            {
                OwningWorld.Scene?.Remove(_highlightPoint);
                if (_selectedComponent.OwningActor is TransformTool3D tool)
                {

                }
                else if (_selectedComponent is UIComponent hudComp)
                {

                }
                else// if (comp != null)
                {
                    if (_selectedComponent is IRigidCollidable d &&
                        d.RigidBodyCollision != null &&
                        d.RigidBodyCollision.SimulatingPhysics &&
                        !Engine.IsPaused)
                    {
                        _dragComponent = null;
                        TransformTool3D.DestroyInstance();

                        _pickedBody = d.RigidBodyCollision;
                        _pickedBody.ForceActivationState(EBodyActivationState.DisableSleep);

                        Vec3 localPivot = Vec3.TransformPosition(_hitPoint, _pickedBody.CenterOfMassTransform.Inverted());
                        TPointPointConstraint p2p = TPointPointConstraint.New(_pickedBody, localPivot);
                        p2p.ImpulseClamp = 60;
                        p2p.Tau = 0.1f;

                        _currentConstraint = p2p;
                        OwningWorld.PhysicsWorld.AddConstraint(_currentConstraint);
                    }
                    else
                    {
                        if (UseTransformTool)
                            TransformTool3D.GetInstance(_selectedComponent, _transformType);
                        else
                        {
                            TransformTool3D.DestroyInstance();
                            if (selectedByViewport)
                            {
                                _dragComponent = _selectedComponent;
                                if (_dragComponent != null)
                                {
                                    Camera c = OwningPawn?.LocalPlayerController?.Viewport?.Camera;
                                    _draggingTestDistance =/* c != null ? c.DistanceFromScreenPlane(_dragComponent.GetWorldPoint()) : */20.0f;
                                }
                            }
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
                TransformTool3D.DestroyInstance();
            }
        }
        public void DoMouseDown()
        {
            MouseDown = true;
            SetSelectedComponent(true, HighlightedComponent);
        }
        public class HighlightPoint : I3DRenderable
        {
            public const int CirclePrecision = 20;
            public static readonly Color Color = Color.LimeGreen;

            public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass3D.OnTopForward, null, false, false);
            public Shape CullingVolume => null;
            public IOctreeNode OctreeNode { get; set; }
            public SceneComponent HighlightedComponent { get; set; }
            public Matrix4 Transform { get; set; } = Matrix4.Identity;

            private TMaterial _material;
            private PrimitiveManager _circlePrimitive;
            private PrimitiveManager _normalPrimitive;

            public HighlightPoint()
            {
                _material = TMaterial.CreateUnlitColorMaterialForward(Color);
                _material.RenderParamsRef.File.DepthTest.Enabled = false;
                _normalPrimitive = new PrimitiveManager(Segment.Mesh(Vec3.Zero, Vec3.Forward), _material);
                _circlePrimitive = new PrimitiveManager(Circle3D.WireframeMesh(1.0f, Vec3.Forward, Vec3.Zero, CirclePrecision), _material);
            }
            
            public void Render()
            {
                if (HighlightedComponent != null && HighlightedComponent != TransformTool3D.Instance?.RootComponent)
                {
                    _circlePrimitive.Render(Transform, Matrix3.Identity);
                    _normalPrimitive.Render(Transform, Matrix3.Identity);
                }
            }
        }
    }
}