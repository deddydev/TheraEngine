using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TheraEngine.Files;
using TheraEngine.Rendering.Models;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableBoneTree : DockContent
    {
        public DockableBoneTree()
        {
            InitializeComponent();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void renameAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pnlRenameAll.Visible = !pnlRenameAll.Visible;
        }

        private void NodeTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            BoneTreeNode b = NodeTree.SelectedNode as BoneTreeNode;
            theraPropertyGrid1.TargetFileObject = b is BoneNode bn ? (IFileObject)bn.Bone : b is MeshSocketNode mn ? (IFileObject)mn.Socket : null;
        }

        public void SetSkeleton(Skeleton skel)
        {
            _skeleton = skel;
            NodeTree.DisplayNodes(skel);
        }

        private Skeleton _skeleton;
        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlRenameAll.Visible = false;
        }
        
        private void btnOkay_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text;
            var keys = _skeleton.BoneNameCache.Keys;
            string[] keyStrs = keys.ToArray();
            int i;
            switch (_searchMethod)
            {
                case ESearchingMethod.Regex:
                    Regex regex = new Regex(searchTerm);
                    var matches = keyStrs.Select(x => regex.Match(x));
                    i = -1;
                    foreach (var match in matches)
                    {
                        ++i;
                        if (!match.Success)
                            continue;
                        Bone bone = _skeleton.BoneNameCache[keyStrs[i]];
                        RenameBone(bone, match.Index, match.Length);
                    }
                    break;
                case ESearchingMethod.Contains:
                    var containing = keyStrs.Select(x => x.IndexOf(searchTerm));
                    i = -1;
                    foreach (int match in containing)
                    {
                        ++i;
                        if (match < 0)
                            continue;
                        Bone bone = _skeleton.BoneNameCache[keyStrs[i]];
                        RenameBone(bone, match, searchTerm.Length);
                    }
                    break;
                case ESearchingMethod.StartsWith:
                    var starting = keyStrs.Select(x => x.StartsWith(searchTerm));
                    i = -1;
                    foreach (int match in starting)
                    {
                        ++i;
                        if (match < 0)
                            continue;
                        Bone bone = _skeleton.BoneNameCache[keyStrs[i]];
                        RenameBone(bone, match, searchTerm.Length);
                    }
                    break;
                case ESearchingMethod.EndsWith:
                    var containing = keyStrs.Select(x => x.IndexOf(searchTerm));
                    i = -1;
                    foreach (int match in containing)
                    {
                        ++i;
                        if (match < 0)
                            continue;
                        Bone bone = _skeleton.BoneNameCache[keyStrs[i]];
                        RenameBone(bone, match, searchTerm.Length);
                    }
                    break;
            }
            pnlRenameAll.Visible = false;
        }
        private void RenameBone(Bone bone, int searchMatchStart, int searchMatchLength)
        {
            switch (_renameMethod)
            {
                case ERenamingMethod.ReplaceFullName:
                    bone.Name = txtRename.Text;
                    break;
                case ERenamingMethod.ReplaceSearchMatch:

                    break;
                case ERenamingMethod.AppendToStart:

                    break;
                case ERenamingMethod.AppendToEnd:

                    break;
                case ERenamingMethod.AppendBeforeSearchMatch:

                    break;
                case ERenamingMethod.AppendAfterSearchMatch:

                    break;
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

        private void btnViewFlat_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnViewAsTree_CheckedChanged(object sender, EventArgs e)
        {

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
        private ESearchingMethod _searchMethod;
        private ERenamingMethod _renameMethod;
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
        }
    }
}
