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
    [TFileDef("Axis-Aligned Bounding Box")]
    public class BoundingBox : Shape
    {
        //public static List<BoundingBox> Active = new List<BoundingBox>();
        public static BoundingBox ExpandableBox() => FromMinMax(float.MaxValue, float.MinValue);

        private EventVec3 _scale = Vec3.Half;
        private EventVec3 _translation = Vec3.Zero;

        [Category("Bounding Box")]
        public EventVec3 HalfExtents
        {
            get => _scale;
            set => _scale = value ?? Vec3.Half;
        }
        [Category("Bounding Box")]
        public EventVec3 Translation
        {
            get => _translation;
            set => _translation = value ?? Vec3.Zero;
        }
        
        #region Properties
        /// <summary>
        /// The minimum corner position coordinate of this <see cref="BoundingBox"/>.
        /// All components are the smallest.
        /// </summary>
        [Category("Bounding Box")]
        public Vec3 Minimum
        {
            get => Translation.Raw - HalfExtents.Raw;
            set
            {
                Vec3 max = Maximum;
                Translation.Raw = (max + value) / 2.0f;
                HalfExtents.Raw = (max - value) / 2.0f;
            }
        }
        /// <summary>
        /// The maximum corner position coordinate of this <see cref="BoundingBox"/>.
        /// All components are the largest.
        /// </summary>
        [Category("Bounding Box")]
        public Vec3 Maximum
        {
            get => Translation + HalfExtents;
            set
            {
                Vec3 min = Minimum;
                Translation.Raw = (value + min) / 2.0f;
                HalfExtents.Raw = (value - min) / 2.0f;
            }
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
        public BoundingBox(EventVec3 halfExtents)
            : this(halfExtents, new EventVec3(Vec3.Zero)) { }
        public BoundingBox(Vec3 halfExtents, Vec3 translation) 
            : this()
        {
            HalfExtents = halfExtents;
            Translation = translation;
        }
        public BoundingBox(EventVec3 halfExtents, EventVec3 translation)
            : this()
        {
            HalfExtents = halfExtents;
            Translation = translation;
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
            => GetCorners(HalfExtents.Raw, transform, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);

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
        public static PrimitiveData SolidMesh(Vec3 min, Vec3 max, bool inwardFacing = false, ECubemapTextureUVs cubemapUVs = ECubemapTextureUVs.None, float bias = 0.002f)
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
                left =      inwardFacing ? VertexQuad.MakeQuad(BFL, BBL, TBL, TFL, leftNormal)      : VertexQuad.MakeQuad(BBL, BFL, TFL, TBL, leftNormal);
                right =     inwardFacing ? VertexQuad.MakeQuad(BBR, BFR, TFR, TBR, rightNormal)     : VertexQuad.MakeQuad(BFR, BBR, TBR, TFR, rightNormal);
                top =       inwardFacing ? VertexQuad.MakeQuad(TBL, TBR, TFR, TFL, topNormal)       : VertexQuad.MakeQuad(TFL, TFR, TBR, TBL, topNormal);
                bottom =    inwardFacing ? VertexQuad.MakeQuad(BFL, BFR, BBR, BBL, bottomNormal)    : VertexQuad.MakeQuad(BBL, BBR, BFR, BFL, bottomNormal);
                front =     inwardFacing ? VertexQuad.MakeQuad(BFR, BFL, TFL, TFR, frontNormal)     : VertexQuad.MakeQuad(BFL, BFR, TFR, TFL, frontNormal);
                back =      inwardFacing ? VertexQuad.MakeQuad(BBL, BBR, TBR, TBL, backNormal)      : VertexQuad.MakeQuad(BBR, BBL, TBL, TBR, backNormal);
            }
            else
            {
                bool widthLarger = cubemapUVs == ECubemapTextureUVs.WidthLarger;
                left = inwardFacing ?
                    VertexQuad.MakeQuad(BFL, BBL, TBL, TFL, leftNormal,     ECubemapFace.NegX, widthLarger, bias) :
                    VertexQuad.MakeQuad(BBL, BFL, TFL, TBL, leftNormal,     ECubemapFace.NegX, widthLarger, bias);
                right = inwardFacing ?
                    VertexQuad.MakeQuad(BBR, BFR, TFR, TBR, rightNormal,    ECubemapFace.PosX, widthLarger, bias) : 
                    VertexQuad.MakeQuad(BFR, BBR, TBR, TFR, rightNormal,    ECubemapFace.PosX, widthLarger, bias);
                top = inwardFacing ?
                    VertexQuad.MakeQuad(TBL, TBR, TFR, TFL, topNormal,      ECubemapFace.PosY, widthLarger, bias) : 
                    VertexQuad.MakeQuad(TFL, TFR, TBR, TBL, topNormal,      ECubemapFace.PosY, widthLarger, bias);
                bottom = inwardFacing ? 
                    VertexQuad.MakeQuad(BFL, BFR, BBR, BBL, bottomNormal,   ECubemapFace.NegY, widthLarger, bias) : 
                    VertexQuad.MakeQuad(BBL, BBR, BFR, BFL, bottomNormal,   ECubemapFace.NegY, widthLarger, bias);
                front = inwardFacing ? 
                    VertexQuad.MakeQuad(BFR, BFL, TFL, TFR, frontNormal,    ECubemapFace.PosZ, widthLarger, bias) :
                    VertexQuad.MakeQuad(BFL, BFR, TFR, TFL, frontNormal,    ECubemapFace.PosZ, widthLarger, bias);
                back = inwardFacing ? 
                    VertexQuad.MakeQuad(BBL, BBR, TBR, TBL, backNormal,     ECubemapFace.NegZ, widthLarger, bias) :
                    VertexQuad.MakeQuad(BBR, BBL, TBL, TBR, backNormal,     ECubemapFace.NegZ, widthLarger, bias);
            }
            
            return PrimitiveData.FromQuads(VertexShaderDesc.PosNormTex(), left, right, top, bottom, front, back);
        }
        /// <summary>
        /// Creates a mesh representing this bounding box.
        /// </summary>
        /// <param name="includeTranslation">If true, makes mesh with minimum and maximum coordinates.
        /// If false, makes the mesh about the origin.</param>
        public PrimitiveData GetSolidMesh(bool includeTranslation)
        {
            if (includeTranslation)
                return SolidMesh(Minimum, Maximum);
            else
                return SolidMesh(-HalfExtents.Raw, HalfExtents.Raw);
        }

        /// <summary>
        /// Creates a mesh representing this bounding box.
        /// </summary>
        /// <param name="includeTranslation">If true, makes mesh with minimum and maximum coordinates.
        /// If false, makes the mesh about the origin.</param>
        public PrimitiveData GetWireframeMesh(bool includeTranslation)
        {
            if (includeTranslation)
                return WireframeMesh(Minimum, Maximum);
            else
                return WireframeMesh(-HalfExtents.Raw, HalfExtents.Raw);
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
        public Frustum AsFrustum(Matrix4 transform) => GetFrustum(HalfExtents.Raw, transform);
        #endregion

        public Box AsBox() => new Box(this);

        //Half extents is one of 8 octants that make up the box, so multiply half extent volume by 8
        public float GetVolume() =>
            HalfExtents.X * HalfExtents.Y * HalfExtents.Z * 8.0f;
        //Each half extent side is one of 4 quadrants on both sides of the box, so multiply each side area by 8
        public float GetSurfaceArea() =>
            HalfExtents.X * HalfExtents.Y * 8.0f +
            HalfExtents.Y * HalfExtents.Z * 8.0f +
            HalfExtents.Z * HalfExtents.X * 8.0f;

        /// <summary>
        /// Expands this bounding box to include the given point.
        /// </summary>
        public void Expand(Vec3 point)
        {
            Vec3 min = Vec3.ComponentMin(point, Minimum);
            Vec3 max = Vec3.ComponentMax(point, Maximum);
            Translation.Raw = (max + min) / 2.0f;
            HalfExtents.Raw = (max - min) / 2.0f;
        }
        public void Expand(BoundingBox box)
        {
            Vec3 min = Vec3.ComponentMin(box.Minimum, box.Maximum, Minimum);
            Vec3 max = Vec3.ComponentMax(box.Minimum, box.Maximum, Maximum);
            Translation.Raw = (max + min) / 2.0f;
            HalfExtents.Raw = (max - min) / 2.0f;
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
        public override EContainment Contains(BoundingBoxStruct box)
            => Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        public override EContainment Contains(BoundingBox box)
            => Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        public override EContainment Contains(Box box)
            => Collision.AABBContainsBox(Minimum, Maximum, box.HalfExtents, box.Transform.Matrix);
        public override EContainment Contains(Sphere sphere)
            => Collision.AABBContainsSphere(Minimum, Maximum, sphere.Center, sphere.Radius);
        public override EContainment Contains(Cone cone)
        {
            bool top = Contains(cone.GetTopPoint());
            bool bot = Contains(cone.GetBottomCenterPoint());
            if (top && bot)
                return EContainment.Contains;
            else if (!top && !bot)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public override EContainment Contains(Cylinder cylinder)
        {
            bool top = Contains(cylinder.GetTopCenterPoint());
            bool bot = Contains(cylinder.GetBottomCenterPoint());
            if (top && bot)
                return EContainment.Contains;
            else if (!top && !bot)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public override EContainment Contains(Capsule capsule)
        {
            Vec3 top = capsule.GetTopCenterPoint();
            Vec3 bot = capsule.GetBottomCenterPoint();
            Vec3 radiusVec = new Vec3(capsule.Radius);
            Vec3 capsuleMin = Vec3.ComponentMin(top, bot) - radiusVec;
            Vec3 capsuleMax = Vec3.ComponentMax(top, bot) + radiusVec;
            Vec3 min = Minimum;
            Vec3 max = Maximum;

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
        #endregion

        #region Static Constructors
        /// <summary>
        /// Creates a new bounding box from minimum and maximum coordinates.
        /// </summary>
        public static BoundingBox FromMinMax(Vec3 min, Vec3 max)
            => new BoundingBox((max - min) * 0.5f, (max + min) * 0.5f);
        /// <summary>
        /// Creates a new bounding box from half extents and a translation.
        /// </summary>
        public static BoundingBox FromHalfExtentsTranslation(Vec3 halfExtents, Vec3 translation)
            => new BoundingBox(halfExtents, translation);
        /// <summary>
        /// Creates a new bounding box from half extents and a translation.
        /// </summary>
        public static BoundingBox FromHalfExtentsTranslation(EventVec3 halfExtents, EventVec3 translation)
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
        public override void SetTransformMatrix(Matrix4 matrix)
            => _translation.Raw = matrix.Translation;
        public override Matrix4 GetTransformMatrix()
            => _translation.AsTranslationMatrix();
        public override Shape HardCopy()
            => FromHalfExtentsTranslation(HalfExtents.Raw, Translation.Raw);
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