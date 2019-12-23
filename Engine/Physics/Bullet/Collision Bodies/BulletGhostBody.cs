using BulletSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics.Bullet.Shapes;

namespace TheraEngine.Physics.Bullet
{
    public class BulletGhostBody : TGhostBody, IBulletCollisionObject
    {
        private PairCachingGhostObject _body;
        [TSerialize(Order = 0)]
        public PairCachingGhostObject Body
        {
            get => _body;
            set
            {
                if (_body != null)
                    _body.UserObject = null;

                _body = value ?? new PairCachingGhostObject();
                _body.UserObject = this;
            }
        }

        public override List<TCollisionObject> CollectOverlappingPairs()
        {
            List<TCollisionObject> list = new List<TCollisionObject>();

            try
            {
                if (Body?.OverlappingPairs != null)
                    foreach (var obj in Body.OverlappingPairs)
                        if (obj?.UserObject is TCollisionObject tobj)
                            list.Add(tobj);
            }
            catch { }
                
            return list;
        }

        CollisionObject IBulletCollisionObject.CollisionObject => Body;
        public override void Dispose()
        {
            base.Dispose();
            Body?.Dispose();
        }

        public BulletGhostBody() : base()
        {
            Body = null;
        }
        public BulletGhostBody(TGhostBodyConstructionInfo info, TCollisionShape shape)
        {
            Body = new PairCachingGhostObject();
            Body.Activate();

            CollisionShape = shape;

            CollisionEnabled = info.CollisionEnabled;
            CollisionGroup = info.CollisionGroup;
            CollidesWith = info.CollidesWith;
            SimulatingPhysics = info.SimulatePhysics;
            SleepingEnabled = info.SleepingEnabled;
            IsKinematic = info.IsKinematic;
            CustomMaterialCallback = info.CustomMaterialCallback;
            CcdMotionThreshold = info.CcdMotionThreshold;
            DeactivationTime = info.DeactivationTime;
            CcdSweptSphereRadius = info.CcdSweptSphereRadius;
            ContactProcessingThreshold = info.ContactProcessingThreshold;

            Friction = info.Friction;
            Restitution = info.Restitution;
            RollingFriction = info.RollingFriction;
            WorldTransform = info.InitialWorldTransform;
            CustomMaterialCallback = info.CustomMaterialCallback;
        }

        #region Collision Object Implementation

        #region Properties
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
                bool hasResponse = (Body.CollisionFlags & CollisionFlags.NoContactResponse) == 0;
                if (hasResponse != Body.HasContactResponse)
                    throw new Exception("Contact response values not as expected.");
                return Body.HasContactResponse;
            }
            set => Body.CollisionFlags = value ?
                    Body.CollisionFlags & ~CollisionFlags.NoContactResponse :
                    Body.CollisionFlags | CollisionFlags.NoContactResponse;
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
                //Body.CollisionShape?.Dispose();
                Body.CollisionShape = (value as IBulletShape)?.Shape;
            }
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
            get => base.CollidesWith;
            set
            {
                //if (CollisionEnabled)
                //{
                    base.CollidesWith = value;
                    if (Body.BroadphaseHandle != null)
                        Body.BroadphaseHandle.CollisionFilterMask = (CollisionFilterGroups)value;
                //}
                //else
                //    _previousCollidesWith = value;
            }
        }
        public override ushort CollisionGroup
        {
            get => base.CollisionGroup;
            set
            {
                base.CollisionGroup = value;
                if (Body.BroadphaseHandle != null)
                    Body.BroadphaseHandle.CollisionFilterGroup = (CollisionFilterGroups)value;
            }
        }
        public override Vec3 AabbMin
        {
            get => Body.BroadphaseHandle?.AabbMin ?? Vec3.Zero;
            set
            {
                if (Body.BroadphaseHandle != null)
                    Body.BroadphaseHandle.AabbMin = value;
            }
        }
        public override Vec3 AabbMax
        {
            get => Body.BroadphaseHandle?.AabbMax ?? Vec3.Zero;
            set
            {
                if (Body.BroadphaseHandle != null)
                    Body.BroadphaseHandle.AabbMax = value;
            }
        }
        #endregion

        #region Methods
        public override void Activate()
        {
            Body.Activate();
        }
        public override void Activate(bool forceActivation)
        {
            Body.Activate(forceActivation);
        }
        public override bool CheckCollideWith(TCollisionObject collisionObject)
        {
            return Body.CheckCollideWith((collisionObject as IBulletCollisionObject)?.CollisionObject);
        }
        public override void ForceActivationState(EBodyActivationState newState)
        {
            Body.ForceActivationState((ActivationState)(int)newState);
        }
        public override void GetWorldTransform(out Matrix4 transform)
        {
            Body.GetWorldTransform(out Matrix t);
            transform = t;
        }
        public override bool HasAnisotropicFriction(EAnisotropicFrictionFlags frictionMode)
        {
            return Body.HasAnisotropicFriction((AnisotropicFrictionFlags)(int)frictionMode);
        }
        public override bool HasAnisotropicFriction()
        {
            return Body.HasAnisotropicFriction();
        }
        public override void SetAnisotropicFriction(Vec3 anisotropicFriction)
        {
            Body.SetAnisotropicFriction(anisotropicFriction);
        }
        public override void SetAnisotropicFriction(Vec3 anisotropicFriction, EAnisotropicFrictionFlags frictionMode)
        {
            Body.SetAnisotropicFriction(anisotropicFriction, (AnisotropicFrictionFlags)(int)frictionMode);
        }
        public override void SetIgnoreCollisionCheck(TCollisionObject collisionObject, bool ignoreCollisionCheck)
        {
            Body.SetIgnoreCollisionCheck((collisionObject as IBulletCollisionObject)?.CollisionObject, ignoreCollisionCheck);
        }
        #endregion

        #endregion

        void IBulletCollisionObject.OnTransformChanged(Matrix4 worldTransform)
        {
            OnTransformChanged(worldTransform);
        }
    }
}