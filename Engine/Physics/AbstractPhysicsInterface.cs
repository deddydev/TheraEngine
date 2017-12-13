using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Physics
{
    public abstract class AbstractPhysicsInterface
    {
        public abstract AbstractPhysicsWorld NewScene();

        public abstract TRigidBody NewRigidBody(TRigidBodyConstructionInfo info);
        public abstract TSoftBody NewSoftBody(TSoftBodyConstructionInfo info);
        
        public abstract TCollisionBox NewBox(Vec3 halfExtents);
        public abstract TCollisionSphere NewSphere(float radius);
    }
}
