namespace System
{
    [Flags]
    public enum EEngineFlags
    {
        //Means this property contains nothing at first
        Transient,
        //Means this property contains the state of something while the game is going.
        State,
        //Means this property contains an initial value
        Default,
        //Means this property only gets something and cannot set it
        Getter, 
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