using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class SkeletalRigidSubMesh : FileObject, ISkeletalMesh
    {
        public SkeletalRigidSubMesh()
        {
            _manager = new PrimitiveManager();
            _cullingVolume = new Sphere(1.0f);
            _name = "Mesh";
        }
        public SkeletalRigidSubMesh(PrimitiveData data, Shape cullingVolume, Material material, string boneName, string name)
        {
            _manager = new PrimitiveManager(data, material);
            _cullingVolume = cullingVolume;
            _name = name;
            _boneName = boneName;
        }

        protected SkeletalMesh _parent;
        //private Matrix4 _normalMatrix;
        internal PrimitiveManager _manager;

        protected string _boneName;
        protected Shape _cullingVolume;
        protected bool _visibleByDefault = true, _renderSolid;
        
        public Shape CullingVolume
        {
            get { return _cullingVolume; }
            set { _cullingVolume = value; }
        }
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
        public PrimitiveManager PrimitiveManager { get { return _manager; } }

        public Bone SingleBind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void SetPrimitiveData(PrimitiveData data) => _manager.Data = data;
        public void SetCullingVolume(Shape volume) { _cullingVolume = volume; }
    }
}
