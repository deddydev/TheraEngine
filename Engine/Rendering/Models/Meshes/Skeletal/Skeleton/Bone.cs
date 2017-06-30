using System;
using System.Collections.Generic;
using BulletSharp;
using TheraEngine.Files;
using TheraEngine.Worlds.Actors;
using System.Xml;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Threading;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering.Models
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

            Skeleton.BoneNameCache.Add(Name, this);
            Skeleton.BoneIndexCache.Add(_index = _skeleton.BoneIndexCache.Count, this);

            if (_billboardType != BillboardType.None || ScaleByDistance)
                Skeleton.AddBillboardBone(this);

            if (_physicsDriver != null)
                Skeleton.AddPhysicsBone(this);

            foreach (Bone b in ChildBones)
                b.CollectChildBones(owner);
        }
        
        public void LinkSingleBindMesh(SkeletalRigidSubMesh m) => _singleBoundMeshes.Add(m);
        public void UnlinkSingleBindMesh(SkeletalRigidSubMesh m) => _singleBoundMeshes.Remove(m);

        [Serialize("BillboardType", IsXmlAttribute = true)]
        private BillboardType _billboardType = BillboardType.None;
        [Serialize("ScaleByDistance", IsXmlAttribute = true)]
        private bool _scaleByDistance = false;

        private float _screenSize = 1.0f;
        internal int _index;
        internal Dictionary<ulong, ThreadSafeList<int>> _influencedVertices = new Dictionary<ulong, ThreadSafeList<int>>();
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

        //Set when regenerating the child cache, which is done any time the bone heirarchy is modified
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

                if (_billboardType != BillboardType.None || ScaleByDistance)
                    Skeleton?.RemoveBillboardBone(this);

                _billboardType = value;

                if (_billboardType != BillboardType.None || ScaleByDistance)
                    Skeleton?.AddBillboardBone(this);
            }
        }
        public bool ScaleByDistance
        {
            get => _scaleByDistance;
            set
            {
                if (_scaleByDistance == value)
                    return;

                if (_billboardType != BillboardType.None || ScaleByDistance)
                    Skeleton?.RemoveBillboardBone(this);

                _scaleByDistance = value;

                if (_billboardType != BillboardType.None || ScaleByDistance)
                    Skeleton?.AddBillboardBone(this);
            }
        }
        public float DistanceScaleScreenSize
        {
            get => _screenSize;
            set => _screenSize = value;
        }

        //public List<PrimitiveManager> PrimitiveManagers => _linkedPrimitiveManagers;

        public void AddPrimitiveManager(IPrimitiveManager m)
        {
            if (!_linkedPrimitiveManagers.Contains(m))
                _linkedPrimitiveManagers.Add(m);
            if (!_influencedVertices.ContainsKey(m.UniqueID))
                _influencedVertices.Add(m.UniqueID, new ThreadSafeList<int>());
        }
        public void RemovePrimitiveManager(IPrimitiveManager m)
        {
            if (_linkedPrimitiveManagers.Contains(m))
                _linkedPrimitiveManagers.Remove(m);
            if (_influencedVertices.ContainsKey(m.UniqueID))
                _influencedVertices.Remove(m.UniqueID);
        }

        //internal void CalculateBillboard()
        //{
        //    Camera c = AbstractRenderer.CurrentCamera;
        //    Matrix4 mtx = FrameMatrix, inv = InverseFrameMatrix;
        //    if (BillboardType != BillboardType.None)
        //    {

        //    }
        //    if (ScaleByDistance)
        //    {
        //        float scale = c.DistanceScale(WorldMatrix.GetPoint(), _screenSize);
        //        mtx = Matrix4.CreateScale(scale) * mtx;
        //        inv = inv * Matrix4.CreateScale(1.0f / scale);
        //    }

        //    //SetFrameMatrix(mtx, inv);

        //    //switch (BillboardType)
        //    //{
        //    //    case BillboardType.PerspectiveXY:
        //    //        break;
        //    //}
        //}

        public void CalcFrameMatrix(bool force = false)
        {
            if (force || Skeleton.BillboardBoneCount == 0)
                CalcFrameMatrix(
                _parent != null ? _parent._frameMatrix : Matrix4.Identity,
                _parent != null ? _parent._inverseFrameMatrix : Matrix4.Identity, true);
        }
        public void CalcFrameMatrix(Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool force = false)
        {
            if (force || Skeleton == null || Skeleton.BillboardBoneCount == 0)
            {
                if (force && (BillboardType != BillboardType.None || ScaleByDistance))
                {
                    Camera c = AbstractRenderer.CurrentCamera;
                    if (BillboardType != BillboardType.None)
                    {
                        Matrix4 invView = c.WorldMatrix.GetRotationMatrix4();
                        Matrix4 view = c.InverseWorldMatrix.GetRotationMatrix4();
                        _frameMatrix = parentMatrix * _frameState.Translation.AsTranslationMatrix() * invView * _frameState.Scale.AsScaleMatrix();
                        _inverseFrameMatrix = (1.0f / _frameState.Scale).AsScaleMatrix() * view * (1.0f / _frameState.Translation).AsTranslationMatrix() * inverseParentMatrix;
                    }
                    else
                    {
                        _frameMatrix = parentMatrix * _frameState.Matrix;
                        _inverseFrameMatrix = _frameState.InverseMatrix * inverseParentMatrix;
                    }
                    if (ScaleByDistance)
                    {
                        float scale = c.DistanceScale(WorldMatrix.GetPoint(), _screenSize);
                        _frameMatrix = _frameMatrix * Matrix4.CreateScale(scale);
                        _inverseFrameMatrix = Matrix4.CreateScale(1.0f / scale) * _inverseFrameMatrix;
                    }
                }
                else
                {
                    _frameMatrix = parentMatrix * _frameState.Matrix;
                    _inverseFrameMatrix = _frameState.InverseMatrix * inverseParentMatrix;
                }
                
                _vertexMatrix = FrameMatrix * InverseBindMatrix;
                _normalMatrix = BindMatrix * InverseFrameMatrix;
                _normalMatrix.Transpose();
                _normalMatrix.OnlyRotationMatrix();

                //Process skinning information dealing with this bone
                if (Engine.Settings.SkinOnGPU)
                    foreach (IPrimitiveManager m in _linkedPrimitiveManagers)
                    {
                        m.ModifiedBoneIndices.Add(_index);
                    }
                else
                {
                    for (int i = 0; i < _linkedPrimitiveManagers.Count; ++i)
                    {
                        IPrimitiveManager m = _linkedPrimitiveManagers[i];
                        ThreadSafeList<int> influenced = _influencedVertices[m.UniqueID];
                        
                        m.ModifiedVertexIndices.UnionWith(influenced);
                    }
                    _influencedInfluences.ForEach(x => x._hasChanged = true);
                }

                foreach (Bone b in _childBones)
                    b.CalcFrameMatrix(_frameMatrix, _inverseFrameMatrix);

                foreach (SceneComponent comp in _childComponents)
                    comp.RecalcGlobalTransform();
            }
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