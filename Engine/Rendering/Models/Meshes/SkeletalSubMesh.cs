using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class SkeletalSubMesh : FileObject, IRenderable
    {
        public SkeletalSubMesh() { }
        public SkeletalSubMesh(PrimitiveData data, string name)
        {
            _manager.Data = data;
            _name = name;
        }

        protected SkeletalMesh _parent;
        //private Matrix4 _normalMatrix;
        internal PrimitiveManager _manager = new PrimitiveManager();
        protected Bone _singleBind;
        protected IShape _cullingVolume;
        protected bool _isRendering, _isVisible, _visibleByDefault, _renderSolid;

        public IShape CullingVolume { get { return _cullingVolume; } }
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
        public Bone SingleBind
        {
            get { return _singleBind; }
            set { _singleBind = value; }
        }
        public SkeletalMesh Model
        {
            get { return _parent; }
            internal set { _parent = value; }
        }
        public void Render()
        {
            if (!Visible || !IsRendering)
                return;

            _manager.Render(SingleBind.WorldMatrix, SingleBind.InverseWorldMatrix.Transposed());
        }
        internal void OnSpawned()
        {
            if (Material != null)
                Material.AddReference(this);
        }
        internal void OnDespawned()
        {
            if (Material != null)
                Material.RemoveReference(this);
        }
        public void SetPrimitiveData(PrimitiveData data) => _manager.Data = data;
        public void SetCullingVolume(BoundingShape volume) { _cullingVolume = volume; }
    }
}
