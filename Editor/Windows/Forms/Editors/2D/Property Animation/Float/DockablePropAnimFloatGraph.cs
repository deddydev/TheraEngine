using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Input;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(PropAnimFloat))]
    public partial class DockablePropAnimFloatGraph : DockableRenderableFileEditor<PropAnimFloat, PropAnimFloatGraphRenderHandler>
    {
        public DockablePropAnimFloatGraph()
        {
            InitializeComponent();

            tsPropAnimFloat.RenderMode = ToolStripRenderMode.Professional;
            tsPropAnimFloat.Renderer = new TheraToolStripRenderer();
        }
        public DockablePropAnimFloatGraph(PropAnimFloat anim) : this() => File = anim;

        public override bool ShouldHideCursor => true;

        protected override bool TrySetFile(PropAnimFloat file)
        {
            if (!base.TrySetFile(file))
                return false;

            RenderPanel.RenderHandler.UI.TargetAnimation = file;
            if (file != null)
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = file;

            return true;
        }

        private void btnZoomExtents_Click(object sender, EventArgs e)
            => RenderPanel.RenderHandler.UI.ZoomExtents();

        private void chkAutoTangents_Click(object sender, EventArgs e)
        {
            chkAutoTangents.Checked = !chkAutoTangents.Checked;
            RenderPanel.RenderHandler.UI.AutoGenerateTangents = chkAutoTangents.Checked;
        }

        private void chkSnapToUnits_Click(object sender, EventArgs e)
        {
            chkSnapToUnits.Checked = !chkSnapToUnits.Checked;
            RenderPanel.RenderHandler.UI.SnapToUnits = chkSnapToUnits.Checked;
        }
    }
    public class PropAnimFloatPlayerController : LocalPlayerController
    {
        public PropAnimFloatPlayerController(ELocalPlayerIndex index) : this(index, null) { }
        public PropAnimFloatPlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => InheritControlledPawnCamera = InheritControlledPawnHUD = false;
    }
    public class PropAnimFloatGraphRenderHandler : UIRenderHandler<EditorUIPropAnimFloat, PropAnimFloatEditorGameMode, PropAnimFloatPlayerController> { }
    public class PropAnimFloatEditorGameMode : UIGameMode<EditorUIPropAnimFloat, PropAnimFloatPlayerController> { }
}
