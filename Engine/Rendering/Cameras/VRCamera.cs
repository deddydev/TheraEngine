using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Cameras
{
    public class VRCamera : Camera
    {
        public VRCamera() : base() { }

        [Browsable(false)]
        public override Vec2 Origin => new Vec2(Width / 2.0f, Height / 2.0f);

        public override float Width => 1.0f;
        public override float Height => 1.0f;
        
        public override float NearZ { get; set; }
        public override float FarZ { get; set; }
        public override bool UsesAutoExposure { get; }

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

        public override void Render(bool shadowPass)
        {
            _transformedFrustum.Render(shadowPass);
        }
        public override void SetUniforms(RenderProgram program)
        {
            base.SetUniforms(program);
            //program.Uniform(EEngineUniform.CameraFovX, FovX);
            //program.Uniform(EEngineUniform.CameraFovY, FovY);
            //program.Uniform(EEngineUniform.CameraAspect, Aspect);
        }
        public override void Resize(float width, float height)
        {
            //_width = width;
            //_height = height;
            //if (!_overrideAspect)
            //    _aspect = _width / _height;
            //if (_transformedFrustumCascade is null)
            //    InitFrustumCascade();
            base.Resize(width, height);
        }
        protected override IFrustum CreateUntransformedFrustum()
        {
            //const bool transformed = false;
            //return new Frustum(_fovY, _aspect, NearZ, FarZ,
            //    transformed ? ForwardVector : Vec3.Forward,
            //    transformed ? UpVector : Vec3.Up,
            //    transformed ? _localPoint.Raw : Vec3.Zero);
            return new Frustum();
        }

        public override void RebaseOrigin(Vec3 newOrigin)
        {

        }
        protected override void OnCreateTransform(out Matrix4 cameraToWorldSpaceMatrix, out Matrix4 worldToCameraSpaceMatrix)
        {
            cameraToWorldSpaceMatrix = Matrix4.Identity;
            worldToCameraSpaceMatrix = Matrix4.Identity;
        }
        public override void SetBloomUniforms(RenderProgram program)
        {

        }
        public override void SetAmbientOcclusionUniforms(RenderProgram program)
        {

        }
        public override void SetPostProcessUniforms(RenderProgram program)
        {

        }
        public override void UpdateExposure(TexRef2D texture)
        {

        }
    }
}
