using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public abstract class ShapeTraceResult
    {
        public TCollisionShape Shape { get; private set; }
        public Matrix4 Start { get; private set; }
        public Matrix4 End { get; private set; }
        public ushort CollisionGroup { get; set; }
        public ushort CollidesWith { get; set; }
        public TCollisionObject[] Ignored { get; private set; }

        public ShapeTraceResult(TCollisionShape shape, Matrix4 start, Matrix4 end, ushort collisionGroup, ushort collidesWith, params TCollisionObject[] ignored)
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
        //internal virtual bool TestApproxCollision(int uniqueID, ushort collisionFilterGroup, ushort collisionFilterMask, Vec3 aabbMin, Vec3 aabbMax, object clientObject)
        //{

        //}

        public bool PerformTrace() => Engine.ShapeTrace(this);
    }

    public abstract class ShapeTraceResultSingle : ShapeTraceResult
    {
        public bool HasHit => Result != null;

        protected ShapeCollisionResult Result { get; set; } = null;

        public TCollisionObject CollisionObject => Result?.CollisionObject;
        public float HitFraction => Result == null ? 1.0f : Result.HitFraction;
        public Vec3 HitNormalWorld => Result == null ? Vec3.Zero : Result.HitNormalWorld;
        public Vec3 HitPointWorld => Result == null ? Vec3.Zero : Result.HitPointWorld;
        public int ShapePart => Result == null ? -1 : Result.ShapePart;
        public int TriangleIndex => Result == null ? -1 : Result.TriangleIndex;

        public ShapeTraceResultSingle(TCollisionShape shape, Matrix4 start, Matrix4 end, ushort collisionGroup, ushort collidesWith, params TCollisionObject[] ignored)
            : base(shape, start, end, collisionGroup, collidesWith, ignored) { }
    }

    public class ShapeTraceResultMulti : ShapeTraceResult
    {
        public List<ShapeCollisionResult> Results { get; } = new List<ShapeCollisionResult>();

        public ShapeTraceResultMulti(TCollisionShape shape, Matrix4 start, Matrix4 end, ushort collisionGroup, ushort collidesWith, params TCollisionObject[] ignored)
            : base(shape, start, end, collisionGroup, collidesWith, ignored) { }

        internal protected override void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, Vec3 hitPointLocal, float hitFraction, int shapePart, int triangleIndex)
        {
            if (!CanAddCommon(obj))
                return;

            if (!normalInWorldSpace)
                hitNormal = Vec3.TransformNormal(hitNormal, obj.WorldTransform);

            hitPointLocal = Vec3.TransformPosition(hitPointLocal, obj.WorldTransform);

            Results.Add(new ShapeCollisionResult(obj, hitFraction, hitNormal, hitPointLocal, shapePart, triangleIndex));
        }
    }

    public class ShapeTraceResultClosest : ShapeTraceResultSingle
    {
        public ShapeTraceResultClosest(TCollisionShape shape, Matrix4 start, Matrix4 end, ushort collisionGroup, ushort collidesWith, params TCollisionObject[] ignored)
            : base(shape, start, end, collisionGroup, collidesWith, ignored) { }

        internal protected override void AddResult(TCollisionObject obj, Vec3 hitNormal, bool normalInWorldSpace, Vec3 hitPointLocal, float hitFraction, int shapePart, int triangleIndex)
        {
            if (!CanAddCommon(obj))
                return;

            if (!normalInWorldSpace)
                hitNormal = Vec3.TransformNormal(hitNormal, obj.WorldTransform);

            hitPointLocal = Vec3.TransformPosition(hitPointLocal, obj.WorldTransform);

            if (Result == null)
                Result = new ShapeCollisionResult(obj, hitFraction, hitNormal, hitPointLocal, shapePart, triangleIndex);
            else if (hitFraction < Result.HitFraction)
            {
                Result.CollisionObject = obj;
                Result.HitFraction = hitFraction;
                Result.HitNormalWorld = hitNormal;
                Result.HitPointWorld = hitPointLocal;
                Result.ShapePart = shapePart;
                Result.TriangleIndex = triangleIndex;
            }
        }
    }
}
