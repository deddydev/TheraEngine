﻿using TheraEngine.Actors;
using TheraEngine.ComponentModel;

namespace TheraEngine.Components
{
    /// <summary>
    /// Logic components define specific functionality that dictates how an <see cref="Actor{T}"/> should operate.
    /// </summary>
    public interface ILogicComponent : IComponent { }
    /// <summary>
    /// Logic components define specific functionality that dictates how an <see cref="Actor{T}"/> should operate.
    /// </summary>
    [TFileExt("lcomp")]
    public abstract class LogicComponent : Component, ILogicComponent { }
}
