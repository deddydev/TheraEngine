using CustomEngine;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering;

namespace System
{
    public class Frustum : IRenderable, IEnumerable<Plane>
    {
        private static List<Frustum> ActiveFrustums = new List<Frustum>();
        private string _renderName;

        public Frustum()
        {
            _boundingSphere = new Sphere() { RenderSolid = false };
            _renderName = "frustum" + ActiveFrustums.Count;
            ActiveFrustums.Add(this);
        }
        public Frustum(
           Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
           Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight,
           Vec3 sphereCenter) : this()
        {
            UpdatePoints(
                farBottomLeft, farBottomRight, farTopLeft, farTopRight,
                nearBottomLeft, nearBottomRight, nearTopLeft, nearTopRight,
                sphereCenter);
        }
        ~Frustum()
        {
            ActiveFrustums.Remove(this);
        }

        public static bool UseBoundingSphere = true;

        private bool _isRendering = false;

        private IOctreeNode _renderNode;

        //For quickly testing if objects in large scenes should even be tested against the frustum at all
        private Sphere _boundingSphere;

        public void GetCornerPoints(int planeIndex, out Vec3 bottomLeft, out Vec3 bottomRight, out Vec3 topRight, out Vec3 topLeft)
        {
            if (planeIndex < 0 || planeIndex > 6)
                throw new IndexOutOfRangeException();
            switch (planeIndex)
            {
                case 0: //Near
                    bottomLeft = NearBottomLeft;
                    bottomRight = NearBottomRight;
                    topRight = NearTopRight;
                    topLeft = NearTopLeft;
                    break;
                case 1: //Far
                    bottomLeft = FarBottomLeft;
                    bottomRight = FarBottomRight;
                    topRight = FarTopRight;
                    topLeft = FarTopLeft;
                    break;
                case 2: //Left
                    bottomLeft = FarBottomLeft;
                    bottomRight = NearBottomLeft;
                    topRight = NearTopLeft;
                    topLeft = FarTopLeft;
                    break;
                case 3: //Right
                    bottomLeft = NearBottomRight;
                    bottomRight = FarBottomRight;
                    topRight = FarTopRight;
                    topLeft = NearTopRight;
                    break;
                case 4: //Top
                    bottomLeft = NearTopLeft;
                    bottomRight = NearTopRight;
                    topRight = FarTopRight;
                    topLeft = FarTopLeft;
                    break;
                case 5: //Bottom
                    bottomLeft = NearBottomRight;
                    bottomRight = NearBottomLeft;
                    topRight = FarBottomLeft;
                    topLeft = FarBottomRight;
                    break;
                default:
                    bottomLeft = Vec3.Zero;
                    bottomRight = Vec3.Zero;
                    topRight = Vec3.Zero;
                    topLeft = Vec3.Zero;
                    break;
            }
        }

        private Plane[] _planes = new Plane[6];
        private Vec3[] _points = new Vec3[8];

        public Vec3 FarBottomLeft => _points[0];
        public Vec3 FarBottomRight => _points[1];

        public Vec3 FarTopLeft => _points[2];
        public Vec3 FarTopRight => _points[3];

        public Vec3 NearBottomLeft => _points[4];
        public Vec3 NearBottomRight => _points[5];

        public Vec3 NearTopLeft => _points[6];
        public Vec3 NearTopRight => _points[7];

        public Plane Near => _planes[0];
        public Plane Far => _planes[1];

        public Plane Left => _planes[2];
        public Plane Right => _planes[3];

        public Plane Top => _planes[4];
        public Plane Bottom => _planes[5];

        public IEnumerable<Vec3> Points => _points;
        public Shape CullingVolume => _boundingSphere;

        public bool IsRendering
        {
            get => _isRendering;
            set => _isRendering = value;
        }
        public IOctreeNode RenderNode
        {
            get => _renderNode;
            set => _renderNode = value;
        }
        public Sphere BoundingSphere
        {
            get => _boundingSphere;
            private set => _boundingSphere = value;
        }
        public Plane[] Planes
        {
            get => _planes;
        }

