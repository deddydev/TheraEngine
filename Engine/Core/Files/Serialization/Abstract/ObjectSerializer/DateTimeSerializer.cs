using Extensions;
using System;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectSerializerFor(typeof(DateTime), CanSerializeAsString = true)]
    public class DateTimeSerializer : BaseObjectSerializer
    {
        #region Tree
        public override void DeserializeTreeToObject()
        {
            TypeProxy type = TreeNode.ObjectType;

            if (TreeNode.Content.GetObject(type, out object literalType))
                TreeNode.Object = literalType;
            else
                Engine.LogWarning($"Element {TreeNode.Name} cannot be parsed as {type.GetFriendlyName()}");
        }
        public override void SerializeTreeFromObject()
        {
            if (!(TreeNode.Object is DateTime time))
                return;

            if (!TreeNode.Content.SetValueAsObject(time))
                Engine.LogWarning($"{time.ToString()} cannot be written as a string.");
        }
        #endregion

        #region String
        public override bool CanWriteAsString(TypeProxy type) => true;
        public override bool ObjectFromString(TypeProxy type, string value, out object result)
        {
            result = DateTime.FromFileTimeUtc(long.Parse(value)).ToLocalTime();
            return true;
        }
        public override bool ObjectToString(object obj, out string str)
        {
            if (obj is DateTime time)
            {
                str = time.ToUniversalTime().ToFileTimeUtc().ToString();
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
