using TheraEngine;
using TheraEngine.Files;
using System.IO;
using System.ComponentModel;
using System;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Microsoft.Build.Evaluation;
using System.Collections.Generic;
using Microsoft.Build.Execution;
using Microsoft.Build.Logging;
using Microsoft.Build.Framework;
using TheraEditor.Windows.Forms;
using System.Threading.Tasks;
using EnvDTE100;

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
        public static readonly string TempDirName = "Temp" + Path.DirectorySeparatorChar.ToString();

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
        public static Project Create(string directory, string name)
        {
            if (!directory.EndsWithDirectorySeparator())
                directory += Path.DirectorySeparatorChar;

            string binariesDir = directory + BinDirName + Path.DirectorySeparatorChar;
            string configDir = directory + ConfigDirName + Path.DirectorySeparatorChar;
            string sourceDir = directory + SourceDirName + Path.DirectorySeparatorChar;
            string contentDir = directory + ContentDirName + Path.DirectorySeparatorChar;
            string tempDir = directory + TempDirName + Path.DirectorySeparatorChar;

            Directory.CreateDirectory(binariesDir);
            Directory.CreateDirectory(configDir);
            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(contentDir);
            Directory.CreateDirectory(tempDir);

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

        public void GenerateSolution() => GenerateSolution(Path.Combine(DirectoryPath, SourceDirName));
        public void GenerateSolution(string dir)
        {
            Task.Run(() =>
            {
                EnvDTE80.DTE2 dte = VisualStudioManager.CreateVSInstance();
                dte.SuppressUI = true;
                Solution4 sln = (Solution4)dte.Solution;
                sln.Create(dir, Name);
                string projPath = GenerateGameProject(dir, dte, sln);
                sln.AddFromFile(projPath, false);
                sln.SaveAs(Path.Combine(dir, Name + ".sln"));
                VisualStudioManager.VSInstanceClosed();
            });

            //Dictionary<string, string> props = new Dictionary<string, string>
            //{
            //    { "Configuration", "Debug" },
            //    { "Platform", "x86" }
            //};
            //EngineLogger logger = new EngineLogger(LoggerVerbosity.Diagnostic);
            //ProjectCollection pc = new ProjectCollection(props, new ILogger[] { logger },
            //    ToolsetDefinitionLocations.ConfigurationFile | ToolsetDefinitionLocations.Registry);
            //BuildParameters buildParams = new BuildParameters(pc);
            //BuildRequestData buildRequest = new BuildRequestData(projectFileName, props, null, new string[] { "Build" }, null);
            //BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParams, buildRequest);
        }

        private string GenerateGameProject(string slnDir, EnvDTE80.DTE2 dte, Solution4 sln)
        {
            string projDir = Path.Combine(slnDir, Name);

            string projPath = Path.Combine(projDir, Name);

            return projPath;
        }

        private class EngineLogger : ILogger
        {
            public EngineLogger() { }
            public EngineLogger(LoggerVerbosity verbosity) => Verbosity = verbosity;

            public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;
            public string Parameters { get; set; }

            private IEventSource _source = null;

            public void Initialize(IEventSource eventSource)
            {
                if ((_source = eventSource) == null)
                    return;

                _source.BuildFinished += BuildFinishedHandler;
                _source.BuildStarted += BuildStartedHandler;
                _source.CustomEventRaised += CustomEventHandler;
                _source.ErrorRaised += ErrorHandler;
                _source.MessageRaised += MessageHandler;
                _source.ProjectFinished += ProjectFinishedHandler;
                _source.ProjectStarted += ProjectStartedHandler;
                _source.TargetFinished += TargetFinishedHandler;
                _source.TargetStarted += TargetStartedHandler;
                _source.TaskFinished += TaskFinishedHandler;
                _source.TaskStarted += TaskStartedHandler;
                _source.WarningRaised += WarningHandler;
            }

            public void Shutdown()
            {
                if (_source == null)
                    return;

                _source.BuildFinished -= BuildFinishedHandler;
                _source.BuildStarted -= BuildStartedHandler;
                _source.CustomEventRaised -= CustomEventHandler;
                _source.ErrorRaised -= ErrorHandler;
                _source.MessageRaised -= MessageHandler;
                _source.ProjectFinished -= ProjectFinishedHandler;
                _source.ProjectStarted -= ProjectStartedHandler;
                _source.TargetFinished -= TargetFinishedHandler;
                _source.TargetStarted -= TargetStartedHandler;
                _source.TaskFinished -= TaskFinishedHandler;
                _source.TaskStarted -= TaskStartedHandler;
                _source.WarningRaised -= WarningHandler;
            }
            
            public void BuildFinishedHandler(object sender, BuildFinishedEventArgs e)
            {
                
            }
            public void BuildStartedHandler(object sender, BuildStartedEventArgs e)
            {

            }
            public void CustomEventHandler(object sender, CustomBuildEventArgs e)
            {

            }
            public void ErrorHandler(object sender, BuildErrorEventArgs e)
            {

            }
            public void MessageHandler(object sender, BuildMessageEventArgs e)
            {

            }
            public void ProjectFinishedHandler(object sender, ProjectFinishedEventArgs e)
            {

            }
            public void ProjectStartedHandler(object sender, ProjectStartedEventArgs e)
            {

            }
            public void TargetFinishedHandler(object sender, TargetFinishedEventArgs e)
            {

            }
            public void TargetStartedHandler(object sender, TargetStartedEventArgs e)
            {

            }
            public void TaskFinishedHandler(object sender, TaskFinishedEventArgs e)
            {

            }
            public void TaskStartedHandler(object sender, TaskStartedEventArgs e)
            {

            }
            public void WarningHandler(object sender, BuildWarningEventArgs e)
            {

            }
        }
        public void Compile()
        {
            string projectFileName = @"...\ConsoleApplication3\ConsoleApplication3.sln";

            ProjectCollection pc = new ProjectCollection();
            Dictionary<string, string> GlobalProperty = new Dictionary<string, string>
            {
                { "Configuration", "Debug" },
                { "Platform", "x86" }
            };

            BuildParameters buildParams = new BuildParameters(pc);
            BuildRequestData buildRequest = new BuildRequestData(projectFileName, GlobalProperty, null, new string[] { "Build" }, null);
            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParams, buildRequest);

            //CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            //CompilerParameters parameters = new CompilerParameters
            //{
            //    GenerateExecutable = true,
            //    OutputAssembly = Path.Combine(FilePath, "Intermediate", Name + ".exe"),
            //};
            //CompilerResults results = codeProvider.CompileAssemblyFromFile(parameters, "");
            //if (results.Errors.Count > 0)
            //{
            //    foreach (CompilerError CompErr in results.Errors)
            //    {
            //        Engine.PrintLine("Line number {0}, Error Number: {1}, '{2};",
            //            CompErr.Line, CompErr.ErrorNumber, CompErr.ErrorText);
            //    }
            //}
        }
    }
}