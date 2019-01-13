using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        {
            _rcSelectionArea = new RenderCommandMethod2D(RenderSelectionAreaMethod);
        }
        public UIPropAnimFloatEditor(Vec2 bounds) : base(bounds)
        {
            _rcSelectionArea = new RenderCommandMethod2D(RenderSelectionAreaMethod);
        }

        public event DelSelectedKeyframeChanged SelectedKeyframeChanged;

        public PropAnimFloat TargetAnimation
        {
            get => _targetAnimation;
            set
            {
                if (_targetAnimation != null)
                {
                    _targetAnimation.Keyframes.Changed -= Keyframes_Changed;
                    _targetAnimation.ConstrainKeyframedFPSChanged -= _position_ConstrainKeyframedFPSChanged;
                    _targetAnimation.BakedFPSChanged -= _position_BakedFPSChanged1;
                    _targetAnimation.LengthChanged -= _position_LengthChanged;
                    //_position.AnimationStarted -= _spline_AnimationStarted;
                    //_position.AnimationPaused -= _spline_AnimationEnded;
                    //_position.AnimationEnded -= _spline_AnimationEnded;
                    _targetAnimation.CurrentPositionChanged -= _position_CurrentPositionChanged;
                    //_spline_AnimationEnded();
                }
                _targetAnimation = value;
                if (_targetAnimation != null)
                {
                    _targetAnimation.Keyframes.Changed += Keyframes_Changed;
                    _targetAnimation.ConstrainKeyframedFPSChanged += _position_ConstrainKeyframedFPSChanged;
                    _targetAnimation.BakedFPSChanged += _position_BakedFPSChanged1;
                    _targetAnimation.LengthChanged += _position_LengthChanged;
                    //_position.AnimationStarted += _spline_AnimationStarted;
                    //_position.AnimationPaused += _spline_AnimationEnded;
                    //_position.AnimationEnded += _spline_AnimationEnded;
                    _targetAnimation.CurrentPositionChanged += _position_CurrentPositionChanged;
                    _targetAnimation.TickSelf = true;
                    //if (_position.State == EAnimationState.Playing)
                    //    _spline_AnimationStarted();

                    _targetAnimation.GetMinMax(
                        out (float Time, float Value)[] min,
                        out (float Time, float Value)[] max);

                    //float minVal = min[0].Value;
                    //float maxVal = max[0].Value;
                    //float midPoint = (maxVal + minVal) * 0.5f;
                    ////_rootTransform.LocalTranslation = new Vec2(_targetAnimation.LengthInSeconds * 0.5f, midPoint);
                    //_rootTransform.Scale = new Vec2(_targetAnimation.LengthInSeconds, maxVal - minVal);
                }
                RegenerateSplinePrimitive();
            }
        }

        private async void _position_BakedFPSChanged1(BasePropAnimBakeable obj)
        {
            await RegenerateSplinePrimitiveAsync();
        }
        private async void _position_LengthChanged(BaseAnimation obj)
        {
            await RegenerateSplinePrimitiveAsync();
        }
        private async void _position_ConstrainKeyframedFPSChanged(PropAnimVector<float, FloatKeyframe> obj)
        {
            await RegenerateSplinePrimitiveAsync();
        }
        private async void Keyframes_Changed(BaseKeyframeTrack obj)
        {
            if (AnimLength != obj.LengthInSeconds || 
                KeyCount != obj.Count ||
                _splinePrimitive == null)
                await RegenerateSplinePrimitiveAsync();
            else
                UpdateSplinePrimitive();
        }

        public void UpdateSplinePrimitive()
        {
            if (DisplayFPS <= 0.0f)
                return;

            var posBuf = _splinePrimitive.Data[EBufferType.Position];
            var colBuf = _splinePrimitive.Data[EBufferType.Color];

            int i;
            float sec = 0.0f;
            float invFps = 1.0f / DisplayFPS;
            for (i = 0; i < posBuf.ElementCount; ++i, sec = i * invFps)
            {
                float val = _targetAnimation.GetValueKeyframed(sec);
                float vel = _targetAnimation.GetVelocityKeyframed(sec);

                float sigmoid = 1.0f / (1.0f + 0.00001f * (vel * vel));
                Vec3 colVal = Vec3.Lerp(Vec3.UnitX, Vec3.UnitZ, sigmoid);
                Vec3 posVal = new Vec3(sec, val, 0.0f);

                posBuf.Set(i * posBuf.Stride, posVal);
                colBuf.Set(i * colBuf.Stride, colVal);
            }

            posBuf.PushData();
            colBuf.PushData();
            
            var tanPosBuf = _tangentPositionPrimitive.Data[EBufferType.Position];
            var keyLinesBuf = _keyframeLinesPrimitive.Data[EBufferType.Position];

            i = 0;
            Vec3 p0, p1;
            float tangentScale = 10.0f;
            foreach (FloatKeyframe kf in _targetAnimation)
            {
                //In value and backward tangent
                KeyframePositions[i] = p0 = new Vec3(kf.Second, kf.InValue, 0.0f);
                p1 = new Vec3(kf.Second - tangentScale, kf.InValue + kf.InTangent * tangentScale, 0.0f);

                tanPosBuf.Set(i * tanPosBuf.Stride, p0);
                keyLinesBuf.Set(((i << 1) + 0) * keyLinesBuf.Stride, p0);
                keyLinesBuf.Set(((i << 1) + 1) * keyLinesBuf.Stride, p1);

                ++i;

                //Out value and forward tangent
                KeyframePositions[i] = p0 = new Vec3(kf.Second, kf.OutValue, 0.0f);
                p1 = new Vec3(kf.Second + tangentScale, kf.OutValue + kf.OutTangent * tangentScale, 0.0f);

                tanPosBuf.Set(i * tanPosBuf.Stride, p0);
                keyLinesBuf.Set(((i << 1) + 0) * keyLinesBuf.Stride, p0);
                keyLinesBuf.Set(((i << 1) + 1) * keyLinesBuf.Stride, p1);

                ++i;
            }

            tanPosBuf.PushData();
            keyLinesBuf.PushData();
        }

        private Vec3[] KeyframePositions { get; set; }
        private float DisplayFPS { get; set; } = 0.0f;
        private float AnimLength { get; set; } = 0.0f;
        private int KeyCount { get; set; } = 0;

        private bool _regenerating = false;
        public async Task RegenerateSplinePrimitiveAsync()
            => await Task.Run(RegenerateSplinePrimitive);
        public void RegenerateSplinePrimitive()
        {
            while (_regenerating) { }
            
            const float Resolution = 0.1f;

            _regenerating = true;
            _splinePrimitive?.Dispose();
            _splinePrimitive = null;
            _pointPrimitive?.Dispose();
            _pointPrimitive = null;
            _tangentPositionPrimitive?.Dispose();
            _tangentPositionPrimitive = null;
            _timePointPrimitive?.Dispose();
            _timePointPrimitive = null;

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
            KeyframePositions = new Vec3[posCount];

            int i;
            float sec = 0.0f;
            for (i = 0; i < splinePoints.Length; ++i, sec = i * invFps)
            {
                float val = _targetAnimation.GetValueKeyframed(sec);
                float vel = _targetAnimation.GetVelocityKeyframed(sec);

                float sigmoid = 1.0f / (1.0f + 0.00001f * (vel * vel));
                Vec3 colVal = Vec3.Lerp(Vec3.UnitX, Vec3.UnitZ, sigmoid);
                Vec3 posVal = new Vec3(sec, val, 0.0f);
                
                splinePoints[i] = new Vertex(posVal, (ColorF4)colVal);
            }

            i = 0;
            Vec3 p0, p1;
            float tangentScale = 10.0f;
            foreach (FloatKeyframe kf in _targetAnimation)
            {
                KeyframePositions[i] = p0 = new Vec3(kf.Second, kf.InValue, 0.0f);
                tangentPositions[i] = p1 = new Vec3(kf.Second - tangentScale, kf.InValue + kf.InTangent * tangentScale, 0.0f);
                keyframeLines[i] = new VertexLine(p0, p1);
                ++i;

                KeyframePositions[i] = p0 = new Vec3(kf.Second, kf.OutValue, 0.0f);
                tangentPositions[i] = p1 = new Vec3(kf.Second + tangentScale, kf.OutValue + kf.OutTangent * tangentScale, 0.0f);
                keyframeLines[i] = new VertexLine(p0, p1);
                ++i;
            }
            //Fill the rest in case of non-matching keyframe counts
            while (i < posCount)
            {
                KeyframePositions[i] = p0 = Vec3.Zero;
                tangentPositions[i] = p1 = Vec3.Zero;
                keyframeLines[i] = new VertexLine(p0, p1);
                ++i;
            }

            VertexLineStrip strip = new VertexLineStrip(false, splinePoints);

            RenderingParameters renderParams = new RenderingParameters
            {
                LineWidth = 1.0f,
                PointSize = 5.0f
            };

            PrimitiveData splineData = PrimitiveData.FromLineStrips(VertexShaderDesc.PosColor(), strip);
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

            _splinePrimitive = new PrimitiveManager(splineData, mat);

            //PrimitiveData velocityData = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), velocity);
            //mat = TMaterial.CreateUnlitColorMaterialForward(Color.Blue);
            //mat.RenderParams = renderParams;
            //_velocityPrimitive = new PrimitiveManager(velocityData, mat);

            PrimitiveData pointData = PrimitiveData.FromPoints(KeyframePositions);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Green);
            mat.RenderParams = renderParams;
            _pointPrimitive = new PrimitiveManager(pointData, mat);

            PrimitiveData tangentData = PrimitiveData.FromPoints(tangentPositions);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Purple);
            mat.RenderParams = renderParams;
            _tangentPositionPrimitive = new PrimitiveManager(tangentData, mat);

            PrimitiveData kfLineData = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), keyframeLines);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Orange);
            mat.RenderParams = renderParams;
            _keyframeLinesPrimitive = new PrimitiveManager(kfLineData, mat);

            PrimitiveData timePointData = PrimitiveData.FromPoints(Vec3.Zero);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.White);
            mat.RenderParams = renderParams;
            _timePointPrimitive = new PrimitiveManager(timePointData, mat);
            
            _rcPoints.Mesh = _pointPrimitive;
            _rcTangents.Mesh = _tangentPositionPrimitive;
            _rcSpline.Mesh = _splinePrimitive;
            _rcKfLines.Mesh = _keyframeLinesPrimitive;
            _rcCurrentPoint.Mesh = _timePointPrimitive;

            _regenerating = false;
        }

        private readonly RenderCommandMethod2D _rcSelectionArea;
        private readonly RenderCommandMesh2D _rcKfLines = new RenderCommandMesh2D();
        private readonly RenderCommandMesh2D _rcCurrentPoint = new RenderCommandMesh2D();
        private readonly RenderCommandMesh2D _rcSpline = new RenderCommandMesh2D();
        private readonly RenderCommandMesh2D _rcPoints = new RenderCommandMesh2D();
        private readonly RenderCommandMesh2D _rcTangents = new RenderCommandMesh2D();

        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(ERenderPass.OnTopForward, 0, 0);
        public BoundingRectangleF AxisAlignedRegion { get; } = new BoundingRectangleF();
        public IQuadtreeNode QuadtreeNode { get; set; }

        [TSerialize]
        public bool RenderSpline { get; set; } = true;
        [TSerialize]
        public bool RenderKeyframeTangentLines { get; set; } = true;
        [TSerialize]
        public bool RenderKeyframeTangentPoints { get; set; } = true;
        [TSerialize]
        public bool RenderKeyframePoints { get; set; } = true;
        [TSerialize]
        public bool RenderCurrentTimePoint { get; set; } = true;
        [TSerialize]
        public bool RenderSelectionArea { get; set; } = true;
        
        private PrimitiveManager _splinePrimitive;
        private PrimitiveManager _pointPrimitive;
        private PrimitiveManager _tangentPositionPrimitive;
        private PrimitiveManager _keyframeLinesPrimitive;
        private PrimitiveManager _timePointPrimitive;

        private PropAnimFloat _targetAnimation;
        private Vec2 _minScale = new Vec2(0.01f), _maxScale = new Vec2(1.0f);
        private Vec2 _lastWorldPos = Vec2.Zero;
        //private Vec2 _lastFocusPoint = Vec2.Zero;
        private bool _rightClickDown = false;
        private FloatKeyframe _selectedKf;
        private FloatKeyframe _highlightedKf;
        private List<DraggedKeyframeInfo> _draggedKeyframes = new List<DraggedKeyframeInfo>();

        protected override UICanvasComponent OnConstructRoot()
        {
            var root = base.OnConstructRoot();
            _baseTransformComponent.WorldTransformChanged += BaseWorldTransformChanged;
            return root;
        }
        private void _position_CurrentPositionChanged(PropAnimVector<float, FloatKeyframe> obj)
        {
            _rcCurrentPoint.WorldMatrix = _baseTransformComponent.WorldMatrix * Matrix4.CreateTranslation(_targetAnimation.CurrentTime, _targetAnimation.CurrentPosition, 0.0f);
        }
        private void BaseWorldTransformChanged()
        {
            Matrix4 mtx = _baseTransformComponent.WorldMatrix;

            _rcKfLines.WorldMatrix =
            _rcSpline.WorldMatrix =
            _rcPoints.WorldMatrix =
            _rcTangents.WorldMatrix = mtx;

            if (_targetAnimation != null)
            {
                Matrix4 pointMtx = Matrix4.CreateTranslation(_targetAnimation.CurrentTime, _targetAnimation.CurrentPosition, 0.0f);
                _rcCurrentPoint.WorldMatrix = mtx * pointMtx;
            }
            else
                _rcCurrentPoint.WorldMatrix = mtx;

            UpdateBackgroundMaterial();
        }
        public override void Resize(Vec2 bounds)
        {
            base.Resize(bounds);
            UpdateBackgroundMaterial();
        }
        private void UpdateBackgroundMaterial()
        {
            TMaterial mat = _backgroundComponent.InterfaceMaterial;
            mat.Parameter<ShaderFloat>(2).Value = _baseTransformComponent.ScaleX;
            mat.Parameter<ShaderVec2>(4).Value = _baseTransformComponent.LocalTranslation;
            float bound = _targetAnimation == null || _targetAnimation.LengthInSeconds <= 0.0f ? 1.0f : _targetAnimation.LengthInSeconds;
            float inc = 10.0f / bound;
            //float fraction = inc - (int)Math.Floor(inc);
            //Engine.PrintLine($"Increment: {inc.ToString()}");
            mat.Parameter<ShaderVec2>(5).Value = inc;
        }
        protected override void OnSpawnedPostComponentSpawn()
        {
            base.OnSpawnedPostComponentSpawn();
            ScreenSpaceUIScene.Add(this);
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
                new ShaderFloat(0.3f, "LineWidth"),
                new ShaderVec2(_baseTransformComponent.LocalTranslation, "Translation"),
                new ShaderVec2(new Vec2(10.0f), "XYIncrements"),
            },
            frag);
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterKeyPressed(EKey.AltLeft, b => _altDown = b, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ControlLeft, b => _ctrlDown = b, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, EButtonInputType.Pressed, LeftClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, EButtonInputType.Released, LeftClickUp, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, EButtonInputType.Pressed, RightClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, EButtonInputType.Released, RightClickUp, EInputPauseType.TickAlways);
            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, EMouseMoveType.Absolute, EInputPauseType.TickAlways);
            input.RegisterKeyEvent(EKey.Space, EButtonInputType.Pressed, TogglePlay, EInputPauseType.TickAlways);
        }
        public void TogglePlay()
        {
            if (_targetAnimation == null)
                return;

            if (_targetAnimation.State == EAnimationState.Playing)
                _targetAnimation.State = EAnimationState.Paused;
            else
                _targetAnimation.State = EAnimationState.Playing;
        }
        private bool _altDown = false;
        private bool _ctrlDown = false;
        internal void LeftClickDown()
        {
            if (_targetAnimation != null)
            {
                Vec2 animPos = CursorPositionTransformRelative();
                float sec = animPos.X;
                float val = animPos.Y;

                if (ClosestKeyframeIndices != null && ClosestKeyframeIndices.Length > 0)
                {
                    foreach (int index in ClosestKeyframeIndices)
                    {
                        _draggedKeyframes = new List<DraggedKeyframeInfo>();
                        var kf = _targetAnimation.Keyframes[index >> 1];
                        bool draggingInValue = (index & 1) == 0 || kf.SyncInOutValues;
                        bool draggingOutValue = (index & 1) == 1 || kf.SyncInOutValues;
                        _draggedKeyframes.Add(new DraggedKeyframeInfo()
                        {
                            Keyframe = kf,
                            DraggingInValue = draggingInValue,
                            DraggingOutValue = draggingOutValue
                        });
                    }
                }
                else
                {
                    _draggedKeyframes = null;

                    float length = TMath.Max(_targetAnimation.LengthInSeconds, sec);
                    var track = _targetAnimation.Keyframes;
                    bool nullTrack = track == null;
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
            _draggedKeyframes = null;
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
            Vec2 pos = CursorPosition();
            if (_draggedKeyframes != null && _draggedKeyframes.Count > 0)
                HandleDragKf(pos);
            else if (_rightClickDown)
                HandleDragView(pos);
            else
                HighlightGraph();
            _cursorPos = pos;
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
        private void HandleDragView(Vec2 cursorPosScreen)
        {
            Vec2 diff = GetWorldCursorDiff(cursorPosScreen);
            _baseTransformComponent.LocalTranslation += diff;
        }
        private class DraggedKeyframeInfo
        {
            public FloatKeyframe Keyframe { get; set; }
            public bool DraggingInValue { get; set; }
            public bool DraggingOutValue { get; set; }
            public bool DraggingInTangent { get; set; }
            public bool DraggingOutTangent { get; set; }
        }
        private void HandleDragKf(Vec2 cursorPosScreen)
        {
            //Vec2 diff = GetWorldCursorDiff(cursorPosScreen);
            Vec3 pos = CursorPositionTransformRelative();
            foreach (var kf in _draggedKeyframes)
            {
                kf.Keyframe.Second = pos.X;
                if (kf.DraggingInValue)
                    kf.Keyframe.InValue = pos.Y;
                if (kf.DraggingOutValue)
                    kf.Keyframe.OutValue = pos.Y;
            }
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
        public int[] ClosestKeyframeIndices { get; private set; }
        private void HighlightGraph()
        {
            ClosestKeyframeIndices = null;
            if (KeyframePositions == null)
                return;

            Vec2 cursorPos = CursorPositionTransformRelative();
            ClosestKeyframeIndices = KeyframePositions.FindAllMatchIndices(x => x.DistanceToFast(cursorPos) < SelectionRadius / _baseTransformComponent.ScaleX);

            //float minDist = float.MaxValue;
            //ClosestKeyframeIndices = new List<int>(pts.Length);
            //for (int i = 0; i < pts.Length; ++i)
            //{
            //    float dist = KeyframePositions[pts[i]].DistanceToFast(cursorPos);
            //    if (dist < minDist)
            //    {
            //        ClosestKeyframeIndices.Add(pts[i]);
            //        minDist = dist;
            //    }
            //}
        }
        protected override void OnScrolledInput(bool down)
        {
            Vec3 worldPoint = CursorPositionWorld();
            _baseTransformComponent.Zoom(down ? 0.1f : -0.1f, worldPoint.Xy, new Vec2(0.1f, 0.1f), null);
            //Engine.PrintLine($"UI scale: {_baseTransformComponent.Scale.X.ToString()}");
        }
        public void AddRenderables(RenderPasses passes)
        {
            if (_targetAnimation == null)
                return;

            if (RenderSpline)
                passes.Add(_rcSpline, RenderInfo.RenderPass);
            if (RenderKeyframePoints)
                passes.Add(_rcPoints, RenderInfo.RenderPass);
            if (RenderKeyframeTangentPoints)
                passes.Add(_rcTangents, RenderInfo.RenderPass);
            if (RenderKeyframeTangentLines)
                passes.Add(_rcKfLines, RenderInfo.RenderPass);
            if (RenderCurrentTimePoint)
                passes.Add(_rcCurrentPoint, RenderInfo.RenderPass);
            if (RenderSelectionArea)
                passes.Add(_rcSelectionArea, RenderInfo.RenderPass);
        }
        public float SelectionRadius { get; set; } = 10.0f;
        private void RenderSelectionAreaMethod()
        {
            Vec2 pos = CursorPositionWorld();
            Vec2 wh = _backgroundComponent.Size;

            if (ClosestKeyframeIndices != null)
                foreach (int index in ClosestKeyframeIndices)
                    if (KeyframePositions?.IndexInArrayRange(index) ?? false)
                        Engine.Renderer.RenderPoint(Vec3.TransformPosition(KeyframePositions[index], _baseTransformComponent.WorldMatrix), Color.Yellow, false, 10.0f);
            
            //Cursor
            Engine.Renderer.RenderCircle(pos + AbstractRenderer.UIPositionBias, AbstractRenderer.UIRotation, SelectionRadius /** _baseTransformComponent.ScaleX*/, false, Editor.TurquoiseColor, 10.0f);
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
        }
    }
}
