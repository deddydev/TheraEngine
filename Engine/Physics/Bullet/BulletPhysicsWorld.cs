using BulletSharp;
using System;
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
        private ConstraintSolver _constraintSolver;
        private BulletDebugDrawer _physicsDebugDrawer;

        public BulletPhysicsWorld()
        {
            _physicsBroadphase = new DbvtBroadphase();
            _collisionConfig = new DefaultCollisionConfiguration();
            _collisionDispatcher = new CollisionDispatcher(_collisionConfig);
            _constraintSolver = new SequentialImpulseConstraintSolver() { RandSeed = Seed, };
            _dynamicsWorld = new DiscreteDynamicsWorld(_collisionDispatcher, _physicsBroadphase, _constraintSolver, _collisionConfig)
            {
                Gravity = Gravity,
                DebugDrawer = _physicsDebugDrawer = new BulletDebugDrawer()
                {
                    DebugMode =
                    DebugDrawModes.DrawNormals |
                    //DebugDrawModes.DrawAabb | 
                    DebugDrawModes.DrawConstraints |
                    DebugDrawModes.DrawConstraintLimits |
                    DebugDrawModes.DrawContactPoints
                    //DebugDrawModes.DrawWireframe |
                    //DebugDrawModes.FastWireframe
                    //DebugDrawModes.DrawFeaturesText
                }
            };
            //_physicsScene.DispatchInfo.UseContinuous = true;
            //_physicsScene.DispatchInfo.AllowedCcdPenetration = 0.1f;
            _dynamicsWorld.PairCache.SetOverlapFilterCallback(new CustomOverlapFilter());

            PersistentManifold.ContactProcessed += PersistentManifold_ContactProcessed;
            PersistentManifold.ContactDestroyed += PersistentManifold_ContactDestroyed;
            ManifoldPoint.ContactAdded += ManifoldPoint_ContactAdded;
        }
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
        }
        private static void ManifoldPoint_ContactAdded(ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
        {
            TCollisionObject obj1 = colObj0Wrap.CollisionObject.UserObject as TCollisionObject;
            TCollisionObject obj2 = colObj1Wrap.CollisionObject.UserObject as TCollisionObject;
            obj1.OnCollided(obj2, CreateCollisionInfo(cp));
        }

        private static TCollisionInfo CreateCollisionInfo(ManifoldPoint point)
        {
            return new TCollisionInfo()
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
            IBulletCollisionObject b = (IBulletCollisionObject)collision;
            _dynamicsWorld.AddCollisionObject(b.CollisionObject, (short)collision.CollisionGroup, (short)collision.CollidesWith);
        }
        public override void RemoveCollisionObject(TCollisionObject collision)
        {
            IBulletCollisionObject b = (IBulletCollisionObject)collision;
            _dynamicsWorld.RemoveCollisionObject(b.CollisionObject);
        }
        private class CollisionPair
        {
            public CollisionPair(TCollisionObject body0, TCollisionObject body1)
            {
                _body0 = body0;
                _body1 = body1;
            }
            public TCollisionObject _body0, _body1;
        }
        public override void StepSimulation(float delta)
        {
            _dynamicsWorld.StepSimulation(delta, 7, Engine.RenderPeriod * Engine.TimeDilation);
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
            if (_dynamicsWorld != null)
            {
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
        }

        public override void DrawDebugWorld()
        {
            _dynamicsWorld.DebugDrawWorld();
        }
    }
}
