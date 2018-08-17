using System.Windows.Forms;
using TheraEngine.Core.Files.XML;
using TheraEngine.ThirdParty;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMSBuildTree : DockContent
    {
        public DockableMSBuildTree()
        {
            InitializeComponent();
            theraPropertyGrid1.PropertiesLoaded += TheraPropertyGrid1_PropertiesLoaded;
        }

        private void TheraPropertyGrid1_PropertiesLoaded(object obj)
        {
            theraPropertyGrid1.ExpandAll();
        }

        public void SetProject(MSBuild.Project project)
        {
            buildTree.Nodes.Clear();
            RecursivePopulate(buildTree.Nodes, project);
        }
        private void RecursivePopulate(TreeNodeCollection collection, IElement element)
        {
            TreeNode node = new TreeNode(element.ElementName) { Tag = element };
            collection.Add(node);
            
            foreach (var elem in element.ChildElementsInOrder())
                RecursivePopulate(node.Nodes, elem);
        }

        private void buildTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            theraPropertyGrid1.TargetObject = buildTree.SelectedNode.Tag;
        }
    }
}
