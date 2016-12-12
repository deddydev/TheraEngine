using CustomEngine.Files;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Skeleton : FileObject
    {
        public Skeleton() : base() { }
        public Skeleton(Bone rootBone) : base() { RootBone = rootBone; }
        public Skeleton(Model model) : base() { Model = model; }
        public Skeleton(Model model, Bone rootBone) : base()
        {
            Model = model;
            RootBone = rootBone;
        }

        private Dictionary<string, Bone> _boneCache = new Dictionary<string, Bone>();
        private Model _model;
        private Bone _rootBone;
        public Bone RootBone
        {
            get { return _rootBone; }
            set
            {
                _rootBone = value;
                RegenerateBoneCache();
            }
        }
        public Model Model
        {
            get { return _model; }
            set { _model = value; }
        }
        public Dictionary<string, Bone> BoneCache { get { return _boneCache; } }
        public void RegenerateBoneCache()
        {
            _boneCache.Clear();
            _rootBone?.CollectChildBones(_boneCache, this);
        }
        public void CalcFrameMatrices()
        {
            _rootBone?.CalcFrameMatrix(Model.WorldMatrix, Model.InverseWorldMatrix);
        }
        internal override void Tick(float delta)
        {
            base.Tick(delta);
        }
    }
}
