using System.Collections;
using System.Collections.Generic;
using TheraEngine.Actors;
using TheraEngine.Input;
using Extensions;
using System.ComponentModel;
using TheraEngine.Worlds;
using System;
using System.Collections.Concurrent;

namespace TheraEngine.Rendering
{
    public interface IRenderHandler : IObjectSlim
    {
        RenderContext Context { get; set; }
        int Width { get; }
        int Height { get; }
        List<ELocalPlayerIndex> ValidPlayerIndices { get; set; }
        IReadOnlyDictionary<ELocalPlayerIndex, Viewport> Viewports { get; }

        void Render();
        void PreRender();
        void SwapBuffers();

        void GotFocus();
        void LostFocus();
        void MouseEnter();
        void MouseLeave();

        void Resize(int width, int height);

        Viewport GetOrAddViewport(ELocalPlayerIndex index);
        Viewport GetViewport(ELocalPlayerIndex index);

        void RegisterController(LocalPlayerController controller);
        void UnregisterController(LocalPlayerController controller);

        void Closed();
    }
    /// <summary>
    /// A render handler is what handles processing a visuals
    /// for a renderer to display on the UI through the render context.
    /// </summary>
    public abstract class BaseRenderHandler : TObjectSlim, IRenderHandler, IEnumerable<Viewport>
    {
        public virtual int MaxViewports => 4;
        public ERenderLibrary RenderLibrary { get; set; } = ERenderLibrary.OpenGL;

        /// <summary>
        /// The RenderContext for this handler 
        /// is what handles tying a renderer to the UI.
        /// </summary>
        public virtual RenderContext Context
        {
            get => _context;
            set => _context = value;
        }
        private RenderContext _context;
        private WorldManager _worldManager;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public abstract void Render();
        public abstract void PreRender();
        public abstract void SwapBuffers();

        public virtual void GotFocus() { }
        public virtual void LostFocus() { }
        public virtual void MouseEnter() { }
        public virtual void MouseLeave() { }

        #region Viewports
        public Viewport GetOrAddViewport(ELocalPlayerIndex index)
        {
            //Sometimes this method is accessed by separate threads at the same time.
            //Lock to ensure one viewport is added before the other thread checks for its existence.
            //This method probably won't be called every frame so this shouldn't be a speed issue.
            //lock (_viewports)
            //{
            return GetViewport(index) ?? AddViewport(index);
            //}
        }

        public Viewport GetViewport(ELocalPlayerIndex index)
            => Viewports.ContainsKey(index) ? Viewports[index] : null;

        private Viewport AddViewport(ELocalPlayerIndex index)
        {
            if (Viewports.Count == MaxViewports)
                return null;

            Viewport newViewport = new Viewport(this, Viewports.Count);
            Viewports.TryAdd(index, newViewport);

            Engine.PrintLine("Added new viewport to {0}: {1}", GetType().GetFriendlyName(), newViewport.Index);

            //Fix the regions of the rest of the viewports
            var twoPlayerPref = Engine.Game?.TwoPlayerPref ?? Viewport.ETwoPlayerPreference.SplitHorizontally;
            var threePlayerPref = Engine.Game?.ThreePlayerPref ?? Viewport.EThreePlayerPreference.PreferFirstPlayer;
            int i = 0;
            foreach (var p in Viewports)
            {
                p.Value.ViewportCountChanged(i, Viewports.Count, twoPlayerPref, threePlayerPref);
                p.Value.Resize(Width, Height);
                p.Value.PlayerIndex = p.Key;
                ++i;
            }

            return newViewport;
        }

        public void RegisterController(LocalPlayerController controller)
            => GetOrAddViewport(controller.LocalPlayerIndex)?.RegisterController(controller);

        public void UnregisterController(LocalPlayerController controller)
        {
            if (controller.Viewport != null && Viewports.Values.Contains(controller.Viewport))
            {
                Viewport v = controller.Viewport;
                v.UnregisterController(controller);

                //if (v.Owners.Count == 0)
                //{
                //    _viewports.Remove(v);
                //    for (int i = 0; i < _viewports.Count; ++i)
                //    {
                //        Viewport p = _viewports[i];
                //        p.ViewportCountChanged(i, _viewports.Count, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);
                //        p.Resize(Width, Height);
                //    }
                //}
            }
        }
        #endregion

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<ELocalPlayerIndex> ValidPlayerIndices { get; set; } = new List<ELocalPlayerIndex>()
        {
            ELocalPlayerIndex.One,
            ELocalPlayerIndex.Two,
            ELocalPlayerIndex.Three,
            ELocalPlayerIndex.Four,
        };

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ConcurrentDictionary<ELocalPlayerIndex, Viewport> Viewports { get; private set; } = new ConcurrentDictionary<ELocalPlayerIndex, Viewport>();

        public WorldManager WorldManager
        {
            get => _worldManager;
            internal set
            {
                OnWorldManagerPreChanged();
                _worldManager = value;
                OnWorldManagerPostChanged();
            }
        }

        IReadOnlyDictionary<ELocalPlayerIndex, Viewport> IRenderHandler.Viewports => Viewports;

        public event Action WorldManagerPreChanged;
        public event Action WorldManagerPostChanged;
        protected virtual void OnWorldManagerPreChanged() => WorldManagerPreChanged?.Invoke();
        protected virtual void OnWorldManagerPostChanged() => WorldManagerPostChanged?.Invoke();

        public IEnumerator<Viewport> GetEnumerator() => ((IEnumerable<Viewport>)Viewports).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Viewport>)Viewports).GetEnumerator();

        public virtual void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            foreach (Viewport v in Viewports.Values)
                v.Resize(Width, Height, true);
        }

        public virtual void Closed() { }
    }
}
