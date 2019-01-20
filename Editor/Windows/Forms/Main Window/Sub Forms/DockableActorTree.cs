using System;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableActorTree : DockContent
    {
        public DockableActorTree()
        {
            InitializeComponent();
        }
        
        private void ActorTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (Editor.Instance.PropertyGridFormActive)
            {
                if (ActorTree.SelectedNode == null)
                {
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.Settings;
                }
                else
                {
                    IActor t = e.Node.Tag as IActor;
                    t.EditorState.Selected = true;
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = t;

                    if (Engine.LocalPlayers.Count > 0)
                    {
                        EditorHud hud = (EditorHud)Engine.LocalPlayers[0].ControlledPawn?.HUD;
                        hud?.SetSelectedComponent(false, t.RootComponent);
                    }
                }
            }
        }

        internal void ActorAdded(IActor item)
        {
            if (Engine.World != null && !Engine.ShuttingDown)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<IActor>(ActorAdded), item);
                    return;
                }
                TreeNode t = new TreeNode(item.ToString()) { Tag = item };
                item.EditorState.TreeNode = t;
                ActorTree.Nodes.Add(t);
            }
        }
        internal void GenerateInitialActorList()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(GenerateInitialActorList));
                return;
            }
            ActorTree.Nodes.Clear();
            if (Engine.World != null)
                ActorTree.Nodes.AddRange(Engine.World.State.SpawnedActors.
                    Select(x => x.EditorState.TreeNode = new TreeNode(x.ToString()) { Tag = x }).ToArray());
        }
        internal void ActorRemoved(IActor item)
        {
            if (Engine.World != null && !Engine.ShuttingDown)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<IActor>(ActorRemoved), item);
                    return;
                }

                if (item?.EditorState?.TreeNode != null)
                {
                    item.EditorState.TreeNode.Remove();
                    item.EditorState.TreeNode = null;
                }
            }
        }
    }
}
