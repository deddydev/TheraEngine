﻿using System;
using System.Collections;
using System.Collections.Generic;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectSerializerFor(typeof(IList), CanSerializeAsString = true)]
    public class IListSerializer : BaseObjectSerializer
    {
        #region Tree
        public override void DeserializeTreeToObject()
        {
            Type arrayType = TreeNode.ObjectType;

            if (!TreeNode.GetElementContent(arrayType, out object array))
            {
                int count = TreeNode.ChildElements.Count;

                IList list;
                if (arrayType.IsArray)
                    list = Activator.CreateInstance(arrayType, count) as IList;
                else
                    list = Activator.CreateInstance(arrayType) as IList;

                Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
                for (int i = 0; i < count; ++i)
                {
                    SerializeElement node = TreeNode.ChildElements[i];
                    node.MemberInfo.MemberType = elementType;
                    node.DeserializeTreeToObject();

                    if (list.IsFixedSize)
                        list[i] = node.Object;
                    else
                        list.Add(node.Object);
                }
                
                array = list;
            }

            TreeNode.Object = array;
        }
        public override void SerializeTreeFromObject()
        {
            if (!(TreeNode.Object is IList list))
                return;

            if (!TreeNode.SetElementContent(list))
            {
                Type elemType = list.DetermineElementType();
                foreach (object o in list)
                {
                    SerializeElement element = new SerializeElement(o, new TSerializeMemberInfo(elemType, null));
                    if (ShouldWriteDefaultMembers || !element.IsObjectDefault())
                    {
                        TreeNode.ChildElements.Add(element);
                        element.SerializeTreeFromObject();
                    }
                }
            }
        }
        #endregion

        #region String
        public override object ObjectFromString(Type type, string value)
        {
            Type elementType = type.DetermineElementType();
            var ser = DetermineObjectSerializer(elementType, true);
            if (ser == null)
                return null;
            
            char separator = '|';
            //if (!SerializationCommon.IsPrimitiveType(elementType))
            //    separator = '|';

            string[] values = value.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            IList list;
            if (type.IsArray)
                list = Activator.CreateInstance(type, values.Length) as IList;
            else
                list = Activator.CreateInstance(type) as IList;
            
            if (list.IsFixedSize)
            {
                for (int i = 0; i < values.Length; ++i)
                    list[i] = ser.ObjectFromString(elementType, values[i]);
            }
            else
            {
                for (int i = 0; i < values.Length; ++i)
                    list.Add(ser.ObjectFromString(elementType, values[i]));
            }

            return list;
        }
        public override bool ObjectToString(object obj, out string str)
        {
            str = null;

            if (!(obj is IList list))
                return true;

            Type arrayType = list.GetType();
            Type elementType = arrayType.DetermineElementType();
            var ser = DetermineObjectSerializer(elementType, true);
            if (ser == null)
                return false;

            string separator = "|";
            //if (!SerializationCommon.IsPrimitiveType(elementType))
            //    separator = "|";
            
            string convert(object elem)
            {
                //No if statements for best speed possible
                ser.ObjectToString(elem, out string subStr);
                return subStr;
            }
            str = list.ToStringList(separator, separator, convert);
            return true;
        }
        #endregion

        #region Binary
        public override void SerializeTreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        protected override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override void DeserializeTreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binReader)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
