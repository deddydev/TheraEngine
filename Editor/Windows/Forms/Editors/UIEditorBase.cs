using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;
using TheraEngine.Rendering.UI;

namespace TheraEditor.Windows.Forms
{
    /// <summary>
    /// UI editor to create shaders in a user-friendly visual graph format.
    /// </summary>
    public abstract class EditorUserInterface : UserInterface, I2DRenderable
    {
        public EditorUserInterface() : base()
            => _rcMethod = new RenderCommandMethod2D(ERenderPass.OnTopForward, RenderMethod);
        public EditorUserInterface(Vec2 bounds) : base(bounds)
            =>_rcMethod = new RenderCommandMethod2D(ERenderPass.OnTopForward, RenderMethod);

        protected UIComponent _baseTransformComponent;
        protected UIMaterialRectangleComponent _backgroundComponent;
        
        protected UITextComponent _originText;
        private Deque<(UITextComponent, UIString2D, float)> _textCacheX = new Deque<(UITextComponent, UIString2D, float)>();
        private Deque<(UITextComponent, UIString2D, float)> _textCacheY = new Deque<(UITextComponent, UIString2D, float)>();
        
        public Font UIFont { get; set; } = new Font("Segoe UI", 10.0f, FontStyle.Regular);

        private readonly RenderCommandMethod2D _rcMethod;

        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(0, 0);
        public BoundingRectangleF AxisAlignedRegion { get; } = new BoundingRectangleF();
        public IQuadtreeNode QuadtreeNode { get; set; }
        
        public float UnitIncrement { get; set; } = 1.0f;

        [TSerialize]
        public float MaxIncrementExclusive { get; set; } = 10.0f;
        [TSerialize]
        public float[] IncrementsRange = new float[] { 1.0f, 2.0f, 5.0f };
        [TSerialize]
        public float TextScale { get; set; } = 1.0f;
        [TSerialize]
        public int InitialVisibleBoxes { get; set; } = 10;

        [TSerialize]
        public float SelectionRadius { get; set; } = 10.0f;
        [TSerialize]
        public float MoveIncrementPerSec { get; set; } = 150.0f;
        [TSerialize]
        public float ZoomTickIncrementPerSec { get; set; } = 0.3f;
        [TSerialize]
        public float ZoomScrollIncrement { get; set; } = 0.05f;
        [TSerialize]
        public bool SnapToIncrement { get; set; } = false;

        protected bool CtrlDown { get; private set; } = false;
        protected bool ShiftDown { get; private set; } = false;
        protected bool RightClickPressed { get; private set; } = false;

        protected Vec2 _minScale = new Vec2(0.01f), _maxScale = new Vec2(1.0f);
        protected Vec2 _lastWorldPos = Vec2.Zero;
        protected float _zoomIn, _zoomOut, _moveUp, _moveDown, _moveLeft, _moveRight;
        protected bool _ticking = false;
        protected virtual bool ShouldTickInput =>
            (_zoomIn + _zoomOut) != 0.0f ||
            (_moveUp + _moveDown) != 0.0f ||
            (_moveLeft + _moveRight) != 0.0f;

