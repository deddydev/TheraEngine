using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors;
using System.Xml;
using System.Runtime.InteropServices;
using System.IO;

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
        internal Dictionary<int, List<int>> _influencedVertices = new Dictionary<int, List<int>>();
        internal List<CPUSkinInfo.LiveInfluence> _influencedInfluences = new List<CPUSkinInfo.LiveInfluence>();
        internal List<SkeletalRigidSubMesh> _singleBoundMeshes = new List<SkeletalRigidSubMesh>();
        internal List<PrimitiveManager> _linkedPrimitiveManagers = new List<PrimitiveManager>();

        [Serialize("ChildBones")]
        private MonitoredList<Bone> _childBones = new MonitoredList<Bone>();
        [Serialize("ChildComponents")]
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
            _vertexMatrix = Matrix4.Identity, _vertexMatrixIT = Matrix4.Identity;

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
        public Matrix4 VertexMatrixIT => _vertexMatrixIT;
        public Skeleton Skeleton => _skeleton;
        public PhysicsDriver PhysicsDriver => _physicsDriver;
        //public List<PrimitiveManager> PrimitiveManagers => _linkedPrimitiveManagers;

        public void AddPrimitiveManager(PrimitiveManager m)
        {
            _linkedPrimitiveManagers.Add(m);
            _influencedVertices.Add(m.BindingId, new List<int>());
        }
        public void RemovePrimitiveManager(PrimitiveManager m)
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
            _vertexMatrixIT = (InverseFrameMatrix * BindMatrix).Transposed().GetRotationMatrix4();

            //Process skinning information dealing with this bone
            if (Engine.Settings.SkinOnGPU)
                foreach (PrimitiveManager m in _linkedPrimitiveManagers)
                {
                    //if (!m._processingSkinning)
                        m.ModifiedBoneIndices.Add(_index);
                }
            else
            {
                for (int i = 0; i < _linkedPrimitiveManagers.Count; ++i)
                {
                    PrimitiveManager m = _linkedPrimitiveManagers[i];
                    List<int> influenced = _influencedVertices[m.BindingId];
                    //m._cpuSkinInfo.UpdatePNBT(influenced);
                    //if (!m._processingSkinning)
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
            _vertexMatrixIT = (InverseFrameMatrix * BindMatrix).Transposed().GetRotationMatrix4();

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
        //protected override int OnCalculateSize(StringTable table)
        //{
        //    table.Add(_name);
        //    int size = Header.Size;
        //    foreach (Bone b in ChildBones)
        //        size += b.CalculateSize(table);
        //    return size;
        //}
        //public unsafe override void Read(VoidPtr address, VoidPtr strings)
        //{
        //    Header h = *(Header*)address;
        //    _name = strings.GetString(h._name);
        //    _frameState = _bindState = h._state;
        //}
        //public unsafe override void Write(VoidPtr address, StringTable table)
        //{
        //    Header* h = (Header*)address;
        //    h->_name = table[_name];
        //    h->_state = _bindState;
        //}
        //public override void Write(XmlWriter writer)
        //{
        //    writer.WriteStartElement("bone");
        //    writer.WriteAttributeString("name", _name);
        //    writer.WriteAttributeString("distanceScale", _scaleByDistance.ToString());
        //    writer.WriteAttributeString("billboard", _billboardType.ToString());
        //    writer.WriteAttributeString("childCount", _childBones.Count.ToString());
        //    _bindState.Write(writer);
        //    foreach (Bone b in ChildBones)
        //        b.Write(writer);
        //    writer.WriteEndElement();
        //}
        //public override void Read(XMLReader reader)
        //{
        //    if (!reader.Name.Equals("bone", true))
        //        throw new Exception();
        //    while (reader.ReadAttribute())
        //    {
        //        if (reader.Name.Equals("name", true))
        //            _name = (string)reader.Value;
        //        else if (reader.Name.Equals("distanceScale", true))
        //            _scaleByDistance = bool.Parse((string)reader.Value);
        //        else if (reader.Name.Equals("billboard", true))
        //            _billboardType = (BillboardType)Enum.Parse(typeof(BillboardType), (string)reader.Value);
        //    }
        //    _skeleton.BoneNameCache.Add(_name, this);
        //    while (reader.BeginElement())
        //    {
        //        if (reader.Name.Equals("bone", true))
        //        {
        //            Bone b = new Bone(_skeleton);
        //            b.Read(reader);
        //            ChildBones.Add(b);
        //        }
        //        else if (reader.Name.Equals("transform", true))
        //        {
        //            _bindState = new FrameState();
        //            _bindState.Read(reader);
        //            _frameState = _bindState;
        //        }
        //        else if (reader.Name.Equals("fileRef", true))
        //        {
        //            if (reader.ReadAttribute())
        //            {

        //            }
        //        }
        //        reader.EndElement();
        //    }
        //}
        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public unsafe struct Header
        //{
        //    public const int Size = 12 + FrameState.Header.Size;

        //    public bint _name;
        //    public bint _parentIndex;
        //    public bushort _scaleByDistance;
        //    public bushort _billboardType;
        //    public FrameState.Header _state;

        //    public bool ScaleByDistance
        //    {
        //        get => _scaleByDistance == 0 ? false : true;
        //        set => _scaleByDistance = (ushort)(value ? 1 : 0);
        //    }
        //    public BillboardType BillboardType
        //    {
        //        get => (BillboardType)(ushort)_billboardType;
        //        set => _billboardType = (ushort)value;
        //    }

        //    public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        //}
    }
}