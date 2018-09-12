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
        
        private PropAnimVec3 _spline;
        private PrimitiveManager _splinePrimitive;
        private PrimitiveManager _velocityPrimitive;
        private PrimitiveManager _pointPrimitive;
        private PrimitiveManager _tangentPrimitive;
        private PrimitiveManager _timePointPrimitive;

        [TSerialize]
        public PropAnimVec3 Spline
        {
            get => _spline;
            set
            {
                if (_spline != null)
                {
                    _spline.Keyframes.Changed -= RegenerateSplinePrimitive;
                    _spline.FPSChanged -= RegenerateSplinePrimitive;
                    _spline.AnimationStarted -= _spline_AnimationStarted;
                    _spline.AnimationPaused -= _spline_AnimationEnded;
                    _spline.AnimationEnded -= _spline_AnimationEnded;
                    _spline_AnimationEnded();
                }
                _spline = value;
                if (_spline != null)
                {
                    _spline.Keyframes.Changed += RegenerateSplinePrimitive;
                    _spline.FPSChanged += RegenerateSplinePrimitive;
                    _spline.AnimationStarted += _spline_AnimationStarted;
                    _spline.AnimationPaused += _spline_AnimationEnded;
                    _spline.AnimationEnded += _spline_AnimationEnded;
                    if (_spline.State == EAnimationState.Playing)
                        _spline_AnimationStarted();
                }
                RegenerateSplinePrimitive();
            }
        }

        private void _spline_AnimationEnded()
            => UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, _spline.Progress, Input.Devices.EInputPauseType.TickAlways);
        private void _spline_AnimationStarted()
            => RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, _spline.Progress, Input.Devices.EInputPauseType.TickAlways);
        
        public SplineComponent() : base()
        {
            Spline = null;
        }
        public SplineComponent(PropAnimVec3 spline) : base()
        {
            Spline = spline;
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

            if (_spline == null || _spline.BakedFrameCount == 0)
                return;

            Vertex[] splinePoints = new Vertex[_spline.BakedFrameCount];
            VertexLine[] velocity = new VertexLine[_spline.BakedFrameCount];
            Vec3[] keyframePositions = new Vec3[_spline.Keyframes.Count << 1];
            Vec3[] tangentPositions = new Vec3[_spline.Keyframes.Count << 1];

            int i, x = 0;
            float sec;
            for (i = 0; i < splinePoints.Length; ++i)
            {
                sec = i / _spline.BakedFramesPerSecond;
                Vertex pos = new Vertex(_spline.GetValueKeyframed(sec));
                splinePoints[i] = pos;
                velocity[i] = new VertexLine(pos, new Vertex(pos.Position + _spline.GetVelocityKeyframed(sec).Normalized()));
            }
            i = 0;
            foreach (Vec3Keyframe keyframe in _spline)
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

        protected override void OnWorldTransformChanged()
        {
            //TODO: use parent matrix
            _rcSpline.WorldMatrix = WorldMatrix;
            _rcVelocity.WorldMatrix = WorldMatrix;
            _rcPoints.WorldMatrix = WorldMatrix;
            _rcTangents.WorldMatrix = WorldMatrix;
            base.OnWorldTransformChanged();
        }

        private readonly RenderCommandMesh3D _rcCurrentPoint = new RenderCommandMesh3D();
        private readonly RenderCommandMesh3D _rcSpline = new RenderCommandMesh3D();
        private readonly RenderCommandMesh3D _rcVelocity = new RenderCommandMesh3D();
        private readonly RenderCommandMesh3D _rcPoints = new RenderCommandMesh3D();
        private readonly RenderCommandMesh3D _rcTangents = new RenderCommandMesh3D();
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            if (_spline == null)
                return;

            passes.Add(_rcSpline, RenderInfo.RenderPass);

            if (RenderCurrentTimePoint)
            {
                Vec3 point = _spline.GetValueKeyframed(_spline.CurrentTime);
                _rcCurrentPoint.WorldMatrix = WorldMatrix * Matrix4.CreateTranslation(point);
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