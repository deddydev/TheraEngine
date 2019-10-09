using AppDomainToolkit;
using Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using TheraEngine;
using TheraEngine.Core.Files;
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
            int num = DomainProxy.OperationCount;
            DomainProxy.CancelOperations();
            toolStripStatusLabel1.Text = num == 1 ?
                "Operation was canceled." :
                num + " operations were canceled.";
        }
        private void BtnViewAnalytics_Click(object sender, EventArgs e)
            => GPUAnalyticsForm.Focus();
        private void BtnNewProject_Click(object sender, EventArgs e) 
            => CreateNewProject();
        private void BtnOpenProject_Click(object sender, EventArgs e) 
            => OpenProject();
        private void BtnSaveProject_Click(object sender, EventArgs e) 
            => SaveProject();
        private void BtnSaveProjectAs_Click(object sender, EventArgs e) 
            => SaveProjectAs();
        private void BtnNewWorld_Click(object sender, EventArgs e)
            => CreateNewWorld();
        private void BtnOpenWorld_Click(object sender, EventArgs e)
            => OpenWorld();
        private void BtnSaveWorld_Click(object sender, EventArgs e)
            => DomainProxy.SaveWorld();
        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
            => DomainProxy.SaveWorldAs();
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
            => TryCloseProject();
        private void btnCloseWorld_Click(object sender, EventArgs e)
            => TryCloseWorld();
        private void btnUndo_Click(object sender, EventArgs e)
            => DomainProxy.Undo();
        private void btnRedo_Click(object sender, EventArgs e)
            => DomainProxy.Redo();
        private void btnViewTools_Click(object sender, EventArgs e)
        {

        }
        private async void btnCompile_Click(object sender, EventArgs e)
            => await Project?.CompileAsync();
        private void visualStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnvDTE80.DTE2 dte = VisualStudioManager.CreateVSInstance();
            if (dte is null)
            {
                Engine.PrintLine($"Unable to launch Visual Studio.");
                return;
            }

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
            => PropertyGridForm.PropertyGrid.TargetObject = Project?.Game?.EngineSettingsOverrideRef;
        private void BtnEditorSettings_Click(object sender, EventArgs e)
            => PropertyGridForm.PropertyGrid.TargetObject = DefaultSettingsRef;
        private void BtnUserSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetObject = Project?.Game?.UserSettingsRef;
        private void BtnWorldSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.SettingsRef;
        private void BtnNewMaterial_Click(object sender, EventArgs e) 
            => new MaterialEditorForm().Show();
        private void btnUploadNewRelease_Click(object sender, EventArgs e)
        {
#if DEBUG
            ReleaseCreatorForm.ShowInstance();
#endif
        }

        private AppDomainContext<TheraAssemblyTargetLoader, TheraAssemblyResolver> _gameDomain;

        public void CopyEditorLibraries(PathReference[] assemblyPaths)
        {
            if (assemblyPaths is null || assemblyPaths.Length == 0)
                return;

            //Get editor exe path
            string editorAssemblyPath = Assembly.GetExecutingAssembly().Location;
            //Get all dll files from editor directory
            string editorDir = Path.GetDirectoryName(editorAssemblyPath);
            string[] editorDLLPaths = Directory.GetFiles(editorDir);

            foreach (var compiledDLLPath in assemblyPaths)
            {
                string compiledDLLDir = Path.GetDirectoryName(compiledDLLPath.Path);
                if (!Directory.Exists(compiledDLLDir))
                {
                    Engine.LogWarning($"DLL directory does not exist at {compiledDLLDir}");
                    continue;
                }

                foreach (var editorDLLPath in editorDLLPaths)
                {
                    string editorDLLName = Path.GetFileName(editorDLLPath);
                    string[] compiledDirDLLS = Directory.GetFiles(compiledDLLDir);

                    //if (!compiledDirDLLS.Any(path => Path.GetFileName(path).EqualsInvariantIgnoreCase(editorDLLName)))
                    //{
                    //Copy the editor's dll to the compile path
                    string destPath = Path.Combine(compiledDLLDir, editorDLLName);
                    File.Copy(editorDLLPath, destPath, true);
                    //}
                }
            }
        }

        public void CreateGameDomain(string gamePath, string rootDir, PathReference[] assemblyPaths)
        {
            DestroyGameDomain();

            //if (project is null)
            //    return;

            Engine.PrintLine("Creating game domain.");
            Engine.PrintLine("Active domains before load: " + string.Join(", ", AppDomainHelper.AppDomains.Select(x => x.FriendlyName)));

            try
            {
                CopyEditorLibraries(assemblyPaths);

                string name = Path.GetFileNameWithoutExtension(gamePath);
                AppDomainSetup setupInfo = new AppDomainSetup()
                {
                    ApplicationName = name,
                    ApplicationBase = rootDir,
                    PrivateBinPath = rootDir,
                    ShadowCopyFiles = "true",
                    ShadowCopyDirectories = assemblyPaths is null ? null : string.Join(";", assemblyPaths.Select(x => Path.GetDirectoryName(x.Path))),
                    LoaderOptimization = LoaderOptimization.MultiDomain,
                    //DisallowApplicationBaseProbing = true,
                };

                _gameDomain = AppDomainContext<TheraAssemblyTargetLoader, TheraAssemblyResolver>.
                    Create<TheraAssemblyTargetLoader, TheraAssemblyResolver>(setupInfo);

                if (assemblyPaths != null)
                    foreach (PathReference path in assemblyPaths)
                    {
                        FileInfo file = new FileInfo(path.Path);
                        if (!file.Exists)
                            continue;

                        _gameDomain.LoadAssembly(LoadMethod.LoadBits, path.Path);
                        _gameDomain.RemoteResolver.AddProbePath(file.Directory.FullName);
                    }

                Engine.Instance.SetDomainProxy<EngineDomainProxyEditor>(_gameDomain.Domain, gamePath);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            finally
            {
                Engine.PrintLine("Game domain created.");
                AppDomainHelper.ResetAppDomainCache();
                Engine.PrintLine("Active domains after load: " + string.Join(", ", AppDomainHelper.AppDomains.Select(x => x.FriendlyName)));
                AppDomainHelper.OnGameDomainLoaded();
            }
        }

        private void DestroyGameDomain()
        {
            Engine.DomainProxy.Stop();
            Engine.Instance.SetDomainProxy<EngineDomainProxyEditor>(AppDomain.CurrentDomain, null);
            _gameDomain?.Dispose();
            _gameDomain = null;
            AppDomainHelper.OnGameDomainUnloaded();
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
                if (domainName == AppDomainHelper.GetPrimaryAppDomain().FriendlyName)
                    throw new Exception();
                Debug.Print($"[{domainName}] Loaded assembly {assemblyName} via {nameof(TheraAssemblyLoader)}");

                //    break;
                //default:
                //    // In case we update the enum but forget to update this logic.
                //    throw new NotSupportedException("The target load method isn't supported!");
                //}

                return assembly;
            }
            public Assembly ReflectionOnlyLoadAssembly(LoadMethod loadMethod, string assemblyPath)
            {
                switch(loadMethod)
                {
                    case LoadMethod.LoadFrom:
                        return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                    case LoadMethod.LoadFile:
                        throw new NotSupportedException("The target load method isn't supported!");
                    case LoadMethod.LoadBits:
                        return Assembly.ReflectionOnlyLoad(File.ReadAllBytes(assemblyPath));
                    default:
                        throw new NotSupportedException("The target load method isn't supported!");
                };
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
                    var dllPath = Path.Combine(path, $"{name.Name}.dll");
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