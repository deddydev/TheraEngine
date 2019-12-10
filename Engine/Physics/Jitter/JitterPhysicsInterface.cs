using System;
using System.Collections.Generic;
using System.IO;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics.Jitter
{
    public class JitterPhysicsInterface : AbstractPhysicsInterface
    {
        public override AbstractPhysicsWorld NewScene()  => new JitterPhysicsWorld();
        public override TRigidBody NewRigidBody(TRigidBodyConstructionInfo info)
        {
            throw new NotImplementedException();
        }
        public override TSoftBody NewSoftBody(TSoftBodyConstructionInfo info)
        {
            throw new NotImplementedException();
        }

        #region Constraints
        public override TPointPointConstraint NewPointPointConstraint(TRigidBody rigidBodyA, TRigidBody rigidBodyB, Vec3 pivotInA, Vec3 pivotInB)
            => throw new NotImplementedException();
        public override TPointPointConstraint NewPointPointConstraint(TRigidBody rigidBodyA, Vec3 pivotInA)
            => throw new NotImplementedException();
        #endregion

        #region Shapes
        public override TCollisionBox NewBox(Vec3 halfExtents)
            => new JitterBox(halfExtents);
        public override TCollisionSphere NewSphere(float radius)
            => throw new NotImplementedException();
            //=> new JitterSphere(radius);
        public override TCollisionConeX NewConeX(float radius, float height)
            => throw new NotImplementedException();
            //=> new JitterConeX(radius, height);
        public override TCollisionConeY NewConeY(float radius, float height)
            => throw new NotImplementedException();
            //=> new JitterConeY(radius, height);
        public override TCollisionConeZ NewConeZ(float radius, float height)
            => throw new NotImplementedException();
            //=> new JitterConeZ(radius, height);
        public override TCollisionCylinderX NewCylinderX(float radius, float height)
            => throw new NotImplementedException();
            //=> new JitterCylinderX(radius, height);
        public override TCollisionCylinderY NewCylinderY(float radius, float height)
            => throw new NotImplementedException();
            //=> new JitterCylinderY(radius, height);
        public override TCollisionCylinderZ NewCylinderZ(float radius, float height)
            => throw new NotImplementedException();
            //=> new JitterCylinderZ(radius, height);
        public override TCollisionCapsuleX NewCapsuleX(float radius, float height)
            => throw new NotImplementedException();
            //=> new JitterCapsuleX(radius, height);
        public override TCollisionCapsuleY NewCapsuleY(float radius, float height)
            => throw new NotImplementedException();
            //=> new JitterCapsuleY(radius, height);
        public override TCollisionCapsuleZ NewCapsuleZ(float radius, float height)
            => throw new NotImplementedException();
            //=> new JitterCapsuleZ(radius, height);
        public override TCollisionHeightField NewHeightField(int heightStickWidth, int heightStickLength, Stream heightfieldData, float heightScale, float minHeight, float maxHeight, int upAxis, TCollisionHeightField.EHeightValueType heightDataType, bool flipQuadEdges)
            => throw new NotImplementedException();

        public override TCollisionCompoundShape NewCompoundShape((Matrix4 localTransform, TCollisionShape shape)[] shapes)
        {
            throw new NotImplementedException();
        }

        public override TCollisionConvexHull NewConvexHull(IEnumerable<Vec3> points)
        {
            throw new NotImplementedException();
        }

        public override TGhostBody NewGhostBody(TGhostBodyConstructionInfo info) => throw new NotImplementedException();
        //=> new JitterHeightField(heightStickLength, heightStickLength, heightfieldData, heightScale, minHeight, maxHeight, upAxis, (PhyScalarType)(int)heightDataType, flipQuadEdges);
        #endregion
    }
}
