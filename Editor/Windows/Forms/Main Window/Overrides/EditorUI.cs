using Extensions;
using System;
using System.Collections.Concurrent;
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
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Editor;
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
    public class EditorUI3D : UserInterface
    {
        public EditorUI3D() : base()
        {
            TransformTool3D.Instance.MouseDown += Instance_MouseDown;
            TransformTool3D.Instance.MouseUp += Instance_MouseUp;
        }
        public EditorUI3D(Vec2 bounds) : base(bounds)
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
                    ((IObject)socket).EditorState,
                    new LocalValueChangeProperty(
                        TransformTool3D.Instance.PrevRootWorldMatrix,
                        socket.WorldMatrix,
                        socket,
                        socket.GetTypeProxy().GetProperty(nameof(ISocket.WorldMatrix))));
            }
        }

        private void Instance_MouseDown()
        {

        }
        
        [TSerialize]
        public float DraggingTestDistance { get; set; } = 20.0f;
        [TSerialize]
        public double FPSUpdateIntervalSeconds { get; set; } = 1.0;

        public Vec3 HitPoint => _hitPoint;
        public Vec3 HitNormal => _hitNormal;
        public float HitDistance => _hitDistance;

        private Vec3 _hitPoint;
        private Vec3 _hitNormal;
        private float _hitDistance;

        private DateTime _lastFPSUpdateTime;
        //private Vec3 _lastHitPoint;
        private HighlightPoint _highlightPoint = new HighlightPoint();
        TRigidBody _pickedBody;
        TPointPointConstraint _currentConstraint;
        private float _toolSize = 1.2f;
        private static ConcurrentDictionary<int, StencilTest>
            _highlightedMaterials = new ConcurrentDictionary<int, StencilTest>(),
            _selectedMaterials = new ConcurrentDictionary<int, StencilTest>();
        internal bool MouseDown { get; set; }//=> OwningPawn.LocalPlayerController.Input.Mouse.LeftClick.IsPressed;

        public bool UseTransformTool => _transformType != TransformType.DragDrop;
        public ISceneComponent DragComponent { get; set; }

        public ISceneComponent HighlightedComponent
        {
            get => _highlightPoint.HighlightedComponent;
            set
            {
                if (HighlightedComponent == value)
                    return;

                if (value == null && HighlightedComponent != null)
                {
                    if (TransformTool3D.Instance == null || (TransformTool3D.Instance != null && HighlightedComponent != TransformTool3D.Instance.RootComponent))
                        _highlightPoint.RenderInfo.Visible = false;
                    //BaseRenderPanel p = Engine.Instance.FocusedPanel?.Control;
                    //p?.BeginInvoke((Action)(() => p.Cursor = Cursors.Default));
                }
                else if (value != null && HighlightedComponent == null)
                {
                    if (TransformTool3D.Instance == null || (TransformTool3D.Instance != null && value != TransformTool3D.Instance.RootComponent))
                        _highlightPoint.RenderInfo.Visible = true;
                    //BaseRenderPanel p = Engine.Instance.FocusedPanel?.Control;
                    //p?.BeginInvoke((Action)(() => p.Cursor = Cursors.Hand));
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
            //if (HighlightedComponent is StaticMeshComponent staticMesh)
            //{
            //    var meshes = staticMesh.Meshes;
            //    if (meshes != null)
            //        foreach (StaticRenderableMesh m in staticMesh.Meshes)
            //            if (m != null)
            //                foreach (var lod in m.LODs)
            //                {
            //                    var tris = lod.Manager.Data.Triangles;
            //                    if (tris != null && tris.Count > 0)
            //                        UpdateMatHighlight(lod.Manager.Material, highlighted);
            //                }
            //}
            //else if (HighlightedComponent is SkeletalMeshComponent skeletalMesh)
            //{
            //    var meshes = skeletalMesh.Meshes;
            //    if (meshes != null)
            //        foreach (SkeletalRenderableMesh m in skeletalMesh.Meshes)
            //            if (m != null)
            //                foreach (var lod in m.LODs)
            //                {
            //                    var tris = lod.Manager.Data.Triangles;
            //                    if (tris != null && tris.Count > 0)
            //                        UpdateMatHighlight(lod.Manager.Material, highlighted);
            //                }
                    
            //}
            //else if (HighlightedComponent is LandscapeComponent landscape)
            //{
            //    UpdateMatHighlight(landscape.Material, highlighted);
            //}
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
                _highlightedMaterials.TryAdd(m.UniqueID, m.RenderParams.StencilTest);
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
                _highlightedMaterials.TryRemove(m.UniqueID, out StencilTest value);
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
                Ref = 0xFF,
                WriteMask = 0xFF,
                ReadMask = 0xFF,
            },
            FrontFace = new StencilTestFace()
            {
                BothFailOp = EStencilOp.Keep,
                StencilPassDepthFailOp = EStencilOp.Replace,
                BothPassOp = EStencilOp.Replace,
                Func = EComparison.Always,
                Ref = 0xFF,
                WriteMask = 0xFF,
                ReadMask = 0xFF,
            },
        };
        public UIViewportComponent SubViewport { get; private set; }
        public UITextComponent SubViewportText { get; private set; }
        public UIString2D FPSText { get; private set; }
        //public UITextProjectionComponent TextOverlay { get; private set; }
        protected override UICanvasComponent OnConstructRoot()
        {
            UICanvasComponent canvas = new UICanvasComponent()
            {
                //DockStyle = UIDockStyle.Fill,
                RenderTransformation = false,
            };

            SubViewport = new UIViewportComponent() { RenderTransformation = false };
            SubViewport.SizeableWidth.Minimum = SizeableElement.Pixels(200.0f, true, EParentBoundsInheritedValue.Width);
            SubViewport.SizeableWidth.SetSizingPercentageOfParent(0.4f);
            SubViewport.SizeableHeight.SetSizingProportioned(SubViewport.SizeableWidth, 9.0f / 16.0f);
            SubViewport.SizeablePosX.SetSizingPercentageOfParent(0.02f);
            SubViewport.SizeablePosY.SetSizingPercentageOfParent(0.02f);
            SubViewport.IsVisible = false;
            canvas.ChildComponents.Add(SubViewport);

            Font f = new Font("Segoe UI", 10.0f, FontStyle.Regular);
            string t = "Selected Camera View";
            Size s = TextRenderer.MeasureText(t, f);
            SubViewportText = new UITextComponent
            {
                DockStyle = EUIDockStyle.Top,
                Height = s.Height,
                RenderTransformation = false,
            };
            SubViewportText.RenderInfo.Visible = false;
            SubViewportText.SizeableHeight.Minimum = SizeableElement.Pixels(s.Height, true, EParentBoundsInheritedValue.Height);
            SubViewportText.SizeableWidth.Minimum = SizeableElement.Pixels(s.Width, true, EParentBoundsInheritedValue.Width);
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
            {
                //Alignment = StringAlignment.Center,
                //LineAlignment = StringAlignment.Near
            };
            SubViewportText.TextDrawer.Text.Add(new UIString2D()
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

            t = "FPS: 000";
            s = TextRenderer.MeasureText(t, f);
            UITextComponent fpsComp = new UITextComponent() { RenderTransformation = false };

            fpsComp.RenderInfo.VisibleByDefault = true;
            fpsComp.SizeableWidth.SetSizingPixels(s.Width);
            fpsComp.SizeableHeight.SetSizingPixels(s.Height);

            fpsComp.SizeablePosX.ParentBoundsInherited = EParentBoundsInheritedValue.Width;
            fpsComp.SizeablePosX.SmallerRelative = true;
            fpsComp.SizeablePosX.SetSizingPixels(0.0f);

            fpsComp.SizeablePosY.ParentBoundsInherited = EParentBoundsInheritedValue.Height;
            fpsComp.SizeablePosY.SmallerRelative = false;
            fpsComp.SizeablePosY.SetSizingPixels(0.0f);
            fpsComp.SizeablePosY.Origin = fpsComp.SizeableHeight;

            fpsComp.TextureResolutionMultiplier = f.Size;
            fpsComp.TextDrawer.Text.Add(FPSText = new UIString2D()
            {
                Font = f,
                Format = sf,
                Text = t,
                TextColor = new ColorF4(0.1f, 1.0f, 0.1f, 1.0f),
            });
            canvas.ChildComponents.Add(fpsComp);

            canvas.OriginRebased += RootComponent_OriginRebased;

            return canvas;
        }

        private void RootComponent_OriginRebased(Vec3 newOrigin)
        {
            _hitPoint -= newOrigin;
        }

        public override void RegisterInput(InputInterface input)
        {
            //input.RegisterMouseMove(OnMouseMove, false, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, EButtonInputType.Pressed, OnMouseDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, EButtonInputType.Released, OnMouseUp, EInputPauseType.TickAlways);

            input.RegisterButtonEvent(EGamePadButton.FaceDown, EButtonInputType.Pressed, OnGamepadSelect, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EGamePadButton.FaceRight, EButtonInputType.Pressed, OnBackInput, EInputPauseType.TickAlways);
            
            input.RegisterKeyEvent(EKey.Number1, EButtonInputType.Pressed, SetTranslationMode, EInputPauseType.TickAlways);
            input.RegisterKeyEvent(EKey.Number2, EButtonInputType.Pressed, SetRotationMode, EInputPauseType.TickAlways);
            input.RegisterKeyEvent(EKey.Number3, EButtonInputType.Pressed, SetScaleMode, EInputPauseType.TickAlways);
            input.RegisterKeyEvent(EKey.Number4, EButtonInputType.Pressed, SetDragDropMode, EInputPauseType.TickAlways);
            
            input.RegisterKeyEvent(EKey.Number5, EButtonInputType.Pressed, SetWorldSpace, EInputPauseType.TickAlways);
            input.RegisterKeyEvent(EKey.Number6, EButtonInputType.Pressed, SetParentSpace, EInputPauseType.TickAlways);
            input.RegisterKeyEvent(EKey.Number7, EButtonInputType.Pressed, SetLocalSpace, EInputPauseType.TickAlways);
            input.RegisterKeyEvent(EKey.Number8, EButtonInputType.Pressed, SetScreenSpace, EInputPauseType.TickAlways);

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
        protected override void OnSpawnedPostComponentSpawn()
        {
            base.OnSpawnedPostComponentSpawn();

            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            _highlightPoint.RenderInfo.LinkScene(_highlightPoint, OwningScene3D);
            SubViewport.IsVisible = false;

            Editor.Instance.ActorTreeForm.EditorHUD = this;
        }
        protected override void OnDespawned()
        {
            base.OnDespawned();

            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            _highlightPoint.RenderInfo.UnlinkScene();

            Editor.Instance.ActorTreeForm.EditorHUD = null;
        }
        private float _averageFPS = 0.0f;
        private int _averageFPSCount = 1;
        private void Tick(float delta)
        {
            DateTime now = DateTime.Now;
            if ((now - _lastFPSUpdateTime).TotalSeconds >= FPSUpdateIntervalSeconds)
            {
                FPSText.Text = "FPS: " + Math.Round(_averageFPS / _averageFPSCount, 0, MidpointRounding.AwayFromZero);
                _averageFPSCount = 1;
                _averageFPS = Engine.RenderFrequency;
                _lastFPSUpdateTime = now;
            }
            else
            {
                _averageFPS += Engine.RenderFrequency;
                ++_averageFPSCount;
            }

            MouseMove();
        }
        private void MouseMove()
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport;
            if (v != null)
            {
                Vec2 viewportPoint = /*gamepad ? v.Center : */CursorPositionViewport(v);
                MouseMove(v, viewportPoint);
            }
        }
        public void MouseMove(Viewport v, Vec2 viewportPoint)
        {
            ISceneComponent dragComp = DragComponent;

            //Transforming object?
            if (TransformTool3D.Instance.IsSpawned)
            {
                Ray cursor = v.GetWorldRay(viewportPoint);
                if (!TransformTool3D.Instance.MouseMove(cursor, v.Camera, MouseDown))
                    HighlightScene(v, viewportPoint);
                else if (!MouseDown)
                    HighlightedComponent = null;//TransformTool3D.Instance.RootComponent;
            }
            else if (_currentConstraint != null) //Dragging physics object?
            {
                Ray cursor = v.GetWorldRay(viewportPoint);
                _currentConstraint.PivotInB = cursor.StartPoint + cursor.Direction * DraggingTestDistance;
            }
            else if (dragComp != null) //Dragging and dropping object?
            {
                IRigidBodyCollidable p = dragComp as IRigidBodyCollidable;
                ISceneComponent comp = v.PickScene(
                    viewportPoint, true, true, true,
                    out _hitNormal, out _hitPoint, out float dist,
                    p != null ? new TRigidBody[] { p.RigidBodyCollision } : new TRigidBody[0]);

                if (dist > DraggingTestDistance)
                    comp = null;

                float upDist = 0.0f;
                if (comp == null)
                {
                    _hitNormal = Vec3.Up;
                    float depth = TMath.DistanceToDepth(DraggingTestDistance, v.Camera.NearZ, v.Camera.FarZ);
                    _hitPoint = v.ScreenToWorld(viewportPoint, depth);
                }

                Vec3 rightCameraVector = v.Camera.RightVector;
                Quat rotation = Quat.FromAxisAngleDeg(_hitNormal, _spawnRotation);
                rightCameraVector = rotation * rightCameraVector;
                Vec3
                    forward = rightCameraVector ^ _hitNormal,
                    up = _hitNormal,
                    right = up ^ forward,
                    translation = HitPoint;

                right.Normalize();
                up.Normalize();
                forward.Normalize();

                translation += up * upDist;

                dragComp.WorldMatrix = Matrix4.CreateSpacialTransform(
                    translation,
                    right * _draggingUniformScale,
                    up * _draggingUniformScale,
                    forward * _draggingUniformScale);
            }
            else //Not doing anything
            {
                HighlightScene(v, viewportPoint);
            }
        }
        private void HighlightScene(Viewport viewport, Vec2 viewportPoint)
        {
            ICamera c = viewport.Camera;
            if (c == null)
                return;

            WorldEditorCameraPawn pawn = OwningPawn as WorldEditorCameraPawn;
            if (pawn.Moving)
            {
                UpdateHighlightPoint(c);
                return;
            }
            
            ISceneComponent comp = viewport.PickScene(viewportPoint, true, true, true, out _hitNormal, out _hitPoint, out _hitDistance);
            bool hasHit = comp != null;
            _highlightPoint.RenderInfo.Visible = hasHit;
            if (hasHit)
            {
                if (comp is UIViewportComponent subViewport)
                {
                    //Convert viewport point to the sub viewport's local space
                    Vec2 subViewportPoint = subViewport.ScreenToLocal(viewportPoint);
                    HighlightScene(subViewport.Viewport, subViewportPoint);

                    //ICameraTransformable camComp = SelectedComponent as ICameraTransformable;
                    //pawn.CameraComp = camComp.RootComponent.ChildComponents[0] as CameraComponent;
                    //pawn.TargetComponent = camComp;
                    //pawn.TargetViewportComponent = subViewport;

                    return;
                }
                //else
                //{
                //    pawn.CameraComp = pawn.RootComponent.ChildComponents[0] as CameraComponent;
                //    pawn.TargetComponent = pawn.RootComponent;
                //    pawn.TargetViewportComponent = null;
                //}
            }
            UpdateHighlightPoint(c);
            HighlightedComponent = comp;
        }
        internal void UpdateHighlightPoint(ICamera c)
        {
            _highlightPoint.Transform =
                Matrix4.CreateTranslation(_hitPoint) *
                _hitNormal.LookatAngles().GetMatrix() *
                Matrix4.CreateScale(c?.DistanceScale(_hitPoint, _toolSize) ?? 1.0f);
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
                OwningWorld.PhysicsWorld3D.RemoveConstraint(_currentConstraint);
                //_currentConstraint.Dispose();
                _currentConstraint = null;
                _pickedBody.ForceActivationState(EBodyActivationState.Active);
                _pickedBody = null;
            }

            if (DragComponent != null)
            {
                LocalValueChangeProperty change = new LocalValueChangeProperty(_prevDragMatrix, DragComponent.WorldMatrix, DragComponent, DragComponent.GetType().GetProperty(nameof(DragComponent.WorldMatrix)));
                Editor.Instance.UndoManager.AddChange(DragComponent.EditorState, change);
                //_selectedComponent = null;
                DragComponent = null;
            }

            _highlightPoint.RenderInfo.Visible = HighlightedComponent != null;
        }
        public ISceneComponent SelectedComponent { get; private set; }

        public void SetSelectedComponent(bool selectedByViewport, ISceneComponent comp, bool fromActorTree = false)
        {
            if (SelectedComponent == comp)
                return;
            
            PreSelectedComponentChanged(selectedByViewport);
            SelectedComponent = comp;
            PostSelectedComponentChanged(selectedByViewport);

            if (!fromActorTree)
            {
                TreeNode t = SelectedComponent?.OwningActor?.EditorState?.TreeNode;
                if (t?.TreeView == null)
                    return;
                
                if (t.TreeView.InvokeRequired)
                    t.TreeView.BeginInvoke((Action) (() => t.TreeView.SelectedNode = t));
                else
                    t.TreeView.SelectedNode = t;
            }
        }
        private void PreSelectedComponentChanged(bool selectedByViewport)
        {
            if (SelectedComponent is CameraComponent camComp)
            {
                //CameraPreviewComponent.PreviewAlwaysVisible
            }
        }
        private void PostSelectedComponentChanged(bool selectedByViewport)
        {
            DragComponent = null;

            if (SelectedComponent is CameraComponent cam &&
                cam.Camera != OwningPawn?.LocalPlayerController?.ViewportCamera)
            {
                SubViewport.ViewportCamera = cam.Camera;
                SubViewport.IsVisible = true;
                if (cam.PreviewAlwaysVisible)
                    Engine.EditorState.PinnedCameraComponent = cam;
            }
            else if (Engine.EditorState.PinnedCameraComponent != null)
            {
                SubViewport.ViewportCamera = Engine.EditorState.PinnedCameraComponent.Camera;
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

                _highlightPoint.RenderInfo.Visible = false;
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

                        Vec3 localPivot = Vec3.TransformPosition(HitPoint, _pickedBody.CenterOfMassTransform.Inverted());

                        _currentConstraint = TPointPointConstraint.New(_pickedBody, localPivot);
                        _currentConstraint.ImpulseClamp = 60;
                        _currentConstraint.Tau = 0.6f;

                        OwningWorld.PhysicsWorld3D.AddConstraint(_currentConstraint);
                    }
                    else
                    {
                        if (UseTransformTool)
                        {
                            TransformTool3D.GetInstance(OwningWorld, SelectedComponent, _transformType);
                        }
                        else
                        {
                            TransformTool3D.DestroyInstance();

                            DragComponent = SelectedComponent;
                            if (DragComponent != null)
                            {
                                _prevDragMatrix = DragComponent.WorldMatrix;
                                ICamera c = OwningPawn?.LocalPlayerController?.Viewport?.Camera;
                                DraggingTestDistance = c != null ? c.DistanceFromScreenPlane(DragComponent.WorldPoint) : DraggingTestDistance;
                            }
                        }
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

            public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D(false, true);
            public ISceneComponent HighlightedComponent { get; set; }
            public Matrix4 Transform { get; set; } = Matrix4.Identity;

            private TMaterial _material;
            private readonly PrimitiveManager _circlePrimitive;
            private readonly PrimitiveManager _normalPrimitive;

            private RenderCommandMesh3D
                _circleRC = new RenderCommandMesh3D(ERenderPass.OnTopForward),
                _normalRC = new RenderCommandMesh3D(ERenderPass.OnTopForward);

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

            public void AddRenderables(RenderPasses passes, ICamera camera)
            {
                if (HighlightedComponent != null && HighlightedComponent != TransformTool3D.Instance?.RootComponent)
                {
                    _circleRC.WorldMatrix = Transform;
                    passes.Add(_circleRC);
                    _normalRC.WorldMatrix = Transform;
                    passes.Add(_normalRC);
                }
            }
        }
    }
}