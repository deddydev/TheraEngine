using CustomEngine.Rendering.Models.Materials;
using System;

namespace CustomEngine.Rendering.Cameras
{
    public class PerspectiveCamera : Camera
    {
        public PerspectiveCamera() : base() { }

        private float _width, _height, _fovX = 90.0f, _fovY = 78.0f, _aspect;

        public override Vec2 Origin { get { return new Vec2(Width / 2.0f, Height / 2.0f); } }
        public override float Width { get { return _width; } }
        public override float Height { get { return _height; } }
        public float Aspect { get { return _aspect; } set { _aspect = value; CalculateProjection(); } }
        public float VerticalFieldOfView
        {
            get { return _fovY; }
            set
            {
                _fovY = value;
                _fovX = 2.0f * CustomMath.RadToDeg((float)Math.Atan(Math.Tan(CustomMath.DegToRad(_fovY / 2.0f)) * _aspect));
                CalculateProjection();
            }
        }
        public float HorizontalFieldOfView
        {
            get { return _fovX; }
            set
            {
                _fovX = value;
                _fovY = 2.0f * CustomMath.RadToDeg((float)Math.Atan(Math.Tan(CustomMath.DegToRad(value / 2.0f)) / _aspect));
                CalculateProjection();
            }
        }
        protected unsafe override void CalculateProjection()
        {
            base.CalculateProjection();
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
            _projectionInverse = Matrix4.CreateInversePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
        }
        public void SetProjectionParams(float aspect, float fovy, float farz, float nearz)
        {
            _aspect = aspect;
            _farZ = farz;
            _nearZ = nearz;

            //This will set _fovX too
            VerticalFieldOfView = fovy;

            CalculateProjection();
        }
        public override void SetUniforms()
        {
            base.SetUniforms();
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraFovX), _fovX);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraFovY), _fovY);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraAspect), _aspect);
        }
        public override void Resize(float width, float height)
        {
            _width = width;
            _height = height;
            _aspect = _width / _height;
            base.Resize(width, height);
        }
        public override void Zoom(float amount) { TranslateRelative(0.0f, 0.0f, amount); }
        protected override Frustum CreateUntransformedFrustum()
        {
            const bool transformed = false;

            float
                tan = (float)Math.Tan(CustomMath.DegToRad(_fovY / 2.0f)),
                nearYDist = tan * _nearZ,
                nearXDist = _aspect * nearYDist,
                farYDist = tan * _farZ,
                farXDist = _aspect * farYDist;

            Vec3
                point = transformed ? _point.Raw : Vec3.Zero,
                forwardDir = transformed ? GetForwardVector() : Vec3.Forward,
                rightDir = transformed ? GetRightVector() : Vec3.Right,
                upDir = transformed ? GetUpVector() : Vec3.Up,
                nearPos = point + forwardDir * _nearZ,
                farPos = point + forwardDir * _farZ,
                nX = rightDir * nearXDist,
                fX = rightDir * farXDist,
                nY = upDir * nearYDist,
                fY = upDir * farYDist,
                ntl = nearPos + nY - nX,
                ntr = nearPos + nY + nX,
                nbl = nearPos - nY - nX,
                nbr = nearPos - nY + nX,
                ftl = farPos + fY - fX,
                ftr = farPos + fY + fX,
                fbl = farPos - fY - fX,
                fbr = farPos - fY + fX;

            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }

        public override float DistanceScale(Vec3 point, float radius)
        {
            return _point.DistanceToFast(point) * radius / (_fovY / 45.0f) * 0.1f;
        }
    }
}
