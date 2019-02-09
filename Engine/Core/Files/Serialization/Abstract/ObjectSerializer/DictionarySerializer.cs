using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectSerializerFor(typeof(IDictionary))]
    public class DictionarySerializer : BaseObjectSerializer
    {
        public IDictionary Dictionary { get; private set; }

        public override async void DeserializeTreeToObject()
        {
            int keyValCount = TreeNode.Children.Count;
            Type dicType = TreeNode.ObjectType;

            Dictionary = Activator.CreateInstance(dicType) as IDictionary;
            TreeNode.Object = Dictionary;

            if (keyValCount <= 0)
                return;

            Type[] args = dicType.GetGenericArguments();
            Type keyType = args[0];
            Type valType = args[1];

            for (int i = 0; i < keyValCount; ++i)
            {
                if (i >= TreeNode.Children.Count)
                    break;

                SerializeElement keyValNode = TreeNode.Children[i];
                if (keyValNode.Children.Count < 2)
                    continue;

                SerializeElement keyNode = keyValNode.Children[0];
                SerializeElement valNode = keyValNode.Children[1];

                keyNode.MemberInfo.MemberType = keyType;
                bool keyObjSet = await keyNode.DeserializeTreeToObject();
                //if (!keyObjSet)
                {
                    keyNode.ObjectChanged += KeyNode_ObjectChanged;
                }

                valNode.MemberInfo.MemberType = valType;
                bool valObjSet = await valNode.DeserializeTreeToObject();
                //if (!valObjSet)
                {
                    valNode.ObjectChanged += ValNode_ObjectChanged;
                }

                if (keyObjSet && valObjSet)
                    Dictionary.Add(keyNode.Object, valNode.Object);
            }
        }

        private void ValNode_ObjectChanged(SerializeElement valNode, object prev)
        {
            SerializeElement keyNode = valNode.Parent.Children[0];
            //if (keyNode.Object == null)
            //    return;

            if (Dictionary.Contains(keyNode.Object))
                Dictionary[keyNode.Object] = valNode.Object;
            else
                Dictionary.Add(keyNode.Object, valNode.Object);
        }

        private void KeyNode_ObjectChanged(SerializeElement keyNode, object prev)
        {
            if (Dictionary.Contains(prev))
                Dictionary.Remove(prev);

            //if (keyNode.Object == null)
            //    return;
            if (Dictionary.Contains(keyNode.Object))
                return;

            SerializeElement valNode = keyNode.Parent.Children[0];

            if (Dictionary.Contains(keyNode.Object))
                Dictionary[keyNode.Object] = valNode.Object;
            else
                Dictionary.Add(keyNode.Object, valNode.Object);
        }

        public override void SerializeTreeFromObject()
        {
            if (!(TreeNode.Object is IDictionary dic))
                return;

            object[] keys = new object[dic.Keys.Count];
            object[] vals = new object[dic.Values.Count];

            Type keyType = dic.DetermineKeyType();
            Type valType = dic.DetermineValueType();

            dic.Keys.CopyTo(keys, 0);
            dic.Values.CopyTo(vals, 0);

            SerializeElement[] keyNodes = keys.Select(obj => new SerializeElement(obj, new TSerializeMemberInfo(keyType, "Key"/*SerializationCommon.GetTypeName(keyType)*/))).ToArray();
            SerializeElement[] valNodes = vals.Select(obj => new SerializeElement(obj, new TSerializeMemberInfo(valType, "Value"/*SerializationCommon.GetTypeName(valType)*/))).ToArray();
            
            for (int i = 0; i < keyNodes.Length; ++i)
            {
                SerializeElement pairNode = new SerializeElement(null, new TSerializeMemberInfo(null, "KV" + i));
                TreeNode.Children.Add(pairNode);

                SerializeElement keyNode = keyNodes[i];
                SerializeElement valNode = valNodes[i];

                pairNode.Children.Add(keyNode);
                keyNode.SerializeTreeFromObject();

                pairNode.Children.Add(valNode);
                valNode.SerializeTreeFromObject();
            }
        }
        protected override int OnGetTreeSize(Serializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override void DeserializeTreeFromBinary(ref VoidPtr address, Deserializer.ReaderBinary binReader)
        {
            throw new NotImplementedException();
        }
        public override void SerializeTreeToBinary(ref VoidPtr address, Serializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }

        public override bool ObjectFromString(Type type, string value, out object result) => throw new NotImplementedException();
        public override bool ObjectToString(object obj, out string str) => throw new NotImplementedException();
        public override bool CanWriteAsString(Type type)
        {
            Type[] types = type.GetGenericArguments();
            return false;
        }
    }
}
