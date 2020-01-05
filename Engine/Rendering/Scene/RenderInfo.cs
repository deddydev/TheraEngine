using System;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering
{
    public enum EEditorVisibility
    {
        Unchanged,
        VisibleAlways,
        VisibleOnlyWhenSelected,
    }
    public interface IRenderInfo : IFileObject
    {
        int SceneID { get; set; }
        bool VisibleByDefault { get; set; }
        bool Visible { get; set; }
#if EDITOR
        bool VisibleInEditorOnly { get; set; }
        EEditorVisibility EditorVisibilityMode { get; set; }
#endif
        DateTime LastRenderedTime { get; set; }

        TimeSpan GetTimeSinceLastRender();
        bool VisibleOnScreen { get; }
    }
    public abstract class RenderInfo : TFileObject, IRenderInfo
    {
        [Browsable(false)]
        public int SceneID { get; set; } = -1;
        [TSerialize]
        public virtual bool VisibleByDefault { get; set; } = true;
        [TSerialize(State = true, Config = false)]
        public virtual bool Visible { get; set; } = false;
#if EDITOR
        [TSerialize]
        public virtual bool VisibleInEditorOnly { get; set; } = false;
        [TSerialize]
        public EEditorVisibility EditorVisibilityMode { get; set; } = EEditorVisibility.Unchanged;
#endif
        [Browsable(false)]
        public DateTime LastRenderedTime { get; set; }
        public TimeSpan GetTimeSinceLastRender() => DateTime.Now - LastRenderedTime;
        public bool VisibleOnScreen => GetTimeSinceLastRender().TotalMilliseconds < Engine.RenderPeriod; //33.33ms per frame = 30 fps
    }
    public delegate float DelGetSortOrder(bool shadowPass);
    public delegate void DelCullingVolumeChanged(TShape oldVolume, TShape newVolume);
}
