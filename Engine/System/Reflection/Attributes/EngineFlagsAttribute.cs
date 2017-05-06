using CustomEngine;
using CustomEngine.Files;

namespace System
{
    /// <summary>
    /// This property is only used by the editor, NOT the game.
    /// </summary>
    public class EditorOnly : Attribute
    {
        
    }
    /// <summary>
    /// This property can be animated.
    /// </summary>
    public class Animatable : Attribute
    {

    }
}