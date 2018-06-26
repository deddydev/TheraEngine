using System.Linq;
using System.Windows.Forms;
using TheraEngine.Core.Files.XML;
using TheraEngine.Rendering.Models;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMSBuildTree : DockContent
    {
        public DockableMSBuildTree()
        {
            InitializeComponent();
        }

        public void SetProject(MSBuild.Project project)
        {
            RecursivePopulate(buildTree.Nodes, project);
        }
        private void RecursivePopulate(TreeNodeCollection collection, IElement element)
        {
            TreeNode node = new TreeNode(element.ReadElementName) { Tag = element };
            collection.Add(node);
            
            foreach (var elem in element.ChildElementsInOrder())
            {
                RecursivePopulate(node.Nodes, elem);
            }
        }

        private void buildTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            theraPropertyGrid1.TargetObject = buildTree.SelectedNode.Tag;
            theraPropertyGrid1.ExpandAll();
        }
    }
}
