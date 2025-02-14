﻿using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Cameras
{
    public class VRCamera : Camera
    {
        public VRCamera() : base() { }

        [Browsable(false)]
        public override Vec2 Origin => new Vec2(Width / 2.0f, Height / 2.0f);

        public override float Width => 1.0f;
        public override float Height => 1.0f;
        
        protected override void OnCalculateProjection(out Matrix4 projMatrix, out Matrix4 inverseProjMatrix)
        {
            projMatrix = _projectionMatrix;
            inverseProjMatrix = _projectionInverse;
        }

        public void SetProjectionMatrix(Matrix4 matrix)
        {
            _projectionMatrix = matrix;
            _projectionInverse = matrix.Inverted();
            CalculateProjection();
        }
        public void SetInverseProjectionMatrix(Matrix4 matrix)
        {
            _projectionInverse = matrix;
            _projectionMatrix = matrix.Inverted();
            CalculateProjection();
        }
        public void SetProjectionMatrices(Matrix4 matrix, Matrix4 inverse)
        {
            _projectionMatrix = matrix;
            _projectionInverse = inverse;
            CalculateProjection();
        }
        
        protected override IFrustum CreateUntransformedFrustum()
        {
            //const bool transformed = false;
            //return new Frustum(_fovY, _aspect, NearZ, FarZ,
            //    transformed ? ForwardVector : Vec3.Forward,
            //    transformed ? UpVector : Vec3.Up,
            //    transformed ? _localPoint.Raw : Vec3.Zero);
            return CreateUntransformedFrustum2();
        }

        public override void RebaseOrigin(Vec3 newOrigin)
        {

        }
        protected override void OnCreateTransform(out Matrix4 cameraToWorldSpaceMatrix, out Matrix4 worldToCameraSpaceMatrix)
        {
            cameraToWorldSpaceMatrix = CameraToComponentSpaceMatrix;
            worldToCameraSpaceMatrix = ComponentToCameraSpaceMatrix;
        }
    }
}
