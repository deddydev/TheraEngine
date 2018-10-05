using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Components;
using TheraEngine.Physics;
using TheraEngine.Core.Reflection.Attributes.Serialization;

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
    [FileExt("bone")]
    [FileDef("Bone")]
    public class Bone : TFileObject, IRigidBodyCollidable, ISocket
    {
        public Bone(Skeleton owner) : this()
        {
            Skeleton = owner;
        }
        public Bone(string name, Transform bindstate, TRigidBodyConstructionInfo info)
        {
            Init(name, bindstate, info);
        }
        public Bone(string name, Transform bindState)
        {
            Init(name, bindState, null);
        }
        public Bone(string name)
        {
            Init(name, new Transform(), null);
        }
        public Bone()
        {
            Init("NewBone", new Transform(), null);
        }
        private void Init(string name, Transform bindState, TRigidBodyConstructionInfo info)
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
            {
                RigidBodyCollision = TRigidBody.New(info);
            }
        }

        private void _rigidBodyCollision_TransformChanged(Matrix4 transform)
        {
            FrameMatrixChanged = true;
        }

        private void FrameStateMatrixChanged(Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            TriggerFrameMatrixUpdate();
        }
        internal void CollectChildBones(Skeleton owner)
        {
            Skeleton = owner;

            Skeleton.BoneNameCache.Add(Name, this);
            _index = Skeleton.BoneIndexCache.Count;
            Skeleton.BoneIndexCache.Add(_index, this);

            if (UsesCamera)
                Skeleton.AddCameraBone(this);

            if (_rigidBodyCollision != null)
                Skeleton.AddPhysicsBone(this);

            foreach (Bone b in ChildBones)
                b.CollectChildBones(owner);
        }
        
        public void LinkSingleBindMesh(SkeletalRigidSubMesh m) => _singleBoundMeshes.Add(m);
        public void UnlinkSingleBindMesh(SkeletalRigidSubMesh m) => _singleBoundMeshes.Remove(m);

        [TSerialize(nameof(BillboardType), XmlNodeType = EXmlNodeType.Attribute)]
        private BillboardType _billboardType = BillboardType.None;
        [TSerialize(nameof(ScaleByDistance), XmlNodeType = EXmlNodeType.Attribute)]
        private bool _scaleByDistance = false;
        internal int _index;
        internal Dictionary<int, Tuple<IPrimitiveManager, ThreadSafeList<int>>> _influencedVertices = new Dictionary<int, Tuple<IPrimitiveManager, ThreadSafeList<int>>>();
        internal List<CPUSkinInfo.LiveInfluence> _influencedInfluences = new List<CPUSkinInfo.LiveInfluence>();
        internal List<SkeletalRigidSubMesh> _singleBoundMeshes = new List<SkeletalRigidSubMesh>();

        [TSerialize("Transform")]
        private Transform _bindState;
        [TSerialize("ChildBones")]
        private EventList<Bone> _childBones = new EventList<Bone>();
        [TSerialize("ConstraintToParent")]
        private TConstraint _parentConstraint;
        private TRigidBody _rigidBodyCollision;
        [TSerialize(nameof(RigidBodyLocalTransform))]
        private Transform _rigidBodyLocalTransform = Transform.GetIdentity();

        [Category("Bone")]
        public Transform RigidBodyLocalTransform
        {
            get => _rigidBodyLocalTransform;
            set
            {
                _rigidBodyLocalTransform = value;
                if (_rigidBodyCollision != null)
                    _rigidBodyCollision.WorldTransform = WorldMatrix * _rigidBodyLocalTransform.Matrix;
            }
        }

        [PostDeserialize]
        internal void PostDeserialize()
        {
            foreach (Bone b in _childBones)
                b._parent = this;
            FrameState = _bindState.HardCopy();
            CalcBindMatrix(true);
            FrameState.MatrixChanged += FrameStateMatrixChanged;
            TriggerFrameMatrixUpdate();
        }

        private Bone _parent;
        private Matrix4
            //Animated transformation matrix relative to the skeleton's root bone, aka model space
            _frameMatrix = Matrix4.Identity, _inverseFrameMatrix = Matrix4.Identity,
            //Non-animated default bone position transforms, in model space
            _bindMatrix = Matrix4.Identity, _inverseBindMatrix = Matrix4.Identity;

        //Used for calculating vertex influences matrices quickly
        private Matrix4 _vtxPosMtx = Matrix4.Identity;
        private Matrix4 _vtxNrmMtx = Matrix4.Identity;

