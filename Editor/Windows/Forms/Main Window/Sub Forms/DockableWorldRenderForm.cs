using System;
using System.Windows.Forms;
using TheraEditor.Actors.Types.Pawns;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Core.Files;
using TheraEngine.GameModes;
using TheraEngine.Timers;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableWorldRenderForm : DockContent, IEditorControl
    {
        public DockableWorldRenderForm(ELocalPlayerIndex playerIndex, int formIndex)
        {
            FormIndex = formIndex;
            PlayerIndex = playerIndex;
            InitializeComponent();
            EditorPawn = new EditorCameraPawn(PlayerIndex)
            {
                HUD = new EditorUI(RenderPanel.ClientSize),
                Name = $"Viewport{(FormIndex + 1).ToString()}_EditorCamera"
            };
            Text = $"Viewport {(FormIndex + 1).ToString()}";
            RenderPanel.AllowDrop = true;
            Engine.PreWorldChanged += Engine_WorldPreChanged;
            Engine.PostWorldChanged += Engine_WorldPostChanged;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
        }
        
        private void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Engine.SetWorldPanel(RenderPanel, false);
            Editor.SetActiveEditorControl(this);
        }
        
        private void Engine_WorldPreChanged()
        {
            if (Engine.World == null || EditorPawn == null)
            {

            }
            else
            {
                Engine.World.DespawnActor(EditorPawn);
            }
        }
        private void Engine_WorldPostChanged()
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action)Engine_WorldPostChanged, BaseRenderPanel.PanelType.Rendering))
                return;
            
            if (Engine.World == null || EditorPawn == null)
            {
                Text = $"Viewport {(FormIndex + 1).ToString()}";
            }
            else
            {
                Engine.World.SpawnActor(EditorPawn);
                Text = $"{Engine.World.Name} (Viewport {(FormIndex + 1).ToString()})";
            }
        }
        
        public int FormIndex { get; private set; }
        public ELocalPlayerIndex PlayerIndex { get; private set; } = ELocalPlayerIndex.One;
        public EditorCameraPawn EditorPawn { get; private set; }

        ELocalPlayerIndex IEditorControl.PlayerIndex => PlayerIndex;
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanel;
        IPawn IEditorControl.EditorPawn => EditorPawn;
        BaseGameMode IEditorControl.GameMode => Engine.ActiveGameMode;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (Editor.ActiveRenderForm != this)
                return;
            Engine.SetWorldPanel(null, false);
            Editor.SetActiveEditorControl(null);
        }
        protected override void OnShown(EventArgs e)
        {
            Engine.World?.SpawnActor(EditorPawn);
            base.OnShown(e);
        }

        private void RenderTick(object sender, FrameEventArgs e)
        {
            RenderPanel.Invalidate();
            Application.DoEvents();
        }

        protected override void OnClosed(EventArgs e)
        {
            Engine.World?.DespawnActor(EditorPawn);
            base.OnClosed(e);
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

        protected override string GetPersistString()
            => GetType() + "," + FormIndex;

        BaseFileWrapper _lastDraggedNode = null;
        IFileObject _dragInstance = null;
        //private float _preRenderFreq, _preUpdateFreq;
        private TransformType _prevTransformType;
        private async void RenderPanel_DragEnter(object sender, DragEventArgs e)
        {
            BaseWrapper[] dragNodes = Editor.Instance.ContentTree?.DraggedNodes;

            if (dragNodes == null || dragNodes.Length != 1)
                return;

            if (!(dragNodes[0] is BaseFileWrapper wrapper))
                return;

            if (_lastDraggedNode != wrapper)
            {
                _lastDraggedNode = wrapper;
                _dragInstance = null;
            }

            IFileObject instance = _dragInstance ?? (_dragInstance = await wrapper.GetNewInstanceAsync());
            if (!(instance is BaseActor actor))
                return;

            //Editor.Instance.DoEvents = false;
            //_preRenderFreq = Engine.TargetRenderFreq;
            //_preUpdateFreq = Engine.TargetUpdateFreq;
            //Engine.TargetRenderFreq = 20.0f;
            //Engine.TargetUpdateFreq = 20.0f;

            BaseRenderPanel.HoveredPanel = RenderPanel;
            RenderPanel.Focus();
            EditorUI hud = EditorPawn.HUD.File as EditorUI;
            Engine.World.SpawnActor(actor, EditorPawn.CameraComp.WorldPoint + EditorPawn.Camera.ForwardVector * hud.DraggingTestDistance);
            _prevTransformType = hud.TransformMode;
            hud.TransformMode = TransformType.DragDrop;
            hud.HighlightedComponent = actor.RootComponentGeneric;
            hud.DoMouseDown();
        }

        private void RenderPanel_DragLeave(object sender, EventArgs e)
        {
            EditorUI hud = EditorPawn.HUD.File as EditorUI;
            if (hud?.DragComponent is null)
                return;

            Engine.World.DespawnActor(hud.DragComponent.OwningActor);
            hud.DoMouseUp();
            hud.TransformMode = _prevTransformType;
            //Engine.TargetUpdateFreq = _preUpdateFreq;
            //Engine.TargetRenderFreq = _preRenderFreq;
            //Editor.Instance.DoEvents = true;
        }

        private void RenderPanel_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            //RenderPanel.Invalidate();
        }

        private void RenderPanel_DragDrop(object sender, DragEventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            _dragInstance = null;
            _lastDraggedNode = null;

            EditorUI hud = EditorPawn.HUD.File as EditorUI;
            if (hud?.DragComponent is null)
                return;

            hud.DoMouseUp();
            hud.TransformMode = _prevTransformType;
            //Engine.TargetUpdateFreq = _preUpdateFreq;
            //Engine.TargetRenderFreq = _preRenderFreq;
            //Editor.Instance.DoEvents = true;
        }
    }
}
