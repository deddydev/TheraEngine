using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Components;
using TheraEngine.Physics;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Actors;
using TheraEngine.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public enum EBillboardType
    {
        None,

        OrthographicY,
        PerspectiveY,

        OrthographicXY,
        PerspectiveXY,

        OrthographicXYZ,
        PerspectiveXYZ,
    }
    public interface IBone : IFileObject, IRigidBodyCollidable, ISocket
    {
        void CollectChildBones(ISkeleton owner);
        void LinkSingleBindMesh(SkeletalRigidSubMesh m);
        void UnlinkSingleBindMesh(SkeletalRigidSubMesh m);

        ITransform RigidBodyLocalTransform { get; set; }

#if EDITOR
        bool Selected { get; set; }
#endif

        ISkeleton Skeleton { get; set; }

        SkeletalMeshComponent OwningComponent { get; }

        Matrix4 FrameMatrix { get; }
        Matrix4 BindMatrix { get; }
        Matrix4 InverseFrameMatrix { get; }
        Matrix4 InverseBindMatrix { get; }
        Matrix4 VertexMatrix { get; }
        bool FrameMatrixChanged { get; set; }

        IEventList<IBone> ChildBones { get; }

        ITransform FrameState { get; set; }
        ITransform BindState { get; set; }

        TConstraint ParentPhysicsConstraint { get; set; }

        EBillboardType BillboardType { get; set; }
        bool ScaleByDistance { get; set; }
        float DistanceScaleScreenSize { get; set; }

        Matrix4 WorldToLocalMatrix(Matrix4 worldMatrix);

        void AddPrimitiveManager(IMeshRenderer m);
        void RemovePrimitiveManager(IMeshRenderer m);
        bool UsesCamera { get; }
        int Index { get; }
        
        Dictionary<int, Tuple<IMeshRenderer, List<int>>> TargetVertices { get; }
        List<CPUSkinInfo.LiveInfluence> TargetInfluences { get; }

        void CalcFrameMatrix(ICamera camera, bool force = false);
        void CalcFrameMatrix(ICamera camera, Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool force = false);

        void CalcBindMatrix(bool updateMesh);
        void CalcBindMatrix(Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool updateMesh);

        void TriggerFrameMatrixUpdate();
    }
    [TFileExt("bone")]
    [TFileDef("Bone")]
    public partial class Bone : Socket<Bone>, IBone
    {
        //TODO: culling volumes for each skinned bone;
        //cull mesh if all influence bones are culled

        public Bone(Skeleton owner) : this()
            => Skeleton = owner;
        public Bone(string name, ITransform bindstate, TRigidBodyConstructionInfo info)
            => Init(name, bindstate, info);
        public Bone(string name, ITransform bindState)
            => Init(name, bindState, null);
        public Bone(string name)
            => Init(name, new TTransform(), null);
        public Bone()
            => Init("NewBone", new TTransform(), null);
        private void Init(string name, ITransform bindState, TRigidBodyConstructionInfo info)
        {
            FrameState = bindState.HardCopy();
            _bindState = bindState.HardCopy();

            FrameState.MatrixChanged += FrameStateMatrixChanged;
            _name = name;

            _childBones.PostAdded += ChildBoneAdded;
            _childBones.PostAddedRange += ChildBonesAddedRange;
            _childBones.PostRemoved += ChildBonesRemoved;
            _childBones.PostRemovedRange += ChildBonesRemovedRange;
            _childBones.PostInserted += ChildBoneInserted;
            _childBones.PostInsertedRange += ChildBonesInsertedRange;

            ChildComponents.PostAdded += ChildComponentsAdded;
            ChildComponents.PostAddedRange += ChildComponentsAddedRange;
            ChildComponents.PostRemoved += ChildComponentsRemoved;
            ChildComponents.PostRemovedRange += ChildComponentsRemovedRange;
            ChildComponents.PostInserted += ChildComponentsInserted;
            ChildComponents.PostInsertedRange += ChildComponentsInsertedRange;

            if (info != null)
                RigidBodyCollision = TRigidBody.New(info);
        }

        public int Index => _index;
        private void RigidBodyCollision_TransformChanged(Matrix4 transform)
        {
            FrameMatrixChanged = true;
        }

        private void FrameStateMatrixChanged(ITransform transform, Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            TriggerFrameMatrixUpdate();
        }
        public void CollectChildBones(ISkeleton owner)
        {
            Skeleton = owner;

            Skeleton.BoneNameCache.Add(Name, this);
            _index = Skeleton.BoneIndexCache.Count;
            Skeleton.BoneIndexCache.Add(_index, this);

            if (UsesCamera)
                Skeleton.AddCameraBone(this);

            if (_rigidBodyCollision != null)
                Skeleton.AddPhysicsBone(this);

            foreach (IBone b in ChildBones)
                b.CollectChildBones(owner);
        }

        public void LinkSingleBindMesh(SkeletalRigidSubMesh m)
            => _singleBoundMeshes.Add(m);
        public void UnlinkSingleBindMesh(SkeletalRigidSubMesh m)
            => _singleBoundMeshes.Remove(m);

        [TSerialize(nameof(BillboardType), NodeType = ENodeType.Attribute)]
        private EBillboardType _billboardType = EBillboardType.None;
        [TSerialize(nameof(ScaleByDistance), NodeType = ENodeType.Attribute)]
        private bool _scaleByDistance = false;
        internal int _index;
        internal Dictionary<int, Tuple<IMeshRenderer, List<int>>> _influencedVertices = new Dictionary<int, Tuple<IMeshRenderer, List<int>>>();
        internal List<CPUSkinInfo.LiveInfluence> _influencedInfluences = new List<CPUSkinInfo.LiveInfluence>();
        internal List<SkeletalRigidSubMesh> _singleBoundMeshes = new List<SkeletalRigidSubMesh>();

        Dictionary<int, Tuple<IMeshRenderer, List<int>>> IBone.TargetVertices => _influencedVertices;
        List<CPUSkinInfo.LiveInfluence> IBone.TargetInfluences => _influencedInfluences;

        [TSerialize("Transform")]
        public ITransform _bindState;
        [TSerialize("ChildBones")]
        public IEventList<IBone> _childBones = new EventList<IBone>();
        [TSerialize("ConstraintToParent")]
        public TConstraint _parentConstraint;
        public TRigidBody _rigidBodyCollision;
        [TSerialize(nameof(RigidBodyLocalTransform))]
        public ITransform _rigidBodyLocalTransform = TTransform.GetIdentity();

        [Category("Bone")]
        public ITransform RigidBodyLocalTransform
        {
            get => _rigidBodyLocalTransform;
            set
            {
                _rigidBodyLocalTransform = value;
                if (_rigidBodyCollision != null)
                    _rigidBodyCollision.WorldTransform = WorldMatrix * _rigidBodyLocalTransform.Matrix;
            }
        }

        [TPostDeserialize]
        internal void PostDeserialize()
        {
            if (_childBones != null)
                foreach (IBone b in _childBones)
                    b.ParentSocket = this;

            FrameState = _bindState.HardCopy();
            CalcBindMatrix(true);
            FrameState.MatrixChanged += FrameStateMatrixChanged;
            TriggerFrameMatrixUpdate();
        }

        public IBone _parent;
        public Matrix4
            //Animated transformation matrix relative to the skeleton's root bone, aka model space
            _frameMatrix = Matrix4.Identity, _inverseFrameMatrix = Matrix4.Identity,
            //Non-animated default bone position transforms, in model space
            _bindMatrix = Matrix4.Identity, _inverseBindMatrix = Matrix4.Identity;

        //Used for calculating vertex influences matrices quickly
        public Matrix4 _vtxPosMtx = Matrix4.Identity;
        public Matrix4 _vtxNrmMtx = Matrix4.Identity;

#if EDITOR
        public bool _selected;
        [Browsable(false)]
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (RigidBodyCollision != null)
                    RigidBodyCollision.SimulatingPhysics = false;
            }
        }
