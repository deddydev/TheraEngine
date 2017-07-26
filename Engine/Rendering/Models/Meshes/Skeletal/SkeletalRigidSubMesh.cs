﻿using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.ComponentModel;
using System;

namespace TheraEngine.Rendering.Models
{
    [FileClass("OBJ", "Skeletal Rigid Sub Mesh")]
    public class SkeletalRigidSubMesh : FileObject, ISkeletalSubMesh
    {
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPassType3D.OpaqueDeferredLit, null);

        public SkeletalRigidSubMesh()
        {
            _name = "RigidSubMesh";
            _material = null;
            _data = null;
        }
        public SkeletalRigidSubMesh(string name, PrimitiveData data, Material material, bool visibleByDefault = true)
        {
            _data = data;
            _material = material;
            _name = name;
            _visibleByDefault = visibleByDefault;
        }
        
        [Serialize("Primitives")]
        protected SingleFileRef<PrimitiveData> _data;
        protected SingleFileRef<Material> _material;
     
        protected bool _visibleByDefault;

        [Serialize(IsXmlAttribute = true)]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
        [Serialize]
        public Material Material
        {
            get => _material;
            set => _material = value;
        }
        public SingleFileRef<PrimitiveData> Data => _data;
    }
}
