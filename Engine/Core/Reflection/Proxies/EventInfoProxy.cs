using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class EventInfoProxy : MemberInfoProxy
    {
        public static ConcurrentDictionary<EventInfo, EventInfoProxy> Proxies { get; }
            = new ConcurrentDictionary<EventInfo, EventInfoProxy>();
        public static EventInfoProxy Get(EventInfo info)
            => info is null ? null : Proxies.GetOrAdd(info, new EventInfoProxy(info));
        public static implicit operator EventInfoProxy(EventInfo info) => Get(info);
        public static implicit operator EventInfo(EventInfoProxy proxy) => proxy.Value;

        private EventInfo Value { get; set; }

        //public EventInfoProxy() { }
        private EventInfoProxy(EventInfo value) : base(value) => Value = value;

        public override bool IsGridViewable => Value.RaiseMethod.IsPublic;
        public override bool IsGridWriteable => Value.AddMethod.IsPublic;

        //
        // Summary:
        //     Gets a value indicating whether the EventInfo has a name with a special meaning.
        //
        // Returns:
        //     true if this event has a special name; otherwise, false.
        public bool IsSpecialName => Value.IsSpecialName;
        //
        // Summary:
        //     Gets the Type object of the underlying event-handler delegate associated with
        //     this event.
        //
        // Returns:
        //     A read-only Type object representing the delegate event handler.
        //
        // Exceptions:
        //   T:System.Security.SecurityException:
        //     The caller does not have the required permission.
        public TypeProxy EventHandlerType => Value.EventHandlerType;
        //
        // Summary:
        //     Gets the method that is called when the event is raised, including non-public
        //     methods.
        //
        // Returns:
        //     The method that is called when the event is raised.
        public MethodInfoProxy RaiseMethod => Value.RaiseMethod;
        //
        // Summary:
        //     Gets the MethodInfo object for removing a method of the event, including non-public
        //     methods.
        //
        // Returns:
        //     The MethodInfo object for removing a method of the event.
        public MethodInfoProxy RemoveMethod => Value.RemoveMethod;
        //
        // Summary:
        //     Gets the System.Reflection.MethodInfo object for the System.Reflection.EventInfo.AddEventHandler(System.Object,System.Delegate)
        //     method of the event, including non-public methods.
        //
        // Returns:
        //     The System.Reflection.MethodInfo object for the System.Reflection.EventInfo.AddEventHandler(System.Object,System.Delegate)
        //     method.
        public MethodInfoProxy AddMethod => Value.AddMethod;
        //
        // Summary:
        //     Gets the attributes for this event.
        //
        // Returns:
        //     The read-only attributes for this event.
        public EventAttributes Attributes => Value.Attributes;
        //
        // Summary:
        //     Gets a value indicating whether the event is multicast.
        //
        // Returns:
        //     true if the delegate is an instance of a multicast delegate; otherwise, false.
        //
        // Exceptions:
        //   T:System.Security.SecurityException:
        //     The caller does not have the required permission.
        public bool IsMulticast => Value.IsMulticast;
    }
}
