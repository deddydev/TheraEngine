﻿namespace TheraEngine.Rendering.Models.Materials
{
    public class Uniform
    {
        public static readonly string BoneMatricesName = "BoneMatrices";
        public static readonly string BoneMatricesITName = "BoneMatricesIT";
        public static readonly string MorphWeightsName = "MorphWeights";
        public static readonly string PointLightsName = "PointLights";
        public static readonly string SpotLightsName = "SpotLights";
        public static readonly string DirectionalLightsName = "DirectionalLights";
        
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
        ModelMatrix,
        ViewMatrix,
        ProjMatrix,
        NormalMatrix,

        InvModelMatrix,
        InvViewMatrix,
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
        CameraForward,
        CameraUp,
        CameraRight,

        RenderDelta,

        ProjOrigin,
        ProjRange,
    }
}
