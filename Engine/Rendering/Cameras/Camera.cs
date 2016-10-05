﻿using CustomEngine.Rendering;
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
        public Vec3 Translation
        {
            get { return _currentTransform.Translation; }
            set { _currentTransform.Translation = value; }
        }
        public Quaternion Rotation
        {
            get { return _currentTransform.Rotation; }
            set { _currentTransform.Rotation = value; }
        }
        public Vec3 Scale
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
        public Camera(Vec3 defaultTranslate, Quaternion defaultRotate, Vec3 defaultScale)
        {
            _currentTransform = _defaultTransform = new FrameState(defaultTranslate, defaultRotate, defaultScale);
        }
        public Camera(FrameState defaultTransform)
        {
            _currentTransform = _defaultTransform = defaultTransform;
        }

        protected abstract float GetWidth();
        protected abstract float GetHeight();
        protected virtual Vec3 AlignScreenPoint(Vec3 screenPoint) { return screenPoint; }
        protected virtual Vec3 UnAlignScreenPoint(Vec3 screenPoint) { return screenPoint; }
        public abstract void Zoom(float amount);
        public abstract void CalculateProjection();
        public virtual void Resize(float width, float height)
        {
            CalculateProjection();
            OnResized?.Invoke();
        }

        public void TranslateRelative(Vec3 v) { TranslateRelative(v.X, v.Y, v.Z); }
        public void TranslateRelative(float x, float y, float z)
        {
            MatrixInverse.Translate(x, y, z);
            _currentTransform.Translation = MatrixInverse.ExtractTranslation();
        }
        public void Rotate(float x, float y, float z) { Rotate(new Vec3(x, y, z)); }
        public void Rotate(Vec3 v)
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

        public void SetTransform(Vec3 translate, Vec3 rotate, Vec3 scale)
        {
            _currentTransform.SetAll(translate, Quaternion.FromEulerAngles(rotate), scale);
        }
        public void SetTransform(Vec3 translate, Quaternion rotate, Vec3 scale)
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
        public Vec3 GetWorldPoint(Vec3 screenPoint)
        {
            screenPoint = AlignScreenPoint(screenPoint);
            return screenPoint.Unproject(0, 0, GetWidth(), GetHeight(), NearDepth, FarDepth, MatrixInverse * _projectionInverse);
        }

        /// <summary>
        /// Projects a screen point to world coordinates.
        /// </summary>
        /// <returns>3D world point perpendicular to the camera with a depth value of z (z is not a distance value!)</returns>
        public Vec3 GetWorldPoint(float x, float y, float z)
        {
            return GetWorldPoint(new Vec3(x, y, z));
        }
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
            return UnAlignScreenPoint(worldPoint.Project(0, 0, GetWidth(), GetHeight(), NearDepth, FarDepth, _projectionMatrix * Matrix));
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
                ray.LinePlaneIntersect(center, Translation.Normalized(center), out point);

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
