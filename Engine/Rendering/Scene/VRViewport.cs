using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering.Scene
{
    public class VRViewport : Viewport
    {
        public PerspectiveCamera LeftEyeCamera { get; } = new PerspectiveCamera();
        public PerspectiveCamera RightEyeCamera { get; } = new PerspectiveCamera();
        
        public VRViewport(): base(null, 0)
        {

        }
        protected override void OnRender(IScene scene, ICamera camera, IUserInterfacePawn hud, FrameBuffer target)
        {
            //base.OnRender(scene, camera, hud, target);

            //ETextureType renderSystemType = ETextureType.OpenGL;
            //var bounds = new VRTextureBounds_t() { uMax = 1, vMax = 1 };
            //var texture = new Texture_t() { eColorSpace = EColorSpace.Gamma, eType = renderSystemType, handle = _renderTarget.GetSurface() };
            //var error = OpenVR.Compositor.Submit(EVREye.Eye_Left, ref texture, ref bounds, EVRSubmitFlags.Submit_Default);
        }
    }
}
