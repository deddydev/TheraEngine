using System.Collections.Generic;
using System.Linq;
using ObjLoader.Loader.Data.Elements;
using System;
using TheraEngine.Core.Maths.Transforms;

namespace ObjLoader.Loader.Data.DataStore
{
    public class DataStore : IDataStore, IGroupDataStore, IVertexDataStore, ITextureDataStore, INormalDataStore,
                             IFaceGroup, IMaterialLibrary, IElementGroup
    {
        private Group _currentGroup;
        
        private readonly List<Group> _groups = new List<Group>();
        private readonly List<Material> _materials = new List<Material>();

        private readonly List<Vec3> _vertices = new List<Vec3>();
        private readonly List<Vec2> _textures = new List<Vec2>();
        private readonly List<Vec3> _normals = new List<Vec3>();

        public IList<Vec3> Vertices
        {
            get { return _vertices; }
        }

        public IList<Vec2> TexCoords
        {
            get { return _textures; }
        }

        public IList<Vec3> Normals
        {
            get { return _normals; }
        }

        public IList<Material> Materials
        {
            get { return _materials; }
        }

        public IList<Group> Groups
        {
            get { return _groups; }
        }

        public void AddFace(Face face)
        {
            PushGroupIfNeeded();

            _currentGroup.AddFace(face);
        }

        public void PushGroup(string groupName)
        {
            _currentGroup = new Group(groupName);
            _groups.Add(_currentGroup);
        }

        private void PushGroupIfNeeded()
        {
            if (_currentGroup == null)
            {
                PushGroup("default");
            }
        }

        public void AddVertex(Vec3 vertex)
        {
            _vertices.Add(vertex);
        }

        public void AddTexture(Vec2 texture)
        {
            _textures.Add(texture);
        }

        public void AddNormal(Vec3 normal)
        {
            _normals.Add(normal);
        }

        public void Push(Material material)
        {
            _materials.Add(material);
        }

        public void SetMaterial(string materialName)
        {
            var material = _materials.SingleOrDefault(x => x.Name.EqualsOrdinal(materialName));
            PushGroupIfNeeded();
            _currentGroup.AddSubGroup(material);
        }
    }
}