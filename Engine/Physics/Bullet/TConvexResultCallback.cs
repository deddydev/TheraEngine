using BulletSharp;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics.ShapeTracing;

namespace TheraEngine.Physics.Bullet
{
    internal class TConvexResultCallback : ConvexResultCallback
    {
        private ShapeTrace _handler;
        public ShapeTrace Handler
        {
            get => _handler;
            set
            {
                _handler = value;

                //Because we call base.NeedsCollision, we need to set base properties
                if (Handler != null)
                {
                    CollisionFilterMask = (CollisionFilterGroups)Handler.CollidesWith;
                    CollisionFilterGroup = (CollisionFilterGroups)Handler.CollisionGroup;
                }
                else
                {
                    CollisionFilterMask = (CollisionFilterGroups)0xFFFF;
                    CollisionFilterGroup = 0;
                }
            }
        }

        public TConvexResultCallback(ShapeTrace handler)
        {
            Handler = handler;
        }
        
        public override float AddSingleResult(LocalConvexResult convexResult, bool normalInWorldSpace)
        {
            TCollisionObject obj = convexResult.HitCollisionObject.UserObject as TCollisionObject;
            Vec3 hitNormal = convexResult.HitNormalLocal;
            Vec3 hitPointLocal = convexResult.HitPointLocal;
            float hitFraction = convexResult.HitFraction;

            int shapePart = -1, triangleIndex = -1;
            if (convexResult.LocalShapeInfo != null)
            {
                shapePart = convexResult.LocalShapeInfo.ShapePart;
                triangleIndex = convexResult.LocalShapeInfo.TriangleIndex;
            }

            Handler.AddResult(obj, hitNormal, normalInWorldSpace, hitPointLocal, hitFraction, shapePart, triangleIndex);

            return hitFraction;
        }
        public override bool NeedsCollision(BroadphaseProxy proxy0)
        {
            //if (_handler.TestApproxCollision(proxy0.UniqueID, (ushort)proxy0.CollisionFilterGroup, (ushort)proxy0.CollisionFilterMask, proxy0.AabbMin, proxy0.AabbMax, proxy0.ClientObject))
                return base.NeedsCollision(proxy0);
            //return false;
        }
    }
}
