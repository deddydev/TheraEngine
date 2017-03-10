﻿using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;
using System.IO;
using System.Xml;

namespace CustomEngine.Rendering.Models
{
    public class StaticSoftSubMesh : FileObject, IStaticMesh
    {
        public override ResourceType ResourceType { get { return ResourceType.StaticSoftSubMesh; } }

        public StaticSoftSubMesh() { }
        public StaticSoftSubMesh(PrimitiveData data, string name)
        {
            _material = null;
            _data = null;
            _name = name;
        }

        protected StaticMesh _parent;
        protected PrimitiveData _data;
        protected Material _material;
        protected Shape _cullingVolume;
        protected bool _isRendering, _isVisible, _visibleByDefault, _renderSolid;

        public Shape CullingVolume { get { return _cullingVolume; } }
        public bool IsRendering
        {
            get { return _isRendering; }
            set { _isRendering = value; }
        }
        public bool VisibleByDefault
        {
            get { return _visibleByDefault; }
        }
        public bool Visible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
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
