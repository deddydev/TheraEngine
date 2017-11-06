﻿using System;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Models
{
    [FileClass("OBJ", "Static Rigid Sub Mesh")]
    public class StaticRigidSubMesh : FileObject, IStaticSubMesh
    {
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPassType3D.OpaqueDeferredLit, null);

        public StaticRigidSubMesh()
        {
            _name = "RigidSubMesh";
            _cullingVolume = null;
            _material = null;
            _data = null;
        }
        public StaticRigidSubMesh(string name, PrimitiveData data, Shape cullingVolume, Material material, bool visibleByDefault = true)
        {
            _cullingVolume = cullingVolume;
            _material = material;
            _data = data;
            _name = name;
            _visibleByDefault = visibleByDefault;
        }
        
        protected PrimitiveData _data;
        protected Material _material;
        protected Shape _cullingVolume;
        protected bool _visibleByDefault = true;

        [Serialize]
        public PrimitiveData Data
        {
            get => _data;
            set => _data = value;
        }
        [Serialize]
        public Shape CullingVolume
        {
            get => _cullingVolume;
            set => _cullingVolume = value;
        }
        [Serialize]
        public Material Material
        {
            get => _material;
            set => _material = value;
        }
        [Serialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
    }
}
