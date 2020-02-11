using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Animation;
using TheraEngine.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;
using TheraEngine.Rendering.UI;

namespace TheraEditor.Windows.Forms
{
    public delegate void DelSelectedKeyframeChanged(Keyframe kf);
    public delegate void DelSelectedTangentChanged(Keyframe kf, bool inTan);
    /// <summary>
    /// UI editor to create shaders in a user-friendly visual graph format.
    /// </summary>
    public partial class EditorUIPropAnimFloat : EditorUI2DBase, I2DRenderable, IPreRendered
    {
        public EditorUIPropAnimFloat() : base() { }
        public EditorUIPropAnimFloat(Vec2 bounds) : base(bounds) { }

        protected UITextRasterComponent _xCoord, _yCoord;
        protected UIString2D _xString, _yString;
        
        private readonly RenderCommandMesh2D _rcKfLines = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        private readonly RenderCommandMesh2D _rcSpline = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        private readonly RenderCommandMesh2D _rcKeyframeInOutPositions = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        private readonly RenderCommandMesh2D _rcTangentPositions = new RenderCommandMesh2D(ERenderPass.OnTopForward);

        [Browsable(false)]
        public bool IsRenderingTangents => !AutoGenerateTangents && RenderKeyframeTangents;
        [Browsable(false)]
        public bool IsDraggingKeyframes => _draggedKeyframes.Count > 0;
        [Browsable(false)]
        public int[] ClosestPositionIndices { get; private set; }
        [Browsable(false)]
        public Vec2 AnimPositionWorld { get; private set; }

        private Vec3[] KeyframeInOutPosInOutTan { get; set; }
        //public float VelocitySigmoidScale { get; set; } = 0.002f;
        private float DisplayFPS { get; set; } = 0.0f;
        private float AnimLength { get; set; } = 0.0f;
        private int KeyCount { get; set; } = 0;

        [TSerialize]
        public EVectorInterpValueType ValueDisplayMode { get; private set; } = EVectorInterpValueType.Position;
        [TSerialize]
        public bool AutoGenerateTangents { get; set; }
        [TSerialize]
        public float TangentScale = 50.0f;
        [TSerialize]
        public bool RenderKeyframeTangents { get; set; } = true;

        private bool _renderAnimPosition = true;
        [TSerialize]
        public bool RenderAnimPosition
        {
            get => _renderAnimPosition;
            set
            {
                _renderAnimPosition = value;
                _xCoord.RenderInfo.IsVisible = _renderAnimPosition;
                _yCoord.RenderInfo.IsVisible = _renderAnimPosition;
            }
        }
        
        private PropAnimFloat _targetAnimation;
        private Dictionary<int, DraggedKeyframeInfo> _draggedKeyframes = new Dictionary<int, DraggedKeyframeInfo>();
        private Vec2 _selectionStartPosAnimRelative;

        private bool _regenerating = false;
        
        public PropAnimFloat TargetAnimation
        {
            get => _targetAnimation;
            set
            {
                if (Set(ref _targetAnimation, value,
                    () =>
                    {
                        _targetAnimation.Keyframes.Changed -= OnChanged;
                        //_targetAnimation.SpeedChanged -= OnSpeedChanged;
                        _targetAnimation.ConstrainKeyframedFPSChanged -= OnConstrainKeyframedFPSChanged;
                        _targetAnimation.BakedFPSChanged -= OnBakedFPSChanged;
                        _targetAnimation.LengthChanged -= _position_LengthChanged;
                        _targetAnimation.CurrentPositionChanged -= OnCurrentPositionChanged;
                    },
                    () =>
                    {
                        _targetAnimation.Keyframes.Changed += OnChanged;
                        //_targetAnimation.SpeedChanged += OnSpeedChanged;
                        _targetAnimation.ConstrainKeyframedFPSChanged += OnConstrainKeyframedFPSChanged;
                        _targetAnimation.BakedFPSChanged += OnBakedFPSChanged;
                        _targetAnimation.LengthChanged += _position_LengthChanged;
                        _targetAnimation.CurrentPositionChanged += OnCurrentPositionChanged;
                        _targetAnimation.TickSelf = true;
                        _targetAnimation.Progress(0.0f);
                    }))
                {
                    ZoomExtents();
                    RegenerateSplinePrimitive();
                }
            }
        }

        private async void OnBakedFPSChanged(BasePropAnimBakeable obj)
            => await RegenerateSplinePrimitiveAsync();
        private async void _position_LengthChanged(BaseAnimation obj)
            => await RegenerateSplinePrimitiveAsync();
        private async void OnConstrainKeyframedFPSChanged(PropAnimVector<float, FloatKeyframe> obj)
            => await RegenerateSplinePrimitiveAsync();
        private void OnChanged(BaseKeyframeTrack obj) 
            => UpdateSplinePrimitive();
        private void OnSpeedChanged(BaseAnimation obj)
            => UpdateSplinePrimitive();

