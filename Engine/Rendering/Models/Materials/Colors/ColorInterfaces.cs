using System.Drawing;

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
