using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Files;

namespace CustomEngine.Rendering.Models
{
    public class Bone : FileObject, IPhysicsDrivable
    {
        public Bone(string name, FrameState bindState)
        {
            Init(name, bindState);
        }
        public Bone(string name)
        {
            Init(name, new FrameState());
        }
        public Bone()
        {
            Init("NewBone", new FrameState());
        }
        private void Init(string name, FrameState bindState)
        {
            _frameState = _bindState = bindState;
            _name = name;
            _children.Added += _children_Added;
            _children.AddedRange += _children_AddedRange;
            _children.Removed += _children_Removed;
            _children.RemovedRange += _children_RemovedRange;
            _children.Inserted += _children_Inserted;
            _children.InsertedRange += _children_InsertedRange;
        }

        internal void CollectChildBones(Dictionary<string, Bone> boneCache, Skeleton owner)
        {
            _skeleton = owner;
            boneCache.Add(Name, this);
            foreach (Bone b in Children)
                b.CollectChildBones(boneCache, owner);
        }

        public void LinkSingleBindMesh(SkeletalRigidSubMesh m) { _singleBoundMeshes.Add(m); }
        public void UnlinkSingleBindMesh(SkeletalRigidSubMesh m) { _singleBoundMeshes.Remove(m); }

        private List<SkeletalRigidSubMesh> _singleBoundMeshes = new List<SkeletalRigidSubMesh>();
        private Skeleton _skeleton;
        private Bone _parent;
        private MonitoredList<Bone> _children = new MonitoredList<Bone>();
        private List<FacePoint> _influencedVertices = new List<FacePoint>();
        private FrameState _frameState, _bindState;
        private Matrix4
            //Animated transformation matrix relative to the skeleton's root bone, aka model space
            _frameMatrix = Matrix4.Identity, _inverseFrameMatrix = Matrix4.Identity,
            //Non-animated default bone position transforms, in model space
            _bindMatrix = Matrix4.Identity, _inverseBindMatrix = Matrix4.Identity,
            //Used for calculating vertex influences matrices quickly
            _vertexMatrix = Matrix4.Identity, _inverseVertexMatrix = Matrix4.Identity,
            _worldMatrix = Matrix4.Identity, _inverseWorldMatrix = Matrix4.Identity;

        public Bone Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                    _parent.Children.Remove(this);
                if (value != null)
                    value.Children.Add(this);
            }
        }
        public MonitoredList<Bone> Children { get { return _children; } }
        public SkeletalMesh Model { get { return _skeleton.Model; } }
        public FrameState FrameState { get { return _frameState; } }
        public FrameState BindState
        {
            get { return _bindState; }
            set
            {
                _bindState = value;
                CalcBindMatrix(false);
            }
        }
        public Matrix4 WorldMatrix { get { return _worldMatrix; } }
        public Matrix4 InverseWorldMatrix { get { return _inverseWorldMatrix; } }
        public Matrix4 FrameMatrix { get { return _frameMatrix; } }
        public Matrix4 BindMatrix { get { return _bindMatrix; } }
        public Matrix4 InverseFrameMatrix { get { return _inverseFrameMatrix; } }
        public Matrix4 InverseBindMatrix { get { return _inverseBindMatrix; } }
        public Matrix4 VertexMatrix { get { return _vertexMatrix; } }
        public Matrix4 InverseVertexMatrix { get { return _inverseVertexMatrix; } }
        public Skeleton Skeleton { get { return _skeleton; } }

        public PhysicsDriver PhysicsDriver
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void CalcFrameMatrix()
        {
            CalcFrameMatrix(Matrix4.Identity, Matrix4.Identity);
        }
        public void CalcFrameMatrix(Matrix4 parentMatrix, Matrix4 inverseParentMatrix)
        {
            _frameMatrix = parentMatrix * _frameState.Matrix;
            _inverseFrameMatrix = _frameState.InverseMatrix * inverseParentMatrix;

            _vertexMatrix = FrameMatrix * InverseBindMatrix;
            _inverseVertexMatrix = InverseFrameMatrix * BindMatrix;

            if (Model == null || Model.LinkedComponent == null)
            {
                _worldMatrix = _frameMatrix;
                _inverseWorldMatrix = _inverseFrameMatrix;
            }
            else
            {
                _worldMatrix = Model.LinkedComponent.WorldMatrix * _frameMatrix;
                _inverseWorldMatrix = _inverseFrameMatrix * Model.LinkedComponent.InverseWorldMatrix;
            }
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

            _vertexMatrix = FrameMatrix * InverseBindMatrix;
            _inverseVertexMatrix = InverseFrameMatrix * BindMatrix;

            if (!updateMesh)
                InfluenceAssets(true);

            foreach (Bone b in _children)
                b.CalcBindMatrix(_bindMatrix, _inverseBindMatrix, updateMesh);
        }
        /// <summary>
        /// If "influence" is false, all vertices will be unweighted from this bone.
        /// Otherwise, all vertices will be re-weighted to this bone.
        /// </summary>
        public void InfluenceAssets(bool influence)
        {

        }
        private void _children_Added(Bone item)
        {
            item._parent = this;
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
            item.CalcFrameMatrix(FrameMatrix, InverseFrameMatrix);
            _skeleton.RegenerateBoneCache();
        }
        private void _children_AddedRange(IEnumerable<Bone> items)
        {
            foreach (Bone item in items)
            {
                item._parent = this;
                item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
                item.CalcFrameMatrix(FrameMatrix, InverseFrameMatrix);
            }
            _skeleton.RegenerateBoneCache();
        }
        private void _children_Inserted(Bone item, int index)
        {
            item._parent = this;
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
            item.CalcFrameMatrix(FrameMatrix, InverseFrameMatrix);
            _skeleton.RegenerateBoneCache();
        }
        private void _children_InsertedRange(IEnumerable<Bone> items, int index)
        {
            foreach (Bone item in items)
            {
                item._parent = this;
                item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
                item.CalcFrameMatrix(FrameMatrix, InverseFrameMatrix);
            }
            _skeleton.RegenerateBoneCache();
        }
        private void _children_Removed(Bone item)
        {
            item._parent = null;
            item.CalcBindMatrix(false);
            item.CalcFrameMatrix();
            _skeleton.RegenerateBoneCache();
        }
        private void _children_RemovedRange(IEnumerable<Bone> items)
        {
            foreach (Bone item in items)
            {
                item._parent = null;
                item.CalcBindMatrix(false);
                item.CalcFrameMatrix();
            }
            _skeleton.RegenerateBoneCache();
        }        
    }
}