        protected override UICanvasComponent OnConstructRoot()
        {
            _baseTransformComponent = new UIComponent();
            _backgroundComponent = new UIMaterialRectangleComponent(GetBackgroundMaterial())
            {
                DockStyle = UIDockStyle.Fill,
                SideAnchorFlags = AnchorFlags.Right | AnchorFlags.Left | AnchorFlags.Top | AnchorFlags.Bottom
            };
            _backgroundComponent.ChildComponents.Add(_baseTransformComponent);

            UICanvasComponent baseUI = new UICanvasComponent();
            baseUI.ChildComponents.Add(_backgroundComponent);

            _baseTransformComponent.WorldTransformChanged += BaseWorldTransformChanged;
            
            _originText = ConstructText(new ColorF4(0.3f), "0", "0", out UIString2D originStr);

            return baseUI;
        }
        public Vec2 GetViewportBottomLeftWorldSpace()
            => Vec3.TransformPosition(Vec3.Zero, _baseTransformComponent.InverseWorldMatrix).Xy;
        public Vec2 GetViewportTopRightWorldSpace()
            => Vec3.TransformPosition(Bounds, _baseTransformComponent.InverseWorldMatrix).Xy;
        public void GetViewportBoundsWorldSpace(out Vec2 min, out Vec2 max)
        {
            min = Vec3.TransformPosition(Vec3.Zero, _baseTransformComponent.InverseWorldMatrix).Xy;
            max = Vec3.TransformPosition(Bounds, _baseTransformComponent.InverseWorldMatrix).Xy;
        }
        protected virtual TMaterial GetBackgroundMaterial()
        {
            GLSLScript frag = Engine.Files.LoadEngineShader("MaterialEditorGraphBG.fs", EGLSLType.Fragment);
            return new TMaterial("MatEditorGraphBG", new ShaderVar[]
            {
                new ShaderVec3(new Vec3(0.15f, 0.15f, 0.15f), "LineColor"),
                new ShaderVec3(new Vec3(0.08f, 0.08f, 0.08f), "BGColor"),
                new ShaderFloat(_baseTransformComponent.ScaleX, "Scale"),
                new ShaderFloat(3.0f, "LineWidth"),
                new ShaderVec2(_baseTransformComponent.LocalTranslation, "Translation"),
                new ShaderFloat(UnitIncrement, "XYIncrement"),
            },
            frag);
        }
        protected UITextComponent ConstructText(ColorF4 color, string initialText, string largestText, out UIString2D str)
        {
            StringFormatFlags flags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
            StringFormat format = new StringFormat(flags);

            //Measure the size of the maximum possible displayed string size
            Size size = TextRenderer.MeasureText(largestText, UIFont);
            float width = size.Width;
            float height = size.Height;

            UITextComponent comp = new UITextComponent();
            comp.RenderInfo.VisibleByDefault = true;
            comp.SizeableHeight.SetSizingPixels(height);
            comp.SizeableWidth.SetSizingPixels(width);
            comp.TextureResolutionMultiplier = UIFont.Size * 0.5f;
            str = new UIString2D()
            {
                Font = UIFont,
                Format = format,
                Text = initialText,
                TextColor = color,
            };
            comp.TextDrawer.Add(true, str);

            _baseTransformComponent.ChildComponents.Add(comp);
            comp.Scale = 1.0f / _baseTransformComponent.Scale * TextScale;

            return comp;
        }
        protected abstract bool GetFocusAreaMinMax(out Vec2 min, out Vec2 max);
        public virtual void ZoomExtents()
        {
            if (GetFocusAreaMinMax(out Vec2 min, out Vec2 max))
            {
                float xBound = max.X - min.X;
                float yBound = max.Y - min.Y;

                if (xBound == 0.0f)
                    xBound = UnitIncrement * InitialVisibleBoxes;
                if (yBound == 0.0f)
                    yBound = UnitIncrement * InitialVisibleBoxes;

                float xScale = Bounds.X / xBound;
                float yScale = Bounds.Y / yBound;

                if (xScale < yScale)
                {
                    _baseTransformComponent.Scale = xScale;
                }
                else
                {
                    _baseTransformComponent.Scale = yScale;
                }

                float midVal = (max.Y + min.Y) * 0.5f;
                _baseTransformComponent.LocalTranslation = new Vec2(0.0f, -(midVal - yBound) * _baseTransformComponent.ScaleX);
            }
            else
                _baseTransformComponent.Scale = TMath.Min(Bounds.X, Bounds.Y) / (UnitIncrement * InitialVisibleBoxes);

            UpdateBackgroundMaterial();
            UpdateTextScale();
        }
        protected virtual void BaseWorldTransformChanged()
        {
            UpdateBackgroundMaterial();
        }
        public override void Resize(Vec2 bounds)
        {
            base.Resize(bounds);
            UpdateBackgroundMaterial();

            //TODO: remove, this is for debug purposes only
            ZoomExtents();
        }
        public void UpdateLineIncrement()
        {
            if (_baseTransformComponent.ScaleX.EqualTo(0.0f, 0.00001f))
                return;

            Vec2 visibleAnimRange = Bounds / _baseTransformComponent.Scale;
            float range = TMath.Min(visibleAnimRange.X, visibleAnimRange.Y);
            if (range == 0.0f || float.IsInfinity(range) || float.IsNaN(range))
                return;

            float invMax = 1.0f / MaxIncrementExclusive;

            int divs = 0;
            int mults = 0;
            while (range >= MaxIncrementExclusive)
            {
                ++divs;
                range *= invMax;
            }
            while (range < 1.0f)
            {
                ++mults;
                range *= MaxIncrementExclusive;
            }
            
            float[] dists = IncrementsRange.Select(x => Math.Abs(range - x)).ToArray();
            float minDist = MaxIncrementExclusive;
            int minDistIndex = 0;
            for (int i = 0; i < dists.Length; ++i)
            {
                if (dists[i] < minDist)
                {
                    minDist = dists[i];
                    minDistIndex = i;
                }
            }

            float inc = IncrementsRange[minDistIndex];
            if (divs > 0)
                inc *= (float)Math.Pow(MaxIncrementExclusive, divs);
            if (mults > 0)
                inc *= (float)Math.Pow(invMax, mults);

            inc /= MaxIncrementExclusive;
            inc *= 0.5f; //half the result
            if (inc != UnitIncrement)
                UnitIncrement = inc;
        }
        
