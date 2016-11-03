using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class PrimitiveComponent<T> : SceneComponent where T : IPrimitive
    {
        protected T _primitive;

        protected override void OnRender(float delta) { _primitive?.Render(delta); }
    }
}
