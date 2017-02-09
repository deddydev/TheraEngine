using BulletSharp;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Collections;

namespace CustomEngine.Rendering.Models
{
    public class Skeleton : FileObject, IEnumerable<Bone>, IRenderable
    {
        public override ResourceType ResourceType { get { return ResourceType.Skeleton; } }

        public Skeleton() : base() { }
        public Skeleton(params Bone[] rootBones) : base()
        {
            RootBones = rootBones;
            foreach (Bone b in RootBones)
            {
                b.CalcBindMatrix(true);
                b.CalcFrameMatrix();
            }
            RegenerateBoneCache();
        }
        public Skeleton(Bone rootBone) : base()
        {
            RootBones = new Bone[1] { rootBone };
            rootBone.CalcBindMatrix(true);
            rootBone.CalcFrameMatrix();
            RegenerateBoneCache();
        }
        public Dictionary<string, Bone> BoneCache
        {
            get { return _boneCache; }
        }

        private Dictionary<string, Bone> _boneCache = new Dictionary<string, Bone>();
        private SkeletalMeshComponent _owningComponent;
        private Bone[] _rootBones;
        public Bone[] RootBones
        {
            get { return _rootBones; }
            set
            {
                _rootBones = value;
                RegenerateBoneCache();
            }
        }
        public SkeletalMeshComponent OwningComponent
        {
            get { return _owningComponent; }
            set { _owningComponent = value; }
        }

        public Bone GetBone(string boneName)
        {
            if (!_boneCache.ContainsKey(boneName))
                return RootBones[0];
            return _boneCache[boneName];
        }

        public void RegenerateBoneCache()
        {
            _boneCache.Clear();
            foreach (Bone b in RootBones)
                b.CollectChildBones(_boneCache, this);
        }
        public void CalcFrameMatrices()
        {
            foreach (Bone b in RootBones)
                b.CalcFrameMatrix();
        }
        internal override void Tick(float delta)
        {
            base.Tick(delta);
        }

        public IEnumerator<Bone> GetEnumerator() { return ((IEnumerable<Bone>)_boneCache.Values).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Bone>)_boneCache.Values).GetEnumerator(); }

        bool _visible, _rendering;
        Shape _cullingVolume = new Sphere(1.0f);
        RenderOctree.Node _renderNode;
        public Shape CullingVolume { get { return _cullingVolume; } }
        public bool IsRendering
        {
            get { return _rendering; }
            set { _rendering = value; }
        }
        public RenderOctree.Node RenderNode
        {
            get { return _renderNode; }
            set { _renderNode = value; }
        }
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public void Render()
        {
            foreach (Bone b in BoneCache.Values)
            {
                Vec3 point = b.WorldMatrix.GetPoint();
                Engine.Renderer.RenderSphere(point, Engine.Renderer.Scene.CurrentCamera.DistanceScale(point, 0.2f), true);
                if (b.Parent != null)
                    Engine.Renderer.RenderLine(point, b.Parent.WorldMatrix.GetPoint());
            }
        }
    }
}
