using OpenTK;
using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;

namespace System
{
    public class Box : IShape
    {
        public Vec3 _min, _max;
        
        public Vec3 Minimum { get { return _min; } set { _min = value; CheckValid(); } }
        public Vec3 Maximum { get { return _max; } set { _max = value; CheckValid(); } }

        public Box(float halfZ, float halfY, float halfX)
        {
            _max = new Vec3(halfX, halfY, halfZ);
            _min = -_max;
            CheckValid();
        }
        public Box(Vec3 min, Vec3 max)
        {
            _min = min;
            _max = max;
            CheckValid();
        }
        public Box(Vec3 bounds)
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
        public bool ContainsPoint(Vec3 point)
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
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            float Top = _max.Z;
            float Bottom = _min.Z;
            float Front = _max.Y;
            float Back = _min.Y;
            float Right = _max.X;
            float Left = _min.X;

            TBL = new Vec3(Top, Back, Left);
            TBR = new Vec3(Top, Back, Right);

            TFL = new Vec3(Top, Front, Left);
            TFR = new Vec3(Top, Front, Right);

            BBL = new Vec3(Bottom, Back, Left);
            BBR = new Vec3(Bottom, Back, Right);

            BFL = new Vec3(Bottom, Front, Left);
            BFR = new Vec3(Bottom, Front, Right);
        }
        public void ExpandBounds(Vec3 point)
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
        public unsafe PrimitiveData GetPrimitives()
        {
            const string p = "Positions";
            const string n = "Normals";

            PrimitiveData data = new PrimitiveData();
            data[p] = new VertexBuffer(0, 8, VertexBuffer.ComponentType.Float, 3, false);
            data[n] = new VertexBuffer(0, 6, VertexBuffer.ComponentType.Float, 3, false);

            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;
            GetCorners(out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
            Vec3* dPtr = (Vec3*)(VoidPtr)data[p];
            *dPtr++ = TBL;
            *dPtr++ = TBR;
            *dPtr++ = TFL;
            *dPtr++ = TFR;
            *dPtr++ = BBL;
            *dPtr++ = BBR;
            *dPtr++ = BFL;
            *dPtr++ = BFR;
            dPtr = (Vec3*)(VoidPtr)data[n];


            Quad left = new Quad();



            left.ToTriangles();
        }
    }
}
