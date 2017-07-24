using TheraEngine.Animation;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Drawing;
using System.ComponentModel;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors
{
    public class SplineComponent : TRSComponent, I3DRenderable
    {
        private RenderInfo3D _renderInfo = new RenderInfo3D(RenderPassType3D.OpaqueForward, null, false);
        public RenderInfo3D RenderInfo => _renderInfo;
        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        private PropAnimVec3 _spline;

        private PrimitiveManager _splinePrimitive;
        private PrimitiveManager _velocityPrimitive;
        private PrimitiveManager _pointPrimitive;
        private PrimitiveManager _tangentPrimitive;
        private bool
            _renderTangents = false, 
            _renderKeyframeTangentPoints = true, 
            _renderKeyframePoints = true;

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
        
        public bool RenderTangents
        {
            get => _renderTangents;
            set => _renderTangents = value;
        }
        public bool RenderKeyframeTangentPoints
        {
            get => _renderKeyframeTangentPoints;
            set => _renderKeyframeTangentPoints = value;
        }
        public bool RenderKeyframePoints
        {
            get => _renderKeyframePoints;
            set => _renderKeyframePoints = value;
        }

        public SplineComponent() : base()
        {
            Spline = null;
        }
        public SplineComponent(PropAnimVec3 spline) : base()
        {
            Spline = spline;
        }

        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            //RenderNode?.ItemMoved(this);
        }
        public override void OnSpawned()
        {
            if (Engine.Settings.RenderSplines)
                Engine.Scene.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (Engine.Settings.RenderSplines)
                Engine.Scene.Remove(this);
            base.OnDespawned();
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
                Vertex[] splinePoints = new Vertex[_spline.FrameCount];
                VertexLine[] velocity = new VertexLine[_spline.FrameCount];
                Vec3[] keyframePositions = new Vec3[_spline.Keyframes.KeyCount << 1];
                Vec3[] tangentPositions = new Vec3[_spline.Keyframes.KeyCount << 1];

                int i, x = 0;
                for (i = 0; i < splinePoints.Length; ++i)
                {
                    Vertex pos = new Vertex(_spline.GetValueKeyframed(i));
                    splinePoints[i] = pos;
                    velocity[i] = new VertexLine(pos, new Vertex(pos._position + _spline.GetVelocityKeyframed(i).NormalizedFast()));
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

                PrimitiveData splineData = PrimitiveData.FromLineStrips(VertexShaderDesc.JustPositions(), strip);
                _splinePrimitive = new PrimitiveManager(splineData, Material.GetUnlitColorMaterialForward(Color.Red));

                PrimitiveData velocityData = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), velocity);
                _velocityPrimitive = new PrimitiveManager(velocityData, Material.GetUnlitColorMaterialForward(Color.Blue));
                
                PrimitiveData pointData = PrimitiveData.FromPoints(keyframePositions);
                _pointPrimitive = new PrimitiveManager(pointData, Material.GetUnlitColorMaterialForward(Color.Green));

                PrimitiveData tangentData = PrimitiveData.FromPoints(tangentPositions);
                _tangentPrimitive = new PrimitiveManager(tangentData, Material.GetUnlitColorMaterialForward(Color.Purple));
            }
        }
        public void Render()
        {
            Engine.Renderer.SetLineSize(5.0f);
            Engine.Renderer.SetPointSize(10.0f);

            _splinePrimitive?.Render(WorldMatrix, Matrix3.Identity);
            if (_renderTangents)
                _velocityPrimitive?.Render(WorldMatrix, Matrix3.Identity);
            
            if (_renderKeyframePoints)
                _pointPrimitive?.Render(WorldMatrix, Matrix3.Identity);
            if (_renderKeyframeTangentPoints)
                _tangentPrimitive?.Render(WorldMatrix, Matrix3.Identity);
        }
    }
}