#endif

        [Browsable(false)]
        public IBone Parent
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
        //Set when regenerating the child cache, which is done any time the bone heirarchy is modified
        [Browsable(false)]
        public ISkeleton Skeleton { get; set; }
        [Browsable(false)]
        public SkeletalMeshComponent OwningComponent => Skeleton?.OwningComponent;
        Matrix4 ICollidable.CollidableWorldMatrix
        {
            get => WorldMatrix;
            set => WorldMatrix = value;
        }
        [Browsable(false)]
        public Matrix4 WorldMatrix
        {
            get => OwningComponent != null ? OwningComponent.WorldMatrix * FrameMatrix : FrameMatrix;
            set
            {
                Matrix4 frameMatrix = OwningComponent.InverseWorldMatrix * value;
                Matrix4 localMatrix = Parent is null ? frameMatrix : Parent.InverseFrameMatrix * frameMatrix;
                FrameState.Matrix = localMatrix;

                if (_rigidBodyCollision != null)
                    _rigidBodyCollision.WorldTransform = value * _rigidBodyLocalTransform.Matrix;
            }
        }

        [Browsable(false)]
        public Matrix4 InverseWorldMatrix
        {
            get => OwningComponent != null ? OwningComponent.InverseWorldMatrix * InverseFrameMatrix : InverseFrameMatrix;
            set
            {
                WorldMatrix = value.Inverted();
            }
        }
        [Browsable(false)]
        public Matrix4 FrameMatrix => _frameMatrix;
        [Browsable(false)]
        public Matrix4 BindMatrix => _bindMatrix;
        [Browsable(false)]
        public Matrix4 InverseFrameMatrix => _inverseFrameMatrix;
        [Browsable(false)]
        public Matrix4 InverseBindMatrix => _inverseBindMatrix;
        [Browsable(false)]
        public Matrix4 VertexMatrix => _vtxPosMtx;
        //[Browsable(false)]
        //public Matrix4 NormalMatrix => _vtxNrmMtx;
        public bool _frameMatrixChanged = true;
        [Browsable(false)]
        public bool FrameMatrixChanged
        {
            get => _frameMatrixChanged;
            set
            {
                _frameMatrixChanged = value;
                if (Parent != null)
                    Parent.FrameMatrixChanged = value;
            }
        }
        //[Browsable(false)]
        //public bool ChildFrameMatrixChanged => _childFrameMatrixChanged;

        public IEventList<ISceneComponent> _childComponents = new EventList<ISceneComponent>();
        private float _distanceScaleScreenSize = 1.0f;

        [TSerialize]
        [Category("Bone")]
        [Browsable(false)]
        public IEventList<ISceneComponent> ChildComponents
        {
            get => _childComponents;
            set
            {
                _childComponents = value ?? new EventList<ISceneComponent>();
            }
        }

        [Category("Bone")]
        [Browsable(false)]
        public IEventList<IBone> ChildBones => _childBones;
        [Category("Bone")]
        public ITransform FrameState { get; set; }
        [Category("Bone")]
        public ITransform BindState
        {
            get => _bindState;
            set
            {
                _bindState = value;
                CalcBindMatrix(false);
            }
        }
        [Category("Physics")]
        public TConstraint ParentPhysicsConstraint
        {
            get => _parentConstraint;
            set
            {
                if (_parentConstraint == value)
                    return;
                if (_parentConstraint != null)
                {
                    _rigidBodyCollision?.Constraints?.Remove(_parentConstraint);
                    if (Skeleton?.OwningComponent != null &&
                        Skeleton.OwningComponent.IsSpawned)
                        Skeleton.OwningComponent.OwningWorld.PhysicsWorld3D?.RemoveConstraint(_parentConstraint);
                }
                _parentConstraint = value;
                if (_parentConstraint != null)
                {
                    if (Skeleton?.OwningComponent != null &&
                        Skeleton.OwningComponent.IsSpawned)
                        Skeleton.OwningComponent.OwningWorld.PhysicsWorld3D?.AddConstraint(_parentConstraint);
                    _rigidBodyCollision?.Constraints?.Add(_parentConstraint);
                }
            }
        }
        [Category("Physics")]
        [TSerialize]
        public TRigidBody RigidBodyCollision
        {
            get => _rigidBodyCollision;
            set => Set(ref _rigidBodyCollision, value, BeforeRigidBodySet, AfterRigidBodySet);
        }
        private void BeforeRigidBodySet()
        {
            if (_parentConstraint != null)
                _rigidBodyCollision?.Constraints?.Remove(_parentConstraint);

            SkeletalMeshComponent comp = Skeleton?.OwningComponent;
            if (comp != null)
            {
                if (comp.IsSpawned)
                    comp.OwningWorld.PhysicsWorld3D?.RemoveCollisionObject(_rigidBodyCollision);
            }

            _rigidBodyCollision.Owner = null;
            _rigidBodyCollision.TransformChanged -= RigidBodyCollision_TransformChanged;

            Skeleton?.RemovePhysicsBone(this);
        }
        private void AfterRigidBodySet()
        {
            Skeleton?.AddPhysicsBone(this);

            _rigidBodyCollision.Owner = this;
            _rigidBodyCollision.TransformChanged += RigidBodyCollision_TransformChanged;

            if (_parentConstraint != null)
                _rigidBodyCollision?.Constraints?.Add(_parentConstraint);

            SkeletalMeshComponent comp = Skeleton?.OwningComponent;
            if (comp != null)
            {
                if (comp.IsSpawned)
                    comp.OwningWorld.PhysicsWorld3D?.AddCollisionObject(_rigidBodyCollision);
            }
        }

        private void PreUsesCameraChange()
        {
            if (UsesCamera)
                Skeleton?.RemoveCameraBone(this);
        }
        private void PostUsesCameraChange()
        {
            if (UsesCamera)
                Skeleton?.AddCameraBone(this);
        }

        [Category("Bone")]
        public EBillboardType BillboardType
        {
            get => _billboardType;
            set
            {
                if (Set(ref _billboardType, value, PreUsesCameraChange, PostUsesCameraChange))
                    OnPropertyChanged(nameof(UsesCamera));
            }
        }
        [Category("Bone")]
        public bool ScaleByDistance
        {
            get => _scaleByDistance;
            set
            {
                if (Set(ref _scaleByDistance, value, PreUsesCameraChange, PostUsesCameraChange))
                    OnPropertyChanged(nameof(UsesCamera));
            }
        }

        [Category("Bone")]
        public float DistanceScaleScreenSize
        {
            get => _distanceScaleScreenSize;
            set => Set(ref _distanceScaleScreenSize, value);
        }

        public Matrix4 WorldToLocalMatrix(Matrix4 worldMatrix)
            => (Parent is null ? (OwningComponent is null ? Matrix4.Identity : OwningComponent.InverseWorldMatrix) : Parent.InverseWorldMatrix) * worldMatrix;

        public void HandleTranslation(Vec3 delta)
        {

        }
        public void HandleScale(Vec3 delta)
        {

        }
        public void HandleRotation(Quat delta)
        {

        }

        public void AddPrimitiveManager(IMeshRenderer m)
        {
            if (!_influencedVertices.ContainsKey(m.BindingId))
                _influencedVertices.Add(m.BindingId, new Tuple<IMeshRenderer, List<int>>(m, new List<int>()));
            Skeleton?.AddPrimitiveManager(m);
        }
        public void RemovePrimitiveManager(IMeshRenderer m)
        {
            _influencedVertices.Remove(m.BindingId);
            Skeleton?.RemovePrimitiveManager(m);
        }

        [Browsable(false)]
        public bool UsesCamera => BillboardType != EBillboardType.None || ScaleByDistance;

        public void CalcFrameMatrix(ICamera camera, bool force = false)
            => CalcFrameMatrix(camera, _parent.FrameMatrix, _parent.InverseFrameMatrix, force);

        public void CalcBindMatrix(bool updateMesh)
        {
            CalcBindMatrix(Matrix4.Identity, Matrix4.Identity, updateMesh);
        }
        public void CalcBindMatrix(Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool updateMesh)
        {
            _bindMatrix = parentMatrix * _bindState.Matrix;
            _inverseBindMatrix = _bindState.InverseMatrix * inverseParentMatrix;

            TriggerFrameMatrixUpdate();

            foreach (Bone b in _childBones)
                b.CalcBindMatrix(_bindMatrix, _inverseBindMatrix, updateMesh);
        }
        /// <summary>
        /// Call if one or more child bones has been updated.
        /// </summary>
        private void TriggerChildFrameMatrixUpdate()
        {
            //if (_childFrameMatrixChanged)
            //    return;
            //_childFrameMatrixChanged = true;
            //if (_parent != null)
            //    _parent.TriggerChildFrameMatrixUpdate();
            //else
            //    _skeleton?.TriggerChildFrameMatrixUpdate();
        }
        /// <summary>
        /// Call if this bone has been updated.
        /// </summary>
        public void TriggerFrameMatrixUpdate()
        {
            //if (_frameMatrixChanged)
            //    return;
            FrameMatrixChanged = true;
            //if (_parent != null)
            //    _parent.TriggerChildFrameMatrixUpdate();
            //else
            //    _skeleton?.TriggerChildFrameMatrixUpdate();
        }

        #region Child Bone List Events
        private void SingleChildBoneAdded(IBone item)
        {
            item.ParentSocket = this;
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
            item.TriggerFrameMatrixUpdate();
        }
        private void SingleChildBoneRemoved(IBone item)
        {
            item.ParentSocket = null;
            item.CalcBindMatrix(false);
            item.TriggerFrameMatrixUpdate();
        }
        private void ChildBoneAdded(IBone item)
        {
            SingleChildBoneAdded(item);

            Skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesAddedRange(IEnumerable<IBone> items)
        {
            foreach (IBone item in items)
                SingleChildBoneAdded(item);

            Skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesRemoved(IBone item)
        {
            SingleChildBoneRemoved(item);

            Skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesRemovedRange(IEnumerable<IBone> items)
        {
            foreach (IBone item in items)
                SingleChildBoneRemoved(item);

            Skeleton?.RegenerateBoneCache();
        }
        private void ChildBoneInserted(IBone item, int index) => ChildBoneAdded(item);
        private void ChildBonesInsertedRange(IEnumerable<IBone> items, int index) => ChildBonesAddedRange(items);
        #endregion

        #region Child Component List Events
        private void ChildComponentsAdded(ISceneComponent item)
        {
            item.ParentSocket = this;
            //((Components.IComponent)item).OwningActor = OwningComponent.OwningActor;
            //item.RecalcWorldTransform();
        }
        private void ChildComponentsAddedRange(IEnumerable<ISceneComponent> items)
        {
            foreach (ISceneComponent item in items)
                ChildComponentsAdded(item);
        }
        private void ChildComponentsInserted(ISceneComponent item, int index)
            => ChildComponentsAdded(item);
        private void ChildComponentsInsertedRange(IEnumerable<ISceneComponent> items, int index)
            => ChildComponentsAddedRange(items);
        private void ChildComponentsRemoved(ISceneComponent item)
        {
            item.ParentSocket = null;
            //((Components.IComponent)item).OwningActor = null;
            //item.RecalcWorldTransform();
        }
        private void ChildComponentsRemovedRange(IEnumerable<ISceneComponent> items)
        {
            foreach (ISceneComponent item in items)
                ChildComponentsRemoved(item);
        }
        #endregion

        public void SetParentInternal(ISocket socket) => _parent = socket as Bone;

        ISocket ISocket.ParentSocket
        {
            get => _parent;
            set => _parent = value as IBone; //TODO: actually perform link
        }
        //bool ISocket.IsTranslatable => true;
        //bool ISocket.IsScalable => true;
        //bool ISocket.IsRotatable => true;

        [Browsable(false)]
        public int ParentSocketChildIndex => _parent.ChildBones.IndexOf(this);

        public IActor OwningActor { get; set; }

        public event DelSocketTransformChange SocketTransformChanged;
    }
}