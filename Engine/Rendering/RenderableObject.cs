using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Rendering
{
    public abstract class RenderableObjectContainer : FileObject
    {
        private GenericPrimitiveComponent _linkedComponent;
        public GenericPrimitiveComponent LinkedComponent
        {
            get { return _linkedComponent; }
            set
            {
                if (_linkedComponent == value)
                    return;
                GenericPrimitiveComponent oldComp = _linkedComponent;
                _linkedComponent = value;
                if (oldComp != null)
                    oldComp.Primitive = null;
                if (_linkedComponent != null)
                    _linkedComponent.Primitive = this;
            }
        }
        public abstract List<PrimitiveData> GetPrimitives();
        public abstract Matrix4 GetWorldMatrix();
        public abstract Matrix4 GetInverseWorldMatrix();
    }
    public abstract class RenderableObject : RenderableObjectContainer
    {
        protected Material _material;
        protected bool _isRendering = true;
        public bool IsRendering
        {
            get { return _isRendering; }
            set { _isRendering = value; }
        }
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public abstract void Render();
        public abstract Shape GetCullingVolume();
        public abstract PrimitiveData GetPrimitiveData();
        public override List<PrimitiveData> GetPrimitives()
        {
            return new List<PrimitiveData>() { GetPrimitiveData() };
        }
        public virtual void OnSpawned()
        {
            if (Material != null)
                Material.AddReference(this);
        }
        public virtual void OnDespawned()
        {
            if (Material != null)
                Material.RemoveReference(this);
        }
    }
}
