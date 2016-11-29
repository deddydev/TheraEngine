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
        public static readonly string ScreenWidthName = "ScreenWidth";
        public static readonly string ScreenHeightName = "ScreenHeight";
        public static readonly string BoneMatricesName = "BoneMatrices";
    }
}
