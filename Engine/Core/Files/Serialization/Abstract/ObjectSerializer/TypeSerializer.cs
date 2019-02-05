using System;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectSerializerFor(typeof(Type), CanSerializeAsString = true)]
    public class TypeSerializer : BaseObjectSerializer
    {
        #region Tree
        public override void DeserializeTreeToObject()
        {
            Type type = TreeNode.ObjectType;

            if (TreeNode.GetElementContent(type, out object literalType))
                TreeNode.Object = literalType;
            else
                Engine.LogWarning($"Element {TreeNode.Name} cannot be parsed as {type.GetFriendlyName()}");
        }
        public override void SerializeTreeFromObject()
        {
            if (!(TreeNode.Object is Type type))
                return;

            if (!TreeNode.SetElementContent(type))
                Engine.LogWarning($"{type.GetFriendlyName()} cannot be written as a string.");
        }
        #endregion

        #region String
        public override bool CanWriteAsString(Type type) => true;
        public override object ObjectFromString(Type type, string value)
            => SerializationCommon.CreateType(value);
        public override bool ObjectToString(object obj, out string str)
        {
            if (obj is Type type)
            {
                str = type.AssemblyQualifiedName;
                return true;
            }
            str = null;
            return false;
        }
        #endregion

        #region Binary
        protected override int OnGetTreeSize(Serializer.WriterBinary binWriter)
            => throw new NotImplementedException();
        public override void SerializeTreeToBinary(ref VoidPtr address, Serializer.WriterBinary binWriter)
            => throw new NotImplementedException();
        public override void DeserializeTreeFromBinary(ref VoidPtr address, Deserializer.ReaderBinary binReader)
            => throw new NotImplementedException();
        #endregion
    }
}
