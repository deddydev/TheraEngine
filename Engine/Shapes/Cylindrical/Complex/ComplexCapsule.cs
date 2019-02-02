using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    public class ComplexCapsule : TShape
    {
        protected Vec3 _upAxis, _center;
        protected float _topRadius, _bottomRadius, _topHeight, _bottomHeight;

        public Vec3 Center
        {
            get => _center;
            set => _center = value;
        }
        public Vec3 UpAxis
        {
            get => _upAxis;
            set => _upAxis = value;
        }
        public float TopRadius
        {
            get => _topRadius;
            set => _topRadius = value;
        }
        public float BottomRadius
        {
            get => _bottomRadius;
            set => _bottomRadius = value;
        }
        public float TopHeight
        {
            get => _topHeight;
            set => _topHeight = value;
        }
        public float BottomHeight
        {
            get => _bottomHeight;
            set => _bottomHeight = value;
        }

        public ComplexCapsule(
            Vec3 center,
            Vec3 upAxis,
            float topRadius,
            float bottomRadius,
            float topHeight,
            float bottomHeight)
        {
            _topRadius = Math.Abs(topRadius);
            _bottomRadius = Math.Abs(bottomRadius);
            _upAxis = upAxis;
            _topHeight = topHeight;
            _bottomHeight = bottomHeight;
            _center = center;
        }
        public float GetTotalTopHeight() => _topHeight + _topRadius;
        public float GetTotalBottomHeight() => _bottomHeight + _bottomRadius;
        public float GetTotalHeight() => GetTotalTopHeight() + GetTotalBottomHeight();
        public override void Render()
        {
            //Engine.Renderer.RenderCapsule(_center, _upAxis, _topHeight, _topRadius, _bottomHeight, _bottomRadius, Matrix4.Identity, _renderSolid);
        }
        public Sphere GetTopSphere()
            => new Sphere(_topRadius, _upAxis * _topHeight);
        public Sphere GetBottomSphere()
            => new Sphere(_bottomRadius, _upAxis * -_bottomHeight);
        
        public override TCollisionShape GetCollisionShape()
        {
            throw new InvalidOperationException("Complex capsule cannot be used for physics.");
        }
        public override bool Contains(Vec3 point)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override TShape HardCopy()
            => new ComplexCapsule(Center, UpAxis, TopRadius, BottomRadius, TopHeight, BottomHeight);
        
        public override Vec3 ClosestPoint(Vec3 point)
        {
            throw new NotImplementedException();
        }

        public override BoundingBox GetAABB()
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(Cone cone)
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(Cylinder cylinder)
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(Capsule capsule)
        {
            throw new NotImplementedException();
        }

        public override void SetTransformMatrix(Matrix4 matrix)
        {
            throw new NotImplementedException();
        }

        public override Matrix4 GetTransformMatrix()
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(BoundingBoxStruct box)
        {
            throw new NotImplementedException();
        }
    }
}
