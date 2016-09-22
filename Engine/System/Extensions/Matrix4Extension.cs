using CustomEngine.Rendering.Models.Meshes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Matrix4Extension
    {
        public static Matrix4 InfluenceMatrix(this Matrix4 value, List<BoneWeight> weights)
        {
            value = Matrix4.Zero;
            foreach (BoneWeight w in weights)
                if (w.Bone != null)
                    value += (w.Bone.FrameMatrix * w.Bone.InverseBindMatrix) * w.Weight;
            return value;
        }
        public static Matrix4 InverseInfluenceMatrix(this Matrix4 value, List<BoneWeight> weights)
        {
            value = Matrix4.Zero;
            foreach (BoneWeight w in weights)
                if (w.Bone != null)
                    value += (w.Bone.InverseFrameMatrix * w.Bone.BindMatrix) * w.Weight;
            return value;
        }
    }
}
