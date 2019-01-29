using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Animation;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
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
    public class UIPropAnimFloatEditor : EditorUserInterface, I2DRenderable
    {
        public UIPropAnimFloatEditor() : base() { }
        public UIPropAnimFloatEditor(Vec2 bounds) : base(bounds) { }

        protected UITextComponent _xCoord, _yCoord;
        protected UIString2D _xString, _yString;
        
        private readonly RenderCommandMesh2D _rcKfLines = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        private readonly RenderCommandMesh2D _rcSpline = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        private readonly RenderCommandMesh2D _rcKeyframeInOutPositions = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        private readonly RenderCommandMesh2D _rcTangentPositions = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        
        public bool IsDraggingKeyframes => _draggedKeyframes.Count > 0;
        public int[] ClosestPositionIndices { get; private set; }
        private Vec3[] KeyframeInOutPosInOutTan { get; set; }
        //public float VelocitySigmoidScale { get; set; } = 0.002f;
        private float DisplayFPS { get; set; } = 0.0f;
        private float AnimLength { get; set; } = 0.0f;
        private int KeyCount { get; set; } = 0;
        public Vec2 AnimPositionWorld { get; private set; }
        [TSerialize]
        public EVectorInterpValueType ValueDisplayMode { get; private set; } = EVectorInterpValueType.Position;
        
        [TSerialize]
        public bool RenderExtrema { get; set; } = true;
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
                _xCoord.RenderInfo.Visible = _renderAnimPosition;
                _yCoord.RenderInfo.Visible = _renderAnimPosition;
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
                if (_targetAnimation != null)
                {
                    _targetAnimation.Keyframes.Changed -= OnChanged;
                    //_targetAnimation.SpeedChanged -= OnSpeedChanged;
                    _targetAnimation.ConstrainKeyframedFPSChanged -= OnConstrainKeyframedFPSChanged;
                    _targetAnimation.BakedFPSChanged -= OnBakedFPSChanged;
                    _targetAnimation.LengthChanged -= _position_LengthChanged;
                    _targetAnimation.CurrentPositionChanged -= OnCurrentPositionChanged;
                }
                _targetAnimation = value;
                if (_targetAnimation != null)
                {
                    _targetAnimation.Keyframes.Changed += OnChanged;
                    //_targetAnimation.SpeedChanged += OnSpeedChanged;
                    _targetAnimation.ConstrainKeyframedFPSChanged += OnConstrainKeyframedFPSChanged;
                    _targetAnimation.BakedFPSChanged += OnBakedFPSChanged;
                    _targetAnimation.LengthChanged += _position_LengthChanged;
                    _targetAnimation.CurrentPositionChanged += OnCurrentPositionChanged;
                    _targetAnimation.TickSelf = true;

                    ZoomExtents();
                }
                RegenerateSplinePrimitive();
            }
        }

        private async void OnBakedFPSChanged(BasePropAnimBakeable obj)
        {
            await RegenerateSplinePrimitiveAsync();
        }
        private async void _position_LengthChanged(BaseAnimation obj)
        {
            await RegenerateSplinePrimitiveAsync();
        }
        private async void OnConstrainKeyframedFPSChanged(PropAnimVector<float, FloatKeyframe> obj)
        {
            await RegenerateSplinePrimitiveAsync();
        }
        private void OnChanged(BaseKeyframeTrack obj)
        {
            UpdateSplinePrimitive();
        }
        private void OnSpeedChanged(BaseAnimation obj)
        {
            UpdateSplinePrimitive();
        }

        private void GetKeyframeInfo(FloatKeyframe kf, out Vec3 inPos, out Vec3 outPos, out Vec3 inTanPos, out Vec3 outTanPos)
        {
            float tangentScale = 50.0f / _baseTransformComponent.ScaleX;
            float velOut = kf.OutTangent;
            float velIn = kf.InTangent;

            Vec2 tangentInVector = new Vec2(-tangentScale, velIn * tangentScale);
            Vec2 tangentOutVector = new Vec2(tangentScale, velOut * tangentScale);
            //tangentInVector.Normalize();
            //tangentOutVector.Normalize();

            inPos = new Vec3(kf.Second, kf.InValue, 0.0f);
            inTanPos = new Vec3(kf.Second + tangentInVector.X, kf.InValue + tangentInVector.Y, 0.0f);

            outPos = new Vec3(kf.Second, kf.OutValue, 0.0f);
            outTanPos = new Vec3(kf.Second + tangentOutVector.X, kf.OutValue + tangentOutVector.Y, 0.0f);
        }
        public float GetMaxSpeed()
        {
            _targetAnimation.GetMinMax(true,
              out (float Time, float Value)[] min,
              out (float Time, float Value)[] max);
            return TMath.Max(Math.Abs(min[0].Value), Math.Abs(max[0].Value));
        }
        const float Resolution = 1.0f;
        private float MinSec { get; set; }
        private float VisibleSecRange { get; set; }
        private int FrameCount { get; set; }
        public void UpdateSplinePrimitive()
        {
            if (_targetAnimation == null)
                return;

            if (AnimLength != _targetAnimation.LengthInSeconds ||
                KeyCount != _targetAnimation.Keyframes.Count ||
                _rcSpline.Mesh == null)
            {
                RegenerateSplinePrimitive();
                return;
            }

            if (DisplayFPS <= 0.0f)
                return;

            float displayFPS;
            GetFocusAreaMinMax(out Vec2 animMin, out Vec2 animMax);
            Vec2 bounds = Bounds;
            Vec2 boundsMinAnimRelative = Vec3.TransformPosition(Vec3.Zero, _baseTransformComponent.InverseWorldMatrix).Xy;
            Vec2 boundsMaxAnimRelative = Vec3.TransformPosition(bounds, _baseTransformComponent.InverseWorldMatrix).Xy;
            float maxSec = TMath.Min(boundsMaxAnimRelative.X, animMax.X);
            float minSec = TMath.Max(boundsMinAnimRelative.X, animMin.X);
            float maxVal = TMath.Min(boundsMaxAnimRelative.Y, animMax.Y);
            float minVal = TMath.Max(boundsMinAnimRelative.Y, animMin.Y);
            float visibleAnimSecRange = maxSec - minSec;
            float visibleAnimValRange = maxVal - minVal;
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

            if (frameCount != FrameCount)
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
                GetKeyframeInfo(kf, out Vec3 inPos, out Vec3 outPos, out Vec3 inTanPos, out Vec3 outTanPos);
                
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
            //while (_regenerating) { }
            
            _regenerating = true;
            _rcKeyframeInOutPositions.Mesh?.Dispose();
            _rcKeyframeInOutPositions.Mesh = null;
            _rcTangentPositions.Mesh?.Dispose();
            _rcTangentPositions.Mesh = null;
            _rcKfLines.Mesh?.Dispose();
            _rcKfLines.Mesh = null;

            if (_targetAnimation == null || (AnimLength = _targetAnimation.LengthInSeconds) <= 0.0f)
            {
                _regenerating = false;
                return;
            }

            GetFocusAreaMinMax(out Vec2 animMin, out Vec2 animMax);
            Vec2 bounds = Bounds;
            Vec2 boundsMinAnimRelative = Vec3.TransformPosition(Vec3.Zero, _baseTransformComponent.InverseWorldMatrix).Xy;
            Vec2 boundsMaxAnimRelative = Vec3.TransformPosition(bounds, _baseTransformComponent.InverseWorldMatrix).Xy;
            float maxSec = TMath.Min(boundsMaxAnimRelative.X, animMax.X);
            float minSec = TMath.Max(boundsMinAnimRelative.X, animMin.X);
            float maxVal = TMath.Min(boundsMaxAnimRelative.Y, animMax.Y);
            float minVal = TMath.Max(boundsMinAnimRelative.Y, animMin.Y);
            float visibleAnimSecRange = maxSec - minSec;
            float visibleAnimValRange = maxVal - minVal;
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
                GetKeyframeInfo(kf, out Vec3 inPos, out Vec3 outPos, out Vec3 inTanPos, out Vec3 outTanPos);

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

        private void GetSplineVertex(float sec, float maxVelocity, out Vec3 pos, out ColorF4 color)
        {
            float val = _targetAnimation.GetValue(sec);
            float vel = _targetAnimation.GetVelocityKeyframed(sec);

            //float time = 1.0f - 1.0f / (1.0f + VelocitySigmoidScale * (vel * vel));
            float time = Math.Abs(vel) / maxVelocity;

            color = Vec3.Lerp(Vec3.UnitZ, Vec3.UnitX, time);
            pos = new Vec3(sec, val, 0.0f);
        }
        protected override void UpdateTextScale()
        {
            base.UpdateTextScale();
            Vec2 scale = 1.0f / _baseTransformComponent.Scale;
            _xCoord.Scale = scale;
            _yCoord.Scale = scale;
        }
        protected override bool GetFocusAreaMinMax(out Vec2 min, out Vec2 max)
        {
            if (_targetAnimation == null)
            {
                min = Vec2.Zero;
                max = Vec2.Zero;
                return false;
            }

            _targetAnimation.GetMinMax(false,
                out (float Time, float Value)[] minResult,
                out (float Time, float Value)[] maxResult);

            float minY = minResult[0].Value;
            float maxY = maxResult[0].Value;
            if (minY > maxY)
            {
                min = Vec2.Zero;
                max = Vec2.Zero;
                return false;
            }

            float minX = 0.0f;
            float maxX = _targetAnimation.LengthInSeconds;

            min = new Vec2(minX, minY);
            max = new Vec2(maxX, maxY);
            return true;
        }
        public override void ZoomExtents()
        {
            base.ZoomExtents();
            UpdateSplinePrimitive();

            //Vec2 animPos = _baseTransformComponent.LocalTranslation / _baseTransformComponent.Scale;
            //Vec2 visibleAnimRange = Bounds / _baseTransformComponent.Scale;
        }
        protected override UICanvasComponent OnConstructRoot()
        {
            var root = base.OnConstructRoot();

            _xCoord = ConstructText(ColorF4.White, "0.0", "-0000.000", out _xString);
            _yCoord = ConstructText(ColorF4.White, "0.0", "-0000.000", out _yString);

            return root;
        }
        private void OnCurrentPositionChanged(PropAnimVector<float, FloatKeyframe> obj)
        {
            Vec3 pos = new Vec3(_targetAnimation.CurrentTime, _targetAnimation.CurrentPosition, 0.0f);
            AnimPositionWorld = Vec3.TransformPosition(pos, _baseTransformComponent.WorldMatrix).Xy;

            Vec2 origin = GetViewportBottomLeftWorldSpace();

            _xCoord.SizeablePosX.ModificationValue = pos.X;
            _yCoord.SizeablePosY.ModificationValue = pos.Y;
            _xCoord.SizeablePosY.ModificationValue = origin.Y;
            _yCoord.SizeablePosX.ModificationValue = origin.X;

            if (ValueDisplayMode != EVectorInterpValueType.Position)
            {
                if (ValueDisplayMode == EVectorInterpValueType.Velocity)
                    pos.Y = _targetAnimation.CurrentVelocity;
                else
                    pos.Y = _targetAnimation.CurrentAcceleration;
            }

            _xString.Text = pos.X.ToString("###0.0##");
            _yString.Text = pos.Y.ToString("###0.0##");
        }
        protected override void BaseWorldTransformChanged()
        {
            Matrix4 mtx = _baseTransformComponent.WorldMatrix;

            _rcKfLines.WorldMatrix =
            _rcSpline.WorldMatrix =
            _rcKeyframeInOutPositions.WorldMatrix =
            _rcTangentPositions.WorldMatrix = mtx;

            if (_targetAnimation != null)
            {
                RenderAnimPosition = true;

                Vec3 pos = new Vec3(_targetAnimation.CurrentTime, _targetAnimation.CurrentPosition, 0.0f);
                AnimPositionWorld = Vec3.TransformPosition(pos, _baseTransformComponent.WorldMatrix).Xy;
            }
            else
                RenderAnimPosition = false;

            Vec2 origin = GetViewportBottomLeftWorldSpace();
            _xCoord.SizeablePosY.ModificationValue = origin.Y;
            _yCoord.SizeablePosX.ModificationValue = origin.X;

            base.BaseWorldTransformChanged();
            UpdateSplinePrimitive();
        }
        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
            
            input.RegisterKeyEvent(EKey.Space, EButtonInputType.Pressed, TogglePlay, EInputPauseType.TickAlways);
        }
        
        private void TogglePlay()
        {
            if (_targetAnimation == null)
                return;

            if (_targetAnimation.State == EAnimationState.Playing)
                _targetAnimation.State = EAnimationState.Paused;
            else
                _targetAnimation.State = EAnimationState.Playing;
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
            if (!Bounds.Contains(v))
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
                    foreach (int index in ClosestPositionIndices)
                    {
                        int kfIndex = index >> 2;
                        var kf = _targetAnimation.Keyframes[kfIndex];

                        // 00 inPos
                        // 01 outPos
                        // 10 inTan
                        // 11 outTan

                        bool inVal = (index & 1) == 0;
                        bool pos = (index & 2) == 0;

                        bool draggingInValue = (pos && inVal) || kf.SyncInOutValues;
                        bool draggingOutValue = (pos && !inVal) || kf.SyncInOutValues;
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
                }
                else if (CtrlDown)
                {
                    //Create a keyframe under the cursor location

                    float length = TMath.Max(_targetAnimation.LengthInSeconds, sec);
                    var track = _targetAnimation.Keyframes;
                    bool nullTrack = track == null;
                    if (length > _targetAnimation.LengthInSeconds)
                        _targetAnimation.SetLength(length, false, nullTrack);

                    if (!nullTrack)
                    {
                        var prevKf = track.GetKeyBefore(sec, false, false);
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

                            //This will update the animation position
                            _targetAnimation.Progress(0.0f);
                        }
                    }
                }
                else
                {
                    //Begin drag selection
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
                            if (SnapToIncrement)
                                sec = sec.RoundToNearestMultiple(UnitIncrement);
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

                            if (kf.Value.DraggingInValue)
                            {
                                float inPos = pos.Y + kf.Value.InValueOffset;
                                if (SnapToIncrement)
                                    inPos = inPos.RoundToNearestMultiple(UnitIncrement);
                                kf.Value.Keyframe.InValue = inPos;
                            }
                            if (kf.Value.DraggingOutValue)
                            {
                                float outPos = pos.Y + kf.Value.OutValueOffset;
                                if (SnapToIncrement)
                                    outPos = outPos.RoundToNearestMultiple(UnitIncrement);
                                kf.Value.Keyframe.OutValue = outPos;
                            }
                        }
                        else
                        {
                            if (kf.Value.DraggingInTangent)
                            {
                                float inPos = pos.Y + kf.Value.InValueOffset;
                                if (SnapToIncrement)
                                    inPos = inPos.RoundToNearestMultiple(UnitIncrement);
                                kf.Value.Keyframe.InTangent = inPos;
                            }
                            if (kf.Value.DraggingOutTangent)
                            {
                                float outPos = pos.Y + kf.Value.OutValueOffset;
                                if (SnapToIncrement)
                                    outPos = outPos.RoundToNearestMultiple(UnitIncrement);
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
                    if (SnapToIncrement)
                        kfSec = kfSec.RoundToNearestMultiple(UnitIncrement);

                    if (kf.Value.DraggingInValue || kf.Value.DraggingOutValue)
                    {
                        kf.Value.Keyframe.Second = kfSec;

                        if (kf.Value.DraggingInValue)
                        {
                            float inPos = pos.Y + kf.Value.InValueOffset;
                            if (SnapToIncrement)
                                inPos = inPos.RoundToNearestMultiple(UnitIncrement);
                            kf.Value.Keyframe.InValue = inPos;
                        }
                        if (kf.Value.DraggingOutValue)
                        {
                            float outPos = pos.Y + kf.Value.OutValueOffset;
                            if (SnapToIncrement)
                                outPos = outPos.RoundToNearestMultiple(UnitIncrement);
                            kf.Value.Keyframe.OutValue = outPos;
                        }
                    }
                    else
                    {
                        float tangentScale = 50.0f / _baseTransformComponent.ScaleX;
                     
                        if (kf.Value.DraggingInTangent)
                        {
                            float inTanY = pos.Y + kf.Value.InTangentOffset;
                            
                            if (SnapToIncrement)
                                inTanY = inTanY.RoundToNearestMultiple(UnitIncrement);

                            float inTanYDiff = inTanY - kf.Value.InValueInitial;

                            kf.Value.Keyframe.InTangent = inTanYDiff / (kfSec - kf.Value.SecondInitial);
                        }
                        if (kf.Value.DraggingOutTangent)
                        {
                            float outTanY = pos.Y + kf.Value.OutTangentOffset;
                            
                            if (SnapToIncrement)
                                outTanY = outTanY.RoundToNearestMultiple(UnitIncrement);

                            float outTanYDiff = outTanY - kf.Value.OutValueInitial;

                            kf.Value.Keyframe.OutTangent = outTanYDiff / (kfSec - kf.Value.SecondInitial);
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
            if (KeyframeInOutPosInOutTan == null)
                return;

            Vec2 cursorPos = CursorPositionTransformRelative();
            float radius = SelectionRadius / _baseTransformComponent.ScaleX;
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
            if (_targetAnimation == null)
                return;
            
            passes.Add(_rcSpline);
            passes.Add(_rcKeyframeInOutPositions);
            passes.Add(_rcTangentPositions);
            passes.Add(_rcKfLines);
        }
        protected override void RenderMethod()
        {
            Vec2 wh = _backgroundComponent.Size;

            //TODO: if a keyframe is dragged past another, its index changes but these indices are not updated
            if (ClosestPositionIndices != null)
                foreach (int index in ClosestPositionIndices)
                    if (KeyframeInOutPosInOutTan?.IndexInArrayRange(index) ?? false)
                        Engine.Renderer.RenderPoint(Vec3.TransformPosition(KeyframeInOutPosInOutTan[index], _baseTransformComponent.WorldMatrix), Color.Yellow, false, 10.0f);

            base.RenderMethod();

            //Animation end line
            if (_targetAnimation != null && _targetAnimation.Keyframes.Count > 0)
            {
                Vec3 end = Vec3.TransformPosition(new Vec2(_targetAnimation.LengthInSeconds, 0.0f), _baseTransformComponent.WorldMatrix);
                Engine.Renderer.RenderLine(new Vec2(end.X, 0.0f), new Vec2(end.X, wh.Y), new ColorF4(0.7f, 0.3f, 0.3f), false, 10.0f);
            }

            if (RenderAnimPosition)
            {
                Engine.Renderer.RenderPoint(AnimPositionWorld, new ColorF4(1.0f), false, 10.0f);
                Engine.Renderer.RenderLine(new Vec2(AnimPositionWorld.X, 0.0f), new Vec2(AnimPositionWorld.X, wh.Y), new ColorF4(1.0f), false, 10.0f);
                Engine.Renderer.RenderLine(new Vec2(0.0f, AnimPositionWorld.Y), new Vec2(wh.X, AnimPositionWorld.Y), new ColorF4(1.0f), false, 10.0f);

                if (_targetAnimation != null)
                {
                    //float invScale = 1.0f / _baseTransformComponent.ScaleX;
                    Engine.Renderer.RenderLine(new Vec2(AnimPositionWorld.X - 15.0f, AnimPositionWorld.Y), new Vec2(AnimPositionWorld.X - 15.0f, AnimPositionWorld.Y + _targetAnimation.CurrentVelocity), new ColorF4(0.5f, 0.5f, 0.0f), false, 10.0f);
                    Engine.Renderer.RenderLine(new Vec2(AnimPositionWorld.X + 15.0f, AnimPositionWorld.Y), new Vec2(AnimPositionWorld.X + 15.0f, AnimPositionWorld.Y + _targetAnimation.CurrentAcceleration), new ColorF4(0.5f, 0.0f, 0.5f), false, 10.0f);
                }
            }
        }
    }
}
