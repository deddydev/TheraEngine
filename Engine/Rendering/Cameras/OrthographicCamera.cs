using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Cameras
{
    public class OrthographicCamera : TypicalCamera
    {
        public OrthographicCamera() : base()
        {
            _scale.Changed += CreateTransform;
            _originPercentages.Changed += _originPercentages_Changed;
        }
        public OrthographicCamera(float nearZ, float farZ)
            : this(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Zero, nearZ, farZ) { }
        public OrthographicCamera(Vec3 scale, Vec3 point, Rotator rotation, Vec2 originPercentages, float nearZ, float farZ)
            : this(16.0f, 9.0f, scale, point, rotation, originPercentages, nearZ, farZ) { }
        public OrthographicCamera(float width, float height, Vec3 scale, Vec3 point, Rotator rotation, Vec2 originPercentages, float nearZ, float farZ)
           : base(width, height, nearZ, farZ, point, rotation)
        {
            _scale.SetRawNoUpdate(scale);
            _scale.Changed += CreateTransform;
            _originPercentages.Changed += _originPercentages_Changed;
            _originPercentages.Raw = originPercentages;
        }

        private void _originPercentages_Changed()
        {
            _orthoLeftPercentage = 0.0f - _originPercentages.X;
            _orthoRightPercentage = 1.0f - _originPercentages.X;
            _orthoBottomPercentage = 0.0f - _originPercentages.Y;
            _orthoTopPercentage = 1.0f - _originPercentages.Y;
            Resize(Width, Height);
        }

        public override float Width
        {
            get => Math.Abs(_orthoRight - _orthoLeft);
            //set => Resize(value, Height);
        }

        public override float Height
        {
            get => Math.Abs(_orthoTop - _orthoBottom);
            //set => Resize(Width, value);
        }

        [Browsable(false)]
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

        [TSerialize("Scale")]
        private EventVec3 _scale = Vec3.One;

        private float _orthoLeft = 0.0f;
        private float _orthoRight = 1.0f;
        private float _orthoBottom = 0.0f;
        private float _orthoTop = 1.0f;

        [TSerialize("OrthoLeftPercentage")]
        private float _orthoLeftPercentage = 0.0f;
        [TSerialize("OrthoRightPercentage")]
        private float _orthoRightPercentage = 1.0f;
        [TSerialize("OrthoBottomPercentage")]
        private float _orthoBottomPercentage = 0.0f;
        [TSerialize("OrthoTopPercentage")]
        private float _orthoTopPercentage = 1.0f;

        private Vec2 _origin;
        [TSerialize("OriginPercentages")]
        private EventVec2 _originPercentages = Vec2.Zero;

        public void SetOriginCentered() => SetOriginPercentages(0.5f, 0.5f);
        public void SetOriginBottomLeft() => SetOriginPercentages(0.0f, 0.0f);
        public void SetOriginTopLeft() => SetOriginPercentages(0.0f, 1.0f);
        public void SetOriginBottomRight() => SetOriginPercentages(1.0f, 0.0f);
        public void SetOriginTopRight() => SetOriginPercentages(1.0f, 1.0f);

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
            Resize(Width, Height);
        }
        //public void Pivot(float y, float x, float radius)
        //{
        //    Vec2 center = Dimensions / 2.0f;
        //    BeginUpdate();
        //    Zoom(-radius, center);
        //    AddRotation(y, x);
        //    Zoom(radius, center);
        //    EndUpdate();
        //}
        public void Zoom(float amount, Vec2 zoomOriginScreenPoint, Vec2 minScale, Vec2 maxScale)
        {
            if (amount == 0.0f)
                return;

            Vec3 worldPoint = ScreenToWorld(zoomOriginScreenPoint, 0.0f);
            Vec2 multiplier = Vec2.One / _scale.Xy * amount;
            Vec2 newScale = _scale.Xy - amount;

            bool xClamped = false;
            bool yClamped = false;
            if (newScale.X < minScale.X)
            {
                newScale.X = minScale.X;
                xClamped = true;
            }
            if (newScale.X > maxScale.X)
            {
                newScale.X = maxScale.X;
                xClamped = true;
            }
            if (newScale.Y < minScale.Y)
            {
                newScale.Y = minScale.Y;
                yClamped = true;
            }
            if (newScale.Y > maxScale.Y)
            {
                newScale.Y = maxScale.Y;
                yClamped = true;
            }

            if (!xClamped || !yClamped)
            {
                _localPoint.SetRawNoUpdate(_localPoint.Raw + (worldPoint - WorldPoint) * multiplier);
                _scale.Xy = newScale;
            }

            //_cameraToWorldSpaceMatrix = _cameraToWorldSpaceMatrix * Matrix4.CreateScale(scale);
            //_worldToCameraSpaceMatrix = Matrix4.CreateScale(-scale) * _worldToCameraSpaceMatrix;
            //UpdateTransformedFrustum();
            //OnTransformChanged();
        }
        protected override void OnCalculateProjection(out Matrix4 projMatrix, out Matrix4 inverseProjMatrix)
        {
            projMatrix = Matrix4.CreateOrthographicOffCenter(_orthoLeft, _orthoRight, _orthoBottom, _orthoTop, _nearZ, _farZ);
            inverseProjMatrix = Matrix4.CreateInverseOrthographicOffCenter(_orthoLeft, _orthoRight, _orthoBottom, _orthoTop, _nearZ, _farZ);
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
            _origin = new Vec2(_orthoLeft, _orthoBottom) + _originPercentages.Raw * width;
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
        protected override IFrustum CreateUntransformedFrustum()
        {
            float w = Width / 2.0f, h = Height / 2.0f;
            return BoundingBox.FromMinMax(new Vec3(-w, -h, -_farZ), new Vec3(w, h, -_nearZ)).AsFrustum();
        }
        protected override void OnCreateTransform(out Matrix4 cameraToWorldSpaceMatrix, out Matrix4 worldToCameraSpaceMatrix)
        {
            cameraToWorldSpaceMatrix = 
                Matrix4.CreateTranslation(_localPoint.Raw) *
                _localRotation.GetMatrix() *
                Matrix4.CreateScale(_scale);

            worldToCameraSpaceMatrix = 
                Matrix4.CreateScale(1.0f / _scale) *
                _localRotation.GetInverseMatrix() *
                Matrix4.CreateTranslation(-_localPoint.Raw);
        }
        public override float DistanceScale(Vec3 point, float radius)
        {
            return _scale.X * 80.0f;
        }
    }
}
