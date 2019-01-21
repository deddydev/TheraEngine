using System;
using System.ComponentModel;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridMemberInfoEvent : PropGridMemberInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string MemberAccessor => "." +Event.Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => Event?.Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public EventInfo Event { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Event?.EventHandlerType;

        public PropGridMemberInfoEvent(IPropGridMemberOwner owner, EventInfo eventInfo) : base(owner)
        {
            Event = eventInfo;
        }
        
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler) { }
        public override object MemberValue { get { return null; } set { } }
    }
}
