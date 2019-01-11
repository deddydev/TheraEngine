using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Animation;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
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
        public UIPropAnimFloatEditor() : base()
        {

        }
        public UIPropAnimFloatEditor(Vec2 bounds) : base(bounds)
        {

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

        private void _position_BakedFPSChanged1(BasePropAnimBakeable obj)
        {
            RegenerateSplinePrimitive();
        }
        private void _position_LengthChanged(BaseAnimation obj)
        {
            RegenerateSplinePrimitive();
        }
        private void _position_ConstrainKeyframedFPSChanged(PropAnimVector<float, FloatKeyframe> obj)
        {
            RegenerateSplinePrimitive();
        }
        private void Keyframes_Changed(BaseKeyframeTrack obj)
        {
            RegenerateSplinePrimitive();
        }

        public void RegenerateSplinePrimitive()
        {
            _splinePrimitive?.Dispose();
            _splinePrimitive = null;
            _velocityPrimitive?.Dispose();
            _velocityPrimitive = null;
            _pointPrimitive?.Dispose();
            _pointPrimitive = null;
            _tangentPositionPrimitive?.Dispose();
            _tangentPositionPrimitive = null;
            _timePointPrimitive?.Dispose();
            _timePointPrimitive = null;

            if (_targetAnimation == null || _targetAnimation.LengthInSeconds <= 0.0f)
                return;

            //TODO: when the FPS is unconstrained, use adaptive vertex points based on velocity/acceleration
            float fps = _targetAnimation.ConstrainKeyframedFPS ?
                _targetAnimation.BakedFramesPerSecond :
                (Engine.TargetFramesPerSecond == 0 ? 60.0f : Engine.TargetFramesPerSecond);

            if (fps <= 0.0f)
                return;

            int frameCount = (int)Math.Ceiling(_targetAnimation.LengthInSeconds * fps) + 1;
            float invFps = 1.0f / fps;
            int kfCount = _targetAnimation.Keyframes.Count << 1;

            Vertex[] splinePoints = new Vertex[frameCount];
            VertexLine[] velocity = new VertexLine[frameCount];
            Vec3[] keyframePositions = new Vec3[kfCount];
            VertexLine[] keyframeLines = new VertexLine[kfCount];
            Vec3[] tangentPositions = new Vec3[kfCount];

            int i;
            float sec;
            for (i = 0; i < splinePoints.Length; ++i)
            {
                sec = i * invFps;
                float val = _targetAnimation.GetValueKeyframed(sec);
                float vel = _targetAnimation.GetVelocityKeyframed(sec);
                float sigmoid = 1.0f / (1.0f + 0.1f * (vel * vel));
                Vec3 velColor = Vec3.Lerp(Vec3.UnitZ, Vec3.UnitX, sigmoid);
                Vec3 posVal = new Vec3(sec, val, 0.0f);
                Vertex pos = new Vertex(posVal) { Color = velColor };
                splinePoints[i] = pos;

                Vec3 velPos = pos.Position;
                velPos += new Vec3(new Vec2(1.0f, vel).Normalized(), 0.0f);
                velocity[i] = new VertexLine(pos, new Vertex(velPos));
            }
            i = 0;
            Vec3 p0, p1;
            float tangentScale = 10.0f;
            foreach (FloatKeyframe kf in _targetAnimation)
            {
                keyframePositions[i] = p0 = new Vec3(kf.Second, kf.InValue, 0.0f);
                tangentPositions[i] = p1 = new Vec3(kf.Second - tangentScale, kf.InValue + kf.InTangent * tangentScale, 0.0f);
                keyframeLines[i] = new VertexLine(p0, p1);
                ++i;

                keyframePositions[i] = p0 = new Vec3(kf.Second, kf.OutValue, 0.0f);
                tangentPositions[i] = p1 = new Vec3(kf.Second + tangentScale, kf.OutValue + kf.OutTangent * tangentScale, 0.0f);
                keyframeLines[i] = new VertexLine(p0, p1);
                ++i;
            }
            //Fill the rest in case of non-matching keyframe counts
            while (i < kfCount)
            {
                keyframePositions[i] = p0 = Vec3.Zero;
                tangentPositions[i] = p1 = Vec3.Zero;
                keyframeLines[i] = new VertexLine(p0, p1);
                ++i;
            }

            VertexLineStrip strip = new VertexLineStrip(false, splinePoints);

            RenderingParameters p = new RenderingParameters
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
"))
            {
                RenderParams = p
            };
            _splinePrimitive = new PrimitiveManager(splineData, mat);

            PrimitiveData velocityData = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), velocity);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Blue);
            mat.RenderParams = p;
            _velocityPrimitive = new PrimitiveManager(velocityData, mat);

            PrimitiveData pointData = PrimitiveData.FromPoints(keyframePositions);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Green);
            mat.RenderParams = p;
            _pointPrimitive = new PrimitiveManager(pointData, mat);

            PrimitiveData tangentData = PrimitiveData.FromPoints(tangentPositions);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Purple);
            mat.RenderParams = p;
            _tangentPositionPrimitive = new PrimitiveManager(tangentData, mat);

            PrimitiveData kfLineData = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), keyframeLines);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Orange);
            mat.RenderParams = p;
            _keyframeLinesPrimitive = new PrimitiveManager(kfLineData, mat);

            PrimitiveData timePointData = PrimitiveData.FromPoints(Vec3.Zero);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.White);
            mat.RenderParams = p;
            _timePointPrimitive = new PrimitiveManager(timePointData, mat);

            _rcVelocity.Mesh = _velocityPrimitive;
            _rcPoints.Mesh = _pointPrimitive;
            _rcTangents.Mesh = _tangentPositionPrimitive;
            _rcSpline.Mesh = _splinePrimitive;
            _rcKfLines.Mesh = _keyframeLinesPrimitive;
            _rcCurrentPoint.Mesh = _timePointPrimitive;
        }

        private readonly RenderCommandMesh2D _rcKfLines = new RenderCommandMesh2D() { WorldMatrix = Matrix4.Identity };
        private readonly RenderCommandMesh2D _rcCurrentPoint = new RenderCommandMesh2D() { WorldMatrix = Matrix4.Identity };
        private readonly RenderCommandMesh2D _rcSpline = new RenderCommandMesh2D() { WorldMatrix = Matrix4.Identity };
        private readonly RenderCommandMesh2D _rcVelocity = new RenderCommandMesh2D() { WorldMatrix = Matrix4.Identity };
        private readonly RenderCommandMesh2D _rcPoints = new RenderCommandMesh2D() { WorldMatrix = Matrix4.Identity };
        private readonly RenderCommandMesh2D _rcTangents = new RenderCommandMesh2D() { WorldMatrix = Matrix4.Identity };

        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(ERenderPass.OnTopForward, 0, 0);
        public BoundingRectangleF AxisAlignedRegion { get; } = new BoundingRectangleF();
        public IQuadtreeNode QuadtreeNode { get; set; }

        [TSerialize]
        public bool RenderSpline { get; set; } = true;
        [TSerialize]
        public bool RenderTangents { get; set; } = false;
        [TSerialize]
        public bool RenderKeyframeTangentLines { get; set; } = true;
        [TSerialize]
        public bool RenderKeyframeTangentPoints { get; set; } = true;
        [TSerialize]
        public bool RenderKeyframePoints { get; set; } = true;
        [TSerialize]
        public bool RenderCurrentTimePoint { get; set; } = true;

        private PrimitiveManager _splinePrimitive;
        private PrimitiveManager _velocityPrimitive;
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
        private FloatKeyframe _draggedKf;

        protected override UICanvasComponent OnConstructRoot()
        {
            var root = base.OnConstructRoot();
            _baseTransformComponent.WorldTransformChanged += BaseWorldTransformChanged;
            return root;
        }
        private void _position_CurrentPositionChanged(PropAnimVector<float, FloatKeyframe> obj)
        {
            _rcCurrentPoint.WorldMatrix = _baseTransformComponent.WorldMatrix * Matrix4.CreateTranslation(_targetAnimation.CurrentTime, _targetAnimation.CurrentPosition, 0.0f);
            //RegenerateSplinePrimitive();
        }
        private void BaseWorldTransformChanged()
        {
            Matrix4 mtx = _baseTransformComponent.WorldMatrix;

            _rcKfLines.WorldMatrix =
            _rcSpline.WorldMatrix =
            _rcVelocity.WorldMatrix =
            _rcPoints.WorldMatrix =
            _rcTangents.WorldMatrix = mtx;

            if (_targetAnimation != null)
            {
                Matrix4 pointMtx = Matrix4.CreateTranslation(_targetAnimation.CurrentTime, _targetAnimation.CurrentPosition, 0.0f);
                _rcCurrentPoint.WorldMatrix = mtx * pointMtx;
            }
            else
            {
                _rcCurrentPoint.WorldMatrix = mtx;
            }

            TMaterial mat = _backgroundComponent.InterfaceMaterial;
            mat.Parameter<ShaderFloat>(2).Value = _baseTransformComponent.ScaleX;
            mat.Parameter<ShaderVec2>(4).Value = _baseTransformComponent.LocalTranslation;
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
                new ShaderVec3(new Vec3(0.3f, 0.12f, 0.13f), "LineColor"),
                new ShaderVec3(new Vec3(0.35f, 0.27f, 0.3f), "BGColor"),
                new ShaderFloat(1.0f, "Scale"),
                new ShaderFloat(0.05f, "LineWidth"),
                new ShaderVec2(new Vec2(0.0f), "Translation"),
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
        }
        private bool _altDown = false;
        private bool _ctrlDown = false;
        internal void LeftClickDown()
        {
            Vec2 worldPos = CursorPositionWorld();
            if (_targetAnimation != null)
            {
                float sec = worldPos.X;
                float val = worldPos.Y;
                _targetAnimation.Keyframes?.Add(new FloatKeyframe(sec, val, 2.0f, EVectorInterpType.Linear));
                float length = TMath.Max(_targetAnimation.LengthInSeconds, sec);
                if (length > _targetAnimation.LengthInSeconds)
                    _targetAnimation.LengthInSeconds = length;
            }
            RegenerateSplinePrimitive();
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
            //if (_draggedArg != null && _highlightedArg != null &&
            //    !ReferenceEquals(_draggedArg, _highlightedArg) &&
            //    _draggedArg.ConnectTo(_highlightedArg))
            //{
            //    if (_draggedArg is IFuncValueInput input1 && _highlightedArg is IFuncValueOutput output1)
            //        OnArgumentsConnected(input1, output1);
            //    else if (_highlightedArg is IFuncValueInput input2 && _draggedArg is IFuncValueOutput output2)
            //        OnArgumentsConnected(input2, output2);
            //}

            //_draggedFunc = null;
            //_inputTree = null;
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
            if (_draggedKf != null)
                HandleDragKf(pos);
            else
            if (_rightClickDown)
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
        private bool _draggingInValue;
        private bool _draggingTangent;
        private void HandleDragKf(Vec2 cursorPosScreen)
        {
            Vec2 diff = GetWorldCursorDiff(cursorPosScreen);
            if (_draggingInValue)
            {
                if (_draggingTangent)
                {
                    //Vec2 pos = new Vec2(draggedKf.Second, draggedKf.InValue) + draggedKf.InTangent;
                }
                else
                {

                }
            }
            else
            {
                if (_draggingTangent)
                {

                }
                else
                {

                }
            }
            //draggedKf.InTangent += Vec3.TransformVector(diff, draggedKf.InverseWorldMatrix).Xy;
        }
        private void HighlightGraph()
        {
            UIComponent comp = FindComponent();
            
        }
        public override void Resize(Vec2 bounds)
        {
            base.Resize(bounds);

            TMaterial mat = _backgroundComponent.InterfaceMaterial;
            mat.Parameter<ShaderFloat>(2).Value = _baseTransformComponent.ScaleX;
            mat.Parameter<ShaderVec2>(4).Value = _baseTransformComponent.LocalTranslation;
        }
        protected override void OnScrolledInput(bool down)
        {
            Vec3 worldPoint = CursorPositionWorld();
            _baseTransformComponent.Zoom(down ? 0.1f : -0.1f, worldPoint.Xy, null, null);
        }
        public void AddRenderables(RenderPasses passes)
        {
            if (_targetAnimation == null)
                return;

            if (RenderSpline)
                passes.Add(_rcSpline, RenderInfo.RenderPass);
            if (RenderTangents)
                passes.Add(_rcVelocity, RenderInfo.RenderPass);
            if (RenderKeyframePoints)
                passes.Add(_rcPoints, RenderInfo.RenderPass);
            if (RenderKeyframeTangentPoints)
                passes.Add(_rcTangents, RenderInfo.RenderPass);
            if (RenderKeyframeTangentLines)
                passes.Add(_rcKfLines, RenderInfo.RenderPass);
            if (RenderCurrentTimePoint)
                passes.Add(_rcCurrentPoint, RenderInfo.RenderPass);
        }
    }
}
