using System;
using System.ComponentModel;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Cameras
{
    public class BlendableCamera : Camera
    {
        private Camera _view1;
        private Camera _view2;
        private float _time;

        [Category("Blendable Camera")]
        public Camera View1
        {
            get => _view1;
            set
            {
                if (_view1 != null)
                {
                    _view1.ProjectionChanged -= CalculateProjection;
                    _view1.TransformChanged -= CreateTransform;
                    _view1.OwningComponent = null;
                }
                _view1 = value;
                if (_view1 != null)
                {
                    _view1.ProjectionChanged += CalculateProjection;
                    _view1.TransformChanged += CreateTransform;
                    _view1.OwningComponent = OwningComponent;
                }
                CalculateProjection();
            }
        }

        [Category("Blendable Camera")]
        public Camera View2
        {
            get => _view2;
            set
            {
                if (_view2 != null)
                {
                    _view2.ProjectionChanged -= CalculateProjection;
                    _view2.TransformChanged -= CreateTransform;
                    _view2.OwningComponent = null;
                }
                _view2 = value;
                if (_view2 != null)
                {
                    _view2.ProjectionChanged += CalculateProjection;
                    _view2.TransformChanged += CreateTransform;
                    _view2.OwningComponent = OwningComponent;
                }
                CalculateProjection();
            }
        }
        [Category("Blendable Camera")]
        public float LinearBlendPercentage
        {
            get => _time;
            set
            {
                _time = value;
                CalculateProjection();
            }
        }
        [Category("Blendable Camera")]
        public override float NearZ
        {
            get => Interp.Lerp(View1?.NearZ ?? 0.0f, View2?.NearZ ?? 0.0f, _time);
            set
            {
                if (View1 != null)
                    View1.NearZ = value;

                if (View2 != null)
                    View2.NearZ = value;
            }
        }
        [Category("Blendable Camera")]
        public override float FarZ
        {
            get => Interp.Lerp(View1?.FarZ ?? 0.0f, View2?.FarZ ?? 0.0f, _time);
            set
            {
                if (View1 != null)
                    View1.FarZ = value;

                if (View2 != null)
                    View2.FarZ = value;
            }
        }
        [Browsable(false)]
        public override CameraComponent OwningComponent
        {
            get => base.OwningComponent;
            set
            {
                base.OwningComponent = value;
                if (_view1 != null)
                    _view1.OwningComponent = value;
                if (_view2 != null)
                    _view2.OwningComponent = value;
            }
        }

        [Browsable(false)]
        public override float Width => Interp.Lerp(View1?.Width ?? 0.0f, View2?.Width ?? 0.0f, _time);
        [Browsable(false)]
        public override float Height => Interp.Lerp(View1?.Height ?? 0.0f, View2?.Height ?? 0.0f, _time);
        [Browsable(false)]
        public override Vec2 Origin => Interp.Lerp(View1?.Origin ?? 0.0f, View2?.Origin ?? 0.0f, _time);

        protected override void OnCreateTransform(
            out Matrix4 cameraToWorldSpaceMatrix, out Matrix4 worldToCameraSpaceMatrix)
        {
            cameraToWorldSpaceMatrix = Matrix4.Lerp(
                View1?.CameraToWorldSpaceMatrix ?? Matrix4.Identity,
                View2?.CameraToWorldSpaceMatrix ?? Matrix4.Identity,
                _time);

            worldToCameraSpaceMatrix = Matrix4.Lerp(
                View1?.WorldToCameraSpaceMatrix ?? Matrix4.Identity,
                View2?.WorldToCameraSpaceMatrix ?? Matrix4.Identity,
                _time);
        }
        protected override void OnCalculateProjection(
            out Matrix4 projMatrix, out Matrix4 inverseProjMatrix)
        {
            projMatrix = Matrix4.Lerp(
                View1?.ProjectionMatrix ?? Matrix4.Identity,
                View2?.ProjectionMatrix ?? Matrix4.Identity,
                _time);

            inverseProjMatrix = Matrix4.Lerp(
               View1?.InverseProjectionMatrix ?? Matrix4.Identity,
               View2?.InverseProjectionMatrix ?? Matrix4.Identity,
               _time);
        }

        protected override IFrustum CreateUntransformedFrustum()
            => Core.Shapes.Frustum.Lerp(
                View1?.UntransformedFrustum ?? new Frustum(),
                View2?.UntransformedFrustum ?? new Frustum(),
                _time);

        public override void RebaseOrigin(Vec3 newOrigin)
        {
            _updating = true;
            View1?.RebaseOrigin(newOrigin);
            View2?.RebaseOrigin(newOrigin);
            _updating = false;
            CreateTransform();
        }

        public override void Resize(float width, float height)
        {
            _updating = true;
            View1?.Resize(width, height);
            View2?.Resize(width, height);
            _updating = false;
            CalculateProjection();
        }

        public override void SetAmbientOcclusionUniforms(RenderProgram program)
        {
            View1?.SetAmbientOcclusionUniforms(program);
            View2?.SetAmbientOcclusionUniforms(program);
        }
        public override void SetBloomUniforms(RenderProgram program)
        {
            View1?.SetBloomUniforms(program);
            View2?.SetBloomUniforms(program);
        }
        public override void SetPostProcessUniforms(RenderProgram program)
        {
            View1?.SetPostProcessUniforms(program);
            View2?.SetPostProcessUniforms(program);
        }

        public override bool UsesAutoExposure =>
            (View1?.UsesAutoExposure ?? false) ||
            (View2?.UsesAutoExposure ?? false);

        public override void UpdateExposure(TexRef2D texture)
        {
            View1?.UpdateExposure(texture);
            View2?.UpdateExposure(texture);
        }
    }
}
