using EnvDTE100;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.XML;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.ThirdParty;
using static TheraEngine.ThirdParty.MSBuild;
using static TheraEngine.ThirdParty.MSBuild.Item;
using static TheraEngine.ThirdParty.MSBuild.Project;

namespace TheraEditor
{
    /// <summary>
    /// Extension of the game class for use with the editor.
    /// </summary>
    [TFileExt("tproj", PreferredFormat = EProprietaryFileFormat.XML)]
    [TFileDef("Thera Engine Project")]
    public class TProject : TGame
    {
        /// <summary>
        /// This is the global GUID for a C# project.
        /// </summary>
        public const string CSharpProjectGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

        [TSerialize(nameof(ProjectGuid))]
        protected Guid _projectGuid = Guid.NewGuid();

        public Guid ProjectGuid => _projectGuid;

        [TSerialize(State = true)]
        [Category("Code")]
        public string TargetBuildConfiguration { get; set; } = "Debug";
        [TSerialize(State = true)]
        [Category("Code")]
        public string TargetBuildPlatform { get; set; } = "x64";

        public string RootNamespace { get; set; } = null;
        public override string Name
        {
            get => base.Name;
            set
            {
                base.Name = value;
                if (string.IsNullOrWhiteSpace(RootNamespace))
                    RootNamespace = Name.ReplaceWhitespace("_");
            }
        }

        [Browsable(false)]
        public string SolutionPath => DirectoryPath == null ? null : Path.Combine(DirectoryPath, Name + ".sln");

