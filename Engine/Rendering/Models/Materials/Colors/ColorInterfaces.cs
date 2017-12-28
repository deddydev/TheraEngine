using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models.Materials
{
    public interface IByteColor
    {
        Color Color { get; set; }
    }
    public interface IFloatColor
    {
        ColorF4 Color { get; set; }
    }

}
