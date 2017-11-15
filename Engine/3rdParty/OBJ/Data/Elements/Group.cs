using System.Collections.Generic;
using ObjLoader.Loader.Data.DataStore;

namespace ObjLoader.Loader.Data.Elements
{
    public class Group : IFaceGroup
    {
        private readonly List<SubGroup> _subGroups = new List<SubGroup>();
        
        public Group(string name)
        {
            Name = name;
        }
        
        public string Name { get; private set; }

        public List<SubGroup> SubGroups => _subGroups;

        public void AddFace(Face face)
        {
            _subGroups[_subGroups.Count - 1].AddFace(face);
        }

        internal void AddSubGroup(Material material)
        {
            _subGroups.Add(new SubGroup(material));
        }
    }
    public class SubGroup : IFaceGroup
    {
        private readonly List<Face> _faces = new List<Face>();

        public SubGroup(Material material)
        {
            Material = material;
        }

        public Material Material { get; set; }
        public IList<Face> Faces { get { return _faces; } }

        public void AddFace(Face face)
        {
            _faces.Add(face);
        }
    }
}