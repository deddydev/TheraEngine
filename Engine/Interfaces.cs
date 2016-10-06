using CustomEngine.Rendering.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace CustomEngine
{
    public interface IRenderable
    {
        void Render();
    }
    public interface ITransformable
    {
        FrameState Transform { get; set; }
    }
    public interface IPanel
    {
        RectangleF Region { get; set; }
        void OnResized();
    }
    public interface IPropertyAnimation
    {

    }
    public interface ILoadable
    {
        Task Load();
        Task Unload();
        string FilePath { get; }
        bool IsLoading { get; }
        bool IsLoaded { get; }
    }
    public interface IRenderState
    {

    }
    public interface INameable
    {
        string Name { get; set; }
    }
    public interface IShape : IRenderable
    {
        bool ContainsPoint(Vec3 point);
    }
}
