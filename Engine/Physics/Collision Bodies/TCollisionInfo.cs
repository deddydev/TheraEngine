using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Physics
{
    public class TCollisionInfo
    {
        public float Distance { get; set; }
        public bool LateralFrictionInitialized { get; set; }
        /// <summary>
        /// <para>If CFM is set to zero, the constraint will be hard. 
        /// If CFM is set to a positive value, it will be possible to violate the constraint by "pushing on it" 
        /// (for example, for contact constraints by forcing the two contacting objects together).
        /// In other words the constraint will be soft, and the softness will increase as CFM increases.</para>
        ///  <para>What is actually happening here is that the constraint is allowed to be violated by an amount proportional to CFM times the restoring force that is needed to enforce the constraint. 
        /// Note that setting CFM to a negative value can have undesirable bad effects, such as instability. Don't do it.</para>
        /// </summary>
        public bool HasContactConstraintForceMixing { get; set; }
        /// <summary>
        /// There is a mechanism to reduce joint error: during each simulation step each joint applies a special force to bring its bodies back into correct alignment. 
        /// This force is controlled by the error reduction parameter (ERP), which has a value between 0 and 1.
        ///The ERP specifies what proportion of the joint error will be fixed during the next simulation step.
        ///If ERP = 0 then no correcting force is applied and the bodies will eventually drift apart as the simulation proceeds.
        ///If ERP = 1 then the simulation will attempt to fix all joint error during the next time step.
        ///However, setting ERP = 1 is not recommended, as the joint error will not be completely fixed due to various internal approximations.
        ///A value of ERP = 0.1 to 0.8 is recommended(0.2 is the default).
        /// </summary>
        public bool HasContactErrorReductionParameter { get; set; }
        public float ContactMotion2 { get; set; }
        public float ContactMotion1 { get; set; }
        public float ContactErp { get; set; }
        public float ContactCfm { get; set; }
        public float CombinedRollingFriction { get; set; }
        public float CombinedRestitution { get; set; }
        public float CombinedFriction { get; set; }
        public float AppliedImpulseLateral2 { get; set; }
        public float AppliedImpulseLateral1 { get; set; }
        public float FrictionCfm { get; set; }
        public int Index0 { get; set; }
        public int Index1 { get; set; }
        public Vec3 LateralFrictionDir1 { get; set; }
        public Vec3 LateralFrictionDir2 { get; set; }
        public int LifeTime { get; set; }
        public Vec3 LocalPointA { get; set; }
        public Vec3 LocalPointB { get; set; }
        public Vec3 NormalWorldOnB { get; set; }
        public int PartId0 { get; set; }
        public int PartId1 { get; set; }
        public Vec3 PositionWorldOnA { get; set; }
        public Vec3 PositionWorldOnB { get; set; }
        public object UserPersistentData { get; set; }
        public float AppliedImpulse { get; set; }
    }
}
