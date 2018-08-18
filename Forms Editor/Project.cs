using Microsoft.Build.Framework;
using Microsoft.VisualStudio.Workspace.Extensions.MSBuild;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TheraEngine;
using TheraEngine.Core.Files.XML;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Files;
using TheraEngine.ThirdParty;
using static TheraEngine.ThirdParty.MSBuild;
using static TheraEngine.ThirdParty.MSBuild.Project;
using static TheraEngine.ThirdParty.MSBuild.PropertyGroup;

namespace TheraEditor
{
    /// <summary>
    /// Extension of the game class for use with the editor.
    /// </summary>
    [FileExt("tproj", PreferredFormat = SerializeFormat.XML)]
    [FileDef("Thera Engine Project")]
    public class Project : Game
    {
        //z[TSerialize(nameof(Guid))]
        protected Guid _guid;
        //[TSerialize(nameof(ProjectGuid))]
        protected Guid _projectGuid;

        public Guid Guid => _guid;
        public Guid ProjectGuid => _projectGuid;

        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Project")]
        public string LocalBinariesDirectory { get; set; }
        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Project")]
        public string LocalSourceDirectory { get; set; }
        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Project")]
        public string LocalConfigDirectory { get; set; }
        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Project")]
        public string LocalContentDirectory { get; set; }
        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Project")]
        public string LocalTempDirectory { get; set; }

        [Category("Project")]
        public string BinariesDirectory => Path.GetFullPath(Path.Combine(DirectoryPath, LocalBinariesDirectory));
        [Category("Project")]
        public string SourceDirectory => Path.GetFullPath(Path.Combine(DirectoryPath, LocalBinariesDirectory));
        [Category("Project")]
        public string ConfigDirectory => Path.GetFullPath(Path.Combine(DirectoryPath, LocalBinariesDirectory));
        [Category("Project")]
        public string ContentDirectory => Path.GetFullPath(Path.Combine(DirectoryPath, LocalBinariesDirectory));
        [Category("Project")]
        public string TempDirectory => Path.GetFullPath(Path.Combine(DirectoryPath, LocalBinariesDirectory));

        [Browsable(false)]
        public ProjectState ProjectState
        {
            get => ProjectStateRef.File;
            set => ProjectStateRef.File = value;
        }
        [TSerialize]
        [Browsable(false)]
        public GlobalFileRef<ProjectState> ProjectStateRef { get; set; }
        [Browsable(false)]
        public EditorSettings EditorSettings
        {
            get => EditorSettingsRef.File;
            set => EditorSettingsRef.File = value;
        }
        [TSerialize]
        //[Browsable(false)]
        public GlobalFileRef<EditorSettings> EditorSettingsRef { get; set; }

        public void SetDirectory(string directory)
        {
            if (!directory.EndsWithDirectorySeparator())
                directory += Path.DirectorySeparatorChar;

            FilePath = GetFilePath(directory, Name, EProprietaryFileFormat.XML, typeof(Project));
            ProjectStateRef.ReferencePathAbsolute = GetFilePath<ProjectState>(ConfigDirectory, Name, EProprietaryFileFormat.XML);
            UserSettingsRef.ReferencePathAbsolute = GetFilePath<UserSettings>(ConfigDirectory, Name, EProprietaryFileFormat.XML);
            EngineSettingsRef.ReferencePathAbsolute = GetFilePath<EngineSettings>(ConfigDirectory, Name, EProprietaryFileFormat.XML);
            EditorSettingsRef.ReferencePathAbsolute = GetFilePath<EditorSettings>(ConfigDirectory, Name, EProprietaryFileFormat.XML);
        }
        public static Project Create(string directory, string name)
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
            UserSettings userSettings = new UserSettings();
            EngineSettings engineSettings = new EngineSettings();
            EditorSettings editorSettings = new EditorSettings();

