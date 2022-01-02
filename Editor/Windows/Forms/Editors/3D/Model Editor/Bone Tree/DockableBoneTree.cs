using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Models;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableBoneTree : DockContent
    {
        public DockableBoneTree()
        {
            InitializeComponent();
            NodeTree.TreeViewNodeSorter = new BoneComparer();
            NodeTree.DrawMode = TreeViewDrawMode.OwnerDrawText;
            NodeTree.DrawNode += NodeTree_DrawNode;
        }
        
        private void NodeTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            BoneTreeNode node = (BoneTreeNode)e.Node;
            Font nodeFont = node.NodeFont ?? tree.Font;
            if (node.IsSelected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Editor.TurquoiseColor), e.Bounds);
                ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, node.ForeColor, node.BackColor);
            }
            else
            {
                int index = node.HighlightIndex;
                int length = node.HighlightLength;
                length = Math.Min(length, node.Text.Length - index);
                if (index >= 0 && length > 0 && index < node.Text.Length)
                {
                    //e.Graphics.FillRectangle(new SolidBrush(tree.BackColor), e.Bounds);

                    CharacterRange[] characterRanges = { new CharacterRange(index, length) };
                    StringFormat stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Near,
                    };
                    stringFormat.SetMeasurableCharacterRanges(characterRanges);
                    Rectangle r = e.Bounds;
                    r.Location = new Point();
                    Region[] regions = e.Graphics.MeasureCharacterRanges(node.Text, nodeFont, e.Bounds, stringFormat);

                    foreach (Region region in regions)
                    {
                        Rectangle rect = Rectangle.Round(region.GetBounds(e.Graphics));
                        rect.X -= 3;
                        e.Graphics.FillRectangle(new SolidBrush(node.BackColor), rect);
                    }
                }
                else
                    e.Graphics.FillRectangle(new SolidBrush(node.BackColor), e.Bounds);
            }
            e.Graphics.DrawString(node.Text, nodeFont, new SolidBrush(node.ForeColor), Rectangle.Inflate(e.Bounds, 2, 0));
        }
        private class BoneComparer : IComparer<TreeNode>, IComparer
        {
            public int Compare(TreeNode x, TreeNode y)
            {
                bool xNull = x is null;
                bool yNull = y is null;
                if (xNull)
                {
                    if (yNull)
                        return 0;
                    else
                        return 1;
                }
                else if (yNull)
                    return -1;

                return x.Text.CompareTo(y.Text);
            }
            public int Compare(object x, object y)
                => Compare(x as TreeNode, y as TreeNode);
        }
        public void DisplayNodes(Skeleton skel)
        {
            NodeTree.Nodes.Clear();
            if (skel != null)
                foreach (Bone b in skel.RootBones)
                    WrapBone(b, NodeTree.Nodes);
        }
        public void DisplayNodes(MeshSocket[] sockets)
        {
            //Nodes.Clear();
            //foreach (Bone b in skel.RootBones)
            //    WrapBone(b, Nodes);
        }
        public void WrapBone(Bone b, TreeNodeCollection c)
        {
            BoneNode node = new BoneNode(b)
            {
                ForeColor = NodeTree.ForeColor,
                BackColor = NodeTree.BackColor,
            };
            c.Add(node);
            lstBonesFlat.Items.Add(b);
            foreach (Bone b2 in b.ChildBones)
                WrapBone(b2, node.Nodes);
        }
        public void WrapSocket(MeshSocket s, TreeNodeCollection c)
        {
            MeshSocketNode node = new MeshSocketNode(s);
            c.Add(node);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void renameAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pnlRenameAll.Visible = !pnlRenameAll.Visible;
            grpRenamingMethod.Visible = true;
        }

        private void NodeTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            BoneTreeNode b = NodeTree.SelectedNode as BoneTreeNode;
            theraPropertyGrid1.TargetObject = b is BoneNode bn ? (IFileObject)bn.Bone : b is MeshSocketNode mn ? (IFileObject)mn.Socket : null;
        }

        public void SetSkeleton(Skeleton skel)
        {
            _skeleton = skel;
            lstBonesFlat.Items.Clear();
            DisplayNodes(skel);
        }

        private Skeleton _skeleton;
        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlRenameAll.Visible = false;
            txtSearch.Text = null;
        }
        
        private void btnOkay_Click(object sender, EventArgs e)
        {
            if (grpRenamingMethod.Visible)
            {
                string searchTerm = txtSearch.Text;
                string[] boneNames = _skeleton.BoneNameCache.Keys.ToArray();
                int i;
                switch (_searchMethod)
                {
                    case ESearchingMethod.Regex:
                        RegexOptions options = RegexOptions.CultureInvariant;
                        if (chkIgnoreCase.Checked)
                            options |= RegexOptions.IgnoreCase;
                        Regex regex = new Regex(searchTerm, options);
                        var matches = boneNames.Select(x => regex.Match(x));
                        i = -1;
                        foreach (var match in matches)
                        {
                            ++i;
                            if (!match.Success)
                                continue;
                            IBone bone = _skeleton.BoneNameCache[boneNames[i]];
                            RenameBone(bone, match.Index, match.Length);
                        }
                        break;
                    case ESearchingMethod.Contains:
                        var containing = boneNames.Select(x => x.IndexOf(searchTerm,
                            chkIgnoreCase.Checked ?
                            StringComparison.InvariantCultureIgnoreCase :
                            StringComparison.InvariantCulture));
                        i = -1;
                        foreach (int match in containing)
                        {
                            ++i;
                            if (match < 0)
                                continue;
                            IBone bone = _skeleton.BoneNameCache[boneNames[i]];
                            RenameBone(bone, match, searchTerm.Length);
                        }
                        break;
                    case ESearchingMethod.StartsWith:
                        var starting = boneNames.Select(x => x.StartsWith(searchTerm,
                            chkIgnoreCase.Checked ?
                            StringComparison.InvariantCultureIgnoreCase :
                            StringComparison.InvariantCulture));
                        i = -1;
                        foreach (bool match in starting)
                        {
                            ++i;
                            if (!match)
                                continue;
                            IBone bone = _skeleton.BoneNameCache[boneNames[i]];
                            RenameBone(bone, 0, searchTerm.Length);
                        }
                        break;
                    case ESearchingMethod.EndsWith:
                        var ending = boneNames.Select(x => x.EndsWith(searchTerm,
                            chkIgnoreCase.Checked ?
                            StringComparison.InvariantCultureIgnoreCase :
                            StringComparison.InvariantCulture));
                        i = -1;
                        foreach (bool match in ending)
                        {
                            ++i;
                            if (!match)
                                continue;
                            IBone bone = _skeleton.BoneNameCache[boneNames[i]];
                            RenameBone(bone, bone.Name.Length - searchTerm.Length, searchTerm.Length);
                        }
                        break;
                }
                txtSearch.Text = null;
                _skeleton.RegenerateBoneCache();
            }
            pnlRenameAll.Visible = false;
        }
        private void RenameBone(IBone bone, int searchMatchStart, int searchMatchLength)
        {
            switch (_renameMethod)
            {
                case ERenamingMethod.ReplaceFullName:
                    bone.Name = txtRename.Text;
                    break;
                case ERenamingMethod.ReplaceSearchMatch:
                    bone.Name = bone.Name.Replace(bone.Name.Substring(searchMatchStart, searchMatchLength), txtRename.Text);
                    break;
                case ERenamingMethod.AppendToStart:
                    bone.Name = txtRename.Text + bone.Name;
                    break;
                case ERenamingMethod.AppendToEnd:
                    bone.Name += txtRename.Text;
                    break;
                case ERenamingMethod.AppendBeforeSearchMatch:
                    {
                        string term = bone.Name.Substring(searchMatchStart, searchMatchLength);
                        bone.Name = bone.Name.Replace(term, txtRename.Text + term);
                        break;
                    }
                case ERenamingMethod.AppendAfterSearchMatch:
                    {
                        string term = bone.Name.Substring(searchMatchStart, searchMatchLength);
                        bone.Name = bone.Name.Replace(term, term + txtRename.Text);
                        break;
                    }
            }
        }

        private void btnViewFlat_Click(object sender, EventArgs e)
        {
            btnViewFlat.Checked = !btnViewFlat.Checked;
            btnViewAsTree.Checked = !btnViewFlat.Checked;
            lstBonesFlat.Visible = btnViewFlat.Checked;
            NodeTree.Visible = !btnViewFlat.Checked;
        }

        private void btnViewAsTree_Click(object sender, EventArgs e)
        {
            btnViewAsTree.Checked = !btnViewAsTree.Checked;
            btnViewFlat.Checked = !btnViewAsTree.Checked;
            lstBonesFlat.Visible = !btnViewAsTree.Checked;
            NodeTree.Visible = btnViewAsTree.Checked;
        }

        private void btnSortByAppearance_Click(object sender, EventArgs e)
        {
            if (btnSortByAppearance.Checked)
                return;
            btnSortByAppearance.Checked = true;
            btnSortByDecendantLevel.Checked = false;
            btnSortAlphabetically.Checked = false;
        }

        private void btnSortByDecendantLevel_Click(object sender, EventArgs e)
        {
            if (btnSortByDecendantLevel.Checked)
                return;
            btnSortByAppearance.Checked = false;
            btnSortByDecendantLevel.Checked = true;
            btnSortAlphabetically.Checked = false;
        }

        private void btnSortAlphabetically_Click(object sender, EventArgs e)
        {
            if (btnSortAlphabetically.Checked)
                return;
            btnSortByAppearance.Checked = false;
            btnSortByDecendantLevel.Checked = false;
            btnSortAlphabetically.Checked = true;
        }

        private enum ESearchingMethod
        {
            Regex,
            Contains,
            StartsWith,
            EndsWith,
        }
        private enum ERenamingMethod
        {
            ReplaceFullName,
            ReplaceSearchMatch,
            AppendAfterSearchMatch,
            AppendBeforeSearchMatch,
            AppendToStart,
            AppendToEnd,
        }
        private ESearchingMethod _searchMethod = ESearchingMethod.Contains;
        private ERenamingMethod _renameMethod = ERenamingMethod.ReplaceSearchMatch;
        private void rdoAppendToStart_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton btn && btn.Checked)
            {
                switch (btn.Name)
                {
                    case nameof(rdoReplaceFullName):
                        _renameMethod = ERenamingMethod.ReplaceFullName;
                        break;
                    case nameof(rdoReplaceSearchTerm):
                        _renameMethod = ERenamingMethod.ReplaceSearchMatch;
                        break;
                    case nameof(rdoAppendAfterSearchTerm):
                        _renameMethod = ERenamingMethod.AppendAfterSearchMatch;
                        break;
                    case nameof(rdoAppendBeforeSearchTerm):
                        _renameMethod = ERenamingMethod.AppendBeforeSearchMatch;
                        break;
                    case nameof(rdoAppendToStart):
                        _renameMethod = ERenamingMethod.AppendToStart;
                        break;
                    case nameof(rdoAppendToEnd):
                        _renameMethod = ERenamingMethod.AppendToEnd;
                        break;
                }
            }
        }

        private void rdoContains_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton btn && btn.Checked)
            {
                switch (btn.Name)
                {
                    case nameof(rdoRegEx):
                        _searchMethod = ESearchingMethod.Regex;
                        break;
                    case nameof(rdoContains):
                        _searchMethod = ESearchingMethod.Contains;
                        break;
                    case nameof(rdoStartsWith):
                        _searchMethod = ESearchingMethod.StartsWith;
                        break;
                    case nameof(rdoEndsWith):
                        _searchMethod = ESearchingMethod.EndsWith;
                        break;
                }
            }
            FindRecursive(NodeTree.Nodes);
            NodeTree.Refresh();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pnlRenameAll.Visible = true;
            grpRenamingMethod.Visible = false;
        }

        private void resetSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
        }

        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            NodeTree.ExpandAll();
        }

        private void btnCloseAll_Click(object sender, EventArgs e)
        {
            NodeTree.CollapseAll();
        }

        private void btnExpandAllSelected_Click(object sender, EventArgs e)
        {
            NodeTree.SelectedNode.ExpandAll();
        }

        private void btnCloseAllSelected_Click(object sender, EventArgs e)
        {
            NodeTree.SelectedNode.Collapse(false);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            FindRecursive(NodeTree.Nodes);
            NodeTree.Refresh();
        }

        private void FindRecursive(TreeNodeCollection nodes, bool makeMatchesVisible = true)
        {
            foreach (BoneTreeNode tn in nodes)
            {
                if (IsMatch(tn.Text, out int index, out int length))
                {
                    tn.BackColor = Editor.TurquoiseColor;
                    tn.ForeColor = NodeTree.ForeColor;
                    tn.HighlightIndex = index;
                    tn.HighlightLength = length;
                    if (makeMatchesVisible)
                        tn.EnsureVisible();
                }
                else
                {
                    tn.BackColor = NodeTree.BackColor;
                    tn.ForeColor = NodeTree.ForeColor;
                    tn.HighlightIndex = -1;
                    tn.HighlightLength = -1;
                }

                FindRecursive(tn.Nodes);
            }
        }

        private bool IsMatch(string boneName, out int index, out int length)
        {
            if (boneName != null)
            {
                switch (_searchMethod)
                {
                    case ESearchingMethod.Regex:
                        RegexOptions options = RegexOptions.CultureInvariant;
                        if (chkIgnoreCase.Checked)
                            options |= RegexOptions.IgnoreCase;
                        Regex regex = new Regex(txtSearch.Text, options);
                        Match match = regex.Match(boneName);
                        index = match.Index;
                        length = match.Length;
                        return match.Success;

                    case ESearchingMethod.Contains:
                        index = boneName.IndexOf(txtSearch.Text,
                            chkIgnoreCase.Checked ?
                            StringComparison.InvariantCultureIgnoreCase :
                            StringComparison.InvariantCulture);
                        length = txtSearch.Text.Length;
                        return index >= 0;

                    case ESearchingMethod.StartsWith:
                        var starting = boneName.StartsWith(txtSearch.Text,
                            chkIgnoreCase.Checked ?
                            StringComparison.InvariantCultureIgnoreCase :
                            StringComparison.InvariantCulture);
                        index = 0;
                        length = txtSearch.Text.Length;
                        return starting;

                    case ESearchingMethod.EndsWith:
                        var ending = boneName.EndsWith(txtSearch.Text,
                            chkIgnoreCase.Checked ?
                            StringComparison.InvariantCultureIgnoreCase :
                            StringComparison.InvariantCulture);
                        index = boneName.Length - txtSearch.Text.Length;
                        length = txtSearch.Text.Length;
                        return ending;
                }
            }
            index = -1;
            length = -1;
            return false;
        }

        private void chkIgnoreCase_CheckedChanged(object sender, EventArgs e)
        {
            FindRecursive(NodeTree.Nodes);
            NodeTree.Refresh();
        }

        private void chkViewMeshSockets_Click(object sender, EventArgs e)
        {

        }
    }
}
