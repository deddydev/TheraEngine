using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectSerializerFor(typeof(IDictionary))]
    public class IDictionarySerializer : BaseObjectSerializer
    {
        public override void DeserializeTreeToObject()
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
                    if (i >= TreeNode.ChildElements.Count)
                        break;

                    var keyValNode = TreeNode.ChildElements[i];
                    if (keyValNode.ChildElements.Count < 2)
                        continue;

                    var keyNode = keyValNode.ChildElements[0];
                    var valNode = keyValNode.ChildElements[1];

                    keyNode.MemberInfo.MemberType = keyType;
                    keyNode.DeserializeTreeToObject();

                    valNode.MemberInfo.MemberType = valType;
                    valNode.DeserializeTreeToObject();

                    dic.Add(keyNode.Object, valNode.Object);
                }
            }
        }
        public override void SerializeTreeFromObject()
        {
            IDictionary dic = TreeNode.Object as IDictionary;

            object[] keys = new object[dic.Keys.Count];
            object[] vals = new object[dic.Values.Count];

            Type keyType = dic.DetermineKeyType();
            Type valType = dic.DetermineValueType();
            Type objType = TreeNode.ObjectType;

            dic.Keys.CopyTo(keys, 0);
            dic.Values.CopyTo(vals, 0);

            SerializeElement[] keyNodes = keys.Select(obj => new SerializeElement(obj, new TSerializeMemberInfo(keyType, "Key"/*SerializationCommon.GetTypeName(keyType)*/))).ToArray();
            SerializeElement[] valNodes = vals.Select(obj => new SerializeElement(obj, new TSerializeMemberInfo(valType, "Value"/*SerializationCommon.GetTypeName(valType)*/))).ToArray();
            
            for (int i = 0; i < keyNodes.Length; ++i)
            {
                SerializeElement pairNode = new SerializeElement(null, new TSerializeMemberInfo(null, "KV" + i.ToString()));
                TreeNode.ChildElements.Add(pairNode);

                SerializeElement keyNode = keyNodes[i];
                SerializeElement valNode = valNodes[i];

                pairNode.ChildElements.Add(keyNode);
                keyNode.SerializeTreeFromObject();

                pairNode.ChildElements.Add(valNode);
                valNode.SerializeTreeFromObject();
            }
        }
        protected override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override void DeserializeTreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binReader)
        {
            throw new NotImplementedException();
        }
        public override void SerializeTreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }

        public override object ObjectFromString(Type type, string value) => throw new NotImplementedException();
        public override bool ObjectToString(object obj, out string str) => throw new NotImplementedException();
        public override bool CanWriteAsString(Type type)
        {
            Type[] types = type.GetGenericArguments();
            return false;
        }
    }
}
