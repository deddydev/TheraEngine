using CustomEngine.Rendering.Meshes;
using System.ComponentModel;
using System.Reflection;

namespace CustomEngine.Components
{
    public class ModelComponent : Component
    {
        public Model Model
        {
            get { return _model; }
            set
            {
                _model = value;
                Changed(MethodBase.GetCurrentMethod());
            }
        }
        private Model _model;

        public override void RenderTick(float deltaTime)
        {
            if (IsSpawned)
                _model?.Render();
        }
    }
}
