using System;
using System.Collections;
using System.Collections.Generic;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectSerializerFor(typeof(Type))]
    public class TypeSerializer : BaseObjectSerializer
    {
        public override bool CanWriteTreeAsString() => true;
        public override void DeserializeTreeToObject()
        {
            Type type = TreeNode.ObjectType;

            if (!TreeNode.GetElementContent(type, out object literalType))
            {
                
            }

            TreeNode.Object = literalType;
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
        public override string SerializeTreeToString()
        {
            bool success = TreeNode.GetElementContentAsString(out string value);
            if (success)
                return value;
            throw new InvalidOperationException();
        }
        protected override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override void DeserializeTreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binReader)
        {
            throw new NotImplementedException();
        }
        public override void DeserializeTreeFromString(string value)
        {
            TreeNode.Object = SerializationCommon.CreateType(value);
        }
    }
}
