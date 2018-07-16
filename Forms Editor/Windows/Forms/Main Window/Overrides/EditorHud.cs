using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheraEditor.Actors.Types.Pawns;
using TheraEngine;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;
using TheraEngine.Rendering.UI;

namespace TheraEditor.Windows.Forms
{
    public class EditorHud : UIManager<UIDockableComponent>
    {
        public EditorHud(Vec2 bounds) : base(bounds)
        {
            TransformTool3D.Instance.MouseDown += Instance_MouseDown;
            TransformTool3D.Instance.MouseUp += Instance_MouseUp;
        }

        private void Instance_MouseUp()
        {
            ISocket socket = TransformTool3D.Instance.TargetSocket;
            if (TransformTool3D.Instance.PrevRootWorldMatrix != socket.WorldMatrix)
            {
                Editor.Instance.UndoManager.AddChange(
                    ((TObject)socket).EditorState,
                    TransformTool3D.Instance.PrevRootWorldMatrix,
                    socket.WorldMatrix,
                    socket,
                    socket.GetType().GetProperty(nameof(ISocket.WorldMatrix)));
            }
        }

        private void Instance_MouseDown()
        {

        }

        [Browsable(false)]
        public float DraggingTestDistance { get; set; } = 20.0f;

        //private Vec3 _lastHitPoint;
        private HighlightPoint _highlightPoint;
        TRigidBody _pickedBody;
        TPointPointConstraint _currentConstraint;
        private Vec3 _hitPoint;
        private float _toolSize = 1.2f;
        private static Dictionary<int, StencilTest>
            _highlightedMaterials = new Dictionary<int, StencilTest>(),
            _selectedMaterials = new Dictionary<int, StencilTest>();
        internal bool MouseDown { get; set; }//=> OwningPawn.LocalPlayerController.Input.Mouse.LeftClick.IsPressed;

        public bool UseTransformTool => _transformType != TransformType.DragDrop;
        public SceneComponent DragComponent { get; set; }

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
                        _highlightPoint.Visible = false;
                    BaseRenderPanel p = BaseRenderPanel.FocusedPanel;
                    p?.BeginInvoke((Action)(() => p.Cursor = Cursors.Default));
                }
                else if (value != null && HighlightedComponent == null)
                {
                    if (TransformTool3D.Instance == null || (TransformTool3D.Instance != null && value != TransformTool3D.Instance.RootComponent))
                        _highlightPoint.Visible = true;
                    BaseRenderPanel p = BaseRenderPanel.FocusedPanel;
                    p?.BeginInvoke((Action)(() => p.Cursor = Cursors.Hand));
                }

