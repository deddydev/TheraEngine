﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Maths;
using System.Xml;
using System.IO;
using System;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Core.Shapes
{
    /// <summary>
    /// Contains the points and planes at the edges and near/far of a camera's view.
    /// </summary>
    [FileClass("FRUSTUM", "Camera View Frustum")]
    public class Frustum : I3DRenderable, IEnumerable<Plane>
    {
        private RenderInfo3D _renderInfo = new RenderInfo3D(ERenderPass3D.OpaqueForward, null, false);
        public RenderInfo3D RenderInfo => _renderInfo;

        [TSerialize("Points")]
        private Vec3[] _points = new Vec3[8];
        private Plane[] _planes = new Plane[6];

        //For quickly testing if objects in large scenes should even be tested against the frustum at all
        [TSerialize("UseBoundingSphere", XmlNodeType = EXmlNodeType.Attribute)]
        private Sphere _boundingSphere;

        [CustomXMLSerializeMethod("UseBoundingSphere")]
        private void SerializeBoundingSphere(XmlWriter writer)
        {
            writer.WriteAttributeString("UseBoundingSphere", UseBoundingSphere.ToString());
        }
        [CustomXMLDeserializeMethod("BoundingSphere")]
        private void DeserializeBoundingSphere(XMLReader reader)
        {
            if (bool.TryParse(reader.Value, out bool result))
                _boundingSphere = result ? new Sphere() : null;
            else
                _boundingSphere = null;
        }

        public Frustum()
        {
            _boundingSphere = new Sphere() { RenderSolid = false };
        }
        public Frustum(
            float fovY,
            float aspect,
            float nearZ,
            float farZ,
            Vec3 forward,
            Vec3 up,
            Vec3 position)
            : this()
        {
            float
                tan = (float)Math.Tan(CustomMath.DegToRad(fovY / 2.0f)),
                nearYDist = tan * nearZ,
                nearXDist = aspect * nearYDist,
                farYDist = tan * farZ,
                farXDist = aspect * farYDist;

            Vec3
                rightDir = forward ^ up,
                nearPos = position + forward * nearZ,
                farPos = position + forward * farZ,
                nX = rightDir * nearXDist,
                fX = rightDir * farXDist,
                nY = up * nearYDist,
                fY = up * farYDist,
                ntl = nearPos + nY - nX,
                ntr = nearPos + nY + nX,
                nbl = nearPos - nY - nX,
                nbr = nearPos - nY + nX,
                ftl = farPos + fY - fX,
                ftr = farPos + fY + fX,
                fbl = farPos - fY - fX,
                fbr = farPos - fY + fX;
            
            UpdatePoints(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        public Frustum(
           Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
           Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight) : this()
        {
            UpdatePoints(
                farBottomLeft, farBottomRight, farTopLeft, farTopRight,
                nearBottomLeft, nearBottomRight, nearTopLeft, nearTopRight);
        }
        private Frustum(
           Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
           Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight,
           Vec3 sphereCenter, float sphereRadius) : this()
        {
            UpdatePoints(
                farBottomLeft, farBottomRight, farTopLeft, farTopRight,
                nearBottomLeft, nearBottomRight, nearTopLeft, nearTopRight, sphereCenter, sphereRadius);
        }

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

        [PostDeserialize]
        private void PostDeserialize()
        {
            UpdatePoints(FarBottomLeft, FarBottomRight, FarTopLeft, FarTopRight, NearBottomLeft, NearBottomRight, NearTopLeft, NearTopRight);
        }

        [Browsable(false)]
        public Vec3 FarBottomLeft => _points[0];
        [Browsable(false)]
        public Vec3 FarBottomRight => _points[1];

        [Browsable(false)]
        public Vec3 FarTopLeft => _points[2];
        [Browsable(false)]
        public Vec3 FarTopRight => _points[3];

        [Browsable(false)]
        public Vec3 NearBottomLeft => _points[4];
        [Browsable(false)]
        public Vec3 NearBottomRight => _points[5];

        [Browsable(false)]
        public Vec3 NearTopLeft => _points[6];
        [Browsable(false)]
        public Vec3 NearTopRight => _points[7];

        [Browsable(false)]
        public Plane Near => _planes[0];
        [Browsable(false)]
        public Plane Far => _planes[1];

        [Browsable(false)]
        public Plane Left => _planes[2];
        [Browsable(false)]
        public Plane Right => _planes[3];

        [Browsable(false)]
        public Plane Top => _planes[4];
        [Browsable(false)]
        public Plane Bottom => _planes[5];

        [Browsable(false)]
        public IEnumerable<Vec3> Points => _points;
        [Browsable(false)]
        public Shape CullingVolume => _boundingSphere;

        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        [Browsable(false)]
        public Sphere BoundingSphere
        {
            get => _boundingSphere;
            private set => _boundingSphere = value;
        }
        [Browsable(false)]
        public Plane[] Planes => _planes;

        [Browsable(false)]
        public ERenderPass3D RenderPass => ERenderPass3D.OpaqueForward;
        [Browsable(false)]
        public float RenderOrder => 0.0f;

        public bool UseBoundingSphere
        {
            get => _boundingSphere != null;
            set
            {
                if (value == UseBoundingSphere)
                    return;

                if (value)
                {
                    _boundingSphere = new Sphere();
                    CalculateBoundingSphere();
                }
                else
                    _boundingSphere = null;
            }
        }

        private void CalculateBoundingSphere()
        {
            if (UseBoundingSphere)
            {
                Miniball miniball = new Miniball(PointSetArray.FromVectors(_points));
                _boundingSphere.Center = new Vec3(miniball.Center[0], miniball.Center[1], miniball.Center[2]);
                _boundingSphere.Radius = miniball.Radius;
            }
        }
        private void UpdateBoundingSphere(Vec3 center, float radius)
        {
            if (UseBoundingSphere)
            {
                _boundingSphere.Center = center;
                _boundingSphere.Radius = radius;
            }
        }

        private void UpdatePoints(
            Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
            Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight)
        {
            _points[0] = farBottomLeft;
            _points[1] = farBottomRight;
            _points[2] = farTopLeft;
            _points[3] = farTopRight;
            _points[4] = nearBottomLeft;
            _points[5] = nearBottomRight;
            _points[6] = nearTopLeft;
            _points[7] = nearTopRight;

            //near, far
            _planes[0] = new Plane(nearBottomRight, nearBottomLeft, nearTopRight);
            _planes[1] = new Plane(farBottomLeft, farBottomRight, farTopLeft);

            //left, right
            _planes[2] = new Plane(nearBottomLeft, farBottomLeft, nearTopLeft);
            _planes[3] = new Plane(farBottomRight, nearBottomRight, farTopRight);

            //top, bottom
            _planes[4] = new Plane(farTopLeft, farTopRight, nearTopLeft);
            _planes[5] = new Plane(nearBottomLeft, nearBottomRight, farBottomLeft);

            CalculateBoundingSphere();
        }
        private void UpdatePoints(
            Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
            Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight,
            Vec3 sphereCenter, float sphereRadius)
        {
            _points[0] = farBottomLeft;
            _points[1] = farBottomRight;
            _points[2] = farTopLeft;
            _points[3] = farTopRight;
            _points[4] = nearBottomLeft;
            _points[5] = nearBottomRight;
            _points[6] = nearTopLeft;
            _points[7] = nearTopRight;

            //near, far
            _planes[0] = new Plane(nearBottomRight, nearBottomLeft, nearTopRight);
            _planes[1] = new Plane(farBottomLeft, farBottomRight, farTopLeft);

            //left, right
            _planes[2] = new Plane(nearBottomLeft, farBottomLeft, nearTopLeft);
            _planes[3] = new Plane(farBottomRight, nearBottomRight, farTopRight);

            //top, bottom
            _planes[4] = new Plane(farTopLeft, farTopRight, nearTopLeft);
            _planes[5] = new Plane(nearBottomLeft, nearBottomRight, farBottomLeft);

            UpdateBoundingSphere(sphereCenter, sphereRadius);
        }

        public bool IntersectsRay(Vec3 startPoint, Vec3 direction, out List<Vec3> points)
        {
            Ray r = new Ray(startPoint, direction);
            points = new List<Vec3>();
            foreach (Plane p in this)
                if (Collision.RayIntersectsPlane(r.StartPoint, r.Direction, p.IntersectionPoint, p.Normal, out Vec3 point))
                    points.Add(point);
            return points.Count > 0;
        }

        public void TransformedVersionOf(Frustum other, Matrix4 transform)
        {
            if (_boundingSphere != null)
            {
                _boundingSphere.Center = other._boundingSphere.Center * transform;
                _boundingSphere.Radius = other._boundingSphere.Radius;
            }
            for (int i = 0; i < 8; ++i)
                _points[i] = other._points[i] * transform;
            for (int i = 0; i < 6; ++i)
                _planes[i] = other._planes[i].TransformedBy(transform);
        }

        public void TransformBy(Matrix4 transform)
        {
            if (_boundingSphere != null)
                _boundingSphere.Center = _boundingSphere.Center * transform;
            for (int i = 0; i < 8; ++i)
                _points[i] = _points[i] * transform;
            for (int i = 0; i < 6; ++i)
                _planes[i].TransformBy(transform);
        }

        public Frustum TransformedBy(Matrix4 transform)
        {
            Frustum f = new Frustum();
            if (_boundingSphere != null)
                f._boundingSphere = new Sphere(_boundingSphere.Radius, _boundingSphere.Center * transform);
            for (int i = 0; i < 8; ++i)
                f._points[i] = _points[i] * transform;
            for (int i = 0; i < 6; ++i)
                f._planes[i] = _planes[i].TransformedBy(transform);
            return f;
        }
        
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
                if (Collision.RayIntersectsPlane(bot.Center, top.Center, p.IntersectionPoint, p.Normal, out Vec3 point))
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
                    if (Segment.GetShortestDistanceToPoint(bottomLeft, bottomRight, point) < radius)
                        return EContainment.Intersects;
                    if (Segment.GetShortestDistanceToPoint(bottomRight, topRight, point) < radius)
                        return EContainment.Intersects;
                    if (Segment.GetShortestDistanceToPoint(topRight, topLeft, point) < radius)
                        return EContainment.Intersects;
                    if (Segment.GetShortestDistanceToPoint(topLeft, bottomLeft, point) < radius)
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

            Engine.Renderer.RenderLine(NearTopLeft, FarTopLeft, SideColor, LineSize);
            Engine.Renderer.RenderLine(NearTopRight, FarTopRight, SideColor, LineSize);
            Engine.Renderer.RenderLine(NearBottomLeft, FarBottomLeft, SideColor, LineSize);
            Engine.Renderer.RenderLine(NearBottomRight, FarBottomRight, SideColor, LineSize);

            Engine.Renderer.RenderLine(NearTopLeft, NearTopRight, NearColor, LineSize);
            Engine.Renderer.RenderLine(NearBottomLeft, NearBottomRight, NearColor, LineSize);
            Engine.Renderer.RenderLine(NearBottomLeft, NearTopLeft, NearColor, LineSize);
            Engine.Renderer.RenderLine(NearBottomRight, NearTopRight, NearColor, LineSize);

            Engine.Renderer.RenderLine(FarTopLeft, FarTopRight, FarColor, LineSize);
            Engine.Renderer.RenderLine(FarBottomLeft, FarBottomRight, FarColor, LineSize);
            Engine.Renderer.RenderLine(FarBottomLeft, FarTopLeft, FarColor, LineSize);
            Engine.Renderer.RenderLine(FarBottomRight, FarTopRight, FarColor, LineSize);
        }

        public Frustum HardCopy()
            => new Frustum(
                FarBottomLeft, FarBottomRight, FarTopLeft, FarTopRight,
                NearBottomLeft, NearBottomRight, NearTopLeft, NearTopRight);
    }
}
