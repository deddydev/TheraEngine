using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectSerializerFor(typeof(IList), CanSerializeAsString = true)]
    public class ListSerializer : BaseObjectSerializer
    {
        #region Tree

        public IList List { get; private set; }

        public override async void DeserializeTreeToObject()
        {
            Type arrayType = TreeNode.ObjectType;

            if (!TreeNode.GetElementContent(arrayType, out object array))
            {
                int count = TreeNode.ChildElements.Count;
                
                if (arrayType.IsArray)
                    List = Activator.CreateInstance(arrayType, count) as IList;
                else
                    List = Activator.CreateInstance(arrayType) as IList;

                Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
                for (int i = 0; i < count; ++i)
                {
                    SerializeElement node = TreeNode.ChildElements[i];
                    node.MemberInfo.MemberType = elementType;
                    bool objSet = await node.DeserializeTreeToObject();
                    node.ObjectChanged += Node_ObjectChanged;
                    
                    if (List.IsFixedSize)
                        List[i] = node.Object;
                    else
                        List.Add(node.Object);
                }
                
                array = List;
            }

            TreeNode.Object = array;
        }

        private void Node_ObjectChanged(SerializeElement element, object previousObject)
        {
            int index = element.Parent.ChildElements.IndexOf(element);
            List[index] = element.Object;
        }

        public override void SerializeTreeFromObject()
        {
            if (!(TreeNode.Object is IList list))
                return;

            if (TreeNode.SetElementContent(list))
                return;

            Type elemType = list.DetermineElementType();
            foreach (object o in list)
            {
                SerializeElement element = new SerializeElement(o, new TSerializeMemberInfo(elemType, null));
                //Even default members must be written so the actual array count and indices all match up
                //if (ShouldWriteDefaultMembers || !element.IsObjectDefault())
                //{
                    TreeNode.ChildElements.Add(element);
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
