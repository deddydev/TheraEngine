namespace System
{
    /// <summary>
    /// This property is only used by the editor, NOT the game.
    /// </summary>
    public class EditorOnly : Attribute
    {
        
    }
    /// <summary>
    /// This property is intended to be animated.
    /// </summary>
    public class Animatable : Attribute
    {

    }
    /// <summary>
    /// This property is set by default and not changed by the game.
    /// </summary>
    public class Default : Attribute
    {

    }
    /// <summary>
    /// This property references a state value that will be changed by the game.
    /// </summary>
    public class State : Attribute
    {

    }
}