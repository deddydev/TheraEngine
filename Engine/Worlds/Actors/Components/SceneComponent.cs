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
        }

        private Matrix4 _invWorldTransform = Matrix4.Identity;
        private Matrix4 _worldTransform = Matrix4.Identity;
        private FrameState _localTransform = FrameState.Identity;
        protected SceneComponent _parent;
        protected MonitoredList<SceneComponent> _children = new MonitoredList<SceneComponent>();
        protected bool _visible;
        protected bool _hiddenInGame = false;
        protected bool _overrideParentRenderState = false;
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

        [Category("Rendering"), Default, EditorOnly]
        public bool HiddenInGame
        {
            get { return _hiddenInGame; }
            set { _hiddenInGame = value; }
        }

        [Category("Rendering"), Default, State, Animatable]
        public bool Visible
        {
            get { return _hiddenInGame ? false : _visible; }
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
            _visible = true;
        }
        public virtual void OnDespawned()
        {
            _visible = false;
        }
        private void _localTransform_MatrixChanged(Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            RecalcGlobalTransform();
        }
        private void RecalcGlobalTransform()
        {
            Matrix4 parentTransform = _parent == null ? Matrix4.Identity : _parent._worldTransform;
            Matrix4 parentInvTransform = _parent == null ? Matrix4.Identity : _parent._worldTransform;
            _worldTransform = _localTransform.Matrix * parentTransform;
            _invWorldTransform = _localTransform.InverseMatrix * parentInvTransform;
            foreach (SceneComponent c in _children)
                c.RecalcGlobalTransform();
        }
        protected void OnChildAdded(SceneComponent s)
        {
            s.Owner = Owner;
            Owner.GenerateSceneComponentCache();
        }

        public List<SceneComponent> GenerateChildCache()
        {
            List<SceneComponent> cache = new List<SceneComponent>();
            cache.Add(this);
            foreach (SceneComponent c in _children)
                c.GenerateChildCache(cache);
            return cache;
                
        }
        private void GenerateChildCache(List<SceneComponent> cache)
        {
            cache.Add(this);
            foreach (SceneComponent c in _children)
                c.GenerateChildCache(cache);
        }
    }
}
