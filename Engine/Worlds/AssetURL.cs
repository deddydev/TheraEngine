using System.Collections.Generic;
using TheraEngine.Components;

namespace TheraEngine.Worlds
{
    public class ComponentURL<T> where T : Component
    {
        private object _resolvedObject;
        //private WorldFileRef _worldFilePath;
        private EventList<string> _memberPath;
        
        public bool CacheObject { get; set; }
        //public object ResolvedObject
        //{
        //    get
        //    {
        //        if (!CacheObject || _resolvedObject == null)
        //            ResolveObject(out _resolvedObject, true);
        //        return _resolvedObject;
        //    }
        //}
        //public WorldReference WorldFilePath
        //{
        //    get => _worldFilePath;
        //    set
        //    {
        //        _worldFilePath = value;
        //        _resolvedObject = null;
        //    }
        //}
        public EventList<string> MemberPath
        {
            get => _memberPath;
            set
            {
                if (_memberPath != null)
                {
                    _memberPath.PostAnythingAdded -= _memberPath_PostAnythingAdded;
                    _memberPath.PostAnythingRemoved -= _memberPath_PostAnythingRemoved;
                }
                _memberPath = value;
                _memberPath.PostAnythingAdded += _memberPath_PostAnythingAdded;
                _memberPath.PostAnythingRemoved += _memberPath_PostAnythingRemoved;
                _resolvedObject = null;
            }
        }

        private void _memberPath_PostAnythingRemoved(string item) => _resolvedObject = null;
        private void _memberPath_PostAnythingAdded(string item) => _resolvedObject = null;

        public enum EAssetResolveError
        {
            NoReferenceExists,
            WorldNotLoaded,
            InvalidPath,
            NullMember,
            NoError,
        }
        //public EAssetResolveError ResolveObject(out object asset, bool onlyIfWorldLoaded = true)
        //{
        //    asset = null;
        //    var fref = WorldFilePath.GetFileRef();
        //    if (fref == null)
        //        return EAssetResolveError.NoReferenceExists;

        //    if (onlyIfWorldLoaded && !fref.IsLoaded)
        //        return EAssetResolveError.WorldNotLoaded;

        //    TWorld world = fref.File;
        //    Type currentType = world.GetType();
        //    object currentObject = world;
        //    string member = null;
        //    for (int i = 0; i < MemberPath.Count; ++i)
        //    {
        //        if (currentObject != null)
        //        {
        //            member = MemberPath[i];

        //            var field = currentType.GetField(member);
        //            if (field != null)
        //            {
        //                currentObject = field.GetValue(currentObject);
        //                currentType = currentObject?.GetType() ?? field.FieldType;
        //                continue;
        //            }
        //            else
        //            {
        //                var property = currentType.GetProperty(member);
        //                if (property != null)
        //                {
        //                    currentObject = property.GetValue(currentObject);
        //                    currentType = currentObject?.GetType() ?? property.PropertyType;
        //                    continue;
        //                }
        //            }

        //            Engine.LogWarning($"No member exists at {string.Join(".", MemberPath)}; {member} is not valid.");
        //            return EAssetResolveError.InvalidPath;
        //        }
        //        else
        //        {
        //            Engine.LogWarning($"No member exists at {string.Join(".", MemberPath)}; {member} is null.");
        //            return EAssetResolveError.NullMember;
        //        }
        //    }

        //    asset = currentObject;
        //    return EAssetResolveError.NoError;
        //}
    }
}
