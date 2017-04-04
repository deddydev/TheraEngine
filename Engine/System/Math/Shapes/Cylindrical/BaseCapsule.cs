using CustomEngine;
using CustomEngine.Rendering.Models;

namespace System
{
    public abstract class BaseCapsule : BaseCylinder
    {
        public BaseCapsule(Vec3 center, Vec3 upAxis, float radius, float halfHeight) 
            : base(center, upAxis, radius, halfHeight) { }
        public Sphere GetTopSphere()
            => new Sphere(Radius, GetTopCenterPoint());
        public Sphere GetBottomSphere()
            => new Sphere(Radius, GetBottomCenterPoint());
        public override void Render()
        {
            Engine.Renderer.RenderCapsule(
                GetTopCenterPoint(),
                GetBottomCenterPoint(),
                _radius, _radius, false);
        }
        public override bool Contains(Vec3 point)
            => Segment.GetClosestDistanceToPoint(GetBottomCenterPoint(), GetTopCenterPoint(), point) <= _radius;
        public Vec3 ClosestPointTo(Vec3 point)
        {
            Vec3 startPoint = GetBottomCenterPoint();
            Vec3 endPoint = GetTopCenterPoint();
            Segment.Part part = Segment.GetDistancePointToSegmentPart(startPoint, endPoint, point, out float closestPartDist);
            switch (part)
            {
                case Segment.Part.StartPoint:
                    return Ray.PointAtLineDistance(startPoint, point, _radius);
                case Segment.Part.EndPoint:
                    return Ray.PointAtLineDistance(endPoint, point, _radius);
                case Segment.Part.Line:
                    return Ray.GetPerpendicularVectorFromPoint(startPoint, endPoint - startPoint, point);
            }
            throw new Exception();
        }
        public Vec3 ClosestPointTo(Sphere sphere)
            => ClosestPointTo(sphere.Center);
        public override EContainment Contains(Sphere sphere)
        {
            Vec3 startPoint = GetBottomCenterPoint();
            Vec3 endPoint = GetTopCenterPoint();
            float pointToSegment = Segment.GetClosestDistanceToPoint(startPoint, endPoint, sphere.Center);
            float maxDist = sphere.Radius + Radius;
            if (pointToSegment > maxDist)
                return EContainment.Disjoint;
            else if (pointToSegment + sphere.Radius < Radius)
                return EContainment.Contains;
            else
                return EContainment.Intersects;
        }
        public override EContainment Contains(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(BoundingBox box)
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
        public override EContainment ContainedWithin(Frustum frustum)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public static PrimitiveData WireframeMesh(Vec3 center, Vec3 upAxis, float radius, float halfHeight, int pointCountHalfCircle)
        {
            upAxis.NormalizeFast();

            Vec3 topPoint = center + upAxis * halfHeight;
            Vec3 bottomPoint = center - upAxis * halfHeight;

            Vec3 forwardNormal, rightNormal;
            if (upAxis == Vec3.Right)
            {
                forwardNormal = Vec3.Forward;
                rightNormal = Vec3.Up;
            }
            else if (upAxis == -Vec3.Right)
            {
                forwardNormal = Vec3.Forward;
                rightNormal = -Vec3.Up;
            }
            else if (upAxis == Vec3.Forward)
            {
                forwardNormal = Vec3.Up;
                rightNormal = Vec3.Right;
            }
            else if (upAxis == -Vec3.Forward)
            {
                forwardNormal = -Vec3.Up;
                rightNormal = Vec3.Right;
            }
            else
            {
                forwardNormal = Vec3.Right ^ upAxis;
                rightNormal = Vec3.Forward ^ upAxis;
            }

            VertexLineStrip topCircleUp = Circle.LineStrip(radius, upAxis, topPoint, pointCountHalfCircle * 2);
            VertexLineStrip topHalfCircleToward = Circle.HalfCircleLineStrip(radius, upAxis, rightNormal, topPoint, pointCountHalfCircle);
            VertexLineStrip topHalfCircleRight = Circle.HalfCircleLineStrip(radius, rightNormal, topPoint, pointCountHalfCircle);
            
            VertexLineStrip bottomCircleDown = Circle.LineStrip(radius, -upAxis, topPoint, pointCountHalfCircle * 2);
            VertexLineStrip bottomHalfCircleAway = Circle.HalfCircleLineStrip(radius, -forwardNormal, topPoint, pointCountHalfCircle);
            VertexLineStrip bottomHalfCircleRight = Circle.HalfCircleLineStrip(radius, rightNormal, topPoint, pointCountHalfCircle);
            
            VertexLineStrip d3 = Circle.LineStrip(radius, Vec3.Right, center, pointCountHalfCircle * 2);
            return PrimitiveData.FromLineStrips(new PrimitiveBufferInfo() { _texcoordCount = 0, _hasNormals = false }, d1, d2, d3);
        }
    }
}
