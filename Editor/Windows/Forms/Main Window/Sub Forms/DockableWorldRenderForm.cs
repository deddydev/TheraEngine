using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableWorldRenderPanel : DockableWorldRenderPanelBase<WorldEditorRenderHandler>
    {
        public DockableWorldRenderPanel(ELocalPlayerIndex playerIndex, int formIndex)
            : base(playerIndex, formIndex)
        {
            InitializeComponent();

            RenderPanel.AllowDrop = true;
            RenderPanel.DragDrop += RenderPanel_DragDrop;
            RenderPanel.DragEnter += RenderPanel_DragEnter;
            RenderPanel.DragOver += RenderPanel_DragOver;
            RenderPanel.DragLeave += RenderPanel_DragLeave;

            Text = $"Viewport {(FormIndex + 1).ToString()}";
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Engine.Instance.DomainProxyCreated += Instance_DomainProxySet;
            Engine.Instance.DomainProxyDestroying += Instance_DomainProxyUnset;
            Instance_DomainProxySet(Engine.DomainProxy);
        }

        private void Instance_DomainProxyUnset(TheraEngine.Core.EngineDomainProxy obj)
        {
            RenderPanel.UnlinkFromWorldManager();
            //((EngineDomainProxyEditor)obj).RemoveRenderHandlerFromEditorGameMode(RenderPanel.Handle);
        }
        private void Instance_DomainProxySet(TheraEngine.Core.EngineDomainProxy obj)
        {
            //((EngineDomainProxyEditor)obj).AddRenderHandlerToEditorGameMode(RenderPanel.Handle);
            RenderPanel.LinkToWorldManager(Editor.Instance.WorldManagerId);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel)
                return;

            Engine.Instance.DomainProxyCreated -= Instance_DomainProxySet;
            Engine.Instance.DomainProxyDestroying -= Instance_DomainProxyUnset;
            Instance_DomainProxyUnset(Engine.DomainProxy);
        }

        #region Drag / Drop Actors

        ContentTreeNode _lastDraggedNode = null;
        IFileObject _dragInstance = null;
        private TransformType _prevTransformType;

        private void RenderPanel_DragEnter(object sender, DragEventArgs e)
        {
            ContentTreeNode[] dragNodes = Editor.Instance.ContentTree?.DraggedNodes;

            if (dragNodes is null || dragNodes.Length == 0)
                return;

            if (!(dragNodes[0] is FileTreeNode fileTreeNode))
                return;

            if (_lastDraggedNode != fileTreeNode)
            {
                _lastDraggedNode = fileTreeNode;
                _dragInstance = null;
            }

            if (!(fileTreeNode.Wrapper is IBaseFileWrapper wrapper))
                return;

            //var type = wrapper.FileType.GetFriendlyName();
            IFileObject instance = _dragInstance ?? (_dragInstance = wrapper.FileRefGeneric.LoadNewInstance());
            if (!(instance is IActor actor))
                return;

            RenderPanel.Focus();

            var handler = RenderPanel.RenderHandler;
            var editorPawn = handler.EditorPawn;
            var world = handler.World;

            EditorUI3D hud = editorPawn.HUD.File as EditorUI3D;

            IMap map = world.Settings.FindOrCreateMap(world.Settings.NewActorTargetMapName);
            map.Actors.Add(actor);

            Vec3 point = editorPawn.CameraComp.WorldPoint + editorPawn.Camera.ForwardVector * hud.DraggingTestDistance;
            world.SpawnActor(actor, point);

            _prevTransformType = hud.TransformMode;
            hud.TransformMode = TransformType.DragDrop;

            hud.HighlightedComponent = actor.RootComponent;
            hud.DoMouseDown();
        }

        private void RenderPanel_DragLeave(object sender, EventArgs e)
        {
            var handler = RenderPanel.RenderHandler;

            EditorUI3D hud = handler?.EditorPawn?.HUD?.File as EditorUI3D;
            if (hud?.DragComponent is null)
                return;

            handler?.World?.DespawnActor(((TheraEngine.Components.IComponent)hud.DragComponent).OwningActor);
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
            var handler = RenderPanel.RenderHandler;

            DragHelper.ImageList_DragLeave(Handle);
            _dragInstance = null;
            _lastDraggedNode = null;

            EditorUI3D hud = handler?.EditorPawn.HUD?.File as EditorUI3D;
            if (hud?.DragComponent is null)
                return;

            hud.DoMouseUp();
            hud.TransformMode = _prevTransformType;
            //Engine.TargetUpdateFreq = _preUpdateFreq;
            //Engine.TargetRenderFreq = _preRenderFreq;
            //Editor.Instance.DoEvents = true;
        }

        #endregion
    }
}
