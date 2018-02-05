using System;
using System.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Rendering;
using TheraEngine.Timers;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using WeifenLuo.WinFormsUI.Docking;
using TheraEngine.Actors.Types;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableRenderForm : DockContent, IEditorControl
    {
        public DockableRenderForm(LocalPlayerIndex playerIndex, int formIndex)
        {
            FormIndex = formIndex;
            PlayerIndex = playerIndex;
            InitializeComponent();
            EditorPawn = new FlyingCameraPawn(PlayerIndex)
            {
                HUD = new EditorHud(RenderPanel.ClientSize),
            };
            Text = string.Format("Viewport {0}", (FormIndex + 1).ToString());
            RenderPanel.AllowDrop = true;
            Engine.PreWorldChanged += Engine_WorldPreChanged;
            Engine.PostWorldChanged += Engine_WorldPostChanged;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
        }

        private void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Engine.SetGamePanel(RenderPanel, false);
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
            if (Engine.World == null || EditorPawn == null)
            {
                Text = string.Format("Viewport {0}", (FormIndex + 1).ToString());
            }
            else
            {
                Engine.World.SpawnActor(EditorPawn);
                Text = string.Format("{0} (Viewport {1})", Engine.World.Name, (FormIndex + 1).ToString());
            }
        }
        
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
            {
                Engine.SetGamePanel(null, false);
                Editor.SetActiveEditorControl(null);
            }
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

        BaseFileWrapper _lastDraggedNode = null;
        FileObject _dragInstance = null;
        //private float _preRenderFreq, _preUpdateFreq;
        private TransformType _prevTransformType;
        private void RenderPanel_DragEnter(object sender, DragEventArgs e)
        {
            BaseWrapper[] dragNodes = Editor.Instance.ContentTree?.DraggedNodes;
            if (dragNodes == null || dragNodes.Length != 1)
                return;
            BaseFileWrapper wrapper = dragNodes[0] as BaseFileWrapper;
            if (wrapper == null)
                return;
            if (_lastDraggedNode != wrapper)
            {
                _lastDraggedNode = wrapper;
                _dragInstance = null;
            }
            FileObject instance = _dragInstance ?? (_dragInstance = wrapper.GetNewInstance());
            if (instance is IActor actor)
            {
                //Editor.Instance.DoEvents = false;
                //_preRenderFreq = Engine.TargetRenderFreq;
                //_preUpdateFreq = Engine.TargetUpdateFreq;
                //Engine.TargetRenderFreq = 20.0f;
                //Engine.TargetUpdateFreq = 20.0f;
                BaseRenderPanel.HoveredPanel = RenderPanel;
                EditorHud hud = EditorPawn.HUD as EditorHud;
                Engine.World.SpawnActor(actor/*, EditorPawn.RootComponent.GetWorldPoint() +
                    EditorPawn.RootComponent.GetForwardDir() * hud._hitDistance*/);
                _prevTransformType = hud.TransformMode;
                hud.TransformMode = TransformType.DragDrop;
                hud.SetSelectedComponent(true, actor.RootComponent);
            }
        }

        private void RenderPanel_DragLeave(object sender, EventArgs e)
        {
            EditorHud hud = EditorPawn.HUD as EditorHud;
            if (hud.DragComponent != null)
            {
                Engine.World.DespawnActor(hud.DragComponent.OwningActor);
                hud.DoMouseUp();
                hud.TransformMode = _prevTransformType;
            }
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
            EditorHud hud = EditorPawn.HUD as EditorHud;
            if (hud.DragComponent != null)
            {
                hud.DoMouseUp();
                hud.TransformMode = _prevTransformType;
            }
            //Engine.TargetUpdateFreq = _preUpdateFreq;
            //Engine.TargetRenderFreq = _preRenderFreq;
            //Editor.Instance.DoEvents = true;
        }
    }
}
