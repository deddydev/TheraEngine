using System.Drawing;
using TheraEngine.Rendering.Models;
using System.Collections.Generic;
using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Shapes
{
    public abstract class BaseCone : Shape
    {
        public override Shape CullingVolume => null;

        public BaseCone(Vec3 center, Rotator rotation, Vec3 scale, Vec3 upAxis, float radius, float height)
        {
            _radius = Math.Abs(radius);
            _height = Math.Abs(height);

            _localUpAxis = upAxis;
            _localUpAxis.Normalize();

            _state.Translation.SetRawNoUpdate(center);
            _state.Rotation.SetRotationsNoUpdate(rotation);
            _state.Scale.SetRawNoUpdate(scale);
            _state.CreateTransform();
        }

        public Transform State
        {
            get => _state;
            set => _state = value;
        }

        protected Transform _state = Transform.GetIdentity(TransformOrder.TRS, RotationOrder.YPR);
        
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
            get => _state.Translation.Raw;
            set => _state.Translation.Raw = value;
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
            => Engine.Renderer.RenderCone(_state.Matrix, _localUpAxis, _radius, _height, _renderSolid, Color.Magenta);

        public static PrimitiveData WireMesh(Vec3 center, Vec3 up, float height, float radius, int sides)
        {
            up.Normalize();

            VertexLine[] lines = new VertexLine[sides * 3];

            Vec3 topPoint = center + (up * (height / 2.0f));
            Vec3 bottomPoint = center - (up * (height / 2.0f));

            Vertex[] sidePoints = Circle3D.Points(radius, up, bottomPoint, sides);

            for (int i = 0, x = 0; i < sides; ++i)
            {
                Vertex sidePoint = sidePoints[i];
                lines[x++] = new VertexLine(bottomPoint, sidePoint.Position);
                lines[x++] = new VertexLine(topPoint, sidePoint.Position);
                lines[x++] = new VertexLine(sidePoints[i + 1 == sides ? 0 : i + 1], sidePoint);
            }

            return PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), lines);
        }
        public static PrimitiveData SolidMesh(Vec3 center, Vec3 up, float height, float radius, int sides, bool closeBottom)
        {
            up.Normalize();

            List<VertexTriangle> tris = new List<VertexTriangle>((sides * 3) * (closeBottom ? 2 : 1));
            
            Vec3 topPoint = center + (up * (height / 2.0f));
            Vec3 bottomPoint = center - (up * (height / 2.0f));

            Vertex[] sidePoints = Circle3D.Points(radius, up, bottomPoint, sides);

            Vec3 diff, normal;
            Vertex topVertex;

            for (int i = 0; i < sides; ++i)
            {
                diff = topPoint - sidePoints[i].Position;
                diff.Normalize();
                normal = diff ^ (up ^ diff).Normalized();
                sidePoints[i].Normal = normal;

                topVertex = new Vertex(topPoint, up, new Vec2(0.5f));
                tris.Add(new VertexTriangle(sidePoints[i + 1 == sides ? 0 : i + 1], sidePoints[i], topVertex));
                if (tris.Count - 2 >= 0)
                {
                    VertexTriangle lastTri = tris[tris.Count - 2];
                    lastTri.Vertex0.Normal += normal;
                    lastTri.Vertex0.Normal.Normalize();
                }
            }

            if (closeBottom)
            {
                List<Vertex> list = new List<Vertex>(sidePoints.Length + 1)
                {
                    new Vertex(bottomPoint, -up, new Vec2(0.5f))
                };
                for (int i = 0; i < sidePoints.Length; ++i)
                {
                    Vertex v2 = sidePoints[i].HardCopy();
                    v2.Normal = -up;
                    list.Add(v2);
                }
                Vertex v3 = sidePoints[0].HardCopy();
                v3.Normal = -up;
                list.Add(v3);
                tris.AddRange(new VertexTriangleFan(list).ToTriangles());
            }

            return PrimitiveData.FromTriangleList(VertexShaderDesc.PosNormTex(), tris);
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
        public override EContainment Contains(BaseCapsule capsule)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(BaseCylinder cylinder)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(BaseCone cone)
        {
            throw new NotImplementedException();
        }
        public override Shape HardCopy()
        {
            return this;
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            throw new NotImplementedException();
        }
        public override Matrix4 GetTransformMatrix()
        {
            return State.Matrix;
        }
        public override Vec3 ClosestPoint(Vec3 point)
        {
            throw new NotImplementedException();
        }
        public override BoundingBox GetAABB()
        {
            throw new NotImplementedException();
        }
    }
}
