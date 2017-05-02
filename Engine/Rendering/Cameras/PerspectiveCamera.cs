using CustomEngine.Rendering.Models.Materials;
using System;
using CustomEngine.Files;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace CustomEngine.Rendering.Cameras
{
    public class PerspectiveCamera : Camera
    {
        public PerspectiveCamera() : base() { }
        public PerspectiveCamera(Vec3 point, Rotator rotation, float nearZ, float farZ, float fovY)
            : base(point, rotation, nearZ, farZ) { VerticalFieldOfView = fovY; }
        public PerspectiveCamera(float nearZ, float farZ, float fovY)
            : base(nearZ, farZ) { VerticalFieldOfView = fovY; }

        [Serialize("Width", IsXmlAttribute = true)]
        private float _width;
        [Serialize("Height", IsXmlAttribute = true)]
        private float _height;
        [Serialize("Aspect", SerializeIf = "_overrideAspect == true")]
        private float _aspect;
        [Serialize("FovX", IsXmlAttribute = true)]
        private float _fovX = 90.0f;
        //[Serialize("FovY", IsXmlAttribute = true)]
        private float _fovY = 78.0f;
        [Serialize("OverrideAspect")]
        private bool _overrideAspect = false;

        public bool OverrideAspect
        {
            get => _overrideAspect;
            set
            {
                _overrideAspect = value;
                if (!_overrideAspect)
                    _aspect = Width / Height;
            }
        }

        public override Vec2 Origin => new Vec2(Width / 2.0f, Height / 2.0f);
        public override float Width => _width;
        public override float Height => _height;
        public float Aspect
        {
            get => _aspect;
            set
            {
                _aspect = value;
                CalculateProjection();
            }
        }
        public float VerticalFieldOfView
        {
            get => _fovY;
            set
            {
                _fovY = value;
                _fovX = 2.0f * CustomMath.RadToDeg((float)Math.Atan(Math.Tan(CustomMath.DegToRad(_fovY / 2.0f)) * _aspect));
                CalculateProjection();
            }
        }
        public float HorizontalFieldOfView
        {
            get => _fovX;
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
            if (!_overrideAspect)
                _aspect = _width / _height;
            base.Resize(width, height);
        }
        public override void Zoom(float amount)
            => TranslateRelative(0.0f, 0.0f, amount);
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
                point = transformed ? _localPoint.Raw : Vec3.Zero,
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

            //TODO: calculation is incorrect; sphere does not intersect with near points
            float h = _farZ - _nearZ;
            float a = 2.0f * nearXDist;
            float b = 2.0f * farXDist;
            float centerDist = h - (h + (a - b) * (a + b) / (4.0f * h)) / 2.0f;
            Vec3 center = Ray.PointAtLineDistance(nearPos, farPos, centerDist);
            
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr, center);
        }
        public override float DistanceScale(Vec3 point, float radius)
        {
            return WorldPoint.DistanceToFast(point) * radius / (_fovY / 45.0f) * 0.1f;
        }
    }
}
