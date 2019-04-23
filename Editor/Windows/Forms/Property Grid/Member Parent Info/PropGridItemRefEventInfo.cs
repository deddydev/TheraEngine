using System;
using System.ComponentModel;
using System.Reflection;
using TheraEngine.Core.Reflection;

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
        public EventInfoProxy Event { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override TypeProxy DataType => Event?.EventHandlerType;

        public PropGridMemberInfoEvent(IPropGridMemberOwner owner, EventInfoProxy eventInfo) : base(owner)
        {
            Event = eventInfo;
        }
        
        internal protected override void SubmitStateChange(object oldValue, object newValue, ValueChangeHandler dataChangeHandler) { }
        public override object MemberValue { get { return null; } set { } }
    }
}
