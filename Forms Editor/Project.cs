using TheraEngine;
using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Drawing.Design;
using System.Design;
using System.ComponentModel.Design;

namespace TheraEditor
{
    [FileClass("TPROJ", "Game Project")]
    public class Project : FileObject
    {
        private SingleFileRef<ProjectState> _state;
        private SingleFileRef<EngineSettings> _engineSettings;
        private SingleFileRef<UserSettings> _userSettings;
        private string _description;
        private string _copyright;
        private string _credits;

        [Serialize]
        [Browsable(false)]
        public ProjectState State
        {
            get => _state;
            set => _state = value;
        }
        [Serialize]
        [Browsable(false)]
        public EngineSettings EngineSettings
        {
            get => _engineSettings;
            set => _engineSettings = value;
        }
        [Serialize]
        [Browsable(false)]
        public UserSettings UserSettings
        {
            get => _userSettings;
            set => _userSettings = value;
        }
        [Serialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Description
        {
            get => _description;
            set => _description = value;
        }
        [Serialize]
        public string Copyright
        {
            get => _copyright;
            set => _copyright = value;
        }
        [Serialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Credits
        {
            get => _credits;
            set => _credits = value;
        }
        public static Project New(string directory, string name)
        {
            Project p = new Project()
            {
                Name = name,
                FilePath = GetFilePath(directory, name, FileFormat.XML, typeof(Project)),
                State = new SingleFileRef<ProjectState>(new ProjectState(), directory, name, FileFormat.XML),
                UserSettings = new SingleFileRef<UserSettings>(new UserSettings(), directory, name, FileFormat.XML),
                EngineSettings = new SingleFileRef<EngineSettings>(new EngineSettings(), directory, name, FileFormat.XML),
            };
            return p;
        }
    }
}