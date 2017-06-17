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
    /// <summary>
    /// Extension of the game class for use with the editor.
    /// </summary>
    [FileClass("TPROJ", "Game Project")]
    public class Project : Game
    {
        public const string BinDirName = "Bin";
        public const string ConfigDirName = "Config";
        public const string SourceDirName = "Source";
        public const string ContentDirName = "Content";

        private SingleFileRef<ProjectState> _state;

        [Serialize]
        [Browsable(false)]
        public ProjectState State
        {
            get => _state;
            set => _state = value;
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
                State = new SingleFileRef<ProjectState>(new ProjectState(), directory, name, FileFormat.XML),
                UserSettings = new SingleFileRef<UserSettings>(new UserSettings(), directory, name, FileFormat.XML),
                EngineSettings = new SingleFileRef<EngineSettings>(new EngineSettings(), directory, name, FileFormat.XML),
            };
            return p;
        }
    }
}