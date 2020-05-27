using System.IO;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class ForwardPassVPRC : ViewportRenderCommand
    {
        private ICamera _renderingCamera;

        private QuadFrameBuffer _lightCombineFBO;
        public QuadFrameBuffer LightCombineFBO
        {
            get => _lightCombineFBO;
            private set
            {
                _lightCombineFBO?.Dispose();
                _lightCombineFBO = value;
            }
        }
        
        private QuadFrameBuffer _forwardPassFBO;
        public QuadFrameBuffer ForwardPassFBO
        {
            get => _forwardPassFBO;
            private set
            {
                _forwardPassFBO?.Dispose();
                _forwardPassFBO = value;
            }
        }

        public override void GenerateFBOs(Viewport viewport)
        {
            TexRef2D HDRSceneTexture = viewport.Pipeline.GetTexture<TexRef2D>("HDRScene");
            TexRef2D DepthStencilTexture = viewport.Pipeline.GetTexture<TexRef2D>("DepthStencil");
            LightCombineFBO = viewport.Pipeline.GetFBO<QuadFrameBuffer>("LightCombine");

            RenderingParameters renderParams = new RenderingParameters();
            renderParams.DepthTest.Enabled = ERenderParamUsage.Unchanged;
            renderParams.DepthTest.UpdateDepth = false;
            renderParams.DepthTest.Function = EComparison.Always;

            GLSLScript brightShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "BrightPass.fs"));
            TexRef2D[] brightRefs = { HDRSceneTexture };
            TMaterial brightMat = new TMaterial("BrightPassMat", renderParams, brightRefs, brightShader);

            ForwardPassFBO = new QuadFrameBuffer(brightMat);
            ForwardPassFBO.SettingUniforms += BrightPassFBO_SettingUniforms;
            ForwardPassFBO.SetRenderTargets(
                (HDRSceneTexture, EFramebufferAttachment.ColorAttachment0, 0, -1),
                (DepthStencilTexture, EFramebufferAttachment.DepthStencilAttachment, 0, -1));
        }

        public override void DestroyFBOs()
        {
            ForwardPassFBO.SettingUniforms -= BrightPassFBO_SettingUniforms;
            ForwardPassFBO?.Dispose();
            ForwardPassFBO = null;

            LightCombineFBO = null;
        }

        private void BrightPassFBO_SettingUniforms(RenderProgram program)
            => _renderingCamera?.SetBloomUniforms(program);

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            _renderingCamera = viewport.RenderingCamera;

            ForwardPassFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            {
                Engine.Renderer.EnableDepthTest(false);

                //Render the deferred pass lighting result
                LightCombineFBO.RenderFullscreen();

                Engine.Renderer.EnableDepthTest(true);
                renderingPasses.Render(ERenderPass.OpaqueForward);

                Engine.Renderer.AllowDepthWrite(false);
                renderingPasses.Render(ERenderPass.Background);
                Engine.Renderer.EnableDepthTest(true);

                //Render forward transparent objects next
                renderingPasses.Render(ERenderPass.TransparentForward);

                //Render forward on-top objects last
                renderingPasses.Render(ERenderPass.OnTopForward);
                Engine.Renderer.EnableDepthTest(false);
            }
            ForwardPassFBO.Unbind(EFramebufferTarget.DrawFramebuffer);

            _renderingCamera = null;
        }
    }
}
