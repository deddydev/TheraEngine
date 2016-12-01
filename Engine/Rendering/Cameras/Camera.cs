﻿using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;

namespace CustomEngine.Rendering.Cameras
{
    public abstract class Camera : FileObject
    {
        public Camera() { Resize(1.0f, 1.0f); }

        public Matrix4 ProjectionMatrix { get { return _projectionMatrix; } }
        public Matrix4 ProjectionMatrixInverse { get { return _projectionInverse; } }
        public Matrix4 Matrix { get { return _invTransform; } }
        public Matrix4 InverseMatrix { get { return _transform; } }
        public float NearZ
        {
            get { return _nearZ; }
            set { _nearZ = value; CalculateProjection(); }
        }
        public float FarZ
        {
            get { return _farZ; }
            set { _farZ = value; CalculateProjection(); }
        }
        public Vec3 Point
        {
            get { return _point; }
            set { _point = value; CreateTransform(); }
        }
        public Rotator Rotation
        {
            get { return _rotate; }
            set { _rotate = value; CreateTransform(); }
        }
        public PostProcessSettings PostProcessSettings
        {
            get { return _postProcessSettings; }
            set { _postProcessSettings = value; }
        }
        public bool IsActive
        {
            get { return _isActive; }
            internal set
            {
                if (_isActive == value)
                    return;

                if (_isActive = value)
                    RegisterUniforms();
                else
                    UnregisterUniforms();
            }
        }
        public abstract float Width { get; }
        public abstract float Height { get; }
        public Vec2 Dimensions { get { return new Vec2(Width, Height); } }
        public abstract Vec2 Origin { get; }

        private bool _isActive = false;
        private Vec3 _projectionRange;
        private Vec3 _projectionOrigin;
        protected Frustum _untransformedFrustum, _transformedFrustum;
        
        protected bool _updating = false;
        protected float _nearZ = 1.0f, _farZ = 2000.0f;
        private PostProcessSettings _postProcessSettings;

        protected Matrix4 
            _projectionMatrix = Matrix4.Identity,
            _projectionInverse = Matrix4.Identity,
            _transform = Matrix4.Identity,
            _invTransform = Matrix4.Identity;

        protected Vec3 _scale = Vec3.One;
        protected Vec3 _point = Vec3.Zero;
        protected Rotator _rotate = new Rotator(Rotator.Order.YPR);