        private void UpdateTextIncrements()
        {
            _baseTransformComponent.IgnoreResizes = true;
            float inc = UnitIncrement * 2.0f;

            Vec2 min = Vec3.TransformPosition(Vec3.Zero, _baseTransformComponent.InverseWorldMatrix).Xy;
            min.X = min.X.RoundToNearestMultiple(inc);
            min.Y = min.Y.RoundToNearestMultiple(inc);

            Vec2 unitCounts = Bounds / (inc * _baseTransformComponent.ScaleX);

            UpdateTextIncPass(unitCounts.X, _textCacheX, min.X, inc, true);
            UpdateTextIncPass(unitCounts.Y, _textCacheY, min.Y, inc, false);
            
            _baseTransformComponent.IgnoreResizes = false;
        }

        private void UpdateTextIncPass(float unitCount, Deque<(UITextComponent, UIString2D, float)> textCache, float minimum, float increment, bool xCoord)
        {
            UITextComponent comp;
            UIString2D str;
            float pos = minimum;
            float max = minimum + increment * unitCount;

            for (int i = 0; i < Math.Max(unitCount, textCache.Count); ++i, pos += increment)
            {
                if (i >= textCache.Count)
                {
                    //Need more cached text components
                    comp = ConstructText(new ColorF4(0.3f), "0", "-0000.000", out str);
                    textCache.PushBack((comp, str, pos));
                }
                else
                {
                    var cache = textCache[(uint)i];
                    comp = cache.Item1;
                    if (i >= unitCount)
                    {
                        comp.RenderInfo.Visible = false;
                        continue;
                    }
                    str = cache.Item2;
                }

                if (pos != 0.0f)
                {
                    str.Text = pos.ToString();//"###0.0##"
                    if (xCoord)
                    {
                        comp.SizeablePosX.ModificationValue = pos;
                        comp.SizeablePosY.ModificationValue = 0.0f;
                    }
                    else
                    {
                        comp.SizeablePosX.ModificationValue = 0.0f;
                        comp.SizeablePosY.ModificationValue = pos;
                    }
                    comp.RenderInfo.Visible = true;
                }
                else
                    comp.RenderInfo.Visible = false;
            }
        }

