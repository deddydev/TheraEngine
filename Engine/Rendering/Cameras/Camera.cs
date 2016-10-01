using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using System;
using System.Drawing;

namespace CustomEngine.Rendering.Cameras
{
    public abstract class Camera
    {
        public float NearDepth { get { return _nearZ; } set { _nearZ = value; CalculateProjection(); } }
        public float FarDepth { get { return _farZ; } set { _farZ = value; CalculateProjection(); } }
        public Matrix4 Matrix { get { return _currentTransform.InverseTransform; } }
        public Matrix4 MatrixInverse { get { return _currentTransform.Transform; } }
        public FrameState CurrentTransform
        {
            get { return _currentTransform; }
            set
            {
                _currentTransform = value;
                OnTransformChanged?.Invoke();
            }
        }
        public FrameState DefaultTransform
        {
            get { return _defaultTransform; }
            set { _defaultTransform = value; }
        }
        public Vector3 Translation
        {
            get { return _currentTransform.Translation; }
            set { _currentTransform.Translation = value; }
        }
        public Quaternion Rotation
        {
            get { return _currentTransform.Rotation; }
            set { _currentTransform.Rotation = value; }
        }
        public Vector3 Scale
        {
            get { return _currentTransform.Scale; }
            set { _currentTransform.Scale = value; }
        }

        protected Matrix4 _projectionMatrix;
        protected Matrix4 _projectionInverse;
        protected FrameState _currentTransform;
        protected FrameState _defaultTransform;

        protected bool _updating;
        protected bool _restrictXRot, _restrictYRot, _restrictZRot;
        protected float _nearZ = 1.0f, _farZ = 200000.0f;

        public event TranslateChanged OnTranslateChanged;
        public event RotateChanged OnRotateChanged;
        public event ScaleChanged OnScaleChanged;
        public event Action OnResized;
        public event Action OnTransformChanged;

        public Camera()
        {
            Reset();
        }
        public Camera(Vector3 defaultTranslate, Quaternion defaultRotate, Vector3 defaultScale)
        {
            _currentTransform = _defaultTransform = new FrameState(defaultTranslate, defaultRotate, defaultScale);
        }
        public Camera(FrameState defaultTransform)
        {
            _currentTransform = _defaultTransform = defaultTransform;
        }

        protected abstract float GetWidth();
        protected abstract float GetHeight();
        protected virtual Vector3 AlignScreenPoint(Vector3 screenPoint) { return screenPoint; }
        protected virtual Vector3 UnAlignScreenPoint(Vector3 screenPoint) { return screenPoint; }
        public abstract void Zoom(float amount);
        public abstract void CalculateProjection();
        public virtual void Resize(float width, float height)
        {
            CalculateProjection();
            OnResized?.Invoke();
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

        public void SetTransform(Vector3 translate, Vector3 rotate, Vector3 scale)
        {
            _currentTransform.SetAll(translate, Quaternion.FromEulerAngles(rotate), scale);
        }
        public void SetTransform(Vector3 translate, Quaternion rotate, Vector3 scale)
        {
            _currentTransform.SetAll(translate, rotate, scale);
        }
        public void Reset()
        {
            _currentTransform = _defaultTransform;
        }
        /// <summary>
        /// Projects a screen point to world coordinates.
        /// </summary>
        /// <returns>3D world point perpendicular to the camera with a depth value of z (z is not a distance value!)</returns>
        public Vector3 GetWorldPoint(Vector3 screenPoint)
        {
            screenPoint = AlignScreenPoint(screenPoint);
            return Vector3.Unproject(screenPoint, 0, 0, GetWidth(), GetHeight(), NearDepth, FarDepth, MatrixInverse * _projectionInverse);
        }

        /// <summary>
        /// Projects a screen point to world coordinates.
        /// </summary>
        /// <returns>3D world point perpendicular to the camera with a depth value of z (z is not a distance value!)</returns>
        public Vector3 GetWorldPoint(float x, float y, float z)
        {
            return GetWorldPoint(new Vector3(x, y, z));
        }
        /// <summary>
        /// Projects a world point to screen coordinates.
        /// </summary>
        /// <returns>2D coordinate on the screen with z as depth (z is not a distance value!)</returns>
        public Vector3 GetScreenPoint(float x, float y, float z) { return GetScreenPoint(new Vector3(x, y, z)); }
        /// <summary>
        /// Projects a world point to screen coordinates.
        /// </summary>
        /// <returns>2D coordinate on the screen with z as depth (z is not a distance value!)</returns>
        public Vector3 GetScreenPoint(Vector3 worldPoint)
        {
            return UnAlignScreenPoint(Vector3.Project(worldPoint, 0, 0, GetWidth(), GetHeight(), NearDepth, FarDepth, _projectionMatrix * Matrix));
        }

        public Ray GetWorldRay(Vector2 screenPoint)
        {
            Vector3 ray1 = GetWorldPoint(screenPoint.X, screenPoint.Y, 0.0f);
            Vector3 ray2 = GetWorldPoint(screenPoint.X, screenPoint.Y, 1.0f);
            return new Ray(ray1, ray2);
        }

        public Vector3 ProjectCameraSphere(Vector2 screenPoint, Vector3 center, float radius, bool clamp)
        {
            Vector3 point;

            //Get ray points
            Ray ray = GetWorldRay(screenPoint);
            if (!ray.LineSphereIntersect(center, radius, out point))
            {
                //If no intersect is found, project the ray through the plane perpendicular to the camera.
                ray.LinePlaneIntersect(center, Translation.Normalized(center), out point);

                //Clamp the point to edge of the sphere
                if (clamp)
                    point = Ray.PointAtLineDistance(center, point, radius);
            }

            return point;
        }

        public void ProjectCameraPlanes(Vector2 screenPoint, Matrix4 transform, out Vector3 xy, out Vector3 yz, out Vector3 xz)
        {
            Ray ray = GetWorldRay(screenPoint);

            Vector3 center = transform.ExtractTranslation();

            ray.LinePlaneIntersect(center, (transform * Vector3.UnitX).Normalized(center), out yz);
            ray.LinePlaneIntersect(center, (transform * Vector3.UnitY).Normalized(center), out xz);
            ray.LinePlaneIntersect(center, (transform * Vector3.UnitZ).Normalized(center), out xy);
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
