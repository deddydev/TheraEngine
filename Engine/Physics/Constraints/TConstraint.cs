using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics
{
    public enum TConstraintType
    {
        PointPoint = 3,
        Hinge = 4,
        ConeTwist = 5,
        D6 = 6,
        Slider = 7,
        Contact = 8,
        D6Spring = 9,
        Gear = 10,
        Fixed = 11,
        D6Spring2 = 12
    }
    public abstract class TConstraint : TObject
    {
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract int UniqueID { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool IsEnabled { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract TRigidBody RigidBodyB { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract TRigidBody RigidBodyA { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract int OverrideNumSolverIterations { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool NeedsFeedback { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 AppliedTorqueBodyB { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 AppliedTorqueBodyA { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 AppliedForceBodyB { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 AppliedForceBodyA { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float DebugDrawSize { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract TConstraintType ConstraintType { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float BreakingImpulseThreshold { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float AppliedImpulse { get; }
        
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract void EnableFeedback(bool needsFeedback);
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract void GetInfo1(TConstraintInfo1 info);
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract void GetInfo2(TConstraintInfo2 info);
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float GetParam(TConstraintParam num, int axis);
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float GetParam(TConstraintParam num);
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract void SetParam(TConstraintParam num, float value, int axis);
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract void SetParam(TConstraintParam num, float value);
    }
    public enum TConstraintParam
    {
        Erp = 1,
        StopErp = 2,
        Cfm = 3,
        StopCfm = 4
    }
    public class TConstraintInfo1
    {
        public int NumConstraintRows { get; set; }
        public int Nub { get; set; }
    }
    public class TConstraintInfo2
    {
        public float Damping { get; set; }
        public float Erp { get; set; }
        public float Fps { get; set; }
        public List<float> J1angularAxis { get; }
        public List<float> J1linearAxis { get; }
        public List<float> J2angularAxis { get; }
        public List<float> J2linearAxis { get; }
        public List<float> LowerLimit { get; }
        public int NumIterations { get; set; }
        public int Rowskip { get; set; }
        public List<float> UpperLimit { get; }
        public List<float> ConstraintError { get; }
        public List<float> Cfm { get; }
    }
}
