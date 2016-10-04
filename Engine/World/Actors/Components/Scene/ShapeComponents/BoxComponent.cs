using System;
using System.Reflection;

namespace CustomEngine.Worlds.Actors.Components
{
    public class BoxComponent : ShapeComponent
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
