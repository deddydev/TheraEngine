using System;
using System.Collections.Generic;
using System.IO;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Physics
{
    public abstract class TCollisionShape : IDisposable
    {
        public abstract float Margin { get; set; }
        public abstract Vec3 LocalScaling { get; set; }

        public Sphere GetBoundingSphere()
        {
            GetBoundingSphere(out Vec3 center, out float radius);
            return new Sphere(radius, center);
        }
        public abstract void GetBoundingSphere(out Vec3 center, out float radius);
        public BoundingBox GetAabb(Matrix4 transform)
        {
            GetAabb(transform, out Vec3 aabbMin, out Vec3 aabbMax);
            return BoundingBox.FromMinMax(aabbMin, aabbMax);
        }
        public abstract void GetAabb(Matrix4 transform, out Vec3 aabbMin, out Vec3 aabbMax);
        public abstract Vec3 CalculateLocalInertia(float mass);

        public abstract void Dispose();
    }
    public abstract class TCollisionSphere : TCollisionShape
    {
        public abstract float Radius { get; set; }

        public static TCollisionSphere New(float radius)
            => Engine.Physics.NewSphere(radius);
    }
    public abstract class TCollisionBox : TCollisionShape
    {
        public abstract Vec3 HalfExtents { get; }

        public static TCollisionBox New(float halfExtentsX, float halfExtentsY, float halfExtentsZ)
            => New(new Vec3(halfExtentsX, halfExtentsY, halfExtentsZ));
        public static TCollisionBox New(Vec3 halfExtents)
            => Engine.Physics.NewBox(halfExtents);
    }

    #region Cone
    public abstract class TCollisionConeX : TCollisionShape
    {
        public abstract float Radius { get; }
        public abstract float Height { get; }
        
        public static TCollisionConeX New(float radius, float height)
            => Engine.Physics.NewConeX(radius, height);
    }
    public abstract class TCollisionConeY : TCollisionShape
    {
        public abstract float Radius { get; }
        public abstract float Height { get; }

        public static TCollisionConeY New(float radius, float height)
            => Engine.Physics.NewConeY(radius, height);
    }
    public abstract class TCollisionConeZ : TCollisionShape
    {
        public abstract float Radius { get; }
        public abstract float Height { get; }

        public static TCollisionConeZ New(float radius, float height)
            => Engine.Physics.NewConeZ(radius, height);
    }
    #endregion

    #region Cylinder
    public abstract class TCollisionCylinderX : TCollisionShape
    {
        public abstract float Radius { get; }
        public abstract float Height { get; }

        public static TCollisionCylinderX New(float radius, float height)
            => Engine.Physics.NewCylinderX(radius, height);
    }
    public abstract class TCollisionCylinderY : TCollisionShape
    {
        public abstract float Radius { get; }
        public abstract float Height { get; }

        public static TCollisionCylinderY New(float radius, float height)
            => Engine.Physics.NewCylinderY(radius, height);
    }
    public abstract class TCollisionCylinderZ : TCollisionShape
    {
        public abstract float Radius { get; }
        public abstract float Height { get; }
        
        public static TCollisionCylinderZ New(float radius, float height)
            => Engine.Physics.NewCylinderZ(radius, height);
    }
    #endregion

    #region Capsule
    public abstract class TCollisionCapsuleX : TCollisionShape
    {
        /// <summary>
        /// The radius of the upper and lower spheres, and the cylinder.
        /// </summary>
        public abstract float Radius { get; }
        /// <summary>
        /// How tall the capsule is, not including the radius on top and bottom.
        /// </summary>
        public abstract float Height { get; }
        
        /// <summary>
        /// Creates a new capsule with height aligned to the X axis.
        /// </summary>
        /// <param name="radius">The radius of the upper and lower spheres, and the cylinder.</param>
        /// <param name="height">How tall the capsule is, not including the radius on top and bottom.</param>
        /// <returns>A new capsule with height aligned to the X axis.</returns>
        public static TCollisionCapsuleX New(float radius, float height)
            => Engine.Physics.NewCapsuleX(radius, height);
    }
    public abstract class TCollisionCapsuleY : TCollisionShape
    {
        /// <summary>
        /// The radius of the upper and lower spheres, and the cylinder.
        /// </summary>
        public abstract float Radius { get; }
        /// <summary>
        /// How tall the capsule is, not including the radius on top and bottom.
        /// </summary>
        public abstract float Height { get; }

        /// <summary>
        /// Creates a new capsule with height aligned to the Y axis.
        /// </summary>
        /// <param name="radius">The radius of the upper and lower spheres, and the cylinder.</param>
        /// <param name="height">How tall the capsule is, not including the radius on top and bottom.</param>
        /// <returns>A new capsule with height aligned to the Y axis.</returns>
        public static TCollisionCapsuleY New(float radius, float height)
            => Engine.Physics.NewCapsuleY(radius, height);
    }
    public abstract class TCollisionCapsuleZ : TCollisionShape
    {
        /// <summary>
        /// The radius of the upper and lower spheres, and the cylinder.
        /// </summary>
        public abstract float Radius { get; }
        /// <summary>
        /// How tall the capsule is, not including the radius on top and bottom.
        /// </summary>
        public abstract float Height { get; }

        /// <summary>
        /// Creates a new capsule with height aligned to the Z axis.
        /// </summary>
        /// <param name="radius">The radius of the upper and lower spheres, and the cylinder.</param>
        /// <param name="height">How tall the capsule is, not including the radius on top and bottom.</param>
        /// <returns>A new capsule with height aligned to the Z axis.</returns>
        public static TCollisionCapsuleZ New(float radius, float height)
            => Engine.Physics.NewCapsuleZ(radius, height);
    }
    #endregion

    public abstract class TCollisionCompoundShape : TCollisionShape
    {
        public static TCollisionCompoundShape New((Matrix4 localTransform, TCollisionShape shape)[] shapes)
            => Engine.Physics.NewCompoundShape(shapes);
    }
    public abstract class TCollisionConvexHull : TCollisionShape
    {
        public static TCollisionConvexHull New(IEnumerable<Vec3> points)
            => Engine.Physics.NewConvexHull(points);
    }

    public abstract class TCollisionHeightField : TCollisionShape
    {
        public enum EHeightValueType
        {
            Single = 0,
            Double = 1,
            Int32 = 2,
            Int16 = 3,
            FixedPoint88 = 4,
            Byte = 5
        }
        
        public static TCollisionHeightField New(
            int heightStickWidth,
            int heightStickLength,
            Stream heightfieldData,
            float heightScale,
            float minHeight,
            float maxHeight,
            int upAxis,
            EHeightValueType heightDataType,
            bool flipQuadEdges)
            => Engine.Physics.NewHeightField(
                heightStickWidth,
                heightStickLength,
                heightfieldData,
                heightScale,
                minHeight,
                maxHeight,
                upAxis,
                heightDataType,
                flipQuadEdges);
    }
}
