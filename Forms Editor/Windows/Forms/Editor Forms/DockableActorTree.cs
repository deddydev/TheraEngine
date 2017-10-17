using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Editor.Instance.PropertyGridForm.theraPropertyGrid1.TargetObject = e.Node.Tag;
        }
    }
}
