using TheraEngine;
using TheraEngine.Rendering.Models;
using System.Globalization;
using BulletSharp;
using System.Drawing;
using System.ComponentModel;

namespace System
{
    /// <summary>
    /// Axis Aligned Bounding Box (AABB)
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BoundingBox : Shape
    {
        //public static List<BoundingBox> Active = new List<BoundingBox>();

        [DefaultValue("0 0 0")]
        [Serialize("HalfExtents")]
        protected EventVec3 _halfExtents = Vec3.Half;
        [DefaultValue("0 0 0")]
        [Serialize("Translation")]
        protected EventVec3 _translation = Vec3.Zero;

        [TypeConverter(typeof(Vec3StringConverter))]
        public Vec3 Minimum
        {
            get => _translation - _halfExtents;
            set
            {
                _translation = (Maximum + value) / 2.0f;
                _halfExtents = (Maximum - value) / 2.0f;
            }
        }
        [TypeConverter(typeof(Vec3StringConverter))]
        public Vec3 Maximum
        {
            get => _translation + _halfExtents;
            set
            {
                _translation = (value + Minimum) / 2.0f;
                _halfExtents = (value - Minimum) / 2.0f;
            }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public EventVec3 HalfExtents
        {
            get => _halfExtents;
            set => _halfExtents = value;
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public EventVec3 Translation
        {
            get => _translation;
            set => _translation = value;
        }

        public BoundingBox(float uniformHalfExtents)
            : this(new Vec3(uniformHalfExtents)) { }
        public BoundingBox(float uniformHalfExtents, Vec3 translation)
            : this(new Vec3(uniformHalfExtents), translation) { }
        public BoundingBox(float halfExtentX, float halfExtentY, float halfExtentZ)
            : this(new Vec3(halfExtentX, halfExtentY, halfExtentZ)) { }
        public BoundingBox(float halfExtentX, float halfExtentY, float halfExtentZ, Vec3 translation)
            : this(new Vec3(halfExtentX, halfExtentY, halfExtentZ), translation) { }
        public BoundingBox(float halfExtentX, float halfExtentY, float halfExtentZ, float x, float y, float z)
            : this(new Vec3(halfExtentX, halfExtentY, halfExtentZ), new Vec3(x, y, z)) { }
        public BoundingBox(Vec3 halfExtents)
            : this(halfExtents, Vec3.Zero) { }
        public BoundingBox(Vec3 halfExtents, Vec3 translation) 
            : this()
        {
            _halfExtents = halfExtents;
            _translation = translation;
        }
        public BoundingBox()
        {
        //    ShapeIndex = Active.Count;
        //    Active.Add(this);
        }
        //~BoundingBox()
        //{
        //    Active.RemoveAt(ShapeIndex);
        //}

        public static BoundingBox FromMinMax(Vec3 min, Vec3 max)
            => new BoundingBox()
            {
                _translation = (max + min) / 2.0f,
                _halfExtents = (max - min) / 2.0f,
            };
        public static BoundingBox FromHalfExtentsTranslation(Vec3 halfExtents, Vec3 translation)
            => new BoundingBox()
            {
                _translation = translation,
                _halfExtents = halfExtents,
            };
        
        public override CollisionShape GetCollisionShape()
            => new BoxShape(HalfExtents);
        
        /// <summary>
        /// [T = top, B = bottom] 
        /// [B = back, F = front] 
        /// [L = left,  R = right]
        /// </summary>
        public void GetCorners(
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
            => GetCorners(Minimum, Maximum, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        
        /// <summary>
        /// [T = top, B = bottom] 
        /// [B = back, F = front] 
        /// [L = left,  R = right]
        /// </summary>
        public static void GetCorners(
            Vec3 min,
            Vec3 max,
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            float Top = max.Y;
            float Bottom = min.Y;
            float Front = max.Z;
            float Back = min.Z;
            float Right = max.X;
            float Left = min.X;

            TBL = new Vec3(Left, Top, Back);
            TBR = new Vec3(Right, Top, Back);

            TFL = new Vec3(Left, Top, Front);
            TFR = new Vec3(Right, Top, Front);

            BBL = new Vec3(Left, Bottom, Back);
            BBR = new Vec3(Right, Bottom, Back);

            BFL = new Vec3(Left, Bottom, Front);
            BFR = new Vec3(Right, Bottom, Front);
        }
        /// <summary>
        /// [T = top, B = bottom] 
        /// [B = back, F = front] 
        /// [L = left,  R = right]
        /// </summary>
        public void GetCorners(
            Matrix4 transform,
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
            => GetCorners(_halfExtents, transform, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        
        /// <summary>
        /// [T = top, B = bottom] 
        /// [B = back, F = front] 
        /// [L = left,  R = right]
        /// </summary>
        public static void GetCorners(
            Vec3 halfExtents,
            Matrix4 transform,
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            float Top = halfExtents.Y;
            float Bottom = -halfExtents.Y;
            float Front = halfExtents.Z;
            float Back = -halfExtents.Z;
            float Right = halfExtents.X;
            float Left = -halfExtents.X;

            TBL = Vec3.TransformPosition(new Vec3(Left, Top, Back), transform);
            TBR = Vec3.TransformPosition(new Vec3(Right, Top, Back), transform);

            TFL = Vec3.TransformPosition(new Vec3(Left, Top, Front), transform);
            TFR = Vec3.TransformPosition(new Vec3(Right, Top, Front), transform);

            BBL = Vec3.TransformPosition(new Vec3(Left, Bottom, Back), transform);
            BBR = Vec3.TransformPosition(new Vec3(Right, Bottom, Back), transform);

            BFL = Vec3.TransformPosition(new Vec3(Left, Bottom, Front), transform);
            BFR = Vec3.TransformPosition(new Vec3(Right, Bottom, Front), transform);
        }
        public Vec3[] GetCorners()
        {
            GetCorners(out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public Vec3[] GetCorners(Matrix4 transform)
        {
            GetCorners(transform, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public static Vec3[] GetCorners(Vec3 halfExtents, Matrix4 transform)
        {
            GetCorners(halfExtents, transform, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public static Vec3[] GetCorners(Vec3 boxMin, Vec3 boxMax)
        {
            GetCorners(boxMin, boxMax, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public void ExpandBounds(Vec3 point)
        {
            Minimum.SetLequalTo(point);
            Maximum.SetGequalTo(point);
        }

        public override void Render()
            => Engine.Renderer.RenderAABB(HalfExtents, Translation, _renderSolid, Color.Blue);

        public static PrimitiveData WireframeMesh(Vec3 min, Vec3 max)
        {
            VertexLine 
                topFront, topRight, topBack, topLeft, 
                frontLeft, frontRight, backLeft, backRight,
                bottomFront, bottomRight, bottomBack, bottomLeft;

            GetCorners(min, max, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);

            topFront = new VertexLine(new Vertex(TFL), new Vertex(TFR));
            topBack = new VertexLine(new Vertex(TBL), new Vertex(TBR));
            topLeft = new VertexLine(new Vertex(TFL), new Vertex(TBL));
            topRight = new VertexLine(new Vertex(TFR), new Vertex(TBR));

            bottomFront = new VertexLine(new Vertex(BFL), new Vertex(BFR));
            bottomBack = new VertexLine(new Vertex(BBL), new Vertex(BBR));
            bottomLeft = new VertexLine(new Vertex(BFL), new Vertex(BBL));
            bottomRight = new VertexLine(new Vertex(BFR), new Vertex(BBR));

            frontLeft = new VertexLine(new Vertex(TFL), new Vertex(BFL));
            frontRight = new VertexLine(new Vertex(TFR), new Vertex(BFR));
            backLeft = new VertexLine(new Vertex(TBL), new Vertex(BBL));
            backRight = new VertexLine(new Vertex(TBR), new Vertex(BBR));

            return PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), 
                topFront, topRight, topBack, topLeft,
                frontLeft, frontRight, backLeft, backRight,
                bottomFront, bottomRight, bottomBack, bottomLeft);
        }
        public static PrimitiveData SolidMesh(Vec3 min, Vec3 max)
        {
            VertexQuad left, right, top, bottom, front, back;

            GetCorners(min, max, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);

            Vec3 rightNormal = Vec3.Right;
            Vec3 frontNormal = -Vec3.Forward;
            Vec3 topNormal = Vec3.Up;
            Vec3 leftNormal = -rightNormal;
            Vec3 backNormal = -frontNormal;
            Vec3 bottomNormal = -topNormal;

            left = VertexQuad.MakeQuad(BBL, BFL, TFL, TBL, leftNormal);
            right = VertexQuad.MakeQuad(BFR, BBR, TBR, TFR, rightNormal);
            top = VertexQuad.MakeQuad(TFL, TFR, TBR, TBL, topNormal);
            bottom = VertexQuad.MakeQuad(BBL, BBR, BFR, BFL, bottomNormal);
            front = VertexQuad.MakeQuad(BFL, BFR, TFR, TFL, frontNormal);
            back = VertexQuad.MakeQuad(BBR, BBL, TBL, TBR, backNormal);

            return PrimitiveData.FromQuads(Culling.Back, VertexShaderDesc.PosNormTex(), left, right, top, bottom, front, back);
        }
        public PrimitiveData GetMesh(bool includeTranslation)
        {
            if (includeTranslation)
                return SolidMesh(Minimum, Maximum);
            else
                return SolidMesh(-_halfExtents, _halfExtents);
        }
        public static Frustum GetFrustum(Vec3 min, Vec3 max)
        {
            GetCorners(min, max, out Vec3 ftl, out Vec3 ftr, out Vec3 ntl, out Vec3 ntr, out Vec3 fbl, out Vec3 fbr, out Vec3 nbl, out Vec3 nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr, (min + max) / 2.0f);
        }
        public static Frustum GetFrustum(Vec3 halfExtents, Matrix4 transform)
        {
            GetCorners(halfExtents, transform, out Vec3 ftl, out Vec3 ftr, out Vec3 ntl, out Vec3 ntr, out Vec3 fbl, out Vec3 fbr, out Vec3 nbl, out Vec3 nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr, transform.GetPoint());
        }
        public Frustum AsFrustum() => GetFrustum(Minimum, Maximum);
        public Frustum AsFrustum(Matrix4 transform) => GetFrustum(_halfExtents, transform);
        public bool Intersects(Ray ray)
        {
            return Collision.RayIntersectsAABBDistance(ray.StartPoint, ray.Direction, Minimum, Maximum, out float distance);
        }
        public bool Intersects(Ray ray, out float distance)
        {
            return Collision.RayIntersectsAABBDistance(ray.StartPoint, ray.Direction, Minimum, Maximum, out distance);
        }
        public bool Intersects(Ray ray, out Vec3 point)
        {
            return Collision.RayIntersectsAABB(ray, Minimum, Maximum, out point);
        }
        public static BoundingBox FromSphere(Sphere sphere)
        {
            return FromMinMax(
                new Vec3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius),
                new Vec3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius));
        }
        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
        {
            return FromMinMax(Vec3.ComponentMin(box1.Minimum, box2.Maximum), Vec3.ComponentMax(box1.Maximum, box2.Maximum));
        }
        public static bool operator ==(BoundingBox left, BoundingBox right) { return left.Equals(ref right); }
        public static bool operator !=(BoundingBox left, BoundingBox right) { return !left.Equals(ref right); }
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
        public bool Equals(ref BoundingBox value) { return Minimum == value.Minimum && Maximum == value.Maximum; }
        public override bool Equals(object value)
        {
            if (!(value is BoundingBox))
                return false;

            var strongValue = (BoundingBox)value;
            return Equals(ref strongValue);
        }

        public override bool Contains(Vec3 point)
        {
            return Collision.AABBContainsPoint(Minimum, Maximum, point);
        }
        public override EContainment Contains(BoundingBox box)
        {
            return Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        }
        public override EContainment Contains(Box box)
        {
            return Collision.AABBContainsBox(Minimum, Maximum, box.HalfExtents, box.WorldMatrix);
        }
        public override EContainment Contains(Sphere sphere)
        {
            return Collision.AABBContainsSphere(Minimum, Maximum, sphere.Center, sphere.Radius);
        }
        public override EContainment ContainedWithin(BoundingBox box)
        {
            return box.Contains(this);
        }
        public override EContainment ContainedWithin(Box box)
        {
            return box.Contains(this);
        }
        public override EContainment ContainedWithin(Sphere sphere)
        {
            return sphere.Contains(this);
        }
        public override EContainment ContainedWithin(Frustum frustum)
        {
            return frustum.Contains(this);
        }
        public override void SetTransform(Matrix4 worldMatrix)
        {
            _translation = worldMatrix.GetPoint();
            base.SetTransform(worldMatrix);
        }
        public override Shape HardCopy()
        {
            return FromHalfExtentsTranslation(_halfExtents, _translation);
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            BoundingBox newBox = FromHalfExtentsTranslation(_halfExtents, _translation);
            newBox.SetTransform(worldMatrix);
            return newBox;
        }
    }
}