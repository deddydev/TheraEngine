using System;
using System.Collections.Generic;
using BulletSharp;

namespace CustomEngine.Rendering.Models
{
    public class Bone : ObjectBase
    {
        public Bone(string name, FrameState bindState)
        {
            _name = name;
            _frameState = _bindState = bindState;
        }

        private string _name;
        private Bone _parent;
        private List<Bone> _children;
        private List<Bone> _socketChildren;
        private List<FacePoint> _influencedVertices;

        //frame state is the bone's transform with an animation applied.
        //bind state is the bone's default transform.
        public FrameState _frameState, _bindState;
        //frame matrix is the bone's world transform with an animation applied.
        //bind matrix is the bone's default transform from the root bone.
        public Matrix4
            _frameMatrix = Matrix4.Identity, _inverseFrameMatrix = Matrix4.Identity, 
            _bindMatrix = Matrix4.Identity, _inverseBindMatrix = Matrix4.Identity;

        public FrameState FrameState { get { return _frameState; } }
        public FrameState BindState { get { return _bindState; } }
        public Matrix4 FrameMatrix { get { return _frameMatrix; } }
        public Matrix4 BindMatrix { get { return _bindMatrix; } }
        public Matrix4 InverseFrameMatrix { get { return _inverseFrameMatrix; } }
        public Matrix4 InverseBindMatrix { get { return _inverseBindMatrix; } }

        public CollisionShape _collision;

        public void CalcFrameMatrix() { CalcFrameMatrix(Matrix4.Identity, Matrix4.Identity); }
        public void CalcFrameMatrix(Matrix4 parentMatrix, Matrix4 inverseParentMatrix)
        {
            _bindMatrix = parentMatrix * _bindState.Matrix;
            _inverseBindMatrix = _bindState.InverseMatrix * inverseParentMatrix;

            foreach (Bone b in _children)
                b.CalcFrameMatrix(_frameMatrix, _inverseFrameMatrix);
        }
        public void CalcBindMatrix(bool updateMesh)
        {
            CalcBindMatrix(Matrix4.Identity, Matrix4.Identity, updateMesh);
        }
        public void CalcBindMatrix(Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool updateMesh)
        {
            if (!updateMesh)
                InfluenceAssets(false);

            _bindMatrix = parentMatrix * _bindState.Matrix;
            _inverseBindMatrix = _bindState.InverseMatrix * inverseParentMatrix;

            if (!updateMesh)
                InfluenceAssets(false);

            foreach (Bone b in _children)
                b.CalcBindMatrix(_bindMatrix, _inverseBindMatrix, updateMesh);
        }
        public void InfluenceAssets(bool influence)
        {

        }
    }
}
