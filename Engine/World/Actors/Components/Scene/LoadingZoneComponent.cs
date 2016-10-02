using System;
using System.Reflection;

namespace CustomEngine.Worlds.Actors.Components
{
    public class LoadingZoneComponent : BoxComponent
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
