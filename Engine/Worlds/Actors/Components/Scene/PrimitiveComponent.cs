using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class PrimitiveComponent<T> : SceneComponent where T : IPrimitive
    {
        protected T _primitive;
    }
}
