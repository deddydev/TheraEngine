using System;
using System.Reflection;

namespace CustomEngine.Worlds.Actors.Components
{
    public class ShapeComponent : SceneComponent
    {
        protected override void OnRender()
        {
            base.OnRender();
        }

        public virtual void OnOverlapped()
        {

        }
    }
}
