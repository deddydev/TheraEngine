using System;
using System.IO;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Rendering.Cameras
{
    public class OrthographicCamera : Camera
    {
        public OrthographicCamera() : base() { }

        public override float Width { get { return Math.Abs(_orthoRight - _orthoLeft); } }
        public override float Height { get { return Math.Abs(_orthoTop - _orthoBottom); } }

        public override Vec2 Origin { get { return new Vec2(_originX, _originY); } }

        private float 
            _orthoLeft = 0.0f, 
            _orthoRight = 1.0f, 
            _orthoBottom = 0.0f, 
            _orthoTop = 1.0f;
        private float
            _orthoLeftPercentage = 0.0f,
            _orthoRightPercentage = 1.0f, 
            _orthoBottomPercentage = 0.0f, 
            _orthoTopPercentage = 1.0f;
        private float
            _originX,
            _originY;
        private float 
            _originXPercentage,
            _originYPercentage;

        public void SetCenteredStyle() { SetOriginPercentages(0.5f, 0.5f); }
        public void SetGraphStyle() { SetOriginPercentages(0.0f, 0.0f); }
        public void SetOriginPercentages(float xPercentage, float yPercentage)
        {
            _originXPercentage = xPercentage;
            _originYPercentage = yPercentage;
            _orthoLeftPercentage = 0.0f - xPercentage;
            _orthoRightPercentage = 1.0f - xPercentage;
            _orthoBottomPercentage = 0.0f - yPercentage;
            _orthoTopPercentage = 1.0f - yPercentage;
        }
        public override void Zoom(float amount)
        {
            float scale = amount >= 0 ? amount : 1.0f / -amount;

            _transform = _transform * Matrix4.CreateScale(scale);
            _invTransform = Matrix4.CreateScale(-scale) * _invTransform;
            UpdateTransformedFrustum();
            OnTransformChanged();
        }
        protected override void CalculateProjection()
        {
            base.CalculateProjection();
            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(_orthoLeft, _orthoRight, _orthoBottom, _orthoTop, _nearZ, _farZ);
            _projectionInverse = Matrix4.CreateInverseOrthographicOffCenter(_orthoLeft, _orthoRight, _orthoBottom, _orthoTop, _nearZ, _farZ);
        }
        public void SetProjectionParams(float farz, float nearz)
        {
            _farZ = farz;
            _nearZ = nearz;
            CalculateProjection();
        }
        public override void Resize(float width, float height)
        {
            _orthoLeft = _orthoLeftPercentage * width;
            _orthoRight = _orthoRightPercentage * width;
            _orthoBottom = _orthoBottomPercentage * height;
            _orthoTop = _orthoTopPercentage * height;
            _originX = _originXPercentage * width;
            _originY = _originYPercentage * height;
            base.Resize(width, height);
        }
        protected Vec3 AlignScreenPoint(Vec3 screenPoint)
        {
            return new Vec3(screenPoint.X + _orthoLeft, screenPoint.Y + _orthoBottom, screenPoint.Z);
        }
        protected Vec3 UnAlignScreenPoint(Vec3 screenPoint)
        {
            return new Vec3(screenPoint.X - _orthoLeft, screenPoint.Y - _orthoBottom, screenPoint.Z);
        }
        protected override Frustum CreateUntransformedFrustum()
        {
            float w = Width / 2.0f, h = Height / 2.0f;
            return new BoundingBox(new Vec3(-w, -h, -_farZ), new Vec3(w, h, -_nearZ)).AsFrustum();
        }

        public override float DistanceScale(Vec3 point, float radius)
        {
            return _scale.X * 80.0f;
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
