using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Skill.FbxSDK;

namespace CubeCreator
{
    public partial class SceneForm : Form
    {
        int exportIndex = -1;
        SdkUtility utility = null;

        public SceneForm()
        {
            InitializeComponent();
            utility = new SdkUtility();
            if (utility.CreateScene())
                Fill_TreeView();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (utility != null)
            {
                if (utility != null)
                    utility.DestroySdkObjects();
                utility = null;
            }
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FrmAbout a = new FrmAbout())
            {
                a.ShowDialog(this);
            }
        }


        // used to add the rootNode name and start to add children nodes
        private void DisplayHierarchy()
        {
            FbxNode rootNode = utility.Scene.RootNode;
            TreeNode root = InsertTreeViewItem(rootNode.Name, treScene.TopNode);
            int num = rootNode.GetChildCount();
            for (int i = 0; i < num; i++)
            {
                FbxNode child = rootNode.GetChild(i);
                DisplayHierarchy_Recurse(child, root);
            }
        }

        // used to add a new treeview item
        TreeNode InsertTreeViewItem(string txt, TreeNode parent)
        {
            TreeNode t = new TreeNode();
            t.Text = txt; // caption
            if (parent != null)
                parent.Nodes.Add(t);
            else
                treScene.Nodes.Add(t);
            return t;
        }

        // used to recursively add children nodes
        void DisplayHierarchy_Recurse(FbxNode node, TreeNode parent)
        {
            //create a new Treeview item with node name and Attribute type name
            string s = SdkUtility.GetNodeNameAndAttributeTypeName(node);
            TreeNode t = InsertTreeViewItem(s, parent);

            // show some KFbxNode parameters
            Add_TreeViewItem_KFbxNode_Parameters(node, t);

            int num = node.GetChildCount();
            for (int i = 0; i < num; i++)
            {
                // recursively call this
                FbxNode child = node.GetChild(i);
                DisplayHierarchy_Recurse(child, t);
            }
        }

        // used to add KFbxNode parameters
        void Add_TreeViewItem_KFbxNode_Parameters(FbxNode node, TreeNode parent)
        {
            if (node == null) return;

            // show node default translation
            string s = SdkUtility.GetDefaultTranslationInfo(node);
            InsertTreeViewItem(s, parent);

            // show node visibility
            string s1 = utility.GetNodeInfo(node);
            InsertTreeViewItem(s1, parent);
        }
        // fill the treeview with the FBX scene content
        void Fill_TreeView()
        {
            treScene.Nodes.Clear();
            //load the FBX scene
            if (utility.Scene == null)
            {
                // dont forget to delete the SdkManager 
                // and all objects created by the SDK manager
                utility.DestroySdkObjects();
                return;
            }

            // display scene hierarchy
            DisplayHierarchy();

            // expand all items of the treeview
            treScene.ExpandAll();

            // dont forget to delete the SdkManager 
            // and all objects created by the SDK manager
            //utility.DestroySdkObjects();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog save = new SaveFileDialog())
            {
                save.AddExtension = true;
                save.Filter = SdkUtility.GetWriterSFNFilters();
                save.Title = "Select the file to save ... (use the file type filter)";
                save.FilterIndex = 1;
                save.ShowDialog(this);
                if (!string.IsNullOrEmpty(save.FileName))
                {
                    txtSave.Text = save.FileName;
                    exportIndex = save.FilterIndex;
                }
                else
                    exportIndex = -1;
            }
            if (exportIndex > 0)
            {
                utility.SaveScene(txtSave.Text, exportIndex - 1, true);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (utility == null)
                return;
            utility.CreateCube(chkMaterial.Checked, chkTexture.Checked, chkAnimate.Checked);
            Fill_TreeView();
        }
    }
}
