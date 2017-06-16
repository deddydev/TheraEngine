﻿using TheraEngine.Files;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TheraEngine.Worlds.Actors;
using System.ComponentModel;
using System.Drawing;

namespace TheraEngine.Rendering.Cameras
{
    public delegate void OwningComponentChange(CameraComponent previous, CameraComponent current);
    public abstract class Camera : FileObject, IRenderable
    {
        public event OwningComponentChange OwningComponentChanged;
        public delegate void TranslationChange(Vec3 oldTranslation);
        public delegate void RotationChange(Rotator oldRotation);

        public bool HasTransparency => false;

        public Camera() 
            : this(16.0f, 9.0f) { }
        public Camera(float width, float height)
            : this(width, height, 1.0f, 10000.0f) { }
        public Camera(float width, float height, float nearZ, float farZ)
            : this(width, height, nearZ, farZ, Vec3.Zero, Rotator.GetZero()) { }
        public Camera(float width, float height, float nearZ, float farZ, Vec3 point, Rotator rotation)
        {
            _postProcessSettings = new PostProcessSettings();
            _transformedFrustum = new Frustum();
            _localRotation = rotation;
            _localPoint = point;
            _nearZ = nearZ;
            _farZ = farZ;
            Resize(width, height);
            _viewTarget = null;
            _localRotation.Changed += CreateTransform;
            _localPoint.Changed += PositionChanged;
            PositionChanged();
        }
        
        public Matrix4 ProjectionMatrix => _projectionMatrix;
        public Matrix4 InverseProjectionMatrix => _projectionInverse;
        public Matrix4 WorldMatrix
        {
            get => _owningComponent != null ? _owningComponent.WorldMatrix : _transform;
            //set
            //{
            //    if (_owningComponent != null)
            //    {
            //        _owningComponent.WorldMatrix = value;
            //        _transform = _owningComponent.LocalMatrix;
            //        _invTransform = _owningComponent.InverseLocalMatrix;
            //    }
            //    else
            //    {
            //        _transform = value;
            //        _invTransform = _transform.Inverted();
            //    }
            //}
        }
        public Matrix4 InverseWorldMatrix
        {
            get => _owningComponent != null ? _owningComponent.InverseWorldMatrix : _invTransform;
            //set
            //{
            //    if (_owningComponent != null)
            //    {
            //        _owningComponent.InverseWorldMatrix = value;
            //        _transform = _owningComponent.LocalMatrix;
            //        _invTransform = _owningComponent.InverseLocalMatrix;
            //    }
            //    else
            //    {
            //        _invTransform = value;
            //        _transform = _invTransform.Inverted();
            //    }
            //}
        }
        public Matrix4 InverseLocalMatrix => _invTransform;
        public Matrix4 LocalMatrix
        {
            get => _transform;
            internal set
            {
                _localPoint.Raw = _transform.GetPoint();
                _localRotation.SetRotations(_transform.GetRotationMatrix4().ExtractRotation().ToYawPitchRoll());
                _transform = value;
                _invTransform = _transform.Inverted();
                UpdateTransformedFrustum();
                OnTransformChanged();
            }
        }
        public float NearZ
        {
            get => _nearZ;
            set
            {
                _nearZ = value;
                CalculateProjection();
            }
        }
        public float FarZ
        {
            get => _farZ;
            set
            {
                _farZ = value;
                CalculateProjection();
            }
        }
        public Vec3 WorldPoint
        {
            get => _owningComponent != null ? _owningComponent.WorldMatrix.GetPoint() : _localPoint.Raw;
        }
        public Vec3 LocalPoint
        {
            get => _localPoint.Raw;
            set => _localPoint.Raw = value;
        }
        public Rotator LocalRotation
        {
            get => _localRotation;
            set => _localRotation.SetRotations(value);
        }
        public PostProcessSettings PostProcessSettings
        {
            get => _postProcessSettings;
            set => _postProcessSettings = value ?? new PostProcessSettings();
        }
        public abstract float Width { get; }
        public abstract float Height { get; }
        public abstract Vec2 Origin { get; }
        public Vec2 Dimensions => new Vec2(Width, Height);
        public List<Viewport> Viewports
        {
            get => _viewports;
            set => _viewports = value;
        }
        public bool IsActiveRenderCamera => _isActive;
        public Shape CullingVolume => _transformedFrustum.CullingVolume;
        public bool IsRendering
        {
            get => _transformedFrustum.IsRendering;
            set => _transformedFrustum.IsRendering = value;
        }
        public IOctreeNode RenderNode
        {
            get => _transformedFrustum.RenderNode;
            set => _transformedFrustum.RenderNode = value;
        }
        public CameraComponent OwningComponent
        {
            get => _owningComponent;
            set
            {
                CameraComponent prev = _owningComponent;
                if (_owningComponent != null)
                    _owningComponent.WorldTransformChanged -= _owningComponent_WorldTransformChanged;
                _owningComponent = value;
                if (_owningComponent != null)
                    _owningComponent.WorldTransformChanged += _owningComponent_WorldTransformChanged;
                OwningComponentChanged?.Invoke(prev, _owningComponent);
                UpdateTransformedFrustum();
            }
        }
        public EventVec3 ViewTarget
        {
            get => _viewTarget;
            set
            {
                if (_viewTarget != null)
                    _viewTarget.Changed -= _viewTarget_Changed;
                _viewTarget = value;
                if (_viewTarget != null)
                    _viewTarget.Changed += _viewTarget_Changed;
                _viewTarget_Changed();
            }
        }
        
