using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class PrimitiveComponent<T> : GenericSceneComponent where T : IPrimitive
    {
        protected T _primitive;
    }
}
