using System.ComponentModel;
using TheraEngine.Actors;

namespace TheraEngine.Components
{
    public interface ILogicComponent : IComponent
    {

    }
    /// <summary>
    /// Logic components define specific functionality that dictates how an <see cref="Actor"/> should operate.
    /// </summary>
    [FileExt("lcomp")]
    public abstract class LogicComponent : Component, ILogicComponent { }
}
