using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class EventInfoProxy : MemberInfoProxy
    {
        public static ConcurrentDictionary<EventInfo, EventInfoProxy> Proxies { get; }
            = new ConcurrentDictionary<EventInfo, EventInfoProxy>();
        public static EventInfoProxy Get(EventInfo info)
            => info == null ? null : Proxies.GetOrAdd(info, new EventInfoProxy(info));
        public static implicit operator EventInfoProxy(EventInfo info) => Get(info);
        public static implicit operator EventInfo(EventInfoProxy proxy) => proxy.Value;

        private EventInfo Value { get; set; }

        //public EventInfoProxy() { }
        private EventInfoProxy(EventInfo value) : base(value) => Value = value;

        public bool IsSpecialName => Value.IsSpecialName;

        public TypeProxy EventHandlerType => Value.EventHandlerType;
    }
}
