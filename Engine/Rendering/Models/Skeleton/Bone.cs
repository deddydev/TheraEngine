﻿using CustomEngine.Rendering.Models.Meshes;
using System.Collections.Generic;
using OpenTK;

namespace CustomEngine.Rendering.Models.Skeleton
{
    public class Bone
    {
        public List<Bone> _children;
        public List<Vertex> _influencedVertices;

        //frame state is the bone's transform with an animation applied.
        //bind state is the bone's default transform.
        public FrameState _frameState, _bindState;
        //frame matrix is the bone's world transform with an animation applied.
        //bind matrix is the bone's default transform from the root bone.
        public Matrix4 _frameMatrix, _bindMatrix, _inverseFrameMatrix, _inverseBindMatrix;

        public Matrix4 FrameMatrix { get { return _frameMatrix; } }
        public Matrix4 BindMatrix { get { return _bindMatrix; } }
        public Matrix4 InverseFrameMatrix { get { return _inverseFrameMatrix; } }
        public Matrix4 InverseBindMatrix { get { return _inverseBindMatrix; } }
    }
}
