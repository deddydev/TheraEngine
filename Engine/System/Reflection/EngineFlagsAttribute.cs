using System;

namespace CustomEngine.Components
{
    [Flags]
    public enum EEngineFlags
    {
        Transient, //Means this property contains nothing at first
        State, //Means this property contains the state of something while the game is going.
        Default, //Means this property contains an initial value
        Getter, //Means this property only gets something and cannot set it
    }
    public class EngineFlagsAttribute : Attribute
    {
        public EEngineFlags _flags;
        public EngineFlagsAttribute(EEngineFlags flags)
        {
            _flags = flags;
        }
    }
}