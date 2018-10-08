using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Tools;
using static TheraEngine.Core.Files.Serialization.TSerializer.WriterBinary;

namespace TheraEngine.Core.Files.Serialization
{
    public class CommonWriter : BaseObjectWriter
    {
        public List<MemberTreeNode> Members { get; private set; }
        public List<IGrouping<string, MemberTreeNode>> CategorizedMembers { get; private set; }

        public override void Initialize()
        {
            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(Info.VariableType);

            Members = members.
                Where(x => (x.Attrib == null || x.Attrib.Condition == null) ? true : ExpressionParser.Evaluate<bool>(x.Attrib.Condition, Object)).
                Select(x => new MemberTreeNode(Object == null ? null : x.GetValue(Object), x, Writer)).
                ToList();

            CategorizedMembers = Members.Where(x => x.Info.Category != null).GroupBy(x => SerializationCommon.FixElementName(x.Info.Category)).ToList();
            foreach (var grouping in CategorizedMembers)
                foreach (MemberTreeNode p in grouping)
                    Members.Remove(p);
        }
        public override async Task GenerateChildTree()
        {
            foreach (MemberTreeNode t in Members)
            {
                await t.GenerateChildTree();
            }
        }
        public override int GetSize(MethodInfo[] customMethods, ref int flagCount, BinaryStringTable table)
        {
            Type t = Info.VariableType;
            if (t.IsValueType)
                return Marshal.SizeOf(t);

            int size = 0;
            
            foreach (MemberTreeNode p in Members)
                size += GetSizeMember(p, customMethods, ref flagCount, table);
            
            foreach (var grouping in CategorizedMembers)
                foreach (MemberTreeNode p in grouping)
                    size += GetSizeMember(p, customMethods, ref flagCount, table);

            return size;
        }
        public override int GetSizeMember(MemberTreeNode node, MethodInfo[] customMethods, ref int flagCount, BinaryStringTable table)
        {
            object value = node.Object;

            MethodInfo customMethod = customMethods.FirstOrDefault(x => string.Equals(node.Info.Name, x.GetCustomAttribute<CustomBinarySerializeSizeMethod>().Name));

            if (customMethod != null)
                return (int)customMethod.Invoke(value, new object[] { table });
            
            if (TryGetSize(node, table, out int size))
                return size;

            Type t = node.Info.VariableType;

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
    }
}
