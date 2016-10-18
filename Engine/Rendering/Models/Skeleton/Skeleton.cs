using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Skeleton : ObjectBase
    {
        public Dictionary<string, Bone> _boneCache;

        private Bone _rootBone = new Bone();

        [PreChanged("PreLink"), PostChanged("PostLink")]
        public Bone RootBone
        {
            get { return _rootBone; }
            set { _rootBone = value ?? new Bone(); }
        }

        protected void PreLink()
        {
            
        }
        protected void PostLink(Bone previous)
        {

        }

        public void CalcFrameMatrices()
        {
            if (_rootBone != null)
                _rootBone.CalcFrameMatrix();
        }
    }
}
