using System.Drawing;
using TheraEngine.Rendering.Models;
using System.Collections.Generic;
using System;
using TheraEngine.Core.Maths.Transforms;
using System.ComponentModel;
using TheraEngine.Physics;
using TheraEngine.Shapes;

namespace TheraEngine.Core.Shapes
{
    public class Cone : TShape
    {
        public Cone(EventVec3 center, Vec3 upAxis, float radius, float height)
        {
            _center = center ?? Vec3.Zero;
            _radius = Math.Abs(radius);
            _height = Math.Abs(height);
            _localUpAxis = upAxis;
            _localUpAxis.Normalize();
        }

        protected EventVec3 _center;
        protected Vec3 _localUpAxis = Vec3.Up;
        protected float _radius = 0.5f, _height = 1.0f;
        
        public Vec3 GetTopPoint()
            => _center + _localUpAxis * (_height / 2.0f);
        public Vec3 GetBottomCenterPoint()
            => _center + _localUpAxis * (_height / -2.0f);
        public Circle3D GetBottomCircle(bool normalFacingIn = false)
            => new Circle3D(_radius, GetBottomCenterPoint(), normalFacingIn ? UpAxis : -UpAxis);
        
        [Category("Cone")]
        public EventVec3 Center
        {
            get => _center;
            set => _center = value ?? Vec3.Zero;
        }
        [Category("Cone")]
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        [Category("Cone")]
        public float Height
        {
            get => _height;
            set => _height = value;
        }
        [Category("Cone")]
        public virtual Vec3 UpAxis
        {
            get => _localUpAxis;
            set => _localUpAxis = value;
        }
        
        public override void Render(bool shadowPass)
            => Engine.Renderer.RenderCone(_center.AsTranslationMatrix(), _localUpAxis, _radius, _height, RenderSolid, Color.Magenta);

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
        public override EContainment Contains(BoundingBoxStruct box)
        {
            throw new NotImplementedException();
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
        public override EContainment Contains(Capsule capsule)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Cylinder cylinder)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Cone cone)
        {
            throw new NotImplementedException();
        }
        public override Vec3 ClosestPoint(Vec3 point)
        {
            throw new NotImplementedException();
        }
        public override BoundingBox GetAABB()
        {
            throw new NotImplementedException();
        }
        public override Matrix4 GetTransformMatrix()
        {
            return Center.AsTranslationMatrix();
        }
        public override void SetTransformMatrix(Matrix4 matrix)
        {
            Center.Raw = matrix.Translation;
        }

        public override TCollisionShape GetCollisionShape() 
            => throw new InvalidOperationException(
                "A cone with an arbitrary up axis cannot be used as a collision shape. " +
                $"Use {nameof(ConeX)}, {nameof(ConeY)}, or {nameof(ConeZ)} instead.");

        public override TShape HardCopy() 
            => new Cone(Center.Raw, UpAxis, Radius, Height);
    }
}
