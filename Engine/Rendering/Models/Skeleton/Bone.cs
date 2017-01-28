using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class Bone : FileObject, IPhysicsDrivable, ISocket
    {
        public override ResourceType ResourceType { get { return ResourceType.Bone; } }

        public Bone(string name, FrameState bindstate, PhysicsDriverInfo info)
        {
            Init(name, bindstate, info);
        }
        public Bone(string name, FrameState bindState)
        {
            Init(name, bindState, null);
        }
        public Bone(string name)
        {
            Init(name, new FrameState(), null);
        }
        public Bone()
        {
            Init("NewBone", new FrameState(), null);
        }
        private void Init(string name, FrameState bindState, PhysicsDriverInfo info)
        {
            _frameState = _bindState = bindState;
            _name = name;

            if (info == null)
                _physicsDriver = null;
            else
                _physicsDriver = new PhysicsDriver(info, MatrixUpdate, SimulationUpdate);

            _childBones.Added += ChildBonesAdded;
            _childBones.AddedRange += ChildBonesAddedRange;
            _childBones.Removed += ChildBonesRemoved;
            _childBones.RemovedRange += ChildBonesRemovedRange;
            _childBones.Inserted += ChildBonesInserted;
            _childBones.InsertedRange += ChildBonesInsertedRange;

            _childComponents.Added += ChildComponentsAdded;
            _childComponents.AddedRange += ChildComponentsAddedRange;
            _childComponents.Removed += ChildComponentsRemoved;
            _childComponents.RemovedRange += ChildComponentsRemovedRange;
            _childComponents.Inserted += ChildComponentsInserted;
            _childComponents.InsertedRange += ChildComponentsInsertedRange;
        }

        public void MatrixUpdate(Matrix4 worldMatrix)
        {

        }
        public void SimulationUpdate(bool isSimulating)
        {

        }

        internal void CollectChildBones(Dictionary<string, Bone> boneCache, Skeleton owner)
        {
            _skeleton = owner;
            boneCache.Add(Name, this);
            foreach (Bone b in ChildBones)
                b.CollectChildBones(boneCache, owner);
        }

        public void LinkSingleBindMesh(SkeletalRigidSubMesh m) { _singleBoundMeshes.Add(m); }
        public void UnlinkSingleBindMesh(SkeletalRigidSubMesh m) { _singleBoundMeshes.Remove(m); }

        private PhysicsDriver _physicsDriver;
        private MonitoredList<Bone> _childBones = new MonitoredList<Bone>();
        private MonitoredList<SceneComponent> _childComponents = new MonitoredList<SceneComponent>();
        private List<SkeletalRigidSubMesh> _singleBoundMeshes = new List<SkeletalRigidSubMesh>();
        private Skeleton _skeleton;
        private Bone _parent;
        private List<FacePoint> _influencedVertices = new List<FacePoint>();
        private FrameState _frameState, _bindState;
        private Matrix4
            //Animated transformation matrix relative to the skeleton's root bone, aka model space
            _frameMatrix = Matrix4.Identity, _inverseFrameMatrix = Matrix4.Identity,
            //Non-animated default bone position transforms, in model space
            _bindMatrix = Matrix4.Identity, _inverseBindMatrix = Matrix4.Identity,
            //Used for calculating vertex influences matrices quickly
            _vertexMatrix = Matrix4.Identity, _vertexMatrixIT = Matrix4.Identity,
            _worldMatrix = Matrix4.Identity, _inverseWorldMatrix = Matrix4.Identity;

        public Bone Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                    _parent.ChildBones.Remove(this);
                if (value != null)
                    value.ChildBones.Add(this);
            }
        }
        public MonitoredList<SceneComponent> ChildComponents { get { return _childComponents; } }
        public MonitoredList<Bone> ChildBones { get { return _childBones; } }
        public SkeletalMeshComponent OwningComponent { get { return _skeleton == null ? null : _skeleton.OwningComponent; } }
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
        public Matrix4 VertexMatrixIT { get { return _vertexMatrixIT; } }
        public Skeleton Skeleton { get { return _skeleton; } }
        public PhysicsDriver PhysicsDriver { get { return _physicsDriver; } }

        public void CalcFrameMatrix()
        {
            CalcFrameMatrix(Matrix4.Identity, Matrix4.Identity);
        }
        public void CalcFrameMatrix(Matrix4 parentMatrix, Matrix4 inverseParentMatrix)
        {
            _frameMatrix = parentMatrix * _frameState.Matrix;
            _inverseFrameMatrix = _frameState.InverseMatrix * inverseParentMatrix;

            _vertexMatrix = FrameMatrix * InverseBindMatrix;
            _vertexMatrixIT = InverseFrameMatrix * BindMatrix;
            _vertexMatrixIT.Transpose();

            if (OwningComponent == null)
            {
                _worldMatrix = _frameMatrix;
                _inverseWorldMatrix = _inverseFrameMatrix;
            }
            else
            {
                _worldMatrix = OwningComponent.WorldMatrix * _frameMatrix;
                _inverseWorldMatrix = _inverseFrameMatrix * OwningComponent.InverseWorldMatrix;
            }

            foreach (Bone b in _childBones)
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
            _vertexMatrixIT = InverseFrameMatrix * BindMatrix;
            _vertexMatrixIT.Transpose();

            if (!updateMesh)
                InfluenceAssets(true);

            foreach (Bone b in _childBones)
                b.CalcBindMatrix(_bindMatrix, _inverseBindMatrix, updateMesh);
        }
        /// <summary>
        /// If "influence" is false, all vertices will be unweighted from this bone.
        /// Otherwise, all vertices will be re-weighted to this bone.
        /// </summary>
        public void InfluenceAssets(bool influence)
        {

        }
        private void ChildBonesAdded(Bone item)
        {
            item._parent = this;
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
            item.CalcFrameMatrix(FrameMatrix, InverseFrameMatrix);
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesAddedRange(IEnumerable<Bone> items)
        {
            foreach (Bone item in items)
            {
                item._parent = this;
                item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
                item.CalcFrameMatrix(FrameMatrix, InverseFrameMatrix);
            }
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesInserted(Bone item, int index)
        {
            item._parent = this;
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
            item.CalcFrameMatrix(FrameMatrix, InverseFrameMatrix);
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesInsertedRange(IEnumerable<Bone> items, int index)
        {
            foreach (Bone item in items)
            {
                item._parent = this;
                item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
                item.CalcFrameMatrix(FrameMatrix, InverseFrameMatrix);
            }
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesRemoved(Bone item)
        {
            item._parent = null;
            item.CalcBindMatrix(false);
            item.CalcFrameMatrix();
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesRemovedRange(IEnumerable<Bone> items)
        {
            foreach (Bone item in items)
            {
                item._parent = null;
                item.CalcBindMatrix(false);
                item.CalcFrameMatrix();
            }
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildComponentsAdded(SceneComponent item)
        {
            item._parent = this;
        }
        private void ChildComponentsAddedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
                item._parent = this;
        }
        private void ChildComponentsInserted(SceneComponent item, int index)
        {
            item._parent = this;
        }
        private void ChildComponentsInsertedRange(IEnumerable<SceneComponent> items, int index)
        {
            foreach (SceneComponent item in items)
                item._parent = this;
        }
        private void ChildComponentsRemoved(SceneComponent item)
        {
            item._parent = null;
        }
        private void ChildComponentsRemovedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
                item._parent = null;
        }
    }
}