        /// <summary>
        /// Returns an X, Y coordinate relative to the camera's Origin, with Z being the normalized depth from NearDepth to FarDepth.
        /// </summary>
        public Vec3 WorldToScreen(Vec3 point)
        {
            return _projectionOrigin + _projectionRange * ((_projectionMatrix * (_transform * point)) + 1.0f) / 2.0f;
        }
        public Vec3 ScreenToWorld(Vec2 point, float depth) { return ScreenToWorld(point.X, point.Y, depth); }
        public Vec3 ScreenToWorld(float x, float y, float depth) { return ScreenToWorld(new Vec3(x, y, depth)); }
        public Vec3 ScreenToWorld(Vec3 screenPoint)
        {
            return _invTransform * (_projectionInverse * ((screenPoint - _projectionOrigin) / _projectionRange * 2.0f - 1.0f));
        }
        protected void CreateTransform()
        {
            //The rotation of the camera points behind it.
            //For example, if the upward rotation increases, 
            //we want the back of the camera to rotate down the same amount
            Rotator rotation = _rotate.WithNegatedRotations();
            
            _transform =
                Matrix4.CreateTranslation(_point) *
                rotation.GetMatrix() *
                Matrix4.CreateScale(_scale);

            rotation.Invert();
            
            _invTransform =
                Matrix4.CreateScale(1.0f / _scale) *
                rotation.GetMatrix() *
                Matrix4.CreateTranslation(-_point);

            _transformedFrustum = _untransformedFrustum.TransformedBy(_transform);

            OnTransformChanged();
        }
        public abstract void Zoom(float amount);
        public void TranslateRelative(float x, float y, float z) => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            _transform = _transform * Matrix4.CreateTranslation(translation);
            _invTransform = Matrix4.CreateTranslation(-translation) * _invTransform;
            _point = _transform.GetPoint();
            _transformedFrustum = _untransformedFrustum.TransformedBy(_transform);
            OnTransformChanged();
        }
        public void TranslateAbsolute(float x, float y, float z) => TranslateAbsolute(new Vec3(x, y, z));
        public void TranslateAbsolute(Vec3 translation)
        {
            _point += translation;
            CreateTransform();
        }
        public Vec3 RotateVector(Vec3 dir) { return _rotate.TransformVector(dir); }
        public Vec3 GetUpVector() { return RotateVector(Vec3.Up); }
        public Vec3 GetForwardVector() { return RotateVector(Vec3.Forward); }
        public Vec3 GetRightVector() { return RotateVector(Vec3.Right); }
        public void Rotate(float yaw, float pitch) { Rotate(yaw, pitch, 0.0f); }
        public void Rotate(Vec3 v) => Rotate(v.X, v.Y, v.Z);
        public void Rotate(float yaw, float pitch, float roll)
            => SetRotate(_rotate.Yaw + yaw, _rotate.Pitch + pitch, _rotate.Roll + roll);
        public void SetRotate(float yaw, float pitch, float roll)
        {
            _rotate.Yaw = yaw;
            _rotate.Pitch = pitch;
            _rotate.Roll = roll;
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
            _rotate = _point.LookatAngles(target);
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
        protected void OnScaleChanged(Vec3 oldScale) { ScaleChanged?.Invoke(oldScale); }

        internal virtual void UnregisterUniforms()
        {
            Uniform.ClearUniforms(
                ECommonUniform.ViewMatrix,
                ECommonUniform.ProjMatrix, 
                ECommonUniform.ScreenWidth, 
                ECommonUniform.ScreenHeight,
                ECommonUniform.ScreenOrigin);
        }
        internal virtual void RegisterUniforms()
        {
            Uniform.ProvideUniform(ECommonUniform.ViewMatrix, UniformViewMatrix);
            Uniform.ProvideUniform(ECommonUniform.ProjMatrix, UniformProjMatrix);
            Uniform.ProvideUniform(ECommonUniform.ScreenWidth, UniformScreenWidth);
            Uniform.ProvideUniform(ECommonUniform.ScreenHeight, UniformScreenHeight);
            Uniform.ProvideUniform(ECommonUniform.ScreenOrigin, UniformScreenOrigin);
        }
        private void UniformViewMatrix(int mId) { Engine.Renderer.Uniform(mId, Uniform.GetLocation(ECommonUniform.ViewMatrix), Matrix); }
        private void UniformProjMatrix(int mId) { Engine.Renderer.Uniform(mId, Uniform.GetLocation(ECommonUniform.ProjMatrix), ProjectionMatrix); }
        private void UniformScreenWidth(int mId) { Engine.Renderer.Uniform(mId, Uniform.GetLocation(ECommonUniform.ScreenWidth), Width); }
        private void UniformScreenHeight(int mId) { Engine.Renderer.Uniform(mId, Uniform.GetLocation(ECommonUniform.ScreenHeight), Height); }
        private void UniformScreenOrigin(int mId) { Engine.Renderer.Uniform(mId, Uniform.GetLocation(ECommonUniform.ScreenOrigin), Origin); }
        protected virtual void CalculateProjection()
        {
            _projectionRange = new Vec3(Dimensions, FarZ - NearZ);
            _projectionOrigin = new Vec3(Origin, NearZ);
            _untransformedFrustum = CreateUntransformedFrustum();
            _transformedFrustum = _untransformedFrustum.TransformedBy(_transform);
        }
        protected abstract Frustum CreateUntransformedFrustum();
        public Frustum GetFrustum() { return _transformedFrustum; }
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
    }
}
