using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Rendering;

namespace TheraEngine.Physics
{
    public abstract class TSoftBody : TCollisionObject
    {
        public static TSoftBody New(TSoftBodyConstructionInfo info)
            => Engine.Physics.NewSoftBody(info);

        public abstract Vec3 WindVelocity { get; set; }
        public abstract float Volume { get; }
        public abstract float TotalMass { get; set; }
        public abstract float TimeAccumulator { get; set; }
        public abstract AlignedTetraArray Tetras { get; }
        public abstract object Tag { get; set; }
        public abstract SolverState SolverState { get; }
        public abstract AlignedSoftContactArray SoftContacts { get; }
        public abstract SoftBodySolver SoftBodySolver { get; set; }
        public abstract AlignedRigidContactArray RigidContacts { get; }
        public abstract Pose Pose { get; }
        public abstract SoftBodyWorldInfo WorldInfo { get; set; }
        public abstract AlignedNoteArray Notes { get; }
        public abstract AlignedNodeArray Nodes { get; }
        public abstract AlignedMaterialArray Materials { get; }
        public abstract AlignedLinkArray Links { get; }
        public abstract AlignedJointArray Joints { get; }
        public abstract Matrix4 InitialWorldTransform { get; set; }
        public abstract AlignedFaceArray Faces { get; }
        public abstract AlignedCollisionObjectArray CollisionDisabledObjects { get; }
        public abstract AlignedClusterArray Clusters { get; }
        public abstract int ClusterCount { get; }
        public abstract Config Cfg { get; }
        public abstract float RestLengthScale { get; set; }
        public abstract Vector3Array Bounds { get; }
        public abstract AlignedAnchorArray Anchors { get; }
    }
}
