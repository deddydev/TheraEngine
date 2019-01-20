using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Animation;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms
{
    public delegate void DelSelectedKeyframeChanged(Keyframe kf);
    public delegate void DelSelectedTangentChanged(Keyframe kf, bool inTan);
    /// <summary>
    /// UI editor to create shaders in a user-friendly visual graph format.
    /// </summary>
    public class UIPropAnimFloatEditor : EditorUserInterface, I2DRenderable
    {
        public UIPropAnimFloatEditor() : base()
            => _rcMethod = new RenderCommandMethod2D(ERenderPass.OnTopForward, RenderMethod);
        public UIPropAnimFloatEditor(Vec2 bounds) : base(bounds)
            =>_rcMethod = new RenderCommandMethod2D(ERenderPass.OnTopForward, RenderMethod);
        protected override UICanvasComponent OnConstructRoot()
        {
            var root = base.OnConstructRoot();
            _baseTransformComponent.WorldTransformChanged += BaseWorldTransformChanged;
            return root;
        }

        private Vec3[] KeyframeInOutPositions { get; set; }
        public float VelocitySigmoidScale { get; set; } = 0.002f;
        private float DisplayFPS { get; set; } = 0.0f;
        private float AnimLength { get; set; } = 0.0f;
        private int KeyCount { get; set; } = 0;
        public Vec2 AnimPosition { get; private set; }

        private readonly RenderCommandMethod2D _rcMethod;
        private readonly RenderCommandMesh2D _rcKfLines = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        private readonly RenderCommandMesh2D _rcSpline = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        private readonly RenderCommandMesh2D _rcKeyframeInOutPositions = new RenderCommandMesh2D(ERenderPass.OnTopForward);
        private readonly RenderCommandMesh2D _rcTangentPositions = new RenderCommandMesh2D(ERenderPass.OnTopForward);

        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(0, 0);
        public BoundingRectangleF AxisAlignedRegion { get; } = new BoundingRectangleF();
        public IQuadtreeNode QuadtreeNode { get; set; }

        [TSerialize]
        public float SelectionRadius { get; set; } = 10.0f;
        [TSerialize]
        public float ZoomIncrement { get; set; } = 0.05f;
        [TSerialize]
        public bool SnapToIncrement { get; set; } = false;
        [TSerialize]
        public bool RenderExtrema { get; set; } = true;
        [TSerialize]
        public bool RenderKeyframeTangents { get; set; } = true;
        [TSerialize]
        public bool RenderAnimPosition { get; set; } = true;

        public bool IsDraggingKeyframes => _draggedKeyframes.Count > 0;
        public int[] ClosestKeyframeIndices { get; private set; }
        
        private bool _ctrlDown = false;
        private bool _shiftDown = false;

        private PropAnimFloat _targetAnimation;
        private Vec2 _minScale = new Vec2(0.01f), _maxScale = new Vec2(1.0f);
        private Vec2 _lastWorldPos = Vec2.Zero;
        private bool _rightClickDown = false;
        private FloatKeyframe _selectedKf;
        private FloatKeyframe _highlightedKf;
        private Dictionary<int, DraggedKeyframeInfo> _draggedKeyframes = new Dictionary<int, DraggedKeyframeInfo>();
        private Vec2 _selectionStartPosAnimRelative;

        private bool _regenerating = false;

        public event DelSelectedKeyframeChanged SelectedKeyframeChanged;

        public PropAnimFloat TargetAnimation
        {
            get => _targetAnimation;
            set
            {
                if (_targetAnimation != null)
                {
                    _targetAnimation.Keyframes.Changed -= OnChanged;
                    _targetAnimation.SpeedChanged -= OnSpeedChanged;
                    _targetAnimation.ConstrainKeyframedFPSChanged -= OnConstrainKeyframedFPSChanged;
                    _targetAnimation.BakedFPSChanged -= OnBakedFPSChanged;
                    _targetAnimation.LengthChanged -= _position_LengthChanged;
                    _targetAnimation.CurrentPositionChanged -= OnCurrentPositionChanged;
                }
                _targetAnimation = value;
                if (_targetAnimation != null)
                {
                    _targetAnimation.Keyframes.Changed += OnChanged;
                    _targetAnimation.SpeedChanged += OnSpeedChanged;
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
        public async void UpdateSplinePrimitive()
        {
            if (AnimLength != _targetAnimation.LengthInSeconds ||
                KeyCount != _targetAnimation.Keyframes.Count ||
                _rcSpline.Mesh == null)
            {
                await RegenerateSplinePrimitiveAsync();
                return;
            }

            if (DisplayFPS <= 0.0f)
                return;

            var posBuf = _rcSpline.Mesh.Data[EBufferType.Position];
            var colBuf = _rcSpline.Mesh.Data[EBufferType.Color];
            var kfPosBuf = _rcKeyframeInOutPositions.Mesh.Data[EBufferType.Position];
            var tanPosBuf = _rcTangentPositions.Mesh.Data[EBufferType.Position];
            var keyLinesBuf = _rcKfLines.Mesh.Data[EBufferType.Position];

            int i;
            float sec = 0.0f;
            float timeIncrement = 1.0f / DisplayFPS;
            for (i = 0; i < posBuf.ElementCount; ++i, sec = i * timeIncrement)
            {
                GetSplineVertex(sec, out Vec3 pos, out ColorF4 color);

                posBuf.Set(i, pos);
                colBuf.Set(i, color);
            }

            i = 0;
            int i2;
            foreach (FloatKeyframe kf in _targetAnimation)
            {
                i2 = i << 1;
                GetKeyframeInfo(kf, out Vec3 inPos, out Vec3 outPos, out Vec3 inTanPos, out Vec3 outTanPos);
                
                KeyframeInOutPositions[i] = inPos;
                kfPosBuf.Set(i, inPos);
                tanPosBuf.Set(i, inTanPos);
                keyLinesBuf.Set(i2, inPos);
                keyLinesBuf.Set(i2 + 1, inTanPos);

                ++i;
                i2 = i << 1;

                KeyframeInOutPositions[i] = outPos;
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
            while (_regenerating) { }
            
            const float Resolution = 0.1f;

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

            //TODO: when the FPS is unconstrained, use adaptive vertex points based on velocity/acceleration
            DisplayFPS = (_targetAnimation.ConstrainKeyframedFPS ?
                _targetAnimation.BakedFramesPerSecond :
                (Engine.TargetFramesPerSecond == 0 ? 60.0f : Engine.TargetFramesPerSecond)) * Resolution;

            if (DisplayFPS <= 0.0f)
            {
                _regenerating = false;
                return;
            }

            int frameCount = (int)Math.Ceiling(_targetAnimation.LengthInSeconds * DisplayFPS) + 1;
            float invFps = 1.0f / DisplayFPS;
            int posCount = (KeyCount = _targetAnimation.Keyframes.Count) << 1;

            Vertex[] splinePoints = new Vertex[frameCount];
            VertexLine[] keyframeLines = new VertexLine[posCount];
            Vec3[] tangentPositions = new Vec3[posCount];
            KeyframeInOutPositions = new Vec3[posCount];

            int i;
            float sec = 0.0f;
            for (i = 0; i < splinePoints.Length; ++i, sec = i * invFps)
            {
                GetSplineVertex(sec, out Vec3 pos, out ColorF4 color);
                splinePoints[i] = new Vertex(pos, color);
            }

            i = 0;
            foreach (FloatKeyframe kf in _targetAnimation)
            {
                GetKeyframeInfo(kf, out Vec3 inPos, out Vec3 outPos, out Vec3 inTanPos, out Vec3 outTanPos);

                KeyframeInOutPositions[i] = inPos;
                tangentPositions[i] = inTanPos;
                keyframeLines[i] = new VertexLine(inPos, inTanPos);
                ++i;

                KeyframeInOutPositions[i] = outPos;
                tangentPositions[i] = outTanPos;
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
            
            PrimitiveData kfInOutPos = PrimitiveData.FromPoints(KeyframeInOutPositions);
            foreach (var buf in kfInOutPos)
            {
                buf.MapData = true;
                buf.Usage = EBufferUsage.DynamicDraw;
            }
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Green);
            mat.RenderParams = renderParams;
            _rcKeyframeInOutPositions.Mesh = new PrimitiveManager(kfInOutPos, mat);

            PrimitiveData tanPos = PrimitiveData.FromPoints(tangentPositions);
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

        private void GetSplineVertex(float sec, out Vec3 pos, out ColorF4 color)
        {
            float val = _targetAnimation.GetValue(sec);
            float vel = _targetAnimation.GetVelocityKeyframed(sec) * _targetAnimation.Speed;

            float sigmoid = 1.0f / (1.0f + VelocitySigmoidScale * (vel * vel));
            color = Vec3.Lerp(Vec3.UnitX, Vec3.UnitZ, sigmoid);
            pos = new Vec3(sec, val, 0.0f);
        }

        public void ZoomExtents()
        {
            if (_targetAnimation == null)
                return;

            _targetAnimation.GetMinMax(
                out (float Time, float Value)[] min,
                out (float Time, float Value)[] max);

            float minVal = min[0].Value;
            float maxVal = max[0].Value;

            float xBound = _targetAnimation.LengthInSeconds;
            float yBound = maxVal - minVal;

            if (xBound == 0.0f)
                xBound = LineIncrement * InitialVisibleBoxes;
            if (yBound == 0.0f)
                yBound = LineIncrement * InitialVisibleBoxes;

            float xScale = Bounds.X / xBound;
            float yScale = Bounds.Y / yBound;

            Vec2 animPos = _baseTransformComponent.LocalTranslation / _baseTransformComponent.Scale;
            Vec2 visibleAnimRange = Bounds / _baseTransformComponent.Scale;

            if (xScale < yScale)
            {
                _baseTransformComponent.Scale = xScale;
                //_baseTransformComponent.LocalTranslation = new Vec2(0.0f, minVal * yScale);
            }
            else
            {
                float animCoordX = _baseTransformComponent.LocalTranslation.X / _baseTransformComponent.Scale.X;

                _baseTransformComponent.Scale = yScale;

                float halfXTrans = xBound * 0.5f;
                float halfXWorld = _baseTransformComponent.Scale.X * halfXTrans;
                float boundHalfXWorld = Bounds.X / 0.5f;
                _baseTransformComponent.LocalTranslation = new Vec2(-halfXWorld, 0.0f);
            }
        }
        private void OnCurrentPositionChanged(PropAnimVector<float, FloatKeyframe> obj)
        {
            AnimPosition = Vec3.TransformPosition(new Vec3(_targetAnimation.CurrentTime, _targetAnimation.CurrentPosition, 0.0f), _baseTransformComponent.WorldMatrix).Xy;
        }
        private void BaseWorldTransformChanged()
        {
            Matrix4 mtx = _baseTransformComponent.WorldMatrix;

            _rcKfLines.WorldMatrix =
            _rcSpline.WorldMatrix =
            _rcKeyframeInOutPositions.WorldMatrix =
            _rcTangentPositions.WorldMatrix = mtx;

            if (_targetAnimation != null)
            {
                RenderAnimPosition = true;
                AnimPosition = Vec3.TransformPosition(new Vec3(_targetAnimation.CurrentTime, _targetAnimation.CurrentPosition, 0.0f), mtx).Xy;
            }
            else
                RenderAnimPosition = false;

            UpdateBackgroundMaterial();
        }
        public override void Resize(Vec2 bounds)
        {
            base.Resize(bounds);
            UpdateBackgroundMaterial();
        }
        public float MaxIncrementExclusive = 10.0f;
        public float[] IncrementsRange = new float[] { 1.0f, 2.0f, 5.0f };
        public void UpdateLineIncrement()
        {
            Vec2 visibleAnimRange = Bounds / _baseTransformComponent.Scale;
            float range = TMath.Min(visibleAnimRange.X, visibleAnimRange.Y);
            if (range == 0.0f)
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

            LineIncrement = inc / MaxIncrementExclusive;

            //float bound = _targetAnimation == null || _targetAnimation.LengthInSeconds <= 0.0f ? 1.0f : _targetAnimation.LengthInSeconds;
            //float LineIncrement = 2.0f; //Initial display max dim should be to 10
            //float maxDisplayBoundAnimRelative = TMath.Max(Bounds.X, Bounds.Y) * 0.5f / _baseTransformComponent.ScaleX;

            //float[] incs = new float[] { 0.1, 0.2, 0.5, 1, 2, 5, 10, 20, 50, 100 };
            //float scale = _baseTransformComponent.ScaleX;
            //float scaledInc = scale * LineIncrement;
            //float fraction = inc - (int)Math.Floor(inc);
            //Engine.PrintLine($"UI scale: { _baseTransformComponent.ScaleX.ToString()} Nearest2: {nearest2} Nearest5: {nearest5} Nearest10: {nearest10}");
        }

        public float LineIncrement { get; set; } = 1.0f;
        public const int InitialVisibleBoxes = 10;
        private void UpdateBackgroundMaterial()
        {
            UpdateLineIncrement();

            TMaterial mat = _backgroundComponent.InterfaceMaterial;
            mat.Parameter<ShaderFloat>(2).Value = _baseTransformComponent.ScaleX;
            mat.Parameter<ShaderVec2>(4).Value = _baseTransformComponent.LocalTranslation;
            mat.Parameter<ShaderFloat>(5).Value = LineIncrement;
        }
        protected override void OnSpawnedPostComponentSpawn()
        {
            base.OnSpawnedPostComponentSpawn();
            ScreenSpaceUIScene.Add(this);

            if (_targetAnimation != null)
                ZoomExtents();
            else
                _baseTransformComponent.Scale = TMath.Min(Bounds.X, Bounds.Y) / (LineIncrement * InitialVisibleBoxes);

            UpdateBackgroundMaterial();
        }
        protected override void OnDespawned()
        {
            base.OnDespawned();
            ScreenSpaceUIScene.Remove(this);
        }
        protected override TMaterial GetBackgroundMaterial()
        {
            GLSLScript frag = Engine.Files.LoadEngineShader("MaterialEditorGraphBG.fs", EGLSLType.Fragment);
            return new TMaterial("MatEditorGraphBG", new ShaderVar[]
            {
                new ShaderVec3(new Vec3(0.15f, 0.15f, 0.15f), "LineColor"),
                new ShaderVec3(new Vec3(0.08f, 0.08f, 0.08f), "BGColor"),
                new ShaderFloat(_baseTransformComponent.ScaleX, "Scale"),
                new ShaderFloat(3.0f, "LineWidth"),
                new ShaderVec2(_baseTransformComponent.LocalTranslation, "Translation"),
                new ShaderFloat(LineIncrement, "XYIncrement"),
            },
            frag);
        }
        public override void RegisterInput(InputInterface input)
        {
            //input.RegisterKeyPressed(EKey.AltLeft,      AltPressed,     EInputPauseType.TickAlways);
            //input.RegisterKeyPressed(EKey.AltRight,     AltPressed,     EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ControlLeft,  CtrlPressed,    EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ControlRight, CtrlPressed,    EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ShiftLeft,    ShiftPressed,   EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ShiftRight,   ShiftPressed,   EInputPauseType.TickAlways);
            
            input.RegisterKeyEvent(EKey.Number1,                EButtonInputType.Pressed,   ToggleSnap,     EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick,   EButtonInputType.Pressed,   LeftClickDown,  EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick,   EButtonInputType.Released,  LeftClickUp,    EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick,  EButtonInputType.Pressed,   RightClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick,  EButtonInputType.Released,  RightClickUp,   EInputPauseType.TickAlways);

            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, EMouseMoveType.Absolute, EInputPauseType.TickAlways);

            input.RegisterKeyEvent(EKey.Space, EButtonInputType.Pressed, TogglePlay, EInputPauseType.TickAlways);
        }

        private void ToggleSnap() => SnapToIncrement = !SnapToIncrement;
        private void TogglePlay()
        {
            if (_targetAnimation == null)
                return;

            if (_targetAnimation.State == EAnimationState.Playing)
                _targetAnimation.State = EAnimationState.Paused;
            else
                _targetAnimation.State = EAnimationState.Playing;
        }

        //private void AltPressed(bool pressed) => _altDown = pressed;
        private void CtrlPressed(bool pressed) => _ctrlDown = pressed;
        private void ShiftPressed(bool pressed)
        {
            _shiftDown = pressed;
            if (IsDraggingKeyframes)
                HandleDraggingKeyframes();
        }
        internal void LeftClickDown()
        {
            Vec2 v = CursorPosition();
            if (!Bounds.Contains(v))
                return;

            if (_targetAnimation != null)
            {
                Vec2 animPos = CursorPositionTransformRelative();
                float sec = animPos.X;
                float val = animPos.Y;

                if (ClosestKeyframeIndices != null && ClosestKeyframeIndices.Length > 0)
                {
                    //Keyframes are within the selection radius

                    _draggedKeyframes.Clear();
                    _selectionStartPosAnimRelative = animPos;
                    foreach (int index in ClosestKeyframeIndices)
                    {
                        int kfIndex = index >> 1;
                        var kf = _targetAnimation.Keyframes[kfIndex];
                        bool draggingInValue = (index & 1) == 0 || kf.SyncInOutValues;
                        bool draggingOutValue = (index & 1) == 1 || kf.SyncInOutValues;

                        if (_draggedKeyframes.ContainsKey(kfIndex))
                        {
                            DraggedKeyframeInfo info = _draggedKeyframes[kfIndex];
                            info.DraggingInValue = info.DraggingInValue || draggingInValue;
                            info.DraggingOutValue = info.DraggingOutValue || draggingOutValue;
                        }
                        else
                        {
                            _draggedKeyframes.Add(kfIndex, new DraggedKeyframeInfo()
                            {
                                Keyframe = kf,

                                DraggingInValue = draggingInValue,
                                DraggingOutValue = draggingOutValue,

                                SecondOffset = kf.Second - animPos.X,
                                InValueOffset = kf.InValue - animPos.Y,
                                OutValueOffset = kf.OutValue - animPos.Y,

                                SecondInitial = kf.Second,
                                InValueInitial = kf.InValue,
                                OutValueInitial = kf.OutValue,
                            });
                        }
                    }
                }
                else
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
            }

            //if (_selectedKf != null && _selectedKf != _highlightedKf)
            //{
            //    //Reset current _selectedKf
            //}
            
            //_selectedKf = _highlightedKf;
            //_lastWorldPos = CursorPositionWorld();

            //if (_selectedKf != null)
            //{
            //    //Update current _selectedKf
            //}
            //else
            //{
            //    _draggedFunc = _selectedKf;
            //}

            //SelectedFunctionChanged?.Invoke(_selectedKf);
        }
        internal void LeftClickUp()
        {
            if (_draggedKeyframes.Count > 0)
                _draggedKeyframes.Clear();
        }
        private void RightClickDown()
        {
            _rightClickDown = true;
            //_lastWorldPos = Camera.LocalPoint.Xy;
            _lastWorldPos = CursorPositionWorld();
        }
        private void RightClickUp()
        {
            _rightClickDown = false;
        }
        protected override void MouseMove(float x, float y)
        {
            Vec2 v = CursorPosition();
            if (!Bounds.Contains(v))
                return;

            if (_draggedKeyframes.Count > 0)
                HandleDraggingKeyframes();
            else if (_rightClickDown)
                HandleDragView();
            else
                HighlightGraph();
            _cursorPos = CursorPosition();
        }
        private Vec2 GetWorldCursorDiff(Vec2 cursorPosScreen)
        {
            Vec2 screenPoint = _cursorPos;//Viewport.WorldToScreen(_lastWorldPos).Xy;
            screenPoint += cursorPosScreen - _cursorPos;
            Vec2 newFocusPoint = Viewport.ScreenToWorld(screenPoint).Xy;
            Vec2 diff = newFocusPoint - _lastWorldPos;
            _lastWorldPos = newFocusPoint;
            return diff;
        }
        private void HandleDragView()
        {
            Vec2 diff = GetWorldCursorDiff(CursorPosition());
            _baseTransformComponent.LocalTranslation += diff;
        }
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
        private void HandleDraggingKeyframes()
        {
            Vec3 pos = CursorPositionTransformRelative();

            if (_shiftDown)
            {
                //Ortho mode
                float xDist = Math.Abs(pos.X - _selectionStartPosAnimRelative.X);
                float yDist = Math.Abs(pos.Y - _selectionStartPosAnimRelative.Y);
                if (xDist > yDist)
                {
                    //Drag left and right only
                    foreach (var kf in _draggedKeyframes)
                    {
                        float sec = pos.X + kf.Value.SecondOffset;
                        if (SnapToIncrement)
                            sec = sec.RoundToNearestMultiple(LineIncrement);
                        kf.Value.Keyframe.Second = sec;

                        if (kf.Value.DraggingInValue)
                            kf.Value.Keyframe.InValue = kf.Value.InValueInitial;
                        if (kf.Value.DraggingOutValue)
                            kf.Value.Keyframe.OutValue = kf.Value.OutValueInitial;
                    }
                }
                else
                {
                    //Drag up and down only
                    foreach (var kf in _draggedKeyframes)
                    {
                        kf.Value.Keyframe.Second = kf.Value.SecondInitial;

                        if (kf.Value.DraggingInValue)
                        {
                            float inPos = pos.Y + kf.Value.InValueOffset;
                            if (SnapToIncrement)
                                inPos = inPos.RoundToNearestMultiple(LineIncrement);
                            kf.Value.Keyframe.InValue = inPos;
                        }

                        if (kf.Value.DraggingOutValue)
                        {
                            float outPos = pos.Y + kf.Value.OutValueOffset;
                            if (SnapToIncrement)
                                outPos = outPos.RoundToNearestMultiple(LineIncrement);
                            kf.Value.Keyframe.OutValue = outPos;
                        }
                    }
                }
            }
            else
            {
                foreach (var kf in _draggedKeyframes)
                {
                    float sec = pos.X + kf.Value.SecondOffset;
                    if (SnapToIncrement)
                        sec = sec.RoundToNearestMultiple(LineIncrement);
                    kf.Value.Keyframe.Second = sec;

                    if (kf.Value.DraggingInValue)
                    {
                        float inPos = pos.Y + kf.Value.InValueOffset;
                        if (SnapToIncrement)
                            inPos = inPos.RoundToNearestMultiple(LineIncrement);
                        kf.Value.Keyframe.InValue = inPos;
                    }

                    if (kf.Value.DraggingOutValue)
                    {
                        float outPos = pos.Y + kf.Value.OutValueOffset;
                        if (SnapToIncrement)
                            outPos = outPos.RoundToNearestMultiple(LineIncrement);
                        kf.Value.Keyframe.OutValue = outPos;
                    }
                }
            }

            //This will update the animation position
            _targetAnimation.Progress(0.0f);
            
            UpdateSplinePrimitive();

            //if (_draggingInValue)
            //{
            //    if (_draggingTangent)
            //    {
            //        //Vec2 pos = new Vec2(draggedKf.Second, draggedKf.InValue) + draggedKf.InTangent;
            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //{
            //    if (_draggingTangent)
            //    {

            //    }
            //    else
            //    {

            //    }
            //}
            //draggedKf.InTangent += Vec3.TransformVector(diff, draggedKf.InverseWorldMatrix).Xy;
        }
        private void HighlightGraph()
        {
            ClosestKeyframeIndices = null;
            if (KeyframeInOutPositions == null)
                return;

            Vec2 cursorPos = CursorPositionTransformRelative();
            float radius = SelectionRadius / _baseTransformComponent.ScaleX;
            ClosestKeyframeIndices = KeyframeInOutPositions.FindAllMatchIndices(x => x.DistanceToFast(cursorPos) < radius);
        }
        protected override void OnScrolledInput(bool down)
        {
            if (_ctrlDown)
                SelectionRadius *= down ? 1.1f : 0.91f;
            else
            {
                Vec3 worldPoint = CursorPositionWorld();
                float increment = _baseTransformComponent.ScaleX * ZoomIncrement;
                _baseTransformComponent.Zoom(down ? increment : -increment, worldPoint.Xy, new Vec2(0.1f, 0.1f), null);
                
                UpdateBackgroundMaterial();
                UpdateSplinePrimitive();
            }
        }
        public void AddRenderables(RenderPasses passes)
        {
            if (_targetAnimation == null)
                return;
            
            passes.Add(_rcSpline);
            passes.Add(_rcKeyframeInOutPositions);
            passes.Add(_rcTangentPositions);
            passes.Add(_rcKfLines);
            passes.Add(_rcMethod);
        }
        private void RenderMethod()
        {
            Vec2 pos = CursorPositionWorld();
            Vec2 wh = _backgroundComponent.Size;

            //TODO: if a keyframe is dragged past another, its index changes but these indices are not updated
            if (ClosestKeyframeIndices != null)
                foreach (int index in ClosestKeyframeIndices)
                    if (KeyframeInOutPositions?.IndexInArrayRange(index) ?? false)
                        Engine.Renderer.RenderPoint(Vec3.TransformPosition(KeyframeInOutPositions[index], _baseTransformComponent.WorldMatrix), Color.Yellow, false, 10.0f);
            
            //Cursor
            Engine.Renderer.RenderCircle(pos + AbstractRenderer.UIPositionBias, AbstractRenderer.UIRotation, SelectionRadius, false, Editor.TurquoiseColor, 10.0f);
            Engine.Renderer.RenderLine(new Vec2(0.0f, pos.Y), new Vec2(wh.X, pos.Y), Editor.TurquoiseColor, false, 10.0f);
            Engine.Renderer.RenderLine(new Vec2(pos.X, 0.0f), new Vec2(pos.X, wh.Y), Editor.TurquoiseColor, false, 10.0f);

            //Grid origin lines
            Vec3 start = Vec3.TransformPosition(new Vec2(0.0f, 0.0f), _baseTransformComponent.WorldMatrix);
            Engine.Renderer.RenderLine(new Vec2(start.X, 0.0f), new Vec2(start.X, wh.Y), new ColorF4(0.55f), false, 10.0f);
            Engine.Renderer.RenderLine(new Vec2(0.0f, start.Y), new Vec2(wh.X, start.Y), new ColorF4(0.55f), false, 10.0f);

            //Animation end line
            if (_targetAnimation != null && _targetAnimation.Keyframes.Count > 0)
            {
                Vec3 end = Vec3.TransformPosition(new Vec2(_targetAnimation.LengthInSeconds, 0.0f), _baseTransformComponent.WorldMatrix);
                Engine.Renderer.RenderLine(new Vec2(end.X, 0.0f), new Vec2(end.X, wh.Y), new ColorF4(0.7f, 0.3f, 0.3f), false, 10.0f);
            }

            if (RenderAnimPosition)
            {
                Engine.Renderer.RenderPoint(AnimPosition, new ColorF4(1.0f), false, 10.0f);
                Engine.Renderer.RenderLine(new Vec2(AnimPosition.X, 0.0f), new Vec2(AnimPosition.X, wh.Y), new ColorF4(1.0f), false, 10.0f);
                Engine.Renderer.RenderLine(new Vec2(0.0f, AnimPosition.Y), new Vec2(wh.X, AnimPosition.Y), new ColorF4(1.0f), false, 10.0f);
            }
        }
    }
}
