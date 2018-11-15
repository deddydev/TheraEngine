using BulletSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics.Bullet;
using TheraEngine.Physics.Bullet.Shapes;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Physics.ShapeTracing;

namespace TheraEngine.Physics
{
    public class BulletPhysicsWorld : AbstractPhysicsWorld
    {
        public const uint Seed = 513u;

        public override Vec3 Gravity
        {
            get => base.Gravity;
            set
            {
                base.Gravity = value;
                if (_dynamicsWorld != null)
                    _dynamicsWorld.Gravity = value;
            }
        }

        private DiscreteDynamicsWorld _dynamicsWorld;
        private BroadphaseInterface _physicsBroadphase;
        private CollisionConfiguration _collisionConfig;
        private CollisionDispatcher _collisionDispatcher;
        private MultiBodyConstraintSolver _constraintSolver;
        private BulletDebugDrawer _physicsDebugDrawer;

        public BulletPhysicsWorld()
        {
            _physicsBroadphase = new DbvtBroadphase();
            _collisionConfig = new SoftBodyRigidBodyCollisionConfiguration();
            _collisionDispatcher = new CollisionDispatcher(_collisionConfig)
            {
                NearCallback = NearCallback
            };
            _constraintSolver = new MultiBodyConstraintSolver() { RandSeed = Seed, };
            _dynamicsWorld = new MultiBodyDynamicsWorld(_collisionDispatcher, _physicsBroadphase, _constraintSolver, _collisionConfig)
            {
                Gravity = Gravity,
                DebugDrawer = _physicsDebugDrawer = new BulletDebugDrawer()
                {
                    DebugMode =
                    //DebugDrawModes.DrawNormals |
                    DebugDrawModes.DrawAabb | 
                    DebugDrawModes.DrawConstraints |
                    DebugDrawModes.DrawConstraintLimits 
                    //|
                    //DebugDrawModes.DrawContactPoints |
                    //DebugDrawModes.DrawWireframe
                    //DebugDrawModes.FastWireframe
                    //DebugDrawModes.DrawFeaturesText
                }
            };

            //_dynamicsWorld.DispatchInfo.DispatchFunction = DispatcherInfo.DispatchFunc.Discrete;
            //_dynamicsWorld.DispatchInfo.UseContinuous = true;
            //_dynamicsWorld.DispatchInfo.AllowedCcdPenetration = 0.1f;
            //_dynamicsWorld.PairCache.SetInternalGhostPairCallback(new CustomOverlappingPair());
            _dynamicsWorld.PairCache.SetOverlapFilterCallback(new CustomOverlapFilter());
            _dynamicsWorld.ApplySpeculativeContactRestitution = true;
            //_dynamicsWorld.SetInternalTickCallback(CustomTickCallback);

            PersistentManifold.ContactProcessed += PersistentManifold_ContactProcessed;
            PersistentManifold.ContactDestroyed += PersistentManifold_ContactDestroyed;
            ManifoldPoint.ContactAdded += ManifoldPoint_ContactAdded;
            CompoundCollisionAlgorithm.CompoundChildShapePairCallback = CompoundChildShapeCallback;
        }
        private void NearCallback(BroadphasePair collisionPair, CollisionDispatcher dispatcher, DispatcherInfo dispatchInfo)
        {
            CollisionDispatcher.DefaultNearCallback(collisionPair, dispatcher, dispatchInfo);
        }
        public void CustomTickCallback(DynamicsWorld world, float timeStep)
        {
            return;

            int numManifolds = world.Dispatcher.NumManifolds;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = world.Dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject body0 = contactManifold.Body0;
                CollisionObject body1 = contactManifold.Body1;

                int numContacts = contactManifold.NumContacts;
                for (int j = 0; j < numContacts; j++)
                {
                    ManifoldPoint pt = contactManifold.GetContactPoint(j);
                    if (pt.Distance < 0.0f)
                    {
                        Vector3 ptA = pt.PositionWorldOnA;
                        Vector3 ptB = pt.PositionWorldOnB;
                        Vector3 normalOnB = pt.NormalWorldOnB;
                    }
                }
            }
        }
        private bool CompoundChildShapeCallback(CollisionShape pShape0, CollisionShape pShape)
        {
            return true;
        }
        
