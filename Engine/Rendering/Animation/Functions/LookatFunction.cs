using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering.Models;

namespace CustomEngine.Rendering.Animation
{
    public class BoneLookatFunction : AnimationFunction
    {
        protected override void Execute(AnimationContainer output, Skeleton skeleton, object[] input)
        {
            string boneName = (string)input[0];
            Bone bone = skeleton[boneName];
            object arg2 = input[1];
            Vec3 destPoint = 
                (arg2 is Vec3 ? (Vec3)arg2 : 
                (arg2 is Bone ? ((Bone)arg2).WorldMatrix.GetPoint() : 
                (arg2 is Matrix4 ? ((Matrix4)arg2).GetPoint() : 
                Vec3.Zero)));
            Vec3 sourcePoint = bone.WorldMatrix.GetPoint();
            bone.FrameState.Rotation.SetRotations(Quat.LookAt(sourcePoint, destPoint, Vec3.TransformVector(Vec3.Forward, bone.Parent.WorldMatrix)).ToEuler());
        }

        protected override List<AnimFuncValueInput> GetValueInputs()
        {
            return new List<AnimFuncValueInput>()
            {
                new AnimFuncValueInput("Bone", AnimArgType.String),
                new AnimFuncValueInput("Point", AnimArgType.Vec3, AnimArgType.Bone, AnimArgType.Matrix4),
            };
        }
        protected override List<AnimFuncValueOutput> GetValueOutputs()
        {
            return base.GetValueOutputs();
        }
    }
}
