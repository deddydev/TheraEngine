using System;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using System.IO;
using System.Xml;

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
            get => _cullingVolume;
            set => _cullingVolume = value;
        }
        public bool VisibleByDefault => _visibleByDefault;
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

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