        public void UpdatePoints(
            Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
            Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight, 
            Vec3 sphereCenter)
        {
            _points[0] = farBottomLeft;
            _points[1] = farBottomRight;
            _points[2] = farTopLeft;
            _points[3] = farTopRight;
            _points[4] = nearBottomLeft;
            _points[5] = nearBottomRight;
            _points[6] = nearTopLeft;
            _points[7] = nearTopRight;

            _boundingSphere.Center = sphereCenter;
            _boundingSphere.Radius = sphereCenter.DistanceToFast(farTopRight);

            //near, far
            _planes[0] = new Plane(nearBottomRight, nearBottomLeft, nearTopRight);
            _planes[1] = new Plane(farBottomLeft, farBottomRight, farTopLeft);

            //left, right
            _planes[2] = new Plane(nearBottomLeft, farBottomLeft, nearTopLeft);
            _planes[3] = new Plane(farBottomRight, nearBottomRight, farTopRight);

            //top, bottom
            _planes[4] = new Plane(farTopLeft, farTopRight, nearTopLeft);
            _planes[5] = new Plane(nearBottomLeft, nearBottomRight, farBottomLeft);
        }

        public bool IntersectsRay(Vec3 startPoint, Vec3 direction, out List<Vec3> points)
        {
            Ray r = new Ray(startPoint, direction);
            points = new List<Vec3>();
            foreach (Plane p in this)
                if (Collision.RayIntersectsPlane(r.StartPoint, r.Direction, p.Point, p.Normal, out Vec3 point))
                    points.Add(point);
            return points.Count > 0;
        }
        public void TransformedVersionOf(Frustum other, Matrix4 transform)
            => UpdatePoints(
                other.FarBottomLeft * transform,
                other.FarBottomRight * transform,
                other.FarTopLeft * transform,
                other.FarTopRight * transform,
                other.NearBottomLeft * transform,
                other.NearBottomRight * transform,
                other.NearTopLeft * transform,
                other.NearTopRight * transform,
                other._boundingSphere.Center * transform);

        public void TransformBy(Matrix4 transform)
            => UpdatePoints(
                FarBottomLeft * transform,
                FarBottomRight * transform,
                FarTopLeft * transform,
                FarTopRight * transform,
                NearBottomLeft * transform,
                NearBottomRight * transform,
                NearTopLeft * transform,
                NearTopRight * transform,
                _boundingSphere.Center * transform);

        public Frustum TransformedBy(Matrix4 transform)
            => new Frustum(
                FarBottomLeft * transform,
                FarBottomRight * transform,
                FarTopLeft * transform,
                FarTopRight * transform,
                NearBottomLeft * transform,
                NearBottomRight * transform,
                NearTopLeft * transform,
                NearTopRight * transform,
                _boundingSphere.Center * transform);
        
