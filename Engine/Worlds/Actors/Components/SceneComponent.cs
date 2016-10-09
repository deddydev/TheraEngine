using CustomEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace CustomEngine.Worlds.Actors.Components
{
    public class SceneComponent : Component, IRenderable, ITransformable
    {
        private FrameState _transform;
        protected MonitoredList<SceneComponent> _children = new MonitoredList<SceneComponent>();
        protected bool _visible;
        protected bool _hiddenInGame = false;
        protected bool _overrideParentRenderState = false;
        protected bool _isRendering = false;

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
        public FrameState Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public virtual void OnSpawned()
        {
            _visible = true;
        }
        public virtual void OnDespawned()
        {
            _visible = false;
        }

        public void Render()
        {
            Renderer.PushMatrix();
            Transform.MultMatrix();
            if (Visible)
                OnRender();
            foreach (SceneComponent comp in _children)
                comp.Render();
            Renderer.PopMatrix();
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
            foreach (SceneComponent c in Children)
                c.GenerateChildCache(cache);
            return cache;
                
        }
        private void GenerateChildCache(List<SceneComponent> cache)
        {
            cache.Add(this);
            foreach (SceneComponent c in Children)
                c.GenerateChildCache(cache);
        }

        protected virtual void OnRender()
        {
            //Do nothing - this component is only used to transform the components it owns
        }
    }
}
