using CustomEngine.Rendering.Models;
using System;
using System.Reflection;

namespace CustomEngine.Worlds.Actors.Components
{
    public class ModelComponent : SceneComponent
    {
        private Model _model;
        
        public Model Model
        {
            get { return _model; }
            set
            {
                _model = value;
            }
        }

        protected override void OnRender() { _model?.Render(); }
    }
}
