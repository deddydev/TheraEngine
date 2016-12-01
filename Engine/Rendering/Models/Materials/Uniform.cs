using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Uniform
    {
        public static readonly string ViewMatrixName = "ViewMatrix";
        public static readonly string ProjMatrixName = "ProjMatrix";
        public static readonly string ModelMatrixName = "ModelMatrix";
        public static readonly string BoneMatricesName = "BoneMatrices";

        private static Dictionary<ECommonUniform, Action> _commonUniforms = new Dictionary<ECommonUniform, Action>();
        private static List<ECommonUniform> _invalidatedUniforms = new List<ECommonUniform>();

        public static void RegisterCommonUniform(ECommonUniform uniform, Action setUniformMethod)
        {
            if (_commonUniforms.ContainsKey(uniform))
                _commonUniforms[uniform] = setUniformMethod;
            else
                _commonUniforms.Add(uniform, setUniformMethod);
        }
        public static void UnregisterCommonUniform(ECommonUniform uniform)
        {
            if (_commonUniforms.ContainsKey(uniform))
                _commonUniforms.Remove(uniform);
        }
        public static void InvalidateCommonUniform(ECommonUniform uniform)
        {
            _invalidatedUniforms.Add(uniform);
        }
        public static void UpdateInvalidatedCommonUniforms()
        {
            foreach (Material m in Engine.LoadedMaterials)
                foreach (ECommonUniform u in _invalidatedUniforms)
                    Engine.Renderer.ProgramUniform(m.BindingId, )
                
        }
        public static void SetCommonUniforms()
        {
            foreach (Action setFunc in _commonUniforms.Values)
                setFunc();
        }
    }
    public enum ECommonUniform
    {
        ModelMatrix,
        ViewMatrix,
        ProjMatrix,
        InvModelMatrix,
        InvViewMatrix,
        InvProjMatrix,
        PrevModelMatrix,
        PrevViewMatrix,
        PrevProjMatrix,
        PrevInvModelMatrix,
        PrevInvViewMatrix,
        PrevInv
        ScreenWidth,
        ScreenHeight,
        FovY,
        FovX,
        Aspect,
        RenderDelta,
    }
}
