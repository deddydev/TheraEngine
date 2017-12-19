using BulletSharp;
using System;
using TheraEngine.Physics.RayTracing;

namespace TheraEngine.Physics.Bullet
{
    internal class TRayResultCallback : RayResultCallback
    {
        private RayTrace _handler;

        public TRayResultCallback(RayTrace handler) => _handler = handler;
        
        public override float AddSingleResult(LocalRayResult rayResult, bool normalInWorldSpace)
        {
            TCollisionObject obj = rayResult.CollisionObject.UserObject as TCollisionObject;
            Vec3 hitNormalLocal = rayResult.HitNormalLocal;
            float hitFraction = rayResult.HitFraction;
            int shapePart = -1, triangleIndex = -1;

            if (rayResult.LocalShapeInfo != null)
            {
                shapePart = rayResult.LocalShapeInfo.ShapePart;
                triangleIndex = rayResult.LocalShapeInfo.TriangleIndex;
            }

            _handler.AddResult(obj, hitNormalLocal, normalInWorldSpace, hitFraction, shapePart, triangleIndex);

            return hitFraction;
        }
        public override bool NeedsCollision(BroadphaseProxy proxy0)
            => _handler.TestApproxCollision(proxy0.UniqueID, (ushort)proxy0.CollisionFilterGroup, (ushort)proxy0.CollisionFilterMask, (Vec3)proxy0.AabbMin, (Vec3)proxy0.AabbMax, proxy0.ClientObject);
    }
}
