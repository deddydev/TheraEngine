using System;

namespace CustomEngine.Rendering.Cameras
{
    public class PerspectiveCamera : Camera
    {
        public float VerticalFieldOfView { get { return _fovY; } set { _fovY = value; CalculateProjection(); } }
        public override float Width { get { return _width; } }
        public override float Height { get { return _height; } }
        public float Aspect { get { return _aspect; } set { _aspect = value; CalculateProjection(); } }
        public float HorizontalFieldOfView
        {
            get { return 2.0f * CustomMath.RadToDeg((float)Math.Atan(Math.Tan(CustomMath.DegToRad(_fovY / 2.0f)) * _aspect)); }
            set { VerticalFieldOfView = 2.0f * CustomMath.RadToDeg((float)Math.Atan(Math.Tan(CustomMath.DegToRad(value / 2.0f)) / _aspect)); }
        }

        private float _width, _height, _fovY = 78.0f, _aspect;
        protected unsafe override void CalculateProjection()
        {
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
            _projectionInverse = Matrix4.CreateInversePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
        }
        public void SetProjectionParams(float aspect, float fovy, float farz, float nearz)
        {
            _aspect = aspect;
            _fovY = fovy;
            _farZ = farz;
            _nearZ = nearz;

            CalculateProjection();
        }
        public override void Resize(float width, float height)
        {
            _width = width;
            _height = height;
            _aspect = _width / _height;
            base.Resize(width, height);
        }
        public override Frustrum GetFrustrum()
        {
            float upAngle = _fovY / 2.0f;
            float tan = (float)Math.Tan(CustomMath.DegToRad(upAngle));
            float nearYDist = tan * _nearZ;
            float nearXDist = _aspect * nearYDist;
            float farYDist = tan * _farZ;
            float farXDist = _aspect * farYDist;
            
            Vec3 forwardDir = GetForwardVector();
            Vec3 rightDir = GetRightVector();
            Vec3 upDir = GetUpVector();
            Vec3 nearPos = _point + forwardDir * _nearZ;
            Vec3 farPos = _point + forwardDir * _farZ;
            Vec3 ntl, /*ntr, */nbl, nbr, ftl, ftr, fbl, fbr;

            Vec3 nX = rightDir * nearXDist;
            Vec3 fX = rightDir * farXDist;
            Vec3 nY = upDir * nearYDist;
            Vec3 fY = upDir * farYDist;

            ntl = nearPos + nY - nX;
            //ntr = nearPos + nY + nX;
            nbl = nearPos - nY - nX;
            nbr = nearPos - nY + nX;
            ftl = farPos + fY - fX;
            ftr = farPos + fY + fX;
            fbl = farPos - fY - fX;
            fbr = farPos - fY + fX;

            Plane near, far, top, bottom, left, right;

            near = new Plane(nearPos, forwardDir);
            far = new Plane(farPos, -forwardDir);

            left = new Plane(nbl, fbl, ntl);
            right = new Plane(fbr, nbr, ftr);

            top = new Plane(ftl, ftr, ntl);
            bottom = new Plane(nbl, nbr, fbl);

            return new Frustrum(near, far, top, bottom, left, right);
        }
    }
}
