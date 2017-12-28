using TheraEngine;
using TheraEngine.Files;
using System.IO;
using System.ComponentModel;

namespace TheraEditor
{
    /// <summary>
    /// Extension of the game class for use with the editor.
    /// </summary>
    [FileDef("Game Project", "tproj", PreferredFormat = SerializeFormat.XML)]
    public class Project : Game
    {
        public const string BinDirName = "Bin\\";
        public const string ConfigDirName = "Config\\";
        public const string SourceDirName = "Source\\";
        public const string ContentDirName = "Content\\";

        private GlobalFileRef<ProjectState> _state;

        [TSerialize]
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
                State = new GlobalFileRef<ProjectState>(new ProjectState()),
                UserSettings = new GlobalFileRef<UserSettings>(new UserSettings()),
                EngineSettings = new GlobalFileRef<EngineSettings>(new EngineSettings()),
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
            Project p = new Project()
            {
                Name = name,
                FilePath = GetFilePath(directory, name, FileFormat.XML, typeof(Project)),
                State = new GlobalFileRef<ProjectState>(new ProjectState(), directory, name, FileFormat.XML),
                UserSettings = new GlobalFileRef<UserSettings>(new UserSettings(), directory, name, FileFormat.XML),
                EngineSettings = new GlobalFileRef<EngineSettings>(new EngineSettings(), directory, name, FileFormat.XML),
            };
            return p;
        }
    }
}