#if EDITOR
        private bool _selected;
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
        //Set when regenerating the child cache, which is done any time the bone heirarchy is modified
        [Browsable(false)]
        public Skeleton Skeleton { get; private set; }
        [Browsable(false)]
        public SkeletalMeshComponent OwningComponent => Skeleton?.OwningComponent;
        [Browsable(false)]
        public Matrix4 WorldMatrix
        {
            get => OwningComponent != null ? OwningComponent.WorldMatrix * FrameMatrix : FrameMatrix;
            set
            {
                Matrix4 frameMatrix = OwningComponent.InverseWorldMatrix * value;
                Matrix4 localMatrix = Parent == null ? frameMatrix : Parent.InverseFrameMatrix * frameMatrix;
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
        private bool _frameMatrixChanged = true;
        [Browsable(false)]
        public bool FrameMatrixChanged
        {
            get => _frameMatrixChanged;
            private set
            {
                _frameMatrixChanged = value;
                if (Parent != null)
                    Parent.FrameMatrixChanged = value;
            }
        }
        //[Browsable(false)]
        //public bool ChildFrameMatrixChanged => _childFrameMatrixChanged;

        [Category("Bone")]
        [Browsable(false)]
        public EventList<SceneComponent> ChildComponents { get; } = new EventList<SceneComponent>();

        [Category("Bone")]
        [Browsable(false)]
        public EventList<Bone> ChildBones => _childBones;
        [Category("Bone")]
        [TSerialize]
        public Transform FrameState { get; private set; }
        [Category("Bone")]
        public Transform BindState
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
                        Skeleton.OwningComponent.OwningWorld.PhysicsWorld?.RemoveConstraint(_parentConstraint);
                }
                _parentConstraint = value;
                if (_parentConstraint != null)
                {
                    if (Skeleton?.OwningComponent != null &&
                        Skeleton.OwningComponent.IsSpawned)
                        Skeleton.OwningComponent.OwningWorld.PhysicsWorld?.AddConstraint(_parentConstraint);
                    _rigidBodyCollision?.Constraints?.Add(_parentConstraint);
                }
            }
        }
        [Category("Physics")]
        [TSerialize]
        public TRigidBody RigidBodyCollision
        {
            get => _rigidBodyCollision;
            set
            {
                if (_rigidBodyCollision == value)
                    return;
                if (_rigidBodyCollision != null)
                {
                    if (Skeleton?.OwningComponent != null)
                    {
                        if (Skeleton.OwningComponent.IsSpawned)
                            Skeleton.OwningComponent.OwningWorld.PhysicsWorld?.RemoveCollisionObject(_rigidBodyCollision);
                    }
                    _rigidBodyCollision.Owner = null;
                    _rigidBodyCollision.TransformChanged -= _rigidBodyCollision_TransformChanged;
                }
                _rigidBodyCollision = value;
                if (_rigidBodyCollision != null)
                {
                    _rigidBodyCollision.Owner = this;
                    _rigidBodyCollision.TransformChanged += _rigidBodyCollision_TransformChanged;
                    if (Skeleton?.OwningComponent != null)
                    {
                        if (Skeleton.OwningComponent.IsSpawned)
                        Skeleton.OwningComponent.OwningWorld.PhysicsWorld?.AddCollisionObject(_rigidBodyCollision);
                    }
                }
            }
        }

        [Category("Bone")]
        public BillboardType BillboardType
        {
            get => _billboardType;
            set
            {
                if (_billboardType == value)
                    return;

                if (UsesCamera)
                    Skeleton?.RemoveCameraBone(this);

                _billboardType = value;

                if (UsesCamera)
                    Skeleton?.AddCameraBone(this);
            }
        }
        [Category("Bone")]
        public bool ScaleByDistance
        {
            get => _scaleByDistance;
            set
            {
                if (_scaleByDistance == value)
                    return;

                if (UsesCamera)
                    Skeleton?.RemoveCameraBone(this);

                _scaleByDistance = value;

                if (UsesCamera)
                    Skeleton?.AddCameraBone(this);
            }
        }
        [Category("Bone")]
        public float DistanceScaleScreenSize { get; set; } = 1.0f;

        public Matrix4 WorldToLocalMatrix(Matrix4 worldMatrix)
        {
            return (Parent == null ? (OwningComponent == null ? Matrix4.Identity : OwningComponent.InverseWorldMatrix) : Parent.InverseWorldMatrix) * worldMatrix;
        }

        public void HandleWorldTranslation(Vec3 delta)
        {

        }
        public void HandleWorldScale(Vec3 delta)
        {

        }
        public void HandleWorldRotation(Quat delta)
        {

        }

        internal void AddPrimitiveManager(IPrimitiveManager m)
        {
            if (!_influencedVertices.ContainsKey(m.BindingId))
                _influencedVertices.Add(m.BindingId, new Tuple<IPrimitiveManager, ThreadSafeList<int>>(m, new ThreadSafeList<int>()));
            Skeleton?.AddPrimitiveManager(m);
        }
        internal void RemovePrimitiveManager(IPrimitiveManager m)
        {
            _influencedVertices.Remove(m.BindingId);
            Skeleton?.RemovePrimitiveManager(m);
        }

        [Browsable(false)]
        public bool UsesCamera => BillboardType != BillboardType.None || ScaleByDistance;

        public void CalcFrameMatrix(Camera camera, bool force = false)
        {
            CalcFrameMatrix(camera, _parent._frameMatrix, _parent._inverseFrameMatrix, force);
        }
        public void CalcFrameMatrix(Camera camera, Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool force = false)
        {
            //WorldMatrix = _rigidBodyCollision.WorldTransform;

            bool usesCamera = UsesCamera;
            bool needsUpdate = FrameMatrixChanged || force || usesCamera;
            if (needsUpdate)
            {
                if (_rigidBodyCollision != null)
                {
                    Matrix4 worldMatrix = _rigidBodyCollision.WorldTransform * _rigidBodyLocalTransform.InverseMatrix;
                    Matrix4 frameMatrix = worldMatrix * OwningComponent.InverseWorldMatrix;
                    Matrix4 localMatrix = inverseParentMatrix * frameMatrix;
                    FrameState.Matrix = localMatrix;
                }
                if (usesCamera)
                {
                    if (BillboardType != BillboardType.None)
                    {
                        //Align rotation using camera
                        HandleBillboarding(parentMatrix, inverseParentMatrix, camera); 
                    }
                    else
                    {
                        //Regular parent-child transformation
                        _frameMatrix = parentMatrix * FrameState.Matrix;
                        _inverseFrameMatrix = FrameState.InverseMatrix * inverseParentMatrix;
                    }

                    if (ScaleByDistance && camera != null)
                    {
                        float scale = camera.DistanceScale(WorldMatrix.Translation, DistanceScaleScreenSize);
                        //Engine.PrintLine(scale.ToString());
                        _frameMatrix = _frameMatrix * Matrix4.CreateScale(scale);
                        _inverseFrameMatrix = Matrix4.CreateScale(1.0f / scale) * _inverseFrameMatrix;
                    }
                }
                else
                {
                    //Regular parent-child transformation
                    _frameMatrix = parentMatrix * FrameState.Matrix;
                    _inverseFrameMatrix = FrameState.InverseMatrix * inverseParentMatrix;
                }
                
                //Precalculate vertex/normal weighting matrices
                _vtxPosMtx = FrameMatrix * InverseBindMatrix;
                //_vtxNrmMtx = (BindMatrix * InverseFrameMatrix).Transposed().GetRotationMatrix4();

                //Process skinning information dealing with this bone
                if (Engine.Settings.SkinOnGPU)
                {
                    foreach (var m in _influencedVertices.Values)
                        m.Item1.ModifiedBoneIndicesUpdating.Add(_index);
                }
                else
                {
                    foreach (var m in _influencedVertices.Values)
                        m.Item1.ModifiedVertexIndicesUpdating.UnionWith(m.Item2);
                    _influencedInfluences.ForEach(x => x._hasChanged = true);
                }

                //Recalculate child component transforms
                foreach (SceneComponent comp in ChildComponents)
                    comp.RecalcWorldTransform();

                //Inform subscribers that the bone's transform has changed
                SocketTransformChanged?.Invoke(this);
            }

            //Recalculate child bone transforms
            foreach (Bone b in _childBones)
                b.CalcFrameMatrix(camera, _frameMatrix, _inverseFrameMatrix, needsUpdate);

            FrameMatrixChanged = false;
        }
        
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
        private void ChildBoneAdded(Bone item)
        {
            item._parent = this;
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
            item.TriggerFrameMatrixUpdate();
            Skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesAddedRange(IEnumerable<Bone> items)
        {
            foreach (Bone item in items)
            {
                item._parent = this;
                item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
                item.TriggerFrameMatrixUpdate();
            }
            Skeleton?.RegenerateBoneCache();
        }
        private void ChildBoneInserted(Bone item, int index)
        {
            item._parent = this;
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
            item.TriggerFrameMatrixUpdate();
            Skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesInsertedRange(IEnumerable<Bone> items, int index)
        {
            foreach (Bone item in items)
            {
                item._parent = this;
                item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
                item.TriggerFrameMatrixUpdate();
            }
            Skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesRemoved(Bone item)
        {
            item._parent = null;
            item.CalcBindMatrix(false);
            item.TriggerFrameMatrixUpdate();
            Skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesRemovedRange(IEnumerable<Bone> items)
        {
            foreach (Bone item in items)
            {
                item._parent = null;
                item.CalcBindMatrix(false);
                item.TriggerFrameMatrixUpdate();
            }
            Skeleton?.RegenerateBoneCache();
        }
        #endregion

        #region Child Component List Events
        private void ChildComponentsAdded(SceneComponent item)
        {
            item._parent = this;
            item.OwningActor = OwningComponent.OwningActor;
            item.RecalcWorldTransform();
        }
        private void ChildComponentsAddedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = this;
                item.OwningActor = OwningComponent.OwningActor;
                item.RecalcWorldTransform();
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
            item.RecalcWorldTransform();
        }
        private void ChildComponentsRemovedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = null;
                item.OwningActor = null;
                item.RecalcWorldTransform();
            }
        }
        #endregion

        private void HandleBillboarding(Matrix4 parentMatrix, Matrix4 inverseParentMatrix, Camera camera)
        {
            if (camera == null)
                return;

            //Apply local translation component to parent matrix
            Matrix4 frameTrans = parentMatrix * FrameState.Translation.Raw.AsTranslationMatrix();
            Matrix4 invFramTrans = (-FrameState.Translation.Raw).AsTranslationMatrix() * inverseParentMatrix;

            //Reset rotation for billboard
            frameTrans = parentMatrix.ClearRotation();
            invFramTrans = inverseParentMatrix.ClearRotation();

            //Calculate angles from current position to camera
            Matrix4 angles = Matrix4.Identity, invAngles = Matrix4.Identity;
            switch (BillboardType)
            {
                case BillboardType.PerspectiveXYZ:

                    Vec3 componentPoint = camera.WorldPoint * OwningComponent.InverseWorldMatrix;
                    Vec3 diff = frameTrans.Translation - componentPoint;
                    Rotator r = diff.LookatAngles();

                    angles = r.GetMatrix();
                    invAngles = r.GetInverseMatrix();

                    break;

                case BillboardType.PerspectiveXY:

                    break;

                case BillboardType.PerspectiveY:

                    break;

                case BillboardType.RotationXYZ:

                    Vec3 up1 = camera.UpVector;
                    Vec3 forward1 = camera.ForwardVector;

                    angles = new Matrix4(
                        new Vec4(forward1 ^ up1, 0.0f),
                        new Vec4(up1, 0.0f),
                        new Vec4(forward1, 0.0f),
                        Vec4.UnitW);

                    invAngles = new Matrix4(
                        new Vec4(up1 ^ forward1, 0.0f),
                        new Vec4(-up1, 0.0f),
                        new Vec4(-forward1, 0.0f),
                        Vec4.UnitW);

                    break;

                case BillboardType.RotationXY:

                    Vec3 forward2 = camera.ForwardVector;
                    Vec3 right2 = camera.RightVector;
                    right2.Y = 0.0f;

                    angles = new Matrix4(
                        new Vec4(right2, 0.0f),
                        new Vec4(right2 ^ forward2, 0.0f),
                        new Vec4(forward2, 0.0f),
                        Vec4.UnitW);

                    invAngles = new Matrix4(
                        new Vec4(-right2, 0.0f),
                        new Vec4(forward2 ^ right2, 0.0f),
                        new Vec4(-forward2, 0.0f),
                        Vec4.UnitW);

                    break;

                case BillboardType.RotationY:

                    Vec3 up3 = Vec3.TransformNormalInverse(Vec3.UnitY, inverseParentMatrix); //Up is related to parent
                    Vec3 forward3 = camera.ForwardVector;
                    forward3.Y = 0.0f;

                    angles = new Matrix4(
                        new Vec4(forward3 ^ up3, 0.0f),
                        new Vec4(up3, 0.0f),
                        new Vec4(forward3, 0.0f), 
                        Vec4.UnitW);

                    invAngles = new Matrix4(
                        new Vec4(up3 ^ forward3, 0.0f),
                        new Vec4(-up3, 0.0f), 
                        new Vec4(-forward3, 0.0f),
                        Vec4.UnitW);

                    break;
            }

            //Fix rotation in relation to parent component
            if (OwningComponent != null)
            {
                angles = OwningComponent.InverseWorldMatrix.GetRotationMatrix4() * angles;
                invAngles = invAngles * OwningComponent.WorldMatrix.GetRotationMatrix4();
            }

            //Multiply translation, rotation and scale parts together
            _frameMatrix = frameTrans * angles * FrameState.Scale.Raw.AsScaleMatrix();
            _inverseFrameMatrix = (1.0f / FrameState.Scale).AsScaleMatrix() * invAngles * invFramTrans;
        }

        ISocket ISocket.ParentSocket => _parent;

        bool ISocket.IsTranslatable => true;
        bool ISocket.IsScalable => true;
        bool ISocket.IsRotatable => true;

        public event DelSocketTransformChange SocketTransformChanged;
        void ISocket.RegisterWorldMatrixChanged(DelSocketTransformChange eventMethod, bool unregister)
        {
            if (unregister)
                SocketTransformChanged -= eventMethod;
            else
                SocketTransformChanged += eventMethod;
        }
    }
}