using BulletSharp;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics.RayTracing;

namespace TheraEngine.Physics.Bullet
{
    internal class TRayResultCallback : RayResultCallback
    {
        public RayTrace Handler { get; set; }

        public TRayResultCallback(RayTrace handler) => Handler = handler;
        
        public override float AddSingleResult(LocalRayResult rayResult, bool normalInWorldSpace)
        {
            TCollisionObject obj = rayResult.CollisionObject.UserObject as TCollisionObject;
            Vec3 hitNormalLocal = rayResult.HitNormalLocal;
            float hitFraction = rayResult.HitFraction;
            int shapePart = -1, triangleIndex = -1;

            //if (rayResult.LocalShapeInfo != null)
            //{
            //    shapePart = rayResult.LocalShapeInfo.ShapePart;
            //    triangleIndex = rayResult.LocalShapeInfo.TriangleIndex;
            //}

            Handler.AddResult(obj, hitNormalLocal, normalInWorldSpace, hitFraction, shapePart, triangleIndex);

            return hitFraction;
        }
        public override bool NeedsCollision(BroadphaseProxy proxy0)
            => Handler.TestApproxCollision(proxy0.UniqueID, (ushort)proxy0.CollisionFilterGroup, (ushort)proxy0.CollisionFilterMask, proxy0.AabbMin, proxy0.AabbMax, proxy0.ClientObject);
    }
}
