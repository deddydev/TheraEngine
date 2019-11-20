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
    [EditorFor(typeof(PropAnimInt))]
    public partial class DockablePropAnimIntGraph : DockableRenderableFileEditor<PropAnimInt, PropAnimIntGraphRenderHandler>
    {
        public DockablePropAnimIntGraph()
        {
            InitializeComponent();
            
            tsPropAnimInt.RenderMode = ToolStripRenderMode.Professional;
            tsPropAnimInt.Renderer = new TheraToolStripRenderer();
        }
        public DockablePropAnimIntGraph(PropAnimInt anim) : this() => File = anim;

        public override bool ShouldHideCursor => true;
        
        protected override bool TrySetFile(PropAnimInt file)
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
    public class PropAnimIntPlayerController : LocalPlayerController
    {
        public PropAnimIntPlayerController(ELocalPlayerIndex index) : this(index, null) { }
        public PropAnimIntPlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => InheritControlledPawnCamera = InheritControlledPawnHUD = false;
    }
    public class PropAnimIntGraphRenderHandler: UIRenderHandler<EditorUIPropAnimInt, PropAnimIntEditorGameMode, PropAnimIntPlayerController> { }
    public class PropAnimIntEditorGameMode : UIGameMode<EditorUIPropAnimInt, PropAnimIntPlayerController> { }
}
