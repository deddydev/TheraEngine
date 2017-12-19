using BulletSharp;
using System;
using TheraEngine.Physics.ShapeTracing;

namespace TheraEngine.Physics.Bullet
{
    internal class TConvexResultCallback : ConvexResultCallback
    {
        private ShapeTrace _handler;

        public TConvexResultCallback(ShapeTrace handler) => _handler = handler;
        
        public override float AddSingleResult(LocalConvexResult convexResult, bool normalInWorldSpace)
        {
            TCollisionObject obj = convexResult.HitCollisionObject.UserObject as TCollisionObject;
            Vec3 hitNormal = convexResult.HitNormalLocal;
            Vec3 hitPointLocal = convexResult.HitPointLocal;
            float hitFraction = convexResult.HitFraction;
            int shapePart = convexResult.LocalShapeInfo.ShapePart;
            int triangleIndex = convexResult.LocalShapeInfo.TriangleIndex;

            _handler.AddResult(obj, hitNormal, normalInWorldSpace, hitPointLocal, hitFraction, shapePart, triangleIndex);

            return hitFraction;
        }
        public override bool NeedsCollision(BroadphaseProxy proxy0) => base.NeedsCollision(proxy0);
            //=> _handler.TestApproxCollision(proxy0.UniqueID, (ushort)proxy0.CollisionFilterGroup, (ushort)proxy0.CollisionFilterMask, (Vec3)proxy0.AabbMin, (Vec3)proxy0.AabbMax, proxy0.ClientObject);
    }
}
