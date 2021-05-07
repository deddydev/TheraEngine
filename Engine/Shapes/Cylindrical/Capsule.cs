using TheraEngine.Rendering.Models;
using System.Drawing;
using static System.Math;
using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths;

namespace TheraEngine.Core.Shapes
{
    [TFileExt("capsule")]
    [TFileDef("Capsule")]
    public abstract class Capsule : Cylinder
    {
        public Capsule(Vec3 upAxis, float radius, float halfHeight)
            : this(Vec3.Zero, upAxis, radius, halfHeight) { }
        public Capsule(EventVec3 center, Vec3 upAxis, float radius, float halfHeight) 
            : base(center, upAxis, radius, halfHeight) { }

        public Sphere GetTopSphere()
            => new Sphere(Radius, GetTopCenterPoint());
        public Sphere GetBottomSphere()
            => new Sphere(Radius, GetBottomCenterPoint());

        public override void Render(bool shadowPass)
        {
            Engine.Renderer.RenderCapsule(_center.AsTranslationMatrix(), _upAxis, _radius, _halfHeight, RenderSolid, Color.Red);
        }

        /// <summary>
        /// Returns the closest point on this shape to the given point.
        /// </summary>
        /// <param name="point">The point determine closeness with.</param>
        public override Vec3 ClosestPoint(Vec3 point)
            => ClosestPoint(point, false);
        /// <summary>
        /// Returns the closest point on this shape to the given point.
        /// </summary>
        /// <param name="point">The point determine closeness with.</param>
        /// <param name="clampIfInside">If true, finds closest edge point even if the given point is inside the capsule. Otherwise, just returns the given point if it is inside.</param>
        public Vec3 ClosestPoint(Vec3 point, bool clampIfInside)
        {
            Vec3 colinearPoint = Segment.ClosestColinearPointToPoint(GetBottomCenterPoint(), GetTopCenterPoint(), point);
            if (!clampIfInside && colinearPoint.DistanceToFast(point) < _radius)
                return point;
            return Ray.PointAtLineDistance(colinearPoint, point, _radius);
        }

        public override BoundingBox GetAABB()
        {
            Vec3 top = GetTopCenterPoint();
            Vec3 bot = GetBottomCenterPoint();
            float radius = Radius;
            Vec3 min = Vec3.ComponentMin(top, bot) - radius;
            Vec3 max = Vec3.ComponentMax(top, bot) + radius;
            return BoundingBox.FromMinMax(min, max);
        }

