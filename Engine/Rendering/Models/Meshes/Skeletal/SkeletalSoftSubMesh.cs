using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class SkeletalSoftSubMesh : FileObject, ISkeletalMesh
    {
        public override ResourceType ResourceType { get { return ResourceType.SkeletalSoftSubMesh; } }

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
        protected string _boneName;

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

        public PrimitiveManager PrimitiveManager { get { return _manager; } }
        public string SingleBindName { get { return _boneName; } }

        public void SetPrimitiveData(PrimitiveData data) => _manager.Data = data;
        public void SetCullingVolume(Shape volume) { _cullingVolume = volume; }

        public void Render()
        {
            
        }
    }
}
