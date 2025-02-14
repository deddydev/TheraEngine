﻿using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Maths;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Shapes;

namespace TheraEngine.Core.Shapes
{
    public enum EFrustumPlane
    {
        Near,
        Far,
        Left,
        Right,
        Top,
        Bottom
    }
    public interface IVolume
    {
        EContainment Contains(Box box);
        EContainment Contains(BoundingBox box);
        EContainment Contains(BoundingBoxStruct box);
        EContainment Contains(Sphere sphere);
        EContainment Contains(TShape shape);
        EContainment Contains(Cone cone);
        //Vec3 ClosestPoint(Vec3 point);
        bool Contains(Vec3 point);
    }
    public interface IFrustum : I3DRenderable, IEnumerable<Plane>, IVolume
    {
        IFrustum[] GetSubFrustums(int slices);
        IFrustum GetSubFrustum(int slices, int index);
        void SetSubFrustumOf(IFrustum parentFrustum, int slices, int index);
        void SetSubFrustums(IFrustum[] slices, bool useEdgeDistance = false);
        void GetCornerPoints(EFrustumPlane planeIndex, out Vec3 bottomLeft, out Vec3 bottomRight, out Vec3 topRight, out Vec3 topLeft);

        Vec3 FarBottomLeft { get; }
        Vec3 FarBottomRight { get; }
        Vec3 FarTopLeft { get; }
        Vec3 FarTopRight { get; }
        Vec3 NearBottomLeft { get; }
        Vec3 NearBottomRight { get; }
        Vec3 NearTopLeft { get; }
        Vec3 NearTopRight { get; }
        Plane Near { get; }
        Plane Far { get; }

        Plane Left { get; }
        Plane Right { get; }
        Plane Top { get; }
        Plane Bottom { get; }

        Sphere BoundingSphere { get; set; }
        Plane[] Planes { get; }
        Vec3[] Points { get; }

        bool UseBoundingSphere { get;  set; }
        IScene3D OwningScene3D { get; set; }

        bool IntersectsRay(Vec3 startPoint, Vec3 direction, out IList<Vec3> points);
        void TransformedVersionOf(IFrustum other, Matrix4 transform);
        void TransformBy(Matrix4 transform);
        IFrustum TransformedBy(Matrix4 transform);
        //bool Contains(Vec3 point);
        //EContainment Contains(TShape shape);
        //EContainment Contains(Box box);
        //EContainment Contains(BoundingBoxStruct box);
        //EContainment Contains(BoundingBox box);
        //EContainment Contains(Sphere sphere);
        //EContainment Contains(Cylinder cylinder);
        //EContainment Contains(Cone cone);
        EContainment Contains(Capsule capsule);
        void Render(bool shadowPass);
        void Render(bool shadowPass, Color NearColor, Color FarColor, Color SideColor, float LineSize, bool RenderSphere);
        IFrustum HardCopy();
        void UpdatePoints(
             Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
             Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight);
    }
    /// <summary>
    /// Contains the points and planes at the edges and near/far of a camera's view.
    /// </summary>
    public class Frustum : TObjectSlim, IFrustum
    {
        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D(false, true);

        [TSerialize("Points")]
        private Vec3[] _points = new Vec3[8];

        //For quickly testing if objects in large scenes should even be tested against the frustum at all
        [TSerialize("UseBoundingSphere", NodeType = ENodeType.Attribute)]
        private Sphere _boundingSphere;

        [CustomMemberSerializeMethod("UseBoundingSphere")]
        private object SerializeBoundingSphere() => UseBoundingSphere;

        [CustomMemberDeserializeMethod("BoundingSphere")]
        private void DeserializeBoundingSphere(object value)
            => _boundingSphere = value is bool result && result ? new Sphere() : null;

