using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Worlds.Actors
{
    public class SpotLightComponent : LightComponent
    {
        private float _outerCutoff, _innerCutoff, _exponent, _brightness, _distance;
        private Vec3 _direction;

        //[DragRange(0.1f, 1000.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Spotlight Component")]
        public float Distance
        {
            get => _distance;
            set => _distance = value;
        }
        [Category("Spotlight Component")]
        public Vec3 Direction
        {
            get => _direction;
            set
            {
                _direction = value.NormalizedFast();
                _rotation.SetDirection(_direction);
            }
        }
        //[DragRange(0.0f, 1000.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Spotlight Component")]
        public float Exponent
        {
            get => _exponent;
            set => _exponent = value;
        }
        //[DragRange(0.1f, 1000.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Spotlight Component")]
        public float Brightness
        {
            get => _brightness;
            set => _brightness = value;
        }
        //[DragRange(0.0f, 90.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Spotlight Component")]
        public float OuterCutoffAngleDegrees
        {
            get => CustomMath.RadToDeg((float)Math.Acos(_outerCutoff));
            set => _outerCutoff = (float)Math.Cos(CustomMath.DegToRad(value));
        }
        //[DragRange(0.0f, 90.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Spotlight Component")]
        public float InnerCutoffAngleDegrees
        {
            get => CustomMath.RadToDeg((float)Math.Acos(_innerCutoff));
            set => _innerCutoff = (float)Math.Cos(CustomMath.DegToRad(value));
        }

        public SpotLightComponent(
            float distance, ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Vec3 direction, float outerCutoffDeg, float innerCutoffDeg, float brightness, float exponent) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            OuterCutoffAngleDegrees = outerCutoffDeg;
            InnerCutoffAngleDegrees = innerCutoffDeg;
            _distance = distance;
            _brightness = brightness;
            _cullingVolume = new ConeZ((float)Math.Tan(CustomMath.DegToRad(outerCutoffDeg)) * _distance, _distance);
            Direction = direction;
            _exponent = exponent;
            //_cullingVolume.State.Rotation.SyncFrom(_rotation);
            //_cullingVolume.State.Translation.SyncFrom(_translation);
        }
        public SpotLightComponent(
            float distance, ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Rotator rotation, float outerCutoffDeg, float innerCutoffDeg, float brightness, float exponent)
            : base(color, diffuseIntensity, ambientIntensity)
        {
            OuterCutoffAngleDegrees = outerCutoffDeg;
            InnerCutoffAngleDegrees = innerCutoffDeg;
            _distance = distance;
            _brightness = brightness;
            _cullingVolume = new ConeZ((float)Math.Tan(CustomMath.DegToRad(outerCutoffDeg)) * _distance, _distance);
            _rotation.SetRotations(rotation);
            _exponent = exponent;
            //_cullingVolume.State.Rotation.SyncFrom(_rotation);
            //_cullingVolume.State.Translation.SyncFrom(_translation);
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            _direction = _rotation.GetDirection();
            _cullingVolume.State.Rotation.SetDirection(-_direction);
            _cullingVolume.State.Translation.Raw = _translation + _direction * (_distance / 2.0f);
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }

        private MaterialFrameBuffer _shadowMap;
        private PerspectiveCamera _shadowCamera;
        private int _shadowWidth, _shadowHeight;
        private Matrix4 _worldToLightSpaceProjMatrix;
        internal protected ConeZ _cullingVolume;

        public override void OnSpawned()
        {
            if (_type == LightType.Dynamic)
            {
                Engine.Scene.Lights.Add(this);
                
                SetShadowMapResolution(2048, 2048);

                if (Engine.Settings.RenderCameraFrustums)
                    Engine.Scene.Add(_shadowCamera);
            }
        }
        public override void OnDespawned()
        {
            if (_type == LightType.Dynamic)
            {
                Engine.Scene.Lights.Remove(this);
                if (Engine.Settings.RenderCameraFrustums)
                    Engine.Scene.Remove(_shadowCamera);
            }
        }
        public override void SetUniforms(int programBindingId)
        {
            Vec3 point = WorldMatrix.GetPoint();

            string indexer = Uniform.SpotLightsName + "[" + _lightIndex + "].";

            Engine.Renderer.Uniform(programBindingId, indexer + "Direction", _direction);
            Engine.Renderer.Uniform(programBindingId, indexer + "OuterCutoff", _outerCutoff);
            Engine.Renderer.Uniform(programBindingId, indexer + "InnerCutoff", _innerCutoff);
            Engine.Renderer.Uniform(programBindingId, indexer + "Exponent", _exponent);

            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Position", point);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Radius", _distance);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Brightness", _brightness);

            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Base.Color", _color.Raw);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Base.WorldToLightSpaceProjMatrix", _worldToLightSpaceProjMatrix);

            _shadowMap.Material.SetTextureUniform(0, Viewport.GBufferTextureCount + Engine.Scene.Lights.DirectionalLights.Count + LightIndex, indexer + "Base.Base.ShadowMap", programBindingId);
        }
        public void SetShadowMapResolution(int width, int height)
        {
            _shadowWidth = width;
            _shadowHeight = height;
            if (_shadowMap == null)
                _shadowMap = new MaterialFrameBuffer(GetShadowMapMaterial(width, height));
            else
                _shadowMap.ResizeTextures(width, height);

            if (_shadowCamera == null)
            {
                _shadowCamera = new PerspectiveCamera(0.1f, _distance - 0.1f, OuterCutoffAngleDegrees * 2.0f, 1.0f);
                _shadowCamera.LocalRotation.SyncFrom(_rotation);
                _shadowCamera.LocalPoint.SyncFrom(_translation);
                _shadowCamera.ProjectionChanged += UpdateMatrix;
                _shadowCamera.TransformChanged += UpdateMatrix;
                UpdateMatrix();
            }
        }
        private void UpdateMatrix()
            => _worldToLightSpaceProjMatrix = _shadowCamera.ProjectionMatrix * _shadowCamera.WorldToCameraSpaceMatrix;

        private static EPixelInternalFormat GetFormat(EDepthPrecision precision)
        {
            switch (precision)
            {
                case EDepthPrecision.Int16: return EPixelInternalFormat.DepthComponent16;
                case EDepthPrecision.Int24: return EPixelInternalFormat.DepthComponent24;
                case EDepthPrecision.Int32: return EPixelInternalFormat.DepthComponent32;
            }
            return EPixelInternalFormat.DepthComponent32f;
        }
        private static Material GetShadowMapMaterial(int width, int height, EDepthPrecision precision = EDepthPrecision.Flt32)
        {
            //These are listed in order of appearance in the shader
            TextureReference[] refs = new TextureReference[]
            {
                new TextureReference("SpotDepth", width, height,
                    GetFormat(precision), EPixelFormat.DepthComponent, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                },
            };
            Shader shader = new Shader(ShaderMode.Fragment, ShaderHelpers.Frag_Nothing);
            return new Material("SpotLightShadowMat", new ShaderVar[0], refs, shader);
        }
        public override void RenderShadowMap(SceneProcessor scene)
        {
            Engine.Renderer.MaterialOverride = _shadowMap.Material;

            _shadowMap.Bind(EFramebufferTarget.Framebuffer);
            Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, _shadowWidth, _shadowHeight, 0.0f, 0.0f));

            Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
            Engine.Renderer.AllowDepthWrite(true);

            scene.PreRender(_shadowCamera, true);
            scene.Render(ERenderPassType3D.OpaqueDeferredLit);
            scene.Render(ERenderPassType3D.OpaqueForward);
            scene.Render(ERenderPassType3D.TransparentForward);
            scene.Render(ERenderPassType3D.OnTopForward);
            scene.PostRender();

            Engine.Renderer.PopRenderArea();
            _shadowMap.Unbind(EFramebufferTarget.Framebuffer);

            Engine.Renderer.MaterialOverride = null;
        }

        public override void BakeShadowMaps()
        {

        }
    }
}