            Project p = new Project()
            {
                _guid = Guid.NewGuid(),
                _projectGuid = Guid.NewGuid(),
                Name = name,
                FilePath = GetFilePath<Project>(directory, name, EProprietaryFileFormat.XML),
                ProjectStateRef = new GlobalFileRef<ProjectState>(directory, "ProjectState", EProprietaryFileFormat.XML, state, true),
                UserSettingsRef = new GlobalFileRef<UserSettings>(cfgDir, "UserSettings", EProprietaryFileFormat.XML, userSettings, true),
                EngineSettingsRef = new GlobalFileRef<EngineSettings>(cfgDir, "EngineSettings", EProprietaryFileFormat.XML, engineSettings, true),
                EditorSettingsRef = new GlobalFileRef<EditorSettings>(cfgDir, "EditorSettings", EProprietaryFileFormat.XML, editorSettings, true),
                LocalBinariesDirectory = bin,
                LocalConfigDirectory = cfg,
                LocalSourceDirectory = src,
                LocalContentDirectory = ctt,
                LocalTempDirectory = tmp,
            };

            p.Export();

            return p;
        }

        private PropertyGroup CreatePropertyGroup(string condition, params (string elementName, string content, string condition)[] properties)
        {
            var grp = new PropertyGroup();
            grp.AddElements(properties.Select(x => (Property)x).ToArray());
            return grp;
        }
        private PropertyGroup CreateConfigPropGrp(bool debug, bool x86, string netVer, bool allowUnsafeCode)
        {
            string config = debug ? "Debug" : "Release";
            string platform = x86 ? "x86" : "x64";
            return CreatePropertyGroup($"'$(Configuration)|$(Platform)' == '{config}|{platform}'",
                ("DebugSymbols", "true", null),
                ("OutputPath", Path.Combine(LocalBinariesDirectory, platform, config), null),
                ("DefineConstants", debug ? "DEBUG;TRACE" : "TRACE", null),
                ("Optimize", "false", null),
                ("AllowUnsafeBlocks", allowUnsafeCode ? "true" : "false", null),
                ("DebugType", debug ? "full" : "pdbonly", null),
                ("PlatformTarget", $"{platform}", null),
                ("ErrorReport", "prompt", null),
                ("CodeAnalysisRuleSet", "MinimumRecommendedRules.ruleset", null),
                ("Prefer32Bit", x86 ? "true" : "false", null),
                ("LangVersion", "latest", null));
        }

