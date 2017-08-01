using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds.Actors;

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
    [TypeConverter(typeof(ExpandableObjectConverter))]
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

            _childBones.PostAdded += ChildBoneAdded;
            _childBones.PostAddedRange += ChildBonesAddedRange;
            _childBones.PostRemoved += ChildBonesRemoved;
            _childBones.PostRemovedRange += ChildBonesRemovedRange;
            _childBones.PostInserted += ChildBoneInserted;
            _childBones.PostInsertedRange += ChildBonesInsertedRange;

            _childComponents.PostAdded += ChildComponentsAdded;
            _childComponents.PostAddedRange += ChildComponentsAddedRange;
            _childComponents.PostRemoved += ChildComponentsRemoved;
            _childComponents.PostRemovedRange += ChildComponentsRemovedRange;
            _childComponents.PostInserted += ChildComponentsInserted;
            _childComponents.PostInsertedRange += ChildComponentsInsertedRange;

            _physicsDriver = info == null ? null : new PhysicsDriver(this, info, MatrixUpdate, SimulationUpdate);
        }
        
        private void _frameState_MatrixChanged(Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            TriggerFrameMatrixUpdate();
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

            if (UsesCamera)
                Skeleton.AddCameraBone(this);

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
        internal Dictionary<int, Tuple<IPrimitiveManager, ThreadSafeList<int>>> _influencedVertices = new Dictionary<int, Tuple<IPrimitiveManager, ThreadSafeList<int>>>();
        internal List<CPUSkinInfo.LiveInfluence> _influencedInfluences = new List<CPUSkinInfo.LiveInfluence>();
        internal List<SkeletalRigidSubMesh> _singleBoundMeshes = new List<SkeletalRigidSubMesh>();

        [Serialize("ChildBones")]
        private MonitoredList<Bone> _childBones = new MonitoredList<Bone>();
        //[Serialize("ChildComponents")]
        private MonitoredList<SceneComponent> _childComponents = new MonitoredList<SceneComponent>();
        [Serialize("PhysicsDriver")]
        private PhysicsDriver _physicsDriver;
        [Serialize("Transform")]
        private FrameState _bindState;

        private bool _frameMatrixChanged = false, _childFrameMatrixChanged = false;
        private Skeleton _skeleton;
        private Bone _parent;
        private FrameState _frameState;
        private Matrix4
            //Animated transformation matrix relative to the skeleton's root bone, aka model space
            _frameMatrix = Matrix4.Identity, _inverseFrameMatrix = Matrix4.Identity,
            //Non-animated default bone position transforms, in model space
            _bindMatrix = Matrix4.Identity, _inverseBindMatrix = Matrix4.Identity;

        //Used for calculating vertex influences matrices quickly
        private Matrix4 _vtxPosMtx = Matrix4.Identity;
        private Matrix4 _vtxNrmMtx = Matrix4.Identity;

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
        public Matrix4 WorldMatrix
        {
            get => OwningComponent != null ? OwningComponent.WorldMatrix * FrameMatrix : FrameMatrix;
            set
            {
                Matrix4 frameMatrix = OwningComponent.InverseWorldMatrix * value;
                Matrix4 localMatrix = Parent == null ? frameMatrix : Parent.InverseFrameMatrix * frameMatrix;
                _frameState.Matrix = localMatrix;
            }
        }
        public Matrix4 InverseWorldMatrix => OwningComponent != null ? OwningComponent.InverseWorldMatrix * InverseFrameMatrix : InverseFrameMatrix;
        public Matrix4 FrameMatrix => _frameMatrix;
        public Matrix4 BindMatrix => _bindMatrix;
        public Matrix4 InverseFrameMatrix => _inverseFrameMatrix;
        public Matrix4 InverseBindMatrix => _inverseBindMatrix;
        public Matrix4 VertexMatrix => _vtxPosMtx;
        public Matrix4 NormalMatrix => _vtxNrmMtx;

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

                if (UsesCamera)
                    Skeleton?.RemoveCameraBone(this);

                _billboardType = value;

                if (UsesCamera)
                    Skeleton?.AddCameraBone(this);
            }
        }
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
        public float DistanceScaleScreenSize
        {
            get => _screenSize;
            set => _screenSize = value;
        }

        public bool FrameMatrixChanged => _frameMatrixChanged;
        public bool ChildFrameMatrixChanged => _childFrameMatrixChanged;

        public void HandleTranslation(Vec3 delta)
        {

        }
        public void HandleScale(Vec3 delta)
        {

        }
        public void HandleRotation(Quat delta)
        {

        }

        public void AddPrimitiveManager(IPrimitiveManager m)
        {
            if (!_influencedVertices.ContainsKey(m.BindingId))
                _influencedVertices.Add(m.BindingId, new Tuple<IPrimitiveManager, ThreadSafeList<int>>(m, new ThreadSafeList<int>()));
        }
        public void RemovePrimitiveManager(IPrimitiveManager m)
        {
            _influencedVertices.Remove(m.BindingId);
        }

        public bool UsesCamera => BillboardType != BillboardType.None || ScaleByDistance;

        public void CalcFrameMatrix(Camera c, bool force = false)
        {
            CalcFrameMatrix(c, _parent._frameMatrix, _parent._inverseFrameMatrix, force);
        }
        public void CalcFrameMatrix(Camera c, Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool force = false)
        {
            bool usesCamera = UsesCamera;
            if (_frameMatrixChanged || force || usesCamera)
            {
                if (usesCamera)
                {
                    if (BillboardType != BillboardType.None)
                    {
                        Vec3 componentPoint = c.WorldPoint * OwningComponent.InverseWorldMatrix;

                        Matrix4 frameTrans = (parentMatrix * _frameState.Translation.Raw.AsTranslationMatrix()).GetPoint().AsTranslationMatrix();
                        Matrix4 invFramTrans = ((-_frameState.Translation.Raw).AsTranslationMatrix() * inverseParentMatrix).GetPoint().AsTranslationMatrix();

                        Matrix4 angles = Matrix4.Identity, invAngles = Matrix4.Identity;

                        switch (BillboardType)
                        {
                            case BillboardType.PerspectiveXYZ:

                                Vec3 diff = frameTrans.GetPoint() - componentPoint;
                                Rotator r = diff.LookatAngles();

                                angles = r.GetMatrix();
                                invAngles = r.GetInverseMatrix();

                                break;
                            case BillboardType.PerspectiveXY:

                                break;
                            case BillboardType.PerspectiveY:

                                break;
                            case BillboardType.RotationXYZ:

                                break;
                            case BillboardType.RotationXY:

                                break;
                            case BillboardType.RotationY:

                                break;
                        }

                        _frameMatrix = frameTrans * angles * _frameState.Scale.Raw.AsScaleMatrix();
                        _inverseFrameMatrix = (1.0f / _frameState.Scale).AsScaleMatrix() * invAngles * invFramTrans;
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
                
                _vtxPosMtx = FrameMatrix * InverseBindMatrix;
                _vtxNrmMtx = (BindMatrix * InverseFrameMatrix).Transposed().GetRotationMatrix4();

                //Process skinning information dealing with this bone
                if (Engine.Settings.SkinOnGPU)
                {
                    foreach (var m in _influencedVertices.Values)
                        m.Item1.ModifiedBoneIndices.Add(_index);
                }
                else
                {
                    foreach (var m in _influencedVertices.Values)
                        m.Item1.ModifiedVertexIndices.UnionWith(m.Item2);
                    _influencedInfluences.ForEach(x => x._hasChanged = true);
                }

                foreach (SceneComponent comp in _childComponents)
                    comp.RecalcGlobalTransform();
            }

            if (_childFrameMatrixChanged || _frameMatrixChanged || usesCamera || force)
            {
                foreach (Bone b in _childBones)
                    b.CalcFrameMatrix(c, _frameMatrix, _inverseFrameMatrix, force || _frameMatrixChanged || usesCamera);
            }

            _childFrameMatrixChanged = false;
            _frameMatrixChanged = false;
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

            //_vtxPosMtx = FrameMatrix * InverseBindMatrix;
            //_vtxNrmMtx = (InverseFrameMatrix * BindMatrix).GetRotationMatrix4();
            //_vtxNrmMtx.Transpose();

            TriggerFrameMatrixUpdate();

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
        /// <summary>
        /// Call if one or more child bones has been updated.
        /// </summary>
        private void TriggerChildFrameMatrixUpdate()
        {
            //if (_childFrameMatrixChanged)
            //    return;
            _childFrameMatrixChanged = true;
            if (_parent != null)
                _parent.TriggerChildFrameMatrixUpdate();
            else
                _skeleton?.TriggerChildFrameMatrixUpdate();
        }
        /// <summary>
        /// Call if this bone has been updated.
        /// </summary>
        public void TriggerFrameMatrixUpdate()
        {
            //if (_frameMatrixChanged)
            //    return;
            _frameMatrixChanged = true;
            if (_parent != null)
                _parent.TriggerChildFrameMatrixUpdate();
            else
                _skeleton?.TriggerChildFrameMatrixUpdate();
        }
        private void ChildBoneAdded(Bone item)
        {
            item._parent = this;
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
            item.TriggerFrameMatrixUpdate();
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesAddedRange(IEnumerable<Bone> items)
        {
            foreach (Bone item in items)
            {
                item._parent = this;
                item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
                item.TriggerFrameMatrixUpdate();
            }
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBoneInserted(Bone item, int index)
        {
            item._parent = this;
            item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
            item.TriggerFrameMatrixUpdate();
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesInsertedRange(IEnumerable<Bone> items, int index)
        {
            foreach (Bone item in items)
            {
                item._parent = this;
                item.CalcBindMatrix(BindMatrix, InverseBindMatrix, false);
                item.TriggerFrameMatrixUpdate();
            }
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesRemoved(Bone item)
        {
            item._parent = null;
            item.CalcBindMatrix(false);
            item.TriggerFrameMatrixUpdate();
            _skeleton?.RegenerateBoneCache();
        }
        private void ChildBonesRemovedRange(IEnumerable<Bone> items)
        {
            foreach (Bone item in items)
            {
                item._parent = null;
                item.CalcBindMatrix(false);
                item.TriggerFrameMatrixUpdate();
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