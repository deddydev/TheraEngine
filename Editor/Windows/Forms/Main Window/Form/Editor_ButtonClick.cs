using AppDomainToolkit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor
    {
        private void BtnLanguage_Click(object sender, EventArgs e)
        {

        }
        private void BtnCancelOp_ButtonClick(object sender, EventArgs e)
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
        private void SaveAsToolStripMenuItem1_Click(object sender, EventArgs e)
            => DomainProxy.SaveWorldAs();
        private void ExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        private void CubeMapEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void BtnVREdit_Click(object sender, EventArgs e)
        {
            Engine.DomainProxy?.ToggleVRActive(WorldManagerId);
        }
        private void TextureGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void BtnCloseProject_Click(object sender, EventArgs e)
            => TryCloseProject();
        private void BtnCloseWorld_Click(object sender, EventArgs e)
            => TryCloseWorld();
        private void BtnUndo_Click(object sender, EventArgs e)
            => DomainProxy.Undo();
        private void BtnRedo_Click(object sender, EventArgs e)
            => DomainProxy.Redo();
        private void BtnViewTools_Click(object sender, EventArgs e)
        {
            
        }
        public async void Compile()
        {
            if (Project is null)
                return;

            await Project.CompileAsync();
            //RunOperationAsync2(null,
            //    "Compiling project...",
            //    "Finished compiling project.",
            //    async (p, c, a) => await ((TProject)a[0]).CompileAsync(),
            //    Project);
        }
        private void BtnCompile_Click(object sender, EventArgs e) => Compile();
        private void VisualStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnvDTE80.DTE2 dte = VisualStudioManager.CreateVSInstance();
            if (dte is null)
            {
                Engine.Out($"Unable to launch Visual Studio.");
                return;
            }

            Engine.Out($"Launched Visual Studio {dte.Edition} {dte.Version}.");
            dte.MainWindow.Visible = true;
            dte.UserControl = true;
            VisualStudioManager.VSInstanceClosed();
        }
        private void BtnContact_Click(object sender, EventArgs e) 
            => Process.Start(ContactURL);
        private void BtnDocumentation_Click(object sender, EventArgs e) 
            => Process.Start(DocumentationURL);
        private void BtnCheckForUpdates_Click(object sender, EventArgs e) 
            => CheckUpdates();
        private void BtnAbout_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }
        private void BtnViewActorTree_Click(object sender, EventArgs e) 
            => ActorTreeForm.Focus();
        private void Viewport1ToolStripMenuItem_Click(object sender, EventArgs e) 
            => RenderForm1.Focus();
        private void Viewport2ToolStripMenuItem_Click(object sender, EventArgs e) 
            => RenderForm2.Focus();
        private void Viewport3ToolStripMenuItem_Click(object sender, EventArgs e) 
            => RenderForm3.Focus();
        private void Viewport4ToolStripMenuItem_Click(object sender, EventArgs e) 
            => RenderForm4.Focus();
        private void BtnViewFileTree_Click(object sender, EventArgs e) 
            => FileTreeForm.Focus();
        private void BtnViewPropertyGrid_Click(object sender, EventArgs e) 
            => PropertyGridForm.Focus();
        private void BtnViewOutput_Click(object sender, EventArgs e) 
            => OutputForm.Focus();
        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void BtPlay_Click(object sender, EventArgs e)
            => DomainProxy.GameState = DomainProxy.GameState != EEditorGameplayState.Editing ? EEditorGameplayState.Editing : EEditorGameplayState.Attached;
        private void BtnPlayDetached_Click(object sender, EventArgs e)
            => DomainProxy.GameState = DomainProxy.GameState != EEditorGameplayState.Attached ? EEditorGameplayState.Attached : EEditorGameplayState.Detached;
        
        private void BtnProjectSettings_Click(object sender, EventArgs e)
            => PropertyGridForm.PropertyGrid.TargetObject = Project;
        private void BtnProjectEngineSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetObject = Project?.Game?.EngineSettingsOverrideRef;
        private void BtnDefaultEngineSettings_Click(object sender, EventArgs e)
            => PropertyGridForm.PropertyGrid.TargetObject = Engine.Instance.DefaultEngineSettingsOverrideRef;
        private void BtnEditorSettings_Click(object sender, EventArgs e)
            => PropertyGridForm.PropertyGrid.TargetObject = DefaultSettingsRef;
        private void BtnUserSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetObject = Project?.Game?.UserSettingsRef;
        private void BtnWorldSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.SettingsRef;
        private void BtnNewMaterial_Click(object sender, EventArgs e) 
            => new MaterialEditorForm().Show();

        internal void Item_LogicComponentsChanged(IActor actor)
        {
            if (InvokeRequired)
            {
                Invoke((Action<IActor>)Item_LogicComponentsChanged, actor);
                return;
            }

            TreeNode node = actor.EditorState.TreeNode;

            node.TreeView?.SuspendLayout();

            for (int i = 1; i < node.Nodes.Count; ++i)
                node.Nodes[i].Remove();

            foreach (ILogicComponent comp in actor.LogicComponents)
            {
                AppDomainHelper.Sponsor(comp);
                TreeNode childNode = new TreeNode(comp.ToString()) { Tag = comp };
                node.Nodes.Add(childNode);
            }

            node.TreeView?.ResumeLayout();
        }

        internal void Item_SceneComponentCacheRegenerated(IActor actor)
        {
            if (InvokeRequired)
            {
                Invoke((Action<IActor>)Item_SceneComponentCacheRegenerated, actor);
                return;
            }

            TreeNode node = actor.EditorState.TreeNode;

            node.TreeView?.SuspendLayout();

            if (node.Nodes.Count > 0)
                node.Nodes[0].Nodes.Clear();

            RecursiveAddSceneComp(node.Nodes[0], actor.RootComponent);

            node.TreeView?.ResumeLayout();
        }

        private static void RecursiveAddSceneComp(TreeNode node, ISocket comp)
        {
            AppDomainHelper.Sponsor(comp);
            node.Text = comp.ToString();
            node.Tag = comp;
            foreach (ISceneComponent child in comp.ChildSockets)
            {
                TreeNode childNode = new TreeNode();
                node.Nodes.Add(childNode);
                RecursiveAddSceneComp(childNode, child);
            }
        }

        private void btnUploadNewRelease_Click(object sender, EventArgs e)
        {
#if DEBUG
            ReleaseCreatorForm.ShowInstance();
#endif
        }

        private AppDomainContext<AssemblyTargetLoader, PathBasedAssemblyResolver> _gameDomain;

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
                    //string[] compiledDirDLLS = Directory.GetFiles(compiledDLLDir);

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
            //if (project is null)
            //    return;

            Engine.Out("Creating game domain.");

            AppDomainHelper.ResetAppDomainCache();
            Engine.Out("Active domains before load: " + AppDomainHelper.AppDomainStringList);

            try
            {
                CopyEditorLibraries(assemblyPaths);

                string name = Path.GetFileNameWithoutExtension(gamePath);
                AppDomainSetup setupInfo = new AppDomainSetup()
                {
                    ApplicationName = name + "_" + DateTime.Now.ToString("hh:mm:sstt"),
                    ApplicationBase = rootDir,
                    PrivateBinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    ShadowCopyFiles = "true",
                    ShadowCopyDirectories = assemblyPaths is null ? null : string.Join(";", assemblyPaths.Select(x => Path.GetDirectoryName(x.Path))),
                    LoaderOptimization = LoaderOptimization.MultiDomainHost,
                    DisallowApplicationBaseProbing = false,
                };

                var domain = AppDomainContext.Create(setupInfo);

                if (assemblyPaths != null)
                    foreach (PathReference path in assemblyPaths)
                    {
                        FileInfo file = new FileInfo(path.Path);
                        if (!file.Exists)
                            continue;

                        //domain.RemoteResolver.AddProbePath(file.Directory.FullName);
                        domain.LoadAssemblyWithReferences(ELoadMethod.LoadFrom, path.Path);
                    }

                Engine.Instance.SetDomainProxy<EngineDomainProxyEditor>(domain.Domain, _gameDomain, gamePath);

                _gameDomain = domain;
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                Engine.Out("Game domain created.");

                AppDomainHelper.ResetAppDomainCache();
                Engine.Out("Active domains after load: " + AppDomainHelper.AppDomainStringList);
            }
        }
    }
}