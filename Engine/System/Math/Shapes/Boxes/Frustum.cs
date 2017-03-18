using CustomEngine;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering;

namespace System
{
    public class Frustum : IRenderable, IEnumerable<Plane>
    {
        private static List<Frustum> ActiveFrustums = new List<Frustum>();
        private string _renderName;

        public Frustum()
        {
            _renderName = "frustum" + ActiveFrustums.Count;
            ActiveFrustums.Add(this);
        }
        public Frustum(
           Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
           Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight,
           Vec3 sphereCenter) : this()
        {
            _boundingSphere = new Sphere() { RenderSolid = false };
            UpdatePoints(
                farBottomLeft, farBottomRight, farTopLeft, farTopRight,
                nearBottomLeft, nearBottomRight, nearTopLeft, nearTopRight,
                sphereCenter);
        }
        ~Frustum()
        {
            ActiveFrustums.Remove(this);
        }

        public static bool UseBoundingSphere = true;

        private bool _isRendering = false;

        private Octree.Node _renderNode;

        //For quickly testing if objects in large scenes should even be tested against the frustum at all
        private Sphere _boundingSphere;

        private Plane[] _planes = new Plane[6];
        private Vec3[] _points = new Vec3[8];

        public Vec3 FarBottomLeft => _points[0];
        public Vec3 FarBottomRight => _points[1];

        public Vec3 FarTopLeft => _points[2];
        public Vec3 FarTopRight => _points[3];

        public Vec3 NearBottomLeft => _points[4];
        public Vec3 NearBottomRight => _points[5];

        public Vec3 NearTopLeft => _points[6];
        public Vec3 NearTopRight => _points[7];

        public Plane Near => _planes[0];
        public Plane Far => _planes[1];

        public Plane Left => _planes[2];
        public Plane Right => _planes[3];

        public Plane Top => _planes[4];
        public Plane Bottom => _planes[5];

        public IEnumerable<Vec3> Points => _points;
        public Shape CullingVolume => _boundingSphere;

        public bool IsRendering
        {
            get => _isRendering;
            set => _isRendering = value;
        }
        public Octree.Node RenderNode
        {
            get => _renderNode;
            set => _renderNode = value;
        }

        public void UpdatePoints(
            Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
            Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight, 
            Vec3 sphereCenter)
        {
            _points[0] = farBottomLeft;
            _points[1] = farBottomRight;
            _points[2] = farTopLeft;
            _points[3] = farTopRight;
            _points[4] = nearBottomLeft;
            _points[5] = nearBottomRight;
            _points[6] = nearTopLeft;
            _points[7] = nearTopRight;

            _boundingSphere.Center = sphereCenter;
            _boundingSphere.Radius = sphereCenter.DistanceToFast(farTopRight);

            //near, far
            _planes[0] = new Plane(nearBottomRight, nearBottomLeft, nearTopRight);
            _planes[1] = new Plane(farBottomLeft, farBottomRight, farTopLeft);

            //left, right
            _planes[2] = new Plane(nearBottomLeft, farBottomLeft, nearTopLeft);
            _planes[3] = new Plane(farBottomRight, nearBottomRight, farTopRight);

            //top, bottom
            _planes[4] = new Plane(farTopLeft, farTopRight, nearTopLeft);
            _planes[5] = new Plane(nearBottomLeft, nearBottomRight, farBottomLeft);
        }
        public void TransformedVersionOf(Frustum other, Matrix4 transform)
            => UpdatePoints(
                other.FarBottomLeft * transform,
                other.FarBottomRight * transform,
                other.FarTopLeft * transform,
                other.FarTopRight * transform,
                other.NearBottomLeft * transform,
                other.NearBottomRight * transform,
                other.NearTopLeft * transform,
                other.NearTopRight * transform,
                other._boundingSphere.Center * transform);

        public void TransformBy(Matrix4 transform)
            => UpdatePoints(
                FarBottomLeft * transform,
                FarBottomRight * transform,
                FarTopLeft * transform,
                FarTopRight * transform,
                NearBottomLeft * transform,
                NearBottomRight * transform,
                NearTopLeft * transform,
                NearTopRight * transform,
                _boundingSphere.Center * transform);

        public Frustum TransformedBy(Matrix4 transform)
            => new Frustum(
                FarBottomLeft * transform,
                FarBottomRight * transform,
                FarTopLeft * transform,
                FarTopRight * transform,
                NearBottomLeft * transform,
                NearBottomRight * transform,
                NearTopLeft * transform,
                NearTopRight * transform,
                _boundingSphere.Center * transform);
        
        public EContainment Contains(Box box) 
            => Collision.FrustumContainsBox1(this, box.HalfExtents, box.WorldMatrix);
        public EContainment Contains(BoundingBox box) 
            => Collision.FrustumContainsAABB(this, box.Minimum, box.Maximum);
        public EContainment Contains(Sphere sphere) 
            => Collision.FrustumContainsSphere(this, sphere.Center, sphere.Radius);
        public EContainment Contains(CapsuleY capsule)
        {
            Sphere top = capsule.GetTopSphere();
            Sphere bottom = capsule.GetBottomSphere();
            EContainment ct = Contains(top);
            EContainment cb = Contains(bottom);

            if (ct == EContainment.Contains && cb == EContainment.Contains)
                return EContainment.Contains;

            if (ct == EContainment.Intersects || cb == EContainment.Intersects)
                return EContainment.Intersects;

            if ((ct == EContainment.Disjoint && cb == EContainment.Contains) ||
                (ct == EContainment.Contains && cb == EContainment.Disjoint))
                return EContainment.Intersects;

            //TODO: test for when both spheres lie outside, but the area between interects the frustum
            return EContainment.Disjoint;
        }
        public IEnumerator<Plane> GetEnumerator() => ((IEnumerable<Plane>)_planes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Plane>)_planes).GetEnumerator();

        public void Render()
        {
            _boundingSphere.Render();

            float LineSize = 10.0f;
            Color NearColor = Color.Orange;
            Color FarColor = Color.DarkRed;
            Color SideColor = Color.LightGreen;

            Engine.Renderer.RenderLine(_renderName + "TopLeft", NearTopLeft, FarTopLeft, SideColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "TopRight", NearTopRight, FarTopRight, SideColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "BottomLeft", NearBottomLeft, FarBottomLeft, SideColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "BottomRight", NearBottomRight, FarBottomRight, SideColor, LineSize);

            Engine.Renderer.RenderLine(_renderName + "TopNear", NearTopLeft, NearTopRight, NearColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "BottomNear", NearBottomLeft, NearBottomRight, NearColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "LeftNear", NearBottomLeft, NearTopLeft, NearColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "RightNear", NearBottomRight, NearTopRight, NearColor, LineSize);

            Engine.Renderer.RenderLine(_renderName + "TopFar", FarTopLeft, FarTopRight, FarColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "BottomFar", FarBottomLeft, FarBottomRight, FarColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "LeftFar", FarBottomLeft, FarTopLeft, FarColor, LineSize);
            Engine.Renderer.RenderLine(_renderName + "RightFar", FarBottomRight, FarTopRight, FarColor, LineSize);
        }

        public Frustum HardCopy()
            => new Frustum(
                FarBottomLeft, FarBottomRight, FarTopLeft, FarTopRight,
                NearBottomLeft, NearBottomRight, NearTopLeft, NearTopRight,
                _boundingSphere.Center);
    }
}
