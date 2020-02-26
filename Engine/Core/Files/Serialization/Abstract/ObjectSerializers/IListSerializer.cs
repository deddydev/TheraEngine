using Extensions;
using System;
using System.Collections;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public interface IStreamableSerializer : IBaseObjectSerializer
    {
        object StreamChunk(int startIndex, int count, MemoryMappedViewStream stream);
    }
    [ObjectSerializerFor(typeof(IList), CanSerializeAsString = true)]
    public class IListSerializer : BaseObjectSerializer
    {
        #region Tree

        public event Action DoneReadingElements;
        public IList List { get; private set; }

        public override async Task DeserializeTreeToObjectAsync()
        {
            TypeProxy listType = TreeNode.ObjectType;

            if (IsStreamable && DoNotReadStreamables)
            {
                //TODO: Cache this serializer and string value in object as streamable property

                return;
            }

            if (TreeNode.Content.GetObject(this, listType, out object list))
            {
                List = list as IList;
                TreeNode.Object = list;
            }
            else
            {
                int count = TreeNode.Children.Count;

                if (listType.IsArray)
                    List = listType.CreateInstance(count) as IList;
                else
                    List = listType.CreateInstance() as IList;

                Task task = ReadElementsAsync(listType, count);

                bool async = TreeNode.MemberInfo?.DeserializeAsync ?? false;
                if (async)
                    TreeNode.Manager.PendingAsyncTasks.Add(task);
                else
                    await task;
                
                TreeNode.Object = List;
            }
        }

        private async Task<object> ReadElementAsync(int index)
        {
            SerializeElement node = TreeNode.Children[index];
            await node.DeserializeTreeToObjectAsync();
            return node.Object;
        }
        private async Task ReadElementsAsync(TypeProxy arrayType, int count)
        {
            TypeProxy elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
            for (int i = 0; i < count; ++i)
            {
                SerializeElement node = TreeNode.Children[i];
                node.MemberInfo.MemberType = elementType;
                bool objSet = await node.DeserializeTreeToObjectAsync();

                node.ObjectChanged += Node_ObjectChanged;

                if (List.IsFixedSize)
                    List[i] = node.Object;
                else
                    List.Add(node.Object);
            }
            DoneReadingElements?.Invoke();
        }
        private void Node_ObjectChanged(SerializeElement element, object previousObject)
        {
            int index = element.Parent.Children.IndexOf(element);
            List[index] = element.Object;
        }

        public override async Task SerializeTreeFromObjectAsync()
        {
            if (!(TreeNode.Object is IList list))
                return;

            if (TreeNode.Content.SetValueAsObject(list))
                return;

            Type elemType = list.DetermineElementType();
            foreach (object o in list)
            {
                SerializeElement element = new SerializeElement(o, new TSerializeMemberInfo(elemType, null));
                //Even default members must be written so the actual array count and indices all match up
                //if (ShouldWriteDefaultMembers || !element.IsObjectDefault())
                //{
                TreeNode.Children.Add(element);
                await element.SerializeTreeFromObjectAsync();
                //}
            }
        }
        #endregion

        #region String
        public override bool CanWriteAsString(TypeProxy type)
        {
            TypeProxy elementType = type.DetermineElementType();
            BaseObjectSerializer ser = GetSerializerFor(elementType, true);
            return ser != null && ser.CanWriteAsString(elementType);
        }
        public override bool ObjectFromString(TypeProxy type, string value, out object result)
        {
            TypeProxy elementType = type.DetermineElementType();
            BaseObjectSerializer ser = GetSerializerFor(elementType, true);
            if (ser is null || !ser.CanWriteAsString(elementType))
            {
                result = null;
                return false;
            }

            const char separator = '|';
            //if (!SerializationCommon.IsPrimitiveType(elementType))
            //    separator = '|';

            string[] values = value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            IList list;
            if (type.IsArray)
                list = type.CreateInstance(values.Length) as IList;
            else
                list = type.CreateInstance() as IList;

            Parse(list, values, elementType, ser);

            result = list;
            return true;
        }

        private void Parse(IList list, string[] values, TypeProxy elementType, BaseObjectSerializer ser)
        {
            object o;
            if (list.IsFixedSize)
            {
                for (int i = 0; i < values.Length; ++i)
                    if (ser.ObjectFromString(elementType, values[i], out o))
                        list[i] = o;
            }
            else
            {
                foreach (string t in values)
                    if (ser.ObjectFromString(elementType, t, out o))
                        list.Add(o);
            }
        }

        public override bool ObjectToString(object obj, out string str)
        {
            const string separator = "|";

            str = null;

            if (!(obj is IList list))
                return false;

            Type arrayType = list.GetType();
            Type elementType = arrayType.DetermineElementType();

            BaseObjectSerializer ser = GetSerializerFor(elementType, true);
            if (ser is null || !ser.CanWriteAsString(elementType))
            {
                string Convert(object elem)
                {
                    //if (!SerializationCommon.IsPrimitiveType(elementType))
                    //    separator = "|";
                    Type indivElemType = elem?.GetType() ?? elementType;
                    BaseObjectSerializer ser2 = GetSerializerFor(indivElemType, true);
                    if (ser2 is null || !ser2.CanWriteAsString(indivElemType))
                        throw new InvalidOperationException();

                    ser2.ObjectToString(elem, out string subStr);
                    return subStr;
                }

                try
                {
                    str = list.ToStringListGeneric(separator, separator, Convert);
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
            else
            {
                //if (!SerializationCommon.IsPrimitiveType(elementType))
                //    separator = "|";

                string Convert(object elem)
                {
                    //No if statements for best speed possible
                    ser.ObjectToString(elem, out string subStr);
                    return subStr;
                }

                str = list.ToStringListGeneric(separator, separator, Convert);
            }
            return true;
        }
        #endregion

        #region Binary
        public override void SerializeTreeToBinary(ref VoidPtr address, Serializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        protected override int OnGetTreeSize(Serializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override void DeserializeTreeFromBinary(ref VoidPtr address, Deserializer.ReaderBinary binReader)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
