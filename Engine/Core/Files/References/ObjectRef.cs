using System;
using System.Collections.Generic;
using TheraEngine.ComponentModel;

namespace TheraEngine.Core.Files.References
{
    public class ObjectRef<T> where T : TObject
    {
        public static Dictionary<Guid, TObject> ObjectCache { get; } = new Dictionary<Guid, TObject>();

        [TSerialize]
        private Guid _refGuid;
        private readonly WeakReference<T> _objectCache = new WeakReference<T>(null);
        public bool TryGetRef(out T value)
        {
            bool valid = _objectCache.TryGetTarget(out value);
            if (!valid)
            {
                valid = ObjectCache.ContainsKey(_refGuid);
                if (valid)
                {
                    TObject tobj = ObjectCache[_refGuid];
                    valid = tobj is T;
                    if (valid)
                    {
                        value = (T)tobj;
                        _objectCache.SetTarget(value);
                    }
                }
            }
            return valid;
        }
        public void SetRef(T value)
        {
            _objectCache.SetTarget(value);
            _refGuid = value?.Guid ?? Guid.Empty;
        }
    }
}
