using BulletSharp;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Collections;

namespace CustomEngine.Rendering.Models
{
    public class Skeleton : FileObject, IEnumerable<Bone>
    {
        public Skeleton() : base()
        {

        }
        public Skeleton(params Bone[] rootBones) : base()
        {
            RootBones = rootBones;
            RegenerateBoneCache();
        }
        public Skeleton(Bone rootBone) : base()
        {
            RootBones = new Bone[1] { rootBone };
            RegenerateBoneCache();
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

        public IEnumerator<Bone> GetEnumerator()
        {
            return ((IEnumerable<Bone>)_boneCache.Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Bone>)_boneCache.Values).GetEnumerator();
        }
    }
}
