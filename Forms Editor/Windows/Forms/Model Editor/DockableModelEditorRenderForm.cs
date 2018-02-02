using System;
using System.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Timers;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableModelEditorRenderForm : DockContent, IEditorControl
    {
        public DockableModelEditorRenderForm(LocalPlayerIndex playerIndex, int formIndex, ModelEditorForm form)
        {
            Form = form;
            FormIndex = formIndex;
            PlayerIndex = playerIndex;
            InitializeComponent();
            RenderPanel.Owner = this;
            EditorPawn = new FlyingCameraPawn(PlayerIndex)
            {
                HUD = new EditorHud(RenderPanel.ClientSize),
            };
            Text = string.Format("Model Viewport {0}", (FormIndex + 1).ToString());
            RenderPanel.AllowDrop = true;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
        }

        private void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
        }
        
        //private void Engine_WorldPreChanged()
        //{
        //    if (Engine.World == null || EditorPawn == null)
        //    {

        //    }
        //    else
        //    {
        //        Engine.World.DespawnActor(EditorPawn);
        //    }
        //}
        //private void Engine_WorldPostChanged()
        //{
        //    if (Engine.World == null || EditorPawn == null)
        //    {
        //        Text = string.Format("Model Viewport {0}", (FormIndex + 1).ToString());
        //    }
        //    else
        //    {
        //        Engine.World.SpawnActor(EditorPawn);
        //        Text = string.Format("{0} (Model Viewport {1})", Engine.World.Name, (FormIndex + 1).ToString());
        //    }
        //}
        
        public ModelEditorForm Form { get; private set; }
        public int FormIndex { get; private set; }
        public LocalPlayerIndex PlayerIndex { get; private set; } = LocalPlayerIndex.One;
        public FlyingCameraPawn EditorPawn { get; private set; }

        LocalPlayerIndex IEditorControl.PlayerIndex => PlayerIndex;
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanel;
        IPawn IEditorControl.EditorPawn => EditorPawn;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
        }
        protected override void OnShown(EventArgs e)
        {
            Form.World.SpawnActor(EditorPawn);
            base.OnShown(e);
        }

        private void RenderTick(object sender, FrameEventArgs e)
        {
            RenderPanel.Invalidate();
            Application.DoEvents();
        }

        protected override void OnClosed(EventArgs e)
        {
            Form.World.DespawnActor(EditorPawn);
            base.OnClosed(e);
        }
        
        protected override void OnResizeBegin(EventArgs e)
        {
            RenderPanel.BeginResize();
            base.OnResizeBegin(e);
        }
        protected override void OnResizeEnd(EventArgs e)
        {
            RenderPanel.EndResize();
            base.OnResizeEnd(e);
        }

        protected override string GetPersistString()
            => GetType().ToString() + "," + FormIndex;

        #region Drag Drop
        //BaseFileWrapper _lastDraggedNode = null;
        //FileObject _dragInstance = null;
        ////private float _preRenderFreq, _preUpdateFreq;
        //private void RenderPanel_DragEnter(object sender, DragEventArgs e)
        //{
        //    BaseWrapper[] dragNodes = Editor.Instance.ContentTree?.DraggedNodes;
        //    if (dragNodes == null || dragNodes.Length != 1)
        //        return;
        //    BaseFileWrapper wrapper = dragNodes[0] as BaseFileWrapper;
        //    if (wrapper == null)
        //        return;
        //    if (_lastDraggedNode != wrapper)
        //    {
        //        _lastDraggedNode = wrapper;
        //        _dragInstance = null;
        //    }
        //    FileObject instance = _dragInstance ?? (_dragInstance = wrapper.GetNewInstance());
        //    if (instance is IActor actor)
        //    {
        //        //Editor.Instance.DoEvents = false;
        //        //_preRenderFreq = Engine.TargetRenderFreq;
        //        //_preUpdateFreq = Engine.TargetUpdateFreq;
        //        //Engine.TargetRenderFreq = 20.0f;
        //        //Engine.TargetUpdateFreq = 20.0f;
        //        BaseRenderPanel.HoveredPanel = RenderPanel;
        //        EditorHud hud = EditorPawn.HUD as EditorHud;
        //        Engine.World.SpawnActor(actor/*, EditorPawn.RootComponent.GetWorldPoint() +
        //            EditorPawn.RootComponent.GetForwardDir() * hud._hitDistance*/);
        //        hud.SetTransformMode(TransformType.DragDrop);
        //        hud.SetSelectedComponent(true, actor.RootComponent);
        //    }
        //}

        //private void RenderPanel_DragLeave(object sender, EventArgs e)
        //{
        //    EditorHud hud = EditorPawn.HUD as EditorHud;
        //    if (hud.DragComponent != null)
        //    {
        //        Engine.World.DespawnActor(hud.DragComponent.OwningActor);
        //        hud.DoMouseUp();
        //    }
        //    //Engine.TargetUpdateFreq = _preUpdateFreq;
        //    //Engine.TargetRenderFreq = _preRenderFreq;
        //    //Editor.Instance.DoEvents = true;
        //}

        //private void RenderPanel_DragOver(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Move;
        //    //RenderPanel.Invalidate();
        //}

        //private void RenderPanel_DragDrop(object sender, DragEventArgs e)
        //{
        //    DragHelper.ImageList_DragLeave(Handle);
        //    _dragInstance = null;
        //    _lastDraggedNode = null;
        //    EditorHud hud = EditorPawn.HUD as EditorHud;
        //    if (hud.DragComponent != null)
        //    {
        //        hud.DoMouseUp();
        //    }
        //    //Engine.TargetUpdateFreq = _preUpdateFreq;
        //    //Engine.TargetRenderFreq = _preRenderFreq;
        //    //Editor.Instance.DoEvents = true;
        //}
        #endregion
    }
}
