using System;
using System.Collections;
using System.Collections.Generic;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(IList))]
    public class IListSerializer : BaseObjectSerializer
    {
        public override void TreeToObject()
        {
            int count = TreeNode.ChildElementMembers.Count;
            Type arrayType = TreeNode.ObjectType;

            IList list;
            if (arrayType.BaseType == typeof(Array))
                list = Activator.CreateInstance(arrayType, count) as IList;
            else
                list = Activator.CreateInstance(arrayType) as IList;

            if (count > 0)
            {
                Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
                for (int i = 0; i < count; ++i)
                {
                    MemberTreeNode node = TreeNode.ChildElementMembers[i];
                    node.MemberInfo.MemberType = elementType;
                    node.TreeToObject();

                    if (list.IsFixedSize)
                        list[i] = node.Object;
                    else
                        list.Add(node.Object);
                }
            }

            TreeNode.Object = list;
        }
        public override void TreeFromObject()
        {
            IList list = TreeNode.Object as IList;

            Type elemType = list.DetermineElementType();
            
            foreach (object o in list)
                TreeNode.ChildElementMembers.Add(new MemberTreeNode(o, new TSerializeMemberInfo(elemType, null)));
        }

        public override void TreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override void TreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binReader)
        {
            throw new NotImplementedException();
        }
    }
}
