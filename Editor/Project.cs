using Extensions;
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
    public class TProject : TFileObject
    {
        public delegate void DelCompileBegun(TProject project);

        public delegate void DelCompileResult(TProject project, bool success);

        public EngineBuildLogger LastBuildLog { get; private set; }
        public bool IsCompiling { get; private set; }

        public event DelCompileBegun CompileStarted;
        public event DelCompileResult CompileCompleted;

        [TSerialize]
        public string IntermediateBuildDirectory { get; private set; } = "obj";
        [TSerialize]
        public LocalFileRef<TGame> GameRef { get; private set; } = new LocalFileRef<TGame>();
        public TGame Game => GameRef.File;
        
        /// <summary>
        /// This is the global GUID for a C# project.
        /// </summary>
        public const string CSharpProjectGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

        [TSerialize(nameof(ProjectGuid))]
        protected Guid _projectGuid = Guid.NewGuid();
        protected string _rootNamespace;

        public Guid ProjectGuid => _projectGuid;

        [TSerialize(State = true)]
        [Category("Code")]
        public string TargetBuildConfiguration { get; set; } = "Debug";
        [TSerialize(State = true)]
        [Category("Code")]
        public string TargetBuildPlatform { get; set; } = "x64";

        public string RootNamespace
        {
            get => string.IsNullOrWhiteSpace(_rootNamespace) ? Name.ReplaceWhitespace("_") : _rootNamespace;
            set => _rootNamespace = value.ReplaceWhitespace("_");
        }
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
        public string SolutionPath => DirectoryPath is null ? null :
            Path.Combine(DirectoryPath, Name + ".sln");

        private string _localBinariesDirectory;
        private string _localSourceDirectory;
        private string _localConfigDirectory;
        private string _localContentDirectory;
        private string _localTempDirectory;
        private string _localLibrariesDirectory;

        public Intellisense Intellisense { get; } = new Intellisense();

        [TSerialize]
        public PathReference[] AssemblyPaths { get; private set; }

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
        [TString(false, true, false, true)]
        [TSerialize]
        [Category("Paths")]
        public string LocalLibrariesDirectory
        {
            get => _localLibrariesDirectory;
            set
            {
                _localLibrariesDirectory = value;
                LibrariesDirectory = string.IsNullOrWhiteSpace(LocalLibrariesDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalLibrariesDirectory));
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

        private EventList<PathReference> _referencedAssemblies = new EventList<PathReference>();

        [TSerialize]
        public EventList<PathReference> ReferencedAssemblies
        {
            get => _referencedAssemblies;
            private set
            {
                if (_referencedAssemblies != null)
                    _referencedAssemblies.CollectionChanged -= _referencedAssemblies_CollectionChanged;
                _referencedAssemblies = value ?? new EventList<PathReference>();
                _referencedAssemblies.CollectionChanged += _referencedAssemblies_CollectionChanged;
            }
        }
        private void _referencedAssemblies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

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
                LibrariesDirectory = null;
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
                LibrariesDirectory = string.IsNullOrWhiteSpace(LocalLibrariesDirectory) ? null :
                    Path.GetFullPath(Path.Combine(DirectoryPath, LocalLibrariesDirectory));
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
        [Browsable(false)]
        public string LibrariesDirectory { get; private set; }
        
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
            MakePaths(directory);
            FilePath = GetFilePath<TProject>(directory, Name, EProprietaryFileFormat.XML);

            void Update<T>(ref GlobalFileRef<T> gref, string defaultName) where T : TFileObject, new()
            {
                if (gref is null)
                {
                    gref = new GlobalFileRef<T>(
                        directory, defaultName, EProprietaryFileFormat.XML, new T());
                }
                else
                {
                    gref.Path.Path = GetFilePath<T>(
                        directory, gref.File?.Name ?? defaultName, EProprietaryFileFormat.XML);
                }
            }

            Update(ref _projectStateRef, nameof(ProjectState));
            Update(ref _editorSettingsRef, nameof(EditorSettings));

            if (Game.UserSettingsRef is null)
            {
                Game.UserSettingsRef = new GlobalFileRef<UserSettings>(
                    directory, nameof(UserSettings), EProprietaryFileFormat.XML, new UserSettings());
            }
            else
            {
                Game.UserSettingsRef.Path.Path = GetFilePath<UserSettings>(
                    directory, Game.UserSettingsRef.File?.Name ?? nameof(UserSettings), EProprietaryFileFormat.XML);
            }

            if (Game.EngineSettingsOverrideRef is null)
            {
                Game.EngineSettingsOverrideRef = new GlobalFileRef<EngineSettings>(
                    directory, nameof(EngineSettings), EProprietaryFileFormat.XML, new EngineSettings());
            }
            else
            {
                Game.EngineSettingsOverrideRef.Path.Path = GetFilePath<EngineSettings>(
                    directory, Game.EngineSettingsOverrideRef.File?.Name ?? nameof(EngineSettings), EProprietaryFileFormat.XML);
            }
        }

        private void MakePaths(string directory)
        {
            if (!directory.EndsWithDirectorySeparator())
                directory += Path.DirectorySeparatorChar;

            string bin = "Bin" + Path.DirectorySeparatorChar;
            string cfg = "Config" + Path.DirectorySeparatorChar;
            string src = "Source" + Path.DirectorySeparatorChar;
            string ctt = "Content" + Path.DirectorySeparatorChar;
            string tmp = "Temp" + Path.DirectorySeparatorChar;
            string lib = "Lib" + Path.DirectorySeparatorChar;

            string binDir = directory + bin;
            string cfgDir = directory + cfg;
            string srcDir = directory + src;
            string cttDir = directory + ctt;
            string tmpDir = directory + tmp;
            string libDir = directory + lib;

            Directory.CreateDirectory(binDir);
            Directory.CreateDirectory(cfgDir);
            Directory.CreateDirectory(srcDir);
            Directory.CreateDirectory(cttDir);
            Directory.CreateDirectory(tmpDir);
            Directory.CreateDirectory(libDir);

            LocalBinariesDirectory = bin;
            LocalConfigDirectory = cfg;
            LocalSourceDirectory = src;
            LocalContentDirectory = ctt;
            LocalTempDirectory = tmp;
            LocalLibrariesDirectory = lib;
        }
        public static async Task<TProject> CreateAsync(string directory, string name)
            => await CreateAsync(directory, name, new UserSettings(), new EngineSettings(), new EditorSettings());
        public static async Task<TProject> CreateAsync(
            string directory,
            string name,
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
            string lib = "Lib" + Path.DirectorySeparatorChar;

            string binDir = directory + bin;
            string cfgDir = directory + cfg;
            string srcDir = directory + src;
            string cttDir = directory + ctt;
            string tmpDir = directory + tmp;
            string libDir = directory + lib;

            Directory.CreateDirectory(binDir);
            Directory.CreateDirectory(cfgDir);
            Directory.CreateDirectory(srcDir);
            Directory.CreateDirectory(cttDir);
            Directory.CreateDirectory(tmpDir);
            Directory.CreateDirectory(libDir);

            ProjectState state = new ProjectState();

            TProject project = new TProject()
            {
                Name = name,

                LocalBinariesDirectory = bin,
                LocalConfigDirectory = cfg,
                LocalSourceDirectory = src,
                LocalContentDirectory = ctt,
                LocalTempDirectory = tmp,
                LocalLibrariesDirectory = lib,

                GameRef = new LocalFileRef<TGame>(cfgDir, name, EProprietaryFileFormat.XML, new TGame()
                {
                    UserSettingsRef
                        = new GlobalFileRef<UserSettings>(cfgDir, nameof(UserSettings),
                        EProprietaryFileFormat.XML, userSettings),

                    EngineSettingsOverrideRef
                        = new GlobalFileRef<EngineSettings>(cfgDir, nameof(EngineSettings),
                        EProprietaryFileFormat.XML, engineSettings),
                }),

                FilePath
                    = GetFilePath<TProject>(directory, name,
                    EProprietaryFileFormat.XML),

                ProjectStateRef
                    = new GlobalFileRef<ProjectState>(directory, nameof(ProjectState),
                    EProprietaryFileFormat.XML, state),

                EditorSettingsOverrideRef
                    = new GlobalFileRef<EditorSettings>(cfgDir, nameof(EditorSettings),
                    EProprietaryFileFormat.XML, editorSettings),
            };

            await Task.WhenAll(
                state.ExportAsync(),
                userSettings.ExportAsync(),
                engineSettings.ExportAsync(),
                editorSettings.ExportAsync(),
                project.ExportAsync());

            await project.GenerateSolutionAsync();
            await project.CompileAsync();

            return project;
        }

        //TODO: only have user program in library,
        //generate EXE on full project build to run game as standalone.
        public void GenerateProgramDotCs()
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
                nameof(LoadAsync),
                nameof(Engine),
                nameof(Engine.Run),
                ext,
                RootNamespace);

            File.WriteAllText(SourceDirectory + "Program.cs", progCs.Replace('@', '{').Replace('#', '}'));
        }
        public string ResolveCSProjPath()
        {
            return Path.Combine(DirectoryPath, Name + ".csproj");
        }
        public async Task GetReferencedAssemblies()
        {
            string csprojPath = ResolveCSProjPath();
            if (!File.Exists(csprojPath))
                return;

            PrintLine("Retrieving referenced assemblies.");
            if (!LibrariesDirectory.IsValidExistingPath())
            {
                if (!string.IsNullOrWhiteSpace(DirectoryPath))
                {
                    if (string.IsNullOrWhiteSpace(LocalLibrariesDirectory))
                        LocalLibrariesDirectory = "Lib" + Path.DirectorySeparatorChar;

                    LibrariesDirectory = Path.GetFullPath(Path.Combine(DirectoryPath, LocalLibrariesDirectory));
                    Directory.CreateDirectory(LibrariesDirectory);
                }
                else
                {
                    Engine.LogWarning("Project does not exist in a directory.");
                    return;
                }
            }

            MSBuild.Project project = await ReadCSProj();

            PrintLine("Iterating through referenced assemblies.");
            ItemGroup[] itemGroups = project.GetChildren<ItemGroup>();
            foreach (ItemGroup itemGroup in itemGroups)
            {
                Item[] items = itemGroup.GetChildren<Item>();
                foreach (Item item in items)
                {
                    if (!item.ElementName.EqualsInvariantIgnoreCase("Reference"))
                        continue;
                    
                    string assemblyNameStr = item.Include;
                    var name = new AssemblyName(assemblyNameStr);
                    var hintPath = item.GetChildren<ItemMetadata>().FirstOrDefault(x => x.ElementName.EqualsInvariantIgnoreCase("HintPath"));
                    string path = name?.CodeBase ?? hintPath?.StringContent?.Value ?? null;
                    if (string.IsNullOrWhiteSpace(path))
                        continue;
                    
                    if (!path.IsAbsolutePath())
                        path = Path.GetFullPath(Path.Combine(DirectoryPath, path));

                    string fileName = Path.GetFileName(path);

                    //Remove the original path
                    if (ReferencedAssemblies.Contains(path))
                        ReferencedAssemblies.Remove(path);

                    if (!File.Exists(path))
                        continue;

                    //Remove any paths with the same file name
                    var matchingNames = ReferencedAssemblies.Where(x =>
                        Path.GetFileName(x.Path).EqualsInvariantIgnoreCase(fileName)).ToArray();
                    foreach (var match in matchingNames)
                        ReferencedAssemblies.Remove(match);

                    //Generate path in lib folder
                    string libPath = Path.Combine(LibrariesDirectory, fileName);

                    File.Copy(path, libPath, true);

                    var libPathRef = ReferencedAssemblies.FirstOrDefault(x => x.Path.EqualsInvariantIgnoreCase(libPath));
                    if (libPathRef is null)
                        ReferencedAssemblies.Add(new PathReference(libPath, EPathType.FileRelative));
                }
            }
            PrintLine("Done iterating through referenced assemblies.");

            string enginePath = typeof(Engine).Assembly.Location;
            string engineLibPath = Path.Combine(LibrariesDirectory, Path.GetFileName(enginePath));
            if (!ReferencedAssemblies.Contains(engineLibPath))
                ReferencedAssemblies.Add(new PathReference(engineLibPath, EPathType.FileRelative));
            File.Copy(enginePath, engineLibPath, true);
        }
        public async void GenerateSolution() => await GenerateSolutionAsync();
        public async Task GenerateSolutionAsync()
        {
            if (!string.IsNullOrWhiteSpace(_name))
                Name = Path.GetFileNameWithoutExtension(FilePath);

            await GetReferencedAssemblies();

            Process[] devenv = Process.GetProcessesByName("DevEnv");
            FileVersionInfo info = null;
            if (devenv.Length > 0)
            {
                Process devp = devenv[0];
                if (devp.Modules.Count > 0)
                    info = devp.Modules[0].FileVersionInfo;
            }

            string ver = info?.ProductVersion ?? "15.7";
            int majorVer = info?.FileMajorPart ?? 15;
            int minorVer = info?.FileMinorPart ?? 7;

            string solutionGuid = Guid.ToString("B").ToUpperInvariant();
            string projectGuid = _projectGuid.ToString("B").ToUpperInvariant();

            //if (forceRegenerateProgramDotCs || !File.Exists(SourceDirectory + "Program.cs"))
            //    GenerateProgramDotCs();

            #region csproj

            PrintLine("Generating new csproj.");
            string csprojPath = Path.Combine(DirectoryPath, Name + ".csproj");

            int op;

            MSBuild.Project exportProj = new MSBuild.Project
            {
                ToolsVersion = majorVer + "." + minorVer,
                DefaultTargets = "Build",
                Schema = "http://schemas.microsoft.com/developer/msbuild/2003"
            };

            Import import = new Import(
                "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props",
                "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')");

            const string targetNetVersion = "v4.7.2";
            const string defaultConfig = "Debug";
            const string defaultPlatform = "x64";
            const string appPropertiesFolder = "Properties"; //Relative to the csproj

            PropertyGroup mainInfo = PropertyGroup.Create(null,
                ("Configuration", defaultConfig, " '$(Configuration)' == '' "),
                ("Platform", defaultPlatform, " '$(Configuration)' == '' "),
                ("ProjectGuid", Guid.ToString("B").ToUpperInvariant(), null),
                ("OutputType", "Library", null), //("OutputType", "WinExe", null),
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

            PropertyGroup debugx86 = CreateConfigPropGrp(true, true, true);
            PropertyGroup debugx64 = CreateConfigPropGrp(true, false, true);
            PropertyGroup releasex86 = CreateConfigPropGrp(false, true, true);
            PropertyGroup releasex64 = CreateConfigPropGrp(false, false, true);

            CollectFiles(
                out List<string> codeFiles,
                out List<string> contentFiles,
                out HashSet<string> usingNamespaces);

            ItemGroup refGrp = new ItemGroup();
            
            foreach (PathReference path in ReferencedAssemblies)
            {
                var name = AssemblyName.GetAssemblyName(path.Path);

                string relPath = path.Path.MakeAbsolutePathRelativeTo(DirectoryPath);

                if (relPath.StartsWith("\\"))
                    relPath = relPath.Substring(1);

                Item asmRef = new Item("Reference")
                {
                    Include = name.ToString(),
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
                        StringContent = new ElementString(relPath),
                    };
                    asmRef.AddElements(specificVersion, hintPath);
                }
                refGrp.AddElements(asmRef);
            }
            
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

            exportProj.AddElements(
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

            //await Editor.RunOperationAsync($"Writing csproj... {csprojPath}", "Done writing csproj.", async (p, c) =>
            await XMLSchemaDefinition<MSBuild.Project>.ExportAsync(csprojPath, exportProj);

            #endregion

            #region sln
            Engine.PrintLine(EOutputVerbosity.Verbose, $"Writing sln... {SolutionPath}");
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
            Engine.PrintLine(EOutputVerbosity.Verbose, $"Done writing sln.");
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

            await CompileAsync();
        }

        public void DeleteIntermediateDirectory()
        {
            try
            {
                string interDirPath = Path.Combine(DirectoryPath, IntermediateBuildDirectory);
                if (Directory.Exists(interDirPath))
                    Directory.Delete(interDirPath, true);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
        }

        public async Task<bool> CompileAsync(bool createGameDomain = true)
            => await CompileAsync(TargetBuildConfiguration, IntPtr.Size == 8 ? "x64" : "x86", createGameDomain);
        public async Task<bool> CompileAsync(string buildConfiguration, string buildPlatform, bool createGameDomain = true)
        {
            if (IsCompiling)
            {
                Engine.PrintLine("Project is already compiling.");
                return false;
            }

            try
            {
                IsCompiling = true;
                CompileStarted?.Invoke(this);
                Engine.PrintLine($"Compiling {buildConfiguration} {buildPlatform} {SolutionPath}");

                DeleteIntermediateDirectory();
                await UpdateEngineLibReferenceAsync();

                LastBuildLog = new EngineBuildLogger();

                Dictionary<string, string> props = new Dictionary<string, string>
                {
                    { "Configuration",  buildConfiguration  },
                    { "Platform",       buildPlatform       },
                };
                BuildRequestData request = new BuildRequestData(SolutionPath, props, null, new[] { "Build" }, null);
                ProjectCollection pc = new ProjectCollection();
                BuildParameters buildParams = new BuildParameters(pc)
                {
                    Loggers = new ILogger[] { LastBuildLog }
                };

                BuildResult result = await Task.Run(() => BuildManager.DefaultBuildManager.Build(buildParams, request));

                await OnBuildCompleted(result, createGameDomain);

                return result.OverallResult == BuildResultCode.Success;
            }
            catch (Exception ex)
            {
                IsCompiling = false;
                Engine.LogException(ex);
                DeleteIntermediateDirectory();
                CompileCompleted?.Invoke(this, false);
                return false;
            }
        }
        private async Task OnBuildCompleted(BuildResult result, bool createGameDomain)
        {
            bool success = result.OverallResult == BuildResultCode.Success;
            try
            {
                if (success)
                {
                    PrintLine(SolutionPath + " : Build succeeded.");

                    ITaskItem[] buildItems = result.ResultsByTarget["Build"].Items;
                    AssemblyPaths = buildItems.Select(x => new PathReference(x.ItemSpec, EPathType.FileRelative)).ToArray();

                    Editor.Instance.CopyEditorLibraries(AssemblyPaths);

                    if (Editor.Instance.DockableErrorListFormActive)
                    {
                        if (Editor.Instance.InvokeRequired)
                            Editor.Instance.BeginInvoke((Action)(() => Editor.Instance.ErrorListForm.SetLog(LastBuildLog)));
                        else
                            Editor.Instance.ErrorListForm.SetLog(LastBuildLog);
                    }

                    await ExportAsync();

                    if (createGameDomain)
                        await CreateGameDomainAsync(true);
                }
                else
                {
                    PrintLine(SolutionPath + " : Build failed.");
                    LastBuildLog.Display();
                }
            }
            finally
            {
                IsCompiling = false;
                DeleteIntermediateDirectory();
                CompileCompleted?.Invoke(this, success);
            }
        }
        public async Task<MSBuild.Project> ReadCSProj()
        {
            string csprojPath = ResolveCSProjPath();
            return 
                //await Editor.RunOperationAsync($"Reading csproj... {csprojPath}", "Done reading csproj.", async (p, c) =>
                await XMLSchemaDefinition<MSBuild.Project>.ImportAsync(csprojPath, 0)/*)*/;
        }
        public async Task WriteCSProj(MSBuild.Project project)
        {
            string csprojPath = ResolveCSProjPath();
            //await Editor.RunOperationAsync($"Writing csproj... {csprojPath}", "Done writing csproj.", async (p, c) => 
            await XMLSchemaDefinition<MSBuild.Project>.ExportAsync(csprojPath, project)/*)*/;
        }
        private async Task UpdateEngineLibReferenceAsync()
        {
            MSBuild.Project project = await ReadCSProj();

            PrintLine("Updating engine library reference in csproj.");
            ItemGroup[] itemGroups = project.GetChildren<ItemGroup>();
            bool updated = false;
            ItemGroup refItemGrp = null;
            foreach (ItemGroup itemGroup in itemGroups)
            {
                Item[] items = itemGroup.GetChildren<Item>();
                foreach (Item item in items)
                {
                    if (!item.ElementName.EqualsInvariantIgnoreCase("Reference"))
                        continue;

                    refItemGrp = itemGroup;

                    if (!item.Include.StartsWith("TheraEngine"))
                        continue;

                    if (updated)
                    {
                        itemGroup.ChildElements[typeof(Item)].Remove(item);
                        continue;
                    }

                    updated = true;

                    item.ClearChildElements();
                    UpdateEngineRefPathItem(item);
                }
            }
            if (!updated)
            {
                Item item = new Item("Reference");
                UpdateEngineRefPathItem(item);
                refItemGrp.AddElements(item);
            }
            PrintLine("Done updating engine library reference in csproj.");

            await WriteCSProj(project);
        }
        private void UpdateEngineRefPathItem(Item item)
        {
            //TODO: 
            //1. Get path to debug game engine build. 
            //2. Compile it if necessary.
            //3. Copy TheraEngine.dll to ProjectDir/Lib
            var asm = typeof(Engine).Assembly;
            item.Include = asm.FullName;
            string absPath = asm.CodeBase;
            string relPath = absPath.MakeAbsolutePathRelativeTo(DirectoryPath);
            item.AddElements(
                new ItemMetadata(0, "SpecificVersion", "True"),
                new ItemMetadata(1, "HintPath", relPath));
        }
        
        //[TPostDeserialize(arguments: false)]
        internal async Task CreateGameDomainAsync(bool callingAfterCompile)
        {
            if (GameRef is null || !GameRef.FileExists)
            {
                TGame game = new TGame();
                GameRef.File = game;

                string path = Path.Combine(DirectoryPath, Name + "." + ExtensionFor<TGame>(EProprietaryFileFormat.XML));
                GameRef.Path.Path = path;

                await game.ExportAsync(path);
            }

            string buildPlatform = IntPtr.Size == 8 ? "x64" : "x86";
            string buildConfiguration = "Debug";

            string rootDir = BinariesDirectory + $"{buildPlatform}\\{buildConfiguration}";
            if (!callingAfterCompile && (!Directory.Exists(rootDir) || AssemblyPaths is null || AssemblyPaths.Any(x => !File.Exists(x.Path))))
            {
                bool success = await Editor.RunOperationAsync(
                    "Compiling project...",
                    "Finished compiling project.",
                    async (p, c) => await CompileAsync(false));

                if (!success)
                    return;
            }

            Editor.Instance.CreateGameDomain(Game?.FilePath, rootDir, AssemblyPaths);
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
                ref List<string> codeFilesRef,
                ref List<string> contentFilesRef,
                ref HashSet<string> referencesRef)
            {
                if (!Directory.Exists(dir))
                    return;

                string[] files = Directory.GetFiles(dir);
                foreach (string path in files)
                {
                    string relPath = path.MakeAbsolutePathRelativeTo(SourceDirectory);
                    string localPath = LocalSourceDirectory + relPath;
                    string ext = localPath.GetExtensionLowercase();
                    switch (ext)
                    {
                        default:
                            {
                                contentFilesRef.Add(localPath);
                            }
                            break;
                        case "cs":
                            {
                                codeFilesRef.Add(localPath);

                                string text = File.ReadAllText(path);
                                int usingsEnd = text.FindFirst(0, "namespace");
                                if (usingsEnd > 0)
                                    text = text.Substring(0, usingsEnd);

                                string usingStr = "using ";
                                int[] usingIndices = text.FindAllOccurrences(0, usingStr);

                                foreach (int i in usingIndices)
                                {
                                    int startIndex = i + usingStr.Length;
                                    int endIndex = text.FindFirst(startIndex, ';');

                                    if (endIndex < 0)
                                        continue;

                                    string reference = text.Substring(startIndex, endIndex - startIndex).Trim();

                                    referencesRef.Add(reference);
                                }
                            }
                            break;
                    }
                }

                string[] dirs = Directory.GetDirectories(dir);
                foreach (string d in dirs)
                    RecursiveCollect(d, ref codeFilesRef, ref contentFilesRef, ref referencesRef);
            }
            RecursiveCollect(SourceDirectory, ref codeFiles, ref contentFiles, ref references);
        }

        public class EngineBuildLogger : TObjectSlim, ILogger
        {
            public List<BuildErrorEventArgs> Errors { get; private set; }
            public List<BuildWarningEventArgs> Warnings { get; private set; }
            public List<BuildMessageEventArgs> Messages { get; private set; }

            public EngineBuildLogger() { }
            public EngineBuildLogger(LoggerVerbosity verbosity) => Verbosity = verbosity;

            public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Minimal;
            public string Parameters { get; set; }

            private IEventSource _source = null;

            public void Initialize(IEventSource eventSource)
            {
                if ((_source = eventSource) is null)
                    return;

                _source.BuildFinished += BuildFinishedHandler;
                _source.BuildStarted += BuildStartedHandler;
                _source.CustomEventRaised += CustomEventHandler;
                _source.ErrorRaised += ErrorHandler;
                _source.MessageRaised += MessageHandler;
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
                if (_source is null)
                    return;

                _source.BuildFinished -= BuildFinishedHandler;
                _source.BuildStarted -= BuildStartedHandler;
                _source.CustomEventRaised -= CustomEventHandler;
                _source.ErrorRaised -= ErrorHandler;
                _source.MessageRaised -= MessageHandler;
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
                Errors = new List<BuildErrorEventArgs>();
                Warnings = new List<BuildWarningEventArgs>();
                Messages = new List<BuildMessageEventArgs>();
            }
            public void BuildFinishedHandler(object sender, BuildFinishedEventArgs e)
            {
                //foreach (var error in Errors)
                //    PrintLine($"Error {error.Code} : {error.Message} [{error.File} line {error.LineNumber} column {error.ColumnNumber}]");
                //foreach (var warning in Warnings)
                //    PrintLine($"Warning {warning.Code} : {e.Message} [{warning.File} line {warning.LineNumber} column {warning.ColumnNumber}]");
                //foreach (var message in Messages)
                //    PrintLine($"Message {message.Code} : {message.Message} [{message.File} line {message.LineNumber} column {message.ColumnNumber}]");
            }
            public void ErrorHandler(object sender, BuildErrorEventArgs e) => Errors.Add(e);
            public void WarningHandler(object sender, BuildWarningEventArgs e) => Warnings.Add(e);
            public void MessageHandler(object sender, BuildMessageEventArgs e) => Messages.Add(e);

            public void CustomEventHandler(object sender, CustomBuildEventArgs e)
            {

            }
            public void ProjectStartedHandler(object sender, ProjectStartedEventArgs e)
            {
                PrintLine($"Started building project {e.ProjectFile}");
            }
            public void ProjectFinishedHandler(object sender, ProjectFinishedEventArgs e)
            {
                PrintLine($"Finished building project {e.ProjectFile}");
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
            public void Display()
            {
                Editor.Instance.ShowErrorForm(this);
            }
        }
    }
}