using TheraEngine;
using TheraEngine.Files;
using System.IO;
using System.ComponentModel;

namespace TheraEditor
{
    /// <summary>
    /// Extension of the game class for use with the editor.
    /// </summary>
    [FileClass("TPROJ", "Thera Engine Project", PreferredFormat = SerializeFormat.XML)]
    public class Project : Game
    {
        public const string BinDirName = "Bin\\";
        public const string ConfigDirName = "Config\\";
        public const string SourceDirName = "Source\\";
        public const string ContentDirName = "Content\\";

        private SingleFileRef<ProjectState> _state;

        [Serialize]
        [Browsable(false)]
        public ProjectState State
        {
            get => _state;
            set => _state = value;
        }
        public void SetDirectory(string directory)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";
            FilePath = GetFilePath(directory, Name, FileFormat.XML, typeof(Project));
            _state.ReferencePath = GetFilePath(ConfigDirName, Name, FileFormat.XML, typeof(ProjectState));
            _userSettings.ReferencePath = GetFilePath(ConfigDirName, Name, FileFormat.XML, typeof(UserSettings));
            _engineSettings.ReferencePath = GetFilePath(ConfigDirName, Name, FileFormat.XML, typeof(EngineSettings));
        }
        public static Project Create(string name)
        {
            Project p = new Project()
            {
                Name = name,
                State = new SingleFileRef<ProjectState>(new ProjectState()),
                UserSettings = new SingleFileRef<UserSettings>(new UserSettings()),
                EngineSettings = new SingleFileRef<EngineSettings>(new EngineSettings()),
            };
            return p;
        }
        public static Project Create(string directory, string name)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";
            string compileDir = directory + BinDirName + "\\";
            string configDir = directory + ConfigDirName + "\\";
            string sourceDir = directory + SourceDirName + "\\";
            string contentDir = directory + ContentDirName + "\\";
            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(configDir);
            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(contentDir);
            ProjectState state = new ProjectState();
            UserSettings userSettings = new UserSettings();
            EngineSettings engineSettings = new EngineSettings();
            Project p = new Project()
            {
                Name = name,
                FilePath = GetFilePath(directory, name, FileFormat.XML, typeof(Project)),
                State = new SingleFileRef<ProjectState>(state, directory, name, FileFormat.XML, true),
                UserSettings = new SingleFileRef<UserSettings>(userSettings, directory, name, FileFormat.XML, true),
                EngineSettings = new SingleFileRef<EngineSettings>(engineSettings, directory, name, FileFormat.XML, true),
            };
            p.Export();
            return p;
        }
    }
}