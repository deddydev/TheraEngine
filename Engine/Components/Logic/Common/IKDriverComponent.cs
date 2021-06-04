using System;
using System.Collections.Generic;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Logic.Common
{
    public class IKDriverComponent : LogicComponent
    {
        public TransformComponent GoalSocket { get; set; }
        public TransformComponent EndEffectorSocket { get; set; }
        public TransformComponent BaseSocket { get; set; }

        public float SqrDistError { get; set; } = 0.01f;
        public float Weight { get; set; } = 1.0f;
        public int MaxIterations { get; set; } = 10;
        
        private List<TransformComponent> SocketChain { get; } = new List<TransformComponent>();

        protected override void OnSpawned()
        {
            base.OnSpawned();

            SocketChain.Clear();

            TransformComponent current = EndEffectorSocket;
            while (current != null && current != BaseSocket?.ParentSocket)
            {
                SocketChain.Add(current);
                current = current.ParentSocket as TransformComponent;
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
            Vec3 goalPos = GoalSocket.Transform.Translation;
            Vec3 effectorPos = EndEffectorSocket.Transform.Translation;
            Vec3 targetPos = Vec3.Lerp(effectorPos, goalPos, Weight);

            int iters = 0;

            do
            {
                for (int i = 0; i < SocketChain.Count - 2; ++i)
                {
                    for (int j = 1; j < i + 3 && j < SocketChain.Count; ++j)
                    {
                        RotateSocket(EndEffectorSocket, SocketChain[j], targetPos);

                        if ((EndEffectorSocket.Transform.Translation - targetPos).LengthSquared <= SqrDistError)
                            return;
                    }
                }
            }
            while (((EndEffectorSocket.Transform.Translation - targetPos).LengthSquared) > SqrDistError && ++iters <= MaxIterations);
        }

        private void RotateSocket(TransformComponent effector, TransformComponent bone, Vec3 goalPos)
        {
            Vec3 effectorPos = effector.Transform.Translation;
            Vec3 bonePos = bone.Transform.Translation;
            Quat boneRot = bone.Transform.Rotation.Value;
            Vec3 boneToEffector = effectorPos - bonePos;
            Vec3 boneToGoal = goalPos - bonePos;
            Quat fromToRot = Quat.BetweenVectors(boneToEffector, boneToGoal);
            Quat newRot = fromToRot * boneRot;
            bone.Transform.Rotation.Value = newRot;
        }
    }
}
