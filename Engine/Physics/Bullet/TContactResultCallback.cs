using BulletSharp;
using TheraEngine.Physics.ContactTesting;

namespace TheraEngine.Physics.Bullet
{
    internal class TContactResultCallback : ContactResultCallback
    {
        private ContactTest _handler;
        public ContactTest Handler
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

        public TContactResultCallback(ContactTest handler)
            => Handler = handler;

        public override float AddSingleResult(
            ManifoldPoint cp,
            CollisionObjectWrapper colObj0Wrap,
            int partId0,
            int index0,
            CollisionObjectWrapper colObj1Wrap,
            int partId1,
            int index1)
        {
            TContactInfo contact = BulletPhysicsWorld.CreateCollisionInfo(cp);
            var obj = ((IBulletCollisionObject)Handler.Object).CollisionObject;
            bool isOtherB = colObj0Wrap.CollisionObject == obj;
            var otherObj = (isOtherB ? colObj1Wrap.CollisionObject.UserObject : colObj0Wrap.CollisionObject.UserObject) as TCollisionObject;

            Handler.AddResult(contact, otherObj, isOtherB);

            return 1.0f;
        }
        public override bool NeedsCollision(BroadphaseProxy proxy0)
        {
            if (proxy0 is null)
                return false;
            //if (_handler.TestApproxCollision(proxy0.UniqueID, (ushort)proxy0.CollisionFilterGroup, (ushort)proxy0.CollisionFilterMask, proxy0.AabbMin, proxy0.AabbMax, proxy0.ClientObject))
                return base.NeedsCollision(proxy0);
            //return false;
        }
    }
}
