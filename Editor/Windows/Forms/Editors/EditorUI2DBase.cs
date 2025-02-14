﻿using Extensions;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Core.Maths;
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
    /// Base class for 2D editor UIs.
    /// </summary>
    public abstract class EditorUI2DBase : UserInterfacePawn, I2DRenderable
    {
        public EditorUI2DBase() : base()
            => _rcMethod = new RenderCommandMethod2D(ERenderPass.OnTopForward, RenderMethod);
        public EditorUI2DBase(Vec2 bounds) : base(bounds)
            => _rcMethod = new RenderCommandMethod2D(ERenderPass.OnTopForward, RenderMethod);

        protected UIMaterialComponent _backgroundComponent;

        protected UITextRasterComponent _originText;
        private ConcurrentDictionary<string, (UITextRasterComponent, UIString2D)> _textCacheX = new ConcurrentDictionary<string, (UITextRasterComponent, UIString2D)>();
        private ConcurrentDictionary<string, (UITextRasterComponent, UIString2D)> _textCacheY = new ConcurrentDictionary<string, (UITextRasterComponent, UIString2D)>();
        protected UITextRasterComponent _xUnitText, _yUnitText;
        protected UIString2D _xUnitString, _yUnitString;

        public Font UIFont { get; set; } = new Font("Segoe UI", 8.0f, FontStyle.Regular);

        private readonly RenderCommandMethod2D _rcMethod;

        public IRenderInfo2D RenderInfo { get; } = new RenderInfo2D(0, 0);
        public BoundingRectangleF AxisAlignedRegion { get; } = new BoundingRectangleF();
        public IQuadtreeNode QuadtreeNode { get; set; }

        public float UnitIncrementX { get; set; } = 1.0f;
        public float UnitIncrementY { get; set; } = 1.0f;

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
        public bool SnapToUnits { get; set; } = false;

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

        public virtual string XUnitString { get; } = null;
        public virtual string YUnitString { get; } = null;
        protected override UICanvasComponent OnConstructRoot()
        {
            OriginTransformComponent = new UITransformComponent() { RenderTransformation = false };

            TMaterial bgmat = GetBackgroundMaterial();
            _backgroundComponent = new UIMaterialComponent(bgmat) { RenderTransformation = false };
            _backgroundComponent.Translation.Xy = 0.0f;
            _backgroundComponent.HorizontalAlignment = EHorizontalAlign.Stretch;
            _backgroundComponent.VerticalAlignment = EVerticalAlign.Stretch;
            _backgroundComponent.ChildSockets.Add(OriginTransformComponent);

            UICanvasComponent baseUI = new UICanvasComponent() { RenderTransformation = false };
            baseUI.ChildSockets.Add(_backgroundComponent);

            OriginTransformComponent.WorldTransformChanged += OriginTransformComponent_WorldTransformChanged;

            _originText = ConstructText(new ColorF4(0.3f), "0", "0", out UIString2D originStr);
            if (!string.IsNullOrWhiteSpace(XUnitString))
                _xUnitText = ConstructText(ColorF4.White, XUnitString, XUnitString, out _xUnitString);
            if (!string.IsNullOrWhiteSpace(YUnitString))
                _yUnitText = ConstructText(ColorF4.White, YUnitString, YUnitString, out _yUnitString);

            return baseUI;
        }

        public Vec2 TransformToOriginRelativeSpace(Vec2 point)
            => Vec3.TransformPosition(point, OriginTransformComponent.InverseWorldMatrix.Value).Xy;
        
        public Vec2 GetViewportBottomLeft()
            => TransformToOriginRelativeSpace(Vec2.Zero);
        public Vec2 GetViewportTopRight()
            => TransformToOriginRelativeSpace(Bounds.Value);
        public Vec2 GetViewportTopLeft()
            => TransformToOriginRelativeSpace(new Vec2(0.0f, Bounds.Y));
        public Vec2 GetViewportBottomRight()
            => TransformToOriginRelativeSpace(new Vec2(Bounds.X, 0.0f));

        public void GetViewportBounds(out Vec2 min, out Vec2 max)
        {
            min = GetViewportBottomLeft();
            max = GetViewportTopRight();
        }
        protected virtual TMaterial GetBackgroundMaterial()
        {
            GLSLScript frag = Engine.Files.Shader("MaterialEditorGraphBG.fs");
            return new TMaterial("MatEditorGraphBG", new ShaderVar[]
            {
                new ShaderVec3(new Vec3(0.15f, 0.15f, 0.15f), "LineColor"),
                new ShaderVec3(new Vec3(0.08f, 0.08f, 0.08f), "BGColor"),
                new ShaderFloat(OriginTransformComponent.Scale.X, "Scale"),
                new ShaderFloat(2.0f, "LineWidth"),
                new ShaderVec2(OriginTransformComponent.ActualTranslation.Value, "Translation"),
                new ShaderFloat(UnitIncrementX, "XYIncrement"),
            },
            frag);
        }
        protected UITextRasterComponent ConstructText(ColorF4 color, string initialText, string largestText, out UIString2D str)
        {
            //Measure the size of the maximum possible displayed string size
            Size size = TextRenderer.MeasureText(largestText, UIFont);
            float width = size.Width;
            float height = size.Height;

            UITextRasterComponent comp = new UITextRasterComponent() { RenderTransformation = false };
            comp.RenderInfo2D.VisibleByDefault = true;
            comp.Size.Value = new Vec2(width, height);
            //comp.TextureResolutionMultiplier = UIFont.Size;

            TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.SingleLine;
            str = new UIString2D(initialText, UIFont, color, flags);
            str.Region.Width = width;
            str.Region.Height = height;

            comp.TextDrawer.Text.Add(str);

            OriginTransformComponent.ChildSockets.Add(comp);

            comp.Scale.Value = 1.0f / OriginTransformComponent.Scale * TextScale;
            
            comp.Scale.BindProperty(nameof(EventVec3.Value),
                OriginTransformComponent.Scale, nameof(EventVec3.Value),
                TextScaleConverter);

            return comp;
        }
        private object TextScaleConverter(object x) => 1.0f / (Vec3)x * TextScale;
        protected abstract bool GetWorkArea(out Vec2 min, out Vec2 max);
        public virtual void ZoomExtents(bool adjustScale = true)
        {
            Vec2 targetTranslation;
            Vec2 targetScale = Vec2.One;

            if (!GetWorkArea(out Vec2 min, out Vec2 max))
            {
                targetTranslation = Vec2.Zero;

                if (adjustScale)
                    targetScale = TMath.Min(Bounds.X, Bounds.Y) / (UnitIncrementX * InitialVisibleBoxes);
            }
            else
            {
                float xBound = max.X - min.X;
                float yBound = max.Y - min.Y;
                Vec2 bottomLeft = -min;

                if (xBound == 0.0f)
                    xBound = UnitIncrementX * InitialVisibleBoxes;
                if (yBound == 0.0f)
                    yBound = UnitIncrementY * InitialVisibleBoxes;

                float xScale = Bounds.X / xBound;
                float yScale = Bounds.Y / yBound;

                if (adjustScale)
                {
                    if (xScale < yScale)
                    {
                        targetScale = xScale;
                        float remaining = (Bounds.Y / targetScale.X - yBound) * 0.5f;
                        bottomLeft.Y += remaining;
                        bottomLeft *= targetScale.X;
                    }
                    else
                    {
                        targetScale = yScale;
                        float remaining = (Bounds.X / targetScale.Y - xBound) * 0.5f;
                        bottomLeft.X += remaining;
                        bottomLeft *= targetScale.Y;
                    }
                }
                else
                {
                    float remaining = (Bounds.Y / OriginTransformComponent.Scale.X - yBound) * 0.5f;
                    bottomLeft.Y += remaining;

                    remaining = (Bounds.X / OriginTransformComponent.Scale.Y - xBound) * 0.5f;
                    bottomLeft.X += remaining;

                    bottomLeft *= OriginTransformComponent.Scale.Xy;
                }

                targetTranslation = bottomLeft;
            }

            OriginTransformComponent.Translation.AnimatePropertyTo(nameof(EventVec3.Xy), 1.0f, targetTranslation);
            //OriginTransformComponent.Translation.Xy = targetTranslation;

            if (adjustScale)
            {
                OriginTransformComponent.Scale.AnimatePropertyTo(nameof(EventVec3.Xy), 1.0f, targetScale);
                //OriginTransformComponent.Scale.Xy = targetScale;
            }
        }
        protected virtual void OriginTransformComponent_WorldTransformChanged(ISceneComponent obj)
        {
            if (_xUnitText != null)
                _xUnitText.Translation.X = (-OriginTransformComponent.Translation.X + Bounds.X - _xUnitText.Width) / OriginTransformComponent.Scale.X;
            
            if (_yUnitText != null)
                _yUnitText.Translation.Y = (-OriginTransformComponent.Translation.Y + Bounds.Y - _yUnitText.Height) / OriginTransformComponent.Scale.Y;
            
            CalcIntervals();
            DisplayIntervals();
            UpdateBackgroundMaterial();
        }
        public override void Resize(Vec2 bounds)
        {
            base.Resize(bounds);

            //TODO: zoom extents of the previous bounds with no scale instead of the target's bounds
            ZoomExtents(false);
        }
        public void CalcIntervals()
        {
            if (OriginTransformComponent.Scale.X.EqualTo(0.0f, 0.00001f))
                return;

            Vec2 visibleAnimRange = Bounds.Value / OriginTransformComponent.Scale.Xy;

            UnitIncrementX = CalcInterval(visibleAnimRange.X, MaxIncrementExclusive, IncrementsRange);
            UnitIncrementY = CalcInterval(visibleAnimRange.Y, MaxIncrementExclusive, IncrementsRange);
        }

        private static float CalcInterval(float range, float maxIncrementExclusive, float[] incrementsRange)
        {
            if (range == 0.0f || float.IsInfinity(range) || float.IsNaN(range))
                return 1.0f;

            float invMax = 1.0f / maxIncrementExclusive;

            int divs = 0;
            int mults = 0;
            while (range >= maxIncrementExclusive)
            {
                ++divs;
                range *= invMax;
            }
            while (range < 1.0f)
            {
                ++mults;
                range *= maxIncrementExclusive;
            }

            float[] dists = incrementsRange.Select(x => Math.Abs(range - x)).ToArray();
            float minDist = maxIncrementExclusive;
            int minDistIndex = 0;
            for (int i = 0; i < dists.Length; ++i)
            {
                if (dists[i] < minDist)
                {
                    minDist = dists[i];
                    minDistIndex = i;
                }
            }

            float inc = incrementsRange[minDistIndex];
            if (divs > 0)
                inc *= (float)Math.Pow(maxIncrementExclusive, divs);
            if (mults > 0)
                inc *= (float)Math.Pow(invMax, mults);

            inc /= maxIncrementExclusive;
            inc *= 0.5f; //half the result

            return inc;
        }

        private void DisplayIntervals()
        {
            float incX = UnitIncrementX * 2.0f;
            float incY = UnitIncrementY * 2.0f;

            GetViewportBounds(
                out Vec2 viewMin,
                out Vec2 viewMax);

            Vec2 intervalStart = new Vec2(
                viewMin.X.RoundedToNearestMultiple(incX), 
                viewMin.Y.RoundedToNearestMultiple(incY));

            Vec2 unitCounts = Vec2.One + Bounds.Value / (new Vec2(incX, incY) * OriginTransformComponent.Scale.Xy);

            UpdateTextIncrements(
                unitCounts.X, 
                _textCacheX, 
                intervalStart.X, 
                incX, true, 
                viewMin.Y * OriginTransformComponent.Scale.X, 
                viewMax.Y * OriginTransformComponent.Scale.X);

            UpdateTextIncrements(
                unitCounts.Y, 
                _textCacheY, 
                intervalStart.Y,
                incY, false, 
                viewMin.X * OriginTransformComponent.Scale.X,
                viewMax.X * OriginTransformComponent.Scale.X);
        }
        private void UpdateTextIncrements(
            float unitCount,
            ConcurrentDictionary<string, (UITextRasterComponent, UIString2D)> textCache,
            float minimum, 
            float increment,
            bool xCoord,
            float minView,
            float maxView)
        {
            if (float.IsNaN(unitCount) || float.IsNaN(minimum) ||
                float.IsInfinity(unitCount) || float.IsInfinity(minimum))
                return;

            try
            {
                var allKeys = textCache.Keys.ToList();
                var visible = Enumerable.Range(0, (int)Math.Ceiling(unitCount)).Select(x =>
                {
                    var value = minimum + x * increment;
                    var vstr = value.ToString("###0.0##");
                    if (!allKeys.Contains(vstr))
                        allKeys.Add(vstr);
                    return (value, vstr);
                }).ToDictionary(x => x.vstr, x => x.value);

                bool IsVisible(string key) => visible.ContainsKey(key);

                UITextRasterComponent comp;
                UIString2D str;

                foreach (var key in allKeys)
                {
                    if (IsVisible(key))
                    {
                        //Visible, and in cache?
                        if (textCache.TryGetValue(key, out var cache))
                        {
                            comp = cache.Item1;
                            str = cache.Item2;
                        }
                        else
                        {
                            //Not in cache, find an unused cache item
                            var unusedKey = textCache.FirstOrDefault(x => !IsVisible(x.Key)).Key;
                            if (unusedKey != null)
                            {
                                if (textCache.TryRemove(unusedKey, out var value))
                                {
                                    comp = value.Item1;
                                    str = value.Item2;

                                    str.Text = key;
                                    textCache.TryAdd(key, value);
                                }
                                else
                                    continue;
                            }
                            else
                            {
                                //Not in cache, none unused
                                //Need more cached text components
                                comp = ConstructText(new ColorF3(0.4f), key, "-0000.000", out str);
                                textCache.TryAdd(key, (comp, str));
                            }
                        }

                        var pos = visible[key];
                        if (xCoord)
                        {
                            float y = 0.0f;
                            if (y < minView)
                                y = minView / OriginTransformComponent.Scale.X;
                            float height = comp.TextDrawer.Text[0].ActualTextSize.Y;
                            if (y + height > maxView)
                                y = (maxView - height) / OriginTransformComponent.Scale.X;
                            comp.Translation.Xy = new Vec2(pos, y);
                        }
                        else
                        {
                            float x = 0.0f;
                            if (x < minView)
                                x = minView / OriginTransformComponent.Scale.X;
                            float width = comp.TextDrawer.Text[0].ActualTextSize.X;
                            if (x + width > maxView)
                                x = (maxView - width) / OriginTransformComponent.Scale.X;
                            comp.Translation.Xy = new Vec2(x, pos);
                        }
                        comp.RenderInfo2D.IsVisible = true;
                    }
                    else
                    {
                        //Not visible, but exists in cache? Hide it
                        if (textCache.TryGetValue(key, out var value))
                            value.Item1.RenderInfo2D.IsVisible = false;
                        continue;
                    }
                }
            }
            catch
            {

            }
        }

        protected void UpdateBackgroundMaterial()
        {
            TMaterial mat = _backgroundComponent.Material;
            mat.Parameter<ShaderFloat>(2).Value = OriginTransformComponent.Scale.X;
            mat.Parameter<ShaderVec2>(4).Value = OriginTransformComponent.ActualTranslation.Value;
            mat.Parameter<ShaderFloat>(5).Value = UnitIncrementX;
        }
        protected override void OnSpawnedPostComponentSpawn()
        {
            base.OnSpawnedPostComponentSpawn();
            RenderInfo.LinkScene(this, RootComponent.ScreenSpaceUIScene);
            ZoomExtents();
        }
        protected override void OnDespawned()
        {
            base.OnDespawned();
            RenderInfo.UnlinkScene();
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterKeyPressed(EKey.ControlLeft, OnCtrlPressed, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ControlRight, OnCtrlPressed, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ShiftLeft, OnShiftPressed, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ShiftRight, OnShiftPressed, EInputPauseType.TickAlways);

            input.RegisterKeyEvent(EKey.Number1, EButtonInputType.Pressed, ToggleSnap, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, EButtonInputType.Pressed, OnLeftClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, EButtonInputType.Released, OnLeftClickUp, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, EButtonInputType.Pressed, OnRightClickPressed, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, EButtonInputType.Released, OnRightClickReleased, EInputPauseType.TickAlways);

            //input.RegisterKeyPressed(EKey.Left, MoveLeft, EInputPauseType.TickAlways);
            //input.RegisterKeyPressed(EKey.Down, MoveDown, EInputPauseType.TickAlways);
            //input.RegisterKeyPressed(EKey.Right, MoveRight, EInputPauseType.TickAlways);
            //input.RegisterKeyPressed(EKey.Up, MoveUp, EInputPauseType.TickAlways);

            input.RegisterKeyPressed(EKey.A, MoveLeft, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.S, MoveDown, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.D, MoveRight, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.W, MoveUp, EInputPauseType.TickAlways);

            input.RegisterKeyPressed(EKey.PageDown, ZoomOut, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.PageUp, ZoomIn, EInputPauseType.TickAlways);

            input.RegisterKeyPressed(EKey.E, ZoomOut, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.Q, ZoomIn, EInputPauseType.TickAlways);

            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);

            base.RegisterInput(input);
        }

        /// <summary>
        /// Cursor position relative to the space of whatever is being rendered as a child of the base transform component.
        /// </summary>
        /// <returns></returns>
        public Vec2 CursorPositionTransformRelative()
            => Vec3.TransformPosition(CursorPositionWorld(), OriginTransformComponent.InverseWorldMatrix.Value).Xy;
        public Vec2 CursorDiffTransformRelative()
               => Vec3.TransformPosition(GetWorldCursorDiff(CursorPosition()), OriginTransformComponent.InverseWorldMatrix.Value).Xy;

        protected void UpdateInputTick()
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
                OriginTransformComponent.Translation.Xy += move;
            }
        }
        private void ZoomOut(bool pressed)
        {
            _zoomOut = pressed ? ZoomTickIncrementPerSec : 0.0f;
            UpdateInputTick();
        }
        private void ZoomIn(bool pressed)
        {
            _zoomIn = pressed ? -ZoomTickIncrementPerSec : 0.0f;
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

        private void ToggleSnap() => SnapToUnits = !SnapToUnits;

        protected virtual void OnCtrlPressed(bool pressed) => CtrlDown = pressed;
        protected virtual void OnShiftPressed(bool pressed) => ShiftDown = pressed;

        protected virtual void OnLeftClickDown()
        {

        }
        protected virtual void OnLeftClickUp()
        {
            if (ContextMenu is null)
                return;

            Vec2 cursorPos = CursorPositionWorld();
            if (ContextMenu.Contains(cursorPos))
                ContextMenu.HoveredMenuItem?.Click();
            
            ContextMenu.Visibility = EVisibility.Collapsed;
        }

        protected virtual void OnRightClickPressed()
        {
            _viewDragged = false;
            _lastWorldPos = CursorPositionWorld();

            RightClickPressed = true;
        }
        protected virtual void OnRightClickReleased()
        {
            RightClickPressed = false;

            if (ContextMenu != null)
                ContextMenu.Visibility = _viewDragged ? EVisibility.Collapsed : EVisibility.Visible;
        }

        public TMenuComponent ContextMenu 
        {
            get => _contextMenu;
            set
            {
                if (_contextMenu != null)
                {
                    _contextMenu.Visibility = EVisibility.Collapsed;
                    RootComponent.ChildSockets.Remove(_contextMenu);
                }

                _contextMenu = value;

                if (_contextMenu != null)
                {
                    _contextMenu.Visibility = EVisibility.Collapsed;
                    RootComponent.ChildSockets.Add(_contextMenu);
                }
            }
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

        private bool _viewDragged = false;
        private TMenuComponent _contextMenu;

        protected abstract bool IsDragging { get; }
        public UITransformComponent OriginTransformComponent { get; protected set; }

        protected override void MouseMove(float x, float y)
        {
            if (Viewport is null)
                return;

            Vec2 cursorPos = CursorPosition();
            if (!Bounds.Value.Contains(cursorPos))
            {
                _cursorPos = cursorPos;
                return;
            }

            if (IsDragging)
                HandleDragItem();
            else if (RightClickPressed)
                HandleDragView();
            else
                HighlightScene();

            _cursorPos = cursorPos;
            _lastWorldPos = Viewport.ScreenToWorld(_cursorPos).Xy;
        }

        protected abstract void HighlightScene();
        protected abstract void HandleDragItem();
        protected virtual void HandleDragView()
        {
            _viewDragged = true;
            Vec2 diff = GetWorldCursorDiff(CursorPosition());
            if (diff.LengthSquared > float.Epsilon)
                OriginTransformComponent.Translation.Xy += diff;
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
            OriginTransformComponent.Zoom(OriginTransformComponent.Scale.X * increment, worldPoint.Xy, new Vec2(0.1f, 0.1f), new Vec2(3000.0f, 3000.0f));
        }
        protected virtual void AddRenderables(RenderPasses passes) { }
        void I2DRenderable.AddRenderables(RenderPasses passes, ICamera camera)
        {
            AddRenderables(passes);
            passes.Add(_rcMethod);
        }
        protected virtual void RenderMethod()
        {
            Vec2 pos = CursorPositionWorld();
            Vec2 wh = _backgroundComponent.ActualSize.Value;

            //Cursor
            Engine.Renderer.RenderCircle(pos + AbstractRenderer.UIPositionBias, AbstractRenderer.UIRotation, SelectionRadius, false, Editor.TurquoiseColor, 10.0f);
            Engine.Renderer.RenderLine(new Vec2(0.0f, pos.Y), new Vec2(wh.X, pos.Y), Editor.TurquoiseColor, false, 10.0f);
            Engine.Renderer.RenderLine(new Vec2(pos.X, 0.0f), new Vec2(pos.X, wh.Y), Editor.TurquoiseColor, false, 10.0f);

            //Grid origin lines
            Vec3 start = Vec3.TransformPosition(new Vec2(0.0f, 0.0f), OriginTransformComponent.WorldMatrix.Value);
            Engine.Renderer.RenderLine(new Vec2(start.X, 0.0f), new Vec2(start.X, wh.Y), new ColorF4(0.55f), false, 10.0f);
            Engine.Renderer.RenderLine(new Vec2(0.0f, start.Y), new Vec2(wh.X, start.Y), new ColorF4(0.55f), false, 10.0f);
        }

        public bool Contains(Vec2 worldPoint)
        {
            return false;
        }

        public Vec2 ClosestPoint(Vec2 worldPoint)
        {
            return new Vec2();
        }
    }
}
