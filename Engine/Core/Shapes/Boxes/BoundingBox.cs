using TheraEngine.Rendering.Models;
using System.Globalization;
using System.Drawing;
using System.ComponentModel;
using System;
using TheraEngine.Physics;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Core.Shapes
{
    /// <summary>
    /// Axis-Aligned Bounding Box (AABB)
    /// </summary>
    [FileDef("Axis-Aligned Bounding Box")]
    public class BoundingBox : Shape
    {
        //public static List<BoundingBox> Active = new List<BoundingBox>();
        public static BoundingBox ExpandableBox() => FromMinMax(float.MaxValue, float.MinValue);

        #region Fields
        [TSerialize("HalfExtents")]
        protected EventVec3 _halfExtents = Vec3.Half;
        [TSerialize("Translation")]
        protected EventVec3 _translation = Vec3.Zero;
        #endregion

        #region Properties
        /// <summary>
        /// The minimum corner position coordinate of this <see cref="BoundingBox"/>.
        /// All components are the smallest.
        /// </summary>
        public Vec3 Minimum
        {
            get => _translation - _halfExtents;
            set
            {
                _translation.Raw = (Maximum + value) / 2.0f;
                _halfExtents.Raw = (Maximum - value) / 2.0f;
            }
        }
        public Box AsBox() => new Box(this);
        /// <summary>
        /// The maximum corner position coordinate of this <see cref="BoundingBox"/>.
        /// All components are the largest.
        /// </summary>
        public Vec3 Maximum
        {
            get => _translation + _halfExtents;
            set
            {
                _translation.Raw = (value + Minimum) / 2.0f;
                _halfExtents.Raw = (value - Minimum) / 2.0f;
            }
        }
        /// <summary>
        /// Half of the box's total bounds.
        /// Basically a vector starting at the box's origin that pushes outward diagonally to dictate a corner.
        /// </summary>
        public EventVec3 HalfExtents
        {
            get => _halfExtents;
            set => _halfExtents = value ?? Vec3.Half;
        }
        /// <summary>
        /// The translation of the box's origin from the world/parent origin.
        /// </summary>
        public EventVec3 Translation
        {
            get => _translation;
            set => _translation = value ?? Vec3.Zero;
        }
        #endregion

        #region Constructors
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
        #endregion
        
        #region Corners
        /// <summary>
        /// Returns the corners of this box at its current position.
        /// Naming system (back, front, etc) is relative to a camera looking in the -Z direction (forward).
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
        /// Returns the corners of a box with the given minimum and maximum corner coordinates.
        /// Naming system (back, front, etc) is relative to a camera looking in the -Z direction (forward).
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
        /// Returns the corners of this box transformed by the given matrix.
        /// Naming system (back, front, etc) is relative to a camera looking in the -Z direction (forward), before the matrix is applied.
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
        /// Returns the corners of a box with the given half extents and transformed by the given matrix.
        /// Naming system (back, front, etc) is relative to a camera looking in the -Z direction (forward), before the matrix is applied.
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
        #endregion

        #region Meshes
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
        public enum ECubemapTextureUVs
        {
            /// <summary>
            /// Specifies that each quad should have normal corner-to-corner UVs.
            /// </summary>
            None,
            /// <summary>
            /// Specifies that each quad should be mapped for a cubemap texture that has a larger width (4x3).
            /// </summary>
            WidthLarger,
            /// <summary>
            /// Specifies that each quad should be mapped for a cubemap texture that has a larger height (3x4).
            /// </summary>
            HeightLarger,
        }
        /// <summary>
        /// Generates a bounding box mesh.
        /// </summary>
        /// <param name="min">Minimum axis-aligned bound of the box.</param>
        /// <param name="max">Maximum axis-aligned bound of the box.</param>
        /// <param name="inwardFacing">If the faces' fronts should face inward instead of outward.</param>
        /// <param name="cubemapUVs">If each quad should use UVs for </param>
        /// <returns></returns>
        public static PrimitiveData SolidMesh(Vec3 min, Vec3 max, bool inwardFacing = false, ECubemapTextureUVs cubemapUVs = ECubemapTextureUVs.None)
        {
            VertexQuad left, right, top, bottom, front, back;

            GetCorners(min, max, 
                out Vec3 TBL,
                out Vec3 TBR,
                out Vec3 TFL,
                out Vec3 TFR,
                out Vec3 BBL,
                out Vec3 BBR,
                out Vec3 BFL,
                out Vec3 BFR);

            Vec3 rightNormal = inwardFacing ? Vec3.Left : Vec3.Right;
            Vec3 frontNormal = inwardFacing ? Vec3.Forward : Vec3.Backward;
            Vec3 topNormal = inwardFacing ? Vec3.Down : Vec3.Up;
            Vec3 leftNormal = -rightNormal;
            Vec3 backNormal = -frontNormal;
            Vec3 bottomNormal = -topNormal;

            if (cubemapUVs == ECubemapTextureUVs.None)
            {
                left = inwardFacing ? VertexQuad.MakeQuad(BFL, BBL, TBL, TFL, leftNormal) : VertexQuad.MakeQuad(BBL, BFL, TFL, TBL, leftNormal);
                right = inwardFacing ? VertexQuad.MakeQuad(BBR, BFR, TFR, TBR, rightNormal) : VertexQuad.MakeQuad(BFR, BBR, TBR, TFR, rightNormal);
                top = inwardFacing ? VertexQuad.MakeQuad(TBL, TBR, TFR, TFL, topNormal) : VertexQuad.MakeQuad(TFL, TFR, TBR, TBL, topNormal);
                bottom = inwardFacing ? VertexQuad.MakeQuad(BFL, BFR, BBR, BBL, bottomNormal) : VertexQuad.MakeQuad(BBL, BBR, BFR, BFL, bottomNormal);
                front = inwardFacing ? VertexQuad.MakeQuad(BFR, BFL, TFL, TFR, frontNormal) : VertexQuad.MakeQuad(BFL, BFR, TFR, TFL, frontNormal);
                back = inwardFacing ? VertexQuad.MakeQuad(BBL, BBR, TBR, TBL, backNormal) : VertexQuad.MakeQuad(BBR, BBL, TBL, TBR, backNormal);
            }
            else
            {
                bool widthLarger = cubemapUVs == ECubemapTextureUVs.WidthLarger;
                left = inwardFacing ? VertexQuad.MakeQuad(BFL, BBL, TBL, TFL, leftNormal, ECubemapFace.NegX, widthLarger) : VertexQuad.MakeQuad(BBL, BFL, TFL, TBL, leftNormal, ECubemapFace.NegX, widthLarger);
                right = inwardFacing ? VertexQuad.MakeQuad(BBR, BFR, TFR, TBR, rightNormal, ECubemapFace.PosX, widthLarger) : VertexQuad.MakeQuad(BFR, BBR, TBR, TFR, rightNormal, ECubemapFace.PosX, widthLarger);
                top = inwardFacing ? VertexQuad.MakeQuad(TBL, TBR, TFR, TFL, topNormal, ECubemapFace.PosY, widthLarger) : VertexQuad.MakeQuad(TFL, TFR, TBR, TBL, topNormal, ECubemapFace.PosY, widthLarger);
                bottom = inwardFacing ? VertexQuad.MakeQuad(BFL, BFR, BBR, BBL, bottomNormal, ECubemapFace.NegY, widthLarger) : VertexQuad.MakeQuad(BBL, BBR, BFR, BFL, bottomNormal, ECubemapFace.NegY, widthLarger);
                front = inwardFacing ? VertexQuad.MakeQuad(BFR, BFL, TFL, TFR, frontNormal, ECubemapFace.PosZ, widthLarger) : VertexQuad.MakeQuad(BFL, BFR, TFR, TFL, frontNormal, ECubemapFace.PosZ, widthLarger);
                back = inwardFacing ? VertexQuad.MakeQuad(BBL, BBR, TBR, TBL, backNormal, ECubemapFace.NegZ, widthLarger) : VertexQuad.MakeQuad(BBR, BBL, TBL, TBR, backNormal, ECubemapFace.NegZ, widthLarger);
            }
            
            return PrimitiveData.FromQuads(Culling.Back, VertexShaderDesc.PosNormTex(), left, right, top, bottom, front, back);
        }
        /// <summary>
        /// Creates a mesh representing this bounding box.
        /// </summary>
        /// <param name="includeTranslation">If true, makes mesh with minimum and maximum coordinates.
        /// If false, makes the mesh about the origin.</param>
        public PrimitiveData GetMesh(bool includeTranslation)
        {
            if (includeTranslation)
                return SolidMesh(Minimum, Maximum);
            else
                return SolidMesh(-_halfExtents, _halfExtents);
        }
        #endregion

        #region Frustums
        /// <summary>
        /// Creates a frustum from the minimum and maximum coordinates of a bounding box.
        /// </summary>
        public static Frustum GetFrustum(Vec3 min, Vec3 max)
        {
            GetCorners(min, max, out Vec3 ftl, out Vec3 ftr, out Vec3 ntl, out Vec3 ntr, out Vec3 fbl, out Vec3 fbr, out Vec3 nbl, out Vec3 nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        /// <summary>
        /// Creates a frustum from the half extents and the transform of a bounding box.
        /// </summary>
        public static Frustum GetFrustum(Vec3 halfExtents, Matrix4 transform)
        {
            GetCorners(halfExtents, transform, out Vec3 ftl, out Vec3 ftr, out Vec3 ntl, out Vec3 ntr, out Vec3 fbl, out Vec3 fbr, out Vec3 nbl, out Vec3 nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        /// <summary>
        /// Converts this bounding box to a frustum.
        /// </summary>
        /// <returns></returns>
        public Frustum AsFrustum() => GetFrustum(Minimum, Maximum);
        /// <summary>
        /// Converts this bounding box to a frustum, transformed by the given matrix.
        /// </summary>
        public Frustum AsFrustum(Matrix4 transform) => GetFrustum(_halfExtents, transform);
        #endregion

        //Half extents is one of 8 octants that make up the box, so multiply half extent volume by 8
        public float GetVolume() =>
            _halfExtents.X * _halfExtents.Y * _halfExtents.Z * 8.0f;
        //Each half extent side is one of 4 quadrants on both sides of the box, so multiply each side area by 8
        public float GetSurfaceArea() =>
            _halfExtents.X * _halfExtents.Y * 8.0f +
            _halfExtents.Y * _halfExtents.Z * 8.0f +
            _halfExtents.Z * _halfExtents.X * 8.0f;

        /// <summary>
        /// Expands this bounding box to include the given point.
        /// </summary>
        public void Expand(Vec3 point)
        {
            Vec3 min = Vec3.ComponentMin(point, Minimum);
            Vec3 max = Vec3.ComponentMax(point, Maximum);
            _translation.Raw = (max + min) / 2.0f;
            _halfExtents.Raw = (max - min) / 2.0f;
        }
        public void Expand(BoundingBox box)
        {
            Vec3 min = Vec3.ComponentMin(box.Minimum, box.Maximum, Minimum);
            Vec3 max = Vec3.ComponentMax(box.Minimum, box.Maximum, Maximum);
            _translation.Raw = (max + min) / 2.0f;
            _halfExtents.Raw = (max - min) / 2.0f;
        }

        #region Collision
        /// <summary>
        /// Returns true if the given <see cref="Ray"/> intersects this <see cref="BoundingBox"/>.
        /// </summary>
        public bool Intersects(Ray ray)
            => Collision.RayIntersectsAABBDistance(ray.StartPoint, ray.Direction, Minimum, Maximum, out float distance);
        /// <summary>
        /// Returns true if the given <see cref="Ray"/> intersects this <see cref="BoundingBox"/>.
        /// Returns the distance of the closest intersection.
        /// </summary>
        public bool Intersects(Ray ray, out float distance)
            => Collision.RayIntersectsAABBDistance(ray, Minimum, Maximum, out distance);
        public bool Intersects(Vec3 start, Vec3 direction, out float distance)
            => Collision.RayIntersectsAABBDistance(start, direction, Minimum, Maximum, out distance);
        /// <summary>
        /// Returns true if the given <see cref="Ray"/> intersects this <see cref="BoundingBox"/>.
        /// Returns the position of the closest intersection.
        /// </summary>
        public bool Intersects(Ray ray, out Vec3 point)
            => Collision.RayIntersectsAABB(ray, Minimum, Maximum, out point);
        //public bool Intersects(Vec3 start, Vec3 direction, out Vec3 point)
        //    => Collision.RayIntersectsAABB(start, direction, Minimum, Maximum, out point);

        public override bool Contains(Vec3 point)
            => Collision.AABBContainsPoint(Minimum, Maximum, point);
        public override EContainment Contains(BoundingBox box)
            => Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        public override EContainment Contains(Box box)
            => Collision.AABBContainsBox(Minimum, Maximum, box.HalfExtents, box.WorldMatrix);
        public override EContainment Contains(Sphere sphere)
            => Collision.AABBContainsSphere(Minimum, Maximum, sphere.Center, sphere.Radius);
        public override EContainment ContainedWithin(BoundingBox box)
            => box.Contains(this);
        public override EContainment ContainedWithin(Box box)
            => box.Contains(this);
        public override EContainment ContainedWithin(Sphere sphere)
            => sphere.Contains(this);
        public override EContainment ContainedWithin(Frustum frustum)
            => frustum.Contains(this);
        #endregion

        #region Static Constructors
        /// <summary>
        /// Creates a new bounding box from minimum and maximum coordinates.
        /// </summary>
        public static BoundingBox FromMinMax(Vec3 min, Vec3 max)
            => new BoundingBox((max - min) / 2.0f, (max + min) / 2.0f);
        /// <summary>
        /// Creates a new bounding box from half extents and a translation.
        /// </summary>
        public static BoundingBox FromHalfExtentsTranslation(Vec3 halfExtents, Vec3 translation)
            => new BoundingBox(halfExtents, translation);
        /// <summary>
        /// Creates a bounding box that encloses the given sphere.
        /// </summary>
        public static BoundingBox EnclosingSphere(Sphere sphere)
            => FromMinMax(sphere.Center - sphere.Radius, sphere.Center + sphere.Radius);
        /// <summary>
        /// Creates a bounding box that includes both given bounding boxes.
        /// </summary>
        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
            => FromMinMax(Vec3.ComponentMin(box1.Minimum, box2.Maximum), Vec3.ComponentMax(box1.Maximum, box2.Maximum));
        #endregion

        public static bool operator ==(BoundingBox left, BoundingBox right) => left.Equals(ref right);
        public static bool operator !=(BoundingBox left, BoundingBox right) => !left.Equals(ref right);

        public bool Equals(ref BoundingBox value)
            => Minimum == value.Minimum && Maximum == value.Maximum;

        #region Overrides
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", Minimum.ToString(), Maximum.ToString());
        }
        public override bool Equals(object value)
        {
            if (!(value is BoundingBox))
                return false;

            var strongValue = (BoundingBox)value;
            return Equals(ref strongValue);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode();
            }
        }
        public override void SetRenderTransform(Matrix4 worldMatrix)
        {
            _translation = worldMatrix.Translation;
            base.SetRenderTransform(worldMatrix);
        }
        public override Shape HardCopy()
        {
            return FromHalfExtentsTranslation(_halfExtents, _translation);
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            BoundingBox newBox = FromHalfExtentsTranslation(_halfExtents, _translation);
            newBox.SetRenderTransform(worldMatrix);
            return newBox;
        }
        public override Matrix4 GetTransformMatrix()
            => _translation.Raw.AsTranslationMatrix();
        public override Vec3 ClosestPoint(Vec3 point)
            => Collision.ClosestPointAABBPoint(Minimum, Maximum, point);
        public override TCollisionShape GetCollisionShape()
            => TCollisionBox.New(HalfExtents);
        public override void Render()
            => Engine.Renderer.RenderAABB(HalfExtents, Translation, _renderSolid, Color.Blue);
        public override BoundingBox GetAABB() => this;
        #endregion
    }
}