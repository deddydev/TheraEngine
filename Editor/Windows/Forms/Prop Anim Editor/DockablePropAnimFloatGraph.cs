//using ComponentOwl.BetterListView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Rendering.Models.Materials.Functions;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockablePropAnimFloatGraph : DockContent, IEditorControl
    {
        public DockablePropAnimFloatGraph()
        {
            InitializeComponent();
            RenderPanel.AllowDrop = false;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
            RenderPanel.MouseEnter += RenderPanel_MouseEnter;
            RenderPanel.MouseLeave += RenderPanel_MouseLeave;
            GameMode = new PropAnimFloatEditorGameMode() { RenderPanel = RenderPanel };
        }

        private void RenderPanel_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }
        private void RenderPanel_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
            if (TargetAnimation != null)
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetFileObject = TargetAnimation;
        }
        
        public PropAnimFloatEditorGameMode GameMode { get; set; }

        LocalPlayerIndex IEditorControl.PlayerIndex => LocalPlayerIndex.One;
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanel;
        IPawn IEditorControl.EditorPawn => RenderPanel.UI;
        BaseGameMode IEditorControl.GameMode => GameMode;

        public PropAnimFloat TargetAnimation
        {
            get => RenderPanel.UI.TargetAnimation;
            internal set
            {
                RenderPanel.UI.TargetAnimation = value;
                if (value != null)
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetFileObject = value;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
        }
        protected override void OnShown(EventArgs e)
        {
            RenderPanel.FormShown();
            base.OnShown(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            RenderPanel.FormClosed();
            base.OnClosing(e);
        }
        //protected override void OnClosed(EventArgs e)
        //{
        //    base.OnClosed(e);
        //}

        public void AlignView(BoundingRectangleF bounds)
        {
            ////Get aspect of the front plane of the aabb
            //aabb.GetCorners(out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            //float w = Math.Abs(TFR.X - TFL.X);
            //float h = Math.Abs(TFR.Y - BFR.Y);
            //float aspect = w / h;

            ////Get the aspect of the camera
            //PerspectiveCamera cam = EditorPawn.Camera;
            //float cAspect = cam.Width / cam.Height;

            ////Start the camera off at the center of the aabb
            //Vec3 pos = aabb.Translation;

            ////Move the camera back to start at the front plane of the aabb
            //pos.Z += aabb.HalfExtents.Z;

            ////Move the camera back to show all of the front plane rectangle
            //pos.Z += aspect > cAspect ? cam.FrustumDistanceAtWidth(w) : cam.FrustumDistanceAtHeight(h);

            //EditorPawn.RootComponent.SetTranslation(pos);
        }

        //protected override void OnResizeBegin(EventArgs e)
        //{
        //    RenderPanel.BeginResize();
        //    base.OnResizeBegin(e);
        //}
        //protected override void OnResizeEnd(EventArgs e)
        //{
        //    RenderPanel.EndResize();
        //    base.OnResizeEnd(e);
        //}

        //private MaterialFunction _dragged = null;
        private void RenderPanel_DragEnter(object sender, DragEventArgs e)
        {
            string[] fmts = e.Data.GetFormats();
            if (fmts == null || fmts.Length == 0)
                return;

            //BetterListViewItemDragData data = e.Data.GetData(fmts[0]) as BetterListViewItemDragData;
            //if (data.Items.Count == 0)
            //    return;

            //BetterListViewItem item = data.Items[0];
            //MaterialEditorForm editor = DockPanel.FindForm() as MaterialEditorForm;
            //MatFuncInfo info = editor.MaterialFunctions._funcs[(int)item.Tag];
            //_dragged = info.CreateNew();

            //UIMaterialEditor ui = RenderPanel.UI;
            //_dragged.LocalTranslation = Vec3.TransformPosition(ui.CursorPositionWorld(), ui._rootTransform.InverseWorldMatrix).Xy;
            //ui.AddMaterialFunction(_dragged);
            //ui._highlightedFunc = _dragged;
            //RenderPanel.Focus();
            //ui.LeftClickDown();
        }
        
        private void RenderPanel_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void RenderPanel_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void RenderPanel_DragLeave_1(object sender, EventArgs e)
        {
            //if (_dragged == null)
            //    return;
            //RenderPanel.UI.LeftClickUp();
            //RenderPanel.UI.RemoveMaterialFunction(_dragged);
            //_dragged = null;
        }
    }
    public class PropAnimFloatPlayerController : LocalPlayerController
    {
        public PropAnimFloatPlayerController(LocalPlayerIndex index) : this(index, null) { }
        public PropAnimFloatPlayerController(LocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => SetViewportCamera = SetViewportHUD = false;
    }
    public class PropAnimFloatGraphRenderPanel : UIRenderPanel<UIPropAnimFloatEditor, PropAnimFloatEditorGameMode, PropAnimFloatPlayerController> { }
    public class PropAnimFloatEditorGameMode : UIGameMode<UIPropAnimFloatEditor, PropAnimFloatPlayerController> { }
}
