using CustomEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class SceneComponent : Component, ITransformable
    {
        public SceneComponent()
        {
            _children.Added += _children_Added;
            _children.AddedRange += _children_AddedRange;
            _children.Inserted += _children_Inserted;
            _children.InsertedRange += _children_InsertedRange;
            _children.Removed += _children_Removed;
            _children.RemovedRange += _children_RemovedRange;
        }

        protected Matrix4 _invWorldTransform = Matrix4.Identity;
        protected Matrix4 _worldTransform = Matrix4.Identity;
        protected Matrix4 _localTransform = Matrix4.Identity;
        protected Matrix4 _invLocalTransform = Matrix4.Identity;

        protected SceneComponent _parent;
        protected MonitoredList<SceneComponent> _children = new MonitoredList<SceneComponent>();
        protected bool _visible;
        protected bool _isRendering = false;

        public MonitoredList<SceneComponent> Children
        {
            get { return _children; }
            set
            {
                _children.Clear();
                _children = value ?? new MonitoredList<SceneComponent>();
            }
        }

        public Matrix4 ParentMatrix { get { return Parent == null ? Matrix4.Identity : Parent.WorldMatrix; } }
        public Matrix4 ParentInvMatrix { get { return Parent == null ? Matrix4.Identity : Parent.InverseWorldMatrix; } }

        /// <summary>
        /// Gets the transformation of this component in relation to the actor's root component.
        /// </summary>
        public Matrix4 GetActorTransform() { return WorldMatrix * Owner.InverseWorldMatrix; }
        /// <summary>
        /// Gets the inverse transformation of this component in relation to the actor's root component.
        /// </summary>
        public Matrix4 GetInvActorTransform() { return InverseWorldMatrix * Owner.WorldMatrix; }

        [Category("Rendering"), State]
        public bool IsRendering { get { return _isRendering; } }
        [Category("Rendering"), State]
        public bool IsSpawned { get { return Owner.IsSpawned; } }
        [Category("Rendering"), Default, State, Animatable]
        public SceneComponent Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                    _parent._children.Remove(this);
                if (value != null)
                    value._children.Add(this);
                else
                {
                    _parent = null;
                    Owner?.GenerateSceneComponentCache();
                    Owner = null;
                }
            }
        }
        [Category("Rendering"), Default, State, Animatable]
        public virtual bool Visible
        {
            get {  return _visible; }
            set { _visible = value; }
        }
        public override void OnSpawned()
        {
            Visible = true;
            foreach (SceneComponent c in _children)
                c.OnSpawned();
        }
        public override void OnDespawned()
        {
            Visible = false;
            foreach (SceneComponent c in _children)
                c.OnDespawned();
        }
        public abstract void RecalcLocalTransform();
        public void RecalcGlobalTransform()
        {
            _worldTransform = ParentMatrix * LocalMatrix;
            _invWorldTransform = InverseLocalMatrix * ParentInvMatrix;
            foreach (SceneComponent c in _children)
                c.RecalcGlobalTransform();
        }
        public override Actor Owner
        {
            get { return base.Owner; }
            set
            {
                base.Owner = value;
                foreach (SceneComponent c in _children)
                    c.Owner = value;
            }
        }
        
        public Matrix4 WorldMatrix { get { return _worldTransform; } }
        public Matrix4 InverseWorldMatrix { get { return _invWorldTransform; } }
        public Matrix4 LocalMatrix { get { return _localTransform; } }
        public Matrix4 InverseLocalMatrix { get { return _invLocalTransform; } }

        public void SetLocalMatrix(Matrix4 matrix, Matrix4 inverse)
        {
            _localTransform = matrix;
            _invLocalTransform = inverse;
            RecalcGlobalTransform();
        }
        public void SetLocalMatrix(Matrix4 matrix)
        {
            _localTransform = matrix;
            _invLocalTransform = matrix.Inverted();
            RecalcGlobalTransform();
        }

        public List<SceneComponent> GenerateChildCache()
        {
            List<SceneComponent> cache = new List<SceneComponent>();
            GenerateChildCache(cache);
            return cache;
        }
        protected virtual void GenerateChildCache(List<SceneComponent> cache)
        {
            cache.Add(this);
            foreach (SceneComponent c in _children)
                c.GenerateChildCache(cache);
        }
        private void _children_RemovedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = null;
                item.Owner = null;
            }
            Owner?.GenerateSceneComponentCache();
        }
        private void _children_Removed(SceneComponent item)
        {
            item._parent = null;
            item.Owner = null;
            Owner?.GenerateSceneComponentCache();
        }
        private void _children_InsertedRange(IEnumerable<SceneComponent> items, int index) => _children_AddedRange(items);
        private void _children_Inserted(SceneComponent item, int index) => _children_Added(item);
        private void _children_AddedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = this;
                item.Owner = Owner;
            }
            Owner?.GenerateSceneComponentCache();
        }

        private void _children_Added(SceneComponent item)
        {
            item._parent = this;
            item.Owner = Owner;
            Owner?.GenerateSceneComponentCache();
        }
    }
}
