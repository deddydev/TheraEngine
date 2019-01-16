using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Animation;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene
{
    public class SplineComponent : TRSComponent, I3DRenderable
    {
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, true) { CastsShadows = false, ReceivesShadows = false };
        
        [Browsable(false)]
        public Shape CullingVolume { get; set; } = null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        [TSerialize]
        public bool RenderBounds { get; set; } = true;
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
        [TSerialize]
        public bool RenderExtrema { get; set; } = true;
        
        private PropAnimVec3 _position;
        private PrimitiveManager _splinePrimitive;
        private PrimitiveManager _velocityTangentsPrimitive;
        private PrimitiveManager _pointPrimitive;
        private PrimitiveManager _tangentPrimitive;
        private PrimitiveManager _keyframeLinesPrimitive;
        private PrimitiveManager _timePointPrimitive;
        private PrimitiveManager _extremaPrimitive;

        [TSerialize]
        public PropAnimVec3 Position
        {
            get => _position;
            set
            {
                if (_position != null)
                {
                    _position.Keyframes.Changed -= Keyframes_Changed;
                    _position.ConstrainKeyframedFPSChanged -= _position_ConstrainKeyframedFPSChanged;
                    _position.BakedFPSChanged -= _position_BakedFPSChanged1;
                    _position.LengthChanged -= _position_LengthChanged;
                    //_position.AnimationStarted -= _spline_AnimationStarted;
                    //_position.AnimationPaused -= _spline_AnimationEnded;
                    //_position.AnimationEnded -= _spline_AnimationEnded;
                    _position.CurrentPositionChanged -= _position_CurrentPositionChanged;
                    //_spline_AnimationEnded();
                }
                _position = value;
                if (_position != null)
                {
                    _position.Keyframes.Changed += Keyframes_Changed;
                    _position.ConstrainKeyframedFPSChanged += _position_ConstrainKeyframedFPSChanged;
                    _position.BakedFPSChanged += _position_BakedFPSChanged1;
                    _position.LengthChanged += _position_LengthChanged;
                    //_position.AnimationStarted += _spline_AnimationStarted;
                    //_position.AnimationPaused += _spline_AnimationEnded;
                    //_position.AnimationEnded += _spline_AnimationEnded;
                    _position.CurrentPositionChanged += _position_CurrentPositionChanged;
                    _position.TickSelf = true;
                    //if (_position.State == EAnimationState.Playing)
                    //    _spline_AnimationStarted();
                }
                RegenerateSplinePrimitive();
            }
        }

        private void _position_BakedFPSChanged1(BasePropAnimBakeable obj)
        {
            RegenerateSplinePrimitive();
        }
        private void _position_CurrentPositionChanged(PropAnimVector<Vec3, Vec3Keyframe> obj)
        {
            RecalcLocalTransform();
            //RegenerateSplinePrimitive();
        }
        private void _position_LengthChanged(BaseAnimation obj)
        {
            RegenerateSplinePrimitive();
        }
        private void _position_ConstrainKeyframedFPSChanged(PropAnimVector<Vec3, Vec3Keyframe> obj)
        {
            RegenerateSplinePrimitive();
        }
        private void Keyframes_Changed(BaseKeyframeTrack obj)
        {
            RegenerateSplinePrimitive();
        }

        public SplineComponent() : base()
        {
            Position = null;
        }
        public SplineComponent(PropAnimVec3 spline) : base()
        {
            Position = spline;
        }

        EventVec3 _cullingVolumeTranslation = null;
        public void RegenerateSplinePrimitive()
        {
            _splinePrimitive?.Dispose();
            _splinePrimitive = null;
            _velocityTangentsPrimitive?.Dispose();
            _velocityTangentsPrimitive = null;
            _pointPrimitive?.Dispose();
            _pointPrimitive = null;
            _tangentPrimitive?.Dispose();
            _tangentPrimitive = null;
            _timePointPrimitive?.Dispose();
            _timePointPrimitive = null;
            _extremaPrimitive?.Dispose();
            _extremaPrimitive = null;

            if (_position == null || _position.LengthInSeconds <= 0.0f)
                return;

            //TODO: when the FPS is unconstrained, use adaptive vertex points based on velocity/acceleration
            float fps = _position.ConstrainKeyframedFPS ?
                _position.BakedFramesPerSecond :
                (Engine.TargetFramesPerSecond == 0 ? 60.0f : Engine.TargetFramesPerSecond);

            int frameCount = (int)Math.Ceiling(_position.LengthInSeconds * fps) + 1;
            float invFps = 1.0f / fps;
            int kfCount = _position.Keyframes.Count << 1;

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
                Vec3 val = _position.GetValueKeyframed(sec);
                Vec3 vel = _position.GetVelocityKeyframed(sec);
                float velLength = vel.LengthFast;
                Vec3 velColor = Vec3.Lerp(Vec3.UnitZ, Vec3.UnitX, 1.0f / (1.0f + 0.1f * (velLength * velLength)));
                Vertex pos = new Vertex(val) { Color = velColor };
                splinePoints[i] = pos;
                velocity[i] = new VertexLine(pos, new Vertex(pos.Position + vel.Normalized()));
            }
            i = 0;
            Vec3 p0, p1;
            foreach (Vec3Keyframe kf in _position)
            {
                keyframePositions[i] = p0 = kf.InValue;
                tangentPositions[i] = p1 = p0 + kf.InTangent;
                keyframeLines[i] = new VertexLine(p0, p1);
                ++i;

                keyframePositions[i] = p0 = kf.OutValue;
                tangentPositions[i] = p1 = p0 + kf.OutTangent;
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

            _position.GetMinMax(
                out (float Time, float Value)[] min, 
                out (float Time, float Value)[] max);

            Vec3[] extrema = new Vec3[6];
            for (int x = 0, index = 0; x < 3; ++x)
            {
                var (TimeMin, ValueMin) = min[x];
                Vec3 minPos = _position.GetValue(TimeMin);
                minPos[x] = ValueMin;
                
                var (TimeMax, ValueMax) = max[x];
                Vec3 maxPos = _position.GetValue(TimeMax);
                maxPos[x] = ValueMax;
                
                extrema[index++] = minPos;
                extrema[index++] = maxPos;
            }

            TMath.ComponentMinMax(out Vec3 minVal, out Vec3 maxVal, extrema);
            var box = BoundingBox.FromMinMax(minVal, maxVal);
            _cullingVolumeTranslation = box.Translation;
            CullingVolume = box;

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
            _velocityTangentsPrimitive = new PrimitiveManager(velocityData, mat);

            PrimitiveData pointData = PrimitiveData.FromPoints(keyframePositions);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Green);
            mat.RenderParams = p;
            _pointPrimitive = new PrimitiveManager(pointData, mat);

            PrimitiveData extremaData = PrimitiveData.FromPoints(extrema);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Red);
            mat.RenderParams = p;
            _extremaPrimitive = new PrimitiveManager(extremaData, mat);

            PrimitiveData tangentData = PrimitiveData.FromPoints(tangentPositions);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Purple);
            mat.RenderParams = p;
            _tangentPrimitive = new PrimitiveManager(tangentData, mat);

            PrimitiveData kfLineData = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), keyframeLines);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Orange);
            mat.RenderParams = p;
            _keyframeLinesPrimitive = new PrimitiveManager(kfLineData, mat);

            PrimitiveData timePointData = PrimitiveData.FromPoints(Vec3.Zero);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.White);
            mat.RenderParams = p;
            _timePointPrimitive = new PrimitiveManager(timePointData, mat);

            _rcVelocityTangents.Mesh = _velocityTangentsPrimitive;
            _rcPoints.Mesh = _pointPrimitive;
            _rcKeyframeTangents.Mesh = _tangentPrimitive;
            _rcSpline.Mesh = _splinePrimitive;
            _rcKfLines.Mesh = _keyframeLinesPrimitive;
            _rcCurrentPoint.Mesh = _timePointPrimitive;
            _rcExtrema.Mesh = _extremaPrimitive;
        }
        private Matrix4 _localTRS = Matrix4.Identity;
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);

            Matrix4 splinePosMtx, invSplinePosMtx;
            if (_position != null)
            {
                splinePosMtx = Matrix4.CreateTranslation(_position.CurrentPosition);
                invSplinePosMtx = Matrix4.CreateTranslation(-_position.CurrentPosition);
            }
            else
            {
                splinePosMtx = Matrix4.Identity;
                invSplinePosMtx = Matrix4.Identity;
            }

            _localTRS = localTransform;

            localTransform = _localTRS * splinePosMtx;
            inverseLocalTransform = invSplinePosMtx * inverseLocalTransform;
        }
        protected override void DeriveMatrix()
        {
            Transform.DeriveTRS(_localTRS, out Vec3 t, out Vec3 s, out Quat r);
            _translation.SetRawNoUpdate(t);
            _scale.SetRawNoUpdate(s);
            _rotation.SetRotationsNoUpdate(r.ToYawPitchRoll());
        }
        protected override void OnWorldTransformChanged()
        {
            Matrix4 mtx = GetParentMatrix() * _localTRS;
            _rcKfLines.WorldMatrix = mtx;
            _rcSpline.WorldMatrix = mtx;
            _rcVelocityTangents.WorldMatrix = mtx;
            _rcPoints.WorldMatrix = mtx;
            _rcKeyframeTangents.WorldMatrix = mtx;
            _rcExtrema.WorldMatrix = mtx;
            _rcCurrentPoint.WorldMatrix = WorldMatrix;

            CullingVolume?.SetTransformMatrix(mtx * _cullingVolumeTranslation.AsTranslationMatrix());

            base.OnWorldTransformChanged();
        }
        
        private readonly RenderCommandMesh3D _rcKfLines = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcCurrentPoint = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcSpline = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcVelocityTangents = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcPoints = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcKeyframeTangents = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcExtrema = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            if (_position == null)
                return;

            if (RenderSpline)
                passes.Add(_rcSpline);
            if (RenderTangents)
                passes.Add(_rcVelocityTangents);
            if (RenderKeyframePoints)
                passes.Add(_rcPoints);
            if (RenderKeyframeTangentPoints)
                passes.Add(_rcKeyframeTangents);
            if (RenderKeyframeTangentLines)
                passes.Add(_rcKfLines);
            if (RenderExtrema)
                passes.Add(_rcExtrema);
            if (RenderBounds)
                CullingVolume?.AddRenderables(passes, camera);
            if (RenderCurrentTimePoint)
                passes.Add(_rcCurrentPoint);
        }
    }
}