using System;
using System.IO;

namespace TheraEngine.Physics
{
    public abstract class AbstractPhysicsInterface
    {
        public abstract AbstractPhysicsWorld NewScene();

        public abstract TRigidBody NewRigidBody(IRigidCollidable owner, TRigidBodyConstructionInfo info);
        public abstract TSoftBody NewSoftBody(ISoftCollidable owner, TSoftBodyConstructionInfo info);

        #region Shapes
        public abstract TCollisionBox NewBox(Vec3 halfExtents);
        public abstract TCollisionSphere NewSphere(float radius);
        public abstract TCollisionConeX NewConeX(float radius, float height);
        public abstract TCollisionConeY NewConeY(float radius, float height);
        public abstract TCollisionConeZ NewConeZ(float radius, float height);
        public abstract TCollisionCylinderX NewCylinderX(float radius, float height);
        public abstract TCollisionCylinderY NewCylinderY(float radius, float height);
        public abstract TCollisionCylinderZ NewCylinderZ(float radius, float height);
        public abstract TCollisionCapsuleX NewCapsuleX(float radius, float height);
        public abstract TCollisionCapsuleY NewCapsuleY(float radius, float height);
        public abstract TCollisionCapsuleZ NewCapsuleZ(float radius, float height);
        public abstract TCollisionHeightField NewHeightField(
            int heightStickWidth, int heightStickLength, Stream heightfieldData,
            float heightScale, float minHeight, float maxHeight,
            int upAxis, TCollisionHeightField.EHeightValueType heightDataType, bool flipQuadEdges);
        #endregion

        #region Constraints
        public abstract TPointPointConstraint NewPointPointConstraint(TRigidBody rigidBodyA, TRigidBody rigidBodyB, Vec3 pivotInA, Vec3 pivotInB);
        public abstract TPointPointConstraint NewPointPointConstraint(TRigidBody rigidBodyA, Vec3 pivotInA);
        #endregion
    }
}