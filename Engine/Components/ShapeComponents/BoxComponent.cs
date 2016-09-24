using CustomEngine.Rendering.Meshes;
using System;
using System.ComponentModel;
using System.Reflection;

namespace CustomEngine.Components
{
    public class BoxComponent : SceneComponent
    {
        public Box Box
        {
            get { return _box; }
            set
            {
                _box = value;
                Changed(MethodBase.GetCurrentMethod());
            }
        }
        private Box _box;

        protected override void OnRender()
        {
            base.OnRender();
            _box?.Render();
        }
    }
}
