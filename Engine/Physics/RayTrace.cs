using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Physics.RayTracing
{
    /// <summary>
    /// Contains properties and methods for projecting a ray in the world and testing for intersections with collision objects.
    /// </summary>
    public abstract class RayTraceResult
    {
        public Vec3 Start { get; private set; }
        public Vec3 End { get; private set; }
        public ushort CollisionGroup { get; private set; }
        public ushort CollidesWith { get; private set; }

        public RayTraceResult(Vec3 start, Vec3 end, ushort collisionGroup, ushort collidesWith)
        {
            Start = start;
            End = end;
            CollisionGroup = collisionGroup;
            CollidesWith = collidesWith;
        }

        internal protected abstract void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, float hitFraction, int shapePart, int triangleIndex);
        internal protected virtual bool TestApproxCollision(int uniqueID, ushort collisionGroup, ushort collidesWith, Vec3 aabbMin, Vec3 aabbMax, object clientObject)
        {
            Vec3 dir = End - Start;

            //I believe this algorithm is faster.
            if (Collision.RayIntersectsAABBDistance(Start, dir, aabbMin, aabbMax, out float distance) && distance * distance < dir.LengthSquared)

            //if (Collision.SegmentIntersectsAABB(Start, End, aabbMin, aabbMax, out Vec3 enterPoint, out Vec3 exitPoint))

            {
                bool rayIntersectsOther = (CollisionGroup & collidesWith) == CollisionGroup;
                bool otherIntersectsRay = (collisionGroup & CollidesWith) == collisionGroup;
                if (rayIntersectsOther && otherIntersectsRay)
                    return true;
            }
            return false;
        }
    }

    public class CollisionResult
    {
        public TCollisionObject CollisionObject { get; internal set; }
        public float HitFraction { get; internal set; }
        public Vec3 HitNormal { get; internal set; }
        public bool HitNormalInWorldSpace { get; internal set; }
        public int ShapePart { get; internal set; }
        public int TriangleIndex { get; internal set; }
        
        internal CollisionResult() { }
        internal CollisionResult(TCollisionObject collisionObject, float hitFraction, Vec3 hitNormal, bool hitNormalInWorldSpace, int shapePart, int triangleIndex)
        {
            CollisionObject = collisionObject;
            HitFraction = hitFraction;
            HitNormal = hitNormal;
            HitNormalInWorldSpace = hitNormalInWorldSpace;
            ShapePart = shapePart;
            TriangleIndex = triangleIndex;
        }
    }

    /// <summary>
    /// Returns a single collision result. Use RayTraceClosest if you're looking for a basic single result.
    /// </summary>
    public abstract class RayTraceResultSingle : RayTraceResult
    {
        public bool HasHit => Result != null;

        public CollisionResult Result { get; protected set; } = null;

        public RayTraceResultSingle(Vec3 start, Vec3 end, ushort collisionGroupFlags, ushort collidesWithFlags)
            : base(start, end, collisionGroupFlags, collidesWithFlags) { }
    }

    /// <summary>
    /// Returns all intersected objects that specify collision with this ray.
    /// </summary>
    public class RayTraceResultMulti : RayTraceResult
    {
        public bool HasHit => Result.Count != 0;
        
        public List<CollisionResult> Result { get; protected set; } = new List<CollisionResult>();

        public RayTraceResultMulti(Vec3 start, Vec3 end, ushort collisionGroupFlags, ushort collidesWithFlags) 
            : base(start, end, collisionGroupFlags, collidesWithFlags) { }

        protected internal override void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, float hitFraction, int shapePart, int triangleIndex)
        {
            Result.Add(new CollisionResult(obj, hitFraction, ));
        }
    }

    /// <summary>
    /// Returns the first intersected object that specifies collision with this ray.
    /// </summary>
    public class RayTraceClosest : RayTraceResultSingle
    {
        public RayTraceClosest(Vec3 start, Vec3 end, ushort collisionGroupFlags, ushort collidesWithFlags)
            : base(start, end, collisionGroupFlags, collidesWithFlags) { }

        protected internal override void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, float hitFraction, int shapePart, int triangleIndex)
        {
            if (Result == null)
                Result = new CollisionResult(obj, hitFraction, hitNormal, normalInWorldSpace, shapePart, triangleIndex);
            else if (hitFraction < Result.HitFraction)
            {
                Result.CollisionObject = obj;
                Result.HitFraction = hitFraction;
                Result.HitNormal = hitNormal;
                Result.HitNormalInWorldSpace = normalInWorldSpace;
                Result.ShapePart = shapePart;
                Result.TriangleIndex = triangleIndex;
            }
        }
    }
}
