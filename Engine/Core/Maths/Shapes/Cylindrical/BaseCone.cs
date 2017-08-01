using TheraEngine;
using System.Drawing;
using System.ComponentModel;
using TheraEngine.Rendering.Models;
using System.Collections.Generic;

namespace System
{
    [FileClass("SHAPE", "Cone")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class BaseCone : Shape
    {
        public BaseCone(Vec3 center, Rotator rotation, Vec3 scale, Vec3 upAxis, float radius, float height)
        {
            _radius = Math.Abs(radius);
            _height = Math.Abs(height);
            _localUpAxis = upAxis;
            _localUpAxis.NormalizeFast();
            _state.Translation = center;
            _state.Rotation = rotation;
            _state.Scale = scale;
        }

        public FrameState State
        {
            get => _state;
            set => _state = value;
        }

        protected FrameState _state = FrameState.GetIdentity(TransformOrder.TRS, RotationOrder.YPR);
        
        protected Vec3 _localUpAxis = Vec3.Up;
        protected float _radius = 0.5f, _height = 1.0f;
        
        public Vec3 GetTopPoint()
            => Vec3.TransformPosition(_localUpAxis * (_height / 2.0f), _state.Matrix);
        public Vec3 GetBottomCenterPoint()
            => Vec3.TransformPosition(_localUpAxis * (_height / -2.0f), _state.Matrix);
        public Circle3D GetBottomCircle(bool normalFacingIn = false)
            => new Circle3D(_radius, GetBottomCenterPoint(), normalFacingIn ? WorldUpAxis : -WorldUpAxis);
        
        public Vec3 Center
        {
            get => _state.Translation;
            set => _state.Translation = value;
        }
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        public float Height
        {
            get => _height;
            set => _height = value;
        }

        public Vec3 LocalUpAxis => _localUpAxis;
        public Vec3 WorldUpAxis => _localUpAxis * _state.Matrix.GetRotationMatrix4();

        public override void SetRenderTransform(Matrix4 worldMatrix)
        {
            _state.Matrix = worldMatrix;
            base.SetRenderTransform(worldMatrix);
        }

        public override void Render()
            => Engine.Renderer.RenderCone(_state.Matrix, _localUpAxis, _radius, _height, _renderSolid, Color.Black);

        public static PrimitiveData SolidMesh(Vec3 center, Vec3 up, float height, float radius, int sides, bool closeBottom)
        {
            up.NormalizeFast();

            List<VertexTriangle> tris = new List<VertexTriangle>((sides * 3) * (closeBottom ? 2 : 1));
            
            Vec3 topPoint = center + (up * (height / 2.0f));
            Vec3 bottomPoint = center - (up * (height / 2.0f));

            Vertex[] sidePoints = Circle3D.Points(radius, up, bottomPoint, sides);

            for (int i = 0; i < sides; ++i)
            {
                Vec3 diff = topPoint - sidePoints[i]._position;
                diff.NormalizeFast();
                Vec3 normal = diff ^ (up ^ diff);
                sidePoints[i]._normal = normal;

                Vertex topVertex = new Vertex(topPoint, normal, new Vec2(0.5f));
                tris.Add(new VertexTriangle(sidePoints[i + 1 == sides ? 0 : i + 1], sidePoints[i], topVertex));
            }

            if (closeBottom)
            {
                List<Vertex> list = new List<Vertex>(sidePoints.Length + 1) { new Vertex(topPoint, -up, new Vec2(0.5f)) };
                foreach (Vertex v in sidePoints)
                {
                    Vertex v2 = v.HardCopy();
                    v2._normal = -up;
                    list.Add(v2);
                }
                tris.AddRange(new VertexTriangleFan(list).ToTriangles());
            }

            return PrimitiveData.FromTriangleList(Culling.Back, VertexShaderDesc.PosNormTex(), tris);
        }

        public override bool Contains(Vec3 point)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Frustum frustum)
        {
            throw new NotImplementedException();
        }
        public override Shape HardCopy()
        {
            throw new NotImplementedException();
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            throw new NotImplementedException();
        }
        public override Matrix4 GetTransformMatrix()
        {
            throw new NotImplementedException();
        }
        public override Vec3 ClosestPoint(Vec3 point)
        {
            throw new NotImplementedException();
        }
    }
}
