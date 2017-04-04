using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CustomEngine.Files;
using System.Xml;
using System.IO;

namespace System
{
    public class Circle : Plane
    {
        public Circle() : base() { _radius = 1.0f; }
        public Circle(float radius, Vec3 normal, float distance) 
            : base(normal, distance)  { _radius = radius; }
        public Circle(float radius, Vec3 point)
            : base(point) { _radius = radius; }
        public Circle(float radius, Vec3 point, Vec3 normal) 
            : base(point, normal) { _radius = radius; }
        public Circle(float radius, Vec3 point0, Vec3 point1, Vec3 point2)
            : base(point0, point1, point2) { _radius = radius; }

        [Serialize("Radius", IsXmlAttribute = true)]
        private float _radius;
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        public PrimitiveData GetSolidMesh(int sides)
            => SolidMesh(Radius, Normal, Point, sides);
        public PrimitiveData GetWireframeMesh(int sides)
            => WireframeMesh(Radius, Normal, Point, sides);
        public VertexLineStrip GetLineStrip(int sides)
            => LineStrip(Radius, Normal, Point, sides);
        
        public static PrimitiveData SolidMesh(float radius, Vec3 normal, Vec3 center, int sides)
        {
            if (sides < 3)
                throw new Exception("A (very low res) circle needs at least 3 sides.");

            normal.Normalize();
            Quat offset = Quat.BetweenVectors(Vec3.Up, normal);
            Vertex[] points = new Vertex[sides + 1];
            points[0] = new Vertex(center, normal);
            float angleInc = CustomMath.PIf * 2.0f / sides, angle = 0.0f;
            for (int i = 1; i < sides; ++i, angle += angleInc)
            {
                Vec3 v = new Vec3((float)Math.Cos(angle), 0.0f, (float)Math.Sin(angle));
                points[i] = new Vertex(center + offset * (radius * v), normal);
            }
            Vec3[] positions = Points(radius, normal, center, sides);
            VertexTriangleFan fan = new VertexTriangleFan();
            return PrimitiveData.FromTriangleFans(Culling.None, new PrimitiveBufferInfo(), fan);
        }
        public static PrimitiveData WireframeMesh(float radius, Vec3 normal, Vec3 center, int sides)
        {
            return PrimitiveData.FromLineStrips(new PrimitiveBufferInfo(), LineStrip(radius, normal, center, sides));
        }
        public static PrimitiveData HalfCircleWireframeMesh(float radius, Vec3 upNormal, Vec3 rightNormal, Vec3 center, int sides)
        {
            return PrimitiveData.FromLineStrips(new PrimitiveBufferInfo(), HalfCircleLineStrip(radius, upNormal, rightNormal, center, sides));
        }
        public static VertexLineStrip LineStrip(float radius, Vec3 normal, Vec3 center, int sides)
        {
            Vec3[] points = Points(radius, normal, center, sides);
            return new VertexLineStrip(true, points.Select(x => new Vertex(x)).ToArray());
        }
        public static VertexLineStrip HalfCircleLineStrip(float radius, Vec3 upNormal, Vec3 rightNormal, Vec3 center, int sides)
        {
            Vec3[] points = HalfCirclePoints(radius, upNormal, rightNormal, center, sides);
            return new VertexLineStrip(false, points.Select(x => new Vertex(x)).ToArray());
        }
        public static Vec3[] Points(float radius, Vec3 normal, Vec3 center, int sides)
        {
            if (sides < 3)
                throw new Exception("A (very low res) circle needs at least 3 sides.");

            normal.NormalizeFast();
            Quat offset = Quat.BetweenVectors(Vec3.Up, normal);
            Vec3[] points = new Vec3[sides];
            float angleInc = CustomMath.PIf * 2.0f / sides;
            float angle = 0.0f;
            for (int i = 0; i < sides; ++i, angle += angleInc)
            {
                Vec3 v = new Vec3((float)Math.Cos(angle), 0.0f, (float)Math.Sin(angle));
                points[i] = center + offset * (radius * v);
            }
            return points;
        }
        public static Vec3[] HalfCirclePoints(float radius, Vec3 upNormal, Vec3 rightNormal, Vec3 center, int sides)
        {
            if (sides < 3)
                throw new Exception("A (very low res) circle needs at least 3 sides.");

            if (upNormal == rightNormal || upNormal == -rightNormal)
                throw new Exception("Normals for half circle cannot be colinear/parallel.");

            sides += 1;
            Vec3 forwardNormal = upNormal ^ rightNormal;
            Quat pitch = Quat.BetweenVectors(Vec3.Up, upNormal);
            Quat roll = Quat.BetweenVectors(Vec3.Right, rightNormal);
            Quat yaw = Quat.BetweenVectors(Vec3.Forward, forwardNormal);
            Quat offset = roll * pitch * yaw;
            Vec3[] points = new Vec3[sides];
            float angleInc = CustomMath.PIf / sides;
            float angle = 0.0f;
            for (int i = 0; i < sides; ++i, angle += angleInc)
            {
                Vec3 v = new Vec3((float)Math.Cos(angle), 0.0f, (float)Math.Sin(angle));
                points[i] = center + offset * (radius * v);
            }
            return points;
        }

        //public new const string XMLTag = "circle";
        //
        //protected override int OnCalculateSize(StringTable table)
        //    => Header.Size;
        //public unsafe override void Write(VoidPtr address, StringTable table)
        //    => *(Header*)address = this;
        //public unsafe override void Read(VoidPtr address, VoidPtr strings)
        //{
        //    base.Read(address, strings);
        //    Header h = *(Header*)address;
        //    _radius = h._radius;
        //}
        //public override void Write(XmlWriter writer)
        //{
        //    writer.WriteStartElement(XMLTag);
        //    writer.WriteAttributeString("radius", _radius.ToString());
        //    writer.WriteElementString("normal", _normal.ToString(false, false));
        //    writer.WriteElementString("distance", _distance.ToString());
        //    //writer.WriteElementString("point", Point.ToString(false, false));
        //    writer.WriteEndElement();
        //}
        //public override void Read(XMLReader reader)
        //{
        //    if (!reader.Name.Equals(XMLTag, true))
        //        throw new Exception();
        //    while (reader.ReadAttribute())
        //    {
        //        if (reader.Name.Equals("radius", true))
        //            _radius = float.Parse((string)reader.Value);
        //    }
        //    while (reader.BeginElement())
        //    {
        //        if (reader.Name.Equals("normal", true))
        //            Normal = Vec3.Parse(reader.ReadElementString());
        //        else if (reader.Name.Equals("distance", true))
        //            _distance = float.Parse(reader.ReadElementString());
        //        //else if (reader.Name.Equals("point", true))
        //        //    Point = Vec3.Parse(reader.ReadElementString());
        //        reader.EndElement();
        //    }
        //}
        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public new struct Header
        //{
        //    public const int Size = Plane.Header.Size + 4;

        //    public Plane.Header _plane;
        //    public float _radius;

        //    public static implicit operator Header(Circle c)
        //    {
        //        return new Header()
        //        {
        //            _plane = c,
        //            _radius = c._radius,
        //        };
        //    }
        //    public static implicit operator Circle(Header h)
        //    {
        //        return new Circle(h._radius, h._plane._normal, h._plane._distance);
        //    }
        //}
    }
}
