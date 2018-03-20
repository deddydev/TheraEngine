namespace TheraEngine.Rendering.Models.Materials
{
    public class Uniform
    {
        public static readonly string BonePosMtxName = "BonePosMtx";
        public static readonly string BoneNrmMtxName = "BoneNrmMtx";
        public static readonly string MorphWeightsName = "MorphWeights";
        public static readonly string PointLightsName = "PointLights";
        public static readonly string SpotLightsName = "SpotLights";
        public static readonly string DirectionalLightsName = "DirLights";
        
        //public static int GetLocation(VertexAttribInfo info)
        //{
        //    return info.GetLocation();
        //}
        public static int GetLocation(int programBindingId, ECommonUniform u)
        {
            return Engine.Renderer.GetUniformLocation(programBindingId, u.ToString());
            //return VertexBuffer.MaxBufferCount + (int)u;
        }
        //public static int GetFirstOpenUniformLocation()
        //{
        //    ECommonUniform lastEnum = Enum.GetValues(typeof(ECommonUniform)).Cast<ECommonUniform>().Max();
        //    return GetLocation(lastEnum) + 1;
        //}
    }
    public enum ECommonUniform
    {
        UpdateDelta,

        ModelMatrix,
        WorldToCameraSpaceMatrix,
        ProjMatrix,
        NormalMatrix,

        InvModelMatrix,
        CameraToWorldSpaceMatrix,
        InvProjMatrix,

        PrevModelMatrix,
        PrevViewMatrix,
        PrevProjMatrix,

        PrevInvModelMatrix,
        PrevInvViewMatrix,
        PrevInvProjMatrix,

        ScreenWidth,
        ScreenHeight,
        ScreenOrigin,

        CameraFovX,
        CameraFovY,
        CameraAspect,
        CameraNearZ,
        CameraFarZ,
        CameraPosition,
        //CameraForward,
        //CameraUp,
        //CameraRight,
    }
}
