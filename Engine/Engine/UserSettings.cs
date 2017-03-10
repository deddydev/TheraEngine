﻿using CustomEngine.Files;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace CustomEngine
{
    public enum EngineQuality
    {
        Lowest,
        Low,
        Medium,
        High,
        Highest
    }
    public enum RenderLibrary
    {
        OpenGL,
        Direct3D11,
    }
    public enum AudioLibrary
    {
        OpenAL,
        DirectSound,
    }
    public enum InputLibrary
    {
        OpenTK,
        XInput,
        Raw,
    }
    [Serializable]
    public class UserSettings : FileObject
    {
        public override ResourceType ResourceType => ResourceType.UserSettings;

        public bool VSync = true;
        public EngineQuality TextureQuality = EngineQuality.Highest;
        public EngineQuality ModelQuality = EngineQuality.Highest;
        public EngineQuality SoundQuality = EngineQuality.Highest;
        public RenderLibrary RenderLibrary = RenderLibrary.OpenGL;
        public AudioLibrary AudioLibrary = AudioLibrary.OpenAL;
        public InputLibrary InputLibrary = InputLibrary.OpenTK;
        
        public static UserSettings FromXML(string filePath)
        {
            return FromXML<UserSettings>(filePath);
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
