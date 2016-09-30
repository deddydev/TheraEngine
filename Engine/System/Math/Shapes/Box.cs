using OpenTK;
using CustomEngine;

namespace System
{
    public class Box : IRenderable
    {
        public Vector3 _min, _max;
        
        public Vector3 Minimum { get { return _min; } set { _min = value; CheckValid(); } }
        public Vector3 Maximum { get { return _max; } set { _max = value; CheckValid(); } }

        public Box(float halfZ, float halfY, float halfX)
        {
            _max = new Vector3(halfX, halfY, halfZ);
            _min = -_max;
            CheckValid();
        }
        public Box(Vector3 min, Vector3 max)
        {
            _min = min;
            _max = max;
            CheckValid();
        }
        public Box(Vector3 bounds)
        {
            _min = -bounds / 2.0f;
            _max = bounds / 2.0f;
            CheckValid();
        }
        private void CheckValid()
        {
            if (_min.X > _max.X)
                MathHelper.Swap(ref _min.X, ref _max.X);
            if (_min.Y > _max.Y)
                MathHelper.Swap(ref _min.Y, ref _max.Y);
            if (_min.Z > _max.Z)
                MathHelper.Swap(ref _min.Z, ref _max.Z);
        }
        public bool ContainsPoint(Vector3 point)
        {
            return point <= _max && point >= _min;
        }
        public ContainsShape ContainsBox(Box box)
        {
            return ContainsShape.No;
        }
        /// <summary>
        /// T = top, B = bottom
        /// B = back, F = front
        /// L = left,  R = right
        /// </summary>
        public void GetCorners(
            out Vector3 TBL,
            out Vector3 TBR,
            out Vector3 TFL,
            out Vector3 TFR,
            out Vector3 BBL,
            out Vector3 BBR,
            out Vector3 BFL,
            out Vector3 BFR)
        {
            float Top = _max.Z;
            float Bottom = _min.Z;
            float Front = _max.Y;
            float Back = _min.Y;
            float Right = _max.X;
            float Left = _min.X;

            TBL = new Vector3(Top, Back, Left);
            TBR = new Vector3(Top, Back, Right);

            TFL = new Vector3(Top, Front, Left);
            TFR = new Vector3(Top, Front, Right);

            BBL = new Vector3(Bottom, Back, Left);
            BBR = new Vector3(Bottom, Back, Right);

            BFL = new Vector3(Bottom, Front, Left);
            BFR = new Vector3(Bottom, Front, Right);
        }
        public void ExpandBounds(Vector3 point)
        {
            _min.SetLequalTo(point);
            _max.SetGequalTo(point);
        }
        public void Render() { Render(false); }
        public void Render(bool solid)
        {
            if (solid)
                Engine.Renderer.DrawBoxSolid(this);
            else
                Engine.Renderer.DrawBoxWireframe(this);
        }
    }
}
