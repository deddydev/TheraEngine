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
        public MemberTreeNode[] Members { get; private set; } = null;

        public override void Initialize()
        {
            List = Object as IList;
            Members = new MemberTreeNode[List.Count];
            Type listType = List.DetermineElementType();
            Type objType = Object.GetType();
            for (int i = 0; i < List.Count; ++i)
                Members[i] = new MemberTreeNode(List[i], new VarInfo(List[i]?.GetType() ?? listType, objType), Writer);
        }
        public override async Task GenerateTree()
        {
            foreach (MemberTreeNode t in Members)
            {
                await t.GenerateTree();
            }
        }
    }
}