                if (HighlightedComponent != null)
                {
                    //EditorState state = HighlightedComponent.OwningActor.EditorState;
                    //state.Highlighted = false;
                    HighlightedComponentChanged(false);
                }
                _highlightPoint.HighlightedComponent = value;
                if (HighlightedComponent != null)
                {
                    //Engine.DebugPrint(_highlightedComponent.OwningActor.Name);
                    //EditorState state = HighlightedComponent.OwningActor.EditorState;
                    //state.Highlighted = true;
                    HighlightedComponentChanged(true);
                }
            }
        }
        private void HighlightedComponentChanged(bool highlighted)
        {
            if (HighlightedComponent is StaticMeshComponent staticMesh)
            {
                var meshes = staticMesh.Meshes;
                if (meshes != null)
                    foreach (StaticRenderableMesh m in staticMesh.Meshes)
                    {
                        foreach (var lod in m.LODs)
                        {
                            var tris = lod.Manager.Data.Triangles;
                            if (tris != null && tris.Count > 0)
                                UpdateMatHighlight(lod.Manager.Material, highlighted);
                        }
                    }
            }
            else if (HighlightedComponent is SkeletalMeshComponent skeletalMesh)
            {
                var meshes = skeletalMesh.Meshes;
                if (meshes != null)
                    foreach (SkeletalRenderableMesh m in skeletalMesh.Meshes)
                    {
                        foreach (var lod in m.LODs)
                        {
                            var tris = lod.Manager.Data.Triangles;
                            if (tris != null && tris.Count > 0)
                                UpdateMatHighlight(lod.Manager.Material, highlighted);
                        }
                    }
            }
            else if (HighlightedComponent is LandscapeComponent landscape)
            {
                UpdateMatHighlight(landscape.Material, highlighted);
            }
        }
        private void UpdateMatHighlight(TMaterial m, bool highlighted)
        {
            if (m == null)
                return;
            if (highlighted)
            {
                if (_highlightedMaterials.ContainsKey(m.UniqueID))
                {
                    //m.RenderParams.StencilTest.BackFace.Ref |= 1;
                    //m.RenderParams.StencilTest.FrontFace.Ref |= 1;
                    return;
                }
                _highlightedMaterials.Add(m.UniqueID, m.RenderParams.StencilTest);
                m.RenderParams.StencilTest = OutlinePassStencil;
            }
            else
            {
                if (!_highlightedMaterials.ContainsKey(m.UniqueID))
                {
                    //m.RenderParams.StencilTest.BackFace.Ref &= ~1;
                    //m.RenderParams.StencilTest.FrontFace.Ref &= ~1;
                    return;
                }
                StencilTest t = _highlightedMaterials[m.UniqueID];
                _highlightedMaterials.Remove(m.UniqueID);
                m.RenderParams.StencilTest = _selectedMaterials.ContainsKey(m.UniqueID) ? _selectedMaterials[m.UniqueID] : t;
            }
        }
        public static StencilTest OutlinePassStencil = new StencilTest()
        {
            Enabled = ERenderParamUsage.Enabled,
            BackFace = new StencilTestFace()
            {
                BothFailOp = EStencilOp.Keep,
                StencilPassDepthFailOp = EStencilOp.Replace,
                BothPassOp = EStencilOp.Replace,
                Func = EComparison.Always,
                Ref = 2,
                WriteMask = 0xFF,
                ReadMask = 0xFF,
            },
            FrontFace = new StencilTestFace()
            {
                BothFailOp = EStencilOp.Keep,
                StencilPassDepthFailOp = EStencilOp.Replace,
                BothPassOp = EStencilOp.Replace,
                Func = EComparison.Always,
                Ref = 1,
                WriteMask = 0xFF,
                ReadMask = 0xFF,
            },
        };
        public UIViewportComponent SubViewport { get; private set; }
        public UITextComponent SubViewportText { get; private set; }
        //public UITextProjectionComponent TextOverlay { get; private set; }
        protected override UIDockableComponent OnConstruct()
        {
            UIDockableComponent dock = new UIDockableComponent()
            {
                DockStyle = UIDockStyle.Fill,
            };

            SubViewport = new UIViewportComponent();
            SubViewport.SizeableWidth.Minimum = new SizeableElement();
            SubViewport.SizeableWidth.Minimum.SetSizingPixels(200.0f, true, ParentBoundsInheritedValue.Width);
            SubViewport.SizeableWidth.SetSizingPercentageOfParent(0.4f, true, ParentBoundsInheritedValue.Width);
            SubViewport.SizeableHeight.SetSizingProportioned(SubViewport.SizeableWidth, 9.0f / 16.0f, true, ParentBoundsInheritedValue.Height);
            SubViewport.SizeablePosX.SetSizingPercentageOfParent(0.02f, true, ParentBoundsInheritedValue.Width);
            SubViewport.SizeablePosY.SetSizingPercentageOfParent(0.02f, true, ParentBoundsInheritedValue.Height);
            dock.ChildComponents.Add(SubViewport);

            Font f = new Font("Segoe UI", 10.0f, FontStyle.Regular);
            string t = "Selected Camera View";
            Size s = TextRenderer.MeasureText(t, f);
            SubViewportText = new UITextComponent
            {
                DockStyle = UIDockStyle.Top,
                Height = s.Height
            };
            SubViewportText.SizeableHeight.Minimum = SizeableElement.Pixels(s.Height, true, ParentBoundsInheritedValue.Height);
            SubViewportText.SizeableWidth.Minimum = SizeableElement.Pixels(s.Width, true, ParentBoundsInheritedValue.Width);
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
            {
                //Alignment = StringAlignment.Center,
                //LineAlignment = StringAlignment.Near
            };
            SubViewportText.TextDrawer.Add(true, new UIString2D()
            {
                Font = f,
                Format = sf,
                Text = t,
                TextColor = new ColorF4(1.0f),
                //OriginPercentages = new Vec2(0.0f, 1.0f),
                //Position = new Vec2(0.0f, 0.0f),
            });
            SubViewport.ChildComponents.Add(SubViewportText);

            //TextOverlay = new UITextProjectionComponent()
            //{
            //    TexScale = new Vec2(1.0f),
            //    DockStyle = HudDockStyle.Fill,
            //};
            //dock.ChildComponents.Add(TextOverlay);

            return dock;
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
            if (DragComponent != null)
            {
                if (Control.ModifierKeys == (Keys.Shift | Keys.Alt))
                    _spawnRotation += up ? 5.0f : -5.0f;
                else if (Control.ModifierKeys == Keys.Alt)
                {
                    DraggingTestDistance *= up ? 0.8f : 1.2f;
                    DraggingTestDistance.ClampMin(0.0f);
                }
                else if (Control.ModifierKeys == Keys.Shift)
                {
                    _draggingUniformScale += up ? -0.05f : 0.05f;
                    _draggingUniformScale.ClampMin(0.0f);
                }
            }
        }

        private void SetWorldSpace() => TransformTool3D.Instance.TransformSpace = ESpace.World;
        private void SetLocalSpace() => TransformTool3D.Instance.TransformSpace = ESpace.Local;
        private void SetScreenSpace() => TransformTool3D.Instance.TransformSpace = ESpace.Screen;
        private void SetParentSpace() => TransformTool3D.Instance.TransformSpace = ESpace.Parent;
        
        private TransformType _transformType = TransformType.Translate;
        private Matrix4 _prevDragMatrix = Matrix4.Identity;

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
                if (UseTransformTool)
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
            OwningWorld.Scene.Add(_highlightPoint);

            PerspectiveCameraActor c = new PerspectiveCameraActor();
            c.Camera.TranslateAbsolute(0.0f, 20.0f, 0.0f);
            OwningWorld.SpawnActor(c);

            SubViewport.IsVisible = false;
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, MouseMove);
            OwningWorld.Scene.Remove(_highlightPoint);
        }
        private void MouseMove(float delta)
        {
            MouseMove(false);
        }
        private void MouseMove(bool gamepad)
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport;
            if (v != null)
            {
                Vec2 viewportPoint = /*gamepad ? v.Center : */CursorPosition(v);
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
                    HighlightScene(v, viewportPoint);
                }
            }
            else if (_currentConstraint != null)
            {
                Ray cursor = v.GetWorldRay(viewportPoint);
                _currentConstraint.PivotInB = cursor.StartPoint + cursor.Direction * DraggingTestDistance;
            }
            else if (DragComponent != null)
            {
                IRigidBodyCollidable p = DragComponent as IRigidBodyCollidable;
                SceneComponent comp = v.PickScene(viewportPoint, true, true, out Vec3 hitNormal, out _hitPoint, out float dist, p != null ? new TRigidBody[] { p.RigidBodyCollision } : new TRigidBody[0]);

                if (dist > DraggingTestDistance)
                    comp = null;

                float upDist = 0.0f;
                if (comp == null)
                {
                    hitNormal = Vec3.Up;
                    float depth = TMath.DistanceToDepth(DraggingTestDistance, v.Camera.NearZ, v.Camera.FarZ);
                    _hitPoint = v.ScreenToWorld(viewportPoint, depth);
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

                DragComponent.WorldMatrix = Matrix4.CreateSpacialTransform(
                    translation,
                    right * _draggingUniformScale,
                    up * _draggingUniformScale,
                    forward * _draggingUniformScale);
            }
            else //Nothing selected
            {
                HighlightScene(v, viewportPoint);
            }
        }
        private void HighlightScene(Viewport v, Vec2 viewportPoint)
        {
            Camera c = v.Camera;
            if (c == null)
                return;

            //Test against HUD
            SceneComponent comp = v.PickScene(viewportPoint, true, false, out Vec3 hitNormal, out _hitPoint, out float dist);
            if (comp == null)
            {
                EditorCameraPawn pawn = OwningPawn as EditorCameraPawn;
                if (pawn.HasHit)
                {
                    hitNormal = pawn.HitNormal;
                    _hitPoint = pawn.HitPoint;
                    dist = pawn.HitDistance;
                    comp = pawn.HitObject.Owner as SceneComponent;
                }
            }

            //Vec3 lerpHitPoint = Vec3.Lerp(_lastHitPoint, _hitPoint, 0.2f);
            _highlightPoint.Transform =
                Matrix4.CreateTranslation(_hitPoint) *
                hitNormal.LookatAngles().GetMatrix() *
                Matrix4.CreateScale(c.DistanceScale(_hitPoint, _toolSize));
            //_lastHitPoint = lerpHitPoint;

            HighlightedComponent = comp;
        }
        private bool IsSimulatedBody(out IRigidBodyCollidable body)
        {
            if (SelectedComponent is IRigidBodyCollidable d &&
                d.RigidBodyCollision != null &&
                d.RigidBodyCollision.SimulatingPhysics &&
                !d.RigidBodyCollision.IsKinematic &&
                !Engine.IsPaused)
            {
                body = d;
                return true;
            }
            else
            {
                body = null;
                return false;
            }
        }
        public void DoMouseDown()
        {
            MouseDown = true;
            SetSelectedComponent(true, HighlightedComponent);
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

            if (DragComponent != null)
            {
                Editor.Instance.UndoManager.AddChange(DragComponent.EditorState, _prevDragMatrix, DragComponent.WorldMatrix, DragComponent, DragComponent.GetType().GetProperty(nameof(DragComponent.WorldMatrix)));
                //_selectedComponent = null;
                DragComponent = null;
            }

            _highlightPoint.Visible = HighlightedComponent != null;
        }
        public SceneComponent SelectedComponent { get; private set; }

        public void SetSelectedComponent(bool selectedByViewport, SceneComponent comp)
        {
            if (SelectedComponent == comp)
                return;

            PreSelectedComponentChanged(selectedByViewport);
            SelectedComponent = comp;
            PostSelectedComponentChanged(selectedByViewport);
        }
        private void PreSelectedComponentChanged(bool selectedByViewport)
        {

        }
        private void PostSelectedComponentChanged(bool selectedByViewport)
        {
            DragComponent = null;

            if (SelectedComponent is CameraComponent cam)
            {
                SubViewport.ViewportCamera = cam.Camera;
                SubViewport.IsVisible = true;
            }
            else
            {
                SubViewport.ViewportCamera = null;
                SubViewport.IsVisible = false;
            }

            if (SelectedComponent != null)
            {
                if (OwningWorld == null)
                    return;

                _highlightPoint.Visible = false;
                if (SelectedComponent.OwningActor is TransformTool3D tool)
                {

                }
                else if (SelectedComponent is UIComponent hudComp)
                {

                }
                else// if (comp != null)
                {
                    if (SelectedComponent is IRigidBodyCollidable d &&
                        d.RigidBodyCollision != null &&
                        d.RigidBodyCollision.SimulatingPhysics &&
                        !d.RigidBodyCollision.IsKinematic &&
                        !Engine.IsPaused)
                    {
                        DragComponent = null;
                        TransformTool3D.DestroyInstance();

                        _pickedBody = d.RigidBodyCollision;
                        _pickedBody.ForceActivationState(EBodyActivationState.DisableSleep);

                        Vec3 localPivot = Vec3.TransformPosition(_hitPoint, _pickedBody.CenterOfMassTransform.Inverted());

                        _currentConstraint = TPointPointConstraint.New(_pickedBody, localPivot);
                        _currentConstraint.ImpulseClamp = 60;
                        _currentConstraint.Tau = 0.6f;

                        OwningWorld.PhysicsWorld.AddConstraint(_currentConstraint);
                    }
                    else
                    {
                        if (UseTransformTool)
                        {
                            TransformTool3D.GetInstance(SelectedComponent, _transformType);
                        }
                        else
                        {
                            TransformTool3D.DestroyInstance();

                            DragComponent = SelectedComponent;
                            if (DragComponent != null)
                            {
                                _prevDragMatrix = DragComponent.WorldMatrix;
                                Camera c = OwningPawn?.LocalPlayerController?.Viewport?.Camera;
                                DraggingTestDistance = c != null ? c.DistanceFromScreenPlane(DragComponent.WorldPoint) : DraggingTestDistance;
                            }
                        }
                    }
                    TreeNode t = SelectedComponent.OwningActor.EditorState.TreeNode;
                    if (t != null)
                    {
                        if (t.TreeView.InvokeRequired)
                            t.TreeView.BeginInvoke((Action)(() => t.TreeView.SelectedNode = t));
                        else
                            t.TreeView.SelectedNode = t;
                    }
                }
            }
            //else
            //{
            //    SubViewport.IsVisible = false;
            //    TransformTool3D.DestroyInstance();
            //}
        }
        public class HighlightPoint : I3DRenderable
        {
            public const int CirclePrecision = 20;
            public static readonly Color Color = Color.LimeGreen;

            public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass.OnTopForward, false, false);
            public Shape CullingVolume => null;
            public IOctreeNode OctreeNode { get; set; }
            public SceneComponent HighlightedComponent { get; set; }
            public Matrix4 Transform { get; set; } = Matrix4.Identity;
            public bool Visible { get; set; }
            public bool VisibleInEditorOnly { get; set; } = true;
            public bool HiddenFromOwner { get; set; } = false;
            public bool VisibleToOwnerOnly { get; set; } = false;

            private TMaterial _material;
            private PrimitiveManager _circlePrimitive;
            private PrimitiveManager _normalPrimitive;

            public HighlightPoint()
            {
                _material = TMaterial.CreateUnlitColorMaterialForward(Color);
                _material.RenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
                _normalPrimitive = new PrimitiveManager(Segment.Mesh(Vec3.Zero, Vec3.Forward), _material);
                _circlePrimitive = new PrimitiveManager(Circle3D.WireframeMesh(1.0f, Vec3.Forward, Vec3.Zero, CirclePrecision), _material);
                _circleRC.Mesh = _circlePrimitive;
                _normalRC.Mesh = _normalPrimitive;
                _circleRC.NormalMatrix = Matrix3.Identity;
                _normalRC.NormalMatrix = Matrix3.Identity;
            }
            
            public void Render()
            {
                if (!Visible)
                    return;

                if (HighlightedComponent != null && HighlightedComponent != TransformTool3D.Instance?.RootComponent)
                {
                    _circlePrimitive.Render(Transform, Matrix3.Identity);
                    _normalPrimitive.Render(Transform, Matrix3.Identity);
                }
            }

            private RenderCommandMesh3D 
                _circleRC = new RenderCommandMesh3D(), 
                _normalRC = new RenderCommandMesh3D();
            public void AddRenderables(RenderPasses passes, Camera camera)
            {
                if (!Visible)
                    return;

                if (HighlightedComponent != null && HighlightedComponent != TransformTool3D.Instance?.RootComponent)
                {
                    _circleRC.WorldMatrix = Transform;
                    passes.Add(_circleRC, RenderInfo.RenderPass);
                    _normalRC.WorldMatrix = Transform;
                    passes.Add(_normalRC, RenderInfo.RenderPass);
                }
            }
        }
    }
}