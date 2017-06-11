using TheraEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.OpenGL
{
    public static class GLEnumExtension
    {
        //Converts one enum to the other using each name.
        public static Enum Convert(this Enum e, Type otherEnum)
        {
            return Enum.Parse(otherEnum, e.ToString()) as Enum;
        }
    }
}
