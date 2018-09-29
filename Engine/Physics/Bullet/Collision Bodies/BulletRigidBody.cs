using BulletSharp;
using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Files;
using TheraEngine.Physics.Bullet.Shapes;

namespace TheraEngine.Physics.Bullet
{
    internal class BulletRigidBody : TRigidBody, IBulletCollisionObject
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
                _body = value;
                if (_body != null)
                    _body.UserObject = this;
            }
        }

        [CustomXMLSerializeMethod(nameof(Body))]
        internal bool SerializeBody(XmlWriter writer, ESerializeFlags flags)
        {
            writer.WriteStartElement(nameof(Body));
            {
                TBulletMotionState state = Body.MotionState as TBulletMotionState;
                writer.WriteAttributeString("Mass", Mass.ToString());
                writer.WriteAttributeString("UseMotionState", (state != null).ToString());
                writer.WriteAttributeString("ShapeType", Body.CollisionShape.ShapeType.ToString());

                if (state != null)
                {
                    Matrix4 mtx = state.WorldTransform;
                    Matrix4 massOff = state.CenterOfMassOffset;
                    writer.WriteAttributeString("WorldTransform", mtx.ToString(" ", "", "", " "));
                    writer.WriteAttributeString("CenterOfMassOffset", massOff.ToString(" ", "", "", " "));
                }
                else
                {
                    Matrix4 mtx = Body.WorldTransform;
                    writer.WriteAttributeString("WorldTransform", mtx.ToString(" ", "", "", " "));
                }

                CollisionShape shape = Body.CollisionShape;
                if (shape != null)
                {
                    writer.WriteStartElement("Shape");
                    {
                        writer.WriteElementString("Margin", shape.Margin.ToString());
                        writer.WriteElementString("LocalScaling", ((Vec3)shape.LocalScaling).ToString("", "", " "));

                        switch (Body.CollisionShape.ShapeType)
                        {
                            case BroadphaseNativeType.BoxShape:
                                BoxShape boxShape = Body.CollisionShape as BoxShape;
                                Vec3 halfExtents = boxShape.HalfExtentsWithoutMargin;
                                writer.WriteElementString("HalfExtents", halfExtents.ToString("", "", " "));
                                break;
                            case BroadphaseNativeType.CapsuleShape:
                                CapsuleShape capsuleShape = Body.CollisionShape as CapsuleShape;
                                int upAxis = capsuleShape.UpAxis;
                                writer.WriteElementString("UpAxis", upAxis.ToString());
                                writer.WriteElementString("Radius", capsuleShape.Radius.ToString());
                                writer.WriteElementString("HalfHeight", capsuleShape.HalfHeight.ToString());
                                break;
                            case BroadphaseNativeType.ConeShape:

                                break;
                            case BroadphaseNativeType.SphereShape:

                                break;
                            case BroadphaseNativeType.CylinderShape:

                                break;
                            case BroadphaseNativeType.TerrainShape:

                                break;
                        }
                    }
                    writer.WriteEndElement();
                }

                //RigidBody properties
                writer.WriteStartElement("Construction");
                {
                    writer.WriteElementString("AngularDamping", AngularDamping.ToString());
                    writer.WriteElementString("AngularSleepingThreshold", AngularSleepingThreshold.ToString());
                    writer.WriteElementString("Friction", Friction.ToString());
                    writer.WriteElementString("LinearDamping", LinearDamping.ToString());
                    writer.WriteElementString("LocalInertia", LocalInertia.ToString("", "", " "));
                    writer.WriteElementString("Restitution", Restitution.ToString());
                    writer.WriteElementString("RollingFriction", RollingFriction.ToString());
                    writer.WriteElementString("LinearSleepingThreshold", LinearSleepingThreshold.ToString());
                }
                writer.WriteEndElement();

                //CollisionObject properties
                writer.WriteStartElement("CollisionObject");
                writer.WriteElementString("CollisionFlags", Body.CollisionFlags.ToString().Replace(", ", "|"));
                writer.WriteElementString("ActivationState", ActivationState.ToString());
                writer.WriteElementString("DeactivationTime", DeactivationTime.ToString());
                writer.WriteElementString("ContactProcessingThreshold", ContactProcessingThreshold.ToString());
                writer.WriteElementString("CcdMotionThreshold", CcdMotionThreshold.ToString());
                writer.WriteElementString("CcdSweptSphereRadius", CcdSweptSphereRadius.ToString());
                writer.WriteElementString("AnisotropicFriction", AnisotropicFriction.ToString("", "", " "));
                writer.WriteElementString("CollidesWith", CollidesWith.ToString());
                writer.WriteElementString("CollisionGroup", CollisionGroup.ToString());
                //writer.WriteElementString("AabbMin", AabbMin.ToString("", "", " "));
                //writer.WriteElementString("AabbMax", AabbMax.ToString("", "", " "));
                writer.WriteEndElement();

                //writer.WriteElementString("AdditionalDamping", AdditionalDamping.ToString());
                //writer.WriteElementString("AdditionalDampingFactor", AdditionalDampingFactor.ToString());
                //writer.WriteElementString("AdditionalLinearDampingThresholdSqr", AdditionalLinearDampingThresholdSqr.ToString());
                //writer.WriteElementString("AdditionalAngularDampingFactor", AdditionalAngularDampingFactor.ToString());
                //writer.WriteElementString("AdditionalAngularDampingThresholdSqr", AdditionalAngularDampingThresholdSqr.ToString());
            }
            writer.WriteEndElement();
            return true;
        }
        [CustomXMLDeserializeMethod(nameof(Body))]
        internal unsafe bool DeserializeBody(XMLReader reader)
        {
            if (reader.Name != nameof(Body))
                return false;

            float mass = 0.0f;
            bool useMotionState = false;
            Matrix4 worldTransform = Matrix4.Identity;
            Matrix4 centerOfMassOffset = Matrix4.Identity;
            TBulletMotionState motionState = null;
            CollisionShape shape = null;
            BroadphaseNativeType type = BroadphaseNativeType.BoxShape;

            while (reader.ReadAttribute())
            {
                string attribName = reader.Name;
                string attribValue = reader.Value;
                switch (attribName)
                {
                    case "Mass":
                        if (!float.TryParse(attribValue, out mass))
                            mass = 0.0f;
                        break;
                    case "UseMotionState":
                        if (!bool.TryParse(attribValue, out useMotionState))
                            useMotionState = false;
                        break;
                    case "ShapeType":
                        if (!Enum.TryParse(attribValue, out type))
                            type = BroadphaseNativeType.BoxShape;
                        break;
                    case "WorldTransform":
                        worldTransform = new Matrix4();
                        worldTransform.ReadFromString(attribValue);
                        break;
                    case "CenterOfMassOffset":
                        centerOfMassOffset = new Matrix4();
                        centerOfMassOffset.ReadFromString(attribValue);
                        break;
                }
            }

            if (useMotionState)
                motionState = new TBulletMotionState(worldTransform, centerOfMassOffset) { Body = this };

            RigidBodyConstructionInfo rbc = null;
            while (reader.BeginElement())
            {
                switch (reader.Name)
                {
                    case "Shape":

                        reader.BeginElement();
                        float margin;
                        reader.ReadValue(&margin);
                        reader.EndElement();

                        reader.BeginElement();
                        string localScalingStr = reader.ReadElementString();
                        Vec3 localScaling = new Vec3(localScalingStr);
                        reader.EndElement();

                        switch (type)
                        {
                            case BroadphaseNativeType.BoxShape:

                                reader.BeginElement();
                                string halfExtentsStr = reader.ReadElementString();
                                Vec3 halfExtents = new Vec3(halfExtentsStr);
                                reader.EndElement();

                                shape = new BoxShape(halfExtents.X, halfExtents.Y, halfExtents.Z);
                                break;
                            case BroadphaseNativeType.CapsuleShape:

                                reader.BeginElement();
                                int upAxis;
                                reader.ReadValue(&upAxis);
                                reader.EndElement();

                                reader.BeginElement();
                                float radius;
                                reader.ReadValue(&radius);
                                reader.EndElement();

                                reader.BeginElement();
                                float height;
                                reader.ReadValue(&height);
                                height *= 2.0f;
                                reader.EndElement();

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
                                break;
                        }
                        shape.Margin = margin;
                        shape.LocalScaling = localScaling;
                        break;

                    case "Construction":

                        rbc = new RigidBodyConstructionInfo(mass, motionState, shape);

                        while (reader.BeginElement())
                        {
                            string name = reader.Name;
                            string value = reader.ReadElementString();
                            switch (name)
                            {
                                case "AngularDamping":
                                    rbc.AngularDamping = float.Parse(value);
                                    break;
                                case "AngularSleepingThreshold":
                                    rbc.AngularSleepingThreshold = float.Parse(value);
                                    break;
                                case "Friction":
                                    rbc.Friction = float.Parse(value);
                                    break;
                                case "LinearDamping":
                                    rbc.LinearDamping = float.Parse(value);
                                    break;
                                case "LocalInertia":
                                    rbc.LocalInertia = new Vec3(value);
                                    break;
                                case "Restitution":
                                    rbc.Restitution = float.Parse(value);
                                    break;
                                case "RollingFriction":
                                    rbc.RollingFriction = float.Parse(value);
                                    break;
                                case "LinearSleepingThreshold":
                                    rbc.LinearSleepingThreshold = float.Parse(value);
                                    break;
                            }
                            reader.EndElement();
                        }
                        break;

                    case "CollisionObject":

                        Body = new RigidBody(rbc);
                        if (!useMotionState)
                            Body.WorldTransform = worldTransform;

                        while (reader.BeginElement())
                        {
                            string name = reader.Name;
                            string value = reader.ReadElementString();
                            switch (name)
                            {
                                case "CollisionFlags":
                                    Body.CollisionFlags = value.Replace("|", ", ").AsEnum<CollisionFlags>();
                                    break;
                                case "ActivationState":
                                    ActivationState = value.AsEnum<EBodyActivationState>();
                                    break;
                                case "DeactivationTime":
                                    Body.DeactivationTime = float.Parse(value);
                                    break;
                                case "ContactProcessingThreshold":
                                    Body.ContactProcessingThreshold = float.Parse(value);
                                    break;
                                case "CcdMotionThreshold":
                                    Body.CcdMotionThreshold = float.Parse(value);
                                    break;
                                case "CcdSweptSphereRadius":
                                    Body.CcdSweptSphereRadius = float.Parse(value);
                                    break;
                                case "AnisotropicFriction":
                                    Body.AnisotropicFriction = new Vec3(value);
                                    break;
                                case "CollidesWith":
                                    CollidesWith = ushort.Parse(value);
                                    break;
                                case "CollisionGroup":
                                    CollisionGroup = ushort.Parse(value);
                                    break;
                            }
                            reader.EndElement();
                        }
                        break;
                }
                reader.EndElement();
            }
            Body?.Activate();
            return true;
        }

        CollisionObject IBulletCollisionObject.CollisionObject => Body;

        public BulletRigidBody() : base(null, null) { }
        public BulletRigidBody(IRigidBodyCollidable owner, RigidBodyConstructionInfo info, TCollisionShape shape) : base(owner, shape)
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
            get => Body.BroadphaseHandle.AabbMin;
            set => Body.BroadphaseHandle.AabbMin = value;
        }
        public override Vec3 AabbMax
        {
            get => Body.BroadphaseHandle.AabbMax;
            set => Body.BroadphaseHandle.AabbMax = value;
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
    }
}
