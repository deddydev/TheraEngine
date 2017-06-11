﻿using System;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.IO;
using System.Xml;

namespace TheraEngine.Rendering.Models
{
    public class StaticRigidSubMesh : FileObject, IStaticSubMesh
    {
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
            get => _cullingVolume;
            set => _cullingVolume = value;
        }
        public bool Visible => _visibleByDefault;
        public Material Material
        {
            get => _material;
            set => _material = value;
        }
        public PrimitiveData Data => _data;
        public StaticMesh Model
        {
            get => _parent;
            internal set => _parent = value;
        }
    }
}
