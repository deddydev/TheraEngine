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
        
        public override async Task CollectSerializedMembers()
        {
            List = TreeNode.Object as IList;

            Type listType = List.DetermineElementType();
            Type objType = TreeNode.ObjectType;

            for (int i = 0; i < List.Count; ++i)
                Members.Add(TreeNode.FormatWriter.CreateNode(List[i], new VarInfo(List[i]?.GetType() ?? listType, objType)));

            foreach (MemberTreeNode t in Members)
            {
                await t.CollectSerializedMembers();
            }
        }
    }
}
