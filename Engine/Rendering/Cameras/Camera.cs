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
            get { return GetForwardVector(); }
            //set { _dir = value; CreateTransform(); }
        }

        public abstract float Width { get; }
        public abstract float Height { get; }
        public Vec2 Dimensions { get { return new Vec2(Width, Height); } }
        public abstract Vec2 Origin { get; }

        private Vec3 _projectionRange;
        private Vec3 _projectionOrigin;
        
        protected bool _updating = false;
        protected bool 
            _restrictXRot,
            _restrictYRot,
            _restrictZRot;
        protected float 
            _nearZ = 1.0f,
            _farZ = 200000.0f;
        private PostProcessSettings _postProcessSettings;

        protected Matrix4 _projectionMatrix = Matrix4.Identity;
        protected Matrix4 _projectionInverse = Matrix4.Identity;

        protected Vec3 _rot = Vec3.Zero, _point = Vec3.Zero;
        protected Matrix4 _transform = Matrix4.Identity, _invTransform = Matrix4.Identity;
        protected Matrix4 _rotation = Matrix4.Identity;
        
        public Vec3 ScreenToWorld(Vec2 point, float depth) { return ScreenToWorld(point.X, point.Y, depth); }
        public Vec3 ScreenToWorld(float x, float y, float depth) { return ScreenToWorld(new Vec3(x, y, depth)); }
        public Vec3 ScreenToWorld(Vec3 screenPoint)
        {
            return _invTransform * (_projectionInverse * ((screenPoint - _projectionOrigin) / _projectionRange * 2.0f - 1.0f));
        }
        /// <summary>
        /// Returns an X, Y coordinate relative to the camera's Origin, with Z being the normalized depth from NearDepth to FarDepth.
        /// </summary>
        public Vec3 WorldToScreen(Vec3 point)
        {
            return _projectionOrigin + _projectionRange * ((_projectionMatrix * (_transform * point)) + 1.0f) / 2.0f;
        }
        protected void CreateTransform()
        {
            _transform = Matrix4.CreateTranslation(_point) * _rotation;
            _invTransform = _transform.Inverted();
            OnTransformChanged();
        }
        public void Zoom(float amount) => TranslateRelative(0.0f, 0.0f, amount);
        public void TranslateRelative(float x, float y, float z) => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            _transform = _transform * Matrix4.CreateTranslation(translation);
            _invTransform = Matrix4.CreateTranslation(-translation) * _invTransform;
            _point = _transform.GetPoint();
            OnTransformChanged();
        }
        public void TranslateAbsolute(float x, float y, float z) => TranslateAbsolute(new Vec3(x, y, z));
        public void TranslateAbsolute(Vec3 translation)
        {
            _point += translation;
            CreateTransform();
        }
        public Vec3 RotateVector(Vec3 dir) { return Vec3.TransformVector(dir, _transform); }
        public Vec3 GetUpVector() { return RotateVector(Vec3.Up); }
        public Vec3 GetForwardVector() { return RotateVector(Vec3.Forward); }
        public Vec3 GetRightVector() { return RotateVector(Vec3.Right); }
        public void Rotate(float yaw, float pitch) { Rotate(yaw, pitch, 0.0f); }
        public void Rotate(Vec3 v) => Rotate(v.X, v.Y, v.Z);
        public void Rotate(float yaw, float pitch, float roll)
        {
            _rot.X -= yaw;
            _rot.Y -= pitch;
            _rot.Z -= roll;

            _rotation = Matrix4.CreateRotationY(_rot.X) * Matrix4.CreateRotationX(_rot.Y);

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
            //_dir = target - _point;
            CreateTransform();
        }
        public void Reset()
        {

        }
        public void SaveCurrentTransform() { }

        public event TranslateChange TranslateChanged;
        public event RotateChange RotateChanged;
        public event ScaleChange ScaleChanged;
        public event Action Resized;
        public event Action TransformChanged;
        
        protected void OnTransformChanged() { TransformChanged?.Invoke(); }
        protected void OnTranslateChanged(Vec3 oldTranslation) { TranslateChanged?.Invoke(oldTranslation); }
        protected void OnRotateChanged(Quaternion oldRotation) { RotateChanged?.Invoke(oldRotation); }
        protected void OnScaleChanged(Vec3 oldScale) { ScaleChanged?.Invoke(oldScale); }

        internal void SetUniforms()
        {
            Engine.Renderer.Uniform(Uniform.ViewMatrixName, Matrix);
            Engine.Renderer.Uniform(Uniform.ProjMatrixName, _projectionMatrix);
        }
        protected virtual void CalculateProjection()
        {
            _projectionRange = new Vec3(Dimensions, FarDepth - NearDepth);
            _projectionOrigin = new Vec3(Origin, NearDepth);
        }
        public virtual void Resize(float width, float height)
        {
            CalculateProjection();
            Resized?.Invoke();
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
        public Ray GetWorldRay(Vec2 screenPoint)
        {
            Vec3 start = ScreenToWorld(screenPoint, 0.0f);
            Vec3 end = ScreenToWorld(screenPoint, 1.0f);
            return new Ray(start, end);
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
        public abstract Frustrum GetFrustrum();
    }
}
