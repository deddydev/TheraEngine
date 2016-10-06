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
        protected bool _visibleByDefault = true;
        protected bool _hiddenInGame = false;
        protected bool _overrideParentRenderState = false;
        protected bool _isRendering = false;

        [Category("Rendering"), Default, EditorOnly]
        public bool HiddenInGame { get { return _hiddenInGame; } set { _hiddenInGame = value; } }
        [Category("Rendering"), Default]
        public bool VisibleByDefault { get { return _visibleByDefault; } set { _visibleByDefault = value; } }
        [Category("Rendering"), State]
        public bool IsSpawned { get { return Owner.IsSpawned; } }
        [Category("Rendering"), State, Animatable]
        public bool IsRendering { get { return _isRendering; } set { _isRendering = value; } }
        [Category("Rendering"), State, Animatable]
        public FrameState Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public virtual void OnSpawned()
        {
            _isRendering = _hiddenInGame ? false : _visibleByDefault;
        }
        public virtual void OnDespawned()
        {
            _isRendering = false;
        }

        public void Render()
        {
            Renderer.PushMatrix();
            Transform.MultMatrix();
            OnRender();
            foreach (SceneComponent comp in Children)
                comp.Render();
            Renderer.PopMatrix();
        }

        protected override void OnChildAdded(ObjectBase obj)
        {
            base.OnChildAdded(obj);

            ((SceneComponent)obj).Owner = Owner;
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
