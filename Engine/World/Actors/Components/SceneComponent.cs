using CustomEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Collections;

namespace CustomEngine.Worlds.Actors.Components
{
    public class SceneComponent : Component, IRenderable, ITransformable, IEnumerable<SceneComponent>
    {
        private FrameState _transform;

        private SceneComponent _parent;
        private List<SceneComponent> _childComponents = new List<SceneComponent>();

        protected bool _visibleByDefault = true;
        protected bool _hiddenInGame = false;
        protected bool _overrideParentRenderState = false;
        protected bool _isRendering = false;

        [EditorOnly]
        public bool HiddenInGame { get { return _hiddenInGame; } set { _hiddenInGame = value; } }

        public bool IsSpawned { get { return Owner.IsSpawned; } }
        public bool VisibleByDefault { get { return _visibleByDefault; } set { _visibleByDefault = value; } }
        public bool IsRendering { get { return _isRendering; } set { _isRendering = value; } }
        public FrameState Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public virtual void OnSpawned() { _isRendering = _hiddenInGame ? false : _visibleByDefault; }
        public virtual void OnDespawned() { _isRendering = false; }

        public void Render()
        {
            Renderer.PushMatrix();
            Transform.MultMatrix();
            OnRender();
            foreach (SceneComponent comp in _childComponents)
                comp.Render();
            Renderer.PopMatrix();
        }

        public void AddChildComponent(SceneComponent comp)
        {
            _childComponents.Add(comp);
            comp._parent = this;
            comp.Owner = Owner;
            Owner.GenerateSceneComponentCache();
        }

        public List<SceneComponent> GenerateChildCache()
        {
            List<SceneComponent> cache = new List<SceneComponent>();
            cache.Add(this);
            foreach (SceneComponent c in _childComponents)
                c.GenerateChildCache(cache);
            return cache;
                
        }
        private void GenerateChildCache(List<SceneComponent> cache)
        {
            cache.Add(this);
            foreach (SceneComponent c in _childComponents)
                c.GenerateChildCache(cache);
        }

        protected virtual void OnRender()
        {
            //Do nothing - this component is only used to transform the components it owns
        }

        public IEnumerator<SceneComponent> GetEnumerator() { return ((IEnumerable<SceneComponent>)_childComponents).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<SceneComponent>)_childComponents).GetEnumerator(); }
    }
}
