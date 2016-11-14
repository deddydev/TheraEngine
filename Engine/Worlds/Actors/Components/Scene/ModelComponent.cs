using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public class ModelComponent : PrimitiveComponent<Model>
    {
        public ModelComponent() { }
        public ModelComponent(Model m) { Model = m; }

        public Model Model
        {
            get { return _primitive; }
            set
            {
                if (_primitive == value)
                    return;
                if (_primitive != null)
                    _primitive.UnlinkComponent(this);
                if ((_primitive = value) != null)
                    _primitive.LinkComponent(this);
            }
        }
        /// <summary>
        /// Visible means this mesh will never be rendered, or will be rendered if placed onscreen.
        /// AKA: long-term visibility
        /// </summary>
        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                if (_visible == value)
                    return;

                base.Visible = value;

                foreach (Mesh m in _primitive)
                {
                    if (_visible)
                        Engine.Renderer.Scene.AddRenderable(m);
                    else
                        Engine.Renderer.Scene.RemoveRenderable(m);
                }
            }
        }
        public override void OnSpawned()
        {
            base.OnSpawned();
            _primitive.OnSpawned();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            _primitive.OnDespawned();
        }
    }
}
