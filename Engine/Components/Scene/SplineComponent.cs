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
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass.OpaqueForward, true, true) { CastsShadows = false, ReceivesShadows = false };

        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        [TSerialize]
        public bool RenderTangents { get; set; } = false;
        [TSerialize]
        public bool RenderKeyframeTangentPoints { get; set; } = true;
        [TSerialize]
        public bool RenderKeyframePoints { get; set; } = true;
        [TSerialize]
        public bool RenderCurrentTimePoint { get; set; } = true;
        
        private PropAnimVec3 _position;
        private PropAnimFloat _speed;
        private PrimitiveManager _splinePrimitive;
        private PrimitiveManager _velocityPrimitive;
        private PrimitiveManager _pointPrimitive;
        private PrimitiveManager _tangentPrimitive;
        private PrimitiveManager _timePointPrimitive;

        [TSerialize]
        public PropAnimVec3 Position
        {
            get => _position;
            set
            {
                if (_position != null)
                {
                    _position.Keyframes.Changed -= RegenerateSplinePrimitive;
                    _position.ConstrainKeyframedFPSChanged -= RegenerateSplinePrimitive;
                    _position.FPSChanged -= RegenerateSplinePrimitive;
                    _position.LengthChanged -= RegenerateSplinePrimitive;
                    _position.AnimationStarted -= _spline_AnimationStarted;
                    _position.AnimationPaused -= _spline_AnimationEnded;
                    _position.AnimationEnded -= _spline_AnimationEnded;
                    _position.CurrentPositionChanged -= RecalcLocalTransform;
                    _spline_AnimationEnded();
                }
                _position = value;
                if (_position != null)
                {
                    _position.Keyframes.Changed += RegenerateSplinePrimitive;
                    _position.ConstrainKeyframedFPSChanged += RegenerateSplinePrimitive;
                    _position.FPSChanged += RegenerateSplinePrimitive;
                    _position.LengthChanged += RegenerateSplinePrimitive;
                    _position.AnimationStarted += _spline_AnimationStarted;
                    _position.AnimationPaused += _spline_AnimationEnded;
                    _position.AnimationEnded += _spline_AnimationEnded;
                    _position.CurrentPositionChanged += RecalcLocalTransform;
                    if (_position.State == EAnimationState.Playing)
                        _spline_AnimationStarted();
                }
                RegenerateSplinePrimitive();
            }
        }
        
        [TSerialize]
        public PropAnimFloat Speed
        {
            get => _speed;
            set => _speed = value;
        }

        private void _spline_AnimationEnded()
            => UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress, Input.Devices.EInputPauseType.TickAlways);
        private void _spline_AnimationStarted()
            => RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress, Input.Devices.EInputPauseType.TickAlways);
        private void Progress(float delta)
        {
            if (Speed != null)
            {
                _position.Speed = Speed.CurrentPosition;
                Speed.Progress(delta);
            }
            _position.Progress(delta);
        }

        public SplineComponent() : base()
        {
            Position = null;
        }
        public SplineComponent(PropAnimVec3 spline) : base()
        {
            Position = spline;
        }

        //[PostDeserialize]
        //private void PostDeserialize()
        //{

        //}

        //public override void RecalcWorldTransform()
        //{
        //    base.RecalcWorldTransform();
        //    //RenderNode?.ItemMoved(this);
        //}

        public void RegenerateSplinePrimitive()
        {
            _splinePrimitive?.Dispose();
            _splinePrimitive = null;
            _velocityPrimitive?.Dispose();
            _velocityPrimitive = null;
            _pointPrimitive?.Dispose();
            _pointPrimitive = null;
            _tangentPrimitive?.Dispose();
            _tangentPrimitive = null;
            _timePointPrimitive?.Dispose();
            _timePointPrimitive = null;

            if (_position == null || _position.LengthInSeconds <= 0.0f)
                return;

            float fps = _position.ConstrainKeyframedFPS ? _position.BakedFramesPerSecond : (Engine.TargetFramesPerSecond == 0 ? 60.0f : Engine.TargetFramesPerSecond);
            int frameCount = (int)Math.Ceiling(_position.LengthInSeconds * fps);
            
            Vertex[] splinePoints = new Vertex[frameCount];
            VertexLine[] velocity = new VertexLine[frameCount];
            Vec3[] keyframePositions = new Vec3[_position.Keyframes.Count << 1];
            Vec3[] tangentPositions = new Vec3[_position.Keyframes.Count << 1];

            int i, x = 0;
            float sec;
            for (i = 0; i < splinePoints.Length; ++i)
            {
                sec = i / fps;
                Vertex pos = new Vertex(_position.GetValueKeyframed(sec));
                splinePoints[i] = pos;
                velocity[i] = new VertexLine(pos, new Vertex(pos.Position + _position.GetVelocityKeyframed(sec).Normalized()));
            }
            i = 0;
            foreach (Vec3Keyframe keyframe in _position)
            {
                keyframePositions[i++] = keyframe.InValue;
                keyframePositions[i++] = keyframe.OutValue;
                tangentPositions[x++] = keyframe.InValue + keyframe.InTangent;
                tangentPositions[x++] = keyframe.OutValue + keyframe.OutTangent;
            }

            VertexLineStrip strip = new VertexLineStrip(false, splinePoints);

            RenderingParameters p = new RenderingParameters
            {
                LineWidth = 1.0f,
                PointSize = 5.0f
            };

            PrimitiveData splineData = PrimitiveData.FromLineStrips(VertexShaderDesc.JustPositions(), strip);
            TMaterial mat = TMaterial.CreateUnlitColorMaterialForward(Color.Red);
            mat.RenderParams = p;
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
            _tangentPrimitive = new PrimitiveManager(tangentData, mat);

            PrimitiveData timePointData = PrimitiveData.FromPoints(Vec3.Zero);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.White);
            mat.RenderParams = p;
            _timePointPrimitive = new PrimitiveManager(timePointData, mat);

            _rcVelocity.Mesh = _velocityPrimitive;
            _rcPoints.Mesh = _pointPrimitive;
            _rcTangents.Mesh = _tangentPrimitive;
            _rcSpline.Mesh = _splinePrimitive;
            _rcCurrentPoint.Mesh = _timePointPrimitive;
        }

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (OwningScene3D != null)
            {
                if (selected)
                    OwningScene3D.Add(this);
                else
                    OwningScene3D.Remove(this);
            }
            base.OnSelectedChanged(selected);
        }
        private Matrix4 _transform = Matrix4.Identity;
        private Matrix4 _splineLocalTransform = Matrix4.Identity;
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);

            _transform = localTransform;
            _splineLocalTransform = Matrix4.CreateTranslation(_position.CurrentPosition);

            localTransform = _transform * _splineLocalTransform;
            inverseLocalTransform = Matrix4.CreateTranslation(-_position.CurrentPosition) * inverseLocalTransform;
        }
        protected override void OnWorldTransformChanged()
        {
            Matrix4 mtx = _transform;
            _rcSpline.WorldMatrix = mtx;
            _rcVelocity.WorldMatrix = mtx;
            _rcPoints.WorldMatrix = mtx;
            _rcTangents.WorldMatrix = mtx;
            base.OnWorldTransformChanged();
        }

        private readonly RenderCommandMesh3D _rcCurrentPoint = new RenderCommandMesh3D();
        private readonly RenderCommandMesh3D _rcSpline = new RenderCommandMesh3D();
        private readonly RenderCommandMesh3D _rcVelocity = new RenderCommandMesh3D();
        private readonly RenderCommandMesh3D _rcPoints = new RenderCommandMesh3D();
        private readonly RenderCommandMesh3D _rcTangents = new RenderCommandMesh3D();
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            if (_position == null)
                return;

            passes.Add(_rcSpline, RenderInfo.RenderPass);

            if (RenderCurrentTimePoint)
            {
                _rcCurrentPoint.WorldMatrix = WorldMatrix;
                passes.Add(_rcCurrentPoint, RenderInfo.RenderPass);
            }
            if (RenderTangents)
                passes.Add(_rcVelocity, RenderInfo.RenderPass);
            if (RenderKeyframePoints)
                passes.Add(_rcPoints, RenderInfo.RenderPass);
            if (RenderKeyframeTangentPoints)
                passes.Add(_rcTangents, RenderInfo.RenderPass);
        }
#endif
    }
}