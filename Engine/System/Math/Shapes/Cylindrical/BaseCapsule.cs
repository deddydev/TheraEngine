using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using CustomEngine;

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
    }
}
