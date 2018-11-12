using System;
using System.ComponentModel;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemRefMethodInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => Method?.Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public MethodInfo Method { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Func<object> GetOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Method?.ReturnType;
        
        public PropGridItemRefMethodInfo(Func<object> owner, MethodInfo method)
        {
            GetOwner = owner;
            Method = method;
        }

        public override bool IsReadOnly() => false;
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler) { }
        public override object MemberValue { get { return null; } set { } }
    }
}
