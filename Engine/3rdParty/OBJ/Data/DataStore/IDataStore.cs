using System.Collections.Generic;
using ObjLoader.Loader.Data.Elements;
using System;

namespace ObjLoader.Loader.Data.DataStore
{
    public interface IDataStore 
    {
        IList<Vec3> Vertices { get; }
        IList<Vec2> TexCoords { get; }
        IList<Vec3> Normals { get; }
        IList<Material> Materials { get; }
        IList<Group> Groups { get; }
    }
}