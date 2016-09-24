using CustomEngine.Rendering.Meshes;
using System;
using System.ComponentModel;
using System.Reflection;

namespace CustomEngine.Components
{
    public class CapsuleComponent : SceneComponent
    {
        public Capsule Capsule
        {
            get { return _capsule; }
            set
            {
                _capsule = value;
                Changed(MethodBase.GetCurrentMethod());
            }
        }
        private Capsule _capsule;

        protected override void OnRender()
        {
            base.OnRender();
            _capsule?.Render();
        }
    }
}
