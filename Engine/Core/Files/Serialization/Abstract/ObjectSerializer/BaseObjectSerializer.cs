using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ObjectSerializerFor : Attribute
    {
        public bool CanSerializeAsBinary { get; set; } = true;
        public bool CanSerializeAsString { get; set; } = false;
        /// <summary>
        /// The type this writer will be collecting members for.
        /// </summary>
        public Type ObjectType { get; }
        /// <summary>
        /// If this object needs specific handling in binary format.
        /// </summary>
        public bool ManualBinarySerialize { get; }
        public ObjectSerializerFor(Type objectType, bool manualBinarySerialize = false)
        {
            ObjectType = objectType;
            ManualBinarySerialize = manualBinarySerialize;
        }
    }
    /// <summary>
    /// Tool to collect all members of an object into an array of children.
    /// </summary>
    public abstract class BaseObjectSerializer
    {
        private static Dictionary<ObjectSerializerFor, Type> ObjectSerializers { get; set; } = null;

        public bool ShouldWriteDefaultMembers
            => TreeNode?.Owner?.Flags.HasFlag(ESerializeFlags.WriteDefaultMembers) ?? false;
        public bool WriteChangedMembersOnly
            => TreeNode?.Owner?.Flags.HasFlag(ESerializeFlags.WriteChangedMembersOnly) ?? false;

        public SerializeElement TreeNode { get; protected internal set; } = null;
        public int TreeSize { get; private set; }
        
        protected abstract int OnGetTreeSize(Serializer.WriterBinary binWriter);
        /// <summary>
        /// Retrieves the size of the serialization tree in bytes.
        /// </summary>
        /// <param name="binWriter"></param>
        /// <returns></returns>
        public int GetTreeSize(Serializer.WriterBinary binWriter)
            => TreeSize = OnGetTreeSize(binWriter);

        /// <summary>
        /// Creates the serialization tree from a binary representation.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="binReader"></param>
        public abstract void DeserializeTreeFromBinary(ref VoidPtr address, Deserializer.ReaderBinary binReader);
        /// <summary>
        /// Creates a binary representation of the serialization tree.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="binWriter"></param>
        public abstract void SerializeTreeToBinary(ref VoidPtr address, Serializer.WriterBinary binWriter);
        
        /// <summary>
        /// Creates an object from a string.
        /// </summary>
        public abstract bool ObjectFromString(Type type, string value, out object result);
        /// <summary>
        /// Creates a string from an object.
        /// </summary>
        /// <returns></returns>
        public abstract bool ObjectToString(object obj, out string str);

        /// <summary>
        /// Creates the TreeNode's object from the serialization tree.
        /// </summary>
        public abstract void DeserializeTreeToObject();
        /// <summary>
        /// Creates the serialization tree from the TreeNode's object.
        /// </summary>
        public abstract void SerializeTreeFromObject();

        public static void ResetObjectSerializerCache(bool reloadNow)
        {
            if (reloadNow)
            {
                Type baseObjSerType = typeof(BaseObjectSerializer);
                IEnumerable<Type> typeList = Engine.FindTypes(type =>
                   baseObjSerType.IsAssignableFrom(type) &&
                   (type.GetCustomAttributeExt<ObjectSerializerFor>() != null));

                ObjectSerializers = new Dictionary<ObjectSerializerFor, Type>();
                foreach (Type type in typeList)
                    ObjectSerializers.Add(type.GetCustomAttributeExt<ObjectSerializerFor>(), type);
            }
            else
            {
                ObjectSerializers = null;
            }
        }

        public abstract bool CanWriteAsString(Type type);

        /// <summary>
        /// Finds the class to use to read and write the given type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="mustAllowStringSerialize"></param>
        /// <param name="mustAllowBinarySerialize"></param>
        /// <returns></returns>
        public static BaseObjectSerializer DetermineObjectSerializer(
            Type objectType, bool mustAllowStringSerialize = false, bool mustAllowBinarySerialize = false)
        {
            if (objectType == null)
            {
                Engine.LogWarning("Unable to create object serializer for null type.");
                return null;
            }
            
            if (ObjectSerializers == null)
                ResetObjectSerializerCache(true);

            var temp = ObjectSerializers.Where(kv => (kv.Key.ObjectType?.IsAssignableFrom(objectType) ?? false));
            if (mustAllowStringSerialize)
                temp = temp.Where(kv => kv.Key.CanSerializeAsString);
            if (mustAllowBinarySerialize)
                temp = temp.Where(kv => kv.Key.CanSerializeAsBinary);
            Type[] types = temp.Select(x => x.Value).ToArray();
            
            Type serType;
            switch (types.Length)
            {
                case 0:
                    serType = typeof(CommonObjectSerializer);
                    break;
                case 1:
                    serType = types[0];
                    break;
                default:
                {
                    int[] counts = types.Select(x => types.Count(x.IsSubclassOf)).ToArray();
                    int min = counts.Min();
                    int[] mins = counts.FindAllMatchIndices(x => x == min);
                    string msg = "Type " + objectType.GetFriendlyName() + " has multiple valid object serializers: " + types.ToStringList(", ", " and ", x => x.GetFriendlyName());
                    msg += ". Narrowed down to " + mins.Select(x => types[x]).ToArray().ToStringList(", ", " and ", x => x.GetFriendlyName());
                    Engine.PrintLine(msg);
                    serType = types[mins[0]];
                    break;
                }
            }
            return (BaseObjectSerializer)Activator.CreateInstance(serType);
        }
    }
}
