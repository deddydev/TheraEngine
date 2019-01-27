namespace TheraEngine.Rendering.Models.Materials
{
    public class Uniform
    {
        public static readonly string BoneTransformsName = "Transforms";
        //public static readonly string BoneNrmMtxName = "BoneNrmMtx";
        public static readonly string MorphWeightsName = "MorphWeights";
        public static readonly string LightsStructName = "LightData";
        
        //public static int GetLocation(VertexAttribInfo info)
        //{
        //    return info.GetLocation();
        //}
        public static int GetLocation(RenderProgram program, EEngineUniform u)
        {
            return program.GetUniformLocation(u.ToString());
            //return VertexBuffer.MaxBufferCount + (int)u;
        }
        //public static int GetFirstOpenUniformLocation()
        //{
        //    ECommonUniform lastEnum = Enum.GetValues(typeof(ECommonUniform)).Cast<ECommonUniform>().Max();
        //    return GetLocation(lastEnum) + 1;
        //}
    }
    public enum EMeshValue
    {
        FragPos,
        FragNorm,
        FragBinorm,
        FragTan,
        FragColor,
        FragUV,
    }
    public enum EEngineUniform
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
