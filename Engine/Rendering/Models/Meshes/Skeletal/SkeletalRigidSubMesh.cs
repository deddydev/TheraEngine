﻿using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public class SkeletalRigidSubMesh : FileObject, ISkeletalSubMesh
    {
        public SkeletalRigidSubMesh()
        {
            _material = null;
            _data = null;
            _name = "Unnamed Mesh";
            _visible = true;
        }
        public SkeletalRigidSubMesh(PrimitiveData data, Material material, string name, bool visible = true)
        {
            _data = data;
            _material = material;
            _name = name;
            _visible = visible;
        }

        protected SkeletalMesh _parent;

        [Serialize("Primitives")]
        protected SingleFileRef<PrimitiveData> _data;
        protected Material _material;
        [Serialize("Visible", IsXmlAttribute = true)]
        protected bool _visible;

        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }
        public Material Material
        {
            get => _material;
            set => _material = value;
        }
        public SingleFileRef<PrimitiveData> Data => _data;
        public SkeletalMesh Model
        {
            get => _parent;
            internal set => _parent = value;
        }
    }
}
