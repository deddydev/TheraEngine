using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(IList))]
    public class IListWriter : BaseObjectWriter
    {
        public IList List { get; private set; }
        public MemberTreeNode[] Values { get; private set; }
        
        public override async Task CollectSerializedMembers()
        {
            List = TreeNode.Object as IList;

            Type elemType = List.DetermineElementType();
            Type objType = TreeNode.ObjectType;

            object[] vals = new object[List.Count];
            List.CopyTo(vals, 0);
            Values = vals.Select(obj => TreeNode.FormatWriter.CreateNode(obj, new VarInfo(obj?.GetType() ?? elemType, objType))).ToArray();

            //Values = new MemberTreeNode[List.Count];
            //for (int i = 0; i < List.Count; ++i)
            //    Values[i] = TreeNode.FormatWriter.CreateNode(List[i], new VarInfo(List[i]?.GetType() ?? listType, objType));

            foreach (MemberTreeNode t in Values)
                await t.CollectSerializedMembers();
        }
    }
}
