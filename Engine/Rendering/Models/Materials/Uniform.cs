using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Pair<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
    public delegate void DelSetUniform(int materialId);
    public class Uniform
    {
        public static readonly string BoneMatricesName = "BoneMatrices";

        private static List<ECommonUniform> _invalidatedUniforms = new List<ECommonUniform>();
        private static Dictionary<ECommonUniform, Pair<int, DelSetUniform>> _commonUniforms 
            = new Dictionary<ECommonUniform, Pair<int, DelSetUniform>>();

        public static void ProvideUniform(ECommonUniform uniform, DelSetUniform setUniformMethod)
        {
            if (_commonUniforms.ContainsKey(uniform))
                _commonUniforms[uniform].Second = setUniformMethod;
            else
                _commonUniforms.Add(uniform, new Pair<int, DelSetUniform>(0, setUniformMethod));
        }
        public static void ClearUniforms(params ECommonUniform[] uArr)
        {
            foreach (ECommonUniform u in uArr)
                if (_commonUniforms.ContainsKey(u))
                    _commonUniforms[u].Second = null;
        }
        public static void RegisterCommonUniform(ECommonUniform uniform)
        {
            if (!_commonUniforms.ContainsKey(uniform))
                _commonUniforms.Add(uniform, new Pair<int, DelSetUniform>(1, null));
            else
                ++_commonUniforms[uniform].First;
        }
        public static void UnregisterCommonUniform(ECommonUniform uniform)
        {
            if (_commonUniforms.ContainsKey(uniform))
                --_commonUniforms[uniform].First;
        }
        public static void InvalidateCommonUniform(ECommonUniform uniform)
        {
            _invalidatedUniforms.Add(uniform);
        }
        public static void UpdateInvalidatedCommonUniforms()
        {
            if (_invalidatedUniforms.Count == 0)
                return;
            foreach (Material m in Engine.World.State._activeMaterials)
                foreach (ECommonUniform u in _invalidatedUniforms)
                {
                    var p = _commonUniforms[u];
                    if (p.First > 0 && p.Second != null)
                        p.Second(m.BindingId);
                }
            _invalidatedUniforms.Clear();
        }
        public static int GetLocation(VertexAttribInfo info)
        {
            return info.GetLocation();
        }
        public static int GetLocation(ECommonUniform u)
        {
            return VertexAttribInfo.MaxBufferCount + (int)u;
        }
        public static int GetFirstOpenUniformLocation()
        {
            ECommonUniform lastEnum = Enum.GetValues(typeof(ECommonUniform)).Cast<ECommonUniform>().Max();
            return GetLocation(lastEnum) + 1;
        }
    }
    public enum ECommonUniform
    {
        //Matrices
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
        PrevInvProjMatrix,

        //Camera
        ScreenWidth,
        ScreenHeight,
        ScreenOrigin,
        FovY,
        FovX,
        Aspect,
        NearZ,
        FarZ,

        //Engine
        RenderDelta,
    }
}
