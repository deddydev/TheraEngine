using CustomEngine.Files;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Skeleton : FileObject
    {
        public Dictionary<string, Bone> _boneCache;

        private Bone _rootBone;
        public Bone RootBone
        {
            get { return _rootBone; }
            set { _rootBone = value; }
        }
        public void CalcFrameMatrices()
        {
            if (_rootBone != null)
                _rootBone.CalcFrameMatrix();
        }
        internal override void Tick(float delta)
        {
            base.Tick(delta);
        }
    }
}
