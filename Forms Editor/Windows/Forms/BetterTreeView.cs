using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public class BetterTreeView : BetterTreeView<TreeNode>
    {

    }
    public class BetterTreeView<T> : TreeView where T : TreeNode
    {

    }
}
