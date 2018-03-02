using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Animation;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Components.Scene
{
    public class SplineComponent : TRSComponent, I3DRenderable
    {
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass3D.OpaqueForward, null, false);

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
                _splinePrimitive = new PrimitiveManager(splineData, TMaterial.CreateUnlitColorMaterialForward(Color.Red));

                PrimitiveData velocityData = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), velocity);
                _velocityPrimitive = new PrimitiveManager(velocityData, TMaterial.CreateUnlitColorMaterialForward(Color.Blue));
                
                PrimitiveData pointData = PrimitiveData.FromPoints(keyframePositions);
                _pointPrimitive = new PrimitiveManager(pointData, TMaterial.CreateUnlitColorMaterialForward(Color.Green));

                PrimitiveData tangentData = PrimitiveData.FromPoints(tangentPositions);
                _tangentPrimitive = new PrimitiveManager(tangentData, TMaterial.CreateUnlitColorMaterialForward(Color.Purple));
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

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (IsSpawned)
            {
                if (selected)
                    OwningScene.Add(this);
                else
                    OwningScene.Remove(this);
            }
            base.OnSelectedChanged(selected);
        }
#endif
    }
}