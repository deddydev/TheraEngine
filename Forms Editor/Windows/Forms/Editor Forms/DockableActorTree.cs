using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Worlds.Actors;
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
            if (ActorTree.SelectedNode == null)
            {
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = Engine.World.Settings;
            }
            else
            {
                IActor t = e.Node.Tag as IActor;
                t.EditorState.Selected = true;
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = e.Node.Tag;

                EditorHud hud = (EditorHud)Engine.LocalPlayers[0].ControlledPawn.HUD;
                hud.SelectedComponent = t.RootComponent;
            }
        }
    }
}
