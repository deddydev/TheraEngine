using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

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
                    DebugDrawModes.DrawContactPoints// | 
                    //DebugDrawModes.DrawWireframe |
                    //DebugDrawModes.FastWireframe
                }
            };
            //_physicsScene.DispatchInfo.UseContinuous = true;
            //_physicsScene.DispatchInfo.AllowedCcdPenetration = 0.1f;
            _dynamicsWorld.PairCache.SetOverlapFilterCallback(new CustomOvelapFilter());

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
            PhysicsDriver driver0 = (PhysicsDriver)colObj0Wrap.CollisionObject.UserObject;
            PhysicsDriver driver1 = (PhysicsDriver)colObj1Wrap.CollisionObject.UserObject;
            driver0.ContactStarted(driver1, cp);
        }
        private class PhysicsDriverPair
        {
            public PhysicsDriverPair(PhysicsDriver driver0, PhysicsDriver driver1)
            {
                _driver0 = driver0;
                _driver1 = driver1;
            }
            public PhysicsDriver _driver0, _driver1;
        }
        public override void Destroy()
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

        public override void RayTrace(Vec3 start, Vec3 end, RayTraceResult result)
        {
            ClosestRayResultCallback c = new ClosestRayResultCallback();
            _dynamicsWorld.RayTest(start, end, c);
        }
        public override void StepSimulation(float delta)
        {
            _dynamicsWorld.StepSimulation(delta, 7, (float)(Engine.RenderPeriod * Engine.TimeDilation));
        }
        public override void Dispose()
        {
            Destroy();
        }

        public override void ShapeTrace(Matrix4 start, Matrix4 end, TCollisionShape shape, ShapeTraceResult result)
        {
            throw new NotImplementedException();
        }

        private class CustomOvelapFilter : OverlapFilterCallback
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

        #region Tracing
        /// <summary>
        /// Finds the closest ray intersection with any physics object.
        /// </summary>
        /// <returns></returns>
        public ClosestRayResultCallback RaycastClosest(Segment ray)
            => RaycastClosest(ray.StartPoint, ray.EndPoint);
        public ClosestRayResultCallback RaycastClosest(Vec3 from, Vec3 to)
        {
            Vector3 fromRef = from;
            Vector3 toRef = to;
            ClosestRayResultCallback callback = new ClosestRayResultCallback(ref fromRef, ref toRef)
            {
                CollisionFilterMask = (CollisionFilterGroups)(short)TCollisionGroup.All,
                CollisionFilterGroup = (CollisionFilterGroups)(short)TCollisionGroup.All,
            };
            _dynamicsWorld.RayTest(from, to, callback);
            return callback;
        }
        public ClosestRayResultExceptCallback RaycastClosestExcept(Segment ray, params CollisionObject[] ignore)
            => RaycastClosestExcept(ray.StartPoint, ray.EndPoint, ignore);
        public ClosestRayResultExceptCallback RaycastClosestExcept(Vec3 from, Vec3 to, params CollisionObject[] ignore)
        {
            Vector3 fromRef = from;
            Vector3 toRef = to;
            ClosestRayResultExceptCallback callback = new ClosestRayResultExceptCallback(ref fromRef, ref toRef, ignore)
            {
                CollisionFilterMask = (CollisionFilterGroups)(short)TCollisionGroup.All,
                CollisionFilterGroup = (CollisionFilterGroups)(short)TCollisionGroup.All,
            };
            _dynamicsWorld.RayTest(from, to, callback);
            return callback;
        }
        public AllHitsRayResultCallback RaycastMultiple(Segment ray)
            => RaycastMultiple(ray.StartPoint, ray.EndPoint);
        public AllHitsRayResultCallback RaycastMultiple(Vec3 from, Vec3 to)
        {
            AllHitsRayResultCallback callback = new AllHitsRayResultCallback(from, to);
            _dynamicsWorld.RayTest(from, to, callback);
            return callback;
        }
        public void ShapeCastClosest(ConvexShape s, Matrix4 start, Matrix4 end, ClosestConvexResultCallback result)
        {
            _dynamicsWorld.ConvexSweepTest(s, start, end, result);
        }
        #endregion
    }

    public class ClosestConvexResultExceptCallback : ClosestConvexResultCallback
    {
        CollisionObject[] _ignore;
        public ClosestConvexResultExceptCallback(params CollisionObject[] ignore) : base()
            => _ignore = ignore;
        public ClosestConvexResultExceptCallback(ref Vector3 from, ref Vector3 to, params CollisionObject[] ignore) : base(ref from, ref to)
            => _ignore = ignore;
        public override float AddSingleResult(LocalConvexResult convexResult, bool normalInWorldSpace)
        {
            if (_ignore.Any(x => x == convexResult.HitCollisionObject))
                return 1.0f;
            return base.AddSingleResult(convexResult, normalInWorldSpace);
        }
    }
    public class ClosestRayResultExceptCallback : ClosestRayResultCallback
    {
        CollisionObject[] _ignore;
        public ClosestRayResultExceptCallback(params CollisionObject[] ignore) : base()
            => _ignore = ignore;
        public ClosestRayResultExceptCallback(ref Vector3 from, ref Vector3 to, params CollisionObject[] ignore) : base(ref from, ref to)
            => _ignore = ignore;
        public override float AddSingleResult(LocalRayResult rayResult, bool normalInWorldSpace)
        {
            if (_ignore.Any(x => x == rayResult.CollisionObject))
                return 1.0f;
            return base.AddSingleResult(rayResult, normalInWorldSpace);
        }
    }
    //public class CustomClosestRayResultCallback : RayResultCallback
    //{
    //    public CustomClosestRayResultCallback() : base()
    //    {

    //    }
    //    public CustomClosestRayResultCallback(Vec3 rayFromWorld, Vec3 rayToWorld) : base()
    //    {

    //    }

    //    private Vec3 _rayStartWorld;
    //    private Vec3 _rayEndWorld;
    //    private Vec3 _hitPointWorld;
    //    private Vec3 _hitNormalWorld;
    //    private float _hitFraction = 1.0f;
    //    private CustomCollisionGroup
    //        _collidesWith = CustomCollisionGroup.All,
    //        //_group = CustomCollisionGroup.All,
    //        _ignore = CustomCollisionGroup.None;

    //    public Vec3 RayStartWorld { get => _rayStartWorld; set => _rayStartWorld = value; }
    //    public Vec3 RayEndWorld { get => _rayEndWorld; set => _rayEndWorld = value; }
    //    public Vec3 HitPointWorld { get => _hitPointWorld; }
    //    public Vec3 HitNormalWorld { get => _hitNormalWorld; }
    //    public CustomCollisionGroup CollidesWith { get => _collidesWith; set => _collidesWith = value; }
    //    public CustomCollisionGroup Ignore { get => _ignore; set => _ignore = value; }

    //    public override float AddSingleResult(LocalRayResult rayResult, bool normalInWorldSpace)
    //    {
    //        if (rayResult.HitFraction < _hitFraction)
    //        {
    //            CollisionObject = rayResult.CollisionObject;
    //            _hitFraction = rayResult.HitFraction;
    //            _hitNormalWorld = normalInWorldSpace ? (Vec3)rayResult.HitNormalLocal : Vec3.TransformNormal(rayResult.HitNormalLocal, rayResult.CollisionObject.WorldTransform).NormalizedFast();
    //            _hitPointWorld = Vec3.Lerp(_rayStartWorld, _rayEndWorld, _hitFraction);
    //        }

    //        return rayResult.HitFraction;
    //    }
    //    public override bool NeedsCollision(BroadphaseProxy proxy0)
    //    {
    //        CustomCollisionGroup g = (CustomCollisionGroup)(short)proxy0.CollisionFilterGroup;
    //        if ((_collidesWith & g) != 0 && (_ignore & g) == 0)
    //            return Collision.SegmentIntersectsAABB(RayStartWorld, RayEndWorld, proxy0.AabbMin, proxy0.AabbMax, out Vec3 enterPoint, out Vec3 exitPoint);
    //        return false;
    //    }
    //}
}
