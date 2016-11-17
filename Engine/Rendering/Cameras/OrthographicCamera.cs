using System;

namespace CustomEngine.Rendering.Cameras
{
    public class OrthographicCamera : Camera
    {
        public OrthographicCamera() : base() { }
        public OrthographicCamera(Matrix4.MultiplyOrder order) : base(order) { }
        public OrthographicCamera(FrameState defaultTransform) : base(defaultTransform) { }
        public OrthographicCamera(Vec3 defaultTranslate, Quaternion defaultRotate, Vec3 defaultScale)
            : base(defaultTranslate, defaultRotate, defaultScale) { }
        public OrthographicCamera(Vec3 defaultTranslate, Quaternion defaultRotate, Vec3 defaultScale, Matrix4.MultiplyOrder order)
            : base(defaultTranslate, defaultRotate, defaultScale, order) { }

        public override float Width { get { return Math.Abs(_orthoRight - _orthoLeft); } }
        public override float Height { get { return Math.Abs(_orthoTop - _orthoBottom); } }

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
            Scale = Scale * new Vec3(scale);
        }
        protected override void CalculateProjection()
        {
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
        protected override Vec3 AlignScreenPoint(Vec3 screenPoint)
        {
            return new Vec3(screenPoint.X + _orthoLeft, screenPoint.Y + _orthoBottom, screenPoint.Z);
        }
        protected override Vec3 UnAlignScreenPoint(Vec3 screenPoint)
        {
            return new Vec3(screenPoint.X - _orthoLeft, screenPoint.Y - _orthoBottom, screenPoint.Z);
        }

        public override Frustrum GetFrustrum()
        {
            float w = Width / 2.0f, h = Height / 2.0f;
            return new Box(new Vec3(-w, _nearZ, -h), new Vec3(w, _farZ, h)).AsFrustrum(CurrentTransform.Matrix);
        }
    }
}