        private CameraComponent _owningComponent;
        private List<Viewport> _viewports = new List<Viewport>();
        internal bool _isActive = false;
        internal Vec3 _projectionRange;
        internal Vec3 _projectionOrigin;
        protected Frustum _untransformedFrustum, _transformedFrustum;
        private EventVec3 _viewTarget = null;
        protected bool _updating = false;
        protected Matrix4
            _projectionMatrix = Matrix4.Identity,
            _projectionInverse = Matrix4.Identity,
            _transform = Matrix4.Identity,
            _invTransform = Matrix4.Identity;
        private Vec3
            _forwardDirection = Vec3.Forward,
            _upDirection = Vec3.Up,
            _rightDirection = Vec3.Right;
        private bool
            _forwardInvalidated = true,
            _upInvalidated = true,
            _rightInvalidated = true;

        [Serialize("Point")]
        protected EventVec3 _localPoint;
        [Serialize("Rotation")]
        protected Rotator _localRotation;
        [Serialize("NearZ")]
        protected float _nearZ;
        [Serialize("FarZ")]
        protected float _farZ;
        [Serialize("PostProcessSettings")]
        private PostProcessSettings _postProcessSettings;

        private void _viewTarget_Changed()
            => SetRotationWithTarget(_viewTarget.Raw);
        private void _owningComponent_WorldTransformChanged()
        {
            _forwardInvalidated = true;
            _upInvalidated = true;
            _rightInvalidated = true;
            UpdateTransformedFrustum();
            if (!_updating)
                OnTransformChanged();
        }

        /// <summary>
        /// Returns an X, Y coordinate relative to the camera's Origin, with Z being the normalized depth from NearDepth to FarDepth.
        /// </summary>
        public Vec3 WorldToScreen(Vec3 point)
            => _projectionOrigin + _projectionRange * ((_projectionMatrix * (WorldMatrix * point)) + 1.0f) / 2.0f;
        public Vec3 ScreenToWorld(Vec2 point, float depth) 
            => ScreenToWorld(point.X, point.Y, depth);
        public Vec3 ScreenToWorld(float x, float y, float depth)
            => ScreenToWorld(new Vec3(x, y, depth));
        public Vec3 ScreenToWorld(Vec3 screenPoint)
            => InverseWorldMatrix * (_projectionInverse * ((screenPoint - _projectionOrigin) / _projectionRange * 2.0f - 1.0f));
        
        protected virtual void PositionChanged()
        {
            if (_viewTarget != null)
                _localRotation.SetRotationsNoUpdate((_viewTarget.Raw - _localPoint).LookatAngles());
            CreateTransform();
        }
        protected virtual void CreateTransform()
        {
            Matrix4 rotMatrix = _localRotation.GetMatrix();
            _transform = Matrix4.CreateTranslation(_localPoint.Raw) * rotMatrix;
            _invTransform = _localRotation.GetInverseMatrix() * Matrix4.CreateTranslation(-_localPoint.Raw);
            UpdateTransformedFrustum();
            OnTransformChanged();
        }
        protected void UpdateTransformedFrustum()
            => _transformedFrustum.TransformedVersionOf(_untransformedFrustum, WorldMatrix);

        public abstract float DistanceScale(Vec3 point, float radius);
        public abstract void Zoom(float amount);

        public void TranslateRelative(float x, float y, float z) 
            => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            _transform = _transform * Matrix4.CreateTranslation(translation);
            _invTransform = Matrix4.CreateTranslation(-translation) * _invTransform;
            _localPoint.SetRawNoUpdate(_transform.GetPoint());
            if (_viewTarget != null)
                SetRotationWithTarget(_viewTarget.Raw);
            else
            {
                UpdateTransformedFrustum();
                OnTransformChanged();
            }
        }

        public void TranslateAbsolute(float x, float y, float z) 
            => TranslateAbsolute(new Vec3(x, y, z));
        public void TranslateAbsolute(Vec3 translation)
            => _localPoint.Raw += translation;
        
