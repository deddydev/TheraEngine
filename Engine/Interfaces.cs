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
        IKeyframeTrack Keyframes { get; }
        int FrameCount { get; set; }
    }
    public interface IKeyframeTrack
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
    public interface ISaveable
    {
        Task Save(string path);
        Task<int> CalculateSize();
        Task<int> OnCalculateSize();
        Task Write(VoidPtr address, IProgress<float> progress);
        int CalculatedSize { get; }
        bool IsSaving { get; }
        bool IsCalculatingSize { get; }
        bool IsWriting { get; }
    }
    public interface IRenderState
    {

    }
    public interface IShape : IRenderable
    {
        bool ContainsPoint(Vec3 point);
    }
}
