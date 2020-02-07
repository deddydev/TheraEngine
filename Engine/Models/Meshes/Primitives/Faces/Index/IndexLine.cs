using Extensions;
using System;
using TheraEngine.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public class IndexLine : TObject, ISerializableString
    {
        public IndexLine() { }
        public IndexLine(IndexPoint point1, IndexPoint point2)
        {
            _point0 = point1;
            _point1 = point2;

            //_point0.AddLine(this);
            //_point1.AddLine(this);
        }

        [TSerialize("Point0")]
        private IndexPoint _point0;
        [TSerialize("Point1")]
        private IndexPoint _point1;

        //public List<IndexPrimitive> _connectedFaces = new List<IndexPrimitive>();

        public IndexPoint Point0 => _point0;
        public IndexPoint Point1 => _point1;

        //public void Unlink()
        //{
        //    _point0.RemoveLine(this);
        //    _point1.RemoveLine(this);
        //}

        //internal void AddFace(IndexPrimitive poly)
        //{
        //    if (!_connectedFaces.Contains(poly))
        //        _connectedFaces.Add(poly);
        //}

        //internal void RemoveFace(IndexPrimitive poly)
        //{
        //    if (_connectedFaces.Contains(poly))
        //        _connectedFaces.Remove(poly);
        //}

        public string WriteToString()
            => $"{Point0.WriteToString()} {Point1.WriteToString()}";
        public void ReadFromString(string str)
        {
            string[] indices = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            _point0 = new IndexPoint();
            _point0.ReadFromString(indices.IndexInRange(0) ? indices[0] : null);

            _point1 = new IndexPoint();
            _point1.ReadFromString(indices.IndexInRange(1) ? indices[1] : null);
        }
    }
}
