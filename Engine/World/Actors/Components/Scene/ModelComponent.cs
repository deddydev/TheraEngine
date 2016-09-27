using CustomEngine.Rendering.Meshes;
using System;
using System.Reflection;

namespace CustomEngine.World.Actors.Components
{
    public class ModelComponent : SceneComponent
    {
        private Model _model;

        [EngineFlagsAttribute(EEngineFlags.Default)]
        public Model Model
        {
            get { return _model; }
            set
            {
                _model = value;
                Changed(MethodBase.GetCurrentMethod());
            }
        }

        protected override void OnRender() { _model?.Render(); }
    }
}
