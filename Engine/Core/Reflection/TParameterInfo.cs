using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class TParameterInfo
    {
        public int Position;
        public ParameterAttributes Attributes;
        //public MemberInfo Member;
        public bool IsIn;
        public bool IsRetval;
        public bool IsLcid;
        public object RawDefaultValue;
        public bool IsOptional;
        public bool IsOut;
        public object DefaultValue;
        //public IEnumerable<CustomAttributeData> CustomAttributes;
        public string Name;
        public TType ParameterType;
        public int MetadataToken;
        public bool HasDefaultValue;

        public TParameterInfo() { }
        public TParameterInfo(ParameterInfo info)
        {
            Position = info.Position;
            Attributes = info.Attributes;
            //Member = info.Member;
            IsIn = info.IsIn;
            IsRetval = info.IsRetval;
            IsLcid = info.IsLcid;
            RawDefaultValue = info.RawDefaultValue;
            IsOptional = info.IsOptional;
            IsOut = info.IsOut;
            DefaultValue = info.DefaultValue;
            //CustomAttributes = info.CustomAttributes;
            Name = info.Name;
            ParameterType = TType.From(info.ParameterType);
            MetadataToken = info.MetadataToken;
            HasDefaultValue = info.HasDefaultValue;
        }
    }
}
