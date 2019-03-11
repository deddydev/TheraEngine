using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Files;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableActorTree : DockContent
    {
        public DockableActorTree()
        {
            InitializeComponent();
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
            
            Engine.World.Settings.Maps.ForEach(x => CacheMap(x));
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

        }

        private void btnRename_Click(object sender, EventArgs e)
        {

        }

        private void btnAddActor_Click(object sender, EventArgs e)
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
                case IActor actor:
                    targetMap = actor.MapAttachment;
                    break;
                case IComponent comp:
                    targetMap = comp.OwningActor?.MapAttachment;
                    break;
            }

            if (targetMap == null)
                targetMap = Engine.World.Settings.FindOrCreateMap();

            BaseActor newActor = Editor.UserCreateInstanceOf<BaseActor>();
            if (newActor == null)
                return;

            targetMap.Actors.Add(newActor);
            Engine.World.SpawnActor(newActor);
        }

        private void btnDeleteActor_Click(object sender, EventArgs e)
        {

        }

        private void btnAddMap_Click(object sender, EventArgs e)
        {

        }

        private void btnRemoveMap_Click(object sender, EventArgs e)
        {

        }

        private void btnAddSceneComp_Click(object sender, EventArgs e)
        {

        }

        private void btnRemoveSceneComp_Click(object sender, EventArgs e)
        {

        }

        private void btnAddLogicComp_Click(object sender, EventArgs e)
        {

        }

        private void btnRemoveLogicComp_Click(object sender, EventArgs e)
        {

        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {

        }
    }
}