        #region Containment
        public override bool Contains(Vec3 point)
            => Segment.ShortestDistanceToPoint(GetBottomCenterPoint(), GetTopCenterPoint(), point) <= _radius;
        public Vec3 ClosestPointTo(Vec3 point)
        {
            Vec3 startPoint = GetBottomCenterPoint();
            Vec3 endPoint = GetTopCenterPoint();
            Segment.ESegmentPart part = Segment.GetDistancePointToSegmentPart(startPoint, endPoint, point, out float closestPartDist);
            switch (part)
            {
                case Segment.ESegmentPart.StartPoint:
                    return Ray.PointAtLineDistance(startPoint, point, _radius);
                case Segment.ESegmentPart.EndPoint:
                    return Ray.PointAtLineDistance(endPoint, point, _radius);
                case Segment.ESegmentPart.Line:
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
            float pointToSegment = Segment.ShortestDistanceToPoint(startPoint, endPoint, sphere.Center);
            float maxDist = sphere.Radius + Radius;
            if (pointToSegment > maxDist)
                return EContainment.Disjoint;
            else if (pointToSegment + sphere.Radius < Radius)
                return EContainment.Contains;
            else
                return EContainment.Intersects;
        }
        public override EContainment Contains(Capsule capsule)
        {
            //TODO
            return EContainment.Contains;
        }
        public override EContainment Contains(Cone cone)
        {
            //TODO
            return EContainment.Contains;
        }
        public override EContainment Contains(Cylinder cylinder)
        {
            //TODO
            return EContainment.Contains;
        }
        public override EContainment Contains(Box box)
        {
            //TODO
            return EContainment.Contains;
        }
        public override EContainment Contains(BoundingBox box)
        {
            //TODO
            return EContainment.Contains;
        }
        #endregion

        #region Mesh
        public static TMesh WireframeMesh(Vec3 center, Vec3 upAxis, float radius, float halfHeight, int pointCountHalfCircle)
        {
            upAxis.Normalize();

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

            int pts = pointCountHalfCircle + 1;
            Vertex[] topPoints1 = new Vertex[pts], topPoints2 = new Vertex[pts];
            Vertex[] botPoints1 = new Vertex[pts], botPoints2 = new Vertex[pts];

            float angleInc = TMath.PIf / pointCountHalfCircle;
            float angle = 0.0f;
            for (int i = 0; i < pts; ++i, angle += angleInc)
            {
                Vec3 v1 = new Vec3((float)Cos(angle), (float)Sin(angle), 0.0f);
                Vec3 v2 = new Vec3(0.0f, (float)Sin(angle), (float)Cos(angle));
                topPoints1[i] = new Vertex(topPoint + radius * v1);
                topPoints2[i] = new Vertex(topPoint + radius * v2);
                botPoints1[i] = new Vertex(bottomPoint - radius * v1);
                botPoints2[i] = new Vertex(bottomPoint - radius * v2);
            }

            VertexLineStrip topCircleUp = Circle3D.LineStrip(radius, upAxis, topPoint, pointCountHalfCircle * 2);
            VertexLineStrip topHalfCircleToward = new VertexLineStrip(false, topPoints1);
            VertexLineStrip topHalfCircleRight = new VertexLineStrip(false, topPoints2);

            VertexLineStrip bottomCircleDown = Circle3D.LineStrip(radius, -upAxis, bottomPoint, pointCountHalfCircle * 2);
            VertexLineStrip bottomHalfCircleAway = new VertexLineStrip(false, botPoints1);
            VertexLineStrip bottomHalfCircleRight = new VertexLineStrip(false, botPoints2);

            VertexLineStrip right = new VertexLineStrip(false, 
                new Vertex(bottomPoint + rightNormal * radius),
                new Vertex(topPoint + rightNormal * radius));
            VertexLineStrip left = new VertexLineStrip(false,
                new Vertex(bottomPoint - rightNormal * radius),
                new Vertex(topPoint - rightNormal * radius));
            VertexLineStrip front = new VertexLineStrip(false,
                new Vertex(bottomPoint + forwardNormal * radius),
                new Vertex(topPoint + forwardNormal * radius));
            VertexLineStrip back = new VertexLineStrip(false,
                new Vertex(bottomPoint - forwardNormal * radius),
                new Vertex(topPoint - forwardNormal * radius));

            return TMesh.Create(VertexShaderDesc.JustPositions(),
                topCircleUp, topHalfCircleToward, topHalfCircleRight,
                bottomCircleDown, bottomHalfCircleAway, bottomHalfCircleRight,
                right, left, front, back);
        }
        public static void WireframeMeshParts(
            Vec3 center, Vec3 upAxis, float radius, float halfHeight, int pointCountHalfCircle,
            out TMesh cylinder, out TMesh topSphereHalf, out TMesh bottomSphereHalf)
        {
            upAxis.Normalize();

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

            int pts = pointCountHalfCircle + 1;

            Vertex[] topPoints1 = new Vertex[pts], topPoints2 = new Vertex[pts];
            Vertex[] botPoints1 = new Vertex[pts], botPoints2 = new Vertex[pts];

            Quat offset = Quat.BetweenVectors(Vec3.Up, upAxis);

            float angleInc = TMath.PIf / pointCountHalfCircle;
            float angle = 0.0f;
            for (int i = 0; i < pts; ++i, angle += angleInc)
            {
                Vec3 v1 = new Vec3((float)Cos(angle), (float)Sin(angle), 0.0f);
                Vec3 v2 = new Vec3(0.0f, (float)Sin(angle), (float)Cos(angle));
                topPoints1[i] = new Vertex(offset * (radius * v1));
                topPoints2[i] = new Vertex(offset * (radius * v2));
                botPoints1[i] = new Vertex(-(offset * (radius * v1)));
                botPoints2[i] = new Vertex(-(offset * (radius * v2)));
            }

            VertexLineStrip topCircleUp = Circle3D.LineStrip(radius, upAxis, topPoint, pointCountHalfCircle * 2);
            VertexLineStrip topHalfCircleToward = new VertexLineStrip(false, topPoints1);
            VertexLineStrip topHalfCircleRight = new VertexLineStrip(false, topPoints2);

            VertexLineStrip bottomCircleDown = Circle3D.LineStrip(radius, -upAxis, bottomPoint, pointCountHalfCircle * 2);
            VertexLineStrip bottomHalfCircleAway = new VertexLineStrip(false, botPoints1);
            VertexLineStrip bottomHalfCircleRight = new VertexLineStrip(false, botPoints2);

            VertexLineStrip right = new VertexLineStrip(false,
                new Vertex(bottomPoint + rightNormal * radius),
                new Vertex(topPoint + rightNormal * radius));
            VertexLineStrip left = new VertexLineStrip(false,
                new Vertex(bottomPoint - rightNormal * radius),
                new Vertex(topPoint - rightNormal * radius));
            VertexLineStrip front = new VertexLineStrip(false,
                new Vertex(bottomPoint + forwardNormal * radius),
                new Vertex(topPoint + forwardNormal * radius));
            VertexLineStrip back = new VertexLineStrip(false,
                new Vertex(bottomPoint - forwardNormal * radius),
                new Vertex(topPoint - forwardNormal * radius));

            cylinder = TMesh.Create(VertexShaderDesc.JustPositions(),
                topCircleUp, bottomCircleDown, right, left, front, back);
            topSphereHalf = TMesh.Create(VertexShaderDesc.JustPositions(),
                topHalfCircleToward, topHalfCircleRight);
            bottomSphereHalf = TMesh.Create(VertexShaderDesc.JustPositions(),
                bottomHalfCircleAway, bottomHalfCircleRight);
        }
        #endregion
    }
}
