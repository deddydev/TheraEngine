using CustomEngine.Rendering.Meshes;
using System.ComponentModel;
using System.Reflection;

namespace CustomEngine.Components
{
    public class ModelComponent : SceneComponent
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
        protected override void OnRender() { _model?.Render(); }
    }
}
