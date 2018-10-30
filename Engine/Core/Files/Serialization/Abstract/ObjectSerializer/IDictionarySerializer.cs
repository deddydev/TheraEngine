using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(IDictionary))]
    public class IDictionarySerializer : BaseObjectSerializer
    {
        public IDictionary Dictionary { get; private set; }
        public MemberTreeNode[] Keys { get; private set; }
        public MemberTreeNode[] Values { get; private set; }
        
        public override void TreeFromBinary(ref VoidPtr address)
        {

        }
        public override void TreeFromObject()
        {
            Dictionary = TreeNode.Object as IDictionary;

            object[] keys = new object[Dictionary.Keys.Count];
            object[] vals = new object[Dictionary.Values.Count];

            Type keyType = Dictionary.DetermineKeyType();
            Type valType = Dictionary.DetermineValueType();
            Type objType = TreeNode.ObjectType;

            Dictionary.Keys.CopyTo(keys, 0);
            Dictionary.Values.CopyTo(vals, 0);
            
            Keys   = keys.Select(obj => new MemberTreeNode(obj, new TSerializeMemberInfo(keyType, SerializationCommon.GetTypeName(keyType)))).ToArray();
            Values = vals.Select(obj => new MemberTreeNode(obj, new TSerializeMemberInfo(valType, SerializationCommon.GetTypeName(valType)))).ToArray();

            TreeNode.ChildElementMembers = new EventList<MemberTreeNode>(Keys.Length);
            for (int i = 0; i < Keys.Length; ++i)
            {
                MemberTreeNode pairNode = new MemberTreeNode(null, null);

                pairNode.ChildElementMembers.Add(Keys[i]);
                pairNode.ChildElementMembers.Add(Values[i]);

                TreeNode.ChildElementMembers.Add(pairNode);
            }
        }

        public override void TreeToObject() => throw new NotImplementedException();
        public override void TreeToBinary(ref VoidPtr address) => throw new NotImplementedException();
    }
}
