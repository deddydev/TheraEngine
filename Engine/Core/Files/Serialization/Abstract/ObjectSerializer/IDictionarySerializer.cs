using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(IDictionary))]
    public class IDictionarySerializer : BaseObjectSerializer
    {
        public override void TreeToObject()
        {
            int keyValCount = TreeNode.ChildElements.Count;
            Type dicType = TreeNode.ObjectType;

            IDictionary dic = Activator.CreateInstance(dicType) as IDictionary;
            TreeNode.Object = dic;

            if (keyValCount > 0)
            {
                var args = dicType.GetGenericArguments();
                Type keyType = args[0];
                Type valType = args[1];

                for (int i = 0; i < keyValCount; ++i)
                {
                    var keyValNode = TreeNode.ChildElements[i];

                    var keyNode = keyValNode.ChildElements[0];
                    var valNode = keyValNode.ChildElements[1];

                    keyNode.MemberInfo.MemberType = keyType;
                    keyNode.TreeToObject();

                    valNode.MemberInfo.MemberType = valType;
                    valNode.TreeToObject();

                    dic.Add(keyNode.Object, valNode.Object);
                }
            }
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

            SerializeElement[] keyNodes = keys.Select(obj => new SerializeElement(obj, new TSerializeMemberInfo(keyType, SerializationCommon.GetTypeName(keyType)))).ToArray();
            SerializeElement[] valNodes = vals.Select(obj => new SerializeElement(obj, new TSerializeMemberInfo(valType, SerializationCommon.GetTypeName(valType)))).ToArray();

            TreeNode.ChildElements = new EventList<SerializeElement>(keyNodes.Length);
            for (int i = 0; i < keyNodes.Length; ++i)
            {
                SerializeElement pairNode = new SerializeElement(null, null);

                pairNode.ChildElements.Add(keyNodes[i]);
                pairNode.ChildElements.Add(valNodes[i]);

                TreeNode.ChildElements.Add(pairNode);
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
