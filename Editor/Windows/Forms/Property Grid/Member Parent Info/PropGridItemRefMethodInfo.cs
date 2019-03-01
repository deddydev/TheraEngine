using System;
using System.ComponentModel;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridMemberInfoMethod : PropGridMemberInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string MemberAccessor => "." + Method?.GetFriendlyName(true) ?? "<null>";
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => Method?.GetFriendlyName(true);
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public MethodInfo Method { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Method?.ReturnType;
        
        public PropGridMemberInfoMethod(IPropGridMemberOwner owner, MethodInfo method) : base(owner)
        {
            Method = method;
        }
        
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler) { }
        public override object MemberValue { get { return null; } set { } }
    }
}
