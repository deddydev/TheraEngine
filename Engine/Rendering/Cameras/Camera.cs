﻿using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;

namespace CustomEngine.Rendering.Cameras
{
    public abstract class Camera : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.Camera; } }

        public Camera()
        {
            Resize(1.0f, 1.0f);
            _rotation.Changed += CreateTransform;
            _point.Changed += CreateTransform;
        }
        public Camera(Vec3 point, Rotator rotation, float nearZ, float farZ)
        {
            Resize(1.0f, 1.0f);
            _rotation = rotation;
            _nearZ = nearZ;
            _farZ = farZ;
            _point = point;
            _rotation.Changed += CreateTransform;
            _point.Changed += CreateTransform;
        }
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
            get { return _point.Raw; }
            set { _point.Raw = value; }
        }
        public Rotator Rotation
        {
            get { return _rotation; }
            set { _rotation.SetRotations(value); }
        }
        public PostProcessSettings PostProcessSettings
        {
            get { return _postProcessSettings; }
            set { _postProcessSettings = value; }
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

        protected EventVec3 _scale = Vec3.One;
        protected EventVec3 _point = Vec3.Zero;
        protected Rotator _rotation = new Rotator(Rotator.Order.YPR);
        protected Vec3 _forwardDirection;

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
            Matrix4 rotMatrix = _rotation.GetMatrix();

            _transform =
                Matrix4.CreateTranslation(_point.Raw) *
                rotMatrix *
                Matrix4.CreateScale(_scale);

            _invTransform =
                Matrix4.CreateScale(1.0f / _scale) *
                _rotation.GetInverseMatrix() *
                Matrix4.CreateTranslation(-_point.Raw);

            _forwardDirection = Vec3.TransformVector(Vec3.Forward, rotMatrix);

            OnTransformChanged();
        }
        protected void UpdateTransformedFrustum()
        {
            _transformedFrustum = _untransformedFrustum.TransformedBy(Matrix4.CreateTranslation(_point) * _rotation.GetMatrix() * Matrix4.CreateScale(_scale));
        }
        public abstract void Zoom(float amount);
        public void TranslateRelative(float x, float y, float z) => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            if (translation == Vec3.Zero)
                return;

            _transform = _transform * Matrix4.CreateTranslation(translation);
            _invTransform = Matrix4.CreateTranslation(-translation) * _invTransform;
            _point.SetRawNoUpdate(_transform.GetPoint());
            UpdateTransformedFrustum();
            OnTransformChanged();
        }
        public void TranslateAbsolute(float x, float y, float z) => TranslateAbsolute(new Vec3(x, y, z));
        public void TranslateAbsolute(Vec3 translation)
        {
            if (translation == Vec3.Zero)
                return;

            _point.Raw += translation;
        }
        public Vec3 RotateVector(Vec3 dir) { return _rotation.TransformVector(dir); }
        public Vec3 GetUpVector() { return RotateVector(Vec3.Up); }
        public Vec3 GetForwardVector() { return _forwardDirection; }
        public Vec3 GetRightVector() { return RotateVector(Vec3.Right); }
        public void Rotate(float pitch, float yaw) { Rotate(pitch, yaw, 0.0f); }
        public void Rotate(float pitch, float yaw, float roll)
            => SetRotate(_rotation.Pitch + pitch, _rotation.Yaw + yaw, _rotation.Roll + roll);
        public void SetRotate(Rotator r)
        {
            _rotation = r;
            _rotation.Changed += CreateTransform;
            CreateTransform();
        }
        public void SetRotate(Vec3 pitchYawRoll)
        {
            _rotation.PitchYawRoll = pitchYawRoll;
            CreateTransform();
        }
        public void SetRotate(float pitch, float yaw, float roll)
        {
            _rotation.SetRotations(pitch, yaw, roll);
        }
        public void Pivot(float y, float x, float radius)
        {
            BeginUpdate();
            Zoom(-radius);
            Rotate(y, x);
            Zoom(radius);
            EndUpdate();
        }
        public void SetViewTarget(Vec3 target)
        {
            _rotation = (target - _point).LookatAngles();
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

        public virtual void SetUniforms()
        {
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ViewMatrix), Matrix);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ProjMatrix), ProjectionMatrix);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ScreenWidth), Width);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ScreenHeight), Height);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ScreenOrigin), Origin);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraNearZ), NearZ);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraFarZ), FarZ);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraPosition), Point);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraForward), _forwardDirection);
        }
        protected virtual void CalculateProjection()
        {
            _projectionRange = new Vec3(Dimensions, FarZ - NearZ);
            _projectionOrigin = new Vec3(Origin, NearZ);
            _untransformedFrustum = CreateUntransformedFrustum();
            UpdateTransformedFrustum();
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
        public abstract float DistanceScale(Vec3 point, float radius);
    }
}
