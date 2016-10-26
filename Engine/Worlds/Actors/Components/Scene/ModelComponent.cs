using CustomEngine.Rendering.Models;
using System;
using System.Reflection;

namespace CustomEngine.Worlds.Actors.Components
{
    public class ModelComponent : SceneComponent
    {
        private Model _model;

        public ModelComponent() { }
        public ModelComponent(Model m) { Model = m; }

        public Model Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;
                if (_model != null)
                    _model.UnlinkComponent(this);
                if ((_model = value) != null)
                    _model.LinkComponent(this);
            }
        }

        protected override void OnRender(float delta) { _model?.Render(delta); }
    }
}
