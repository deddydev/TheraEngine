using CustomEngine;
using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace TheraEditor
{
    public class Project : FileObject
    {
        public ProjectState State
        {
            get => _state;
            set => _state = value;
        }
        public EngineSettings EngineSettings
        {
            get => _engineSettings;
            set => _engineSettings = value;
        }
        public UserSettings UserSettings
        {
            get => _userSettings;
            set => _userSettings = value;
        }

        [Serialize("State")]
        private ProjectState _state;
        [Serialize("EngineSettings")]
        private EngineSettings _engineSettings;
        [Serialize("UserSettings")]
        private UserSettings _userSettings;

        //public override void Read(VoidPtr address, VoidPtr strings)
        //{

        //}
        //public override void Read(XMLReader reader)
        //{

        //}
        //public override void Write(VoidPtr address, StringTable table)
        //{

        //}
        //public override void Write(XmlWriter writer)
        //{

        //}
        //protected override int OnCalculateSize(StringTable table)
        //{
        //    return EngineSettings.Header.Size + UserSettings.Header.Size;
        //}
    }
}
