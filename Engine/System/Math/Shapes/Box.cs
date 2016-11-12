using OpenTK;
using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using CustomEngine.Files;
using System.Globalization;

namespace System
{
    public class Box : Shape
    {
        public Vec3 _min, _max;
        
        public Vec3 Minimum { get { return _min; } set { _min = value; CheckValid(); } }
        public Vec3 Maximum { get { return _max; } set { _max = value; CheckValid(); } }

        public Box(float halfZ, float halfY, float halfX)
        {
            _max = new Vec3(halfX, halfY, halfZ);
            _min = -_max;
            CheckValid();
        }
        public Box(Vec3 min, Vec3 max)
        {
            _min = min;
            _max = max;
            CheckValid();
        }
        public Box(Vec3 bounds)
        {
            _min = -bounds / 2.0f;
            _max = bounds / 2.0f;
            CheckValid();
        }
        private void CheckValid()
        {
            if (_min.X > _max.X)
                MathHelper.Swap(ref _min.X, ref _max.X);
            if (_min.Y > _max.Y)
                MathHelper.Swap(ref _min.Y, ref _max.Y);
            if (_min.Z > _max.Z)
                MathHelper.Swap(ref _min.Z, ref _max.Z);
        }
        public Vec3 CenterPoint
        {
            get { return (_min + _max) / 2.0f; }
            set
            {
                Vec3 currentOrigin = CenterPoint;
                Vec3 newOrigin = value;
                Vec3 diff = newOrigin - currentOrigin;
                _min += diff;
                _max += diff;
            }
        }
        public void GetCorners(
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            GetCorners(Matrix4.Identity, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        }
        /// <summary>
        /// T = top, B = bottom
        /// B = back, F = front
        /// L = left,  R = right
        /// </summary>
        public void GetCorners(Matrix4 transform,
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            float Top = _max.Z;
            float Bottom = _min.Z;
            float Front = _max.Y;
            float Back = _min.Y;
            float Right = _max.X;
            float Left = _min.X;

            TBL = transform * new Vec3(Top, Back, Left);
            TBR = transform * new Vec3(Top, Back, Right);

            TFL = transform * new Vec3(Top, Front, Left);
            TFR = transform * new Vec3(Top, Front, Right);

            BBL = transform * new Vec3(Bottom, Back, Left);
            BBR = transform * new Vec3(Bottom, Back, Right);

            BFL = transform * new Vec3(Bottom, Front, Left);
            BFR = transform * new Vec3(Bottom, Front, Right);
        }
        public Vec3[] GetCorners() { return GetCorners(Matrix4.Identity); }
        public Vec3[] GetCorners(Matrix4 transform)
        {
            float Top = _max.Z;
            float Bottom = _min.Z;
            float Front = _max.Y;
            float Back = _min.Y;
            float Right = _max.X;
            float Left = _min.X;
            return new Vec3[8]
            {
                transform * new Vec3(Top, Back, Left),
                transform * new Vec3(Top, Back, Right),

                transform * new Vec3(Top, Front, Left),
                transform * new Vec3(Top, Front, Right),

                transform * new Vec3(Bottom, Back, Left),
                transform * new Vec3(Bottom, Back, Right),

                transform * new Vec3(Bottom, Front, Left),
                transform * new Vec3(Bottom, Front, Right),
            };
        }
        public void ExpandBounds(Vec3 point)
        {
            _min.SetLequalTo(point);
            _max.SetGequalTo(point);
        }
        public void Render() { Render(false); }
        public void Render(bool solid)
        {
            if (solid)
                Engine.Renderer.DrawBoxSolid(this);
            else
                Engine.Renderer.DrawBoxWireframe(this);
        }
        public override List<PrimitiveData> GetPrimitives()
        {
            VertexQuad left, right, top, bottom, front, back;
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;

            GetCorners(out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);

            Vec3 rightNormal = Vec3.UnitX;
            Vec3 frontNormal = Vec3.UnitY;
            Vec3 topNormal = Vec3.UnitZ;
            Vec3 leftNormal = -rightNormal;
            Vec3 backNormal = -frontNormal;
            Vec3 bottomNormal = -topNormal;

            left = VertexQuad.MakeQuad(BBL, BFL, TBL, TFL, leftNormal);
            right = VertexQuad.MakeQuad(BFR, BBR, TFR, TBR, rightNormal);
            top = VertexQuad.MakeQuad(TFL, TFR, TBL, TBR, topNormal);
            bottom = VertexQuad.MakeQuad(BBL, BBR, BFL, BFR, bottomNormal);
            front = VertexQuad.MakeQuad(BFL, BFR, TFL, TFR, frontNormal);
            back = VertexQuad.MakeQuad(BBR, BBL, TBR, TBL, backNormal);

            return new List<PrimitiveData>() { PrimitiveData.FromQuads(left, right, top, bottom, front, back) };
        }
        public bool Intersects(Ray ray) { float distance; return Collision.RayIntersectsBoxDistance(ray, this, out distance); }
        public bool Intersects(Ray ray, out float distance) { return Collision.RayIntersectsBoxDistance(ray, this, out distance); }
        public bool Intersects(Ray ray, out Vec3 point) { return Collision.RayIntersectsBox(ray, this, out point); }
        public EPlaneIntersection Intersects(Plane plane) { return Collision.PlaneIntersectsBox(plane, this); }
        public bool Intersects(Box box) { return Collision.BoxIntersectsBox(this, box); }
        public bool Intersects(Sphere sphere) { return Collision.BoxIntersectsSphere(this, sphere); }
        public override bool Contains(Vec3 point) { return Collision.BoxContainsPoint(this, point); }
        public EContainment Contains(Box box) { return Collision.BoxContainsBox(this, box); }
        public EContainment Contains(Sphere sphere) { return Collision.BoxContainsSphere(this, sphere); }
        public EContainment WithinFrustrum(Frustrum frustrum) { return Collision.FrustrumContainsBox(frustrum, this); }
        public static Box FromSphere(Sphere sphere)
        {
            return new Box(
                new Vec3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius),
                new Vec3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius));
        }
        public static Box Merge(Box box1, Box box2)
        {
            return new Box(Vec3.ComponentMin(box1.Minimum, box2.Maximum), Vec3.ComponentMax(box1.Maximum, box2.Maximum));
        }
        public static bool operator ==(Box left, Box right) { return left.Equals(ref right); }
        public static bool operator !=(Box left, Box right) { return !left.Equals(ref right); }
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", Minimum.ToString(), Maximum.ToString());
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode();
            }
        }
        public bool Equals(ref Box value) { return Minimum == value.Minimum && Maximum == value.Maximum; }
        public override bool Equals(object value)
        {
            if (!(value is Box))
                return false;

            var strongValue = (Box)value;
            return Equals(ref strongValue);
        }
        public Frustrum AsFrustrum(Matrix4 transform)
        {
            Quaternion rotation = transform.ExtractRotation();

            Plane front, back, top, bottom, left, right;
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;
            GetCorners(transform, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);

            Vec3 rightNormal = rotation * Vec3.UnitX;
            Vec3 frontNormal = rotation * Vec3.UnitY;
            Vec3 topNormal = rotation * Vec3.UnitZ;
            Vec3 leftNormal = -rightNormal;
            Vec3 backNormal = -frontNormal;
            Vec3 bottomNormal = -topNormal;

            front = new Plane(BFR, BFL, TFR);
            back = new Plane(BBL, BBR, TBL);
            top = new Plane(TBL, TBR, TFL);
            bottom = new Plane(BFL, BFR, BBL);
            left = new Plane(BFL, BBL, TFL);
            right = new Plane(BBR, BFR, TBR);
            return new Frustrum(front, back, top, bottom, left, right);
        }
    }
}