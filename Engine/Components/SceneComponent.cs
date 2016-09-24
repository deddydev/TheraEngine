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

        public FrameState Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public IEnumerator<SceneComponent> GetEnumerator()
        {
            return ((IEnumerable<SceneComponent>)_childComponents).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<SceneComponent>)_childComponents).GetEnumerator();
        }

        public void Render()
        {
            GL.PushMatrix();
            Transform.MultMatrix();
            OnRender();
            foreach (SceneComponent comp in _childComponents)
                comp.Render();
            GL.PopMatrix();
        }

        protected virtual void OnRender() { }
    }
}
