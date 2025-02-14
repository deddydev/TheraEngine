﻿using Extensions;
using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models
{
    public class IndexTriangle : IndexPolygon, ISerializableString
    {
        public override FaceType Type => FaceType.Triangles;

        public IndexPoint Point0 => _points[0];
        public IndexPoint Point1 => _points[1];
        public IndexPoint Point2 => _points[2];

        public IndexTriangle() { }
        /// <summary>
        /// Counter-Clockwise winding
        ///     2
        ///    / \
        ///   /   \
        ///  0-----1
        /// </summary>
        public IndexTriangle(IndexPoint point0, IndexPoint point1, IndexPoint point2)
        {
            _points.Add(point0);
            _points.Add(point1);
            _points.Add(point2);

            //IndexLine e01 = point0.LinkTo(point1);
            //IndexLine e12 = point1.LinkTo(point2);
            //IndexLine e20 = point2.LinkTo(point0);

            //e01.AddFace(this);
            //e12.AddFace(this);
            //e20.AddFace(this);
        }

        public override List<IndexTriangle> ToTriangles()
            => new List<IndexTriangle>() { this };

        public override bool Equals(object obj)
        {
            return !(obj is IndexTriangle t) ? false :
                t.Point0 == Point0 && t.Point1 == Point1 && t.Point2 == Point2;
        }
        public override int GetHashCode()
            => base.GetHashCode();

        public string WriteToString()
            => $"{Point0.WriteToString()} {Point1.WriteToString()} {Point2.WriteToString()}";
        public void ReadFromString(string str)
        {
            string[] indices = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            _points = new List<IndexPoint>() { new IndexPoint(), new IndexPoint(), new IndexPoint() };
            _points[0].ReadFromString(indices.IndexInRange(0) ? indices[0] : null);
            _points[1].ReadFromString(indices.IndexInRange(1) ? indices[1] : null);
            _points[2].ReadFromString(indices.IndexInRange(2) ? indices[2] : null);
        }
    }
}
