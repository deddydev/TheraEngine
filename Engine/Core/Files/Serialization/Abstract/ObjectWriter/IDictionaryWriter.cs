using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(IDictionary))]
    public class IDictionaryWriter : BaseObjectWriter
    {
        public IDictionary Dictionary { get; private set; }
        public MemberTreeNode[] Keys { get; private set; }
        public MemberTreeNode[] Values { get; private set; }
        
        public override async Task GenerateTree()
        {
            Dictionary = TreeNode.Object as IDictionary;

            object[] keys = new object[Dictionary.Keys.Count];
            object[] vals = new object[Dictionary.Values.Count];

            Type keyType = Dictionary.DetermineKeyType();
            Type valType = Dictionary.DetermineValueType();
            Type objType = TreeNode.ObjectType;

            Dictionary.Keys.CopyTo(keys, 0);
            Dictionary.Values.CopyTo(vals, 0);

            Keys = keys.Select(x => new MemberTreeNode(x, new VarInfo(x?.GetType() ?? keyType, objType), TreeNode.Writer)).ToArray();
            Values = vals.Select(x => new MemberTreeNode(x, new VarInfo(x?.GetType() ?? valType, objType), TreeNode.Writer)).ToArray();

            foreach (var key in Keys)
            {
                await key.GenerateTree();
            }
            foreach (var val in Values)
            {
                await val.GenerateTree();
            }
        }
    }
}
