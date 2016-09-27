using CustomEngine.Rendering.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace CustomEngine.World
{
    public unsafe class Camera
    {
        public Matrix4 Matrix { get { return _currentTransform.InverseTransform; } }
        public Matrix4 MatrixInverse { get { return _currentTransform.Transform; } }

        public Matrix4 _projectionMatrix;
        public Matrix4 _projectionInverse;

        private FrameState _currentTransform;
        private FrameState _defaultTransform;

        public Vector4 _orthoDimensions = new Vector4(0, 1, 0, 1);
        public bool _ortho, _restrictXRot, _restrictYRot, _restrictZRot;
        public float
            _fovY = 45.0f,
            _nearZ = 1.0f,
            _farZ = 200000.0f,
            _width = 1,
            _height = 1,
            _aspect = 1;

        bool _updating;

        public void SetProjectionParams(float aspect, float fovy, float farz, float nearz)
        {
            _aspect = aspect;
            _fovY = fovy;
            _farZ = farz;
            _nearZ = nearz;

            CalculateProjection();
        }

        public float VerticalFieldOfView { get { return _fovY; } set { _fovY = value; CalculateProjection(); } }
        public float NearDepth { get { return _nearZ; } set { _nearZ = value; CalculateProjection(); } }
        public float FarDepth { get { return _farZ; } set { _farZ = value; CalculateProjection(); } }
        public float Width { get { return _width; } set { _width = value; CalculateProjection(); } }
        public float Height { get { return _height; } set { _height = value; CalculateProjection(); } }
        public float Aspect { get { return _aspect; } set { _aspect = value; CalculateProjection(); } }
        public bool Orthographic { get { return _ortho; } set { if (_ortho == value) return; _ortho = value; CalculateProjection(); } }

        public Camera() { Reset(); }
        public Camera(float width, float height, Vector3 defaultTranslate, Quaternion defaultRotate, Vector3 defaultScale)
        {
            _width = width;
            _height = height;
            _currentTransform = _defaultTransform = new FrameState(defaultTranslate, defaultRotate, defaultScale);
            _orthoDimensions = new Vector4(-_width / 2.0f, _width / 2.0f, _height / 2.0f, -_height / 2.0f);
        }
        
        public void Scale(float x, float y, float z) { Scale(new Vector3(x, y, z)); }
        public void Scale(Vector3 v)
        {
            _scale *= v;
            Apply();
        }
        public void Zoom(float amt)
        {
            if (_ortho)
            {
                float scale = (amt >= 0 ? amt / 2.0f : 2.0f / -amt);
                Scale(scale, scale, scale);
            }
            else
                Translate(0.0f, 0.0f, amt);
        }
        public void Translate(Vector3 v) { Translate(v._x, v._y, v._z); }
        public void Translate(float x, float y, float z)
        {
            _matrix = Matrix.TranslationMatrix(-x, -y, -z) * Matrix;
            MatrixInverse.Translate(x, y, z);

            PositionChanged();
        }
        public void Rotate(float x, float y, float z) { Rotate(new Vector3(x, y, z)); }
        public void Rotate(Vector3 v)
        {
            //Fix for left and right dragging when the camera is upside down
            if (_rotation._x < -90.0f || _rotation._x > 90.0f)
                v._y = -v._y;

            if (_restrictXRot) v._x = 0.0f;
            if (_restrictYRot) v._y = 0.0f;
            if (_restrictZRot) v._z = 0.0f;

            _rotation = (_rotation + v).RemappedToRange(-180.0f, 180.0f);

            Apply();
        }

        private void Apply()
        {
            //Grab vertex from matrix
            Vector3 point = GetPoint();

            //Reset matrices
            _matrix = Matrix.ReverseTransformMatrix(_scale, _rotation, point);
            _matrixInverse = Matrix.TransformMatrix(_scale, _rotation, point);

            PositionChanged();
        }

        public void Rotate(float x, float y) { Rotate(x, y, 0); }
        public void Pivot(float radius, float x, float y)
        {
            _updating = true;
            Translate(0, 0, -radius);
            Rotate(x, y);
            Translate(0, 0, radius);
            _updating = false;
            PositionChanged();
        }

        public void Set(Vector3 translate, Vector3 rotate, Vector3 scale) { }
        public void Set(Vector3 translate, Quaternion rotate, Vector3 scale)
        {
            _currentTransform.SetAll(translate, rotate, scale);
            PositionChanged();
        }

        public void Reset()
        {
            _currentTransform = _defaultTransform;
            PositionChanged();
        }
        private void PositionChanged()
        {
            if (!_updating && OnPositionChanged != null)
                OnPositionChanged();
        }

        public void CalculateProjection()
        {
            if (_ortho)
            {
                _projectionMatrix = Matrix4.CreateOrthographicOffCenter(_orthoDimensions, _nearZ, _farZ);
                _projectionInverse = Matrix4.ReverseOrthographicMatrix(_orthoDimensions, _nearZ, _farZ);
            }
            else
            {
                _projectionMatrix = Matrix4.PerspectiveMatrix(_fovY, _aspect, _nearZ, _farZ);
                _projectionInverse = Matrix4.ReversePerspectiveMatrix(_fovY, _aspect, _nearZ, _farZ);
            }
        }

        public event Action OnDimensionsChanged, OnPositionChanged;

        public void SetDimensions(float width, float height)
        {
            _width = width;
            _height = height;
            _aspect = _width / _height;

            _orthoDimensions = new Vector4(-_width / 2.0f, _width / 2.0f, _height / 2.0f, -_height / 2.0f);

            CalculateProjection();

            if (OnDimensionsChanged != null)
                OnDimensionsChanged();
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
                CustomMath.LinePlaneIntersect(ray1, ray2, center, GetPoint().Normalize(center), out point);

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

            Vector3 center = transform.GetPoint();

            CustomMath.LinePlaneIntersect(ray1, ray2, center, (transform * Vector3.UnitX).Normalize(center), out yz);
            CustomMath.LinePlaneIntersect(ray1, ray2, center, (transform * Vector3.UnitY).Normalize(center), out xz);
            CustomMath.LinePlaneIntersect(ray1, ray2, center, (transform * Vector3.UnitZ).Normalize(center), out xy);
        }

        public void LoadProjection()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            fixed (Matrix* p = &_projectionMatrix)
                GL.LoadMatrix((float*)p);
        }
        public void LoadModelView()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            fixed (Matrix* p = &_matrix)
                GL.LoadMatrix((float*)p);
        }

        public unsafe void ZoomExtents(Vector3 point, float distance)
        {
            if (!_ortho)
                _rotation = new Vector3();

            Vector3 position = point + new Vector3(0.0f, 0.0f, distance);
            _matrix = Matrix.ReverseTransformMatrix(_scale, _rotation, position);
            _matrixInverse = Matrix.TransformMatrix(_scale, _rotation, position);
        }

        public void SaveDefaults()
        {
            _defaultTranslate = GetPoint();
            _defaultRotate = _rotation;
            _defaultScale = _scale;
        }
    }
}