        public void CollectCodeFiles(
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
                string[] f = Directory.GetFiles(dir);
                foreach (string path in f)
                {
                    if (Path.GetExtension(path).Substring(1).ToLowerInvariant() == "cs")
                    {
                        codeFiles2.Add(path);
                        string text = File.ReadAllText(path);
                        string usingStr = "using ";
                        int[] usingIndices = text.FindAllOccurrences(0, usingStr);
                        foreach (int i in usingIndices)
                        {
                            int startIndex = i + usingStr.Length;
                            int endIndex = text.FindFirst(startIndex, ';');
                            string reference = text.Substring(startIndex, endIndex - startIndex).Trim();
                            references2.Add(reference);
                        }
                    }
                    else
                    {
                        contentFiles2.Add(path);
                    }
                }

                string[] dirs = Directory.GetDirectories(dir);
                foreach (string d in dirs)
                    RecursiveCollect(d, ref codeFiles2, ref contentFiles2, ref references2);
            }
            RecursiveCollect(SourceDirectory, ref codeFiles, ref contentFiles, ref references);
        }

        public void GenerateSolution() =>
            GenerateSolution(Path.Combine(SourceDirectory, "Solution"));
        public async void GenerateSolution(string slnDir)
        {
            MSBuild.Project p = new MSBuild.Project();
            var import = new Import(
                "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props",
                "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')");

            const string netVer = "v4.7.2";
            var mainInfo = CreatePropertyGroup(null,
                ("Configuration", "Debug", " '$(Configuration)' == '' "),
                ("Platform", "x64", " '$(Configuration)' == '' "),
                ("ProjectGuid", Guid.ToString("B").ToUpperInvariant(), null),
                ("OutputType", "WinExe", null),
                ("AppDesignerFolder", "Properties", null),
                ("RootNamespace", Name, null),
                ("AssemblyName", Name, null),
                ("TargetFrameworkVersion", netVer, null),
                ("FileAlignment", "512", null),
                ("AutoGenerateBindingRedirects", "true", null),
                ("TargetFrameworkProfile", null, null));

            var debugx86 = CreateConfigPropGrp(true, true, netVer, true);
            var debugx64 = CreateConfigPropGrp(true, false, netVer, true);
            var releasex86 = CreateConfigPropGrp(false, true, netVer, true);
            var releasex64 = CreateConfigPropGrp(false, false, netVer, true);

            CollectCodeFiles(out List<string> codeFiles, out List<string> contentFiles, out HashSet<string> references);

            ItemGroup refGrp = new ItemGroup();
            refGrp.AddElements(references.Select(x => new Item("Reference") { Include = x }).ToArray());

            ItemGroup compileGrp = new ItemGroup();
            compileGrp.AddElements(codeFiles.Select(x => new Item("Compile") { Include = x }).ToArray());

            ItemGroup contentGrp = new ItemGroup();
            compileGrp.AddElements(contentFiles.Select(x => new Item("Content") { Include = x }).ToArray());

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
                compileGrp,
                csharpImport,
                beforeBuild,
                afterBuild);

            var def = new XMLSchemeDefinition<MSBuild.Project>();
            await def.ExportAsync(Path.Combine(slnDir, Name + ".csproj"), p);

            var info = Process.GetProcessesByName("DevEnv")[0].Modules[0].FileVersionInfo;
            string ver = info.ProductVersion;
            int majorVer = info.FileMajorPart;
            //0 = projects, 1 = pre solution list, 2 = post solution list, 3 = solution GUID
            string slnTmpl = File.ReadAllText(Path.Combine(Engine.Settings.EngineDataFolder, "SolutionTemplate.sln"));
            //0 = project name, 1 = project GUID
            //The first GUID is the GUID of a C# project package
            string projTmpl = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"{0}\", \"{0}.csproj\", \"{1}\"\nEndProject";
            //0 = config name, 1 = platform name
            string preSolTmpl = "{0}|{1} = {0}|{1}";
            //0 = project GUID, 1 = config name, 2 = platform name
            string postSolTmpl = "{0}.{1}|{2}.ActiveCfg = {1}|{2}";

            //{3BA74071-A8FB-473A-9388-0C8F75872F41}.Debug|Any CPU.ActiveCfg = ReleaseGameObfuscated|x86
            //{3BA74071-A8FB-473A-9388-0C8F75872F41}.Debug|Any CPU.Build.0 = ReleaseGameObfuscated|x86

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

        //private string GenerateGameProject(string slnDir, EnvDTE80.DTE2 dte, Solution4 sln)
        //{
        //    Dictionary<string, string> props = new Dictionary<string, string>
        //    {
        //        { "Configuration", "Debug" },
        //        { "Platform", "x86" }
        //    };
        //    EngineLogger logger = new EngineLogger(LoggerVerbosity.Diagnostic);
        //    ProjectCollection pc = new ProjectCollection(props, new ILogger[] { logger },
        //        ToolsetDefinitionLocations.ConfigurationFile | ToolsetDefinitionLocations.Registry);
        //    BuildParameters buildParams = new BuildParameters(pc);
        //    BuildRequestData buildRequest = new BuildRequestData(Name, props, null, new string[] { "Build" }, null);
        //    BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParams, buildRequest);

        //    //File.WriteAllText(projPath, @"");
        //    return projPath;
        //}

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