        private void UpdateBackgroundMaterial()
        {
            UpdateLineIncrement();

            TMaterial mat = _backgroundComponent.InterfaceMaterial;
            mat.Parameter<ShaderFloat>(2).Value = _baseTransformComponent.ScaleX;
            mat.Parameter<ShaderVec2>(4).Value = _baseTransformComponent.LocalTranslation;
            mat.Parameter<ShaderFloat>(5).Value = UnitIncrement;

            UpdateTextIncrements();
        }
        protected override void OnSpawnedPostComponentSpawn()
        {
            base.OnSpawnedPostComponentSpawn();
            RenderInfo.LinkScene(this, ScreenSpaceUIScene);
            ScreenSpaceUIScene.Add(this);
            ZoomExtents();
        }
        protected override void OnDespawned()
        {
            base.OnDespawned();
            RenderInfo.UnlinkScene();
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterKeyPressed(EKey.ControlLeft,  OnCtrlPressed,    EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ControlRight, OnCtrlPressed,    EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ShiftLeft,    OnShiftPressed,   EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ShiftRight,   OnShiftPressed,   EInputPauseType.TickAlways);
            
            input.RegisterKeyEvent(EKey.Number1,                EButtonInputType.Pressed,   ToggleSnap,     EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick,   EButtonInputType.Pressed,   OnLeftClickDown,  EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick,   EButtonInputType.Released,  OnLeftClickUp,    EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick,  EButtonInputType.Pressed,   OnRightClickPressed, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick,  EButtonInputType.Released,  OnRightClickReleased,   EInputPauseType.TickAlways);

            input.RegisterKeyPressed(EKey.Left, MoveLeft, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.Down, MoveDown, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.Right, MoveRight, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.Up, MoveUp, EInputPauseType.TickAlways);

            input.RegisterKeyPressed(EKey.A, MoveLeft, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.S, MoveDown, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.D, MoveRight, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.W, MoveUp, EInputPauseType.TickAlways);

            input.RegisterKeyPressed(EKey.PageDown, ZoomOut, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.PageUp, ZoomIn, EInputPauseType.TickAlways);

            input.RegisterKeyPressed(EKey.Q, ZoomOut, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.E, ZoomIn, EInputPauseType.TickAlways);

            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, EMouseMoveType.Absolute, EInputPauseType.TickAlways);
        }

        /// <summary>
        /// Cursor position relative to the space of whatever is being rendered as a child of the base transform component.
        /// </summary>
        /// <returns></returns>
        public Vec2 CursorPositionTransformRelative()
            => Vec3.TransformPosition(CursorPositionWorld(), _baseTransformComponent.InverseWorldMatrix).Xy;

