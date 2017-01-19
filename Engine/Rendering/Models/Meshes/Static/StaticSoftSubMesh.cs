using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class StaticSoftSubMesh : FileObject, IStaticMesh
    {
        public StaticSoftSubMesh() { }
        public StaticSoftSubMesh(PrimitiveData data, string name)
        {
            _manager.Data = data;
            _name = name;
        }

        protected StaticMesh _parent;
        //private Matrix4 _normalMatrix;
        internal PrimitiveManager _manager = new PrimitiveManager();
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
            get { return _manager.Material; }
            set { _manager.Material = value; }
        }
        public StaticMesh Model
        {
            get { return _parent; }
            internal set { _parent = value; }
        }

        public PrimitiveManager PrimitiveManager { get { return _manager; } }

        public void SetPrimitiveData(PrimitiveData data) => _manager.Data = data;
        public void SetCullingVolume(Shape volume) { _cullingVolume = volume; }
    }
}
