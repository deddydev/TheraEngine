using System;
using System.Collections.Generic;
using System.IO;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics
{
    public abstract class AbstractPhysicsInterface : TObjectSlim
    {
        /// <summary>
        /// Creates a new physics scene using the specified physics library.
        /// </summary>
        /// <returns>A new scene to populate with physics bodies and constraints.</returns>
        public abstract AbstractPhysicsWorld NewScene();

        /// <summary>
        /// Creates a new rigid body using the specified physics library.
        /// </summary>
        /// <param name="info">Construction information.</param>
        /// <returns>A new rigid body.</returns>
        public abstract TRigidBody NewRigidBody(TRigidBodyConstructionInfo info);

        public abstract TGhostBody NewGhostBody(TGhostBodyConstructionInfo info);

        /// <summary>
        /// Creates a new soft body using the specified physics library.
        /// </summary>
        /// <param name="info">Construction information.</param>
        /// <returns>A new soft body.</returns>
        public abstract TSoftBody NewSoftBody(TSoftBodyConstructionInfo info);

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

        public abstract TCollisionCompoundShape NewCompoundShape((Matrix4 localTransform, TCollisionShape shape)[] shapes);
        public abstract TCollisionConvexHull NewConvexHull(IEnumerable<Vec3> points);
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