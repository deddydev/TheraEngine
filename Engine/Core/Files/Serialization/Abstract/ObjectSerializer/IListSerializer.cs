using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectSerializerFor(typeof(IList), CanSerializeAsString = true)]
    public class IListSerializer : BaseObjectSerializer
    {
        #region Tree

        public event Action DoneReadingElements;
        public IList List { get; private set; }
        public bool DeserializeAsync { get; private set; } = false;

        public override void DeserializeTreeToObject()
        {
            Type listType = TreeNode.ObjectType;
            DeserializeAsync = TreeNode.MemberInfo?.DeserializeAsync ?? false;

            if (!TreeNode.Content.GetObject(listType, out object list))
            {
                int count = TreeNode.Children.Count;
                
                if (listType.IsArray)
                    List = Activator.CreateInstance(listType, count) as IList;
                else
                    List = Activator.CreateInstance(listType) as IList;
                
                if (DeserializeAsync)
                    Task.Run(() => ReadElements(listType, count)).ContinueWith(t => DoneReadingElements?.Invoke());
                else
                {
                    ReadElements(listType, count);
                    DoneReadingElements?.Invoke();
                }

                list = List;
            }

            TreeNode.Object = list;
        }
        private async void ReadElements(Type arrayType, int count)
        {
            Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
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
        }
        private void Node_ObjectChanged(SerializeElement element, object previousObject)
        {
            int index = element.Parent.Children.IndexOf(element);
            List[index] = element.Object;
        }

        public override void SerializeTreeFromObject()
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
                    element.SerializeTreeFromObject();
                //}
            }
        }
        #endregion

        #region String
        public override bool CanWriteAsString(Type type)
        {
            Type elementType = type.DetermineElementType();
            BaseObjectSerializer ser = DetermineObjectSerializer(elementType, true);
            return ser != null && ser.CanWriteAsString(elementType);
        }
        public override bool ObjectFromString(Type type, string value, out object result)
        {
            Type elementType = type.DetermineElementType();
            BaseObjectSerializer ser = DetermineObjectSerializer(elementType, true);
            if (ser == null || !ser.CanWriteAsString(elementType))
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
                list = Activator.CreateInstance(type, values.Length) as IList;
            else
                list = Activator.CreateInstance(type) as IList;

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

            result = list;
            return true;
        }
        public override bool ObjectToString(object obj, out string str)
        {
            str = null;

            if (!(obj is IList list))
                return true;

            Type arrayType = list.GetType();
            Type elementType = arrayType.DetermineElementType();
            BaseObjectSerializer ser = DetermineObjectSerializer(elementType, true);
            if (ser == null || !ser.CanWriteAsString(elementType))
                return false;

            const string separator = "|";
            //if (!SerializationCommon.IsPrimitiveType(elementType))
            //    separator = "|";
            
            string Convert(object elem)
            {
                //No if statements for best speed possible
                ser.ObjectToString(elem, out string subStr);
                return subStr;
            }
            str = list.ToStringList(separator, separator, Convert);
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
