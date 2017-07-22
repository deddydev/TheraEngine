using TheraEngine;
using System.Drawing;
using System.ComponentModel;

namespace System
{
    [FileClass("SHAPE", "Cone")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class BaseCone : Shape
    {
        public BaseCone(Vec3 center, Rotator rotation, Vec3 scale, Vec3 upAxis, float radius, float height)
        {
            _radius = Math.Abs(radius);
            _height = Math.Abs(height);
            _localUpAxis = upAxis;
            _localUpAxis.NormalizeFast();
            _state.Translation = center;
            _state.Rotation = rotation;
            _state.Scale = scale;
        }

        public FrameState State
        {
            get => _state;
            set => _state = value;
        }

        protected FrameState _state = FrameState.GetIdentity(TransformOrder.TRS, RotationOrder.YPR);
        
        protected Vec3 _localUpAxis = Vec3.Up;
        protected float _radius = 0.5f, _height = 1.0f;
        
        public Vec3 GetTopPoint()
            => Vec3.TransformPosition(_localUpAxis * (_height / 2.0f), _state.Matrix);
        public Vec3 GetBottomCenterPoint()
            => Vec3.TransformPosition(_localUpAxis * (_height / -2.0f), _state.Matrix);
        public Circle3D GetBottomCircle(bool normalFacingIn = false)
            => new Circle3D(_radius, GetBottomCenterPoint(), normalFacingIn ? WorldUpAxis : -WorldUpAxis);
        
        public Vec3 Center
        {
            get => _state.Translation;
            set => _state.Translation = value;
        }
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        public float Height
        {
            get => _height;
            set => _height = value;
        }

        public Vec3 LocalUpAxis => _localUpAxis;
        public Vec3 WorldUpAxis => _localUpAxis * _state.Matrix.GetRotationMatrix4();

        public override void SetRenderTransform(Matrix4 worldMatrix)
        {
            _state.Matrix = worldMatrix;
            base.SetRenderTransform(worldMatrix);
        }

        public override void Render()
            => Engine.Renderer.RenderCone(_state.Matrix, _localUpAxis, _radius, _height, _renderSolid, Color.Black);

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
        public override EContainment ContainedWithin(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Frustum frustum)
        {
            throw new NotImplementedException();
        }
        public override Shape HardCopy()
        {
            throw new NotImplementedException();
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            throw new NotImplementedException();
        }
        public override Matrix4 GetTransformMatrix()
        {
            throw new NotImplementedException();
        }
        public override Vec3 ClosestPoint(Vec3 point)
        {
            throw new NotImplementedException();
        }
    }
}
