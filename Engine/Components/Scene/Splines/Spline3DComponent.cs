﻿using System;
using System.Drawing;
using TheraEngine.Animation;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene
{
    public class Spline3DComponent : TransformComponent, I3DRenderable
    {
        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, true) { CastsShadows = false, ReceivesShadows = false };
        
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
        
        private LocalFileRef<PropAnimVec3> _spline = new LocalFileRef<PropAnimVec3>();
        private MeshRenderer _splinePrimitive;
        private MeshRenderer _velocityTangentsPrimitive;
        private MeshRenderer _pointPrimitive;
        private MeshRenderer _tangentPrimitive;
        private MeshRenderer _keyframeLinesPrimitive;
        private MeshRenderer _timePointPrimitive;
        private MeshRenderer _extremaPrimitive;

        [TSerialize]
        public LocalFileRef<PropAnimVec3> SplineRef
        {
            get => _spline;
            set
            {
                if (_spline != null)
                {
                    _spline.Loaded -= _spline_Loaded;
                    _spline.Unloaded -= _spline_Unloaded;
                }
                _spline = value ?? new LocalFileRef<PropAnimVec3>();
                if (_spline != null)
                {
                    _spline.Loaded += _spline_Loaded;
                    _spline.Unloaded += _spline_Unloaded;
                }
            }
        }

        public PropAnimVec3 Spline => SplineRef.File;

        protected override async void OnSpawned()
        {
            base.OnSpawned();
            await SplineRef.GetInstanceAsync();
        }

        private void _spline_Unloaded(PropAnimVec3 spline)
        {
            spline.Keyframes.Changed -= Keyframes_Changed;
            spline.ConstrainKeyframedFPSChanged -= _position_ConstrainKeyframedFPSChanged;
            spline.BakedFPSChanged -= _position_BakedFPSChanged1;
            spline.LengthChanged -= _position_LengthChanged;
            spline.CurrentPositionChanged -= _position_CurrentPositionChanged;
            //spline.AnimationStarted -= _spline_AnimationStarted;
            //spline.AnimationPaused -= _spline_AnimationEnded;
            //spline.AnimationEnded -= _spline_AnimationEnded;
        }
        private void _spline_Loaded(PropAnimVec3 spline)
        {
            spline.Keyframes.Changed += Keyframes_Changed;
            spline.ConstrainKeyframedFPSChanged += _position_ConstrainKeyframedFPSChanged;
            spline.BakedFPSChanged += _position_BakedFPSChanged1;
            spline.LengthChanged += _position_LengthChanged;
            spline.CurrentPositionChanged += _position_CurrentPositionChanged;
            //spline.AnimationStarted += _spline_AnimationStarted;
            //spline.AnimationPaused += _spline_AnimationEnded;
            //spline.AnimationEnded += _spline_AnimationEnded;

            RecalcLocalTransform();
            RegenerateSplinePrimitive();
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

        public Spline3DComponent() : this(null) { }
        public Spline3DComponent(PropAnimVec3 spline) : base() => SplineRef = spline;
        
        EventVec3 _cullingVolumeTranslation = null;
        public void RegenerateSplinePrimitive()
        {
            var spline = Spline;

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

            if (spline is null || spline.LengthInSeconds <= 0.0f)
                return;

            //TODO: when the FPS is unconstrained, use adaptive vertex points based on velocity/acceleration
            float fps = spline.ConstrainKeyframedFPS || spline.IsBaked ?
                spline.BakedFramesPerSecond :
                (Engine.TargetFramesPerSecond == 0 ? 30.0f : Engine.TargetFramesPerSecond);

            int frameCount = (int)Math.Ceiling(spline.LengthInSeconds * fps) + 1;
            float invFps = 1.0f / fps;
            int kfCount = spline.Keyframes.Count << 1;

            TVertex[] splinePoints = new TVertex[frameCount];
            TVertexLine[] velocity = new TVertexLine[frameCount];
            Vec3[] keyframePositions = new Vec3[kfCount];
            TVertexLine[] keyframeLines = new TVertexLine[kfCount];
            Vec3[] tangentPositions = new Vec3[kfCount];

            int i;
            float sec;
            for (i = 0; i < splinePoints.Length; ++i)
            {
                sec = i * invFps;
                Vec3 val = spline.GetValue(sec);
                Vec3 vel = spline.GetVelocityKeyframed(sec);
                float velLength = vel.LengthFast;
                Vec3 velColor = Vec3.Lerp(Vec3.UnitZ, Vec3.UnitX, 1.0f / (1.0f + 0.1f * (velLength * velLength)));
                TVertex pos = new TVertex(val) { Color = velColor };
                splinePoints[i] = pos;
                velocity[i] = new TVertexLine(pos, new TVertex(pos.Position + vel.Normalized()));
            }
            i = 0;
            Vec3 p0, p1;
            foreach (Vec3Keyframe kf in spline)
            {
                keyframePositions[i] = p0 = kf.InValue;
                tangentPositions[i] = p1 = p0 + kf.InTangent;
                keyframeLines[i] = new TVertexLine(p0, p1);
                ++i;

                keyframePositions[i] = p0 = kf.OutValue;
                tangentPositions[i] = p1 = p0 + kf.OutTangent;
                keyframeLines[i] = new TVertexLine(p0, p1);
                ++i;
            }
            //Fill the rest in case of non-matching keyframe counts
            while (i < kfCount)
            {
                keyframePositions[i] = p0 = Vec3.Zero;
                tangentPositions[i] = p1 = Vec3.Zero;
                keyframeLines[i] = new TVertexLine(p0, p1);
                ++i;
            }

            VertexLineStrip strip = new VertexLineStrip(false, splinePoints);

            spline.GetMinMax(false,
                out (float Time, float Value)[] min, 
                out (float Time, float Value)[] max);

            Vec3[] extrema = new Vec3[6];
            for (int x = 0, index = 0; x < 3; ++x)
            {
                var (TimeMin, ValueMin) = min[x];
                Vec3 minPos = spline.GetValue(TimeMin);
                minPos[x] = ValueMin;
                
                var (TimeMax, ValueMax) = max[x];
                Vec3 maxPos = spline.GetValue(TimeMax);
                maxPos[x] = ValueMax;
                
                extrema[index++] = minPos;
                extrema[index++] = maxPos;
            }

            TMath.ComponentMinMax(out Vec3 minVal, out Vec3 maxVal, extrema);
            var box = BoundingBox.FromMinMax(minVal, maxVal);
            _cullingVolumeTranslation = box.Translation;
            RenderInfo.CullingVolume = box.GetVolume() == 0 ? null : box;

            RenderingParameters p = new RenderingParameters
            {
                LineWidth = 1.0f,
                PointSize = 5.0f
            };

            TMesh splineData = TMesh.Create(VertexShaderDesc.PosColor(), strip);
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
            _splinePrimitive = new MeshRenderer(splineData, mat);

            TMesh velocityData = TMesh.Create(VertexShaderDesc.JustPositions(), velocity);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Blue);
            mat.RenderParams = p;
            _velocityTangentsPrimitive = new MeshRenderer(velocityData, mat);

            TMesh pointData = TMesh.Create(keyframePositions);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Green);
            mat.RenderParams = p;
            _pointPrimitive = new MeshRenderer(pointData, mat);

            TMesh extremaData = TMesh.Create(extrema);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Red);
            mat.RenderParams = p;
            _extremaPrimitive = new MeshRenderer(extremaData, mat);

            TMesh tangentData = TMesh.Create(tangentPositions);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Purple);
            mat.RenderParams = p;
            _tangentPrimitive = new MeshRenderer(tangentData, mat);

            TMesh kfLineData = TMesh.Create(VertexShaderDesc.JustPositions(), keyframeLines);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.Orange);
            mat.RenderParams = p;
            _keyframeLinesPrimitive = new MeshRenderer(kfLineData, mat);

            TMesh timePointData = TMesh.Create(Vec3.Zero);
            mat = TMaterial.CreateUnlitColorMaterialForward(Color.White);
            mat.RenderParams = p;
            _timePointPrimitive = new MeshRenderer(timePointData, mat);

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
            if (_spline.IsLoaded)
            {
                var spline = Spline;
                var pos = spline.CurrentPosition;
                splinePosMtx = pos.AsTranslationMatrix();
                invSplinePosMtx = pos.AsInverseTranslationMatrix();
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
        //protected override void DeriveMatrix()
        //{
        //    _localTRS.DeriveTRS(out Vec3 t, out Vec3 s, out Quat r);
        //    _translation.SetValueSilent(t);
        //    _scale.SetValueSilent(s);
        //    _rotation.SetRotationsNoUpdate(r.ToRotator());
        //}
        protected override void OnWorldTransformChanged(bool recalcChildWorldTransformsNow = true)
        {
            Matrix4 mtx = ParentWorldMatrix * _localTRS;
            _rcKfLines.WorldMatrix = mtx;
            _rcSpline.WorldMatrix = mtx;
            _rcVelocityTangents.WorldMatrix = mtx;
            _rcPoints.WorldMatrix = mtx;
            _rcKeyframeTangents.WorldMatrix = mtx;
            _rcExtrema.WorldMatrix = mtx;
            _rcCurrentPoint.WorldMatrix = WorldMatrix;

            RenderInfo.CullingVolume?.SetTransformMatrix(mtx * _cullingVolumeTranslation.AsTranslationMatrix());

            base.OnWorldTransformChanged(recalcChildWorldTransformsNow);
        }
        
        private readonly RenderCommandMesh3D _rcKfLines = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcCurrentPoint = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcSpline = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcVelocityTangents = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcPoints = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcKeyframeTangents = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        private readonly RenderCommandMesh3D _rcExtrema = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            if (_spline is null)
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
                RenderInfo.CullingVolume?.AddRenderables(passes, camera);

            if (RenderCurrentTimePoint)
                passes.Add(_rcCurrentPoint);
        }
    }
}