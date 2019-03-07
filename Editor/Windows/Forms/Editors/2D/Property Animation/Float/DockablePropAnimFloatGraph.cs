using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Core.Files;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms
{
    public abstract class DockableRenderableFileEditor : DockContent, IEditorControl
    {
        public virtual ELocalPlayerIndex PlayerIndex { get; } = ELocalPlayerIndex.One;
        public BaseGameMode GameMode { get; protected set; }
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanelGeneric as BaseRenderPanel;
        
        public abstract IPawn EditorPawn { get; }
        public abstract World World { get; }
        
        protected abstract IUIRenderPanel RenderPanelGeneric { get; }

        protected abstract TFileObject File { get; }

        public DockableRenderableFileEditor()
        {

        }

        protected void RenderPanel_MouseEnter(object sender, EventArgs e) => Cursor.Hide();
        protected void RenderPanel_MouseLeave(object sender, EventArgs e) => Cursor.Show();
        protected void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
            if (File != null)
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = File;
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
            base.OnHandleDestroyed(e);
        }
        protected override void OnShown(EventArgs e)
        {
            RenderPanelGeneric.FormShown();
            base.OnShown(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            RenderPanelGeneric.FormClosed();
            base.OnClosing(e);
        }

        protected void btnSave_Click(object sender, EventArgs e) => Save();
        protected void btnSaveAs_Click(object sender, EventArgs e) => SaveAs();
        protected void btnClose_Click(object sender, EventArgs e) => Close();

        public async void Save()
        {
            if (File == null)
                return;
            if (string.IsNullOrWhiteSpace(File.FilePath))
                SaveAs();
            else
            {
                int op = Editor.Instance.BeginOperation("Saving file...", out Progress<float> progress, out CancellationTokenSource cancel);
                await File.RootFile.ExportAsync(ESerializeFlags.Default, progress, cancel.Token);
                Editor.Instance.EndOperation(op);
            }
        }
        public async void SaveAs()
        {
            if (File == null)
                return;
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = File.GetFilter() })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                    await File.ExportAsync(sfd.FileName, ESerializeFlags.Default);
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (File == null || !File.EditorState.IsDirty)
                return;

            DialogResult result = MessageBox.Show(this, "Do you want to save your work before closing?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button3);
            if (result == DialogResult.Cancel)
                e.Cancel = true;
            else if (result == DialogResult.Yes)
                Save();
        }
    }
    [EditorFor(typeof(PropAnimFloat))]
    public partial class DockablePropAnimFloatGraph : DockableRenderableFileEditor
    {
        public DockablePropAnimFloatGraph()
        {
            InitializeComponent();

            RenderPanel.AllowDrop = false;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
            RenderPanel.MouseEnter += RenderPanel_MouseEnter;
            RenderPanel.MouseLeave += RenderPanel_MouseLeave;

            GameMode = new PropAnimFloatEditorGameMode() { RenderPanel = RenderPanel };

            tsPropAnimFloat.RenderMode = ToolStripRenderMode.Professional;
            tsPropAnimFloat.Renderer = new TheraToolstripRenderer();
        }
        public DockablePropAnimFloatGraph(PropAnimFloat anim) : this() => TargetAnimation = anim;
        
        public override IPawn EditorPawn => RenderPanel.UI;
        public override World World => RenderPanel.World;
        protected override TFileObject File => TargetAnimation;
        protected override IUIRenderPanel RenderPanelGeneric => RenderPanel;

        public PropAnimFloat TargetAnimation
        {
            get => RenderPanel.UI.TargetAnimation;
            internal set
            {
                RenderPanel.UI.TargetAnimation = value;
                if (value != null)
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = value;
            }
        }
        private void btnZoomExtents_Click(object sender, EventArgs e)
        {
            RenderPanel.UI.ZoomExtents();
        }
        private void chkAutoTangents_Click(object sender, EventArgs e)
        {
            chkAutoTangents.Checked = !chkAutoTangents.Checked;
            RenderPanel.UI.AutoGenerateTangents = chkAutoTangents.Checked;
        }
        private void chkSnapToUnits_Click(object sender, EventArgs e)
        {
            chkSnapToUnits.Checked = !chkSnapToUnits.Checked;
            RenderPanel.UI.SnapToUnits = chkSnapToUnits.Checked;
        }
    }
    public class PropAnimFloatPlayerController : LocalPlayerController
    {
        public PropAnimFloatPlayerController(ELocalPlayerIndex index) : this(index, null) { }
        public PropAnimFloatPlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => SetViewportCamera = SetViewportHUD = false;
    }
    public class PropAnimFloatGraphRenderPanel : UIRenderPanel<EditorUIPropAnimFloat, PropAnimFloatEditorGameMode, PropAnimFloatPlayerController> { }
    public class PropAnimFloatEditorGameMode : UIGameMode<EditorUIPropAnimFloat, PropAnimFloatPlayerController> { }
}
