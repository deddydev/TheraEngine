using System;
using System.Collections;
using System.Collections.Generic;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(IList))]
    public class IListSerializer : BaseObjectSerializer
    {
        public override void DeserializeTreeToObject()
        {
            Type arrayType = TreeNode.ObjectType;
            Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];

            int count = 0;
            if (!TreeNode.GetElementContent(arrayType, out object array))
            {
                count = TreeNode.ChildElements.Count;

                IList list;
                if (arrayType.IsArray)
                    list = Activator.CreateInstance(arrayType, count) as IList;
                else
                    list = Activator.CreateInstance(arrayType) as IList;

                if (count > 0)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        SerializeElement node = TreeNode.ChildElements[i];
                        node.MemberInfo.MemberType = elementType;
                        node.DeserializeTreeToObject();

                        if (list.IsFixedSize)
                            list[i] = node.Object;
                        else
                            list.Add(node.Object);
                    }
                }
                array = list;
            }

            TreeNode.Object = array;
        }
        public override void SerializeTreeFromObject()
        {
            if (!(TreeNode.Object is IList list))
                return;

            Type elemType = list.DetermineElementType();

            foreach (object o in list)
            {
                SerializeElement element = new SerializeElement(o, new TSerializeMemberInfo(elemType, null));
                TreeNode.ChildElements.Add(element);
                element.SerializeTreeFromObject();
            }
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
