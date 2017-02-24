using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;
using System.IO;
using System.Xml;

namespace CustomEngine.Rendering.Models
{
    public class SkeletalRigidSubMesh : FileObject, ISkeletalMesh
    {
        public override ResourceType ResourceType { get { return ResourceType.SkeletalRigidSubMesh; } }

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
            _boneName = boneName;
        }

        protected SkeletalMesh _parent;
        protected PrimitiveData _data;
        protected Material _material;
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
    }
}
