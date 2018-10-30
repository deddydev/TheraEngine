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
        public override void TreeToObject()
        {

        }
        public override void TreeFromObject()
        {
            IDictionary dic = TreeNode.Object as IDictionary;

            object[] keys = new object[dic.Keys.Count];
            object[] vals = new object[dic.Values.Count];

            Type keyType = dic.DetermineKeyType();
            Type valType = dic.DetermineValueType();
            Type objType = TreeNode.ObjectType;

            dic.Keys.CopyTo(keys, 0);
            dic.Values.CopyTo(vals, 0);

            MemberTreeNode[] keyNodes = keys.Select(obj => new MemberTreeNode(obj, new TSerializeMemberInfo(keyType, SerializationCommon.GetTypeName(keyType)))).ToArray();
            MemberTreeNode[] valNodes = vals.Select(obj => new MemberTreeNode(obj, new TSerializeMemberInfo(valType, SerializationCommon.GetTypeName(valType)))).ToArray();

            TreeNode.ChildElementMembers = new EventList<MemberTreeNode>(keyNodes.Length);
            for (int i = 0; i < keyNodes.Length; ++i)
            {
                MemberTreeNode pairNode = new MemberTreeNode(null, null);

                pairNode.ChildElementMembers.Add(keyNodes[i]);
                pairNode.ChildElementMembers.Add(valNodes[i]);

                TreeNode.ChildElementMembers.Add(pairNode);
            }
        }

        public override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override void TreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binReader)
        {
            throw new NotImplementedException();
        }
        public override void TreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
    }
}
