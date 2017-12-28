using TheraEngine;
using TheraEngine.Files;
using System.IO;
using System.ComponentModel;
using System;

namespace TheraEditor
{
    /// <summary>
    /// Extension of the game class for use with the editor.
    /// </summary>
    [FileExt("tproj", PreferredFormat = SerializeFormat.XML)]
    [FileDef("Thera Engine Project")]
    public class Project : Game
    {
        public static readonly string BinDirName = "Bin" + Path.DirectorySeparatorChar.ToString();
        public static readonly string ConfigDirName = "Config" + Path.DirectorySeparatorChar.ToString();
        public static readonly string SourceDirName = "Source" + Path.DirectorySeparatorChar.ToString();
        public static readonly string ContentDirName = "Content" + Path.DirectorySeparatorChar.ToString();

        private GlobalFileRef<ProjectState> _state;

        [TSerialize]
        [Browsable(false)]
        public ProjectState ProjectState
        {
            get => _state;
            set => _state = value;
        }
        public void SetDirectory(string directory)
        {
            if (!directory.EndsWithDirectorySeparator())
                directory += Path.DirectorySeparatorChar;

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
                ProjectState = new ProjectState(),
                UserSettings = new UserSettings(),
                EngineSettings = new EngineSettings(),
            };
            return p;
        }
        public static Project Create(string directory, string name)
        {
            if (!directory.EndsWithDirectorySeparator())
                directory += Path.DirectorySeparatorChar;

            string binariesDir = directory + BinDirName + Path.DirectorySeparatorChar;
            string configDir = directory + ConfigDirName + Path.DirectorySeparatorChar;
            string sourceDir = directory + SourceDirName + Path.DirectorySeparatorChar;
            string contentDir = directory + ContentDirName + Path.DirectorySeparatorChar;

            Directory.CreateDirectory(binariesDir);
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
                ProjectState = new GlobalFileRef<ProjectState>(directory, name, ProprietaryFileFormat.XML, state, true),
                UserSettings = new GlobalFileRef<UserSettings>(directory, name, ProprietaryFileFormat.XML, userSettings, true),
                EngineSettings = new GlobalFileRef<EngineSettings>(directory, name, ProprietaryFileFormat.XML, engineSettings, true),
            };
            p.Export();
            return p;
        }
    }
}