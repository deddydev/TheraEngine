using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class SkeletalSoftSubMesh : FileObject, IMesh
    {
        public SkeletalSoftSubMesh() { }
        public SkeletalSoftSubMesh(PrimitiveData data, string name)
        {
            _manager.Data = data;
            _name = name;
        }

        protected SkeletalMesh _parent;
        //private Matrix4 _normalMatrix;
        internal PrimitiveManager _manager = new PrimitiveManager();
        protected Shape _cullingVolume;
        protected bool _visibleByDefault;

        public Shape CullingVolume { get { return _cullingVolume; } }
        public bool VisibleByDefault
        {
            get { return _visibleByDefault; }
        }
        public Material Material
        {
            get { return _manager.Material; }
            set { _manager.Material = value; }
        }
        public SkeletalMesh Model
        {
            get { return _parent; }
            internal set { _parent = value; }
        }

        public PrimitiveManager PrimitiveManager
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsRendering
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public RenderOctree.OctreeNode RenderNode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Visible
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetPrimitiveData(PrimitiveData data) => _manager.Data = data;
        public void SetCullingVolume(Shape volume) { _cullingVolume = volume; }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
