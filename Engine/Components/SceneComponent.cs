using CustomEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CustomEngine.Components
{
    public class SceneComponent : Component, IRenderable, ITransformable, IEnumerable<SceneComponent>
    {
        private FrameState _transform;
        private List<SceneComponent> _childComponents = new List<SceneComponent>();

        private bool _visibleByDefault = true;
        private bool _hiddenInGame = false;
        private bool _overrideParentRenderState = false;
        private bool _isRendering = false;

        [EngineFlags(EEngineFlags.State | EEngineFlags.Getter)]
        public bool IsSpawned { get { return Owner.IsSpawned; } }
        [EngineFlags(EEngineFlags.Default)]
        public bool VisibleByDefault { get { return _visibleByDefault; } set { _visibleByDefault = value; } }
        [EngineFlags(EEngineFlags.Default)]
        public bool HiddenInGame { get { return _hiddenInGame; } set { _hiddenInGame = value; } }
        [EngineFlags(EEngineFlags.State)]
        public bool IsRendering { get { return _isRendering; } set { _isRendering = value; } }
        [EngineFlags(EEngineFlags.State)]
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

        protected virtual void OnRender() { }

        public IEnumerator<SceneComponent> GetEnumerator() { return ((IEnumerable<SceneComponent>)_childComponents).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<SceneComponent>)_childComponents).GetEnumerator(); }
    }
}
