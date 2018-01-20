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
        private GlobalFileRef<EditorSettings> _editorSettings;

        [Browsable(false)]
        public ProjectState ProjectState
        {
            get => _state.File;
            set => _state.File = value;
        }
        [TSerialize]
        [Browsable(false)]
        public GlobalFileRef<ProjectState> ProjectStateRef
        {
            get => _state;
            set => _state = value;
        }
        [Browsable(false)]
        public EditorSettings EditorSettings
        {
            get => _editorSettings.File;
            set => _editorSettings.File = value;
        }
        [TSerialize]
        [Browsable(false)]
        public GlobalFileRef<EditorSettings> EditorSettingsRef
        {
            get => _editorSettings;
            set => _editorSettings = value;
        }

        public void SetDirectory(string directory)
        {
            if (!directory.EndsWithDirectorySeparator())
                directory += Path.DirectorySeparatorChar;

            FilePath = GetFilePath(directory, Name, ProprietaryFileFormat.XML, typeof(Project));
            ProjectStateRef.ReferencePath = GetFilePath(ConfigDirName, Name, ProprietaryFileFormat.XML, typeof(ProjectState));
            UserSettingsRef.ReferencePath = GetFilePath(ConfigDirName, Name, ProprietaryFileFormat.XML, typeof(UserSettings));
            EngineSettingsRef.ReferencePath = GetFilePath(ConfigDirName, Name, ProprietaryFileFormat.XML, typeof(EngineSettings));
            EditorSettingsRef.ReferencePath = GetFilePath(ConfigDirName, Name, ProprietaryFileFormat.XML, typeof(EditorSettings));
        }
        public static Project Create(string name)
        {
            Project p = new Project()
            {
                Name = name,
                ProjectStateRef = new ProjectState(),
                UserSettingsRef = new UserSettings(),
                EngineSettingsRef = new EngineSettings(),
                EditorSettingsRef = new EditorSettings()
                {

                },
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
            EditorSettings editorSettings = new EditorSettings();

            Project p = new Project()
            {
                Name = name,
                FilePath = GetFilePath<Project>(directory, name, ProprietaryFileFormat.XML),
                ProjectStateRef = new GlobalFileRef<ProjectState>(directory, "ProjectState", ProprietaryFileFormat.XML, state, true),
                UserSettingsRef = new GlobalFileRef<UserSettings>(configDir, "UserSettings", ProprietaryFileFormat.XML, userSettings, true),
                EngineSettingsRef = new GlobalFileRef<EngineSettings>(configDir, "EngineSettings", ProprietaryFileFormat.XML, engineSettings, true),
                EditorSettingsRef = new GlobalFileRef<EditorSettings>(configDir, "EditorSettings", ProprietaryFileFormat.XML, editorSettings, true),
            };
            p.Export();
            return p;
        }
    }
}