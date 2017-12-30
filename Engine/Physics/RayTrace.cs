using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Physics.RayTracing
{
    /// <summary>
    /// Provides information about a ray intersection.
    /// </summary>
    public class RayCollisionResult
    {
        public TCollisionObject CollisionObject { get; internal set; }
        public float HitFraction { get; internal set; }
        public Vec3 HitNormalWorld { get; internal set; }
        public Vec3 HitPointWorld { get; internal set; }
        public int ShapePart { get; internal set; }
        public int TriangleIndex { get; internal set; }
        
        internal RayCollisionResult() { }
        internal RayCollisionResult(
            TCollisionObject collisionObject,
            float hitFraction,
            Vec3 hitNormalWorld,
            Vec3 hitPointWorld,
            int shapePart, 
            int triangleIndex)
        {
            CollisionObject = collisionObject;
            HitFraction = hitFraction;
            HitNormalWorld = hitNormalWorld;
            HitPointWorld = hitPointWorld;
            ShapePart = shapePart;
            TriangleIndex = triangleIndex;
        }
    }
    /// <summary>
    /// Contains properties and methods for projecting a ray in the world and testing for intersections with collision objects.
    /// </summary>
    public abstract class RayTrace
    {
        public Vec3 StartPointWorld { get; private set; }
        public Vec3 EndPointWorld { get; private set; }
        public ushort CollisionGroup { get; private set; }
        public ushort CollidesWith { get; private set; }
        public TCollisionObject[] Ignored { get; private set; }
        public abstract bool HasHit { get; }

        public RayTrace(
            Vec3 startPointWorld,
            Vec3 endPointWorld,
            ushort collisionGroup,
            ushort collidesWith,
            params TCollisionObject[] ignored)
        {
            StartPointWorld = startPointWorld;
            EndPointWorld = endPointWorld;
            CollisionGroup = collisionGroup;
            CollidesWith = collidesWith;
            Ignored = ignored;
        }

        protected bool CanAddCommon(TCollisionObject obj)
            => obj.HasContactResponse && !Ignored.Any(x => x == obj);

        internal protected abstract void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, float hitFraction, int shapePart, int triangleIndex);
        internal protected virtual bool TestApproxCollision(int uniqueID, ushort collisionGroup, ushort collidesWith, Vec3 aabbMin, Vec3 aabbMax, object clientObject)
        {
            //if (Ignored.Any(x => x.UniqueID == uniqueID))
            //    return false;

            Vec3 dir = EndPointWorld - StartPointWorld;

            //I believe this algorithm is faster.
            if (Collision.RayIntersectsAABBDistance(StartPointWorld, dir, aabbMin, aabbMax, out float distance) && distance * distance < dir.LengthSquared)
            //if (Collision.SegmentIntersectsAABB(Start, End, aabbMin, aabbMax, out Vec3 enterPoint, out Vec3 exitPoint))
            {
                bool rayIntersectsOther = (CollisionGroup & collidesWith) == CollisionGroup;
                bool otherIntersectsRay = (collisionGroup & CollidesWith) == collisionGroup;
                if (rayIntersectsOther && otherIntersectsRay)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Performs the trace in the world and returns true if there are any collision results.
        /// </summary>
        public bool Trace() => Engine.RayTrace(this);
    }

    /// <summary>
    /// Returns a single collision result. Use RayTraceClosest if you're looking for a basic single result.
    /// </summary>
    public abstract class RayTraceSingle : RayTrace
    {
        public override bool HasHit => Result != null;

        protected RayCollisionResult Result { get; set; } = null;

        public TCollisionObject CollisionObject => Result?.CollisionObject;
        public float HitFraction => Result == null ? 1.0f : Result.HitFraction;
        public Vec3 HitNormalWorld => Result == null ? Vec3.Zero : Result.HitNormalWorld;
        public Vec3 HitPointWorld => Result == null ? Vec3.Zero : Result.HitPointWorld;
        public int ShapePart => Result == null ? -1 : Result.ShapePart;
        public int TriangleIndex => Result == null ? -1 : Result.TriangleIndex;

        public RayTraceSingle(Vec3 start, Vec3 end, ushort collisionGroupFlags, ushort collidesWithFlags, params TCollisionObject[] ignored)
            : base(start, end, collisionGroupFlags, collidesWithFlags, ignored) { }
    }

    /// <summary>
    /// Returns all intersected objects that specify collision with this ray.
    /// </summary>
    public class RayTraceMulti : RayTrace
    {
        public override bool HasHit => Results.Count != 0;
        
        public List<RayCollisionResult> Results { get; } = new List<RayCollisionResult>();

        public RayTraceMulti(Vec3 start, Vec3 end, ushort collisionGroupFlags, ushort collidesWithFlags, params TCollisionObject[] ignored) 
            : base(start, end, collisionGroupFlags, collidesWithFlags, ignored) { }

        protected internal override void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, float hitFraction, int shapePart, int triangleIndex)
        {
            if (!CanAddCommon(obj))
                return;

            if (!normalInWorldSpace)
                hitNormal = Vec3.TransformVector(hitNormal, obj.WorldTransform);

            Results.Add(new RayCollisionResult(obj, hitFraction, hitNormal, Vec3.Lerp(StartPointWorld, EndPointWorld, hitFraction), shapePart, triangleIndex));
        }
    }

    /// <summary>
    /// Returns the first intersected object that specifies collision with this ray.
    /// </summary>
    public class RayTraceClosest : RayTraceSingle
    {
        public RayTraceClosest(Vec3 start, Vec3 end, ushort collisionGroupFlags, ushort collidesWithFlags, params TCollisionObject[] ignored)
            : base(start, end, collisionGroupFlags, collidesWithFlags, ignored) { }

        protected internal override void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, float hitFraction, int shapePart, int triangleIndex)
        {
            if (!CanAddCommon(obj))
                return;

            Vec3 hitPointWorld = Vec3.Lerp(StartPointWorld, EndPointWorld, hitFraction);
            if (Result == null)
                Result = new RayCollisionResult(obj, hitFraction, hitNormal, hitPointWorld, shapePart, triangleIndex);
            else if (hitFraction < Result.HitFraction)
            {
                Result.CollisionObject = obj;
                Result.HitFraction = hitFraction;
                Result.HitNormalWorld = hitNormal;
                Result.HitPointWorld = hitPointWorld;
                Result.ShapePart = shapePart;
                Result.TriangleIndex = triangleIndex;
            }
        }
    }
}
