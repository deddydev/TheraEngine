using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Uniform
    {
        public static readonly string PositionMatricesName = "PositionMatrices";
        public static readonly string NormalMatricesName = "NormalMatrices";
        public static readonly string PointLightsName = "PointLights";
        public static readonly string SpotLightsName = "SpotLights";
        public static readonly string DirectionalLightsName = "DirectionalLights";
        
        //public static int GetLocation(VertexAttribInfo info)
        //{
        //    return info.GetLocation();
        //}
        public static int GetLocation(ECommonUniform u)
        {
            return Engine.Renderer.GetUniformLocation(u.ToString());
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
        
//         InvModelMatrix,
//         InvViewMatrix,
//         InvProjMatrix,
// 
//         PrevModelMatrix,
//         PrevViewMatrix,
//         PrevProjMatrix,
// 
//         PrevInvModelMatrix,
//         PrevInvViewMatrix,
//         PrevInvProjMatrix,

        ScreenWidth,
        ScreenHeight,
        ScreenOrigin,
        CameraFovY,
        CameraFovX,
        CameraAspect,
        CameraNearZ,
        CameraFarZ,
        CameraPosition,
        CameraForward,
        RenderDelta,
    }
}
