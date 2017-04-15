using OpenTK;
using System.Reflection;
using System;
using CustomEngine.Files;
using System.IO;
using System.Xml;

namespace CustomEngine.Audio
{
    public class StaticSoundEffect : SoundDataBase
    {
        private Vector3 _worldPosition;
        private float _loudness;
    }
}
