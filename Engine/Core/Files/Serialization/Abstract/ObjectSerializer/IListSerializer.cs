using System;
using System.Collections;
using System.Collections.Generic;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectSerializerFor(typeof(IList))]
    public class IListSerializer : BaseObjectSerializer
    {
        public override void DeserializeTreeToObject()
        {
            Type arrayType = TreeNode.ObjectType;

            if (!TreeNode.GetElementContent(arrayType, out object array))
            {
                int count = TreeNode.ChildElements.Count;

                IList list;
                if (arrayType.IsArray)
                    list = Activator.CreateInstance(arrayType, count) as IList;
                else
                    list = Activator.CreateInstance(arrayType) as IList;

                Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
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
                
                array = list;
            }

            TreeNode.Object = array;
        }
        public override void SerializeTreeFromObject()
        {
            if (!(TreeNode.Object is IList list))
                return;

            if (!TreeNode.SetElementContent(list))
            {
                Type elemType = list.DetermineElementType();
                foreach (object o in list)
                {
                    SerializeElement element = new SerializeElement(o, new TSerializeMemberInfo(elemType, null));
                    if (ShouldWriteDefaultMembers || !element.IsObjectDefault())
                    {
                        TreeNode.ChildElements.Add(element);
                        element.SerializeTreeFromObject();
                    }
                }
            }
        }
        public override void SerializeTreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        protected override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override void DeserializeTreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binReader)
        {
            throw new NotImplementedException();
        }

        public override void DeserializeTreeFromString(string value) => throw new NotImplementedException();
        public override string SerializeTreeToString() => throw new NotImplementedException();
    }
}
