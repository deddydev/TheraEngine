using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;

namespace CustomEngine.Rendering.Cameras
{
    public abstract class Camera : FileObject
    {
        public PostProcessSettings PostProcessSettings
        {
            get { return _postProcessSettings; }
            set { _postProcessSettings = value; }
        }
        public float NearDepth
        {
            get { return _nearZ; }
            set { _nearZ = value; CalculateProjection(); }
        }
        public float FarDepth
        {
            get { return _farZ; }
            set { _farZ = value; CalculateProjection(); }
        }
        public Matrix4 ProjectionMatrix { get { return _projectionMatrix; } }
        public Matrix4 ProjectionMatrixInverse { get { return _projectionInverse; } }
        public Matrix4 Matrix { get { return _invTransform; } }
        public Matrix4 InverseMatrix { get { return _transform; } }
        public Vec3 Point
        {
            get { return _point; }
            set { _point = value; CreateTransform(); }
        }
        public Vec3 Direction
        {
            get { return _dir; }
            set { _dir = value; CreateTransform(); }
        }

        public abstract float Width { get; }
        public abstract float Height { get; }

        protected bool _updating;
        protected bool _restrictXRot, _restrictYRot, _restrictZRot;
        protected float _nearZ = 1.0f, _farZ = 200000.0f;
        private PostProcessSettings _postProcessSettings;

        protected Matrix4 _projectionMatrix;
        protected Matrix4 _projectionInverse;

        protected Vec3 _point = Vec3.Zero, _dir = Vec3.Forward;
        protected Quaternion _rotate = Quaternion.Identity;
        protected Matrix4 _transform = Matrix4.Identity, _invTransform = Matrix4.Identity;
        protected void CreateTransform()
        {
            _transform = Matrix4.LookAt(_point, _point + _dir, Vec3.Up);
            _invTransform = _transform.Inverted();
            _rotate = Quaternion.FromMatrix(_transform);
            OnTransformChanged();
        }
        public void Zoom(float amount) => TranslateRelative(0.0f, 0.0f, amount);
        public void TranslateRelative(float x, float y, float z) => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            _transform = Matrix4.CreateTranslation(translation) * _transform;
            _invTransform = Matrix4.CreateTranslation(-translation) * _invTransform;
            _point = _transform.ExtractTranslation();
        }
        public void TranslateAbsolute(float x, float y, float z) => TranslateAbsolute(new Vec3(x, y, z));
        public void TranslateAbsolute(Vec3 translation)
        {
            _transform = _transform * Matrix4.CreateTranslation(translation);
            _invTransform = _invTransform * Matrix4.CreateTranslation(-translation);
            _point = _transform.ExtractTranslation();
        }
        public Vec3 GetUpVector() { return _rotate * Vec3.Up; }
        public Vec3 GetForwardVector() { return _rotate * Vec3.Forward; }
        public Vec3 GetRightVector() { return _rotate * Vec3.Right; }
        public void Rotate(float yaw, float pitch) { Rotate(yaw, pitch, 0.0f); }
        public void Rotate(Vec3 v) => Rotate(v.X, v.Y, v.Z);
        public void Rotate(float yaw, float pitch, float roll)
        {
            if (!yaw.IsZero())
                _dir = Quaternion.FromAxisAngle(Vec3.Up, yaw) * _dir;
            if (!pitch.IsZero())
                _dir = Quaternion.FromAxisAngle(Vec3.Right, pitch) * _dir;
            if (!roll.IsZero())
                _dir = Quaternion.FromAxisAngle(Vec3.Forward, roll) * _dir;
            CreateTransform();
        }
        public void Pivot(float x, float y, float radius)
        {
            BeginUpdate();
            Zoom(-radius);
            Rotate(x, y);
            Zoom(radius);
            EndUpdate();
        }
        public void SetViewTarget(Vec3 target)
        {
            _dir = target - _point;
            CreateTransform();
        }

        public event TranslateChange TranslateChanged;
        public event RotateChange RotateChanged;
        public event ScaleChange ScaleChanged;
        public event Action Resized;
        public event Action TransformChanged;