        private void UpdateInputTick()
        {
            bool should = ShouldTickInput;
            if (should != _ticking)
            {
                if (_ticking = should)
                    Engine.RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input, TickInput);
                else
                    Engine.UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Input, TickInput);
            }
        }
        protected virtual void TickInput(float delta)
        {
            float zoom = _zoomIn + _zoomOut;
            float moveX = _moveLeft + _moveRight;
            float moveY = _moveUp + _moveDown;
            if (zoom != 0.0f)
                Zoom(zoom * delta);
            if (moveX != 0.0f || moveY != 0.0f)
            {
                Vec2 move = new Vec2(moveX * delta, moveY * delta);
                _baseTransformComponent.LocalTranslation += move;
            }
        }
        private void ZoomIn(bool pressed)
        {
            _zoomIn = pressed ? ZoomTickIncrementPerSec : 0.0f;
            UpdateInputTick();
        }
        private void ZoomOut(bool pressed)
        {
            _zoomOut = pressed ? -ZoomTickIncrementPerSec : 0.0f;
            UpdateInputTick();
        }
        private void MoveUp(bool pressed)
        {
            _moveUp = pressed ? -MoveIncrementPerSec : 0.0f;
            UpdateInputTick();
        }
        private void MoveRight(bool pressed)
        {
            _moveRight = pressed ? -MoveIncrementPerSec : 0.0f;
            UpdateInputTick();
        }
        private void MoveDown(bool pressed)
        {
            _moveDown = pressed ? MoveIncrementPerSec : 0.0f;
            UpdateInputTick();
        }
        private void MoveLeft(bool pressed)
        {
            _moveLeft = pressed ? MoveIncrementPerSec : 0.0f;
            UpdateInputTick();
        }

        private void ToggleSnap() => SnapToIncrement = !SnapToIncrement;
        
        protected virtual void OnCtrlPressed(bool pressed) => CtrlDown = pressed;
        protected virtual void OnShiftPressed(bool pressed) => ShiftDown = pressed;

        protected abstract void OnLeftClickDown();
        protected abstract void OnLeftClickUp();
        protected virtual void OnRightClickPressed()
        {
            RightClickPressed = true;
            _lastWorldPos = CursorPositionWorld();
        }
        protected virtual void OnRightClickReleased()
        {
            RightClickPressed = false;
        }

        protected Vec2 GetWorldCursorDiff(Vec2 cursorPosScreen)
        {
            Vec2 screenPoint = _cursorPos;
            screenPoint += cursorPosScreen - _cursorPos;
            Vec2 newFocusPoint = Viewport.ScreenToWorld(screenPoint).Xy;
            Vec2 diff = newFocusPoint - _lastWorldPos;
            _lastWorldPos = newFocusPoint;
            return diff;
        }

        protected abstract bool IsDragging { get; }
        protected override void MouseMove(float x, float y)
        {
            Vec2 cursorPos = CursorPosition();
            if (!Bounds.Contains(cursorPos))
                return;

            if (IsDragging)
                HandleDragItem();
            else if (RightClickPressed)
                HandleDragView();
            else
                HighlightScene();

            _cursorPos = cursorPos;
        }

        protected abstract void HighlightScene();
        protected abstract void HandleDragItem();
        protected virtual void HandleDragView()
        {
            Vec2 diff = GetWorldCursorDiff(CursorPosition());
            _baseTransformComponent.LocalTranslation += diff;
        }
        protected override void OnScrolledInput(bool down)
        {
            if (CtrlDown)
                SelectionRadius *= down ? 1.1f : 0.91f;
            else
                Zoom(down ? ZoomScrollIncrement : -ZoomScrollIncrement);
        }
        protected virtual void Zoom(float increment)
        {
            Vec3 worldPoint = CursorPositionWorld();
            _baseTransformComponent.Zoom(_baseTransformComponent.ScaleX * increment, worldPoint.Xy, new Vec2(0.1f, 0.1f), null);

            UpdateBackgroundMaterial();
            UpdateTextScale();
        }
        protected virtual void UpdateTextScale()
        {
            Vec2 scale = 1.0f / _baseTransformComponent.Scale;
            _originText.Scale = scale;
            foreach (var comp in _textCacheX)
                comp.Item1.Scale = scale;
            foreach (var comp in _textCacheY)
                comp.Item1.Scale = scale;
        }
        protected virtual void AddRenderables(RenderPasses passes) { }
        void I2DRenderable.AddRenderables(RenderPasses passes, Camera camera)
        {
            AddRenderables(passes);
            passes.Add(_rcMethod);
        }
        protected virtual void RenderMethod()
        {
            Vec2 pos = CursorPositionWorld();
            Vec2 wh = _backgroundComponent.Size;

            //Cursor
            Engine.Renderer.RenderCircle(pos + AbstractRenderer.UIPositionBias, AbstractRenderer.UIRotation, SelectionRadius, false, Editor.TurquoiseColor, 10.0f);
            Engine.Renderer.RenderLine(new Vec2(0.0f, pos.Y), new Vec2(wh.X, pos.Y), Editor.TurquoiseColor, false, 10.0f);
            Engine.Renderer.RenderLine(new Vec2(pos.X, 0.0f), new Vec2(pos.X, wh.Y), Editor.TurquoiseColor, false, 10.0f);

            //Grid origin lines
            Vec3 start = Vec3.TransformPosition(new Vec2(0.0f, 0.0f), _baseTransformComponent.WorldMatrix);
            Engine.Renderer.RenderLine(new Vec2(start.X, 0.0f), new Vec2(start.X, wh.Y), new ColorF4(0.55f), false, 10.0f);
            Engine.Renderer.RenderLine(new Vec2(0.0f, start.Y), new Vec2(wh.X, start.Y), new ColorF4(0.55f), false, 10.0f);
        }
    }
}
