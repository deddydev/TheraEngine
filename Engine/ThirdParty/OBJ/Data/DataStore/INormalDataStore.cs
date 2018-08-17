using System;
using TheraEngine.Core.Maths.Transforms;

namespace ObjLoader.Loader.Data.DataStore
{
    public interface INormalDataStore
    {
        void AddNormal(Vec3 normal);
    }
}