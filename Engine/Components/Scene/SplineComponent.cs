using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Animation;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene
{
    public class SplineComponent : TRSComponent, I3DRenderable
    {
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass.OpaqueForward, false, true) { CastsShadows = false, ReceivesShadows = false };

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

        private PropAnimVec3 _spline;

        private PrimitiveManager _splinePrimitive;
        private PrimitiveManager _velocityPrimitive;
        private PrimitiveManager _pointPrimitive;
        private PrimitiveManager _tangentPrimitive;

        public PropAnimVec3 Spline
        {
            get => _spline;
            set
            {
                _spline = value;
                if (_spline != null)
                    _spline.Keyframes.Changed += RegenerateSplinePrimitive;
                RegenerateSplinePrimitive();
            }
        }

        public SplineComponent() : base()
        {
            Spline = null;
        }
        public SplineComponent(PropAnimVec3 spline) : base()
        {
            Spline = spline;
        }

        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            //RenderNode?.ItemMoved(this);
        }

        public void RegenerateSplinePrimitive()
        {
            //List<Vertex> vertices = new List<Vertex>();
            //Vec3 point, velocity;
            //int max = _spline.FrameCount - 1;
            //for (float i = 0.0f, step = 0.0f; i < max; i = (i + step).ClampMax(max))
            //{
            //    point = _spline.GetValueKeyframed(i);
            //    velocity = _spline.GetVelocityKeyframed(i);
            //    step = (velocity.LengthFast / 2.0f).ClampMin(0.1f);
            //    vertices.Add(new Vertex(point));
            //}
            //VertexLineStrip strip = new VertexLineStrip(false, vertices.ToArray());

            if (_spline == null)
                _splinePrimitive = null;
            else
            {
                Vertex[] splinePoints = new Vertex[_spline.BakedFrameCount];
                VertexLine[] velocity = new VertexLine[_spline.BakedFrameCount];
                Vec3[] keyframePositions = new Vec3[_spline.Keyframes.Count << 1];
                Vec3[] tangentPositions = new Vec3[_spline.Keyframes.Count << 1];

                int i, x = 0;
                for (i = 0; i < splinePoints.Length; ++i)
                {
                    Vertex pos = new Vertex(_spline.GetValueKeyframed(i));
                    splinePoints[i] = pos;
                    velocity[i] = new VertexLine(pos, new Vertex(pos.Position + _spline.GetVelocityKeyframed(i).NormalizedFast()));
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
            }
        }

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            
            base.OnSelectedChanged(selected);
        }

        private RenderCommandMesh3D _rcSpline = new RenderCommandMesh3D();
        private RenderCommandMesh3D _rcVelocity = new RenderCommandMesh3D();
        private RenderCommandMesh3D _rcPoints = new RenderCommandMesh3D();
        private RenderCommandMesh3D _rcTangents = new RenderCommandMesh3D();
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            _rcSpline.Mesh = _splinePrimitive;
            _rcSpline.WorldMatrix = WorldMatrix;
            _rcSpline.NormalMatrix = Matrix3.Identity;
            passes.Add(_rcSpline, RenderInfo.RenderPass);

            if (RenderTangents)
            {
                _rcTangents.Mesh = _velocityPrimitive;
                _rcTangents.WorldMatrix = WorldMatrix;
                _rcTangents.NormalMatrix = Matrix3.Identity;
                passes.Add(_rcTangents, RenderInfo.RenderPass);
            }
            if (RenderKeyframePoints)
            {
                _rcTangents.Mesh = _pointPrimitive;
                _rcTangents.WorldMatrix = WorldMatrix;
                _rcTangents.NormalMatrix = Matrix3.Identity;
                passes.Add(_rcTangents, RenderInfo.RenderPass);
            }
            if (RenderKeyframeTangentPoints)
            {
                _rcTangents.Mesh = _tangentPrimitive;
                _rcTangents.WorldMatrix = WorldMatrix;
                _rcTangents.NormalMatrix = Matrix3.Identity;
                passes.Add(_rcTangents, RenderInfo.RenderPass);
            }
        }
#endif
    }
}