﻿using System;
using System.Diagnostics;
using System.Reflection;
using TheraEngine;

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
                _operations[i].Cancel();
            
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
    }
}