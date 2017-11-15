using System.Windows.Forms;
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
            Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = e.Node.Tag;
        }
    }
}
