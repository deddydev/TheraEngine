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
using BulletSharp;

namespace CustomEngine.Rendering
{
    public interface IRenderableObjectContainer
    {
        PrimitiveComponent LinkedComponent { get; set; }
        List<PrimitiveData> GetPrimitives();
        Matrix4 WorldMatrix { get; set; }
        Matrix4 InverseWorldMatrix { get; set; }
        void OnSpawned();
        void OnDespawned();
        RenderableObject[] GetChildren(bool visibleByDefaultOnly);
        RenderableObject[] GetVisibleChildren();
        RenderableObject[] GetHiddenChildren();
    }
    public abstract class RenderableObjectContainer<T> : FileObject, IRenderableObjectContainer, IEnumerable<T> where T : RenderableObject
    {
        private Matrix4 
            _worldMatrix = Matrix4.Identity,
            _inverseWorldMatrix = Matrix4.Identity;
        protected PrimitiveComponent _linkedComponent;
        protected MonitoredList<T> _children = new MonitoredList<T>();

        public RenderableObjectContainer() : base()
        {
            _children.Removed += ChildRemoved;
            _children.Added += ChildAdded;
        }
        public virtual Matrix4 WorldMatrix
        {
            get { return _worldMatrix; }
            set
            {
                _worldMatrix = value;
                foreach (T child in _children)
                    child.WorldMatrix = _worldMatrix;
            }
        }
        public virtual Matrix4 InverseWorldMatrix
        {
            get { return _inverseWorldMatrix; }
            set
            {
                _inverseWorldMatrix = value;
                foreach (T child in _children)
                    child.InverseWorldMatrix = _inverseWorldMatrix;
            }
        }
        public MonitoredList<T> Children { get { return _children; } }
        public PrimitiveComponent LinkedComponent
        {
            get { return _linkedComponent; }
            set
            {
                if (_linkedComponent == value)
                    return;
                PrimitiveComponent oldComp = _linkedComponent;
                _linkedComponent = value;
                if (oldComp != null)
                    oldComp.Primitive = null;
                if (_linkedComponent != null)
                {
                    _worldMatrix = _linkedComponent.WorldMatrix;
                    _inverseWorldMatrix = _linkedComponent.InverseLocalMatrix;
                    _linkedComponent.Primitive = this;
                }
                else
                {
                    _worldMatrix = Matrix4.Identity;
                    _inverseWorldMatrix = Matrix4.Identity;
                }
            }
        }
        public virtual List<PrimitiveData> GetPrimitives()
        {
            return _children.Select(x => x.GetPrimitiveData()).ToList();
        }
        protected virtual void ChildAdded(T item) { }
        protected virtual void ChildRemoved(T item) { }
        public virtual void OnSpawned()
        {
            _children.ForEach(x => x.OnSpawned());
        }
        public virtual void OnDespawned()
        {
            _children.ForEach(x => x.OnDespawned());
        }
        public RenderableObject[] GetChildren(bool visibleByDefaultOnly)
        {
            if (visibleByDefaultOnly)
                return _children.Select(x => x as RenderableObject).Where(x => x.VisibleByDefault).ToArray();
            else
                return _children.Select(x => x as RenderableObject).ToArray();
        }
        public RenderableObject[] GetVisibleChildren()
        {
            return _children.Select(x => x as RenderableObject).Where(x => x.Visible).ToArray();
        }
        public RenderableObject[] GetHiddenChildren()
        {
            return _children.Select(x => x as RenderableObject).Where(x => !x.Visible).ToArray();
        }
        public IEnumerator<T> GetEnumerator() { return ((IEnumerable<T>)_children).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<T>)_children).GetEnumerator(); }
    }
    public abstract class RenderableObject : RenderableObjectContainer<RenderableObject>
    {
        protected Material _material;
        protected bool _isRendering = true, _visible = true, _visibleByDefault = true;
        public bool IsRendering
        {
            get { return _isRendering; }
            set { _isRendering = value; }
        }
        public bool VisibleByDefault
        {
            get { return _visibleByDefault; }
        }
        public virtual Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public bool _collisionEnabled;
        protected Shape _cullingVolume;
        protected RigidBody _collision;
        public RigidBody CollisionObject
        {
            get { return _collision; }
            set
            {
                if (_collision != null)
                {
                    if (_collisionEnabled && Engine.World != null)
                        Engine.World.PhysicsScene.AddRigidBody(_collision);
                    _collision.UserObject = null;
                }
                _collision = value;
                if (_collision != null)
                {
                    if (_collisionEnabled && Engine.World != null)
                        Engine.World.PhysicsScene.AddRigidBody(_collision);
                    _collision.UserObject = this;
                }
            }
        }
        public virtual bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;

                if (_visible && _collision != null)
                    Engine.World.PhysicsScene.RemoveRigidBody(_collision);
                _visible = value;
                if (_visible && _collision != null)
                    Engine.World.PhysicsScene.AddRigidBody(_collision);
            }
        }

        public virtual Shape CullingVolume { get { return _cullingVolume; } }
        public abstract void Render();
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