        public Vec3 RotateVector(Vec3 dir)
            => Vec3.TransformVector(dir, WorldMatrix).NormalizedFast();
        public Vec3 GetUpVector()
        {
            if (_upInvalidated)
            {
                _upDirection = RotateVector(Vec3.Up);
                _upInvalidated = false;
            }
            return _upDirection;
        }
        public Vec3 GetForwardVector()
        {
            if (_forwardInvalidated)
            {
                _forwardDirection = RotateVector(Vec3.Forward);
                _forwardInvalidated = false;
            }
            return _forwardDirection;
        }
        public Vec3 GetRightVector()
        {
            if (_rightInvalidated)
            {
                _rightDirection = RotateVector(Vec3.Right);
                _rightInvalidated = false;
            }
            return _rightDirection;
        }

        public void AddRotation(float pitch, float yaw) 
            => AddRotation(pitch, yaw, 0.0f);
        public void AddRotation(float pitch, float yaw, float roll) 
            => SetRotation(
                _localRotation.Pitch + pitch,
                _localRotation.Yaw + yaw, 
                _localRotation.Roll + roll);

        public void SetRotation(Rotator r) 
            => _localRotation.SetRotations(r);
        public void SetRotation(Vec3 pitchYawRoll)
            => _localRotation.PitchYawRoll = pitchYawRoll;
        public void SetRotation(float pitch, float yaw, float roll)
            => _localRotation.SetRotations(pitch, yaw, roll);
        public void SetRotationWithTarget(Vec3 target)
        {
            //if (_owningComponent != null)
            //    target = Vec3.TransformPosition(target, _owningComponent.InverseWorldMatrix);
            SetRotation((target - _localPoint).LookatAngles());
        }
        public void Pivot(float y, float x, float radius)
        {
            BeginUpdate();
            Zoom(-radius);
            AddRotation(y, x);
            Zoom(radius);
            EndUpdate();
        }
        
        public void Reset()
        {
            _localPoint.Raw = Vec3.Zero;
            _localRotation.YawPitchRoll = Vec3.Zero;
        }
        public void SaveCurrentTransform() { }

        public event TranslationChange TranslationChanged;
        public event RotationChange RotationChanged;
        //public event Action Resized;
        public event Action TransformChanged;

        protected void OnTransformChanged()
        {
            _forwardInvalidated = _upInvalidated = _rightInvalidated = true;
            _updating = true;
            TransformChanged?.Invoke();
            _updating = false;
        }
        protected void OnTranslationChanged(Vec3 oldTranslation) 
            => TranslationChanged?.Invoke(oldTranslation);
        protected void OnRotationChanged(Rotator oldRotation)
            => RotationChanged?.Invoke(oldRotation);
        
        public virtual void SetUniforms()
        {
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ViewMatrix),     InverseWorldMatrix);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ProjMatrix),     ProjectionMatrix);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ScreenWidth),    Width);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ScreenHeight),   Height);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ScreenOrigin),   Origin);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraNearZ),    NearZ);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraFarZ),     FarZ);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraPosition), WorldPoint);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraForward),  GetForwardVector());
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraUp),       GetUpVector());
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraRight),    GetRightVector());
        }
        [PostDeserialize]
        protected virtual void CalculateProjection()
        {
            _projectionRange = new Vec3(Dimensions, FarZ - NearZ);
            _projectionOrigin = new Vec3(Origin, NearZ);
            _untransformedFrustum = CreateUntransformedFrustum();
            UpdateTransformedFrustum();
        }
        //Child camera types must override this
        public virtual void Resize(float width, float height)
            => CalculateProjection();
        protected abstract Frustum CreateUntransformedFrustum();
        public Frustum GetFrustum() 
            => _transformedFrustum;
        private void BeginUpdate() 
            =>_updating = true;
        private void EndUpdate()
        {
            _updating = false;
            TransformChanged?.Invoke();
        }

        public Segment GetWorldSegment(Vec2 screenPoint)
        {
            Vec3 start = ScreenToWorld(screenPoint, 0.0f);
            Vec3 end = ScreenToWorld(screenPoint, 1.0f);
            return new Segment(start, end);
        }
        public Ray GetWorldRay(Vec2 screenPoint)
        {
            Vec3 start = ScreenToWorld(screenPoint, 0.0f);
            Vec3 end = ScreenToWorld(screenPoint, 1.0f);
            return new Ray(start, end - start);
        }
        public Vec3 GetPointAtDepth(Vec2 screenPoint, float depth)
            => ScreenToWorld(screenPoint, depth);
        public Vec3 GetPointAtDistance(Vec2 screenPoint, float distance)
            => GetWorldSegment(screenPoint).PointAtLineDistance(distance);

        public void Render()
        {
            _transformedFrustum.Render();
            if (_viewTarget != null)
                Engine.Renderer.RenderLine(_transformedFrustum.RenderName + "_ViewTarget", WorldPoint, _viewTarget.Raw, Color.DarkGray, 10.0f);
        }
    }
}
