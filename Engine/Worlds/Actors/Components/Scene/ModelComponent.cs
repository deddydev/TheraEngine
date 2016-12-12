using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;
using System.ComponentModel;

namespace CustomEngine.Worlds.Actors.Components
{
    public class ModelComponent : PrimitiveComponent
    {
        public ModelComponent(Model m) { Model = m; }

        [Category("Rendering")]
        public Model Model
        {
            get { return (Model)Primitive; }
            set { Primitive = value; }
        }
        /// <summary>
        /// Visible means this mesh will never be rendered, or will be rendered if placed onscreen.
        /// AKA: long-term visibility
        /// </summary>
        [Category("Rendering")]
        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                if (_visible == value)
                    return;

                base.Visible = value;
                foreach (Mesh m in Model)
                    if (_visible)
                        Engine.Renderer.Scene.AddRenderable(m);
                    else
                        Engine.Renderer.Scene.RemoveRenderable(m);
            }
        }
        public override void OnSpawned()
        {
            base.OnSpawned();
            Model?.OnSpawned();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            Model?.OnDespawned();
        }
    }
}