        public EContainment Contains(Box box) 
            => Collision.FrustumContainsBox1(this, box.HalfExtents, box.WorldMatrix);
        public EContainment Contains(BoundingBox box) 
            => Collision.FrustumContainsAABB(this, box.Minimum, box.Maximum);
        public EContainment Contains(Sphere sphere) 
            => Collision.FrustumContainsSphere(this, sphere.Center, sphere.Radius);
        public EContainment Contains(BaseCapsule capsule)
        {
            if (capsule.ContainedWithin(BoundingSphere) == EContainment.Disjoint)
                return EContainment.Disjoint;

            Sphere top = capsule.GetTopSphere();
            Sphere bot = capsule.GetBottomSphere();
            EContainment ct = Contains(top);
            EContainment cb = Contains(bot);

            if (ct == EContainment.Contains && cb == EContainment.Contains)
                return EContainment.Contains;
            
            if (ct == EContainment.Intersects || cb == EContainment.Intersects || cb != ct)
                return EContainment.Intersects;

            //Both spheres are disjoint, but the cylinder might be intersecting
            float radius = capsule.Radius;
            for (int i = 0; i < 6; ++i)
            {
                Plane p = Planes[i];
                GetCornerPoints(i, out Vec3 bottomLeft, out Vec3 bottomRight, out Vec3 topRight, out Vec3 topLeft);
                //Intersect the capsule's inner segment with the plane as a ray
                if (Collision.RayIntersectsPlane(bot.Center, top.Center, p.Point, p.Normal, out Vec3 point))
                {
                    //Make sure the resulting point is even within the range of the capsule
                    Segment.Part part = Segment.GetDistancePointToSegmentPart(bot.Center, top.Center, point, out float closestPartDist);
                    if (closestPartDist > radius)
                        continue;

                    switch (part)
                    {
                        case Segment.Part.Line:
                            Vec3 perp = Ray.GetPerpendicularVectorFromPoint(bot.Center, top.Center - bot.Center, point);

                            break;
                        case Segment.Part.StartPoint:
                        case Segment.Part.EndPoint:
                            Vec3 partPoint = part == Segment.Part.StartPoint ? bot.Center : top.Center;

                            break;
                    }

                    //First test for direct intersections with the plane, within its bounds
                    if (point.IsInTriangle(bottomLeft, bottomRight, topRight) &&
                        point.IsInTriangle(bottomLeft, topLeft, topRight))
                    {

                        return EContainment.Intersects;
                    }

                    //Now test distances to each plane edge
                    if (Segment.GetClosestDistanceToPoint(bottomLeft, bottomRight, point) < radius)
                        return EContainment.Intersects;
                    if (Segment.GetClosestDistanceToPoint(bottomRight, topRight, point) < radius)
                        return EContainment.Intersects;
                    if (Segment.GetClosestDistanceToPoint(topRight, topLeft, point) < radius)
                        return EContainment.Intersects;
                    if (Segment.GetClosestDistanceToPoint(topLeft, bottomLeft, point) < radius)
                        return EContainment.Intersects;
                }
                else
                {
                    //Segment is parallel to plane, can use any point for distance
                    float closestDist = p.DistanceTo(bot.Center);
                    if (closestDist < radius)
                    {
                        //Close enough to intersect, make sure within plane bounds
                        Vec3 normal = -p.Normal * closestDist;
                        Vec3 projectedEndPoint = top.Center + normal;
                        Vec3 projectedStartPoint = bot.Center + normal;

                        //TODO: test if any part of the segment is within plane bounds

                        return EContainment.Intersects;
                    }
                }
            }
            return EContainment.Disjoint;
        }
        public IEnumerator<Plane> GetEnumerator() => ((IEnumerable<Plane>)_planes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Plane>)_planes).GetEnumerator();

        public void Render()
        {
            _boundingSphere.Render();

            float LineSize = 10.0f;
            Color NearColor = Color.Orange;
            Color FarColor = Color.DarkRed;
            Color SideColor = Color.LightGreen;

            Engine.Renderer.RenderLine(_renderName + "TopLeft", NearTopLeft, FarTopLeft, SideColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "TopRight", NearTopRight, FarTopRight, SideColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "BottomLeft", NearBottomLeft, FarBottomLeft, SideColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "BottomRight", NearBottomRight, FarBottomRight, SideColor, LineSize);

            Engine.Renderer.RenderLine(_renderName + "TopNear", NearTopLeft, NearTopRight, NearColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "BottomNear", NearBottomLeft, NearBottomRight, NearColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "LeftNear", NearBottomLeft, NearTopLeft, NearColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "RightNear", NearBottomRight, NearTopRight, NearColor, LineSize);

            Engine.Renderer.RenderLine(_renderName + "TopFar", FarTopLeft, FarTopRight, FarColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "BottomFar", FarBottomLeft, FarBottomRight, FarColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "LeftFar", FarBottomLeft, FarTopLeft, FarColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "RightFar", FarBottomRight, FarTopRight, FarColor, LineSize);
        }

        public Frustum HardCopy()
            => new Frustum(
                FarBottomLeft, FarBottomRight, FarTopLeft, FarTopRight,
                NearBottomLeft, NearBottomRight, NearTopLeft, NearTopRight,
                _boundingSphere.Center);
    }
}
