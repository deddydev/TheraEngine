using System;
using System.ComponentModel;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemRefEventInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => Event?.Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public EventInfo Event { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Func<object> GetOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Event?.EventHandlerType;

        public PropGridItemRefEventInfo(Func<object> owner, EventInfo @event)
        {
            GetOwner = owner;
            Event = @event;
        }

        public override bool IsReadOnly() => false;
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler) { }
        public override object MemberValue { get { return null; } set { } }
    }
}
