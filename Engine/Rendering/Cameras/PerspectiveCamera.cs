using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Cameras
{
    public class PerspectiveCamera : Camera
    {
        public float VerticalFieldOfView { get { return _fovY; } set { _fovY = value; CalculateProjection(); } }
        public float Width { get { return _width; } set { _width = value; CalculateProjection(); } }
        public float Height { get { return _height; } set { _height = value; CalculateProjection(); } }
        public float Aspect { get { return _aspect; } set { _aspect = value; CalculateProjection(); } }

        private float _width, _height, _fovY = 45.0f, _aspect;

        public override void Zoom(float amount)
        {
            TranslateRelative(0.0f, 0.0f, amount);
        }
        public override void CalculateProjection()
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

        protected override float GetWidth() { return Width; }
        protected override float GetHeight() { return Height; }
    }
}