        const float Resolution = 1.0f;
        private float MinSec { get; set; }
        private float VisibleSecRange { get; set; }
        private int FrameCount { get; set; }

        public void UpdateSplinePrimitive(bool renderPass = false)
        {
            if ((!Engine.IsSingleThreaded && !renderPass) || _regenerating)
            {
                QueueSplineUpdate = true;
                return;
            }

            if (_targetAnimation is null)
                return;

            if (AnimLength != _targetAnimation.LengthInSeconds ||
                KeyCount != _targetAnimation.Keyframes.Count ||
                _rcSpline.Mesh is null)
            {
                RegenerateSplinePrimitive();
                return;
            }

            if (DisplayFPS <= 0.0f)
                return;

            float displayFPS;
            GetWorkArea(out Vec2 animMin, out Vec2 animMax);
            GetViewportBounds(out Vec2 boundsMin, out Vec2 boundsMax);
            float maxSec = TMath.Min(boundsMax.X, animMax.X);
            float minSec = TMath.Max(boundsMin.X, animMin.X);
            //float maxVal = TMath.Min(boundsMax.Y, animMax.Y);
            //float minVal = TMath.Max(boundsMin.Y, animMin.Y);
            float visibleAnimSecRange = maxSec - minSec;
            //float visibleAnimValRange = maxVal - minVal;

            //if (_targetAnimation.ConstrainKeyframedFPS || _targetAnimation.IsBaked)
            //{
            //    displayFPS = _targetAnimation.BakedFramesPerSecond * Resolution;
            //    minSec = 0.0f;
            //    visibleAnimSecRange = maxSec = _targetAnimation.LengthInSeconds;
            //}
            //else
            {
                //TODO: when the FPS is unconstrained, use adaptive vertex points based on velocity/acceleration
                displayFPS = Bounds.X / visibleAnimSecRange * Resolution;
            }
            float secondsPerFrame = 1.0f / displayFPS;

            int frameCount = (int)Math.Ceiling(visibleAnimSecRange * displayFPS) + 1;

            if (Math.Abs(frameCount - FrameCount) > 1)
            {
                RegenerateSplinePrimitive();
                return;
            }

            float maxVel = GetMaxSpeed();

            var posBuf = _rcSpline.Mesh.Data[EBufferType.Position];
            var colBuf = _rcSpline.Mesh.Data[EBufferType.Color];
            var kfPosBuf = _rcKeyframeInOutPositions.Mesh.Data[EBufferType.Position];
            var tanPosBuf = _rcTangentPositions.Mesh.Data[EBufferType.Position];
            var keyLinesBuf = _rcKfLines.Mesh.Data[EBufferType.Position];

            int i;
            float sec = minSec;
            float timeIncrement = 1.0f / displayFPS;
            for (i = 0; i < posBuf.ElementCount; ++i, sec += timeIncrement)
            {
                GetSplineVertex(sec, maxVel, out Vec3 pos, out ColorF4 color);

                posBuf.Set(i, pos);
                colBuf.Set(i, color);
            }

            i = 0;
            int i2;
            foreach (FloatKeyframe kf in _targetAnimation)
            {
                i2 = i << 1;
                GetKeyframePosInfo(kf, out Vec3 inPos, out Vec3 outPos, out Vec3 inTanPos, out Vec3 outTanPos);
                
                KeyframeInOutPosInOutTan[i2] = inPos;
                KeyframeInOutPosInOutTan[i2 + 1] = outPos;

                kfPosBuf.Set(i, inPos);
                tanPosBuf.Set(i, inTanPos);
                keyLinesBuf.Set(i2, inPos);
                keyLinesBuf.Set(i2 + 1, inTanPos);

                ++i;
                i2 = i << 1;

                KeyframeInOutPosInOutTan[i2] = inTanPos;
                KeyframeInOutPosInOutTan[i2 + 1] = outTanPos;

                kfPosBuf.Set(i, outPos);
                tanPosBuf.Set(i, outTanPos);
                keyLinesBuf.Set(i2, outPos);
                keyLinesBuf.Set(i2 + 1, outTanPos);

                ++i;
            }
        }
        public async Task RegenerateSplinePrimitiveAsync()
            => await Task.Run((Action)RegenerateSplinePrimitive);
        public void RegenerateSplinePrimitive()
        {
            _regenerating = true;
            _rcKeyframeInOutPositions.Mesh?.Dispose();
            _rcKeyframeInOutPositions.Mesh = null;
            _rcTangentPositions.Mesh?.Dispose();
            _rcTangentPositions.Mesh = null;
            _rcKfLines.Mesh?.Dispose();
            _rcKfLines.Mesh = null;

            if (_targetAnimation is null || (AnimLength = _targetAnimation.LengthInSeconds) <= 0.0f)
            {
                _regenerating = false;
                return;
            }

            GetWorkArea(out Vec2 animMin, out Vec2 animMax);
            GetViewportBounds(out Vec2 boundsMin, out Vec2 boundsMax);
            float maxSec = TMath.Min(boundsMax.X, animMax.X);
            float minSec = TMath.Max(boundsMin.X, animMin.X);
            //float maxVal = TMath.Min(boundsMax.Y, animMax.Y);
            //float minVal = TMath.Max(boundsMin.Y, animMin.Y);
            float visibleAnimSecRange = maxSec - minSec;
            //float visibleAnimValRange = maxVal - minVal;

            if (visibleAnimSecRange <= 0.0f)
                visibleAnimSecRange = Bounds.X;
            DisplayFPS = Bounds.X / visibleAnimSecRange * Resolution;
            
            float secondsPerFrame = 1.0f / DisplayFPS;

            int frameCount = (int)Math.Ceiling(visibleAnimSecRange * DisplayFPS) + 1;

            MinSec = minSec;
            VisibleSecRange = visibleAnimSecRange;
            FrameCount = frameCount;

            int posCount = (KeyCount = _targetAnimation.Keyframes.Count) << 1;
            Vertex[] splinePoints = new Vertex[frameCount];
            VertexLine[] keyframeLines = new VertexLine[posCount];
            Vec3[] kfInOutPositions = new Vec3[posCount];
            Vec3[] kfInOutTangents = new Vec3[posCount];
            KeyframeInOutPosInOutTan = new Vec3[posCount << 1];

            float maxVel = GetMaxSpeed();
            int i;
            float sec = minSec;
            for (i = 0; i < splinePoints.Length; ++i, sec += secondsPerFrame)
            {
                GetSplineVertex(sec, maxVel, out Vec3 pos, out ColorF4 color);
                splinePoints[i] = new Vertex(pos, color);
            }

            i = 0;
            int i2 = 0;
            foreach (FloatKeyframe kf in _targetAnimation)
            {
                i2 = i << 1;
                GetKeyframePosInfo(kf, out Vec3 inPos, out Vec3 outPos, out Vec3 inTanPos, out Vec3 outTanPos);

                KeyframeInOutPosInOutTan[i2] = inPos;
                KeyframeInOutPosInOutTan[i2 + 1] = outPos;

                kfInOutPositions[i] = inPos;
                kfInOutTangents[i] = inTanPos;
                keyframeLines[i] = new VertexLine(inPos, inTanPos);

                ++i;
                i2 = i << 1;

                KeyframeInOutPosInOutTan[i2] = inTanPos;
                KeyframeInOutPosInOutTan[i2 + 1] = outTanPos;

                kfInOutPositions[i] = outPos;
                kfInOutTangents[i] = outTanPos;
                keyframeLines[i] = new VertexLine(outPos, outTanPos);
                ++i;
            }

            VertexLineStrip strip = new VertexLineStrip(false, splinePoints);

            RenderingParameters renderParams = new RenderingParameters
            {
                LineWidth = 1.0f,
                PointSize = 5.0f
            };

            PrimitiveData splinePosColor = PrimitiveData.FromLineStrips(VertexShaderDesc.PosColor(), strip);
            foreach (var buf in splinePosColor)
            {
                buf.MapData = true;
                buf.Usage = EBufferUsage.DynamicDraw;
            }
            
            TMaterial mat = new TMaterial("SplineColor", new GLSLScript(EGLSLType.Fragment,
@"
#version 450

layout (location = 0) out vec4 OutColor;
layout (location = 4) in vec4 FragColor0;

void main()
{
    OutColor = FragColor0;
}
")) { RenderParams = renderParams };

            _rcSpline.Mesh = new PrimitiveManager(splinePosColor, mat);
            
            PrimitiveData kfInOutPos = PrimitiveData.FromPoints(kfInOutPositions);
            foreach (var buf in kfInOutPos)
            {
                buf.MapData = true;
                buf.Usage = EBufferUsage.DynamicDraw;
            }
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Green);
            mat.RenderParams = renderParams;
            _rcKeyframeInOutPositions.Mesh = new PrimitiveManager(kfInOutPos, mat);

            PrimitiveData tanPos = PrimitiveData.FromPoints(kfInOutTangents);
            foreach (var buf in tanPos)
            {
                buf.MapData = true;
                buf.Usage = EBufferUsage.DynamicDraw;
            }
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Purple);
            mat.RenderParams = renderParams;
            _rcTangentPositions.Mesh = new PrimitiveManager(tanPos, mat);

            PrimitiveData kfLines = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), keyframeLines);
            foreach (var buf in kfLines)
            {
                buf.MapData = true;
                buf.Usage = EBufferUsage.DynamicDraw;
            }
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Orange);
            mat.RenderParams = renderParams;
            _rcKfLines.Mesh = new PrimitiveManager(kfLines, mat);
            
            _regenerating = false;
        }
        public override void ZoomExtents(bool adjustScale = true)
        {
            base.ZoomExtents(adjustScale);
            UpdateSplinePrimitive();
        }
        protected override UICanvasComponent OnConstructRoot()
        {
            var root = base.OnConstructRoot();

            _xCoord = ConstructText(ColorF4.White, "0.0", "-0000.000", out _xString);
            _yCoord = ConstructText(ColorF4.White, "0.0", "-0000.000", out _yString);

            return root;
        }
        public override string XUnitString => "Sec";
        public override string YUnitString => "Value";
        private void OnCurrentPositionChanged(PropAnimVector<float, FloatKeyframe> obj)
        {
            Vec3 pos = new Vec3(_targetAnimation.CurrentTime, GetCurrentPosition(), 0.0f);
            AnimPositionWorld = Vec3.TransformPosition(pos, OriginTransformComponent.WorldMatrix).Xy;

            Vec2 origin = GetViewportBottomLeft();

            _xCoord.Translation.X = pos.X;
            _yCoord.Translation.Y = pos.Y;

            _xCoord.Translation.Y = origin.Y;
            _yCoord.Translation.X = origin.X;
            
            _xString.Text = pos.X.ToString("###0.0##");
            _yString.Text = pos.Y.ToString("###0.0##");

            OriginTransformComponent.InvalidateLayout();
        }
        private bool _redrewLastMove = false;
        protected override void OriginTransformComponent_WorldTransformChanged(ISceneComponent obj)
        {
            Matrix4 mtx = OriginTransformComponent.WorldMatrix;

            _rcKfLines.WorldMatrix =
            _rcSpline.WorldMatrix =
            _rcKeyframeInOutPositions.WorldMatrix =
            _rcTangentPositions.WorldMatrix = mtx;

            if (RenderAnimPosition = _targetAnimation != null)
            {
                Vec3 animPos = new Vec3(_targetAnimation.CurrentTime, GetCurrentPosition(), 0.0f);
                _xCoord.Translation.X = animPos.X;
                _yCoord.Translation.Y = animPos.Y;
                AnimPositionWorld = Vec3.TransformPosition(animPos, mtx).Xy;
            }
            else
            {
                _xCoord.Translation.X = 0.0f;
                _yCoord.Translation.Y = 0.0f;
                AnimPositionWorld = mtx.TranslationXy;
            }

            Vec2 origin = GetViewportBottomLeft();
            _xCoord.Translation.Y = origin.Y;
            _yCoord.Translation.X = origin.X;

            Vec2 bl = GetViewportBottomLeft();
            Vec2 tr = GetViewportTopRight();

            bool shouldRedraw = 0.0f < bl.X || (TargetAnimation?.LengthInSeconds ?? 0.0f) > tr.X;
            if (shouldRedraw || _redrewLastMove)
            {
                UpdateSplinePrimitive();
                _redrewLastMove = shouldRedraw;
            }

            base.OriginTransformComponent_WorldTransformChanged(obj);
        }
        //protected override void ResizeLayout()
        //{
        //    base.ResizeLayout();

        //}

        protected float _moveTimeLeft = 0.0f;
        protected float _moveTimeRight = 0.0f;
        protected override bool ShouldTickInput
            => base.ShouldTickInput || (_targetAnimation != null && (_moveTimeLeft + _moveTimeRight) != 0.0f);
        protected override void TickInput(float delta)
        {
            base.TickInput(delta);

            float moveTime = _moveTimeLeft + _moveTimeRight;
            if (moveTime != 0.0f)
                _targetAnimation.Progress(delta * moveTime);
        }

        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
            
            input.RegisterKeyEvent(EKey.Space, EButtonInputType.Pressed, TogglePlay, EInputPauseType.TickAlways);

            input.RegisterKeyPressed(EKey.Left, MoveTimeLeft, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.Right, MoveTimeRight, EInputPauseType.TickAlways);
            //input.RegisterKeyPressed(EKey.Down, MoveTimeDown, EInputPauseType.TickAlways);
            //input.RegisterKeyPressed(EKey.Up, MoveTimeUp, EInputPauseType.TickAlways);
        }

        private void MoveTimeRight(bool pressed)
        {
            _moveTimeRight = pressed ? 1.0f : 0.0f;
            UpdateInputTick();
        }
        private void MoveTimeLeft(bool pressed)
        {
            _moveTimeLeft = pressed ? -1.0f : 0.0f;
            UpdateInputTick();
        }

        private void TogglePlay()
        {
            if (_targetAnimation is null)
                return;

            _targetAnimation.State = _targetAnimation.State == EAnimationState.Playing ? EAnimationState.Paused : EAnimationState.Playing;
        }
        
        protected override void OnShiftPressed(bool pressed)
        {
            base.OnShiftPressed(pressed);
            if (IsDraggingKeyframes)
                HandleDragItem();
        }

        protected override void OnLeftClickDown()
        {
            Vec2 v = CursorPosition();
            if (!Bounds.Raw.Contains(v))
                return;

            if (_targetAnimation != null)
            {
                Vec2 animPos = CursorPositionTransformRelative();
                float sec = animPos.X;
                float val = animPos.Y;

                if (ClosestPositionIndices != null && ClosestPositionIndices.Length > 0)
                {
                    //Keyframes are within the selection radius

                    _draggedKeyframes.Clear();
                    _selectionStartPosAnimRelative = animPos;
                    HashSet<int> uniqueKfs = new HashSet<int>();
                    foreach (int index in ClosestPositionIndices)
                    {
                        int kfIndex = index >> 2;
                        uniqueKfs.Add(kfIndex);
                        var kf = _targetAnimation.Keyframes[kfIndex];

                        // 00 inPos
                        // 01 outPos
                        // 10 inTan
                        // 11 outTan

                        bool inVal = (index & 1) == 0;
                        bool pos = (index & 2) == 0;

                        bool draggingInValue = pos && (inVal || kf.SyncInOutValues);
                        bool draggingOutValue = pos && (!inVal || kf.SyncInOutValues);
                        bool draggingPos = draggingInValue || draggingOutValue;

                        bool draggingInTangent = !draggingPos && ((!pos && inVal) || kf.SyncInOutTangentDirections || kf.SyncInOutTangentMagnitudes);
                        bool draggingOutTangent = !draggingPos && ((!pos && !inVal) || kf.SyncInOutTangentDirections || kf.SyncInOutTangentMagnitudes);
                        
                        if (_draggedKeyframes.ContainsKey(kfIndex))
                        {
                            DraggedKeyframeInfo info = _draggedKeyframes[kfIndex];
                            info.DraggingInValue = info.DraggingInValue || draggingInValue;
                            info.DraggingOutValue = info.DraggingOutValue || draggingOutValue;
                            info.DraggingInTangent = info.DraggingInTangent || draggingInTangent;
                            info.DraggingOutTangent = info.DraggingOutTangent || draggingOutTangent;
                        }
                        else
                        {
                            _draggedKeyframes.Add(kfIndex, new DraggedKeyframeInfo()
                            {
                                Keyframe = kf,

                                DraggingInValue = draggingInValue,
                                DraggingOutValue = draggingOutValue,
                                DraggingInTangent = draggingInTangent,
                                DraggingOutTangent = draggingOutTangent,

                                SecondOffset = kf.Second - animPos.X,
                                InValueOffset = kf.InValue - animPos.Y,
                                OutValueOffset = kf.OutValue - animPos.Y,

                                InTangentOffset = kf.InValue + kf.InTangent - animPos.Y,
                                OutTangentOffset = kf.OutValue + kf.OutTangent - animPos.Y,

                                SecondInitial = kf.Second,
                                InValueInitial = kf.InValue,
                                OutValueInitial = kf.OutValue,
                                InTangentInitial = kf.InTangent,
                                OutTangentInitial = kf.OutTangent,
                            });
                        }
                    }
                    if (uniqueKfs.Count == 1)
                    {
                        //var graph = Viewport.OwningPanel.FindForm() as DockablePropAnimFloatGraph;
                        //graph.DockPanel
                        Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = _targetAnimation.Keyframes[ClosestPositionIndices[0] >> 2];
                    }
                    else
                        Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = _targetAnimation;
                }
                else if (CtrlDown)
                {
                    //Create a keyframe under the cursor location

                    float length = TMath.Max(_targetAnimation.LengthInSeconds, sec);
                    var track = _targetAnimation.Keyframes;
                    bool nullTrack = track is null;
                    if (length > _targetAnimation.LengthInSeconds)
                        _targetAnimation.SetLength(length, false, nullTrack);

                    if (!nullTrack)
                    {
                        var prevKf = track.GetKeyBefore(sec);
                        var nextKf = prevKf?.Next;
                        if (prevKf != null && prevKf.Second.EqualTo(sec, 0.01f))
                        {
                            prevKf.OutValue = val;
                        }
                        else if (nextKf != null && nextKf.Second.EqualTo(sec, 0.01f))
                        {
                            nextKf.InValue = val;
                        }
                        else
                        {
                            FloatKeyframe kf = new FloatKeyframe(sec, val, 0.0f, EVectorInterpType.CubicHermite);
                            track.Add(kf);
                            kf.GenerateTangents();

                            //This will update the animation position
                            _targetAnimation.Progress(0.0f);
                        }
                    }
                }
                else
                {
                    //Begin drag selection
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = _targetAnimation;
                }
            }
        }
        protected override void OnLeftClickUp()
        {
            if (_draggedKeyframes.Count > 0)
            {
                //LocalValueChange[] changes = new LocalValueChange[_draggedKeyframes.Count];
                //int i = 0;
                //foreach (var kf in _draggedKeyframes)
                //{
                //    var info = kf.Value;
                //    LocalValueChange change = new LocalValueChangeProperty();
                //    Editor.Instance.UndoManager.AddChange(DragComponent.EditorState, _prevDragMatrix, DragComponent.WorldMatrix, DragComponent, DragComponent.GetType().GetProperty(nameof(DragComponent.WorldMatrix)));
                //}
                _draggedKeyframes.Clear();
            }
        }
        protected override bool IsDragging => _draggedKeyframes.Count > 0;

        public bool QueueSplineUpdate { get; private set; }

        private class DraggedKeyframeInfo
        {
            public FloatKeyframe Keyframe { get; set; }

            public bool DraggingInValue { get; set; } = false;
            public bool DraggingOutValue { get; set; } = false;
            public bool DraggingInTangent { get; set; } = false;
            public bool DraggingOutTangent { get; set; } = false;

            public float SecondOffset { get; set; }
            public float InValueOffset { get; set; }
            public float OutValueOffset { get; set; }
            public float InTangentOffset { get; set; }
            public float OutTangentOffset { get; set; }
            
            public float SecondInitial { get; set; }
            public float InValueInitial { get; set; }
            public float OutValueInitial { get; set; }
            public float InTangentInitial { get; set; }
            public float OutTangentInitial { get; set; }
        }
        protected override void HandleDragItem()
        {
            Vec3 pos = CursorPositionTransformRelative();

            if (ShiftDown)
            {
                //Ortho mode
                float xDist = Math.Abs(pos.X - _selectionStartPosAnimRelative.X);
                float yDist = Math.Abs(pos.Y - _selectionStartPosAnimRelative.Y);
                if (xDist > yDist)
                {
                    //Drag left and right only
                    foreach (var kf in _draggedKeyframes)
                    {
                        if (kf.Value.DraggingInValue || kf.Value.DraggingOutValue)
                        {
                            float sec = pos.X + kf.Value.SecondOffset;
                            if (SnapToUnits)
                                sec = sec.RoundedToNearestMultiple(UnitIncrementX);
                            kf.Value.Keyframe.Second = sec;

                            if (kf.Value.DraggingInValue)
                                kf.Value.Keyframe.InValue = kf.Value.InValueInitial;
                            if (kf.Value.DraggingOutValue)
                                kf.Value.Keyframe.OutValue = kf.Value.OutValueInitial;
                        }
                        else
                        {
                            if (kf.Value.DraggingInTangent)
                                kf.Value.Keyframe.InTangent = kf.Value.InTangentInitial;
                            if (kf.Value.DraggingOutTangent)
                                kf.Value.Keyframe.OutTangent = kf.Value.OutTangentInitial;
                        }
                    }
                }
                else
                {
                    //Drag up and down only
                    foreach (var kf in _draggedKeyframes)
                    {
                        if (kf.Value.DraggingInValue || kf.Value.DraggingOutValue)
                        {
                            kf.Value.Keyframe.Second = kf.Value.SecondInitial;

                            bool syncVals = kf.Value.Keyframe.SyncInOutValues;
                            bool syncTanMags = kf.Value.Keyframe.SyncInOutTangentMagnitudes;
                            bool syncTanDirs = kf.Value.Keyframe.SyncInOutTangentDirections;
                            if (kf.Value.DraggingInValue && kf.Value.DraggingOutValue)
                            {
                                kf.Value.Keyframe.SyncInOutValues = false;
                                kf.Value.Keyframe.SyncInOutTangentDirections = false;
                                kf.Value.Keyframe.SyncInOutTangentMagnitudes = false;
                            }

                            if (kf.Value.DraggingInValue)
                            {
                                float inPos = pos.Y + kf.Value.InValueOffset;
                                if (SnapToUnits)
                                    inPos = inPos.RoundedToNearestMultiple(UnitIncrementX);
                                kf.Value.Keyframe.InValue = inPos;
                                if (AutoGenerateTangents)
                                {
                                    kf.Value.Keyframe.GenerateAdjacentTangents(true, false);
                                }
                            }
                            if (kf.Value.DraggingOutValue)
                            {
                                float outPos = pos.Y + kf.Value.OutValueOffset;
                                if (SnapToUnits)
                                    outPos = outPos.RoundedToNearestMultiple(UnitIncrementX);
                                kf.Value.Keyframe.OutValue = outPos;
                                if (AutoGenerateTangents)
                                {
                                    kf.Value.Keyframe.GenerateAdjacentTangents(false, true);
                                }
                            }
                            if (AutoGenerateTangents && kf.Value.DraggingInValue && kf.Value.DraggingOutValue)
                            {
                                kf.Value.Keyframe.UnifyTangents(EUnifyBias.Average);
                            }

                            kf.Value.Keyframe.SyncInOutValues = syncVals;
                            kf.Value.Keyframe.SyncInOutTangentDirections = syncTanDirs;
                            kf.Value.Keyframe.SyncInOutTangentMagnitudes = syncTanMags;
                        }
                        else
                        {
                            if (kf.Value.DraggingInTangent)
                            {
                                float inPos = pos.Y + kf.Value.InValueOffset;
                                if (SnapToUnits)
                                    inPos = inPos.RoundedToNearestMultiple(UnitIncrementX);
                                kf.Value.Keyframe.InTangent = inPos;
                            }
                            if (kf.Value.DraggingOutTangent)
                            {
                                float outPos = pos.Y + kf.Value.OutValueOffset;
                                if (SnapToUnits)
                                    outPos = outPos.RoundedToNearestMultiple(UnitIncrementX);
                                kf.Value.Keyframe.OutTangent = outPos;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var kf in _draggedKeyframes)
                {
                    float kfSec = pos.X + kf.Value.SecondOffset;
                    if (SnapToUnits)
                        kfSec = kfSec.RoundedToNearestMultiple(UnitIncrementX);

                    if (kf.Value.DraggingInValue || kf.Value.DraggingOutValue)
                    {
                        kf.Value.Keyframe.Second = kfSec;

                        bool syncVals = kf.Value.Keyframe.SyncInOutValues;
                        bool syncTanMags = kf.Value.Keyframe.SyncInOutTangentMagnitudes;
                        bool syncTanDirs = kf.Value.Keyframe.SyncInOutTangentDirections;
                        if (kf.Value.DraggingInValue && kf.Value.DraggingOutValue)
                        {
                            kf.Value.Keyframe.SyncInOutValues = false;
                            kf.Value.Keyframe.SyncInOutTangentDirections = false;
                            kf.Value.Keyframe.SyncInOutTangentMagnitudes = false;
                        }

                        if (kf.Value.DraggingInValue)
                        {
                            float inPos = pos.Y + kf.Value.InValueOffset;
                            if (SnapToUnits)
                                inPos = inPos.RoundedToNearestMultiple(UnitIncrementX);
                            kf.Value.Keyframe.InValue = inPos;
                            if (AutoGenerateTangents)
                            {
                                kf.Value.Keyframe.GenerateAdjacentTangents(true, false);
                            }
                        }
                        if (kf.Value.DraggingOutValue)
                        {
                            float outPos = pos.Y + kf.Value.OutValueOffset;
                            if (SnapToUnits)
                                outPos = outPos.RoundedToNearestMultiple(UnitIncrementX);
                            kf.Value.Keyframe.OutValue = outPos;
                            if (AutoGenerateTangents)
                            {
                                kf.Value.Keyframe.GenerateAdjacentTangents(false, true);
                            }
                        }
                        if (AutoGenerateTangents && kf.Value.DraggingInValue && kf.Value.DraggingOutValue)
                        {
                            kf.Value.Keyframe.UnifyTangents(EUnifyBias.Average);
                        }

                        kf.Value.Keyframe.SyncInOutValues = syncVals;
                        kf.Value.Keyframe.SyncInOutTangentDirections = syncTanDirs;
                        kf.Value.Keyframe.SyncInOutTangentMagnitudes = syncTanMags;
                    }
                    else
                    {
                        float tangentScale = TangentScale / OriginTransformComponent.Scale.X;
                     
                        if (kf.Value.DraggingInTangent)
                        {
                            float posY = pos.Y/* + kf.Value.InValueOffset*/;
                            if (SnapToUnits)
                                posY = posY.RoundedToNearestMultiple(UnitIncrementX);

                            //m = y / x
                            //y = pos.Y - kf.Value.InValueInitial
                            //x = pos.X - kf.Value.SecondInitial
                            kf.Value.Keyframe.InTangent = -(posY - kf.Value.InValueInitial) / (pos.X - kf.Value.SecondInitial);
                        }
                        if (kf.Value.DraggingOutTangent)
                        {
                            float posY = pos.Y/* + kf.Value.OutValueOffset*/;
                            
                            if (SnapToUnits)
                                posY = posY.RoundedToNearestMultiple(UnitIncrementX);
                            
                            kf.Value.Keyframe.OutTangent = (posY - kf.Value.OutValueInitial) / (pos.X - kf.Value.SecondInitial);
                        }
                    }
                }
            }

            //This will update the animation position
            _targetAnimation.Progress(0.0f);
            
            UpdateSplinePrimitive();
        }
        protected override void HighlightScene()
        {
            ClosestPositionIndices = null;
            if (KeyframeInOutPosInOutTan is null)
                return;

            Vec2 cursorPos = CursorPositionTransformRelative();
            float radius = SelectionRadius / OriginTransformComponent.Scale.X;
            ClosestPositionIndices = KeyframeInOutPosInOutTan.FindAllMatchIndices(x => x.DistanceToFast(cursorPos) < radius);
        }
        protected override void OnScrolledInput(bool down)
        {
            if (CtrlDown)
                SelectionRadius *= down ? 1.1f : 0.91f;
            else
                Zoom(down ? ZoomScrollIncrement : -ZoomScrollIncrement);
        }
        protected override void AddRenderables(RenderPasses passes)
        {
            if (_targetAnimation is null)
                return;
            
            passes.Add(_rcSpline);
            passes.Add(_rcKeyframeInOutPositions);

            if (IsRenderingTangents)
            {
                passes.Add(_rcTangentPositions);
                passes.Add(_rcKfLines);
            }
        }
        protected override void RenderMethod()
        {
            Vec2 wh = _backgroundComponent.ActualSize.Raw;

            //TODO: if a keyframe is dragged past another, its index changes but these indices are not updated
            int[] close = ClosestPositionIndices;
            if (close != null)
                foreach (int index in close)
                    if (KeyframeInOutPosInOutTan?.IndexInRange(index) ?? false)
                        Engine.Renderer.RenderPoint(Vec3.TransformPosition(KeyframeInOutPosInOutTan[index], OriginTransformComponent.WorldMatrix), Color.Yellow, false, 10.0f);

            base.RenderMethod();

            //Animation end line
            if (_targetAnimation != null && _targetAnimation.Keyframes.Count > 0)
            {
                Vec3 end = Vec3.TransformPosition(new Vec2(_targetAnimation.LengthInSeconds, 0.0f), OriginTransformComponent.WorldMatrix);
                Engine.Renderer.RenderLine(new Vec2(end.X, 0.0f), new Vec2(end.X, wh.Y), new ColorF4(0.7f, 0.3f, 0.3f), false, 10.0f);
            }

            if (RenderAnimPosition)
            {
                Engine.Renderer.RenderPoint(AnimPositionWorld, new ColorF4(1.0f), false, 10.0f);
                Engine.Renderer.RenderLine(new Vec2(AnimPositionWorld.X, 0.0f), new Vec2(AnimPositionWorld.X, wh.Y), new ColorF4(1.0f), false, 10.0f);
                Engine.Renderer.RenderLine(new Vec2(0.0f, AnimPositionWorld.Y), new Vec2(wh.X, AnimPositionWorld.Y), new ColorF4(1.0f), false, 10.0f);

                if (_targetAnimation != null)
                {
                    Vec2 vel = new Vec2(1.0f, GetCurrentVelocity());
                    vel.Normalize();
                    vel *= TangentScale;
                    Engine.Renderer.RenderLine(AnimPositionWorld, AnimPositionWorld + vel, new ColorF4(0.5f, 0.5f, 0.0f), false, 10.0f);

                    Vec2 acc = new Vec2(1.0f, GetCurrentAcceleration());
                    acc.Normalize();
                    acc *= TangentScale;
                    Engine.Renderer.RenderLine(AnimPositionWorld, AnimPositionWorld + acc, new ColorF4(0.5f, 0.0f, 0.5f), false, 10.0f);
                }
            }
        }

        public bool PreRenderEnabled => !Engine.IsSingleThreaded && QueueSplineUpdate;
        public void PreRenderUpdate(ICamera camera) { }
        public void PreRenderSwap()
        {
            QueueSplineUpdate = false;
            UpdateSplinePrimitive(true);
        }
        public void PreRender(Viewport viewport, ICamera camera) { }
    }
}