        public Frustum()
        {
            _boundingSphere = new Sphere() { RenderSolid = false };
            _renderCommand = new RenderCommandMethod3D(ERenderPass.OpaqueForward, Render);
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
                tan = (float)Math.Tan(TMath.DegToRad(fovY / 2.0f)),
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
        
        public IFrustum[] GetSubFrustums(int slices)
        {
            Segment topLeft = new Segment(NearTopLeft, FarTopLeft);
            Segment topRight = new Segment(NearTopRight, FarTopRight);
            Segment bottomLeft = new Segment(NearBottomLeft, FarBottomLeft);
            Segment bottomRight = new Segment(NearBottomRight, FarBottomRight);
            float totalDist = topLeft.Length;
            float sliceDist = totalDist / slices;
            Frustum[] sliceFrustums = new Frustum[slices];
            for (int i = 0; i < slices; ++i)
            {
                float nearDist = i * sliceDist;
                float farDist = nearDist + sliceDist;
                Vec3 nearBottomLeft = bottomLeft.PointAtLineDistance(nearDist);
                Vec3 nearBottomRight = bottomRight.PointAtLineDistance(nearDist);
                Vec3 nearTopLeft = topLeft.PointAtLineDistance(nearDist);
                Vec3 nearTopRight = topRight.PointAtLineDistance(nearDist);
                Vec3 farBottomLeft = bottomLeft.PointAtLineDistance(farDist);
                Vec3 farBottomRight = bottomRight.PointAtLineDistance(farDist);
                Vec3 farTopLeft = topLeft.PointAtLineDistance(farDist);
                Vec3 farTopRight = topRight.PointAtLineDistance(farDist);
                sliceFrustums[i] = new Frustum(farBottomLeft, farBottomRight, farTopLeft, farTopRight, nearBottomLeft, nearBottomRight, nearTopLeft, nearTopRight);
            }
            return sliceFrustums;
        }
        public IFrustum GetSubFrustum(int slices, int index)
        {
            Segment topLeft         = new Segment(NearTopLeft, FarTopLeft);
            Segment topRight        = new Segment(NearTopRight, FarTopRight);
            Segment bottomLeft      = new Segment(NearBottomLeft, FarBottomLeft);
            Segment bottomRight     = new Segment(NearBottomRight, FarBottomRight);
            float totalDist         = topLeft.Length;
            float sliceDist         = totalDist / slices;
            float nearDist          = index * sliceDist;
            float farDist           = nearDist + sliceDist;
            Vec3 nearBottomLeft     = bottomLeft.PointAtLineDistance(nearDist);
            Vec3 nearBottomRight    = bottomRight.PointAtLineDistance(nearDist);
            Vec3 nearTopLeft        = topLeft.PointAtLineDistance(nearDist);
            Vec3 nearTopRight       = topRight.PointAtLineDistance(nearDist);
            Vec3 farBottomLeft      = bottomLeft.PointAtLineDistance(farDist);
            Vec3 farBottomRight     = bottomRight.PointAtLineDistance(farDist);
            Vec3 farTopLeft         = topLeft.PointAtLineDistance(farDist);
            Vec3 farTopRight        = topRight.PointAtLineDistance(farDist);
            return new Frustum(farBottomLeft, farBottomRight, farTopLeft, farTopRight, nearBottomLeft, nearBottomRight, nearTopLeft, nearTopRight);
        }
        public void SetSubFrustumOf(IFrustum parentFrustum, int slices, int index)
        {
            Segment topLeft = new Segment(NearTopLeft, FarTopLeft);
            Segment topRight = new Segment(NearTopRight, FarTopRight);
            Segment bottomLeft = new Segment(NearBottomLeft, FarBottomLeft);
            Segment bottomRight = new Segment(NearBottomRight, FarBottomRight);
            float totalDist = topLeft.Length;
            float sliceDist = totalDist / slices;
            float nearDist = index * sliceDist;
            float farDist = nearDist + sliceDist;
            Vec3 nearBottomLeft = bottomLeft.PointAtLineDistance(nearDist);
            Vec3 nearBottomRight = bottomRight.PointAtLineDistance(nearDist);
            Vec3 nearTopLeft = topLeft.PointAtLineDistance(nearDist);
            Vec3 nearTopRight = topRight.PointAtLineDistance(nearDist);
            Vec3 farBottomLeft = bottomLeft.PointAtLineDistance(farDist);
            Vec3 farBottomRight = bottomRight.PointAtLineDistance(farDist);
            Vec3 farTopLeft = topLeft.PointAtLineDistance(farDist);
            Vec3 farTopRight = topRight.PointAtLineDistance(farDist);
            UpdatePoints(farBottomLeft, farBottomRight, farTopLeft, farTopRight, nearBottomLeft, nearBottomRight, nearTopLeft, nearTopRight);
        }
        public void SetSubFrustums(IFrustum[] slices, bool useEdgeDistance = false)
        {
            Segment topLeft = new Segment(NearTopLeft, FarTopLeft);
            Segment topRight = new Segment(NearTopRight, FarTopRight);
            Segment bottomLeft = new Segment(NearBottomLeft, FarBottomLeft);
            Segment bottomRight = new Segment(NearBottomRight, FarBottomRight);
            float totalDist = useEdgeDistance ? topLeft.Length : Far.DistanceTo(NearTopLeft);
            float sliceDist = totalDist / slices.Length;
            for (int i = 0; i < slices.Length; ++i)
            {
                float nearDist = i * sliceDist;
                float farDist = nearDist + sliceDist;
                Vec3 nearBottomLeft, nearBottomRight, nearTopLeft, nearTopRight, farBottomLeft, farBottomRight, farTopLeft, farTopRight;
                if (useEdgeDistance)
                {
                    nearBottomLeft  = bottomLeft.PointAtLineDistance(nearDist);
                    nearBottomRight = bottomRight.PointAtLineDistance(nearDist);
                    nearTopLeft     = topLeft.PointAtLineDistance(nearDist);
                    nearTopRight    = topRight.PointAtLineDistance(nearDist);
                    farBottomLeft   = bottomLeft.PointAtLineDistance(farDist);
                    farBottomRight  = bottomRight.PointAtLineDistance(farDist);
                    farTopLeft      = topLeft.PointAtLineDistance(farDist);
                    farTopRight     = topRight.PointAtLineDistance(farDist);
                }
                else
                {
                    Vec3 nearPlanePoint = NearTopLeft + Near.Normal * nearDist;
                    Vec3 farPlanePoint  = NearTopLeft + Near.Normal * farDist;
                    Vec3 bottomLeftDir  = bottomLeft.DirectionVector.Normalized();
                    Vec3 bottomRightDir = bottomRight.DirectionVector.Normalized();
                    Vec3 topLeftDir     = topLeft.DirectionVector.Normalized();
                    Vec3 topRightDir    = topRight.DirectionVector.Normalized();
                    Collision.RayIntersectsPlane(bottomLeft.StartPoint,     bottomLeftDir,  nearPlanePoint, Vec3.Forward,  out nearBottomLeft);
                    Collision.RayIntersectsPlane(bottomRight.StartPoint,    bottomRightDir, nearPlanePoint, Vec3.Forward,  out nearBottomRight);
                    Collision.RayIntersectsPlane(topLeft.StartPoint,        topLeftDir,     nearPlanePoint, Vec3.Forward,  out nearTopLeft);
                    Collision.RayIntersectsPlane(topRight.StartPoint,       topRightDir,    nearPlanePoint, Vec3.Forward,  out nearTopRight);
                    Collision.RayIntersectsPlane(bottomLeft.StartPoint,     bottomLeftDir,  farPlanePoint,  Vec3.Backward, out farBottomLeft);
                    Collision.RayIntersectsPlane(bottomRight.StartPoint,    bottomRightDir, farPlanePoint,  Vec3.Backward, out farBottomRight);
                    Collision.RayIntersectsPlane(topLeft.StartPoint,        topLeftDir,     farPlanePoint,  Vec3.Backward, out farTopLeft);
                    Collision.RayIntersectsPlane(topRight.StartPoint,       topRightDir,    farPlanePoint,  Vec3.Backward, out farTopRight);
                }
                slices[i].UpdatePoints(farBottomLeft, farBottomRight, farTopLeft, farTopRight, nearBottomLeft, nearBottomRight, nearTopLeft, nearTopRight);
            }
        }

        public void GetCornerPoints(EFrustumPlane planeIndex, out Vec3 bottomLeft, out Vec3 bottomRight, out Vec3 topRight, out Vec3 topLeft)
        {
            switch (planeIndex)
            {
                case EFrustumPlane.Near:
                    bottomLeft = NearBottomLeft;
                    bottomRight = NearBottomRight;
                    topRight = NearTopRight;
                    topLeft = NearTopLeft;
                    break;
                case EFrustumPlane.Far:
                    bottomLeft = FarBottomLeft;
                    bottomRight = FarBottomRight;
                    topRight = FarTopRight;
                    topLeft = FarTopLeft;
                    break;
                case EFrustumPlane.Left:
                    bottomLeft = FarBottomLeft;
                    bottomRight = NearBottomLeft;
                    topRight = NearTopLeft;
                    topLeft = FarTopLeft;
                    break;
                case EFrustumPlane.Right:
                    bottomLeft = NearBottomRight;
                    bottomRight = FarBottomRight;
                    topRight = FarTopRight;
                    topLeft = NearTopRight;
                    break;
                case EFrustumPlane.Top:
                    bottomLeft = NearTopLeft;
                    bottomRight = NearTopRight;
                    topRight = FarTopRight;
                    topLeft = FarTopLeft;
                    break;
                case EFrustumPlane.Bottom:
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

        [TPostDeserialize]
        internal void PostDeserialize()
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
        public Plane Near => Planes[0];
        [Browsable(false)]
        public Plane Far => Planes[1];

        [Browsable(false)]
        public Plane Left => Planes[2];
        [Browsable(false)]
        public Plane Right => Planes[3];

        [Browsable(false)]
        public Plane Top => Planes[4];
        [Browsable(false)]
        public Plane Bottom => Planes[5];

        [Browsable(false)]
        public Vec3[] Points => _points;
        [Browsable(false)]
        public Sphere BoundingSphere
        {
            get => _boundingSphere;
            set => _boundingSphere = value;
        }
        [Browsable(false)]
        public Plane[] Planes { get; } = new Plane[6];

        //[Browsable(false)]
        //public ERenderPass3D RenderPass => ERenderPass3D.OpaqueForward;
        //[Browsable(false)]
        //public float RenderOrder => 0.0f;

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

        public IScene3D OwningScene3D { get; set; }

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

        public void UpdatePoints(
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
            Planes[0] = new Plane(nearBottomRight, nearBottomLeft, nearTopRight);
            Planes[1] = new Plane(farBottomLeft, farBottomRight, farTopLeft);

            //left, right
            Planes[2] = new Plane(nearBottomLeft, farBottomLeft, nearTopLeft);
            Planes[3] = new Plane(farBottomRight, nearBottomRight, farTopRight);

            //top, bottom
            Planes[4] = new Plane(farTopLeft, farTopRight, nearTopLeft);
            Planes[5] = new Plane(nearBottomLeft, nearBottomRight, farBottomLeft);

            //CalculateBoundingSphere();
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
            Planes[0] = new Plane(nearBottomRight, nearBottomLeft, nearTopRight);
            Planes[1] = new Plane(farBottomLeft, farBottomRight, farTopLeft);

            //left, right
            Planes[2] = new Plane(nearBottomLeft, farBottomLeft, nearTopLeft);
            Planes[3] = new Plane(farBottomRight, nearBottomRight, farTopRight);

            //top, bottom
            Planes[4] = new Plane(farTopLeft, farTopRight, nearTopLeft);
            Planes[5] = new Plane(nearBottomLeft, nearBottomRight, farBottomLeft);

            UpdateBoundingSphere(sphereCenter, sphereRadius);
        }

        public bool IntersectsRay(Vec3 startPoint, Vec3 direction, out IList<Vec3> points)
        {
            Ray r = new Ray(startPoint, direction);
            points = new List<Vec3>();
            foreach (Plane p in this)
                if (Collision.RayIntersectsPlane(r.StartPoint, r.Direction, p.IntersectionPoint, p.Normal, out Vec3 point))
                    points.Add(point);
            return points.Count > 0;
        }

        public void TransformedVersionOf(IFrustum other, Matrix4 transform)
        {
            if (_boundingSphere != null)
            {
                _boundingSphere.Center.Value = other.BoundingSphere.Center.Value * transform;
                _boundingSphere.Radius = other.BoundingSphere.Radius;
            }
            for (int i = 0; i < 8; ++i)
                _points[i] = other.Points[i] * transform;
            for (int i = 0; i < 6; ++i)
                Planes[i] = other.Planes[i].TransformedBy(transform);
        }

        public void TransformBy(Matrix4 transform)
        {
            if (_boundingSphere != null)
                _boundingSphere.Center.Value = _boundingSphere.Center.Value * transform;
            for (int i = 0; i < 8; ++i)
                _points[i] = _points[i] * transform;
            for (int i = 0; i < 6; ++i)
                Planes[i].TransformBy(transform);
        }

        public IFrustum TransformedBy(Matrix4 transform)
        {
            IFrustum f = new Frustum();
            if (_boundingSphere != null)
                f.BoundingSphere = new Sphere(_boundingSphere.Radius, _boundingSphere.Center.Value * transform);
            for (int i = 0; i < 8; ++i)
                f.Points[i] = _points[i] * transform;
            for (int i = 0; i < 6; ++i)
                f.Planes[i] = Planes[i].TransformedBy(transform);
            return f;
        }
        public bool Contains(Vec3 point)
            => Collision.FrustumContainsPoint(this, point);
        public EContainment Contains(TShape shape)
            => shape.GetType().GetFriendlyName() switch
            {
                nameof(Box) => Contains(shape as Box),
                nameof(BoundingBox) => Contains(shape as BoundingBox),
                nameof(Sphere) => Contains(shape as Sphere),
                nameof(Cone) => Contains(shape as Cone),
                nameof(Capsule) => Contains(shape as Capsule),
                nameof(Cylinder) => Contains(shape as Cylinder),
                _ => EContainment.Contains,
            };
        public EContainment Contains(Box box) 
            => Collision.FrustumContainsBox1(this, box.HalfExtents, box.Transform.Matrix);
        public EContainment Contains(BoundingBoxStruct box)
            => Collision.FrustumContainsAABB(this, box.Minimum, box.Maximum);
        public EContainment Contains(BoundingBox box) 
            => Collision.FrustumContainsAABB(this, box.Minimum, box.Maximum);
        public EContainment Contains(Sphere sphere) 
            => Collision.FrustumContainsSphere(this, sphere.Center, sphere.Radius);
        public EContainment Contains(Cylinder cylinder)
        {
            return EContainment.Contains;
        }
        public EContainment Contains(Cone cone)
        {
            bool top = Contains(cone.GetTopPoint());
            bool bot = Contains(cone.GetBottomCenterPoint());
            if (top && bot)
                return EContainment.Contains;
            else if (!top && !bot)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public EContainment Contains(Capsule capsule)
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
                GetCornerPoints((EFrustumPlane)i, out Vec3 bottomLeft, out Vec3 bottomRight, out Vec3 topRight, out Vec3 topLeft);
                //Intersect the capsule's inner segment with the plane as a ray
                if (Collision.RayIntersectsPlane(bot.Center, top.Center, p.IntersectionPoint, p.Normal, out Vec3 point))
                {
                    //Make sure the resulting point is even within the range of the capsule
                    Segment.ESegmentPart part = Segment.GetDistancePointToSegmentPart(bot.Center, top.Center, point, out float closestPartDist);
                    if (closestPartDist > radius)
                        continue;

                    switch (part)
                    {
                        //TODO
                        case Segment.ESegmentPart.Line:
                            //Vec3 perp = Ray.GetPerpendicularVectorFromPoint(bot.Center, top.Center - bot.Center, point);

                            break;
                        case Segment.ESegmentPart.StartPoint:
                        case Segment.ESegmentPart.EndPoint:
                            //Vec3 partPoint = part == Segment.ESegmentPart.StartPoint ? bot.Center : top.Center;

                            break;
                    }

                    //First test for direct intersections with the plane, within its bounds
                    if (point.IsInTriangle(bottomLeft, bottomRight, topRight) &&
                        point.IsInTriangle(bottomLeft, topLeft, topRight))
                    {
                        return EContainment.Intersects;
                    }

                    //Now test distances to each plane edge
                    if (Segment.ShortestDistanceToPoint(bottomLeft, bottomRight, point) < radius)
                        return EContainment.Intersects;
                    if (Segment.ShortestDistanceToPoint(bottomRight, topRight, point) < radius)
                        return EContainment.Intersects;
                    if (Segment.ShortestDistanceToPoint(topRight, topLeft, point) < radius)
                        return EContainment.Intersects;
                    if (Segment.ShortestDistanceToPoint(topLeft, bottomLeft, point) < radius)
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
        public IEnumerator<Plane> GetEnumerator() => ((IEnumerable<Plane>)Planes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Plane>)Planes).GetEnumerator();

        public void Render(bool shadowPass) => Render(shadowPass, Color.Orange, Color.DarkRed, Color.LightGreen, 1.0f, false);
        public void Render(bool shadowPass, Color NearColor, Color FarColor, Color SideColor, float LineSize, bool RenderSphere)
        {
            if (RenderSphere)
                _boundingSphere.Render(shadowPass);
            
            //TODO: use PrimitiveManager; render translucent planes
            Engine.Renderer.RenderLine(NearTopLeft, FarTopLeft, SideColor, true, LineSize);
            Engine.Renderer.RenderLine(NearTopRight, FarTopRight, SideColor, true, LineSize);
            Engine.Renderer.RenderLine(NearBottomLeft, FarBottomLeft, SideColor, true, LineSize);
            Engine.Renderer.RenderLine(NearBottomRight, FarBottomRight, SideColor, true, LineSize);

            Engine.Renderer.RenderLine(NearTopLeft, NearTopRight, NearColor, true, LineSize);
            Engine.Renderer.RenderLine(NearBottomLeft, NearBottomRight, NearColor, true, LineSize);
            Engine.Renderer.RenderLine(NearBottomLeft, NearTopLeft, NearColor, true, LineSize);
            Engine.Renderer.RenderLine(NearBottomRight, NearTopRight, NearColor, true, LineSize);

            Engine.Renderer.RenderLine(FarTopLeft, FarTopRight, FarColor, true, LineSize);
            Engine.Renderer.RenderLine(FarBottomLeft, FarBottomRight, FarColor, true, LineSize);
            Engine.Renderer.RenderLine(FarBottomLeft, FarTopLeft, FarColor, true, LineSize);
            Engine.Renderer.RenderLine(FarBottomRight, FarTopRight, FarColor, true, LineSize);
        }

        public IFrustum HardCopy()
            => new Frustum(
                FarBottomLeft, FarBottomRight, FarTopLeft, FarTopRight,
                NearBottomLeft, NearBottomRight, NearTopLeft, NearTopRight);

        private RenderCommandMethod3D _renderCommand;
        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            passes.Add(_renderCommand);
        }

        public static IFrustum Lerp(IFrustum frustum1, IFrustum frustum2, float time)
            => new Frustum(
                Vec3.Lerp(frustum1.FarBottomLeft,   frustum2.FarBottomLeft,     time),
                Vec3.Lerp(frustum1.FarBottomRight,  frustum2.FarBottomRight,    time),
                Vec3.Lerp(frustum1.FarTopLeft,      frustum2.FarTopLeft,        time),
                Vec3.Lerp(frustum1.FarTopRight,     frustum2.FarTopRight,       time),
                Vec3.Lerp(frustum1.NearBottomLeft,  frustum2.NearBottomLeft,    time),
                Vec3.Lerp(frustum1.NearBottomRight, frustum2.NearBottomRight,   time),
                Vec3.Lerp(frustum1.NearTopLeft,     frustum2.NearTopLeft,       time),
                Vec3.Lerp(frustum1.NearTopRight,    frustum2.NearTopRight,      time));
    }
}
