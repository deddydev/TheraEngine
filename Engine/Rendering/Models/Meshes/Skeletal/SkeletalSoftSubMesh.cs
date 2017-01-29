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
            _data = data;
            _name = name;
        }

        protected SkeletalMesh _parent;
        protected PrimitiveData _data;
        protected Material _material;
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
            get { return _material; }
            set { _material = value; }
        }
        public PrimitiveData Data
        {
            get { return _data; }
        }
        public SkeletalMesh Model
        {
            get { return _parent; }
            internal set { _parent = value; }
        }
        
        public string SingleBindName { get { return _boneName; } }
        
        public void Render()
        {
            
        }
    }
}
