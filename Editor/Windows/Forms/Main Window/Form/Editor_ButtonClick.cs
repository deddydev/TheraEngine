using AppDomainToolkit;
using Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Mesh;
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
        public void Compile()
        {
            if (Project is null)
                return;

            Project.CompileAsync();
            //RunOperationAsync2(null,
            //    "Compiling project...",
            //    "Finished compiling project.",
            //    async (p, c, a) => await ((TProject)a[0]).CompileAsync(),
            //    Project);
        }
        private void btnCompile_Click(object sender, EventArgs e) => Compile();
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

        internal void Item_LogicComponentsChanged(IActor actor)
        {
            Invoke((Action)(() =>
            {
                TreeNode node = actor.EditorState.TreeNode;
                for (int i = 1; i < node.Nodes.Count; ++i)
                    node.Nodes[i].Remove();

                foreach (LogicComponent comp in actor.LogicComponents)
                {
                    AppDomainHelper.Sponsor(comp);
                    TreeNode childNode = new TreeNode(comp.ToString()) { Tag = comp };
                    node.Nodes.Add(childNode);
                }
            }));
        }

        internal void Item_SceneComponentCacheRegenerated(IActor actor)
        {
            Invoke((Action)(() =>
            {
                TreeNode node = actor.EditorState.TreeNode;
                node.Nodes[0].Nodes.Clear();
                RecursiveAddSceneComp(node.Nodes[0], actor.RootComponent);
            }));
        }

        private static void RecursiveAddSceneComp(TreeNode node, ISocket comp)
        {
            AppDomainHelper.Sponsor(comp);
            node.Text = comp.ToString();
            node.Tag = comp;
            foreach (SceneComponent child in comp.ChildComponents)
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
            //if (project is null)
            //    return;

            Engine.PrintLine("Creating game domain.");

            AppDomainHelper.ResetAppDomainCache();
            Engine.PrintLine("Active domains before load: " + AppDomainHelper.AppDomainStringList);

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
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            finally
            {
                Engine.PrintLine("Game domain created.");

                AppDomainHelper.ResetAppDomainCache();
                Engine.PrintLine("Active domains after load: " + AppDomainHelper.AppDomainStringList);
            }
        }
    }
}