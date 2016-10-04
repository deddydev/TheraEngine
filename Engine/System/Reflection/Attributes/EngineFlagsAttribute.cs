namespace System
{
    [Flags]
    public enum EEngineFlags
    {
        //Means this property contains nothing at first. Used for states usually
        Transient = 0x1,
        //Means this property contains an initial value
        Default = 0x2,
        //Means this property only gets something and cannot set it
        Getter = 0x4,
        //Means this property can be changed by the game at runtime
        Animatable = 0x8,
        //Means this property is only used by the editor
        EditorOnly = 0x10,
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