        private static HashSet<TContactInfo> _contacts = new HashSet<TContactInfo>();
        private static void PersistentManifold_ContactProcessed(ManifoldPoint cp, CollisionObject body0, CollisionObject body1)
        {
            //PhysicsDriver driver0 = (PhysicsDriver)body0.UserObject;
            //PhysicsDriver driver1 = (PhysicsDriver)body1.UserObject;
            //cp.UserPersistentData = new PhysicsDriverPair(driver0, driver1);
            //driver0.ContactStarted(driver1, cp);
        }
        private static void PersistentManifold_ContactDestroyed(object userPersistantData)
        {
            //PhysicsDriverPair drivers = (PhysicsDriverPair)userPersistantData;
            //drivers._driver0.ContactEnded(drivers._driver1);
            //drivers._driver1.ContactEnded(drivers._driver0);
            //TContactInfo contact = userPersistantData as TContactInfo;
            //_contacts.Remove(contact);
        }
        private static void ManifoldPoint_ContactAdded(ManifoldPoint cp,
            CollisionObjectWrapper colObj0Wrap, int partId0, int index0,
            CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
        {
            //if (colObj0Wrap.CollisionObject.CollisionShape.ShapeType == BroadphaseNativeType.CompoundShape)
            //{
            //    CompoundShape compound = colObj0Wrap.CollisionObject.CollisionShape as CompoundShape;
            //    CollisionShape childShape = compound.GetChildShape(index0);
            //}
            //if (colObj1Wrap.CollisionObject.CollisionShape.ShapeType == BroadphaseNativeType.CompoundShape)
            //{
            //    CompoundShape compound = colObj1Wrap.CollisionObject.CollisionShape as CompoundShape;
            //    CollisionShape childShape = compound.GetChildShape(index1);
            //}

            TCollisionObject obj1 = colObj0Wrap.CollisionObject.UserObject as TCollisionObject;
            TCollisionObject obj2 = colObj1Wrap.CollisionObject.UserObject as TCollisionObject;
            TContactInfo contact = CreateCollisionInfo(cp);
            contact.BodyA = obj1;
            contact.BodyB = obj2;
            //if (_contacts.Contains(contact))
            //    return;
            //_contacts.Add(contact);
            cp.UserPersistentData = contact;
            if (obj1.HasContactResponse && obj2.HasContactResponse)
            {
                obj1.OnCollided(obj2, contact, true);
                obj2.OnCollided(obj1, contact, false);
            }
            else
            {
                obj1.OnOverlapped(obj2, contact, true);
                obj2.OnOverlapped(obj1, contact, false);
            }
        }

        private static TContactInfo CreateCollisionInfo(ManifoldPoint point)
        {
            return new TContactInfo()
            {
                AppliedImpulse = point.AppliedImpulse,
                AppliedImpulseLateral1 = point.AppliedImpulseLateral1,
                AppliedImpulseLateral2 = point.AppliedImpulseLateral2,
                LocalPointA = point.LocalPointA,
                LocalPointB = point.LocalPointB,
                LateralFrictionDir1 = point.LateralFrictionDir1,
                LateralFrictionDir2 = point.LateralFrictionDir2,
                FrictionCfm  = point.FrictionCfm,
                CombinedFriction = point.CombinedFriction,
                Distance = point.Distance,
                UserPersistentData = point.UserPersistentData,
                CombinedRollingFriction = point.CombinedRollingFriction,
                CombinedRestitution = point.CombinedRestitution,
                ContactCfm = point.ContactCfm,
                ContactErp = point.ContactErp,
                ContactMotion1 = point.ContactMotion1,
                ContactMotion2 = point.ContactMotion2,
                HasContactConstraintForceMixing = point.ContactPointFlags.HasFlag(ContactPointFlags.HasContactCfm),
                HasContactErrorReductionParameter = point.ContactPointFlags.HasFlag(ContactPointFlags.HasContactErp),
                LateralFrictionInitialized = point.ContactPointFlags.HasFlag(ContactPointFlags.LateralFrictionInitialized),
                Index0 = point.Index0,
                Index1 = point.Index1,
                LifeTime = point.LifeTime,
                NormalWorldOnB = point.NormalWorldOnB,
                PartId0 = point.PartId0,
                PartId1 = point.PartId1,
                PositionWorldOnA = point.PositionWorldOnA,
                PositionWorldOnB = point.PositionWorldOnB,
            };
        }

        public override void AddCollisionObject(TCollisionObject collision)
        {
            IBulletCollisionObject b = collision as IBulletCollisionObject;
            if (b?.CollisionObject != null)
                _dynamicsWorld.AddCollisionObject(b.CollisionObject, (short)collision.CollisionGroup, (short)collision.CollidesWith);
            else
                Engine.LogWarning("Collision object is null; cannot add to Bullet physics world.");
        }
        public override void RemoveCollisionObject(TCollisionObject collision)
        {
            IBulletCollisionObject b = collision as IBulletCollisionObject;
            if (b?.CollisionObject != null)
                _dynamicsWorld.RemoveCollisionObject(b.CollisionObject);
            else
                Engine.LogWarning("Collision object is null; nothing to remove from Bullet physics world.");
        }
        public override void StepSimulation(float delta)
        {
            _dynamicsWorld.StepSimulation(delta, 2, Engine.RenderPeriod * Engine.TimeDilation);
        }
        private class CustomOverlappingPair : OverlappingPairCallback
        {
            public override BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
            {
                throw new NotImplementedException();
            }

            public override IntPtr RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, Dispatcher dispatcher)
            {
                throw new NotImplementedException();
            }

            public override void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy0, Dispatcher dispatcher)
            {
                throw new NotImplementedException();
            }
        }
        private class CustomOverlapFilter : OverlapFilterCallback
        {
            public override bool NeedBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
            {
                if (proxy0 == null || proxy1 == null)
                    return false;

                bool collides =
                    (proxy0.CollisionFilterGroup & proxy1.CollisionFilterMask) != 0 &&
                    (proxy1.CollisionFilterGroup & proxy0.CollisionFilterMask) != 0;

                return collides;
            }
        }
        public override void UpdateAabbs()
        {
            _dynamicsWorld.UpdateAabbs();
        }
        protected override void OnUpdateSingleAabb(TCollisionObject collision)
        {
            _dynamicsWorld.UpdateSingleAabb(((IBulletCollisionObject)collision).CollisionObject);
        }
        public override bool RayTrace(RayTrace trace)
        {
            TRayResultCallback callback = new TRayResultCallback(trace);
            _dynamicsWorld.RayTest(trace.StartPointWorld, trace.EndPointWorld, callback);
            return trace.HasHit;
        }
        public override bool ShapeTrace(ShapeTrace trace)
        {
            TConvexResultCallback callback = new TConvexResultCallback(trace);
            if (trace.AllowedCcdPenetration >= 0.0f)
                _dynamicsWorld.ConvexSweepTest((ConvexShape)((IBulletShape)trace.Shape).Shape, trace.Start, trace.End, callback, trace.AllowedCcdPenetration);
            else
                _dynamicsWorld.ConvexSweepTest((ConvexShape)((IBulletShape)trace.Shape).Shape, trace.Start, trace.End, callback);
            return trace.HasHit;
        }
        public override void AddConstraint(TConstraint constraint)
        {
            _dynamicsWorld.AddConstraint(((IBulletConstraint)constraint).Constraint);
            //Engine.LogWarning("Constraint added");
        }
        public override void RemoveConstraint(TConstraint constraint)
        {
            _dynamicsWorld.RemoveConstraint(((IBulletConstraint)constraint).Constraint);
            //Engine.LogWarning("Constraint removed");
        }
        public override void Dispose()
        {
            if (_dynamicsWorld == null)
                return;
            
            //Remove and dispose of constraints
            int i;
            for (i = _dynamicsWorld.NumConstraints - 1; i >= 0; --i)
            {
                TypedConstraint constraint = _dynamicsWorld.GetConstraint(i);
                _dynamicsWorld.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            //Remove the rigidbodies from the dynamics world and delete them
            for (i = _dynamicsWorld.NumCollisionObjects - 1; i >= 0; --i)
            {
                CollisionObject obj = _dynamicsWorld.CollisionObjectArray[i];
                if (obj is RigidBody body && body.MotionState != null)
                    body.MotionState.Dispose();
                _dynamicsWorld.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            _dynamicsWorld.Dispose();
            _dynamicsWorld = null;

            _constraintSolver?.Dispose();
            _constraintSolver = null;

            _physicsBroadphase?.Dispose();
            _physicsBroadphase = null;

            _collisionDispatcher?.Dispose();
            _collisionDispatcher = null;

            _collisionConfig?.Dispose();
            _collisionConfig = null;

            _physicsDebugDrawer?.Dispose();
            _physicsDebugDrawer = null;
        }

        public override void DrawDebugWorld()
        {
            _dynamicsWorld.DebugDrawWorld();
        }
    }
}
