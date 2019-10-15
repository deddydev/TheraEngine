using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    [Serializable]
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
    public abstract class BaseObjectSerializer : TObject
    {
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
        public abstract bool ObjectFromString(TypeProxy type, string value, out object result);
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

        public abstract bool CanWriteAsString(TypeProxy type);

        static BaseObjectSerializer()
        {
            ClearObjectSerializerCache();
        }

        public static Lazy<Dictionary<ObjectSerializerFor, TypeProxy>> ObjectSerializers { get; set; }
        /// <summary>
        /// Finds the class to use to read and write the given type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="mustAllowStringSerialize"></param>
        /// <param name="mustAllowBinarySerialize"></param>
        /// <returns></returns>
        public static BaseObjectSerializer DetermineObjectSerializer(
            TypeProxy objectType, bool mustAllowStringSerialize = false, bool mustAllowBinarySerialize = false)
        {
            if (objectType is null)
            {
                Engine.LogWarning("Unable to create object serializer for null type.");
                return null;
            }

            var temp = ObjectSerializers.Value.Where(kv =>
                objectType?.IsAssignableTo(kv.Key.ObjectType) ?? false);

            if (mustAllowStringSerialize)
                temp = temp.Where(kv => kv.Key.CanSerializeAsString);
            if (mustAllowBinarySerialize)
                temp = temp.Where(kv => kv.Key.CanSerializeAsBinary);

            TypeProxy[] types = temp.Select(x => x.Value).ToArray();

            TypeProxy serType;
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

            //Engine.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] Determined object serializer for {objectType.GetFriendlyName()}: {serType.GetFriendlyName()}");

            return serType.CreateInstance() as BaseObjectSerializer;
        }
        private static Dictionary<ObjectSerializerFor, TypeProxy> GetObjectSerializers()
        {
            Engine.PrintLine($"Reloading object serializer cache.");
            Type baseObjSerType = typeof(BaseObjectSerializer);
            IEnumerable<TypeProxy> typeList = AppDomainHelper.FindTypes(type =>
                type.IsAssignableTo(baseObjSerType) &&
                type.HasCustomAttribute<ObjectSerializerFor>());

            var serializers = new Dictionary<ObjectSerializerFor, TypeProxy>();
            foreach (Type type in typeList)
            {
                var attrib = type.GetCustomAttribute<ObjectSerializerFor>();
                if (!serializers.ContainsKey(attrib))
                    serializers.Add(attrib, type);
            }
            Engine.PrintLine("Done loading object serializer cache.");
            return serializers;
        }
        public static void ClearObjectSerializerCache()
        {
            Engine.PrintLine("Clearing object serializer cache.");
            ObjectSerializers = new Lazy<Dictionary<ObjectSerializerFor, TypeProxy>>(GetObjectSerializers, LazyThreadSafetyMode.PublicationOnly);
        }

    }
}
