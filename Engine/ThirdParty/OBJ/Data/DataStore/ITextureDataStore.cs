using System;
using TheraEngine.Core.Maths.Transforms;

namespace ObjLoader.Loader.Data.DataStore
{
    public interface ITextureDataStore
    {
        void AddTexture(Vec2 texture);
    }
}