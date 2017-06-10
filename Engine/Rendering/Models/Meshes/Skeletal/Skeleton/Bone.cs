using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors;
using System.Xml;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace CustomEngine.Rendering.Models
{
    public enum BillboardType
    {
        None,
        RotationY,
        PerspectiveY,
        RotationXY,
        PerspectiveXY,
        RotationXYZ,
        PerspectiveXYZ,
    }
    [FileClass("BONE", "Bone")]
    public class Bone : FileObject, IPhysicsDrivable, ISocket
    {
        public Bone(Skeleton owner)
        {
            _skeleton = owner;
        }
        public Bone(string name, FrameState bindstate, PhysicsConstructionInfo info)
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
        private void Init(string name, FrameState bindState, PhysicsConstructionInfo info)
        {
            _frameState = _bindState = bindState;
            _frameState.MatrixChanged += _frameState_MatrixChanged;
            _name = name;

            _childBones.PostAdded += ChildBonesAdded;
            _childBones.PostAddedRange += ChildBonesAddedRange;
            _childBones.PostRemoved += ChildBonesRemoved;
            _childBones.PostRemovedRange += ChildBonesRemovedRange;
            _childBones.PostInserted += ChildBonesInserted;
            _childBones.PostInsertedRange += ChildBonesInsertedRange;

            _childComponents.PostAdded += ChildComponentsAdded;
            _childComponents.PostAddedRange += ChildComponentsAddedRange;
            _childComponents.PostRemoved += ChildComponentsRemoved;
            _childComponents.PostRemovedRange += ChildComponentsRemovedRange;
            _childComponents.PostInserted += ChildComponentsInserted;
            _childComponents.PostInsertedRange += ChildComponentsInsertedRange;

            _physicsDriver = info == null ? null : new PhysicsDriver(this, info, MatrixUpdate, SimulationUpdate);

            //_linkedPrimitiveManagers.Added += _linkedPrimitiveManagers_Added;
            //_linkedPrimitiveManagers.Removed += _linkedPrimitiveManagers_Removed;
        }

        private void _linkedPrimitiveManagers_Removed(PrimitiveManager item)
        {
            //foreach (Bone b in ChildBones)
            //    b.PrimitiveManagers.Remove(item);
        }

        private void _linkedPrimitiveManagers_Added(PrimitiveManager item)
        {
            //foreach (Bone b in ChildBones)
            //    b.PrimitiveManagers.Add(item);
        }

        private void _frameState_MatrixChanged(Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            CalcFrameMatrix();
        }

        public void MatrixUpdate(Matrix4 worldMatrix)
        {

        }

        public void SimulationUpdate(bool isSimulating)
        {

        }

        internal void CollectChildBones(Skeleton owner)
        {
            _skeleton = owner;
            _skeleton.BoneNameCache.Add(Name, this);
            _skeleton.BoneIndexCache.Add(_index = _skeleton.BoneIndexCache.Count, this);
            if (_physicsDriver != null)
                _skeleton.PhysicsDrivableBones.Add(this);
            foreach (Bone b in ChildBones)
                b.CollectChildBones(owner);
        }

        public void LinkSingleBindMesh(SkeletalRigidSubMesh m)
            => _singleBoundMeshes.Add(m);
        public void UnlinkSingleBindMesh(SkeletalRigidSubMesh m) 
            => _singleBoundMeshes.Remove(m);

        [Serialize("BillboardType", IsXmlAttribute = true)]
        private BillboardType _billboardType = BillboardType.None;
        [Serialize("ScaleByDistance", IsXmlAttribute = true)]
        private bool _scaleByDistance = false;

        internal int _index;
        internal Dictionary<int, ThreadSafeList<int>> _influencedVertices = new Dictionary<int, ThreadSafeList<int>>();
        internal List<CPUSkinInfo.LiveInfluence> _influencedInfluences = new List<CPUSkinInfo.LiveInfluence>();
        internal List<SkeletalRigidSubMesh> _singleBoundMeshes = new List<SkeletalRigidSubMesh>();
        internal List<IPrimitiveManager> _linkedPrimitiveManagers = new List<IPrimitiveManager>();

        [Serialize("ChildBones")]
        private MonitoredList<Bone> _childBones = new MonitoredList<Bone>();
        //[Serialize("ChildComponents")]
        private MonitoredList<SceneComponent> _childComponents = new MonitoredList<SceneComponent>();
        [Serialize("PhysicsDriver")]
        private PhysicsDriver _physicsDriver;
        [Serialize("Transform")]
        private FrameState _bindState;

        private Skeleton _skeleton;
        private Bone _parent;
        private FrameState _frameState;
        private Matrix4
            //Animated transformation matrix relative to the skeleton's root bone, aka model space
            _frameMatrix = Matrix4.Identity, _inverseFrameMatrix = Matrix4.Identity,
            //Non-animated default bone position transforms, in model space
            _bindMatrix = Matrix4.Identity, _inverseBindMatrix = Matrix4.Identity,
            //Used for calculating vertex influences matrices quickly
            _vertexMatrix = Matrix4.Identity, _normalMatrix = Matrix4.Identity;

        public Bone Parent
        {
            get => _parent;
            set
            {
                if (_parent != null)
                    _parent.ChildBones.Remove(this);
                if (value != null)
                    value.ChildBones.Add(this);
            }
        }
        public MonitoredList<SceneComponent> ChildComponents => _childComponents;
        public MonitoredList<Bone> ChildBones => _childBones;
        public SkeletalMeshComponent OwningComponent => _skeleton?.OwningComponent;
        public FrameState FrameState => _frameState;
        public FrameState BindState
        {
            get => _bindState;
            set
            {
                _bindState = value;
                CalcBindMatrix(false);
            }
        }
        public Matrix4 WorldMatrix => OwningComponent != null ? OwningComponent.WorldMatrix * FrameMatrix : FrameMatrix;
        public Matrix4 InverseWorldMatrix => OwningComponent != null ? OwningComponent.InverseWorldMatrix * InverseFrameMatrix : InverseFrameMatrix;
        public Matrix4 FrameMatrix => _frameMatrix;
        public Matrix4 BindMatrix => _bindMatrix;
        public Matrix4 InverseFrameMatrix => _inverseFrameMatrix;
        public Matrix4 InverseBindMatrix => _inverseBindMatrix;
        public Matrix4 VertexMatrix => _vertexMatrix;
        public Matrix4 NormalMatrix => _normalMatrix;
        public Skeleton Skeleton => _skeleton;
        public PhysicsDriver PhysicsDriver => _physicsDriver;

        public Matrix4 WorldToLocalMatrix(Matrix4 worldMatrix)
        {
            return (Parent == null ? (OwningComponent == null ? Matrix4.Identity : OwningComponent.InverseWorldMatrix) : Parent.InverseWorldMatrix) * worldMatrix;
        }

        public BillboardType BillboardType
        {
            get => _billboardType;
            set
            {
                if (_billboardType == value)
                    return;
                _billboardType = value;
            }
        }
        public bool ScaleByDistance
        {
            get => _scaleByDistance;
            set
            {
                if (_scaleByDistance == value)
                    return;
                _scaleByDistance = value;
            }
        }

        //public List<PrimitiveManager> PrimitiveManagers => _linkedPrimitiveManagers;

        public void AddPrimitiveManager(IPrimitiveManager m)
        {
            _linkedPrimitiveManagers.Add(m);
            _influencedVertices.Add(m.BindingId, new ThreadSafeList<int>());
        }
        public void RemovePrimitiveManager(IPrimitiveManager m)
        {
            _linkedPrimitiveManagers.Remove(m);
            _influencedVertices.Remove(m.BindingId);
        }

        public void CalcFrameMatrix()
        {
            CalcFrameMatrix(
                _parent != null ? _parent._frameMatrix : Matrix4.Identity,
                _parent != null ? _parent._inverseFrameMatrix : Matrix4.Identity);
        }
        public void CalcFrameMatrix(Matrix4 parentMatrix, Matrix4 inverseParentMatrix)
        {
            _frameMatrix = parentMatrix * _frameState.Matrix;
            _inverseFrameMatrix = _frameState.InverseMatrix * inverseParentMatrix;

            _vertexMatrix = FrameMatrix * InverseBindMatrix;
            _normalMatrix = BindMatrix * InverseFrameMatrix;
            _normalMatrix.Transpose();
            _normalMatrix.OnlyRotationMatrix();

            //Process skinning information dealing with this bone
            if (Engine.Settings.SkinOnGPU)
                foreach (IPrimitiveManager m in _linkedPrimitiveManagers)
                {
                    //while (m._processingSkinning)
                    //    Thread.Sleep(1);

                    m.ModifiedBoneIndices.Add(_index);
                }
            else
            {
                for (int i = 0; i < _linkedPrimitiveManagers.Count; ++i)
                {
                    IPrimitiveManager m = _linkedPrimitiveManagers[i];
                    ThreadSafeList<int> influenced = _influencedVertices[m.BindingId];

                    //while (m._processingSkinning)
                    //    Thread.Sleep(1);

                    m.ModifiedVertexIndices.UnionWith(influenced);
                }
                _influencedInfluences.ForEach(x => x._hasChanged = true);
            }

            foreach (Bone b in _childBones)
                b.CalcFrameMatrix(_frameMatrix, _inverseFrameMatrix);
            foreach (SceneComponent comp in _childComponents)
                comp.RecalcGlobalTransform();
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
            _normalMatrix = (InverseFrameMatrix * BindMatrix).Transposed();

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
            item.OwningActor = OwningComponent.OwningActor;
            item.RecalcGlobalTransform();
        }
        private void ChildComponentsAddedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = this;
                item.OwningActor = OwningComponent.OwningActor;
                item.RecalcGlobalTransform();
            }
        }
        private void ChildComponentsInserted(SceneComponent item, int index)
            => ChildComponentsAdded(item);
        private void ChildComponentsInsertedRange(IEnumerable<SceneComponent> items, int index)
            => ChildComponentsAddedRange(items);
        private void ChildComponentsRemoved(SceneComponent item)
        {
            item._parent = null;
            item.OwningActor = null;
            item.RecalcGlobalTransform();
        }
        private void ChildComponentsRemovedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = null;
                item.OwningActor = null;
                item.RecalcGlobalTransform();
            }
        }
    }
}