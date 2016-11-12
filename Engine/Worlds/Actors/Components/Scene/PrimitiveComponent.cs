using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class PrimitiveComponent<T> : SceneComponent where T : IPrimitive
    {
        protected T _primitive;

        public override void OnSpawned()
        {
            base.OnSpawned();
            _primitive.OnSpawned();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            _primitive.OnDespawned();
        }
    }
}
