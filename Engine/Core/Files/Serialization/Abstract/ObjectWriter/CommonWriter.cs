using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Tools;
using static TheraEngine.Core.Files.Serialization.TSerializer.WriterBinary;

namespace TheraEngine.Core.Files.Serialization
{
    public class CommonWriter : BaseObjectWriter
    {
        public List<MemberTreeNode> Children { get; private set; }
        //public Dictionary<string, MemberTreeNode[]> CategorizedChildren { get; private set; }
        private int NonAttributeCount { get; set; }
        
        public override async Task GenerateTree()
        {
            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);

            Children = members.
                Where(x => (x.Attrib == null || x.Attrib.Condition == null) ? true : ExpressionParser.Evaluate<bool>(x.Attrib.Condition, TreeNode.Object)).
                Select(x => new MemberTreeNode(TreeNode.Object == null ? null : x.GetValue(TreeNode.Object), x, TreeNode.Writer)).
                ToList();
            
            var categorizedChildren = Children.
                Where(x => x.MemberInfo.Category != null).
                GroupBy(x => x.MemberInfo.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());
            foreach (var grouping in categorizedChildren)
                foreach (MemberTreeNode p in grouping.Value)
                    Children.Remove(p);
            foreach (var cat in categorizedChildren)
                Children.Add(new MemberTreeNode(TreeNode.Object, TreeNode.MemberInfo, TreeNode.Writer));
            
            int childElementCount = Children.Where(x => !x.MemberInfo.Attrib.IsXmlAttribute && x.Object != null).Count();
            NonAttributeCount = childElementCount + categorizedChildren.Count;
            Attributes = new List<(string, object)>();
            ChildElements = new List<MemberTreeNode>();
            
            foreach (MemberTreeNode member in Children)
                await CollectMember(member);

            //foreach (var group in CategorizedChildren)
            //    foreach (MemberTreeNode member in group.Value)
            //        await CollectMember(member);
        }
        private async Task CollectMember(MemberTreeNode member)
        {
            if (member.MemberInfo.Attrib.State && !TreeNode.Writer.Flags.HasFlag(ESerializeFlags.SerializeState))
                return;
            if (member.MemberInfo.Attrib.Config && !TreeNode.Writer.Flags.HasFlag(ESerializeFlags.SerializeConfig))
                return;

            MethodInfo customMethod = TreeNode.CustomMethods.FirstOrDefault(
                x => string.Equals(member.MemberInfo.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));
            if (customMethod != null)
                customMethod.Invoke(TreeNode.Object, new object[] { _writer, _flags });
            else if (member.Object == null)
                return;
            
            Type valueType = member.ObjectType;
            if (member.MemberInfo.Attrib.IsXmlElementString)
            {
                if (SerializationCommon.GetString(member.Object, member.MemberInfo.VariableType, out string result))
                {
                    if (NonAttributeCount == 1)
                        SingleSerializableChildData = result;
                    else
                        Attributes
                        await _writer.WriteAttributeStringAsync(null, member.MemberInfo.Name, null, result);
                }
                else
                    await member.GenerateTree();
            }
            else if (member.MemberInfo.Attrib.IsXmlAttribute)
            {
                if (SerializationCommon.GetString(member.Object, member.VariableType, out string result))
                    _writer.WriteAttributeString(member.Name, result);
                else
                    await member.GenerateTree();
            }
            else
                await member.GenerateTree();
            
        }
        public override int GetSize(MethodInfo[] customMethods, ref int flagCount, BinaryStringTable table)
        {
            Type t = Info.VariableType;
            if (t.IsValueType)
                return Marshal.SizeOf(t);

            int size = 0;
            
            foreach (MemberTreeNode p in Children)
                size += GetSizeMember(p, customMethods, ref flagCount, table);
            
            foreach (var grouping in CategorizedChildren)
                foreach (MemberTreeNode p in grouping)
                    size += GetSizeMember(p, customMethods, ref flagCount, table);

            return size;
        }
        public override int GetSizeMember(MemberTreeNode node, MethodInfo[] customMethods, ref int flagCount, BinaryStringTable table)
        {
            object value = node.Object;

            MethodInfo customMethod = customMethods.FirstOrDefault(x => string.Equals(node.MemberInfo.Name, x.GetCustomAttribute<CustomBinarySerializeSizeMethod>().Name));

            if (customMethod != null)
                return (int)customMethod.Invoke(value, new object[] { table });
            
            if (TryGetSize(node, table, out int size))
                return size;

            Type t = node.MemberInfo.VariableType;

            if (t == typeof(bool))
                ++flagCount;
            else if (t == typeof(string))
            {
                if (value != null)
                    table.Add(value.ToString());
                size += 4;
            }
            else if (t.IsEnum)
            {
                //table.Add(value.ToString());
                size += 4;
            }
            else if (t.IsValueType)
            {
                if (node.Members.Count > 0)
                    size += node.GetSize(table);
                else
                    size += Marshal.SizeOf(value);
            }
            else
                size += node.GetSize(table);

            return size;
        }
        public override bool Write(ref VoidPtr address, BinaryStringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
