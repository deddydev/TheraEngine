using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors;
using System.IO;
using System.Xml;

namespace CustomEngine.Rendering.Models
{
    public class SkeletalRigidSubMesh : FileObject, ISkeletalMesh
    {
        public SkeletalRigidSubMesh()
        {
            _material = null;
            _data = null;
            _cullingVolume = new Sphere(1.0f);
            _name = "Mesh";
        }
        public SkeletalRigidSubMesh(PrimitiveData data, Shape cullingVolume, Material material, string boneName, string name)
        {
            _data = data;
            _material = material;
            _cullingVolume = cullingVolume;
            _name = name;
        }

        protected SkeletalMesh _parent;
        [Serialize("Primitives")]
        protected PrimitiveData _data;
        protected Material _material;
        [Serialize("CullingVolume")]
        protected Shape _cullingVolume;
        [Serialize("Visible")]
        protected bool _visible;

        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }
        public Shape CullingVolume
        {
            get => _cullingVolume;
            set => _cullingVolume = value;
        }
        public Material Material
        {
            get => _material;
            set => _material = value;
        }
        public PrimitiveData Data => _data;
        public SkeletalMesh Model
        {
            get => _parent;
            internal set => _parent = value;
        }

        public string SingleBindName => _data._singleBindBone;
    }
}
