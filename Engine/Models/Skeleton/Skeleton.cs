using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering.Models
{
    public interface ISkeleton : IFileObject, IEnumerable<IBone>, I3DRenderable
    {
        IBone this[string name] { get; }
        IBone this[int index] { get; }

        IBone[] RootBones { get; set; }

        Dictionary<string, IBone> BoneNameCache { get; }
        Dictionary<int, IBone> BoneIndexCache { get; }
        SkeletalMeshComponent OwningComponent { get; set; }

        IReadOnlyCollection<IBone> CameraRelativeBones { get; }
        IReadOnlyCollection<IBone> PhysicsDrivableBones { get; }

        IBone GetBone(string boneName);
        void RegenerateBoneCache();

        IScene3D OwningScene3D { get; }

        void AddCameraBone(IBone bone);
        void RemoveCameraBone(IBone bone);
        void AddPhysicsBone(IBone bone);
        void RemovePhysicsBone(IBone bone);

        void AddPrimitiveManager(IPrimitiveManager m);
        void RemovePrimitiveManager(IPrimitiveManager m);
        void SwapBuffers();
        void UpdateBones(ICamera c, Matrix4 worldMatrix, Matrix4 inverseWorldMatrix);
    }
    [TFileExt("skel")]
    [TFileDef("Model Skeleton")]
    public class Skeleton : TFileObject, ISkeleton
    {
        public IRenderInfo3D RenderInfo { get; }
            = new RenderInfo3D(false, true) { CastsShadows = false, ReceivesShadows = false };
        
        public Skeleton() : base()
        {
            _rc = new RenderCommandMethod3D(ERenderPass.OnTopForward, Render);
        }
        public Skeleton(params IBone[] rootBones) : base()
        {
            _rc = new RenderCommandMethod3D(ERenderPass.OnTopForward, Render);
            RootBones = rootBones;
            foreach (IBone b in RootBones)
            {
                b.CalcBindMatrix(true);
                b.TriggerFrameMatrixUpdate();
            }
            RegenerateBoneCache();
        }
        public Skeleton(IBone rootBone) : base()
        {
            _rc = new RenderCommandMethod3D(ERenderPass.OnTopForward, Render);
            RootBones = new IBone[1] { rootBone };
            rootBone.CalcBindMatrix(true);
            rootBone.TriggerFrameMatrixUpdate();
            RegenerateBoneCache();
        }

        public IBone this[string name]
            => BoneNameCache.ContainsKey(name) ? BoneNameCache[name] : null;
        public IBone this[int index] 
            => BoneIndexCache.ContainsKey(index) ? BoneIndexCache[index] : null;
        
        //Internal usage information, not serialized
        private List<IBone> _physicsDrivableBones = new List<IBone>();
        private List<IBone> _cameraBones = new List<IBone>();

        //private bool _childMatrixModified = false;

        private IBone[] _rootBones;

        [TSerialize]
        public IBone[] RootBones
        {
            get => _rootBones;
            set
            {
                _rootBones = value;
                RegenerateBoneCache();
            }
        }
        [Browsable(false)]
        public Dictionary<string, IBone> BoneNameCache { get; } = new Dictionary<string, IBone>();

        [Browsable(false)]
        public Dictionary<int, IBone> BoneIndexCache { get; } = new Dictionary<int, IBone>();
        [Browsable(false)]
        public SkeletalMeshComponent OwningComponent { get; set; }

        public IEnumerator<IBone> GetEnumerator() 
            => ((IEnumerable<IBone>)BoneNameCache.Values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() 
            => ((IEnumerable<IBone>)BoneNameCache.Values).GetEnumerator();
        
        public IReadOnlyCollection<IBone> CameraRelativeBones => _cameraBones;
        public IReadOnlyCollection<IBone> PhysicsDrivableBones => _physicsDrivableBones;

        [TPostDeserialize]
        internal void PostDeserialize()
        {
            //TriggerChildFrameMatrixUpdate();
            RegenerateBoneCache();
            //UpdateBones(null, Matrix4.Identity, Matrix4.Identity);
        }

        public IBone GetBone(string boneName)
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
                foreach (IBone b in RootBones)
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
            foreach (IBone b in BoneNameCache.Values)
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
        public IScene3D OwningScene3D => OwningComponent?.OwningScene3D;

        void ISkeleton.AddCameraBone(IBone bone)
        {
            _cameraBones.Add(bone);
        }
        void ISkeleton.RemoveCameraBone(IBone bone)
        {
            _cameraBones.Remove(bone);
        }
        void ISkeleton.AddPhysicsBone(IBone bone)
        {
            _physicsDrivableBones.Add(bone);
        }
        void ISkeleton.RemovePhysicsBone(IBone bone)
        {
            _physicsDrivableBones.Remove(bone);
        }
        //public void TriggerChildFrameMatrixUpdate()
        //{
        //    //_childMatrixModified = true;
        //}

        private readonly RenderCommandMethod3D _rc;
        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            passes.Add(_rc);
        }

        private Dictionary<int, IPrimitiveManager> _managers = new Dictionary<int, IPrimitiveManager>();
        public void AddPrimitiveManager(IPrimitiveManager m)
        {
            if (!_managers.ContainsKey(m.BindingId))
                _managers.Add(m.BindingId, m);
        }
        public void RemovePrimitiveManager(IPrimitiveManager m)
        {
            if (_managers.ContainsKey(m.BindingId))
                _managers.Remove(m.BindingId);
        }
        public void SwapBuffers()
        {
            foreach (IPrimitiveManager m in _managers.Values)
            {
                m.SwapModifiedBuffers();
            }
        }
        public void UpdateBones(ICamera c, Matrix4 worldMatrix, Matrix4 inverseWorldMatrix)
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
