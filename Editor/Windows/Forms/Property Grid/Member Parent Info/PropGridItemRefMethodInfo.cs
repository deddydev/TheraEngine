using System.ComponentModel;
using TheraEngine.Core.Reflection;

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
        public MethodInfoProxy Method { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override TypeProxy DataType => Method?.ReturnType;
        
        public PropGridMemberInfoMethod(IPropGridMemberOwner owner, MethodInfoProxy method) : base(owner)
        {
            Method = method;
        }
        
        internal protected override void SubmitStateChange(object oldValue, object newValue, ValueChangeHandler dataChangeHandler) { }
        public override object MemberValue { get { return null; } set { } }
    }
}
