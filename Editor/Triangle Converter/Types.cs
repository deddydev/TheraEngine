//This has been converted from C++ and modified.
//Original source:
//http://users.telenet.be/tfautre/softdev/tristripper/

using System;
using System.Collections.Generic;

namespace Editor.TriangleConverter
{
    public class Triangle
    {
        public Triangle() { }
        public Triangle(uint A, uint B, uint C)
        {
            _a = A;
            _b = B;
            _c = C;
            _stripID = 0;
        }

        public void ResetStripID() { _stripID = 0; }
        public void SetStripID(uint StripID) { _stripID = StripID; }
        public uint StripID { get { return _stripID; } }

        public uint A { get { return _a; } }
        public uint B { get { return _b; } }
        public uint C { get { return _c; } }
        
        private uint _a;
        private uint _b;
        private uint _c;

        private uint _stripID;
        public uint _index;
    }

    public class TriangleEdge
    {
        public TriangleEdge(uint A, uint B) { m_A = A; m_B = B; }

        public uint A { get { return m_A; } }
        public uint B { get { return m_B; } }

        public static bool operator ==(TriangleEdge left, TriangleEdge right)
        {
            return ((left.A == right.A) && (left.B == right.B));
        }
        public static bool operator !=(TriangleEdge left, TriangleEdge right)
        {
            return ((left.A != right.A) || (left.B != right.B));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TriangleEdge))
                return false;

            return this == obj as TriangleEdge;
        }

        public override int GetHashCode()
        {
            return m_A.GetHashCode() ^ m_B.GetHashCode();
        }

        public uint m_A;
        public uint m_B;

        public override string ToString()
        {
            return String.Format("{0} {1}", m_A, m_B);
        }
    }

    public class TriEdge : TriangleEdge
    {
        public TriEdge(uint A, uint B, uint TriPos) : base(A, B) { m_TriPos = TriPos; }
        public uint TriPos { get { return m_TriPos; } }
        private uint m_TriPos;

        public static bool operator ==(TriEdge left, TriEdge right)
        {
            return ((left.A == right.A) && (left.B == right.B));
        }
        public static bool operator !=(TriEdge left, TriEdge right)
        {
            return ((left.A != right.A) || (left.B != right.B));
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TriEdge))
                return false;

            return this == obj as TriEdge;
        }

        public override int GetHashCode()
        {
            return m_A.GetHashCode() ^ m_B.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", m_A, m_B, m_TriPos);
        }
    }

    public enum TriOrder { ABC, BCA, CAB };
    public class Strip
    {
        public Strip()
        {
            _start = uint.MaxValue;
            _order = TriOrder.ABC;
            _size = 0;
        }

        public Strip(uint Start, TriOrder Order, uint Size)
        {
            _start = Start;
            _order = Order;
            _size = Size;
        }

        public uint Start { get { return _start; } }
        public TriOrder Order { get { return _order; } }
        public uint Size { get { return _size; } }

        private uint _start;
        private TriOrder _order;
        private uint _size;
    }
}
