using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class SkeletalRigidSubMesh : FileObject, IRenderable
    {
        public SkeletalRigidSubMesh()
        {
            _manager = new PrimitiveManager();
            _cullingVolume = new Sphere(1.0f);
            _name = "Mesh";
        }
        public SkeletalRigidSubMesh(PrimitiveData data, Shape cullingVolume, Material material, string boneName, string name)
        {
            _manager = new PrimitiveManager(data, material);
            _cullingVolume = cullingVolume;
            _name = name;
            _boneName = boneName;
        }

        protected SkeletalMesh _parent;
        //private Matrix4 _normalMatrix;
        internal PrimitiveManager _manager;

        protected string _boneName;
        protected Bone _singleBind;

        protected Shape _cullingVolume;

        internal void SkeletonChanged(Skeleton skeleton)
        {
            _singleBind = skeleton != null ? skeleton.GetBone(_boneName) : null;
        }

        protected bool _isRendering, _isVisible, _visibleByDefault = true, _renderSolid;

        public Shape CullingVolume
        {
            get { return _cullingVolume; }
            set { _cullingVolume = value; }
        }
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
            //if (!Visible || !IsRendering)
            //    return;

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
        public void SetCullingVolume(Shape volume) { _cullingVolume = volume; }
    }
}
