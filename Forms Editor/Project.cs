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
            FilePath = GetFilePath(directory, Name, ProprietaryFileFormat.XML, typeof(Project));
            _state.ReferencePath = GetFilePath(ConfigDirName, Name, ProprietaryFileFormat.XML, typeof(ProjectState));
            UserSettings.ReferencePath = GetFilePath(ConfigDirName, Name, ProprietaryFileFormat.XML, typeof(UserSettings));
            EngineSettings.ReferencePath = GetFilePath(ConfigDirName, Name, ProprietaryFileFormat.XML, typeof(EngineSettings));
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
                FilePath = GetFilePath(directory, name, ProprietaryFileFormat.XML, typeof(Project)),
                State = new SingleFileRef<ProjectState>(directory, name, ProprietaryFileFormat.XML, state, true),
                UserSettings = new SingleFileRef<UserSettings>(directory, name, ProprietaryFileFormat.XML, userSettings, true),
                EngineSettings = new SingleFileRef<EngineSettings>(directory, name, ProprietaryFileFormat.XML, engineSettings, true),
            };
            p.Export();
            return p;
        }
    }
}