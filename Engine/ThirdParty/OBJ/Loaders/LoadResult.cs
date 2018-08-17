using System.Collections.Generic;
using ObjLoader.Loader.Data;
using ObjLoader.Loader.Data.Elements;
using System;
using TheraEngine.Core.Maths.Transforms;

namespace ObjLoader.Loader.Loaders
{
    public class LoadResult  
    {
        public IList<Vec3> Vertices { get; set; }
        public IList<Vec2> TexCoords { get; set; }
        public IList<Vec3> Normals { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Material> Materials { get; set; }
    }
}