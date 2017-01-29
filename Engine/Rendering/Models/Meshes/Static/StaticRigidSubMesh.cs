using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class StaticRigidSubMesh : FileObject, IStaticMesh
    {
        public override ResourceType ResourceType { get { return ResourceType.StaticRigidSubMesh; } }

        public StaticRigidSubMesh()
        {
            _material = null;
            _data = null;
            _cullingVolume = new Sphere(1.0f);
            _name = "Mesh";
        }
        public StaticRigidSubMesh(PrimitiveData data, Shape cullingVolume, Material material, string name)
        {
            _data = data;
            _material = material;
            _cullingVolume = cullingVolume;
            _name = name;
        }

        protected StaticMesh _parent;
        protected PrimitiveData _data;
        protected Material _material;
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
            get { return _material; }
            set { _material = value; }
        }
        public PrimitiveData Data
        {
            get { return _data; }
        }
        public StaticMesh Model
        {
            get { return _parent; }
            internal set { _parent = value; }
        }
    }
}
