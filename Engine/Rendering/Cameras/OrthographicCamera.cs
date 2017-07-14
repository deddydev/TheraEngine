using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Cameras
{
    public class OrthographicCamera : Camera
    {
        public OrthographicCamera() : base()
        {
            _scale.Changed += CreateTransform;
        }
        public OrthographicCamera(Vec3 scale, Vec3 point, Rotator rotation, Vec2 originPercentages, float nearZ, float farZ)
            : base(16.0f, 9.0f, nearZ, farZ, point, rotation)
        {
            _scale.SetRawNoUpdate(scale);
            _scale.Changed += CreateTransform;
            _originPercentages.Raw = originPercentages;
            _originPercentages.Changed += _originPercentages_Changed;
        }

        private void _originPercentages_Changed()
        {

        }

        public override float Width
        {
            get => Math.Abs(_orthoRight - _orthoLeft);
            set => Resize(value, Height);
        }

        public override float Height
        {
            get => Math.Abs(_orthoTop - _orthoBottom);
            set => Resize(Width, value);
        }

        public override Vec2 Origin => _origin;

        [Category("Orthographic Camera")]
        public EventVec3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _scale.Changed += CreateTransform;
                CreateTransform();
            }
        }

        [Serialize("Scale")]
        private EventVec3 _scale = Vec3.One;

        private float _orthoLeft = 0.0f;
        private float _orthoRight = 1.0f;
        private float _orthoBottom = 0.0f;
        private float _orthoTop = 1.0f;

        [Serialize("OrthoLeftPercentage")]
        private float _orthoLeftPercentage = 0.0f;
        [Serialize("OrthoRightPercentage")]
        private float _orthoRightPercentage = 1.0f;
        [Serialize("OrthoBottomPercentage")]
        private float _orthoBottomPercentage = 0.0f;
        [Serialize("OrthoTopPercentage")]
        private float _orthoTopPercentage = 1.0f;

        private Vec2 _origin;
        [Serialize("OriginPercentages")]
        private EventVec2 _originPercentages = Vec2.Zero;

        public void SetCenteredStyle() 
            => SetOriginPercentages(0.5f, 0.5f);
        public void SetGraphStyle() 
            => SetOriginPercentages(0.0f, 0.0f);

        public void SetOriginPercentages(Vec2 percentages)
            => SetOriginPercentages(percentages.X, percentages.Y);
        public void SetOriginPercentages(float xPercentage, float yPercentage)
        {
            _originPercentages.X = xPercentage;
            _originPercentages.Y = yPercentage;
            _orthoLeftPercentage = 0.0f - xPercentage;
            _orthoRightPercentage = 1.0f - xPercentage;
            _orthoBottomPercentage = 0.0f - yPercentage;
            _orthoTopPercentage = 1.0f - yPercentage;
        }
        public override void Zoom(float amount)
        {
            float scale = amount >= 0 ? amount : 1.0f / -amount;

            _cameraToWorldSpaceMatrix = _cameraToWorldSpaceMatrix * Matrix4.CreateScale(scale);
            _worldToCameraSpaceMatrix = Matrix4.CreateScale(-scale) * _worldToCameraSpaceMatrix;
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
            _origin = _originPercentages * width;
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
            return BoundingBox.FromMinMax(new Vec3(-w, -h, -_farZ), new Vec3(w, h, -_nearZ)).AsFrustum();
        }
        protected override void CreateTransform()
        {
            Matrix4 rotMatrix = _localRotation.GetMatrix();
            _cameraToWorldSpaceMatrix = Matrix4.CreateTranslation(_localPoint.Raw) * rotMatrix * Matrix4.CreateScale(_scale);
            _worldToCameraSpaceMatrix = Matrix4.CreateScale(1.0f / _scale) * _localRotation.GetInverseMatrix() * Matrix4.CreateTranslation(-_localPoint.Raw);
            OnTransformChanged();
        }
        public override float DistanceScale(Vec3 point, float radius)
        {
            return _scale.X * 80.0f;
        }
    }
}
