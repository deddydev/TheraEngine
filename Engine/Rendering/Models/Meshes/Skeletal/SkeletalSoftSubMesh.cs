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
    public class SkeletalSoftSubMesh : FileObject, ISkeletalMesh
    {
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
        protected bool _visible;
        protected string _boneName;

        public Shape CullingVolume { get { return _cullingVolume; } }
        public bool Visible
        {
            get { return _visible; }
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