        protected void OnResized() { Resized?.Invoke(); }
        protected void OnTransformChanged() { TransformChanged?.Invoke(); }
        protected void OnTranslateChanged(Vec3 oldTranslation) { TranslateChanged?.Invoke(oldTranslation); }
        protected void OnRotateChanged(Quaternion oldRotation) { RotateChanged?.Invoke(oldRotation); }
        protected void OnScaleChanged(Vec3 oldScale) { ScaleChanged?.Invoke(oldScale); }

        internal void SetUniforms()
        {
            Engine.Renderer.Uniform(Uniform.ViewMatrixName, Matrix);
            Engine.Renderer.Uniform(Uniform.ProjMatrixName, _projectionMatrix);
        }
        protected virtual Vec3 AlignScreenPoint(Vec3 screenPoint) { return screenPoint; }
        protected virtual Vec3 UnAlignScreenPoint(Vec3 screenPoint) { return screenPoint; }
        protected abstract void CalculateProjection();
        public virtual void Resize(float width, float height)
        {
            CalculateProjection();
            OnResized();
        }
        private void BeginUpdate()
        {
            _updating = true;
        }
        private void EndUpdate()
        {
            _updating = false;
            TransformChanged?.Invoke();
        }
        public void Reset()
        {
            
        }
        /// <summary>
        /// Projects a screen point to world coordinates.
        /// </summary>
        /// <returns>3D world point perpendicular to the camera with a depth value of z (z is not a distance value!)</returns>
        public Vec3 GetWorldPoint(Vec3 screenPoint)
        {
            screenPoint = AlignScreenPoint(screenPoint);
            return screenPoint.Unproject(0, 0, Width, Height, NearDepth, FarDepth, InverseMatrix * _projectionInverse);
        }

        /// <summary>
        /// Projects a screen point to world coordinates.
        /// </summary>
        /// <returns>3D world point perpendicular to the camera with a depth value of z (z is not a distance value!)</returns>
        public Vec3 GetWorldPoint(float x, float y, float z) { return GetWorldPoint(new Vec3(x, y, z)); }
        /// <summary>
        /// Projects a world point to screen coordinates.
        /// </summary>
        /// <returns>2D coordinate on the screen with z as depth (z is not a distance value!)</returns>
        public Vec3 GetScreenPoint(float x, float y, float z) { return GetScreenPoint(new Vec3(x, y, z)); }
        /// <summary>
        /// Projects a world point to screen coordinates.
        /// </summary>
        /// <returns>2D coordinate on the screen with z as depth (z is not a distance value!)</returns>
        public Vec3 GetScreenPoint(Vec3 worldPoint)
        {
            return UnAlignScreenPoint(worldPoint.Project(0, 0, Width, Height, NearDepth, FarDepth, _projectionMatrix * Matrix));
        }
        public Ray GetWorldRay(Vec2 screenPoint)
        {
            Vec3 ray1 = GetWorldPoint(screenPoint.X, screenPoint.Y, 0.0f);
            Vec3 ray2 = GetWorldPoint(screenPoint.X, screenPoint.Y, 1.0f);
            return new Ray(ray1, ray2);
        }
        public Vec3 ProjectCameraSphere(Vec2 screenPoint, Vec3 center, float radius, bool clamp)
        {
            Vec3 point;

            //Get ray points
            Ray ray = GetWorldRay(screenPoint);
            if (!ray.LineSphereIntersect(center, radius, out point))
            {
                //If no intersect is found, project the ray through the plane perpendicular to the camera.
                ray.LinePlaneIntersect(center, Point.Normalized(center), out point);

                //Clamp the point to edge of the sphere
                if (clamp)
                    point = Ray.PointAtLineDistance(center, point, radius);
            }

            return point;
        }

        public void ProjectCameraPlanes(Vec2 screenPoint, Matrix4 transform, out Vec3 xy, out Vec3 yz, out Vec3 xz)
        {
            Ray ray = GetWorldRay(screenPoint);

            Vec3 center = transform.ExtractTranslation();

            ray.LinePlaneIntersect(center, (transform * Vec3.UnitX).Normalized(center), out yz);
            ray.LinePlaneIntersect(center, (transform * Vec3.UnitY).Normalized(center), out xz);
            ray.LinePlaneIntersect(center, (transform * Vec3.UnitZ).Normalized(center), out xy);
        }
        public void SaveCurrentTransform() { }

        public abstract Frustrum GetFrustrum();
    }
}
