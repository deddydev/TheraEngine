using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace CustomEngine.Rendering.Camera
{
    public abstract class Camera
    {
        public Matrix4 Matrix { get { return _currentTransform.InverseTransform; } }
        public Matrix4 MatrixInverse { get { return _currentTransform.Transform; } }

        public Matrix4 _projectionMatrix;
        public Matrix4 _projectionInverse;

        private FrameState _currentTransform;
        private FrameState _defaultTransform;

        private float _orthoLeft = 0.0f, _orthoRight = 1.0f, _orthoBottom = 0.0f, _orthoTop = 1.0f;

        public bool _ortho, _restrictXRot, _restrictYRot, _restrictZRot;
        public float
            _fovY = 45.0f,
            _nearZ = 1.0f,
            _farZ = 200000.0f,
            _width = 1,
            _height = 1,
            _aspect = 1;

        bool _updating;

        public float VerticalFieldOfView { get { return _fovY; } set { _fovY = value; CalculateProjection(); } }
        public float NearDepth { get { return _nearZ; } set { _nearZ = value; CalculateProjection(); } }
        public float FarDepth { get { return _farZ; } set { _farZ = value; CalculateProjection(); } }
        public float Width { get { return _width; } set { _width = value; CalculateProjection(); } }
        public float Height { get { return _height; } set { _height = value; CalculateProjection(); } }
        public float Aspect { get { return _aspect; } set { _aspect = value; CalculateProjection(); } }
        public bool Orthographic { get { return _ortho; } set { if (_ortho == value) return; _ortho = value; CalculateProjection(); } }

        public event TranslateChanged OnTranslateChanged;
        public event RotateChanged OnRotateChanged;
        public event ScaleChanged OnScaleChanged;

        public Camera()
        {
            _width = _height = 0;
            Reset();
        }
        public Camera(float width, float height, Vector3 defaultTranslate, Quaternion defaultRotate, Vector3 defaultScale)
        {
            _width = width;
            _height = height;
            _currentTransform = _defaultTransform = new FrameState(defaultTranslate, defaultRotate, defaultScale);
            _orthoLeft = -_width / 2.0f;
            _orthoRight = _width / 2.0f;
            _orthoBottom = -_height / 2.0f;
            _orthoTop = _height / 2.0f;
        }

        public Vector3 Point
        {
            get { return _currentTransform.Translation; }
            set { _currentTransform.Translation = value; }
        }
        public Quaternion Rotation { get { return _currentTransform.Rotation; } }
        public Vector3 Scale { get { return _currentTransform.Scale; } }

        public abstract void Zoom(float amount);

        public void Zoom(float amt)
        {
            if (_ortho)
            {
                float scale = amt >= 0 ? amt : 1.0f / -amt;
                Scale(scale, scale, scale);
            }
            else
                TranslateRelative(0.0f, 0.0f, amt);
        }
        public void TranslateRelative(Vector3 v) { TranslateRelative(v.X, v.Y, v.Z); }
        public void TranslateRelative(float x, float y, float z)
        {
            MatrixInverse.Translate(x, y, z);
            _currentTransform.Translation = MatrixInverse.ExtractTranslation();
        }
        public void Rotate(float x, float y, float z) { Rotate(new Vector3(x, y, z)); }
        public void Rotate(Vector3 v)
        {
            //Fix for left and right dragging when the camera is upside down
            //if (_rotation._x < -90.0f || _rotation._x > 90.0f)
            //    v._y = -v._y;

            if (_restrictXRot) v.X = 0.0f;
            if (_restrictYRot) v.Y = 0.0f;
            if (_restrictZRot) v.Z = 0.0f;

            _currentTransform.ApplyRotation(v);
        }

        public void Rotate(float x, float y) { Rotate(x, y, 0); }
        public void Pivot(float radius, float x, float y)
        {
            BeginUpdate();
            Zoom(-radius);
            Rotate(x, y);
            Zoom(radius);
            EndUpdate();
        }

        private void BeginUpdate()
        {
            _updating = true;
        }
        private void EndUpdate()
        {
            _updating = false;
            OnTransformChanged?.Invoke();
        }

        public void Set(Vector3 translate, Vector3 rotate, Vector3 scale)
        {
            _currentTransform.SetAll(translate, Quaternion.FromEulerAngles(rotate), scale);
        }
        public void Set(Vector3 translate, Quaternion rotate, Vector3 scale)
        {
            _currentTransform.SetAll(translate, rotate, scale);
        }
        public void SetProjectionParams(float aspect, float fovy, float farz, float nearz)
        {
            _aspect = aspect;
            _fovY = fovy;
            _farZ = farz;
            _nearZ = nearz;

            CalculateProjection();
        }

        public void Reset()
        {
            _currentTransform = _defaultTransform;
        }

        public void CalculateProjection()
        {
            if (_ortho)
            {
                _projectionMatrix = Matrix4.CreateOrthographicOffCenter(_orthoDimensions.X, , _height, _nearZ, _farZ);
                _projectionInverse = Matrix4.CreateInverseOrthographicOffCenter(_width, _height, _nearZ, _farZ);
            }
            else
            {
                _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
                _projectionInverse = Matrix4.CreateInversePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
            }
        }
        public void Resize(float width, float height)
        {
            _width = width;
            _height = height;
            _aspect = _width / _height;
            CalculateProjection();
            OnResized?.Invoke();
        }
        /// <summary>
        /// Projects a screen point to world coordinates.
        /// </summary>
        /// <returns>3D world point perpendicular to the camera with a depth value of z (z is not a distance value!)</returns>
        public Vector3 UnProject(Vector3 point)
        {
            return Vector3.Unproject(point, 0, 0, Width, Height, NearDepth, FarDepth, MatrixInverse * _projectionInverse);
        }
        /// <summary>
        /// Projects a screen point to world coordinates.
        /// </summary>
        /// <returns>3D world point perpendicular to the camera with a depth value of z (z is not a distance value!)</returns>
        public Vector3 UnProject(float x, float y, float z)
        {
            return UnProject(new Vector3(x, y, z));
        }
        /// <summary>
        /// Projects a world point to screen coordinates.
        /// </summary>
        /// <returns>2D coordinate on the screen with z as depth (z is not a distance value!)</returns>
        public Vector3 Project(float x, float y, float z) { return Project(new Vector3(x, y, z)); }
        /// <summary>
        /// Projects a world point to screen coordinates.
        /// </summary>
        /// <returns>2D coordinate on the screen with z as depth (z is not a distance value!)</returns>
        public Vector3 Project(Vector3 source)
        {
            return Vector3.Project(source, 0, 0, Width, Height, NearDepth, FarDepth, _projectionMatrix * Matrix);
        }

        public Vector3 ProjectCameraSphere(Vector2 screenPoint, Vector3 center, float radius, bool clamp)
        {
            Vector3 point;

            //Get ray points
            Vector3 ray1 = UnProject(screenPoint.X, screenPoint.Y, 0.0f);
            Vector3 ray2 = UnProject(screenPoint.X, screenPoint.Y, 1.0f);

            if (!CustomMath.LineSphereIntersect(ray1, ray2, center, radius, out point))
            {
                //If no intersect is found, project the ray through the plane perpendicular to the camera.
                CustomMath.LinePlaneIntersect(ray1, ray2, center, Point.Normalized(center), out point);

                //Clamp the point to edge of the sphere
                if (clamp)
                    point = CustomMath.PointAtLineDistance(center, point, radius);
            }

            return point;
        }

        public void ProjectCameraPlanes(Vector2 screenPoint, Matrix4 transform, out Vector3 xy, out Vector3 yz, out Vector3 xz)
        {
            Vector3 ray1 = UnProject(screenPoint.X, screenPoint.Y, 0.0f);
            Vector3 ray2 = UnProject(screenPoint.X, screenPoint.Y, 1.0f);

            Vector3 center = transform.ExtractTranslation();

            CustomMath.LinePlaneIntersect(ray1, ray2, center, (transform * Vector3.UnitX).Normalized(center), out yz);
            CustomMath.LinePlaneIntersect(ray1, ray2, center, (transform * Vector3.UnitY).Normalized(center), out xz);
            CustomMath.LinePlaneIntersect(ray1, ray2, center, (transform * Vector3.UnitZ).Normalized(center), out xy);
        }

        public void LoadProjection()
        {
            Engine.Renderer.MatrixMode(MtxMode.Projection);
            Engine.Renderer.LoadMatrix(_projectionMatrix);
        }
        public void LoadModelView()
        {
            Engine.Renderer.MatrixMode(MtxMode.Modelview);
            Engine.Renderer.LoadMatrix(Matrix);
        }
        public void SaveCurrentTransform()
        {
            _defaultTransform = _currentTransform;
        }
    }
}
