﻿using BulletSharp;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics.Bullet.Constraints
{
    internal class BulletPointPointConstraint : TPointPointConstraint, IBulletConstraint
    {
        public BulletPointPointConstraint(BulletRigidBody rigidBodyA, Vec3 pivotInA)
        {
            BulletRigidBodyA = rigidBodyA;
            Constraint = new Point2PointConstraint(rigidBodyA.Body, pivotInA);
        }

        public BulletPointPointConstraint(BulletRigidBody rigidBodyA, BulletRigidBody rigidBodyB, Vec3 pivotInA, Vec3 pivotInB)
        {
            BulletRigidBodyA = rigidBodyA;
            BulletRigidBodyB = rigidBodyB;
            Constraint = new Point2PointConstraint(rigidBodyA.Body, rigidBodyB.Body, pivotInA, pivotInB);
        }

        public Point2PointConstraint Constraint { get; set; }
        TypedConstraint IBulletConstraint.Constraint => Constraint;

        public override Vec3 PivotInB
        {
            get => Constraint.PivotInB;
            set => Constraint.PivotInB = value;
        }
        public override Vec3 PivotInA
        {
            get => Constraint.PivotInA;
            set => Constraint.PivotInA = value;
        }
        public override float Tau
        {
            get => Constraint.Setting.Tau;
            set => Constraint.Setting.Tau = value;
        }
        public override float ImpulseClamp
        {
            get => Constraint.Setting.ImpulseClamp;
            set => Constraint.Setting.ImpulseClamp = value;
        }
        public override float Damping
        {
            get => Constraint.Setting.Damping;
            set => Constraint.Setting.Damping = value;
        }
        public override bool HasErrorReductionParameter => (Constraint.Flags & Point2PointFlags.Erp) != 0;
        public override bool HasConstraintForceMixing => (Constraint.Flags & Point2PointFlags.Cfm) != 0;

        public override int UniqueID => Constraint.Uid;

        public override bool IsEnabled
        {
            get => Constraint.IsEnabled;
            set => Constraint.IsEnabled = value;
        }

        public override TRigidBody RigidBodyB => BulletRigidBodyB;
        public override TRigidBody RigidBodyA => BulletRigidBodyA;
        public BulletRigidBody BulletRigidBodyB { get; }
        public BulletRigidBody BulletRigidBodyA { get; }

        public override int OverrideNumSolverIterations
        {
            get => Constraint.OverrideNumSolverIterations;
            set => Constraint.OverrideNumSolverIterations = value;
        }
        public override bool NeedsFeedback
        {
            get => Constraint.NeedsFeedback;
            //set => Constraint.NeedsFeedback = value;
        }
        public override Vec3 AppliedTorqueBodyB
        {
            get => Constraint.JointFeedback.AppliedTorqueBodyB;
            set => Constraint.JointFeedback.AppliedTorqueBodyB = value;
        }
        public override Vec3 AppliedTorqueBodyA
        {
            get => Constraint.JointFeedback.AppliedTorqueBodyA;
            set => Constraint.JointFeedback.AppliedTorqueBodyA = value;
        }
        public override Vec3 AppliedForceBodyB
        {
            get => Constraint.JointFeedback.AppliedForceBodyB;
            set => Constraint.JointFeedback.AppliedForceBodyB = value;
        }
        public override Vec3 AppliedForceBodyA
        {
            get => Constraint.JointFeedback.AppliedForceBodyA;
            set => Constraint.JointFeedback.AppliedForceBodyA = value;
        }
        public override float DebugDrawSize
        {
            get => Constraint.DebugDrawSize;
            set => Constraint.DebugDrawSize = value;
        }
        public override float BreakingImpulseThreshold
        {
            get => Constraint.BreakingImpulseThreshold;
            set => Constraint.BreakingImpulseThreshold = value;
        }
        public override float AppliedImpulse => Constraint.AppliedImpulse;
        public override void EnableFeedback(bool needsFeedback)
        {
            Constraint.EnableFeedback(needsFeedback);
        }

        public override void GetInfo1(TConstraintInfo1 info)
        {
            
        }

        public override void GetInfo2(TConstraintInfo2 info)
        {
            
        }
        
        public override float GetParam(TConstraintParam num, int axis)
        {
            return Constraint.GetParam((ConstraintParam)num, axis);
        }
        public override float GetParam(TConstraintParam num)
        {
            return Constraint.GetParam((ConstraintParam)num);
        }
        public override void SetParam(TConstraintParam num, float value, int axis)
        {
            Constraint.SetParam((ConstraintParam)num, value, axis);
        }
        public override void SetParam(TConstraintParam num, float value)
        {
            Constraint.SetParam((ConstraintParam)num, value);
        }
    }
}
