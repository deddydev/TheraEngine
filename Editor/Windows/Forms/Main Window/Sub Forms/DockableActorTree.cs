using AppDomainToolkit;
using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Components;
using TheraEngine.Components.Scene;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;
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
            ctxActorTree.Renderer = new TheraForm.TheraToolStripRenderer();

            Engine.Instance.DomainProxyDestroying += Instance_DomainProxyUnset;
        }

        private void Instance_DomainProxyUnset(EngineDomainProxy obj)
        {
            ActorTree.Nodes.Clear();
            EditorHUD = null;
        }

        public EditorUI3D EditorHUD
        {
            get => _editorHUD;
            set
            {
                _editorHUD = value;
                AppDomainHelper.Sponsor(_editorHUD);
            }
        }

        private readonly Dictionary<Guid, TreeNode> _mapTreeNodes = new Dictionary<Guid, TreeNode>();
        private TreeNode _dynamicActorsMapNode = null;
        private EditorUI3D _editorHUD;

        private void ActorTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (IsSuspended || !Editor.Instance.PropertyGridFormActive)
                return;

            if (ActorTree.SelectedNode is null)
            {
                EditorHUD?.SetSelectedComponent(false, null, true);
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = Editor.DomainProxy?.World?.Settings;
            }
            else
                switch (e.Node.Tag)
                {
                    case IWorld world:
                        {
                            Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = world;
                            break;
                        }
                    case IActor actor:
                        {
                            actor.EditorState.Selected = true;
                            EditorHUD?.SetSelectedComponent(false, actor.RootComponent, true);
                            Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = actor;
                            break;
                        }
                    case TheraEngine.Components.IComponent component:
                        {
                            component.EditorState.Selected = true;
                            if (component is ISceneComponent sceneComp)
                                EditorHUD?.SetSelectedComponent(false, sceneComp, true);
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
        public void UnlinkWorld()
        {
            bool worldExists = World != null;
            if (worldExists)
            {
                World.PropertyChanged -= World_PropertyChanged;
                UnlinkSettings();

                var actors = World.State.SpawnedActors;
                actors.ForEach(ActorDespawned);
                actors.PostAnythingAdded -= ActorSpawned;
                actors.PostAnythingRemoved -= ActorDespawned;

                World.PreBeginPlay -= World_PreBeginPlay;
                World.PostBeginPlay -= World_PostBeginPlay;
                World.PreEndPlay -= World_PreEndPlay;
                World.PostEndPlay -= World_PostEndPlay;
            }
            Clear();
        }

        private IWorld World { get; set; }
        public void LinkWorld(IWorld world)
        {
            if (InvokeRequired)
            {
                Invoke((Action<IWorld>)LinkWorld, world);
                return;
            }

            Clear();

            World = world;
            if (World is null || Engine.ShuttingDown)
                return;

            TreeNode node = new TreeNode(World.Name ?? "World") { Tag = World };
            ActorTree.Nodes.Add(node);

            World.PropertyChanged += World_PropertyChanged;
            LinkSettings(World.Settings);

            var actors = World.State.SpawnedActors;
            
            //ActorTree.SuspendLayout();
            //actors.ForEach(ActorSpawned);
            //ActorTree.ResumeLayout();

            actors.PostAnythingAdded += ActorSpawned;
            actors.PostAnythingRemoved += ActorDespawned;

            World.PreBeginPlay += World_PreBeginPlay;
            World.PostBeginPlay += World_PostBeginPlay;
            World.PreEndPlay += World_PreEndPlay;
            World.PostEndPlay += World_PostEndPlay;
        }

        private bool IsSuspended { get; set; } = false;
        private void World_PreEndPlay()
        {
            Engine.Out("ActorTreeForm : Suspending layout.");
            ActorTree.SuspendLayout();
            IsSuspended = true;
        }
        private void World_PostEndPlay()
        {
            Engine.Out("ActorTreeForm : Resuming layout.");
            ActorTree.ResumeLayout(true);
            IsSuspended = false;
        }
        private void World_PreBeginPlay()
        {
            Engine.Out("ActorTreeForm : Suspending layout.");
            ActorTree.SuspendLayout();
            IsSuspended = true;
        }
        private void World_PostBeginPlay()
        {
            Engine.Out("ActorTreeForm : Resuming layout.");
            ActorTree.ResumeLayout(true);
            IsSuspended = false;
        }

        private void MapRefRemoved(string key, LocalFileRef<IMap> value)
        {
            if (value is null)
                return;

            if (value.IsLoaded)
                MapUnloaded(value.File);

            Engine.Out("ActorTreeForm : Map ref removed.");

            value.Loaded -= MapLoaded;
            value.Unloaded -= MapUnloaded;
        }
        private void MapRefAdded(string key, LocalFileRef<IMap> value)
        {
            if (value is null)
                return;

            Engine.Out("ActorTreeForm : Map ref added.");

            value.Loaded += MapLoaded;
            value.Unloaded += MapUnloaded;
        }

        private void MapUnloaded(IMap map)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IMap>(MapUnloaded), map);
                return;
            }
            Engine.Out("ActorTreeForm : Map unloaded.");
            FindAndRemoveMapNode(map);
        }
        private void MapLoaded(IMap map)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IMap>(MapLoaded), map);
                return;
            }
            Engine.Out("ActorTreeForm : Map loaded.");
            FindOrCreateMapNode(map);
        }

        private void World_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is IWorld world))
                return;

            if (e.PropertyName.EqualsInvariant(nameof(IWorld.Settings)))
            {
                Engine.Out("ActorTreeForm : World settings property changed.");
                LinkSettings(world.Settings);
            }
        }
        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is IWorldSettings settings))
                return;

            if (e.PropertyName.EqualsInvariant(nameof(IWorldSettings.Maps)))
            {
                Engine.Out("ActorTreeForm : Maps property changed.");
                LinkMapsList(settings.Maps);
            }
        }

        private WorldSettings CurrentSettings { get; set; }
        private void LinkSettings(WorldSettings settings)
        {
            UnlinkSettings();

            Engine.Out("ActorTreeForm : Linking settings.");

            CurrentSettings = settings;
            if (CurrentSettings is null)
                return;

            AppDomainHelper.Sponsor(CurrentSettings);

            var maps = CurrentSettings.Maps;
            LinkMapsList(maps);
            CurrentSettings.PropertyChanged += Settings_PropertyChanged;
        }
        private void UnlinkSettings()
        {
            if (CurrentSettings is null)
                return;

            Engine.Out("ActorTreeForm : Unlinking settings.");

            CurrentSettings.PropertyChanged -= Settings_PropertyChanged;
            UnlinkMapsList();

            CurrentSettings = null;
        }

        private EventDictionary<string, LocalFileRef<IMap>> Maps { get; set; }
        private void UnlinkMapsList()
        {
            if (Maps is null)
                return;

            Engine.Out($"ActorTreeForm : Unlinking maps list, {Maps.Count} maps.");

            ActorTree.SuspendLayout();
            Maps.ForEach(x => MapRefRemoved(x.Key, x.Value));
            ActorTree.ResumeLayout();

            Maps.Added -= MapRefAdded;
            Maps.Removed -= MapRefRemoved;
            Maps = null;
        }
        private void LinkMapsList(EventDictionary<string, LocalFileRef<IMap>> maps)
        {
            UnlinkMapsList();

            Maps = maps;
            if (Maps is null)
                return;

            AppDomainHelper.Sponsor(Maps);

            Engine.Out($"ActorTreeForm : Linking maps list, {Maps.Count} maps.");

            ActorTree.SuspendLayout();
            Maps.ForEach(x => MapRefAdded(x.Key, x.Value));
            ActorTree.ResumeLayout();

            Maps.Added += MapRefAdded;
            Maps.Removed += MapRefRemoved;
        }

        private void FindAndRemoveMapNode(IMap map)
        {
            if (ActorTree.Nodes.Count == 0 || map is null)
                return;

            var worldNode = ActorTree.Nodes[0];
            if (worldNode is null)
                return;

            Engine.Out("ActorTreeForm : Attempting to remove map node.");
            if (_mapTreeNodes.ContainsKey(map.Guid))
            {
                TreeNode mapNode = _mapTreeNodes[map.Guid];
                worldNode.Nodes.Remove(mapNode);
                _mapTreeNodes.Remove(map.Guid);
                map.Renamed -= MapRenamed;

                Engine.Out("ActorTreeForm : Successfully removed map node.");
            }
        }
        internal TreeNode FindOrCreateMapNode(IMap map)
        {
            if (ActorTree.Nodes.Count == 0)
                return null;

            var worldNode = ActorTree.Nodes[0];
            if (worldNode is null)
                return null;

            TreeNode mapNode;
            if (map is null)
            {
                if (_dynamicActorsMapNode is null)
                {
                    _dynamicActorsMapNode = new TreeNode("Dynamic Actors");
                    worldNode.Nodes.Insert(0, _dynamicActorsMapNode);
                }
                mapNode = _dynamicActorsMapNode;
            }
            else if (!_mapTreeNodes.ContainsKey(map.Guid))
            {
                Engine.Out("ActorTreeForm : Creating map node.");

                AppDomainHelper.Sponsor(map);
                mapNode = new TreeNode(map.Name) { Tag = map };
                worldNode.Nodes.Add(mapNode);
                _mapTreeNodes.Add(map.Guid, mapNode);
                map.Renamed += MapRenamed;
                OnDisplayMapNode(map, mapNode);
            }
            else
                mapNode = _mapTreeNodes[map.Guid];

            return mapNode;
        }

        private void MapRenamed(TObject node, string oldName)
        {
            if (_mapTreeNodes.ContainsKey(node.Guid))
            {
                var treeNode = _mapTreeNodes[node.Guid];
                if (treeNode != null)
                    treeNode.Text = node.Name;
            }
        }

        private void OnDisplayMapNode(IMap map, TreeNode node)
        {
            var actors = World?.State?.SpawnedActors;
            if (actors is null)
                return;

            var mapActors = actors.Where(x => x.MapAttachment == map).ToArray();
            if (mapActors.Length == 0)
                return;

            ActorsSpawned(mapActors);
        }

        private void ActorsSpawned(IEnumerable<IActor> actors)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IEnumerable<IActor>>(ActorsSpawned), actors);
                return;
            }

            Engine.Out("ActorTreeForm : Spawning multiple actors.");

            ActorTree.SuspendLayout();
            actors.ForEach(ActorSpawned);
            ActorTree.ResumeLayout();
        }

        internal void ActorSpawned(IActor actor)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IActor>(ActorSpawned), actor);
                return;
            }

            if (actor?.OwningWorld is null || Engine.ShuttingDown)
                return;

            if (actor.HasEditorState && !actor.EditorState.DisplayInActorTree)
                return;

            AppDomainHelper.Sponsor(actor);
            IMap map = actor.MapAttachment;
            TreeNode mapNode = FindOrCreateMapNode(map);

            TreeNode node = new TreeNode(actor.ToString()) { Tag = actor };

            //Add node for the root scene component
            node.Nodes.Add(new TreeNode());

            actor.EditorState.TreeNode = node;
            mapNode.Nodes.Add(node);
            node.EnsureVisible();

            Editor.Instance.Item_LogicComponentsChanged(actor);
            Editor.Instance.Item_SceneComponentCacheRegenerated(actor);

            actor.SceneComponentCacheRegenerated += Editor.Instance.Item_SceneComponentCacheRegenerated;
            actor.LogicComponentsChanged += Editor.Instance.Item_LogicComponentsChanged;

            //Engine.PrintLine("ActorTreeForm : Spawned actor.");
        }
        internal void ActorDespawned(IActor actor)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IActor>(ActorDespawned), actor);
                return;
            }

            if (actor is null || Engine.ShuttingDown)
                return;

            if (actor.HasEditorState)
            {
                actor.EditorState.TreeNode?.Remove();
                actor.EditorState.TreeNode = null;
            }

            actor.SceneComponentCacheRegenerated -= Editor.Instance.Item_SceneComponentCacheRegenerated;
            actor.LogicComponentsChanged -= Editor.Instance.Item_LogicComponentsChanged;
        }

        public void Clear()
        {
            if (InvokeRequired)
            {
                Invoke((Action)Clear);
                return;
            }
            _dynamicActorsMapNode = null;
            _mapTreeNodes.Clear();
            ActorTree.Nodes.Clear();
        }

        private void ActorTree_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode node = ActorTree.GetNodeAt(e.Location);
            if (node?.TreeView != null)
                node.TreeView.SelectedNode = node;
        }

        private void ctxActorTree_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = ActorTree.SelectedNode;
            bool programmatic = !(node?.Tag is IObject tobj) || tobj.ConstructedProgrammatically;

            btnMoveUp.Visible = btnMoveDown.Visible = node?.Tag is TheraEngine.Components.IComponent;
            btnMoveDown.Enabled = btnMoveAsChildToSibNext.Enabled = node?.NextNode != null && !programmatic;
            btnMoveUp.Enabled = btnMoveAsChildToSibPrev.Enabled = node?.PrevNode != null && !programmatic;

            btnMoveAsSibToParent.Visible =
            btnMoveAsChildToSibNext.Visible =
            btnMoveAsChildToSibPrev.Visible =
            btnNewSiblingSceneComp.Visible =
            btnNewChildSceneComp.Visible =
            btnInsertNewParentSceneComp.Visible = 
            node?.Tag is ISceneComponent;

            splt1.Visible = btnMoveUp.Visible || btnMoveDown.Visible || btnMoveAsSibToParent.Visible || btnMoveAsChildToSibNext.Visible || btnMoveAsChildToSibPrev.Visible || btnNewSiblingSceneComp.Visible;

            if (node is null)
            {
                btnNewLogicComp.Visible = false;
                btnRemove.Enabled = false;
                return;
            }

            btnRemove.Enabled = !programmatic;
            switch (node.Tag)
            {
                case IWorld world:

                    break;
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

                    btnInsertNewParentSceneComp.Enabled = parentIsSceneComp;
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
            btnInsertNewParentSceneComp.Visible =
            splt1.Visible =

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
            var world = Editor.DomainProxy?.World;
            var settings = world?.Settings;
            if (settings is null)
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

                    comp.ChildSockets.Add(subActorComp);
                    //targetMap = comp.OwningActor?.MapAttachment;

                    return;
            }

            if (targetMap is null)
                targetMap = settings.FindOrCreateMap(settings.NewActorTargetMapName);

            IActor newActor = Editor.DomainProxy.UserCreateInstanceOf<IActor>();
            if (newActor is null)
                return;

            targetMap.Actors.Add(newActor);
            world.SpawnActor(newActor);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            TreeNode node = ActorTree.SelectedNode;
            if (node?.Tag is null)
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
                    sceneComp.ParentSocket?.ChildSockets?.Remove(sceneComp);
                    break;
                case ILogicComponent logicComp:
                    logicComp.OwningActor?.LogicComponents?.Remove(logicComp);
                    break;
            }
            node.Remove();
        }

        private void btnNewMap_Click(object sender, EventArgs e)
        {
            var settings = Editor.DomainProxy?.World?.Settings;
            if (settings is null)
            {
                Engine.LogWarning("Unable to create new map, world settings is null.");
                return;
            }

            var map = settings.FindOrCreateMap("Map" + settings.Maps.Count);
            TreeNode node = FindOrCreateMapNode(map);
            ActorTree.LabelEdit = true;
            node.BeginEdit();
        }
        private void btnInsertNewParentSceneComp_Click(object sender, EventArgs e)
        {
            var node = ActorTree.SelectedNode;
            if (node.Tag is ISceneComponent comp && comp.ParentSocket != null)
            {
                RemoteAction.Invoke(AppDomainHelper.GameAppDomain, comp, (comp2) =>
                {
                    ISceneComponent newComp = Editor.UserCreateInstanceOf<ISceneComponent>(true, Editor.Instance);
                    if (newComp != null)
                    {
                        var parent = comp2.ParentSocket;
                        comp2.ParentSocket = null;
                        parent.ChildSockets.Add(newComp);
                        newComp.ChildSockets.Add(comp2);
                    }
                });
            }
        }
        private void btnNewChildSceneComp_Click(object sender, EventArgs e)
        {
            var node = ActorTree.SelectedNode;
            if (node.Tag is ISceneComponent comp)
                Editor.DomainProxy.NewSceneComponentChild(comp);
        }
        private void btnAddLogicComp_Click(object sender, EventArgs e)
        {
            var node = ActorTree.SelectedNode;
            switch (node.Tag)
            {
                case IActor actor:
                    {
                        ILogicComponent newComp = Editor.DomainProxy.UserCreateInstanceOf<ILogicComponent>();
                        if (newComp != null)
                            actor.LogicComponents.Add(newComp);
                    }
                    break;
                case ILogicComponent logicComp:
                    {
                        ILogicComponent newComp = Editor.DomainProxy.UserCreateInstanceOf<ILogicComponent>();
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
                        int index = socket?.ChildSockets?.IndexOf(sceneComp) ?? -1;
                        if (index > 0)
                        {
                            socket.ChildSockets.RemoveAt(index);
                            socket.ChildSockets.Insert(index - 1, sceneComp);
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
                        int index = socket.ChildSockets.IndexOf(sceneComp);
                        if (index >= 0 && index + 1 < socket.ChildSockets.Count)
                        {
                            socket.ChildSockets.RemoveAt(index);
                            socket.ChildSockets.Insert(index + 1, sceneComp);
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
