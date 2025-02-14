﻿using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Animation
{
    public class BoneLookatFunction : AnimationFunction
    {
        protected override void Execute(AnimationTree output, ISkeleton skeleton, object[] input)
        {
            string boneName = (string)input[0];
            IBone bone = skeleton[boneName];
            object arg2 = input[1];
            Vec3 destPoint = 
                (arg2 is Vec3 ? (Vec3)arg2 : 
                (arg2 is Bone ? ((Bone)arg2).WorldMatrix.Translation : 
                (arg2 is Matrix4 ? ((Matrix4)arg2).Translation : 
                Vec3.Zero)));
            Vec3 sourcePoint = bone.WorldMatrix.Translation;
            bone.FrameState.Rotation.Value = Quat.LookAt(sourcePoint, destPoint, Vec3.TransformVector(Vec3.Forward, bone.ParentSocket.WorldMatrix));
        }

        protected override AnimFuncValueInput[] GetValueInputs()
        {
            return new AnimFuncValueInput[]
            {
                //new AnimFuncValueInput("Bone", AnimArgType.String),
                //new AnimFuncValueInput("Point", AnimArgType.Vec3, AnimArgType.Bone, AnimArgType.Matrix4),
            };
        }
        protected override AnimFuncValueOutput[] GetValueOutputs()
        {
            return base.GetValueOutputs();
        }
    }
}
