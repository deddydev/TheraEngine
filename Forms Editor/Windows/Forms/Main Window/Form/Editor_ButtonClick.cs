using System;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor
    {
        private void BtnViewAnalytics_Click(object sender, EventArgs e)
        {
            GPUAnalyticsForm.Focus();
        }
        private void BtnNewProject_Click(object sender, EventArgs e) => CreateNewProject();
        private void BtnOpenProject_Click(object sender, EventArgs e) => OpenProject();
        private void BtnSaveProject_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_project.FilePath))
            {
                BtnSaveProjectAs_Click(sender, e);
                return;
            }
            _project.Export();
        }
        private void BtnSaveProjectAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = TFileObject.GetFilter<Project>(),
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                    _project.Export(sfd.FileName);
            }
        }
        private void BtnNewWorld_Click(object sender, EventArgs e)
        {
            CreateNewWorld();
        }
        private void BtnOpenWorld_Click(object sender, EventArgs e)
        {
            OpenWorld();
        }
        private void BtnSaveWorld_Click(object sender, EventArgs e)
        {
            if (CurrentWorld == null)
                return;

            if (string.IsNullOrEmpty(CurrentWorld.FilePath))
            {
                saveAsToolStripMenuItem1_Click(sender, e);
                return;
            }
            ContentTree.WatchProjectDirectory = false;
            CurrentWorld.Export();
            ContentTree.WatchProjectDirectory = true;
        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CurrentWorld == null)
                return;

            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = TFileObject.GetFilter<World>(),
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    ContentTree.WatchProjectDirectory = false;
                    CurrentWorld.Export(sfd.FileName);
                    ContentTree.WatchProjectDirectory = true;
                }
            }
        }
        private void btnUndo_Click(object sender, EventArgs e) => UndoManager.Undo();
        private void btnRedo_Click(object sender, EventArgs e) => UndoManager.Redo();

        private void btnViewTools_Click(object sender, EventArgs e)
        {

        }

        private void btnCompile_Click(object sender, EventArgs e)
        {

            //Project.Compile("Debug", "x86");
        }

        private void visualStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnvDTE80.DTE2 dte = VisualStudioManager.CreateVSInstance();
            Engine.PrintLine($"Launched Visual Studio {dte.Edition} {dte.Version}.");
            dte.MainWindow.Visible = true;
            dte.UserControl = true;
            VisualStudioManager.VSInstanceClosed();
        }

        private void btnContact_Click(object sender, EventArgs e)
        {

        }
        private void btnDocumentation_Click(object sender, EventArgs e)
        {

        }
        private void btnCheckForUpdates_Click(object sender, EventArgs e) => CheckUpdates();
        private void btnAbout_Click(object sender, EventArgs e)
        {

        }

        private void BtnViewActorTree_Click(object sender, EventArgs e)
        {
            ActorTreeForm.Focus();
        }

        private void Viewport1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderForm1.Focus();
        }

        private void viewport2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderForm2.Focus();
        }

        private void viewport3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderForm3.Focus();
        }

        private void viewport4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderForm4.Focus();
        }

        private void btnViewFileTree_Click(object sender, EventArgs e)
        {
            FileTreeForm.Focus();
        }

        private void btnViewPropertyGrid_Click(object sender, EventArgs e)
        {
            PropertyGridForm.Focus();
        }

        private void btnViewOutput_Click(object sender, EventArgs e)
        {
            OutputForm.Focus();
        }

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
        {
            GameState = EEditorGameplayState.Editing;
        }

        private void BtnProjectSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetFileObject = Project;
        }
        private void BtnEngineSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetFileObject = Project?.EngineSettingsRef;
        }
        private void BtnEditorSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetFileObject = DefaultSettingsRef;
        }
        private void BtnUserSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetFileObject = Project?.UserSettingsRef;
        }
        private void BtnWorldSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetFileObject = Engine.World?.SettingsRef;
        }
        private void BtnNewMaterial_Click(object sender, EventArgs e)
        {
            new MaterialEditorForm().Show();
        }
    }
}
