using BulletSharp;
using System;
using System.ComponentModel;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics.Bullet.Shapes;

namespace TheraEngine.Physics.Bullet
{
    public class BulletRigidBody : TRigidBody, IBulletCollisionObject
    {
        private RigidBody _body;
        [TSerialize(Order = 0)]
        public RigidBody Body
        {
            get => _body;
            set
            {
                if (_body != null)
                    _body.UserObject = null;
                _body = value ?? new RigidBody(new RigidBodyConstructionInfo(0.0f, null, null));
                _body.UserObject = this;
            }
        }

        CollisionObject IBulletCollisionObject.CollisionObject => Body;
        public override void Dispose()
        {
            base.Dispose();
            Body?.Dispose();
        }

        public BulletRigidBody() : base()
        {
            Body = null;
        }
        public BulletRigidBody(TRigidBodyConstructionInfo info) : base()
        {
            TBulletMotionState state = null;

            if (info.UseMotionState)
                state = new TBulletMotionState(info.InitialWorldTransform, info.CenterOfMassOffset);

            RigidBodyConstructionInfo bulletInfo = new RigidBodyConstructionInfo(
                info.Mass, state, ((IBulletShape)info.CollisionShape).Shape, info.LocalInertia)
            {
                AdditionalDamping = info.AdditionalDamping,
                AdditionalDampingFactor = info.AdditionalDampingFactor,
                AdditionalLinearDampingThresholdSqr = info.AdditionalLinearDampingThresholdSqr,
                AngularDamping = info.AngularDamping,
                AngularSleepingThreshold = info.AngularSleepingThreshold,
                Friction = info.Friction,
                LinearDamping = info.LinearDamping,
                AdditionalAngularDampingThresholdSqr = info.AdditionalAngularDampingThresholdSqr,
                Restitution = info.Restitution,
                RollingFriction = info.RollingFriction,
                StartWorldTransform = info.InitialWorldTransform,
                LinearSleepingThreshold = info.LinearSleepingThreshold,
                AdditionalAngularDampingFactor = info.AdditionalAngularDampingFactor,
            };
            
            Body = new RigidBody(bulletInfo);
            Body.Activate();

            CollisionShape = info.CollisionShape;

            Constraints.PostAnythingAdded += Constraints_PostAnythingAdded;
            Constraints.PostAnythingRemoved += Constraints_PostAnythingRemoved;

            if (state != null)
                state.Body = this;

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
        }
        public BulletRigidBody(RigidBodyConstructionInfo info, TCollisionShape shape) : base()
        {
            Body = new RigidBody(info);
            Body.Activate();

            CollisionShape = shape;

            Constraints.PostAnythingAdded += Constraints_PostAnythingAdded;
            Constraints.PostAnythingRemoved += Constraints_PostAnythingRemoved;
        }

        private void Constraints_PostAnythingRemoved(TConstraint item)
        {
            Body.RemoveConstraintRef(((IBulletConstraint)item).Constraint);
        }

