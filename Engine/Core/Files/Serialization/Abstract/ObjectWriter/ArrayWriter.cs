using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(Array))]
    public class ArrayWriter : BaseObjectWriter
    {
        public Array Array { get; private set; }
        public MemberTreeNode[] Values { get; private set; }
        
        public override async Task GenerateTree()
        {
            Array = TreeNode.Object as Array;
            
            Type elemType = Array.DetermineElementType();
            Type objType = TreeNode.ObjectType;

            object[] vals = new object[Array.Length];
            Array.CopyTo(vals, 0);
            Values = vals.Select(x => new MemberTreeNode(x, new VarInfo(x?.GetType() ?? elemType, objType), TreeNode.Writer)).ToArray();
            
            foreach (var val in Values)
            {
                await val.GenerateTree();
            }
        }
    }
}
