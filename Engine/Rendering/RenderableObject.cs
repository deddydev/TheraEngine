using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering.Models.Materials;
using System.Collections;

namespace CustomEngine.Rendering
{
    public interface IRenderableObjectContainer
    {
        GenericPrimitiveComponent LinkedComponent { get; set; }
        List<PrimitiveData> GetPrimitives();
        Matrix4 GetWorldMatrix();
        Matrix4 GetInverseWorldMatrix();
        void OnSpawned();
        void OnDespawned();
    }
    public abstract class RenderableObjectContainer<T> : FileObject, IRenderableObjectContainer, IEnumerable<T> where T : RenderableObject
    {
        protected GenericPrimitiveComponent _linkedComponent;
        protected MonitoredList<T> _children = new MonitoredList<T>();

        public RenderableObjectContainer() : base()
        {
            _children.Removed += ChildRemoved;
            _children.Added += ChildAdded;
        }
        protected virtual void ChildAdded(T item) { }
        protected virtual void ChildRemoved(T item) { }
        public MonitoredList<T> Children { get { return _children; } }
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
        public virtual List<PrimitiveData> GetPrimitives()
        {
            return _children.Select(x => x.GetPrimitiveData()).ToList();
        }
        public virtual Matrix4 GetWorldMatrix()
        {
            return LinkedComponent == null ? Matrix4.Identity : LinkedComponent.WorldMatrix;
        }
        public virtual Matrix4 GetInverseWorldMatrix()
        {
            return LinkedComponent == null ? Matrix4.Identity : LinkedComponent.InverseWorldMatrix;
        }
        public virtual void OnSpawned()
        {
            _children.ForEach(x => x.OnSpawned());
        }
        public virtual void OnDespawned()
        {
            _children.ForEach(x => x.OnDespawned());
        }

        public IEnumerator<T> GetEnumerator() { return ((IEnumerable<T>)_children).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<T>)_children).GetEnumerator(); }
    }
    public abstract class RenderableObject : RenderableObjectContainer<RenderableObject>
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
            set { _material = value; OnMaterialChanged(); }
        }
        protected virtual void OnMaterialChanged() { }
        public abstract void Render();
        public abstract Shape GetCullingVolume();
        public abstract PrimitiveData GetPrimitiveData();
        public override List<PrimitiveData> GetPrimitives()
        {
            return new List<PrimitiveData>() { GetPrimitiveData() };
        }
        public override void OnSpawned()
        {
            base.OnSpawned();
            if (Material != null)
                Material.AddReference(this);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            if (Material != null)
                Material.RemoveReference(this);
        }
    }
}
