using TheraEngine.Files;
using TheraEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Shapes;
using TheraEngine.Worlds.Actors.Components.Scene.Mesh;

namespace TheraEngine.Rendering.Models
{
    [FileClass("SKEL", "Model Skeleton")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Skeleton : FileObject, IEnumerable<Bone>, I3DRenderable
    {
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass3D.OpaqueForward, null, false);

        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        public Skeleton() : base() { }
        public Skeleton(params Bone[] rootBones) : base()
        {
            RootBones = rootBones;
            foreach (Bone b in RootBones)
            {
                b.CalcBindMatrix(true);
                b.TriggerFrameMatrixUpdate();
            }
            RegenerateBoneCache();
        }
        public Skeleton(Bone rootBone) : base()
        {
            RootBones = new Bone[1] { rootBone };
            rootBone.CalcBindMatrix(true);
            rootBone.TriggerFrameMatrixUpdate();
            RegenerateBoneCache();
        }

        public Bone this[string name]
            => BoneNameCache.ContainsKey(name) ? BoneNameCache[name] : null;
        public Bone this[int index] 
            => BoneIndexCache.ContainsKey(index) ? BoneIndexCache[index] : null;

        bool _visible,
            _visibleInEditorOnly = true,
            _hiddenFromOwner = false,
            _visibleToOwnerOnly = false,
            _visibleByDefault = false;
        
        //Internal usage information, not serialized
        private List<Bone> _physicsDrivableBones = new List<Bone>();
        private List<Bone> _cameraBones = new List<Bone>();
        private Dictionary<string, Bone> _boneNameCache = new Dictionary<string, Bone>();
        private Dictionary<int, Bone> _boneIndexCache = new Dictionary<int, Bone>();
        private SkeletalMeshComponent _owningComponent;
        //private bool _childMatrixModified = false;

        
        private Bone[] _rootBones;

        [TSerialize]
        public Bone[] RootBones
        {
            get => _rootBones;
            set
            {
                _rootBones = value;
                RegenerateBoneCache();
            }
        }
        [Browsable(false)]
        public Dictionary<string, Bone> BoneNameCache => _boneNameCache;
        [Browsable(false)]
        public Dictionary<int, Bone> BoneIndexCache => _boneIndexCache;
        [Browsable(false)]
        public SkeletalMeshComponent OwningComponent
        {
            get => _owningComponent;
            set => _owningComponent = value;
        }

        public IEnumerator<Bone> GetEnumerator() 
            => ((IEnumerable<Bone>)_boneNameCache.Values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() 
            => ((IEnumerable<Bone>)_boneNameCache.Values).GetEnumerator();

        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        public void UpdateBones(Camera c)
        {
            //Looping recursively is more efficient than looping through the whole bone cache
            //because bone subtrees that do not need updating will be skipped entirely
            //instead of being iterated through
            if (RootBones != null)
                foreach (Bone b in RootBones)
                    b.CalcFrameMatrix(c, Matrix4.Identity, Matrix4.Identity);
        }

        public bool VisibleInEditorOnly
        {
            get => _visibleInEditorOnly;
            set => _visibleInEditorOnly = value;
        }
        public bool HiddenFromOwner
        {
            get => _hiddenFromOwner;
            set => _hiddenFromOwner = value;
        }
        public bool VisibleToOwnerOnly
        {
            get => _visibleToOwnerOnly;
            set => _visibleToOwnerOnly = value;
        }
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }

        public ReadOnlyCollection<Bone> GetCameraRelativeBones() => _cameraBones.AsReadOnly();
        public ReadOnlyCollection<Bone> GetPhysicsDrivableBones() => _physicsDrivableBones.AsReadOnly();
        
        public Bone GetBone(string boneName)
        {
            if (!_boneNameCache.ContainsKey(boneName))
                return RootBones[0];
            return _boneNameCache[boneName];
        }
        public void RegenerateBoneCache()
        {
            _boneNameCache.Clear();
            _boneIndexCache.Clear();
            _physicsDrivableBones.Clear();
            _cameraBones.Clear();
            if (RootBones != null)
                foreach (Bone b in RootBones)
                    b.CollectChildBones(this);
        }
        internal void WorldMatrixChanged()
        {
            //_cullingVolume.SetRenderTransform(_owningComponent == null ? Matrix4.Identity : _owningComponent.WorldMatrix);
        }
        public void Render()
        {
            //_cullingVolume.Render();
            foreach (Bone b in BoneNameCache.Values)
            {
                Vec3 point = b.WorldMatrix.GetPoint();
                Engine.Renderer.RenderPoint(point, b.Parent == null ? Color.Orange : Color.Purple, 15.0f);
                if (b.Parent != null)
                    Engine.Renderer.RenderLine(point, b.Parent.WorldMatrix.GetPoint(), Color.Blue, 5.0f);
                //float scale = AbstractRenderer.CurrentCamera.DistanceScale(point, 2.0f);
                //Engine.Renderer.RenderLine(point, Vec3.TransformPosition(Vec3.Up * scale, b.WorldMatrix), Color.Red, 5.0f);
                //Engine.Renderer.RenderLine(point, Vec3.TransformPosition(Vec3.Right * scale, b.WorldMatrix), Color.Green, 5.0f);
                //Engine.Renderer.RenderLine(point, Vec3.TransformPosition(Vec3.Forward * scale, b.WorldMatrix), Color.Blue, 5.0f);
            }
        }
        [Browsable(false)]
        internal int BillboardBoneCount => _cameraBones.Count;
        internal void AddCameraBone(Bone bone)
        {
            _cameraBones.Add(bone);
        }
        internal void RemoveCameraBone(Bone bone)
        {
            _cameraBones.Remove(bone);
        }
        internal void AddPhysicsBone(Bone bone)
        {
            _physicsDrivableBones.Add(bone);
        }
        internal void RemovePhysicsBone(Bone bone)
        {
            _physicsDrivableBones.Remove(bone);
        }
        public void TriggerChildFrameMatrixUpdate()
        {
            //_childMatrixModified = true;
        }
    }
}
