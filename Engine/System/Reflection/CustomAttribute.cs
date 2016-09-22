using System;

namespace CustomEngine.Components
{
    [Flags]
    public enum EEngineFlags
    {
        Transient,
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