        private void Constraints_PostAnythingAdded(TConstraint item)
        {
            Body.AddConstraintRef(((IBulletConstraint)item).Constraint);
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
                Body.CollisionShape = ((IBulletShape)value).Shape;
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
                if (CollisionEnabled)
                {
                    base.CollidesWith = value;
                    if (Body.BroadphaseHandle != null)
                        Body.BroadphaseHandle.CollisionFilterMask = (CollisionFilterGroups)value;
                }
                else
                    _previousCollidesWith = value;
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

        #region Rigid Body Implementation

        #region Properties
        public override Vec3 TotalTorque => Body.TotalTorque;
        public override Vec3 TotalForce => Body.TotalForce;
        public override Quat Orientation => Body.Orientation;
        public override int ConstraintCount => Body.NumConstraintRefs;
        public override Vec3 LocalInertia => Body.LocalInertia;
        public override Vec3 LinearVelocity
        {
            get => Body.LinearVelocity;
            set => Body.LinearVelocity = value;
        }
        public override float LinearSleepingThreshold => Body.LinearSleepingThreshold;
        public override Vec3 LinearFactor
        {
            get => Body.LinearFactor;
            set => Body.LinearFactor = value;
        }
        public override float LinearDamping => Body.LinearDamping;
        public override bool IsInWorld => Body.IsInWorld;
        public override float Mass => 1.0f / Body.InvMass;
        public override Matrix4 InvInertiaTensorWorld => Body.InvInertiaTensorWorld;
        public override Vec3 InvInertiaDiagLocal
        {
            get => Body.InvInertiaDiagLocal;
            set => Body.InvInertiaDiagLocal = value;
        }
        public override Vec3 Gravity
        {
            get => Body.Gravity;
            set => Body.Gravity = value;
        }
        public override int FrictionSolverType
        {
            get => Body.FrictionSolverType;
            set => Body.FrictionSolverType = value;
        }
        public override int ContactSolverType
        {
            get => Body.ContactSolverType;
            set => Body.ContactSolverType = value;
        }
        public override Matrix4 CenterOfMassTransform
        {
            get => Body.CenterOfMassTransform;
            set => Body.CenterOfMassTransform = value;
        }
        public override Vec3 CenterOfMassPosition => Body.CenterOfMassPosition;
        public override Vec3 AngularVelocity
        {
            get => Body.AngularVelocity;
            set => Body.AngularVelocity = value;
        }
        public override float AngularSleepingThreshold => Body.AngularSleepingThreshold;
        public override bool WantsSleeping => Body.WantsSleeping;
        public override Vec3 AngularFactor
        {
            get => Body.AngularFactor;
            set => Body.AngularFactor = value;
        }
        public override float AngularDamping => Body.AngularDamping;
        #endregion

        #region Methods

        public override void ApplyCentralForce(Vec3 force)
        {
            Body.ApplyCentralForce(force);
        }

        public override void ApplyCentralImpulse(Vec3 impulse)
        {
            Body.ApplyCentralImpulse(impulse);
        }

        public override void ApplyForce(Vec3 force, Vec3 relativePosition)
        {
            Body.ApplyForce(force, relativePosition);
        }
        
        public override void ApplyImpulse(Vec3 impulse, Vec3 relativePosition)
        {
            Body.ApplyImpulse(impulse, relativePosition);
        }

        public override void ApplyTorque(Vec3 torque)
        {
            Body.ApplyTorque(torque);
        }

        public override void ApplyTorqueImpulse(Vec3 torque)
        {
            Body.ApplyTorqueImpulse(torque);
        }

        public override void ClearForces()
        {
            Body.ClearForces();
        }

        public override void GetAabb(out Vec3 aabbMin, out Vec3 aabbMax)
        {
            Body.GetAabb(out Vector3 min, out Vector3 max);
            aabbMin = min;
            aabbMax = max;
        }

        public override Vec3 GetVelocityInLocalPoint(Vec3 relativePosition)
        {
            return Body.GetVelocityInLocalPoint(relativePosition);
        }

        public override void ProceedToTransform(Matrix4 newTrans)
        {
            Body.ProceedToTransform(newTrans);
        }

        public override void SetDamping(float linearDamping, float angularDamping)
        {
            Body.SetDamping(linearDamping, angularDamping);
        }

        public override void SetMassProps(float mass, Vec3 inertia)
        {
            Body.SetMassProps(mass, inertia);
        }

        public override void SetSleepingThresholds(float linear, float angular)
        {
            Body.SetSleepingThresholds(linear, angular);
        }

        public override void Translate(Vec3 offset)
        {
            Body.Translate(offset);
        }

        #endregion

        #endregion
        
        void IBulletCollisionObject.OnTransformChanged(Matrix4 worldTransform)
        {
            OnTransformChanged(worldTransform);
        }

        [CustomMemberSerializeMethod(nameof(Body))]
        private void SerializeBody(SerializeElement node)
        {
            TBulletMotionState state = Body.MotionState as TBulletMotionState;
            node.AddAttribute("Mass", Mass);
            node.AddAttribute("UseMotionState", state != null);
            node.AddAttribute("ShapeType", Body.CollisionShape?.ShapeType ?? BroadphaseNativeType.EmptyShape);

            if (state != null)
            {
                node.AddAttribute("WorldTransform", state.WorldTransform);
                node.AddAttribute("CenterOfMassOffset", state.CenterOfMassOffset);
            }
            else
            {
                node.AddAttribute("WorldTransform", Body.WorldTransform);
            }

            CollisionShape shape = Body.CollisionShape;
            if (shape != null)
            {
                SerializeElement shapeNode = new SerializeElement() { Name = "Shape" };

                shapeNode.AddChildElementObject("Margin", shape.Margin);
                shapeNode.AddChildElementObject("LocalScaling", (Vec3)shape.LocalScaling);

                switch (Body.CollisionShape.ShapeType)
                {
                    case BroadphaseNativeType.BoxShape:
                        {
                            BoxShape boxShape = Body.CollisionShape as BoxShape;
                            Vec3 halfExtents = boxShape.HalfExtentsWithoutMargin;
                            shapeNode.AddChildElementObject("HalfExtents", halfExtents);
                        }
                        break;
                    case BroadphaseNativeType.CapsuleShape:
                        {
                            CapsuleShape capsuleShape = Body.CollisionShape as CapsuleShape;
                            int upAxis = capsuleShape.UpAxis;
                            shapeNode.AddChildElementObject("UpAxis", upAxis);
                            shapeNode.AddChildElementObject("Radius", capsuleShape.Radius);
                            shapeNode.AddChildElementObject("HalfHeight", capsuleShape.HalfHeight);
                        }
                        break;
                    case BroadphaseNativeType.ConeShape:
                        {
                            ConeShape coneShape = Body.CollisionShape as ConeShape;
                            int upAxis = coneShape.ConeUpIndex;
                            shapeNode.AddChildElementObject("UpAxis", upAxis);
                            shapeNode.AddChildElementObject("Radius", coneShape.Radius);
                            shapeNode.AddChildElementObject("Height", coneShape.Height);
                        }
                        break;
                    case BroadphaseNativeType.SphereShape:
                        {
                            SphereShape sphereShape = Body.CollisionShape as SphereShape;
                            shapeNode.AddChildElementObject("Radius", sphereShape.Radius);
                        }
                        break;
                    case BroadphaseNativeType.CylinderShape:
                        {
                            CylinderShape cylinderShape = Body.CollisionShape as CylinderShape;
                            Vec3 halfExtents = cylinderShape.HalfExtentsWithoutMargin;
                            int upAxis = cylinderShape.UpAxis;
                            shapeNode.AddChildElementObject("UpAxis", upAxis);
                            shapeNode.AddChildElementObject("Radius", cylinderShape.Radius);
                            shapeNode.AddChildElementObject("HalfExtents", halfExtents);
                        }
                        break;
                    case BroadphaseNativeType.TerrainShape:
                        HeightfieldTerrainShape terrainShape = Body.CollisionShape as HeightfieldTerrainShape;

                        break;
                }
                node.ChildElements.Add(shapeNode);
            }

            //RigidBody properties
            SerializeElement constrNode = new SerializeElement() { Name = "Construction" };
            constrNode.AddChildElementObject("AngularDamping", AngularDamping);
            constrNode.AddChildElementObject("AngularSleepingThreshold", AngularSleepingThreshold);
            constrNode.AddChildElementObject("Friction", Friction);
            constrNode.AddChildElementObject("LinearDamping", LinearDamping);
            constrNode.AddChildElementObject("LocalInertia", LocalInertia);
            constrNode.AddChildElementObject("Restitution", Restitution);
            constrNode.AddChildElementObject("RollingFriction", RollingFriction);
            constrNode.AddChildElementObject("LinearSleepingThreshold", LinearSleepingThreshold);
            node.ChildElements.Add(constrNode);

            //CollisionObject properties
            SerializeElement collNode = new SerializeElement() { Name = "CollisionObject" };
            collNode.AddChildElementObject("CollisionFlags", Body.CollisionFlags);
            collNode.AddChildElementObject("ActivationState", ActivationState);
            collNode.AddChildElementObject("DeactivationTime", DeactivationTime);
            collNode.AddChildElementObject("ContactProcessingThreshold", ContactProcessingThreshold);
            collNode.AddChildElementObject("CcdMotionThreshold", CcdMotionThreshold);
            collNode.AddChildElementObject("CcdSweptSphereRadius", CcdSweptSphereRadius);
            collNode.AddChildElementObject("AnisotropicFriction", AnisotropicFriction);
            collNode.AddChildElementObject("CollidesWith", CollidesWith);
            collNode.AddChildElementObject("CollisionGroup", CollisionGroup);
            //collNode.AddChildElementObject("AabbMin", AabbMin);
            //collNode.AddChildElementObject("AabbMax", AabbMax);
            node.ChildElements.Add(collNode);

            //writer.WriteElementString("AdditionalDamping", AdditionalDamping.ToString());
            //writer.WriteElementString("AdditionalDampingFactor", AdditionalDampingFactor.ToString());
            //writer.WriteElementString("AdditionalLinearDampingThresholdSqr", AdditionalLinearDampingThresholdSqr.ToString());
            //writer.WriteElementString("AdditionalAngularDampingFactor", AdditionalAngularDampingFactor.ToString());
            //writer.WriteElementString("AdditionalAngularDampingThresholdSqr", AdditionalAngularDampingThresholdSqr.ToString());
        }
        [CustomMemberDeserializeMethod(nameof(Body))]
        private void DeserializeBody(SerializeElement node)
        {
            float mass = 0.0f;
            bool useMotionState = false;
            Matrix4 worldTransform = Matrix4.Identity;
            Matrix4 centerOfMassOffset = Matrix4.Identity;
            TBulletMotionState motionState = null;
            CollisionShape shape = null;
            BroadphaseNativeType type = BroadphaseNativeType.BoxShape;
            RigidBodyConstructionInfo rbc = null;

            foreach (var attrib in node.Attributes)
            {
                switch (attrib.Name)
                {
                    case "Mass":
                        if (!attrib.GetObjectAs(out mass))
                            mass = 0.0f;
                        break;
                    case "UseMotionState":
                        if (!attrib.GetObjectAs(out useMotionState))
                            useMotionState = false;
                        break;
                    case "ShapeType":
                        if (!attrib.GetObjectAs(out type))
                            type = BroadphaseNativeType.BoxShape;
                        break;
                    case "WorldTransform":
                        if (!attrib.GetObjectAs(out worldTransform))
                            worldTransform = Matrix4.Identity;
                        break;
                    case "CenterOfMassOffset":
                        if (!attrib.GetObjectAs(out centerOfMassOffset))
                            centerOfMassOffset = Matrix4.Identity;
                        break;
                }
            }

            if (useMotionState)
                motionState = new TBulletMotionState(worldTransform, centerOfMassOffset) { Body = this };

            SerializeElement shapeNode = node.GetChildElement("Shape");
            SerializeElement constrNode = node.GetChildElement("Construction");
            SerializeElement collObjNode = node.GetChildElement("CollisionObject");

            if (shapeNode != null)
                shape = ParseShape(shapeNode, type);
            if (constrNode != null)
                rbc = ParseConstruction(constrNode, mass, motionState, shape);
            if (collObjNode != null)
                Body = ParseCollisionObject(collObjNode, rbc, useMotionState, worldTransform);
        }

        private RigidBody ParseCollisionObject(SerializeElement childTreeElement, RigidBodyConstructionInfo rbc, bool useMotionState, Matrix4 worldTransform)
        {
            bool hasValue;
            float value;

            Body = new RigidBody(rbc);
            if (!useMotionState)
                Body.WorldTransform = worldTransform;

            foreach (var childElement in childTreeElement.ChildElements)
            {
                switch (childElement.Name)
                {
                    case "CollisionFlags":
                        hasValue = childElement.GetElementContentAs(out CollisionFlags collFlags);
                        Body.CollisionFlags = hasValue ? collFlags : CollisionFlags.None;
                        break;
                    case "ActivationState":
                        hasValue = childElement.GetElementContentAs(out EBodyActivationState activationState);
                        ActivationState = hasValue ? activationState : EBodyActivationState.DisableSimulation;
                        break;
                    case "DeactivationTime":
                        hasValue = childElement.GetElementContentAs(out value);
                        Body.DeactivationTime = hasValue ? value : 0.0f;
                        break;
                    case "ContactProcessingThreshold":
                        hasValue = childElement.GetElementContentAs(out value);
                        Body.ContactProcessingThreshold = hasValue ? value : 0.0f;
                        break;
                    case "CcdMotionThreshold":
                        hasValue = childElement.GetElementContentAs(out value);
                        Body.CcdMotionThreshold = hasValue ? value : 0.0f;
                        break;
                    case "CcdSweptSphereRadius":
                        hasValue = childElement.GetElementContentAs(out value);
                        Body.CcdSweptSphereRadius = hasValue ? value : 0.0f;
                        break;
                    case "AnisotropicFriction":
                        hasValue = childElement.GetElementContentAs(out Vec3 anisoFric);
                        Body.AnisotropicFriction = hasValue ? anisoFric : Vec3.Zero;
                        break;
                    case "CollidesWith":
                        hasValue = childElement.GetElementContentAs(out ushort collWith);
                        CollidesWith = hasValue ? collWith : (ushort)0;
                        break;
                    case "CollisionGroup":
                        hasValue = childElement.GetElementContentAs(out ushort collGrp);
                        CollisionGroup = hasValue ? collGrp : (ushort)0;
                        break;
                }
            }
            Body?.Activate();
            return Body;
        }

        private RigidBodyConstructionInfo ParseConstruction(SerializeElement childElement, float mass, MotionState motionState, CollisionShape shape)
        {
            RigidBodyConstructionInfo rbc = new RigidBodyConstructionInfo(mass, motionState, shape);
            float value;
            bool hasValue;

            foreach (var childElement2 in childElement.ChildElements)
            {
                switch (childElement2.Name)
                {
                    case "AngularDamping":
                        hasValue = childElement2.GetElementContentAs(out value);
                        rbc.AngularDamping = hasValue ? value : 0.0f;
                        break;
                    case "AngularSleepingThreshold":
                        hasValue = childElement2.GetElementContentAs(out value);
                        rbc.AngularSleepingThreshold = hasValue ? value : 0.0f;
                        break;
                    case "Friction":
                        hasValue = childElement2.GetElementContentAs(out value);
                        rbc.Friction = hasValue ? value : 0.0f;
                        break;
                    case "LinearDamping":
                        hasValue = childElement2.GetElementContentAs(out value);
                        rbc.LinearDamping = hasValue ? value : 0.0f;
                        break;
                    case "LocalInertia":
                        hasValue = childElement2.GetElementContentAs(out Vec3 vec3Value);
                        rbc.LocalInertia = hasValue ? vec3Value : Vec3.Zero;
                        break;
                    case "Restitution":
                        hasValue = childElement2.GetElementContentAs(out value);
                        rbc.Restitution = hasValue ? value : 0.0f;
                        break;
                    case "RollingFriction":
                        hasValue = childElement2.GetElementContentAs(out value);
                        rbc.RollingFriction = hasValue ? value : 0.0f;
                        break;
                    case "LinearSleepingThreshold":
                        hasValue = childElement2.GetElementContentAs(out value);
                        rbc.LinearSleepingThreshold = hasValue ? value : 0.0f;
                        break;
                }
            }
            return rbc;
        }

        private CollisionShape ParseShape(SerializeElement childElement, BroadphaseNativeType type)
        {
            CollisionShape shape = null;

            bool hasMargin = childElement.GetChildElementObject("Margin", out float margin);
            bool hasLocalScaling = childElement.GetChildElementObject("LocalScaling", out Vec3 localScaling);

            switch (type)
            {
                case BroadphaseNativeType.SphereShape:
                    {
                        bool hasRadius = childElement.GetChildElementObject("Radius", out float radius);
                        shape = new SphereShape(radius);
                    }
                    break;
                case BroadphaseNativeType.BoxShape:
                    {
                        bool hasHalfExtents = childElement.GetChildElementObject("HalfExtents", out Vec3 halfExtents);
                        shape = new BoxShape(halfExtents.X, halfExtents.Y, halfExtents.Z);
                    }
                    break;
                case BroadphaseNativeType.CapsuleShape:
                    {
                        bool hasUpAxis = childElement.GetChildElementObject("UpAxis", out int upAxis);
                        bool hasRadius = childElement.GetChildElementObject("Radius", out float radius);
                        bool hasHeight = childElement.GetChildElementObject("HalfHeight", out float height);

                        if (!hasUpAxis)
                            upAxis = 1;
                        if (!hasHeight)
                            height = 0.5f;
                        height *= 2.0f;

                        switch (upAxis)
                        {
                            case 0:
                                shape = new CapsuleShapeX(radius, height);
                                break;
                            default:
                            case 1:
                                shape = new CapsuleShape(radius, height);
                                break;
                            case 2:
                                shape = new CapsuleShapeZ(radius, height);
                                break;
                        }
                    }
                    break;
            }
            if (shape != null)
            {
                shape.Margin = margin;
                shape.LocalScaling = localScaling;
            }
            return shape;
        }
    }
}
