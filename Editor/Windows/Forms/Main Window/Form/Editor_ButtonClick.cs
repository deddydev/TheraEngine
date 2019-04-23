using AppDomainToolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using TheraEngine;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor
    {
        private void btnLanguage_Click(object sender, EventArgs e)
        {

        }
        private void btnCancelOp_ButtonClick(object sender, EventArgs e)
        {
            for (int i = 0; i < _operations.Count; ++i)
                _operations[i]?.Cancel();
            
            EndOperation(-1);
            toolStripStatusLabel1.Text = _operations.Count == 1 ?
                "Operation was canceled." :
                _operations.Count + " operations were canceled.";
        }
        private void BtnViewAnalytics_Click(object sender, EventArgs e)
            => GPUAnalyticsForm.Focus();
        private void BtnNewProject_Click(object sender, EventArgs e) 
            => CreateNewProject();
        private void BtnOpenProject_Click(object sender, EventArgs e) 
            => OpenProject();
        private void BtnSaveProject_Click(object sender, EventArgs e) 
            => SaveFile(_project);
        private void BtnSaveProjectAs_Click(object sender, EventArgs e) 
            => SaveFileAs(_project);
        private void BtnNewWorld_Click(object sender, EventArgs e)
            => CreateNewWorld();
        private void BtnOpenWorld_Click(object sender, EventArgs e)
            => OpenWorld();
        private void BtnSaveWorld_Click(object sender, EventArgs e)
            => SaveFile(CurrentWorld);
        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
            => SaveFileAs(CurrentWorld);
        private void extensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void cubeMapEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void textureGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void btnCloseProject_Click(object sender, EventArgs e)
            => Project = null;
        private void btnCloseWorld_Click(object sender, EventArgs e)
            => CloseWorld();
        private void btnUndo_Click(object sender, EventArgs e)
            => UndoManager.Undo();
        private void btnRedo_Click(object sender, EventArgs e)
            => UndoManager.Redo();
        private void btnViewTools_Click(object sender, EventArgs e)
        {

        }
        private async void btnCompile_Click(object sender, EventArgs e)
            => await Project.CompileAsync();
        private void visualStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnvDTE80.DTE2 dte = VisualStudioManager.CreateVSInstance();
            Engine.PrintLine($"Launched Visual Studio {dte.Edition} {dte.Version}.");
            dte.MainWindow.Visible = true;
            dte.UserControl = true;
            VisualStudioManager.VSInstanceClosed();
        }
        private void btnContact_Click(object sender, EventArgs e) 
            => Process.Start(ContactURL);
        private void btnDocumentation_Click(object sender, EventArgs e) 
            => Process.Start(DocumentationURL);
        private void btnCheckForUpdates_Click(object sender, EventArgs e) 
            => CheckUpdates();
        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }
        private void BtnViewActorTree_Click(object sender, EventArgs e) 
            => ActorTreeForm.Focus();
        private void Viewport1ToolStripMenuItem_Click(object sender, EventArgs e) 
            => RenderForm1.Focus();
        private void viewport2ToolStripMenuItem_Click(object sender, EventArgs e) 
            => RenderForm2.Focus();
        private void viewport3ToolStripMenuItem_Click(object sender, EventArgs e) 
            => RenderForm3.Focus();
        private void viewport4ToolStripMenuItem_Click(object sender, EventArgs e) 
            => RenderForm4.Focus();
        private void btnViewFileTree_Click(object sender, EventArgs e) 
            => FileTreeForm.Focus();
        private void btnViewPropertyGrid_Click(object sender, EventArgs e) 
            => PropertyGridForm.Focus();
        private void btnViewOutput_Click(object sender, EventArgs e) 
            => OutputForm.Focus();
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void BtPlay_Click(object sender, EventArgs e)
            => GameState = GameState != EEditorGameplayState.Editing ? EEditorGameplayState.Editing : EEditorGameplayState.Attached;
        private void btnPlayDetached_Click(object sender, EventArgs e)
            => GameState = GameState != EEditorGameplayState.Attached ? EEditorGameplayState.Attached : EEditorGameplayState.Detached;
        private void EndGameplay()
        {
            if (GameState == EEditorGameplayState.Detached)
                GameState = EEditorGameplayState.Editing;
            else
                GameState = EEditorGameplayState.Detached;
        }

        private void BtnProjectSettings_Click(object sender, EventArgs e)
            => PropertyGridForm.PropertyGrid.TargetObject = Project;
        private void BtnEngineSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetObject = Project?.EngineSettingsOverrideRef;
        private void BtnEditorSettings_Click(object sender, EventArgs e)
            => PropertyGridForm.PropertyGrid.TargetObject = DefaultSettingsRef;
        private void BtnUserSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetObject = Project?.UserSettingsRef;
        private void BtnWorldSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.SettingsRef;
        private void BtnNewMaterial_Click(object sender, EventArgs e) 
            => new MaterialEditorForm().Show();
        private async void btnUploadNewRelease_Click(object sender, EventArgs e)
        {
            await Github.ReleaseCreator.CreateNewRelease(Assembly.GetExecutingAssembly(), "test release");
        }

        private AppDomainContext<Editor.TheraAssemblyTargetLoader, TheraAssemblyResolver> _gameDomain;
        [Browsable(false)]
        public ProjectDomainProxy DomainProxy { get; private set; }

        private TypeProxy TypeCreationFailed(string typeDeclaration)
            => DomainProxy.CreateType(typeDeclaration);

        public void CopyEditorLibraries(string[] assemblyPaths)
        {
            if (assemblyPaths == null || assemblyPaths.Length == 0)
                return;

            //Get editor exe path
            string editorAssemblyPath = Assembly.GetExecutingAssembly().Location;
            //Get all dll files from editor directory
            string editorDir = Path.GetDirectoryName(editorAssemblyPath);
            string[] editorDLLPaths = Directory.GetFiles(editorDir);

            foreach (var compiledDLLPath in assemblyPaths)
            {
                foreach (var editorDLLPath in editorDLLPaths)
                {
                    string editorDLLName = Path.GetFileName(editorDLLPath);
                    string compiledDLLDir = Path.GetDirectoryName(compiledDLLPath);
                    string[] compiledDirDLLS = Directory.GetFiles(compiledDLLDir);

                    //if (!compiledDirDLLS.Any(path => Path.GetFileName(path).
                    //    EqualsInvariantIgnoreCase(editorDLLName)))
                    //{
                    //Copy the editor's dll to the compile path
                    string destPath = Path.Combine(compiledDLLDir, editorDLLName);
                    File.Copy(editorDLLPath, destPath, true);
                    //}
                }
            }
        }

        public void CreateGameDomain(TProject project, string rootDir, string[] assemblyPaths)
        {
            Engine.PrintLine("Creating game domain.");
            Engine.PrintLine("Active domains before load: " + string.Join(", ", AppDomainHelper.AppDomains.Select(x => x.FriendlyName)));

            try
            {
                if (_gameDomain != null)
                {
                    DomainProxy.Destroyed();
                    _gameDomain.Dispose();
                    _gameDomain = null;
                }

                CopyEditorLibraries(assemblyPaths);

                AppDomainSetup setupInfo = new AppDomainSetup()
                {
                    ApplicationName = project.Name,
                    ApplicationBase = rootDir,
                    PrivateBinPath = rootDir,
                    ShadowCopyFiles = "true",
                    ShadowCopyDirectories = string.Join(";", assemblyPaths.Select(x => Path.GetDirectoryName(x))),
                    LoaderOptimization = LoaderOptimization.MultiDomain,
                    //DisallowApplicationBaseProbing = true,
                };

                _gameDomain = AppDomainContext<TheraAssemblyTargetLoader, TheraAssemblyResolver>.
                    Create<TheraAssemblyTargetLoader, TheraAssemblyResolver>(setupInfo);

                if (assemblyPaths != null)
                    foreach (string path in assemblyPaths)
                    {
                        FileInfo file = new FileInfo(path);
                        if (!file.Exists)
                            continue;

                        Engine.PrintLine("Loading compiled assembly at " + path);
                        _gameDomain.RemoteResolver.AddProbePath(file.Directory.FullName);
                        _gameDomain.LoadAssembly(LoadMethod.LoadBits, path);
                    }

                DomainProxy = _gameDomain.Domain.CreateInstanceAndUnwrap<ProjectDomainProxy>();

                //dynamic dynProxy = proxy;
                //string info = dynProxy.GetVersionInfo();

                //Type type = typeof(ProjectDomainProxy);
                //string info3 = type.Assembly.CodeBase;
                //string info4 = dynProxy.GetType().Assembly.CodeBase;

                //Engine.PrintLine(info);
                //Engine.PrintLine(info3);
                //Engine.PrintLine(info4);

                TypeProxy.TypeCreationFailed = TypeCreationFailed;

                var lease = DomainProxy.InitializeLifetimeService() as ILease;
                lease.Register(DomainProxy.SponsorRef);

                DomainProxy.Created(project);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }

            Engine.PrintLine("Game domain created.");

            AppDomainHelper.ResetAppDomainCache();
            Engine.PrintLine("Active domains after load: " + string.Join(", ", AppDomainHelper.AppDomains.Select(x => x.FriendlyName)));

            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Engine.PrintLine(string.Join("\n", assemblies.Select(x => x.FullName)));
            //_gameDomain.Domain.AssemblyLoad += Domain_AssemblyLoad;
        }

        public class TheraAssemblyLoader : MarshalByRefObject, IAssemblyLoader
        {
            public Assembly LoadAssembly(LoadMethod loadMethod, string assemblyPath, string pdbPath = null)
            {
                Assembly assembly;
                //switch (loadMethod)
                //{
                //    case LoadMethod.LoadFrom:
                //        assembly = Assembly.LoadFrom(assemblyPath);
                //        break;
                //    case LoadMethod.LoadFile:
                //        assembly = Assembly.LoadFile(assemblyPath);
                //        break;
                //    case LoadMethod.LoadBits:

                // Attempt to load the PDB bits along with the assembly to avoid image exceptions.
                if (string.IsNullOrEmpty(pdbPath))
                    pdbPath = Path.ChangeExtension(assemblyPath, "pdb");

                // Only load the PDB if it exists--we may be dealing with a release assembly.
                if (File.Exists(pdbPath))
                {
                    assembly = Assembly.Load(
                        File.ReadAllBytes(assemblyPath),
                        File.ReadAllBytes(pdbPath));
                }
                else
                {
                    assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
                }

                string assemblyName = assembly.GetName().Name;
                string domainName = AppDomain.CurrentDomain.FriendlyName;
                Debug.Print($"{nameof(AppDomain)} {domainName} loaded assembly {assemblyName} via {nameof(TheraAssemblyLoader)}");

                //    break;
                //default:
                //    // In case we update the enum but forget to update this logic.
                //    throw new NotSupportedException("The target load method isn't supported!");
                //}

                return assembly;
            }
            public Assembly ReflectionOnlyLoadAssembly(LoadMethod loadMethod, string assemblyPath)
            {
                Assembly assembly;
                switch (loadMethod)
                {
                    case LoadMethod.LoadFrom:
                        assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                        break;
                    case LoadMethod.LoadFile:
                        throw new NotSupportedException("The target load method isn't supported!");
                    case LoadMethod.LoadBits:
                        assembly = Assembly.ReflectionOnlyLoad(File.ReadAllBytes(assemblyPath));
                        break;
                    default:
                        // In case we upadate the enum but forget to update this logic.
                        throw new NotSupportedException("The target load method isn't supported!");
                }

                return assembly;
            }
            /// <inheritdoc />
            /// <remarks>
            /// This implementation will perform a best-effort load of the target assembly and its required references
            /// into the current application domain. The .NET framework pins us in on which call we're allowed to use
            /// when loading these assemblies, so we'll need to rely on the AssemblyResolver instance attached to the
            /// AppDomain in order to load the way we want.
            /// </remarks>
            public IList<Assembly> LoadAssemblyWithReferences(LoadMethod loadMethod, string assemblyPath)
            {
                var list = new List<Assembly>();
                var assembly = LoadAssembly(loadMethod, assemblyPath);
                list.Add(assembly);

                foreach (var reference in assembly.GetReferencedAssemblies())
                    list.Add(Assembly.Load(reference));

                return list;
            }

            public Assembly[] GetAssemblies()
                => AppDomain.CurrentDomain.GetAssemblies();

            public Assembly[] ReflectionOnlyGetAssemblies()
                => AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();
        }
        private class TheraAssemblyTargetLoader : MarshalByRefObject, IAssemblyTargetLoader
        {
            public TheraAssemblyTargetLoader()
            {
                _loader = new TheraAssemblyLoader();
            }

            private readonly IAssemblyLoader _loader;

            public IAssemblyTarget LoadAssembly(LoadMethod loadMethod, string assemblyPath, string pdbPath = null)
            {
                var assembly = _loader.LoadAssembly(loadMethod, assemblyPath, pdbPath);
                IAssemblyTarget target;
                if (loadMethod == LoadMethod.LoadBits)
                {
                    // Assemblies loaded by bits will have the codebase set to the assembly that loaded it. Set it to the correct path here.
                    var codebaseUri = new Uri(assemblyPath);
                    target = AssemblyTarget.FromPath(codebaseUri, assembly.Location, assembly.FullName);
                }
                else
                {
                    target = AssemblyTarget.FromAssembly(assembly);
                }

                return target;
            }
            public IAssemblyTarget ReflectionOnlyLoadAssembly(LoadMethod loadMethod, string assemblyPath)
            {
                IAssemblyTarget target;
                var assembly = _loader.ReflectionOnlyLoadAssembly(loadMethod, assemblyPath);
                if (loadMethod == LoadMethod.LoadBits)
                {
                    // Assemlies loaded by bits will have the codebase set to the assembly that loaded it. Set it to the correct path here.
                    var codebaseUri = new Uri(assemblyPath);
                    target = AssemblyTarget.FromPath(codebaseUri, assembly.Location, assembly.FullName);
                }
                else
                {
                    target = AssemblyTarget.FromAssembly(assembly);
                }

                return target;
            }
            public IList<IAssemblyTarget> LoadAssemblyWithReferences(LoadMethod loadMethod, string assemblyPath)
            {
                return _loader.LoadAssemblyWithReferences(loadMethod, assemblyPath).Select(x => AssemblyTarget.FromAssembly(x)).ToList();
            }
            public IAssemblyTarget[] GetAssemblies()
            {
                var assemblies = _loader.GetAssemblies();
                return assemblies.Select(x => AssemblyTarget.FromAssembly(x)).ToArray();
            }
            public IAssemblyTarget[] ReflectionOnlyGetAssemblies()
            {
                var assemblies = _loader.ReflectionOnlyGetAssemblies();
                return assemblies.Select(x => AssemblyTarget.FromAssembly(x)).ToArray();
            }
        }
        private class TheraAssemblyResolver : MarshalByRefObject, IAssemblyResolver
        {
            public TheraAssemblyResolver()
                : this(null, LoadMethod.LoadFrom) { }

            public TheraAssemblyResolver(
                IAssemblyLoader loader = null,
                LoadMethod loadMethod = LoadMethod.LoadFrom)
            {
                _probePaths = new HashSet<string>();
                _loader = loader ?? new TheraAssemblyLoader();
                LoadMethod = loadMethod;
            }

            private readonly HashSet<string> _probePaths;
            private readonly IAssemblyLoader _loader;

            private string _applicationBase;
            private string _privateBinPath;

            public LoadMethod LoadMethod { get; set; }

            public string ApplicationBase
            {
                get => _applicationBase;
                set
                {
                    _applicationBase = value;
                    AddProbePath(value);
                }
            }
            public string PrivateBinPath
            {
                get => _privateBinPath;
                set
                {
                    _privateBinPath = value;
                    AddProbePath(value);
                }
            }

            public void AddProbePath(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return;

                if (path.Contains(";"))
                {
                    var paths = path.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    AddProbePaths(paths);
                }
                else
                    AddProbePaths(path);
            }
            public void AddProbePaths(params string[] paths)
            {
                foreach (var path in paths)
                {
                    if (string.IsNullOrEmpty(path))
                        continue;

                    var dir = new DirectoryInfo(path);
                    if (!_probePaths.Contains(dir.FullName))
                        _probePaths.Add(dir.FullName);
                }
            }
            /// <summary>
            /// Removes the given probe path or semicolon separated list of probe paths from the assembly loader.
            /// </summary>
            /// <param name="path">The path to remove.</param>
            public void RemoveProbePath(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return;

                if (path.Contains(";"))
                {
                    var paths = path.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    RemoveProbePaths(paths);
                }
                else
                    RemoveProbePaths(path);
            }
            /// <summary>
            /// Removes the given probe paths from the assembly loader.
            /// </summary>
            /// <param name="paths">The paths to remove.</param>
            public void RemoveProbePaths(params string[] paths)
            {
                foreach (var dir in from path in paths
                                    where !string.IsNullOrEmpty(path)
                                    select new DirectoryInfo(path))
                    _probePaths.Remove(dir.FullName);
            }
            public Assembly Resolve(object sender, ResolveEventArgs args)
            {
                var name = new AssemblyName(args.Name);
                foreach (var path in _probePaths)
                {
                    var dllPath = Path.Combine(path, string.Format("{0}.dll", name.Name));
                    if (File.Exists(dllPath))
                        return _loader.LoadAssembly(LoadMethod, dllPath);

                    var exePath = Path.ChangeExtension(dllPath, "exe");
                    if (File.Exists(exePath))
                        return _loader.LoadAssembly(LoadMethod, exePath);
                }

                return null;
            }
        }
    }
}