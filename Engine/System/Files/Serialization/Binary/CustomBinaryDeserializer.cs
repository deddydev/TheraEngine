using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace CustomEngine.Files.Serialization
{
    public static partial class CustomBinarySerializer
    {
        /// <summary>
        /// Reads a file from the stream as xml.
        /// </summary>
        public static unsafe object Deserialize(string filePath, Type t)
        {
            object obj;
            using (FileMap map = FileMap.FromFile(filePath))
            {
                ushort parts = map.Address.UShort;
                bool encrypted = (parts & 0xFF00) != 0;
                bool compressed = (parts & 0x00FF) != 0;
                if (encrypted)
                {

                }
                if (compressed)
                {

                }
                FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                VoidPtr addr = hdr->Data;
                obj = (FileObject)ReadObject(t, ref addr, hdr->Strings);
                if (obj is FileObject o)
                    o.FilePath = filePath;
            }
            return obj;
        }
        private static object ReadObject(Type t, ref VoidPtr address, VoidPtr strings)
        {
            //Collect the members of this object's type that are serialized
            List<VarInfo> serializedMembers = SerializationCommon.CollectSerializedMembers(t);

            //Create the object
            object obj = SerializationCommon.CreateObject(t);

            //Get pre and post deserialize methods
            MethodInfo[] methods = t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<MethodInfo> preMethods = new List<MethodInfo>();
            List<MethodInfo> postMethods = new List<MethodInfo>();
            foreach (MethodInfo m in methods)
            {
                PreDeserialize pre = m.GetCustomAttribute<PreDeserialize>();
                if (pre != null && pre.RunForFormats.HasFlag(SerializeFormatFlag.Binary))
                    preMethods.Add(m);
                PostDeserialize post = m.GetCustomAttribute<PostDeserialize>();
                if (post != null && post.RunForFormats.HasFlag(SerializeFormatFlag.Binary))
                    postMethods.Add(m);
            }

            //Run pre-deserialize method
            foreach (MethodInfo m in preMethods.OrderBy(x => x.GetCustomAttribute<PreDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PreDeserialize>().Arguments);

            //Get members categorized together
            List<IGrouping<string, VarInfo>> categorized = serializedMembers.
                Where(x => x.Category != null).
                GroupBy(x => x.Category).ToList();

            //Remove categorized members from original list
            foreach (IGrouping<string, VarInfo> grouping in categorized)
                foreach (VarInfo p in grouping)
                    serializedMembers.Remove(p);

            //Read attributes first
            List<VarInfo> attribs = serializedMembers.Where(x => x.Attrib.IsXmlAttribute).ToList();
            //while (reader.ReadAttribute())
            //{
            //    //Look for matching attribute member with the same name
            //    VarInfo attrib = attribs.FirstOrDefault(x => reader.Name.Equals(x.Name, true));
            //    if (attrib != null)
            //    {
            //        Type fieldType = attrib.VariableType;
            //        object value = ParseString(reader.Value, fieldType);
            //        attrib.SetValue(obj, value);
            //        attribs.Remove(attrib);
            //    }
            //}

            //foreach (VarInfo attrib in attribs)
            //    attrib.SetValue(obj, attrib.Attrib.DefaultValue);

            //List<VarInfo> elements = serializedMembers.Where(x => !x.Attrib.IsXmlAttribute).ToList();
            //while (reader.BeginElement())
            //{
            //    var category = categorized.FirstOrDefault(x => reader.Name.Equals(x.Key, true));
            //    if (category != null)
            //    {
            //        while (reader.ReadAttribute())
            //        {
            //            VarInfo p = category.FirstOrDefault(x => x.Attrib.IsXmlAttribute && reader.Name.Equals(x.Name, true));
                        
            //        }
            //        while (reader.BeginElement())
            //        {

            //        }
            //    }
            //    else
            //    {
            //        VarInfo element = elements.FirstOrDefault(x => reader.Name.Equals(x.Name, true));
            //        if (element != null)
            //        {
            //            Type fieldType = element.VariableType;

            //        }
            //    }
            //    reader.EndElement();
            //}

            foreach (MethodInfo m in postMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PostDeserialize>().Arguments);

            return obj;
        }
        private static object ParseString(string value, Type t)
        {
            if (t.GetInterface("IParsable") != null)
            {
                IParsable o = (IParsable)Activator.CreateInstance(t);
                o.ReadFromString(value);
                return o;
            }
            switch (t.Name)
            {
                case "Boolean":
                    return Boolean.Parse(value);
                case "SByte":
                    return SByte.Parse(value);
                case "Byte":
                    return Byte.Parse(value);
                case "Char":
                    return Char.Parse(value);
                case "Int16":
                    return Int16.Parse(value);
                case "UInt16":
                    return UInt16.Parse(value);
                case "Int32":
                    return Int32.Parse(value);
                case "UInt32":
                    return UInt32.Parse(value);
                case "Int64":
                    return Int64.Parse(value);
                case "UInt64":
                    return UInt64.Parse(value);
                case "Single":
                    return Single.Parse(value);
                case "Double":
                    return Double.Parse(value);
                case "Decimal":
                    return Decimal.Parse(value);
            }
            throw new Exception(t.ToString() + " is not parsable");
            return null;
        }
    }
}
