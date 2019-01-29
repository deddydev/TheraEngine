using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering.Models
{
    [TFileExt("skel")]
    [TFileDef("Model Skeleton")]
    public class Skeleton : TFileObject, IEnumerable<Bone>, I3DRenderable
    {
        public RenderInfo3D RenderInfo { get; }
            = new RenderInfo3D(false, false) { CastsShadows = false, ReceivesShadows = false };

        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        public Skeleton() : base()
        {
            _rc = new RenderCommandMethod3D(ERenderPass.OnTopForward, Render);
        }
        public Skeleton(params Bone[] rootBones) : base()
        {
            _rc = new RenderCommandMethod3D(ERenderPass.OnTopForward, Render);
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
            _rc = new RenderCommandMethod3D(ERenderPass.OnTopForward, Render);
            RootBones = new Bone[1] { rootBone };
            rootBone.CalcBindMatrix(true);
            rootBone.TriggerFrameMatrixUpdate();
            RegenerateBoneCache();
        }

        public Bone this[string name]
            => BoneNameCache.ContainsKey(name) ? BoneNameCache[name] : null;
        public Bone this[int index] 
            => BoneIndexCache.ContainsKey(index) ? BoneIndexCache[index] : null;
        
        //Internal usage information, not serialized
        private List<Bone> _physicsDrivableBones = new List<Bone>();
        private List<Bone> _cameraBones = new List<Bone>();

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
        public Dictionary<string, Bone> BoneNameCache { get; } = new Dictionary<string, Bone>();

        [Browsable(false)]
        public Dictionary<int, Bone> BoneIndexCache { get; } = new Dictionary<int, Bone>();
        [Browsable(false)]
        public SkeletalMeshComponent OwningComponent { get; set; }

        public IEnumerator<Bone> GetEnumerator() 
            => ((IEnumerable<Bone>)BoneNameCache.Values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() 
            => ((IEnumerable<Bone>)BoneNameCache.Values).GetEnumerator();
        
        public IReadOnlyCollection<Bone> GetCameraRelativeBones() => _cameraBones;
        public IReadOnlyCollection<Bone> GetPhysicsDrivableBones() => _physicsDrivableBones;

        [TPostDeserialize]
        internal void PostDeserialize()
        {
            //TriggerChildFrameMatrixUpdate();
            RegenerateBoneCache();
            //UpdateBones(null, Matrix4.Identity, Matrix4.Identity);
        }

        public Bone GetBone(string boneName)
        {
            if (!BoneNameCache.ContainsKey(boneName))
                return RootBones[0];
            return BoneNameCache[boneName];
        }
        public void RegenerateBoneCache()
        {
            BoneNameCache.Clear();
            BoneIndexCache.Clear();
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
        //RenderingParameters p = new RenderingParameters()
        //{
        //    DepthTest = new DepthTest()
        //    {
        //        Enabled = false,
        //    },
        //};
        public void Render()
        {
            //_cullingVolume.Render();
            foreach (Bone b in BoneNameCache.Values)
            {
                Vec3 point = b.WorldMatrix.Translation;
                Engine.Renderer.RenderPoint(point, b.Parent == null ? Color.Orange : Color.Purple, false, 5.0f);
                if (b.Parent != null)
                    Engine.Renderer.RenderLine(point, b.Parent.WorldMatrix.Translation, Color.Blue, false, 1.0f);
                //float scale = AbstractRenderer.CurrentCamera.DistanceScale(point, 2.0f);
                //Engine.Renderer.RenderLine(point, Vec3.TransformPosition(Vec3.Up * scale, b.WorldMatrix), Color.Red, 5.0f);
                //Engine.Renderer.RenderLine(point, Vec3.TransformPosition(Vec3.Right * scale, b.WorldMatrix), Color.Green, 5.0f);
                //Engine.Renderer.RenderLine(point, Vec3.TransformPosition(Vec3.Forward * scale, b.WorldMatrix), Color.Blue, 5.0f);
            }
        }
        [Browsable(false)]
        internal int BillboardBoneCount => _cameraBones.Count;

        [Browsable(false)]
        public Scene3D OwningScene3D => OwningComponent?.OwningScene3D;

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
        //public void TriggerChildFrameMatrixUpdate()
        //{
        //    //_childMatrixModified = true;
        //}

        private readonly RenderCommandMethod3D _rc;
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_rc);
        }

        private Dictionary<int, IPrimitiveManager> _managers = new Dictionary<int, IPrimitiveManager>();
        internal void AddPrimitiveManager(IPrimitiveManager m)
        {
            if (!_managers.ContainsKey(m.BindingId))
                _managers.Add(m.BindingId, m);
        }
        internal void RemovePrimitiveManager(IPrimitiveManager m)
        {
            if (_managers.ContainsKey(m.BindingId))
                _managers.Remove(m.BindingId);
        }
        internal void SwapBuffers()
        {
            foreach (IPrimitiveManager m in _managers.Values)
            {
                m.SwapModifiedBuffers();
            }
        }
        public void UpdateBones(Camera c, Matrix4 worldMatrix, Matrix4 inverseWorldMatrix)
        {
            //Looping recursively is more efficient than looping through the whole bone cache
            //because bone subtrees that do not need updating will be skipped entirely
            //instead of being iterated through
            if (RootBones != null)
                foreach (Bone b in RootBones)
                    b.CalcFrameMatrix(c, worldMatrix, inverseWorldMatrix);
        }

    }
}
