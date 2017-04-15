using System;
using System.IO;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Audio
{
    /// <summary>
    /// Plays equally in both speakers; not tied to any world position.
    /// </summary>
    public class AmbientSound : SoundDataBase
    {
        AmbientSound()
        {
            _looped = true;
        }
    }
}
