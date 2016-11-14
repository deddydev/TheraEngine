using CustomEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public class SceneComponent : Component, ITransformable
    {
        public SceneComponent(Matrix4.MultiplyOrder order = Matrix4.MultiplyOrder.SRT)
        {
            _localTransform = FrameState.GetIdentity(order);
            _localTransform.MatrixChanged += _localTransform_MatrixChanged;
            _children.Added += _children_Added;
            _children.AddedRange += _children_AddedRange;
            _children.Inserted += _children_Inserted;
            _children.InsertedRange += _children_InsertedRange;
            _children.Removed += _children_Removed;
            _children.RemovedRange += _children_RemovedRange;
        }

        private Matrix4 _invWorldTransform = Matrix4.Identity;
        private Matrix4 _worldTransform = Matrix4.Identity;
        private FrameState _localTransform = FrameState.Identity;
        protected SceneComponent _parent;
        protected MonitoredList<SceneComponent> _children = new MonitoredList<SceneComponent>();
        protected bool _visible;
        protected bool _isRendering = false;

        /// <summary>
        /// Gets the transformation of this component in relation to the actor's root component.
        /// </summary>
        public Matrix4 GetActorTransform() { return Owner.Transform.InverseMatrix * _worldTransform; }
        /// <summary>
        /// Gets the inverse transformation of this component in relation to the actor's root component.
        /// </summary>
        public Matrix4 GetInvActorTransform() { return Owner.Transform.Matrix * _invWorldTransform; }

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
        [Category("Rendering"), Default, State, Animatable]
        public FrameState LocalTransform
        {
            get { return _localTransform; }
            set { _localTransform = value; }
        }
        [Category("Rendering"), Default, State, Animatable]
        public Matrix4.MultiplyOrder TransformOrder
        {
            get { return _localTransform.TransformOrder; }
            set { _localTransform.TransformOrder = value; }
        }
        public virtual void OnSpawned()
        {
            Visible = true;
            foreach (SceneComponent c in _children)
                c.OnSpawned();
        }
        public virtual void OnDespawned()
        {
            Visible = false;
            foreach (SceneComponent c in _children)
                c.OnDespawned();
        }
        private void _localTransform_MatrixChanged(Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            RecalcGlobalTransform();
        }
        public void RecalcGlobalTransform()
        {
            Matrix4 parentTransform = _parent == null ? Matrix4.Identity : _parent._worldTransform;
            Matrix4 parentInvTransform = _parent == null ? Matrix4.Identity : _parent._worldTransform;
            _worldTransform = _localTransform.Matrix * parentTransform;
            _invWorldTransform = _localTransform.InverseMatrix * parentInvTransform;
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
