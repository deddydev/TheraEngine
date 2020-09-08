using System;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Logic.Common
{
    public class IKDriverComponent : LogicComponent
    {
        public IBone GoalSocket { get; set; }
        public IBone EndEffectorSocket { get; set; }
        public IBone BaseSocket { get; set; }

        public float SqrDistError { get; set; } = 0.01f;
        public float Weight { get; set; } = 1.0f;
        public int MaxIterations { get; set; } = 10;
        
        private List<IBone> SocketChain { get; } = new List<IBone>();

        protected override void OnSpawned()
        {
            base.OnSpawned();

            SocketChain.Clear();

            IBone current = EndEffectorSocket;
            while (current != null && current != BaseSocket?.Parent)
            {
                SocketChain.Add(current);
                current = current.Parent;
            }

            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Update);
        }
        protected override void OnDespawned()
        {
            base.OnDespawned();

            SocketChain.Clear();

            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Update);
        }

        private void Update(float delta)
        {
            Vec3 goalPos = GoalSocket.FrameMatrix.Translation;
            Vec3 effectorPos = EndEffectorSocket.FrameMatrix.Translation;
            Vec3 targetPos = Vec3.Lerp(effectorPos, goalPos, Weight);

            int iters = 0;

            do
            {
                for (int i = 0; i < SocketChain.Count - 2; ++i)
                {
                    for (int j = 1; j < i + 3 && j < SocketChain.Count; ++j)
                    {
                        RotateBone(EndEffectorSocket, SocketChain[j], targetPos);

                        if ((EndEffectorSocket.FrameMatrix.Translation - targetPos).LengthSquared <= SqrDistError)
                            return;
                    }
                }
            }
            while (((EndEffectorSocket.FrameMatrix.Translation - targetPos).LengthSquared) > SqrDistError && ++iters <= MaxIterations);
        }

        private void RotateBone(IBone effector, IBone bone, Vec3 goalPos)
        {
            Vec3 effectorPos = effector.FrameMatrix.Translation;
            Vec3 bonePos = bone.FrameMatrix.Translation;
            Quat boneRot = bone.FrameState.Rotation.Raw;
            Vec3 boneToEffector = effectorPos - bonePos;
            Vec3 boneToGoal = goalPos - bonePos;
            Quat fromToRot = Quat.BetweenVectors(boneToEffector, boneToGoal);
            Quat newRot = fromToRot * boneRot;
            bone.FrameState.Rotation.Raw = newRot;
        }
    }
}
