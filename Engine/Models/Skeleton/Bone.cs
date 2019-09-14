﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Components;
using TheraEngine.Physics;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Rendering.Models
{
    public enum EBillboardType
    {
        None,
        RotationY,
        PerspectiveY,
        RotationXY,
        PerspectiveXY,
        RotationXYZ,
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

        IBone Parent { get; set; }
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

        void AddPrimitiveManager(IPrimitiveManager m);
        void RemovePrimitiveManager(IPrimitiveManager m);
        bool UsesCamera { get; }
        int Index { get; }
        
        Dictionary<int, Tuple<IPrimitiveManager, List<int>>> InfluencedVertices { get; }
        List<CPUSkinInfo.LiveInfluence> InfluencedInfluences { get; }

        void CalcFrameMatrix(ICamera camera, bool force = false);
        void CalcFrameMatrix(ICamera camera, Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool force = false);

        void CalcBindMatrix(bool updateMesh);
        void CalcBindMatrix(Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool updateMesh);

        void TriggerFrameMatrixUpdate();
    }
    [TFileExt("bone")]
    [TFileDef("Bone")]
    public class Bone : TFileObject, IBone
    {
        //TODO: culling volumes for each skinned bone; cull mesh if all influence bones are culled

        public Bone(Skeleton owner) : this()
            => Skeleton = owner;
        public Bone(string name, ITransform bindstate, TRigidBodyConstructionInfo info)
            => Init(name, bindstate, info);
        public Bone(string name, ITransform bindState)
            => Init(name, bindState, null);
        public Bone(string name)
            => Init(name, new Transform(), null);
        public Bone()
            => Init("NewBone", new Transform(), null);
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
        private void _rigidBodyCollision_TransformChanged(Matrix4 transform)
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
        
        public void LinkSingleBindMesh(SkeletalRigidSubMesh m) => _singleBoundMeshes.Add(m);
        public void UnlinkSingleBindMesh(SkeletalRigidSubMesh m) => _singleBoundMeshes.Remove(m);

        [TSerialize(nameof(BillboardType), NodeType = ENodeType.Attribute)]
        private EBillboardType _billboardType = EBillboardType.None;
        [TSerialize(nameof(ScaleByDistance), NodeType = ENodeType.Attribute)]
        private bool _scaleByDistance = false;
        internal int _index;
        internal Dictionary<int, Tuple<IPrimitiveManager, List<int>>> _influencedVertices = new Dictionary<int, Tuple<IPrimitiveManager, List<int>>>();
        internal List<CPUSkinInfo.LiveInfluence> _influencedInfluences = new List<CPUSkinInfo.LiveInfluence>();
        internal List<SkeletalRigidSubMesh> _singleBoundMeshes = new List<SkeletalRigidSubMesh>();
        
        Dictionary<int, Tuple<IPrimitiveManager, List<int>>> IBone.InfluencedVertices => _influencedVertices;
        List<CPUSkinInfo.LiveInfluence> IBone.InfluencedInfluences => _influencedInfluences;

        [TSerialize("Transform")]
        private ITransform _bindState;
        [TSerialize("ChildBones")]
        private IEventList<IBone> _childBones = new EventList<IBone>();
        [TSerialize("ConstraintToParent")]
        private TConstraint _parentConstraint;
        private TRigidBody _rigidBodyCollision;
        [TSerialize(nameof(RigidBodyLocalTransform))]
        private ITransform _rigidBodyLocalTransform = Transform.GetIdentity();

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
            foreach (IBone b in _childBones)
                b.SetParentInternal(this);
            FrameState = _bindState.HardCopy();
            CalcBindMatrix(true);
            FrameState.MatrixChanged += FrameStateMatrixChanged;
            TriggerFrameMatrixUpdate();
        }

        private IBone _parent;
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
        private bool _frameMatrixChanged = true;
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

        private IEventList<ISceneComponent> _childComponents = new EventList<ISceneComponent>();
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
            set
            {
                if (_rigidBodyCollision == value)
                    return;
                SkeletalMeshComponent comp = Skeleton?.OwningComponent;
                if (_rigidBodyCollision != null)
                {
                    if (comp != null)
                    {
                        if (comp.IsSpawned)
                            comp.OwningWorld.PhysicsWorld3D?.RemoveCollisionObject(_rigidBodyCollision);
                    }
                    _rigidBodyCollision.Owner = null;
                    _rigidBodyCollision.TransformChanged -= _rigidBodyCollision_TransformChanged;
                    Skeleton?.RemovePhysicsBone(this);
                }
                _rigidBodyCollision = value;
                if (_rigidBodyCollision != null)
                {
                    Skeleton?.AddPhysicsBone(this);
                    _rigidBodyCollision.Owner = this;
                    _rigidBodyCollision.TransformChanged += _rigidBodyCollision_TransformChanged;
                    if (comp != null)
                    {
                        if (comp.IsSpawned)
                            comp.OwningWorld.PhysicsWorld3D?.AddCollisionObject(_rigidBodyCollision);
                    }
                }
            }
        }

        [Category("Bone")]
        public EBillboardType BillboardType
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
            => (Parent is null ? (OwningComponent is null ? Matrix4.Identity : OwningComponent.InverseWorldMatrix) : Parent.InverseWorldMatrix) * worldMatrix;
        
        public void HandleWorldTranslation(Vec3 delta)
        {

        }
        public void HandleWorldScale(Vec3 delta)
        {

        }
        public void HandleWorldRotation(Quat delta)
        {

        }

        public void AddPrimitiveManager(IPrimitiveManager m)
        {
            if (!_influencedVertices.ContainsKey(m.BindingId))
                _influencedVertices.Add(m.BindingId, new Tuple<IPrimitiveManager, List<int>>(m, new List<int>()));
            Skeleton?.AddPrimitiveManager(m);
        }
        public void RemovePrimitiveManager(IPrimitiveManager m)
        {
            _influencedVertices.Remove(m.BindingId);
            Skeleton?.RemovePrimitiveManager(m);
        }

        [Browsable(false)]
        public bool UsesCamera => BillboardType != EBillboardType.None || ScaleByDistance;

        public void CalcFrameMatrix(ICamera camera, bool force = false)
        {
            CalcFrameMatrix(camera, _parent.FrameMatrix, _parent.InverseFrameMatrix, force);
        }
        public void CalcFrameMatrix(ICamera camera, Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool force = false)
        {
            //WorldMatrix = _rigidBodyCollision.WorldTransform;

            bool usesCamera = UsesCamera;
            bool needsUpdate = FrameMatrixChanged || force || usesCamera;
            if (needsUpdate)
            {
                if (_rigidBodyCollision != null)
                {
                    Matrix4 bodyMtx = _rigidBodyCollision.WorldTransform;
                    bodyMtx = bodyMtx.ClearScale();
                    Matrix4 worldMatrix = bodyMtx * _rigidBodyLocalTransform.InverseMatrix;
                    Matrix4 frameMatrix = worldMatrix * OwningComponent.InverseWorldMatrix;
                    Matrix4 localMatrix = inverseParentMatrix * frameMatrix;
                    FrameState.Matrix = localMatrix;
                }
                if (usesCamera)
                {
                    if (BillboardType != EBillboardType.None)
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
                        _frameMatrix *= Matrix4.CreateScale(scale);
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
        private void SingleChildBoneAdded(IBone item)
        {
            item.SetParentInternal(this);
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
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
        private void ChildBoneInserted(IBone item, int index)
        {
            ChildBoneAdded(item);
        }
        private void ChildBonesInsertedRange(IEnumerable<IBone> items, int index)
        {
            ChildBonesAddedRange(items);
        }
        private void SingleChildBoneRemoved(IBone item)
        {
            item.SetParentInternal(null);
            item.CalcBindMatrix(false);
            item.TriggerFrameMatrixUpdate();
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
        #endregion

        #region Child Component List Events
        private void ChildComponentsAdded(ISceneComponent item)
        {
            item.SetParentInternal(this);
            item.OwningActor = OwningComponent.OwningActor;
            item.RecalcWorldTransform();
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
            item.SetParentInternal(null);
            item.OwningActor = null;
            item.RecalcWorldTransform();
        }
        private void ChildComponentsRemovedRange(IEnumerable<ISceneComponent> items)
        {
            foreach (ISceneComponent item in items)
                ChildComponentsRemoved(item);
        }
        #endregion

        private void HandleBillboarding(Matrix4 parentMatrix, Matrix4 inverseParentMatrix, ICamera camera)
        {
            if (camera is null)
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
                case EBillboardType.PerspectiveXYZ:

                    Vec3 componentPoint = camera.WorldPoint * OwningComponent.InverseWorldMatrix;
                    Vec3 diff = frameTrans.Translation - componentPoint;
                    Rotator r = diff.LookatAngles();

                    angles = r.GetMatrix();
                    invAngles = r.GetInverseMatrix();

                    break;

                case EBillboardType.PerspectiveXY:

                    break;

                case EBillboardType.PerspectiveY:

                    break;

                case EBillboardType.RotationXYZ:

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

                case EBillboardType.RotationXY:

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

                case EBillboardType.RotationY:

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
                invAngles *= OwningComponent.WorldMatrix.GetRotationMatrix4();
            }

            //Multiply translation, rotation and scale parts together
            _frameMatrix = frameTrans * angles * FrameState.Scale.Raw.AsScaleMatrix();
            _inverseFrameMatrix = (1.0f / FrameState.Scale).AsScaleMatrix() * invAngles * invFramTrans;
        }

        public void SetParentInternal(ISocket socket) => _parent = socket as Bone;

        ISocket ISocket.ParentSocket
        {
            get => _parent;
            set => _parent = value as Bone; //TODO: actually perform link
        }
        bool ISocket.IsTranslatable => true;
        bool ISocket.IsScalable => true;
        bool ISocket.IsRotatable => true;

        [Browsable(false)]
        public int ParentSocketChildIndex => _parent.ChildBones.IndexOf(this);

        public event DelSocketTransformChange SocketTransformChanged;
    }
}