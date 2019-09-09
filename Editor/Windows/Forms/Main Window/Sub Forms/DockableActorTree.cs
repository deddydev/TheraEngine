using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Components;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableActorTree : DockContent
    {
        public DockableActorTree()
        {
            InitializeComponent();

            ctxActorTree.RenderMode = ToolStripRenderMode.Professional;
            ctxActorTree.Renderer = new TheraForm.TheraToolstripRenderer();
        }
        
        public EditorUI3D EditorHUD { get; set; }

        private readonly Dictionary<Guid, TreeNode> _mapTreeNodes = new Dictionary<Guid, TreeNode>();
        private TreeNode _dynamicActorsMapNode = null;

        private void ActorTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!Editor.Instance.PropertyGridFormActive)
                return;
            
            if (ActorTree.SelectedNode == null)
            {
                EditorHUD?.SetSelectedComponent(false, null, true);
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.Settings;
            }
            else switch (e.Node.Tag)
            {
                case IActor actor:
                {
                    actor.EditorState.Selected = true;
                    EditorHUD?.SetSelectedComponent(false, actor.RootComponent, true);
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = actor;
                    break;
                }
                case IComponent component:
                {
                    component.EditorState.Selected = true;
                    EditorHUD?.SetSelectedComponent(false, component as ISceneComponent, true);
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = component;
                    break;
                }
                case IMap map:
                {
                    EditorHUD?.SetSelectedComponent(false, null, true);
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = map;
                    break;
                }
            }
        }
        internal void GenerateInitialActorList(IWorld world)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<IWorld>)GenerateInitialActorList, world);
                return;
            }

            ClearMaps();

            if (world is null || Engine.ShuttingDown)
                return;

            world.Settings.Maps.ForEach(x => CacheMap(x.Value.File));
            world.State.SpawnedActors.ForEach(ActorSpawned);
        }
        internal TreeNode CacheMap(IMap map)
        {
            TreeNode mapNode;
            if (map == null)
            {
                if (_dynamicActorsMapNode == null)
                {
                    _dynamicActorsMapNode = new TreeNode("Dynamic Actors");
                    ActorTree.Nodes.Insert(0, _dynamicActorsMapNode);
                }
                mapNode = _dynamicActorsMapNode;
            }
            else if (!_mapTreeNodes.ContainsKey(map.Guid))
            {
                mapNode = new TreeNode(map.Name) { Tag = map };
                ActorTree.Nodes.Add(mapNode);
                _mapTreeNodes.Add(map.Guid, mapNode);
                map.Renamed += Map_Renamed;
            }
            else
                mapNode = _mapTreeNodes[map.Guid];

            return mapNode;
        }

        private void Map_Renamed(TObject node, string oldName)
            => _mapTreeNodes[node.Guid].Text = node.Name;

        internal void ActorSpawned(IActor item)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IActor>(ActorSpawned), item);
                return;
            }

            if (item?.OwningWorld is null || Engine.ShuttingDown)
                return;

            IMap map = item.MapAttachment;
            TreeNode mapNode = CacheMap(map);

            TreeNode node = new TreeNode(item.ToString()) { Tag = item };
            node.Nodes.Add(new TreeNode());

            item.EditorState.TreeNode = node;
            mapNode.Nodes.Add(node);
            node.EnsureVisible();

            item.SceneComponentCacheRegenerated += Item_SceneComponentCacheRegenerated;
            item.LogicComponentsChanged += Item_LogicComponentsChanged;

            Item_LogicComponentsChanged(item);
            Item_SceneComponentCacheRegenerated(item);
        }
        internal void ActorDespawned(IActor item)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IActor>(ActorDespawned), item);
                return;
            }

            if (item is null || Engine.ShuttingDown)
                return;

            if (item.EditorState?.TreeNode != null)
            {
                item.EditorState.TreeNode.Remove();
                item.EditorState.TreeNode = null;
            }

            item.SceneComponentCacheRegenerated -= Item_SceneComponentCacheRegenerated;
            item.LogicComponentsChanged -= Item_LogicComponentsChanged;
        }
        private void Item_LogicComponentsChanged(IActor actor)
        {
            TreeNode node = actor.EditorState.TreeNode;

            for (int i = 1; i < node.Nodes.Count; ++i)
                node.Nodes[i].Remove();
            
            foreach (LogicComponent comp in actor.LogicComponents)
            {
                TreeNode childNode = new TreeNode(comp.ToString()) { Tag = comp };
                node.Nodes.Add(childNode);
            }
        }

        private static void Item_SceneComponentCacheRegenerated(IActor actor)
        {
            TreeNode node = actor.EditorState.TreeNode;
            node.Nodes[0].Nodes.Clear();
            RecursiveAddSceneComp(node.Nodes[0], actor.RootComponent);
        }
        private static void RecursiveAddSceneComp(TreeNode node, ISocket comp)
        {
            node.Text = comp.ToString();
            node.Tag = comp;
            foreach (SceneComponent child in comp.ChildComponents)
            {
                TreeNode childNode = new TreeNode();
                node.Nodes.Add(childNode);
                RecursiveAddSceneComp(childNode, child);
            }
        }

        public void ClearMaps()
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action)ClearMaps);
                return;
            }
            _dynamicActorsMapNode = null;
            _mapTreeNodes.Clear();
            ActorTree.Nodes.Clear();
        }

        private void ctxActorTree_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TreeNode node = ActorTree.SelectedNode;
            bool programmatic = node.Tag is IObject tobj && tobj.ConstructedProgrammatically;

            btnMoveUp.Visible = btnMoveDown.Visible = node?.Tag is IComponent;
            btnMoveDown.Enabled = btnMoveAsChildToSibNext.Enabled = node?.NextNode != null && !programmatic;
            btnMoveUp.Enabled = btnMoveAsChildToSibPrev.Enabled = node?.PrevNode != null && !programmatic;

            btnMoveAsSibToParent.Visible =
            btnMoveAsChildToSibNext.Visible =
            btnMoveAsChildToSibPrev.Visible = 
            btnNewSiblingSceneComp.Visible =
            btnNewChildSceneComp.Visible =
            node?.Tag is ISceneComponent;

            splt1.Visible = btnMoveUp.Visible || btnMoveDown.Visible || btnMoveAsSibToParent.Visible || btnMoveAsChildToSibNext.Visible || btnMoveAsChildToSibPrev.Visible || btnNewSiblingSceneComp.Visible;

            if (node == null)
            {
                btnNewLogicComp.Visible = false;
                btnRemove.Enabled = false;
                return;
            }

            btnRemove.Enabled = !programmatic;
            switch (node.Tag)
            {
                case IMap map:
                    btnNewLogicComp.Visible = false;
                    btnNewMap.Visible = true;
                    btnNewActor.Visible = true;
                    break;
                case IActor actor:
                    btnNewMap.Visible = false;
                    btnNewActor.Visible = true;
                    btnNewLogicComp.Visible = true;
                    break;
                case ISceneComponent sceneComp:
                    ISceneComponent sceneCompParent = sceneComp.ParentSocket as ISceneComponent;
                    bool parentIsSceneComp = sceneCompParent != null;
                    btnNewSiblingSceneComp.Enabled = parentIsSceneComp;
                    btnMoveAsSibToParent.Enabled = parentIsSceneComp && sceneCompParent.ParentSocket is ISceneComponent;
                    btnNewMap.Visible = false;
                    btnNewActor.Visible = true;
                    btnNewLogicComp.Visible = false;
                    break;
                case ILogicComponent logicComp:
                    btnNewMap.Visible = false;
                    btnNewActor.Visible = false;
                    btnNewLogicComp.Visible = true;
                    break;
            }
        }
        private void ctxActorTree_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            btnMoveUp.Visible =
            btnMoveDown.Visible =
            btnNewLogicComp.Visible =
            btnMoveAsSibToParent.Visible =
            btnMoveAsChildToSibNext.Visible =
            btnMoveAsChildToSibPrev.Visible =
            btnNewSiblingSceneComp.Visible =
            btnNewChildSceneComp.Visible =
            splt1.Visible =
            true;

            btnNewSiblingSceneComp.Enabled =
            btnMoveAsChildToSibNext.Enabled =
            btnMoveAsChildToSibPrev.Enabled =
            btnRemove.Enabled =
            btnMoveDown.Enabled =
            btnMoveUp.Enabled =
            true;
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (ActorTree.SelectedNode != null && !ActorTree.SelectedNode.IsEditing)
                ActorTree.SelectedNode.BeginEdit();
        }

        private void btnNewActor_Click(object sender, EventArgs e)
        {
            if (Engine.World?.Settings == null)
                return;

            TreeNode node = ActorTree.SelectedNode;
            IMap targetMap = null;

            switch (node.Tag)
            {
                case IMap map:
                    targetMap = map;
                    break;
                case IActor actor:
                    targetMap = actor.MapAttachment;
                    break;
                case ISceneComponent comp:

                    if (!(Editor.UserCreateInstanceOf<ISubActorComponent>() is ISceneComponent subActorComp))
                        return;

                    comp.ChildComponents.Add(subActorComp);
                    //targetMap = comp.OwningActor?.MapAttachment;

                    return;
            }

            if (targetMap == null)
                targetMap = Engine.World.Settings.FindOrCreateMap(Engine.World.Settings.NewActorTargetMapName);

            IActor newActor = Editor.UserCreateInstanceOf<IActor>();
            if (newActor == null)
                return;

            targetMap.Actors.Add(newActor);
            Engine.World.SpawnActor(newActor);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            TreeNode node = ActorTree.SelectedNode;
            if (node?.Tag == null)
                return;
            switch (node.Tag)
            {
                case IMap map:
                    {
                        var maps = map.OwningWorld.Settings.Maps;
                        var matches = maps.Where(x => x.Value.IsLoaded && x.Value.File == map);
                        foreach (var match in matches)
                        {
                            match.Value?.File?.EndPlay();
                            maps.Remove(match.Key);
                        }
                    }
                    break;
                case IActor actor:
                    {
                        var map = actor.MapAttachment;
                        actor?.Despawn();
                        map?.Actors?.Remove(actor);
                    }
                    break;
                case ISceneComponent sceneComp:
                    sceneComp.ParentSocket?.ChildComponents?.Remove(sceneComp);
                    break;
                case ILogicComponent logicComp:
                    logicComp.OwningActor?.LogicComponents?.Remove(logicComp);
                    break;
            }
            node.Remove();
        }

        private void btnNewMap_Click(object sender, EventArgs e)
        {
            var map = Engine.World.Settings.FindOrCreateMap("Map" + Engine.World.Settings.Maps.Count);
            TreeNode node = CacheMap(map);
            ActorTree.LabelEdit = true;
            node.BeginEdit();
        }
        private void btnNewChildSceneComp_Click(object sender, EventArgs e)
        {
            var node = ActorTree.SelectedNode;
            if (node.Tag is ISceneComponent comp)
            {
                ISceneComponent newComp = Editor.UserCreateInstanceOf<ISceneComponent>();
                if (newComp != null)
                    comp.ChildComponents.Add(newComp);
            }
        }
        private void btnAddLogicComp_Click(object sender, EventArgs e)
        {
            var node = ActorTree.SelectedNode;
            switch (node.Tag)
            {
                case IActor actor:
                    {
                        ILogicComponent newComp = Editor.UserCreateInstanceOf<ILogicComponent>();
                        if (newComp != null)
                            actor.LogicComponents.Add(newComp);
                    }
                    break;
                case ILogicComponent logicComp:
                    {
                        ILogicComponent newComp = Editor.UserCreateInstanceOf<ILogicComponent>();
                        if (newComp != null)
                            logicComp.OwningActor.LogicComponents.Add(newComp);
                    }
                    break;
            }
        }
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            var node = ActorTree.SelectedNode;
            switch (node.Tag)
            {
                case SceneComponent sceneComp:
                    {
                        var socket = sceneComp.ParentSocket;
                        int index = socket?.ChildComponents?.IndexOf(sceneComp) ?? -1;
                        if (index > 0)
                        {
                            socket.ChildComponents.RemoveAt(index);
                            socket.ChildComponents.Insert(index - 1, sceneComp);
                        }
                        node.MoveUp();
                    }
                    break;
                case LogicComponent logicComp:
                    {
                        var comps = logicComp.OwningActor?.LogicComponents;
                        int index = comps?.IndexOf(logicComp) ?? -1;
                        if (index > 0)
                        {
                            comps.RemoveAt(index);
                            comps.Insert(index - 1, logicComp);
                        }
                        node.MoveUp();
                    }
                    break;
            }
        }
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            var node = ActorTree.SelectedNode;
            switch (node.Tag)
            {
                case SceneComponent sceneComp:
                    {
                        var socket = sceneComp.ParentSocket;
                        int index = socket.ChildComponents.IndexOf(sceneComp);
                        if (index >= 0 && index + 1 < socket.ChildComponents.Count)
                        {
                            socket.ChildComponents.RemoveAt(index);
                            socket.ChildComponents.Insert(index + 1, sceneComp);
                        }
                    }
                    break;
                case LogicComponent logicComp:
                    {
                        var comps = logicComp.OwningActor?.LogicComponents;
                        int index = comps.IndexOf(logicComp);
                        if (index >= 0 && index + 1 < comps.Count)
                        {
                            comps.RemoveAt(index);
                            comps.Insert(index + 1, logicComp);
                        }
                    }
                    break;
            }
        }
        private void btnMoveAsSibToParent_Click(object sender, EventArgs e)
        {

        }
        private void btnMoveAsChildToSibPrev_Click(object sender, EventArgs e)
        {

        }
        private void btnMoveAsChildToSibNext_Click(object sender, EventArgs e)
        {

        }

        private void ActorTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            TreeNode node = ActorTree.SelectedNode;
            if (node?.Tag is IObject obj)
                obj.Name = e.Label;
        }
    }
}
