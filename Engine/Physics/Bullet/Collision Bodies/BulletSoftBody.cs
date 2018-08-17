using BulletSharp;
using BulletSharp.SoftBody;
using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics.Bullet.Shapes;

namespace TheraEngine.Physics.Bullet
{
    internal class BulletSoftBody : TSoftBody, IBulletCollisionObject
    {
        private SoftBody _body;
        public SoftBody Body
        {
            get => _body;
            set
            {
                if (_body != null)
                    _body.UserObject = null;
                _body = value;
                if (_body != null)
                    _body.UserObject = this;
            }
        }

        CollisionObject IBulletCollisionObject.CollisionObject => Body;

        #region Collision Object Implementation
        public override int IslandTag
        {
            get => Body.IslandTag;
            set => Body.IslandTag = value;
        }
        public override bool IsActive => Body.IsActive;
        public override Matrix4 WorldTransform
        {
            get => Body.WorldTransform;
            set => Body.WorldTransform = value;
        }
        public override Matrix4 InterpolationWorldTransform
        {
            get => Body.InterpolationWorldTransform;
            set => Body.InterpolationWorldTransform = value;
        }
        public override Vec3 InterpolationLinearVelocity
        {
            get => Body.InterpolationLinearVelocity;
            set => Body.InterpolationLinearVelocity = value;
        }
        public override Vec3 InterpolationAngularVelocity
        {
            get => Body.InterpolationAngularVelocity;
            set => Body.InterpolationAngularVelocity = value;
        }
        public override float HitFraction
        {
            get => Body.HitFraction;
            set => Body.HitFraction = value;
        }

        public override bool HasContactResponse
        {
            get
            {
                bool noResponse = (Body.CollisionFlags & CollisionFlags.NoContactResponse) != 0;
                if (noResponse == Body.HasContactResponse)
                    throw new Exception("Contact response values not as expected.");
                return Body.HasContactResponse;
            }
            set => Body.CollisionFlags = value ?
                    Body.CollisionFlags & ~CollisionFlags.NoContactResponse :
                    Body.CollisionFlags | CollisionFlags.NoContactResponse;
        }

        public override float Friction
        {
            get => Body.Friction;
            set => Body.Friction = value;
        }
        public override float DeactivationTime
        {
            get => Body.DeactivationTime;
            set => Body.DeactivationTime = value;
        }
        public override float ContactProcessingThreshold
        {
            get => Body.ContactProcessingThreshold;
            set => Body.ContactProcessingThreshold = value;
        }
        public override TCollisionShape CollisionShape
        {
            get => base.CollisionShape;
            set
            {
                base.CollisionShape = value;
                Body.CollisionShape = ((IBulletShape)value).Shape;
            }
        }
        public override bool IsKinematic
        {
            get
            {
                bool isKinematic = (Body.CollisionFlags & CollisionFlags.KinematicObject) != 0;
                if (isKinematic != Body.IsKinematicObject)
                    throw new Exception("Kinematic values not as expected.");
                return Body.IsKinematicObject;
            }
            set => Body.CollisionFlags = value ?
                    Body.CollisionFlags | CollisionFlags.KinematicObject :
                    Body.CollisionFlags & ~CollisionFlags.KinematicObject;
        }
        public override bool IsStatic
        {
            get
            {
                bool isStatic = (Body.CollisionFlags & CollisionFlags.StaticObject) != 0;
                if (isStatic != Body.IsStaticObject)
                    throw new Exception("Static values not as expected.");
                return Body.IsStaticObject;
            }
            set => Body.CollisionFlags = value ?
                    Body.CollisionFlags | CollisionFlags.StaticObject :
                    Body.CollisionFlags & ~CollisionFlags.StaticObject;
        }
        public override bool CustomMaterialCallback
        {
            get => (Body.CollisionFlags & CollisionFlags.CustomMaterialCallback) != 0;
            set => Body.CollisionFlags = value ?
                  Body.CollisionFlags | CollisionFlags.CustomMaterialCallback :
                  Body.CollisionFlags & ~CollisionFlags.CustomMaterialCallback;
        }
        public override float CcdSweptSphereRadius
        {
            get => Body.CcdSweptSphereRadius;
            set => Body.CcdSweptSphereRadius = value;
        }
        public override float CcdSquareMotionThreshold => Body.CcdSquareMotionThreshold;
        public override float CcdMotionThreshold
        {
            get => Body.CcdMotionThreshold;
            set => Body.CcdMotionThreshold = value;
        }
        public override Vec3 AnisotropicFriction
        {
            get => Body.AnisotropicFriction;
            set => Body.AnisotropicFriction = value;
        }
        public override EBodyActivationState ActivationState
        {
            get => (EBodyActivationState)(int)Body.ActivationState;
            set => Body.ActivationState = (ActivationState)(int)value;
        }
        public override bool MergesSimulationIslands => Body.MergesSimulationIslands;
        public override float RollingFriction
        {
            get => Body.RollingFriction;
            set => Body.RollingFriction = value;
        }
        public override float Restitution
        {
            get => Body.Restitution;
            set => Body.Restitution = value;
        }

        public override ushort CollidesWith
        {
            get => (ushort)Body.BroadphaseHandle.CollisionFilterMask;
            set => Body.BroadphaseHandle.CollisionFilterMask = (CollisionFilterGroups)value;
        }
        public override ushort CollisionGroup
        {
            get => (ushort)Body.BroadphaseHandle.CollisionFilterGroup;
            set => Body.BroadphaseHandle.CollisionFilterGroup = (CollisionFilterGroups)value;
        }
        public override Vec3 AabbMin
        {
            get => Body.BroadphaseHandle.AabbMin;
            set => Body.BroadphaseHandle.AabbMin = value;
        }
        public override Vec3 AabbMax
        {
            get => Body.BroadphaseHandle.AabbMax;
            set => Body.BroadphaseHandle.AabbMax = value;
        }
        public override Vec3 WindVelocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override float Volume => throw new NotImplementedException();

        public override float TotalMass { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Matrix4 InitialWorldTransform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        #region Soft Body Implementation

        #endregion

        public BulletSoftBody(ISoftBodyCollidable owner, SoftBodyWorldInfo info, TCollisionShape shape) : base(owner, shape)
        {
            Body = new SoftBody(info);
            CollisionShape = shape;
        }

        void IBulletCollisionObject.OnTransformChanged(Matrix4 worldTransform)
        {
            OnTransformChanged(worldTransform);
        }

        public override void Activate()
        {
            throw new NotImplementedException();
        }

        public override void Activate(bool forceActivation)
        {
            throw new NotImplementedException();
        }

        public override bool CheckCollideWith(TCollisionObject collisionObject)
        {
            throw new NotImplementedException();
        }

        public override void ForceActivationState(EBodyActivationState newState)
        {
            throw new NotImplementedException();
        }

        public override void GetWorldTransform(out Matrix4 transform)
        {
            throw new NotImplementedException();
        }

        public override bool HasAnisotropicFriction(EAnisotropicFrictionFlags frictionMode)
        {
            throw new NotImplementedException();
        }

        public override bool HasAnisotropicFriction()
        {
            throw new NotImplementedException();
        }

        public override void SetAnisotropicFriction(Vec3 anisotropicFriction)
        {
            throw new NotImplementedException();
        }

        public override void SetAnisotropicFriction(Vec3 anisotropicFriction, EAnisotropicFrictionFlags frictionMode)
        {
            throw new NotImplementedException();
        }

        public override void SetIgnoreCollisionCheck(TCollisionObject collisionObject, bool ignoreCollisionCheck)
        {
            throw new NotImplementedException();
        }
    }
}