        private string _localBinariesDirectory;
        private string _localSourceDirectory;
        private string _localConfigDirectory;
        private string _localContentDirectory;
        private string _localTempDirectory;

        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Paths")]
        public string LocalBinariesDirectory
        {
            get => _localBinariesDirectory;
            set
            {
                _localBinariesDirectory = value;
                BinariesDirectory = string.IsNullOrWhiteSpace(LocalBinariesDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalBinariesDirectory));
            }
        }
        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Paths")]
        public string LocalSourceDirectory
        {
            get => _localSourceDirectory;
            set
            {
                _localSourceDirectory = value;
                SourceDirectory = string.IsNullOrWhiteSpace(LocalSourceDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalSourceDirectory));
            }
        }
        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Paths")]
        public string LocalConfigDirectory
        {
            get => _localConfigDirectory;
            set
            {
                _localConfigDirectory = value;
                ConfigDirectory = string.IsNullOrWhiteSpace(LocalConfigDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalConfigDirectory));
            }
        }
        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Paths")]
        public string LocalContentDirectory
        {
            get => _localContentDirectory;
            set
            {
                _localContentDirectory = value;
                ContentDirectory = string.IsNullOrWhiteSpace(LocalContentDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalContentDirectory));
            }
        }
        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Paths")]
        public string LocalTempDirectory
        {
            get => _localTempDirectory;
            set
            {
                _localTempDirectory = value;
                TempDirectory = string.IsNullOrWhiteSpace(LocalTempDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalTempDirectory));
            }
        }
        [Browsable(false)]
        [TString(false, true, false)]
        [Category("Object")]
        public override string FilePath
        {
            get => base.FilePath;
            set
            {
                base.FilePath = value;
                UpdatePaths();
            }
        }
        private void UpdatePaths()
        {
            if (string.IsNullOrWhiteSpace(DirectoryPath))
            {
                BinariesDirectory = null;
                SourceDirectory = null;
                ConfigDirectory = null;
                ContentDirectory = null;
                TempDirectory = null;
            }
            else
            {
                BinariesDirectory = string.IsNullOrWhiteSpace(LocalBinariesDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalBinariesDirectory));
                SourceDirectory = string.IsNullOrWhiteSpace(LocalSourceDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalSourceDirectory));
                ConfigDirectory = string.IsNullOrWhiteSpace(LocalConfigDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalConfigDirectory));
                ContentDirectory = string.IsNullOrWhiteSpace(LocalContentDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalContentDirectory));
                TempDirectory = string.IsNullOrWhiteSpace(LocalTempDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalTempDirectory));
            }
        }

        [Browsable(false)]
        public string BinariesDirectory { get; private set; }
        [Browsable(false)]
        public string SourceDirectory { get; private set; }
        [Browsable(false)]
        public string ConfigDirectory { get; private set; }
        [Browsable(false)]
        public string ContentDirectory { get; private set; }
        [Browsable(false)]
        public string TempDirectory { get; private set; }

        [TSerialize(nameof(ProjectStateRef), State = true, Config = false)]
        private GlobalFileRef<ProjectState> _projectStateRef;
        [TSerialize(nameof(EditorSettingsOverrideRef))]
        private GlobalFileRef<EditorSettings> _editorSettingsRef;

        [Category("Project")]
        public GlobalFileRef<ProjectState> ProjectStateRef
        {
            get => _projectStateRef;
            set => _projectStateRef = value;
        }
        [Category("Project")]
        public GlobalFileRef<EditorSettings> EditorSettingsOverrideRef
        {
            get => _editorSettingsRef;
            set => _editorSettingsRef = value;
        }

        [Browsable(false)]
        public ProjectState ProjectState
        {
            get => ProjectStateRef?.File;
            set
            {
                if (ProjectStateRef != null)
                    ProjectStateRef.File = value;
                else
                    ProjectStateRef = value;
            }
        }
        [Browsable(false)]
        public EditorSettings EditorSettings
        {
            get => EditorSettingsOverrideRef?.File;
            set
            {
                if (EditorSettingsOverrideRef != null)
                    EditorSettingsOverrideRef.File = value;
                else
                    EditorSettingsOverrideRef = value;
            }
        }

        public void SetDirectoryDefaults(string directory)
        {
            if (!directory.EndsWithDirectorySeparator())
                directory += Path.DirectorySeparatorChar;

            string bin = "Bin" + Path.DirectorySeparatorChar;
            string cfg = "Config" + Path.DirectorySeparatorChar;
            string src = "Source" + Path.DirectorySeparatorChar;
            string ctt = "Content" + Path.DirectorySeparatorChar;
            string tmp = "Temp" + Path.DirectorySeparatorChar;

            string binDir = directory + bin;
            string cfgDir = directory + cfg;
            string srcDir = directory + src;
            string cttDir = directory + ctt;
            string tmpDir = directory + tmp;

            Directory.CreateDirectory(binDir);
            Directory.CreateDirectory(cfgDir);
            Directory.CreateDirectory(srcDir);
            Directory.CreateDirectory(cttDir);
            Directory.CreateDirectory(tmpDir);

            FilePath = GetFilePath<TProject>(directory, Name, EProprietaryFileFormat.XML);
            void Update<T>(ref GlobalFileRef<T> gref, string defaultName) where T : TFileObject, new()
            {
                if (gref == null)
                {
                    gref = new GlobalFileRef<T>(
                        directory, defaultName, EProprietaryFileFormat.XML, new T());
                }
                else
                {
                    gref.Path.Absolute = GetFilePath<T>(
                        directory, gref.File?.Name ?? defaultName, EProprietaryFileFormat.XML);
                }
            }

            Update(ref _projectStateRef, nameof(ProjectState));
            Update(ref _userSettingsRef, nameof(UserSettings));
            Update(ref _engineSettingsRef, nameof(EngineSettings));
            Update(ref _editorSettingsRef, nameof(EditorSettings));

            LocalBinariesDirectory = bin;
            LocalConfigDirectory = cfg;
            LocalSourceDirectory = src;
            LocalContentDirectory = ctt;
            LocalTempDirectory = tmp;
        }

        public static async Task<TProject> CreateAsync(
            string directory, string name,
            UserSettings userSettings,
            EngineSettings engineSettings,
            EditorSettings editorSettings)
        {
            if (!directory.EndsWithDirectorySeparator())
                directory += Path.DirectorySeparatorChar;

            string bin = "Bin" + Path.DirectorySeparatorChar;
            string cfg = "Config" + Path.DirectorySeparatorChar;
            string src = "Source" + Path.DirectorySeparatorChar;
            string ctt = "Content" + Path.DirectorySeparatorChar;
            string tmp = "Temp" + Path.DirectorySeparatorChar;

            string binDir = directory + bin;
            string cfgDir = directory + cfg;
            string srcDir = directory + src;
            string cttDir = directory + ctt;
            string tmpDir = directory + tmp;

            Directory.CreateDirectory(binDir);
            Directory.CreateDirectory(cfgDir);
            Directory.CreateDirectory(srcDir);
            Directory.CreateDirectory(cttDir);
            Directory.CreateDirectory(tmpDir);

            ProjectState state = new ProjectState();

            TProject project = new TProject()
            {
                Name = name,

                LocalBinariesDirectory = bin,
                LocalConfigDirectory = cfg,
                LocalSourceDirectory = src,
                LocalContentDirectory = ctt,
                LocalTempDirectory = tmp,

                FilePath
                    = GetFilePath<TProject>(directory, name,
                    EProprietaryFileFormat.XML),

                ProjectStateRef
                    = new GlobalFileRef<ProjectState>(directory, nameof(ProjectState),
                    EProprietaryFileFormat.XML, state),

                UserSettingsRef
                    = new GlobalFileRef<UserSettings>(cfgDir, nameof(UserSettings),
                    EProprietaryFileFormat.XML, userSettings),

                EngineSettingsOverrideRef
                    = new GlobalFileRef<EngineSettings>(cfgDir, nameof(EngineSettings),
                    EProprietaryFileFormat.XML, engineSettings),

                EditorSettingsOverrideRef
                    = new GlobalFileRef<EditorSettings>(cfgDir, nameof(EditorSettings),
                    EProprietaryFileFormat.XML, editorSettings),
            };

            await state.ExportAsync();
            await userSettings.ExportAsync();
            await engineSettings.ExportAsync();
            await editorSettings.ExportAsync();

            await project.ExportAsync();

            project.GenerateSolution(true);
            await project.CompileAsync();

            return project;
        }

        public void GenerateProgramCS()
        {
            //0 = game name
            //1 = game class name
            //2 = file object class name
            //3 = LoadAsync static method name
            //4 = Engine class name
            //5 = Run method in Engine class
            //6 = xtgame, game class extension (in XML)
            //7 = default namespace

            string progTmpl = File.ReadAllText(Path.Combine(Engine.Settings.EngineDataFolder, "ProgramTemplate.cs"));
            string ext = GetFileExtension<TGame>().GetFullExtension(EProprietaryFileFormat.XML);
            string progCs = string.Format(progTmpl,
                Name,
                nameof(TGame),
                nameof(TFileObject),
                nameof(TFileObject.LoadAsync),
                nameof(Engine),
                nameof(Engine.Run),
                ext,
                RootNamespace);
            File.WriteAllText(SourceDirectory + "Program.cs", progCs.Replace('@', '{').Replace('#', '}'));
        }

        public async void GenerateSolution(bool forceRegenerateProgramCS = false)
        {
            Process[] devenv = Process.GetProcessesByName("DevEnv");
            FileVersionInfo info = null;
            if (devenv != null && devenv.Length > 0)
            {
                Process devp = devenv[0];
                if (devp.Modules != null && devp.Modules.Count > 0)
                    info = devp.Modules[0].FileVersionInfo;
            }
            string ver = info?.ProductVersion ?? "15.7";
            int majorVer = info?.FileMajorPart ?? 15;
            int minorVer = info?.FileMinorPart ?? 7;
            string solutionGuid = Guid.ToString("B").ToUpperInvariant();
            string projectGuid = _projectGuid.ToString("B").ToUpperInvariant();

            if (forceRegenerateProgramCS || !File.Exists(SourceDirectory + "Program.cs"))
                GenerateProgramCS();

            #region csproj
            string csprojPath = Path.Combine(DirectoryPath, Name + ".csproj");
            MSBuild.Project p = new MSBuild.Project
            {
                ToolsVersion = majorVer.ToString() + "." + minorVer.ToString(),
                DefaultTargets = "Build",
                Schema = "http://schemas.microsoft.com/developer/msbuild/2003"
            };

            var import = new Import(
                "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props",
                "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')");

            const string targetNetVersion = "v4.7.2";
            const string defaultConfig = "Debug";
            const string defaultPlatform = "x64";
            const string appPropertiesFolder = "Properties"; //Relative to the csproj

            var mainInfo = PropertyGroup.Create(null,
                ("Configuration", defaultConfig, " '$(Configuration)' == '' "),
                ("Platform", defaultPlatform, " '$(Configuration)' == '' "),
                ("ProjectGuid", Guid.ToString("B").ToUpperInvariant(), null),
                ("OutputType", "WinExe", null),
                ("AppDesignerFolder", appPropertiesFolder, null),
                ("RootNamespace", RootNamespace, null),
                ("AssemblyName", Name, null),
                ("TargetFrameworkVersion", targetNetVersion, null),
                ("FileAlignment", "512", null),
                ("AutoGenerateBindingRedirects", "true", null),
                ("TargetFrameworkProfile", null, null));

            PropertyGroup CreateConfigPropGrp(bool debug, bool x86, bool allowUnsafeCode)
            {
                string config = debug ? "Debug" : "Release";
                string platform = x86 ? "x86" : "x64";
                return PropertyGroup.Create($"'$(Configuration)|$(Platform)' == '{config}|{platform}'",
                    ("DebugSymbols", "true", null),
                    ("OutputPath", Path.Combine(LocalBinariesDirectory, platform, config), null),
                    ("DefineConstants", debug ? "DEBUG;TRACE" : "TRACE", null),
                    ("Optimize", debug ? "false" : "true", null),
                    ("AllowUnsafeBlocks", allowUnsafeCode ? "true" : "false", null),
                    ("DebugType", debug ? "full" : "pdbonly", null),
                    ("PlatformTarget", $"{platform}", null),
                    ("ErrorReport", "prompt", null),
                    ("CodeAnalysisRuleSet", "MinimumRecommendedRules.ruleset", null),
                    ("Prefer32Bit", x86 ? "true" : "false", null),
                    ("LangVersion", "latest", null));
            }

            var debugx86 = CreateConfigPropGrp(true, true, true);
            var debugx64 = CreateConfigPropGrp(true, false, true);
            var releasex86 = CreateConfigPropGrp(false, true, true);
            var releasex64 = CreateConfigPropGrp(false, false, true);

            CollectFiles(
                out List<string> codeFiles,
                out List<string> contentFiles,
                out HashSet<string> references);

            //TODO: copy the thera engine dll to the game exe directory first
            Assembly engineAssembly = typeof(Engine).Assembly;
            string engineDLLPath = engineAssembly.CodeBase;
            if (engineDLLPath.StartsWith("file:///"))
                engineDLLPath = engineDLLPath.Substring(8);
            engineDLLPath = engineDLLPath.MakeAbsolutePathRelativeTo(csprojPath);
            if (engineDLLPath.StartsWith("\\"))
                engineDLLPath = engineDLLPath.Substring(1);

            ItemGroup refGrp = new ItemGroup();
            {
                Item theraEngineRef = new Item("Reference")
                {
                    Include = engineAssembly.GetName().ToString()
                };
                {
                    ItemMetadata specificVersion = new ItemMetadata
                    {
                        ElementName = "SpecificVersion",
                        StringContent = new ElementString("False"),
                    };
                    ItemMetadata hintPath = new ItemMetadata
                    {
                        ElementName = "HintPath",
                        StringContent = new ElementString(engineDLLPath),
                    };
                    theraEngineRef.AddElements(specificVersion, hintPath);
                }
                refGrp.AddElements(theraEngineRef);
            }
            refGrp.AddElements(references.OrderBy(x => x).Select(x => new Item("Reference") { Include = x }).ToArray());

            ItemGroup compileGrp = new ItemGroup();
            compileGrp.AddElements(codeFiles.OrderBy(x => x).Select(x => new Item("Compile") { Include = x }).ToArray());

            ItemGroup contentGrp = new ItemGroup();
            contentGrp.AddElements(contentFiles.OrderBy(x => x).Select(x => new Item("Content") { Include = x }).ToArray());

            //ItemGroup folderGrp = new ItemGroup();
            //folderGrp.AddElements(new Item("Folder") { Include = LocalSourceDirectory });

            Import csharpImport = new Import("$(MSBuildToolsPath)\\Microsoft.CSharp.targets", null);

            Target beforeBuild = new Target
            {
                Name = "BeforeBuild"
            };
            Target afterBuild = new Target
            {
                Name = "AfterBuild"
            };

            p.AddElements(
                import,
                mainInfo,
                debugx86,
                debugx64,
                releasex86,
                releasex64,
                refGrp,
                //folderGrp,
                compileGrp,
                contentGrp,
                csharpImport,
                beforeBuild,
                afterBuild);

            var def = new XMLSchemeDefinition<MSBuild.Project>();
            int op = Editor.Instance.BeginOperation("Exporting csproj...", out Progress<float> progress, out CancellationTokenSource cancel);
            await def.ExportAsync(csprojPath, p, progress, cancel.Token);
            Editor.Instance.EndOperation(op);
            #endregion

            #region sln
            //0 = project name, 1 = project GUID, 2 = project type
            //The first GUID is the GUID of a C# project package
            string projTmpl = "Project(\"{2}\") = \"{0}\", \"{0}.csproj\", \"{1}\"\nEndProject\n";
            //TODO: allow multiple projects
            string projects = string.Format(projTmpl, Name, projectGuid, CSharpProjectGuid);

            //0 = config name, 1 = platform name
            string preSolTmpl = "\t{0}|{1} = {0}|{1}\n\t";
            string preSol = string.Empty;
            void AppendPreSol(string config, string platform)
                => preSol += string.Format(preSolTmpl, config, platform);
            AppendPreSol("Debug", "x64");
            AppendPreSol("Debug", "x86");
            AppendPreSol("Release", "x64");
            AppendPreSol("Release", "x86");

            //0 = project GUID, 1 = config name, 2 = platform name
            string postSolTmpl = "\t{0}.{1}|{2}.ActiveCfg = {1}|{2}\n\t\t{0}.{1}|{2}.Build.0 = {1}|{2}\n\t";
            string postSol = string.Empty;
            void AppendPostSol(string guid, string config, string platform)
                => postSol += string.Format(postSolTmpl, guid, config, platform);
            AppendPostSol(projectGuid, "Debug", "x86");
            AppendPostSol(projectGuid, "Debug", "x64");
            AppendPostSol(projectGuid, "Release", "x86");
            AppendPostSol(projectGuid, "Release", "x64");

            //0 = projects, 1 = pre solution list, 2 = post solution list, 3 = solution GUID
            string slnTmpl = File.ReadAllText(Path.Combine(Engine.Settings.EngineDataFolder, "SolutionTemplate.sln"));
            string sln = string.Format(slnTmpl, projects, preSol, postSol, solutionGuid, majorVer.ToString(), ver);

            File.WriteAllText(SolutionPath, sln);
            #endregion

            //EnvDTE80.DTE2 dte = VisualStudioManager.CreateVSInstance();
            //dte.SuppressUI = true;
            //Solution4 sln = (Solution4)dte.Solution;
            //sln.Create(slnDir, Name);
            //string projDir = Path.Combine(slnDir, Name);
            //string projPath = Path.Combine(projDir, Name + ".csproj");
            //string csTemplatePath = sln.GetProjectTemplate("ConsoleApplication.zip", "CSharp");
            //EnvDTE.Project proj = sln.AddFromTemplate(csTemplatePath, projDir, Name, false);
            ////string projPath = GenerateGameProject(dir, dte, sln);
            ////sln.AddFromFile(projPath, false);
            //sln.SaveAs(Path.Combine(slnDir, Name + ".sln"));
            //VisualStudioManager.VSInstanceClosed();
        }
        public async Task CompileAsync()
            => await Task.Run(() => Compile());
        public async Task CompileAsync(string buildConfiguration, string buildPlatform)
            => await Task.Run(() => Compile(buildConfiguration, buildPlatform));
        public void Compile()
            => Compile(TargetBuildConfiguration, TargetBuildPlatform);
        public void Compile(string buildConfiguration, string buildPlatform)
        {
            ProjectCollection pc = new ProjectCollection();
            Dictionary<string, string> globalProperties = new Dictionary<string, string>
            {
                { "Configuration", buildConfiguration },
                { "Platform", buildPlatform },
            };
            BuildRequestData request = new BuildRequestData(SolutionPath, globalProperties, null, new string[] { "Build" }, null);
            BuildParameters bp = new BuildParameters(pc)
            {
                Loggers = new ILogger[]
                {
                    new EngineLogger()
                }
            };
            BuildResult result = BuildManager.DefaultBuildManager.Build(bp, request);
            if (result.OverallResult == BuildResultCode.Success)
            {
                PrintLine(SolutionPath + " : Build succeeded.");
            }
            else
            {
                PrintLine(SolutionPath + " : Build failed.");
                //foreach (var target in result.ResultsByTarget)
                //{
                //    if (target.Value.ResultCode == TargetResultCode.Failure)
                //    {
                //        Engine.PrintLine(target.Key + " : Build failed.");
                //        if (target.Value.Exception != null)
                //            Engine.PrintLine("Exception:\n" + target.Value.Exception.ToString());
                //    }
                //}
            }
            pc.UnregisterAllLoggers();
        }

        public void CollectFiles(
            out List<string> codeFiles,
            out List<string> contentFiles,
            out HashSet<string> references)
        {
            codeFiles = new List<string>();
            contentFiles = new List<string>();
            references = new HashSet<string>();
            void RecursiveCollect(
                string dir,
                ref List<string> codeFiles2,
                ref List<string> contentFiles2,
                ref HashSet<string> references2)
            {
                if (!Directory.Exists(dir))
                    return;

                string[] files = Directory.GetFiles(dir);
                foreach (string path in files)
                {
                    string path2 = LocalSourceDirectory + path.MakeAbsolutePathRelativeTo(SourceDirectory).Substring(1);
                    if (Path.GetExtension(path2).Substring(1).ToLowerInvariant() == "cs")
                    {
                        codeFiles2.Add(path2);
                        string text = File.ReadAllText(path);
                        int end = text.FindFirst(0, "namespace");
                        if (end > 0)
                            text = text.Substring(0, end);
                        string usingStr = "using ";
                        int[] usingIndices = text.FindAllOccurrences(0, usingStr);
                        foreach (int i in usingIndices)
                        {
                            int startIndex = i + usingStr.Length;
                            int endIndex = text.FindFirst(startIndex, ';');
                            if (endIndex < 0)
                                continue;
                            string reference = text.Substring(startIndex, endIndex - startIndex).Trim();
                            if (!reference.StartsWith("TheraEngine"))
                                references2.Add(reference);
                        }
                    }
                    else
                        contentFiles2.Add(path2);
                }

                string[] dirs = Directory.GetDirectories(dir);
                foreach (string d in dirs)
                    RecursiveCollect(d, ref codeFiles2, ref contentFiles2, ref references2);
            }
            RecursiveCollect(SourceDirectory, ref codeFiles, ref contentFiles, ref references);
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
                //_source.MessageRaised += MessageHandler;
                _source.ProjectFinished += ProjectFinishedHandler;
                _source.ProjectStarted += ProjectStartedHandler;
                //_source.TargetFinished += TargetFinishedHandler;
                //_source.TargetStarted += TargetStartedHandler;
                //_source.TaskFinished += TaskFinishedHandler;
                //_source.TaskStarted += TaskStartedHandler;
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
                //_source.MessageRaised -= MessageHandler;
                _source.ProjectFinished -= ProjectFinishedHandler;
                _source.ProjectStarted -= ProjectStartedHandler;
                //_source.TargetFinished -= TargetFinishedHandler;
                //_source.TargetStarted -= TargetStartedHandler;
                //_source.TaskFinished -= TaskFinishedHandler;
                //_source.TaskStarted -= TaskStartedHandler;
                _source.WarningRaised -= WarningHandler;
            }

            public void BuildStartedHandler(object sender, BuildStartedEventArgs e)
            {

            }
            public void BuildFinishedHandler(object sender, BuildFinishedEventArgs e)
            {

            }
            public void CustomEventHandler(object sender, CustomBuildEventArgs e)
            {

            }
            public void ErrorHandler(object sender, BuildErrorEventArgs e)
            {
                PrintLine($"Error {e.Code} : {e.Message} [{e.File} line {e.LineNumber} column {e.ColumnNumber}]");
            }
            public void MessageHandler(object sender, BuildMessageEventArgs e)
            {
                PrintLine($"Message {e.Code} : {e.Message} [{e.File} line {e.LineNumber} column {e.ColumnNumber}]");
            }
            public void ProjectStartedHandler(object sender, ProjectStartedEventArgs e)
            {
                PrintLine($"Started project {e.ProjectFile}");
            }
            public void ProjectFinishedHandler(object sender, ProjectFinishedEventArgs e)
            {
                PrintLine($"Finished project {e.ProjectFile}");
            }
            public void TargetStartedHandler(object sender, TargetStartedEventArgs e)
            {
                PrintLine($"Started target {e.TargetFile}");
            }
            public void TargetFinishedHandler(object sender, TargetFinishedEventArgs e)
            {
                PrintLine($"Finished target {e.TargetFile}");
            }
            public void TaskStartedHandler(object sender, TaskStartedEventArgs e)
            {
                PrintLine($"Started task {e.TaskName}");
            }
            public void TaskFinishedHandler(object sender, TaskFinishedEventArgs e)
            {
                PrintLine($"Finished task {e.TaskName}");
            }
            public void WarningHandler(object sender, BuildWarningEventArgs e)
            {
                PrintLine($"Warning {e.Code} : {e.Message} [{e.File} line {e.LineNumber} column {e.ColumnNumber}]");
            }
        }
        //public void Compile(string configuration, string platform)
        //{
        //    string projectFileName = @"...\ConsoleApplication3\ConsoleApplication3.sln";

        //    ProjectCollection pc = new ProjectCollection();
        //    Dictionary<string, string> GlobalProperty = new Dictionary<string, string>
        //    {
        //        { "Configuration", configuration },
        //        { "Platform", platform }
        //    };

        //    BuildParameters buildParams = new BuildParameters(pc);
        //    BuildRequestData buildRequest = new BuildRequestData(projectFileName, GlobalProperty, null, new string[] { "Build" }, null);
        //    BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParams, buildRequest);

        //    //CSharpCodeProvider codeProvider = new CSharpCodeProvider();
        //    //CompilerParameters parameters = new CompilerParameters
        //    //{
        //    //    GenerateExecutable = true,
        //    //    OutputAssembly = Path.Combine(FilePath, "Intermediate", Name + ".exe"),
        //    //};
        //    //CompilerResults results = codeProvider.CompileAssemblyFromFile(parameters, "");
        //    //if (results.Errors.Count > 0)
        //    //{
        //    //    foreach (CompilerError CompErr in results.Errors)
        //    //    {
        //    //        Engine.PrintLine("Line number {0}, Error Number: {1}, '{2};",
        //    //            CompErr.Line, CompErr.ErrorNumber, CompErr.ErrorText);
        //    //    }
        //    //}
        //}
    }
}