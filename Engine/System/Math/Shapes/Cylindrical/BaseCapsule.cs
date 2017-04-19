using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Drawing;
using static System.Math;

namespace System
{
    public abstract class BaseCapsule : BaseCylinder
    {
        public BaseCapsule(Vec3 center, Rotator rotation, Vec3 scale, Vec3 upAxis, float radius, float halfHeight) 
            : base(center, rotation, scale, upAxis, radius, halfHeight) { }

        public Sphere GetTopSphere()
            => new Sphere(Radius, GetTopCenterPoint());
        public Sphere GetBottomSphere()
            => new Sphere(Radius, GetBottomCenterPoint());

        public override void Render()
        {
            Engine.Renderer.RenderCapsule(
                ShapeName, _state.Matrix, _localUpAxis, _radius, _halfHeight, _renderSolid, Color.Red);
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
            Vec3 top = GetTopCenterPoint();
            Vec3 bot = GetBottomCenterPoint();
            Vec3 radiusVec = new Vec3(_radius);
            Vec3 capsuleMin = Vec3.ComponentMin(top, bot) - radiusVec;
            Vec3 capsuleMax = Vec3.ComponentMax(top, bot) + radiusVec;
            Vec3 min = box.Minimum;
            Vec3 max = box.Maximum;

            bool containsX = false, containsY = false, containsZ = false;
            bool disjointX = false, disjointY = false, disjointZ = false;
            
            containsX = capsuleMin.X >= min.X && capsuleMax.X <= max.X;
            containsY = capsuleMin.Y >= min.Y && capsuleMax.Y <= max.Y;
            containsZ = capsuleMin.Z >= min.Z && capsuleMax.Z <= max.Z;
            if (!containsX) disjointX = capsuleMax.X < min.X || capsuleMin.X > max.X;
            if (!containsY) disjointY = capsuleMax.Y < min.Y || capsuleMin.Y > max.Y;
            if (!containsZ) disjointZ = capsuleMax.Z < min.Z || capsuleMin.Z > max.Z;
            if (containsX && containsY && containsZ)
                return EContainment.Contains;
            if (disjointX && disjointY && disjointZ)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public override EContainment ContainedWithin(Box box)
        {
            return ContainedWithin(box.AsFrustum());
        }
        public override EContainment ContainedWithin(Frustum frustum)
        {
            if (frustum == null)
                return EContainment.Disjoint;
            return frustum.Contains(this);
        }
        public override EContainment ContainedWithin(Sphere sphere)
        {
            Vec3 top = GetTopCenterPoint();
            Vec3 bot = GetBottomCenterPoint();
            float topDist = top.DistanceToFast(sphere.Center);
            float botDist = bot.DistanceToFast(sphere.Center);
            float distToCenter = Segment.GetClosestDistanceToPoint(bot, top, sphere.Center);
            bool containsTop = topDist + _radius < sphere.Radius;
            bool containsBot = botDist + _radius < sphere.Radius;
            bool containsSides = distToCenter + _radius < sphere.Radius;
            if (containsTop != containsBot)
                return EContainment.Intersects;
            if (containsBot && containsTop)
                return containsSides ? EContainment.Contains : EContainment.Intersects;
            else
                return containsSides ? EContainment.Intersects : EContainment.Disjoint;
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

            int pts = pointCountHalfCircle + 1;
            Vertex[] topPoints1 = new Vertex[pts], topPoints2 = new Vertex[pts];
            Vertex[] botPoints1 = new Vertex[pts], botPoints2 = new Vertex[pts];

            float angleInc = CustomMath.PIf / pointCountHalfCircle;
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

            VertexLineStrip topCircleUp = Circle.LineStrip(radius, upAxis, topPoint, pointCountHalfCircle * 2);
            VertexLineStrip topHalfCircleToward = new VertexLineStrip(false, topPoints1);
            VertexLineStrip topHalfCircleRight = new VertexLineStrip(false, topPoints2);

            VertexLineStrip bottomCircleDown = Circle.LineStrip(radius, -upAxis, bottomPoint, pointCountHalfCircle * 2);
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

            return PrimitiveData.FromLineStrips(new PrimitiveBufferInfo() { _texcoordCount = 0, _hasNormals = false },
                topCircleUp, topHalfCircleToward, topHalfCircleRight,
                bottomCircleDown, bottomHalfCircleAway, bottomHalfCircleRight,
                right, left, front, back);
        }
        public static void WireframeMeshParts(
            Vec3 center, Vec3 upAxis, float radius, float halfHeight, int pointCountHalfCircle,
            out PrimitiveData cylinder, out PrimitiveData topSphereHalf, out PrimitiveData bottomSphereHalf)
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

            int pts = pointCountHalfCircle + 1;

            Vertex[] topPoints1 = new Vertex[pts], topPoints2 = new Vertex[pts];
            Vertex[] botPoints1 = new Vertex[pts], botPoints2 = new Vertex[pts];

            Quat offset = Quat.BetweenVectors(Vec3.Up, upAxis);

            float angleInc = CustomMath.PIf / pointCountHalfCircle;
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

            VertexLineStrip topCircleUp = Circle.LineStrip(radius, upAxis, topPoint, pointCountHalfCircle * 2);
            VertexLineStrip topHalfCircleToward = new VertexLineStrip(false, topPoints1);
            VertexLineStrip topHalfCircleRight = new VertexLineStrip(false, topPoints2);

            VertexLineStrip bottomCircleDown = Circle.LineStrip(radius, -upAxis, bottomPoint, pointCountHalfCircle * 2);
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

            cylinder = PrimitiveData.FromLineStrips(new PrimitiveBufferInfo() { _texcoordCount = 0, _hasNormals = false },
                topCircleUp, bottomCircleDown, right, left, front, back);
            topSphereHalf = PrimitiveData.FromLineStrips(new PrimitiveBufferInfo() { _texcoordCount = 0, _hasNormals = false },
                topHalfCircleToward, topHalfCircleRight);
            bottomSphereHalf = PrimitiveData.FromLineStrips(new PrimitiveBufferInfo() { _texcoordCount = 0, _hasNormals = false },
                bottomHalfCircleAway, bottomHalfCircleRight);
        }
    }
}
