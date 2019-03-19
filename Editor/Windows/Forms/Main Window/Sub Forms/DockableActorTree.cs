using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEditor.Core.Extensions;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Components;
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
                case Component component:
                {
                    component.EditorState.Selected = true;
                    EditorHUD?.SetSelectedComponent(false, component as SceneComponent, true);
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = component;
                    break;
                }
                case Map map:
                {
                    EditorHUD?.SetSelectedComponent(false, null, true);
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = map;
                    break;
                }
            }
        }
        internal void GenerateInitialActorList()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(GenerateInitialActorList));
                return;
            }

            ClearMaps();

            if (Engine.World == null || Engine.ShuttingDown)
                return;
            
            Engine.World.Settings.Maps.ForEach(x => CacheMap(x.Value));
            Engine.World.State.SpawnedActors.ForEach(ActorSpawned);
        }
        internal TreeNode CacheMap(Map map)
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

            if (Engine.World == null || Engine.ShuttingDown || item == null)
                return;

            Map map = item.MapAttachment;
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

            if (Engine.World == null || Engine.ShuttingDown || item == null)
                return;

            if (item?.EditorState?.TreeNode != null)
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

            btnMoveUp.Visible = btnMoveDown.Visible = node?.Tag is IComponent;
            btnMoveDown.Enabled = btnMoveAsChildToSibNext.Enabled = node?.NextNode != null;
            btnMoveUp.Enabled = btnMoveAsChildToSibPrev.Enabled = node?.PrevNode != null;

            btnMoveAsSibToParent.Visible =
            btnMoveAsChildToSibNext.Visible =
            btnMoveAsChildToSibPrev.Visible = 
            btnNewSiblingSceneComp.Visible =
            btnNewChildSceneComp.Visible =
            node?.Tag is SceneComponent;

            splt1.Visible = btnMoveUp.Visible || btnMoveDown.Visible || btnMoveAsSibToParent.Visible || btnMoveAsChildToSibNext.Visible || btnMoveAsChildToSibPrev.Visible || btnNewSiblingSceneComp.Visible;

            if (node == null)
            {
                btnNewLogicComp.Visible = false;
                btnRemove.Enabled = false;
                return;
            }

            switch (node.Tag)
            {
                case Map map:
                    btnNewLogicComp.Visible = false;
                    break;
                case BaseActor actor:

                    break;
                case SceneComponent sceneComp:
                    btnNewLogicComp.Visible = false;
                    btnNewSiblingSceneComp.Enabled = sceneComp.ParentSocket is SceneComponent;
                    break;
                case LogicComponent logicComp:
                    
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
            Map targetMap = null;

            switch (node.Tag)
            {
                case Map map:
                    targetMap = map;
                    break;
                case BaseActor actor:
                    targetMap = actor.MapAttachment;
                    break;
                case Component comp:
                    targetMap = comp.OwningActor?.MapAttachment;
                    break;
            }

            if (targetMap == null)
                targetMap = Engine.World.Settings.FindOrCreateMap(Engine.World.Settings.NewActorTargetMapName);

            BaseActor newActor = Editor.UserCreateInstanceOf<BaseActor>();
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
                case Map map:
                    {
                        var maps = map.OwningWorld.Settings.Maps;
                        var matches = maps.Where(x => x.Value.IsLoaded && x.Value.File == map);
                        foreach (var match in matches)
                            maps.Remove(match.Key);
                    }
                    break;
                case BaseActor actor:
                    {
                        var map = actor.MapAttachment;
                        map?.Actors?.Remove(actor);
                    }
                    break;
                case SceneComponent sceneComp:
                    {
                        var socket = sceneComp.ParentSocket;
                        socket?.ChildComponents?.Remove(sceneComp);
                    }
                    break;
                case LogicComponent logicComp:
                    {
                        var actor = logicComp.OwningActor;
                        actor?.LogicComponents?.Remove(logicComp);
                    }
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
            if (node.Tag is SceneComponent comp)
            {
                SceneComponent newComp = Editor.UserCreateInstanceOf<SceneComponent>();
                if (newComp != null)
                    comp.ChildComponents.Add(newComp);
            }
        }
        private void btnAddLogicComp_Click(object sender, EventArgs e)
        {
            var node = ActorTree.SelectedNode;
            switch (node.Tag)
            {
                case BaseActor actor:
                    {
                        LogicComponent newComp = Editor.UserCreateInstanceOf<LogicComponent>();
                        if (newComp != null)
                            actor.LogicComponents.Add(newComp);
                    }
                    break;
                case LogicComponent logicComp:
                    {
                        LogicComponent newComp = Editor.UserCreateInstanceOf<LogicComponent>();
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
