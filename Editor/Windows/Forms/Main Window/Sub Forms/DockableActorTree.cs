using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
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

        private Dictionary<Guid, TreeNode> _mapTreeNodes = new Dictionary<Guid, TreeNode>();
        private TreeNode _dynamicActorsMapNode = null;

        private void ActorTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (Editor.Instance.PropertyGridFormActive)
            {
                if (ActorTree.SelectedNode == null)
                {
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.Settings;
                }
                else if (e.Node.Tag is IActor actor)
                {
                    actor.EditorState.Selected = true;
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = actor;

                    if (Engine.LocalPlayers.Count > 0)
                    {
                        EditorHud hud = (EditorHud)Engine.LocalPlayers[0].ControlledPawn?.HUD;
                        hud?.SetSelectedComponent(false, actor.RootComponent);
                    }
                }
                else if (e.Node.Tag is Component component)
                {
                    component.EditorState.Selected = true;
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = component;

                    if (component is SceneComponent sceneComp && Engine.LocalPlayers.Count > 0)
                    {
                        EditorHud hud = (EditorHud)Engine.LocalPlayers[0].ControlledPawn?.HUD;
                        hud?.SetSelectedComponent(false, sceneComp);
                    }
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

            if (Engine.World == null || Engine.ShuttingDown)
                return;

            _dynamicActorsMapNode = null;
            _mapTreeNodes.Clear();
            ActorTree.Nodes.Clear();

            Engine.World.Settings.Maps.ForEach(x => CacheMap(x));
            Engine.World.State.SpawnedActors.ForEach(x => ActorSpawned(x));
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

        private void Item_SceneComponentCacheRegenerated(IActor actor)
        {
            TreeNode node = actor.EditorState.TreeNode;
            node.Nodes[0].Nodes.Clear();
            RecursiveAddSceneComp(node.Nodes[0], actor.RootComponent);
        }
        private void RecursiveAddSceneComp(TreeNode node, SceneComponent comp)
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

        private void newActorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IActor actor = Editor.UserCreateInstanceOf<IActor>(true);
            if (actor != null)
                Engine.World.SpawnActor(actor);
        }
    }
}
