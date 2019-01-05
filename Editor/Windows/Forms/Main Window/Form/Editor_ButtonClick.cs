using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core;
using TheraEngine.Core.Files.Serialization;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor
    {
        private void languageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void btnCancelOp_ButtonClick(object sender, EventArgs e)
        {
            for (int i = 0; i < _operations.Count; ++i)
                _operations[i].Cancel();

            EndOperation(-1);
            toolStripStatusLabel1.Text = _operations.Count == 1 ?
                "Operation was canceled." :
                _operations.Count.ToString() + " operations were canceled.";
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
        private void btnCompile_Click(object sender, EventArgs e)
            => Project.Compile();
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
        {
            if (GameState != EEditorGameplayState.Editing)
                GameState = EEditorGameplayState.Editing;
            else
                GameState = EEditorGameplayState.Attached;
        }
        private void btnPlayDetached_Click(object sender, EventArgs e)
        {
            if (GameState != EEditorGameplayState.Attached)
                GameState = EEditorGameplayState.Attached;
            else
                GameState = EEditorGameplayState.Detached;
        }
        private void EndGameplay() 
            => GameState = EEditorGameplayState.Editing;
        private void BtnProjectSettings_Click(object sender, EventArgs e)
            => PropertyGridForm.PropertyGrid.TargetFileObject = Project;
        private void BtnEngineSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetFileObject = Project?.EngineSettingsOverrideRef;
        private void BtnEditorSettings_Click(object sender, EventArgs e)
            => PropertyGridForm.PropertyGrid.TargetFileObject = DefaultSettingsRef;
        private void BtnUserSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetFileObject = Project?.UserSettingsRef;
        private void BtnWorldSettings_Click(object sender, EventArgs e) 
            => PropertyGridForm.PropertyGrid.TargetFileObject = Engine.World?.SettingsRef;
        private void BtnNewMaterial_Click(object sender, EventArgs e) 
            => new MaterialEditorForm().Show();
        private async void btnUploadNewRelease_Click(object sender, EventArgs e)
        {
#if DEBUG
            string progDir = Application.StartupPath;
            string[] exts =
            {
                ".dll",
                ".exe",
                ".config",
//#if DEBUG
//                ".pdb", //Debug symbols, not needed for public releases
//                ".xml", //Library code documentation, also not needed
//#endif
            };

            Engine.PrintLine("Creating new release...");

            //TODO: create release creator form

            string[] paths = Directory.EnumerateFileSystemEntries(progDir, "*.*", SearchOption.AllDirectories).
                Where(x => exts.Contains(Path.GetExtension(x).ToLowerInvariant())).ToArray();

            string tempFolderName = SerializationCommon.ResolveDirectoryName(progDir, "temp");
            string tempFolderPath = progDir + Path.DirectorySeparatorChar + tempFolderName;

            //if (Directory.Exists(tempFolderPath))
            //    Directory.Delete(tempFolderPath);
            Directory.CreateDirectory(tempFolderPath);
            string newPath;
            string relativePath;
            string[] parts;
            string fileName;
            foreach (string path in paths)
            {
                newPath = tempFolderPath;
                relativePath = path.Substring(progDir.Length);
                parts = relativePath.Split(Path.DirectorySeparatorChar);
                for (int i = 0; i < parts.Length - 1; ++i)
                {
                    newPath += Path.DirectorySeparatorChar + parts[i];
                    if (!Directory.Exists(newPath))
                        Directory.CreateDirectory(newPath);
                }
                fileName = parts[parts.Length - 1];
                File.Copy(path, newPath + Path.DirectorySeparatorChar + fileName);
            }

            string tagName = CreateTagName();
            string zipFilePath = progDir + Path.DirectorySeparatorChar + tagName + ".zip";

            if (File.Exists(zipFilePath))
                File.Delete(zipFilePath);

            int op = BeginOperation("Creating update zip...", out Progress<float> progress, out CancellationTokenSource cancel);
            IProgress<float> iProg = progress;
            await Task.Run(() => ZipFileWithProgress.CreateFromDirectory(tempFolderPath, zipFilePath, iProg));
            EndOperation(op);
            
            DeleteDirectory(tempFolderPath);

            await Github.ReleaseCreator.CreateNewRelease(Assembly.GetExecutingAssembly(), "test release", zipFilePath);

            File.Delete(zipFilePath);
#endif
        }
        public static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
                DeleteDirectory(directory);

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
        private static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption)
            => filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        private static string CreateTagName()
        {
            Assembly editorAssembly = Assembly.GetExecutingAssembly();
            AssemblyName name = editorAssembly.GetName();
            Version ver = name.Version;
            return $"{name.Name.ReplaceWhitespace("_")}_v{ver.ToString()}";
        }
    }
}