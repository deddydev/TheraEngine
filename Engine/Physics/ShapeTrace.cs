using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds;

namespace TheraEngine.Physics.ShapeTracing
{
    /// <summary>
    /// Provides information about a ray intersection.
    /// </summary>
    public class ShapeCollisionResult
    {
        public TCollisionObject CollisionObject { get; internal set; }
        public float HitFraction { get; internal set; }
        public Vec3 HitNormalWorld { get; internal set; }
        public Vec3 HitPointWorld { get; internal set; }
        public int ShapePart { get; internal set; }
        public int TriangleIndex { get; internal set; }
        
        internal ShapeCollisionResult() { }
        internal ShapeCollisionResult(
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
    public abstract class ShapeTrace
    {
        public TCollisionShape Shape { get; set; }
        public Matrix4 Start { get; set; }
        public Matrix4 End { get; set; }
        public ushort CollisionGroup { get; set; }
        public ushort CollidesWith { get; set; }
        public TCollisionObject[] Ignored { get; set; }
        public float AllowedCcdPenetration { get; set; } = -1.0f;
        public abstract bool HasHit { get; }

        public ShapeTrace(TCollisionShape shape, Matrix4 start, Matrix4 end, ushort collisionGroup, ushort collidesWith, params TCollisionObject[] ignored)
        {
            Start = start;
            End = end;
            Shape = shape;
            CollisionGroup = collisionGroup;
            CollidesWith = collidesWith;
            Ignored = ignored;
        }

        protected bool CanAddCommon(TCollisionObject obj)
            => obj.HasContactResponse && !Ignored.Any(x => x == obj);
        
        internal protected abstract void AddResult(TCollisionObject obj, Vec3 hitNormalLocal, bool normalInWorldSpace, Vec3 hitPointLocal, float hitFraction, int shapePart, int triangleIndex);
        internal virtual bool TestApproxCollision(int uniqueID, ushort collisionGroup, ushort collidesWith, Vec3 aabbMin, Vec3 aabbMax, object clientObject)
        {
            bool rayIntersectsOther = (CollisionGroup & collidesWith) == CollisionGroup;
            bool otherIntersectsRay = (collisionGroup & CollidesWith) == collisionGroup;
            return rayIntersectsOther && otherIntersectsRay;
        }

        /// <summary>
        /// Performs the trace in the world and returns true if there are any collision results.
        /// </summary>
        public bool Trace(IWorld world)
            => Engine.ShapeTrace(this, world);
        
        internal abstract void Reset();
    }

    public abstract class ShapeTraceSingle : ShapeTrace
    {
        public override bool HasHit => Result != null;

        protected ShapeCollisionResult Result { get; set; } = null;
        internal override void Reset() => Result = null;

        public TCollisionObject CollisionObject => Result?.CollisionObject;
        public float HitFraction => Result is null ? 1.0f : Result.HitFraction;
        public Vec3 HitNormalWorld => Result is null ? Vec3.Zero : Result.HitNormalWorld;
        public Vec3 HitPointWorld => Result is null ? Vec3.Zero : Result.HitPointWorld;
        public int ShapePart => Result is null ? -1 : Result.ShapePart;
        public int TriangleIndex => Result is null ? -1 : Result.TriangleIndex;

        public ShapeTraceSingle(TCollisionShape shape, Matrix4 start, Matrix4 end, ushort collisionGroup, ushort collidesWith, params TCollisionObject[] ignored)
            : base(shape, start, end, collisionGroup, collidesWith, ignored) { }
    }

    public class ShapeTraceMulti : ShapeTrace
    {
        public override bool HasHit => Results.Count > 0;

        public List<ShapeCollisionResult> Results { get; } = new List<ShapeCollisionResult>();
        internal override void Reset() => Results.Clear();

        public ShapeTraceMulti(TCollisionShape shape, Matrix4 start, Matrix4 end, ushort collisionGroup, ushort collidesWith, params TCollisionObject[] ignored)
            : base(shape, start, end, collisionGroup, collidesWith, ignored) { }

        internal protected override void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, Vec3 hitPointWorld, float hitFraction, int shapePart, int triangleIndex)
        {
            if (!CanAddCommon(obj))
                return;

            if (!normalInWorldSpace)
                hitNormal = Vec3.TransformNormal(hitNormal, obj.WorldTransform);
            
            Results.Add(new ShapeCollisionResult(obj, hitFraction, hitNormal, hitPointWorld, shapePart, triangleIndex));
        }
    }

    public class ShapeTraceClosest : ShapeTraceSingle
    {
        public ShapeTraceClosest(TCollisionShape shape, Matrix4 start, Matrix4 end, ushort collisionGroup, ushort collidesWith, params TCollisionObject[] ignored)
            : base(shape, start, end, collisionGroup, collidesWith, ignored) { }

        internal protected override void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, Vec3 hitPointWorld, float hitFraction, int shapePart, int triangleIndex)
        {
            if (!CanAddCommon(obj))
                return;

            if (!normalInWorldSpace)
                hitNormal = Vec3.TransformNormal(hitNormal, obj.WorldTransform);
            
            if (Result is null)
                Result = new ShapeCollisionResult(obj, hitFraction, hitNormal, hitPointWorld, shapePart, triangleIndex);
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
