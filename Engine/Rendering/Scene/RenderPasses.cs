﻿using Extensions;
using System.Collections.Generic;
using TheraEngine.Core;

namespace TheraEngine.Rendering
{
    public enum ERenderPass
    {
        /// <summary>
        /// Use for any objects that will ALWAYS be rendered behind the scene, even if they are outside of the viewing frustum.
        /// </summary>
        Background,
        /// <summary>
        /// Use for any fully opaque objects that are always lit.
        /// </summary>
        OpaqueDeferredLit,
        /// <summary>
        /// Renders right after all opaque deferred objects.
        /// More than just decals can be rendered in this pass, it is simply for deferred renderables after all opaque deferred objects have been rendered.
        /// </summary>
        DeferredDecals,
        /// <summary>
        /// Use for any opaque objects that you need special lighting for (or no lighting at all).
        /// </summary>
        OpaqueForward,
        /// <summary>
        /// Use for all objects that use alpha translucency
        /// </summary>
        TransparentForward,
        /// <summary>
        /// Renders on top of everything that has been previously rendered.
        /// </summary>
        OnTopForward,
    }
    public class RenderPasses : TObjectSlim
    {
        public RenderPasses()
        {
            _nearToFarSorter = new RenderSortNearToFar();
            _farToNearSorter = new RenderSortFarToNear();
            _updatingPasses = new SortedSet<RenderCommand>[]
            {
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_farToNearSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
            };
            _renderingPasses = new SortedSet<RenderCommand>[]
            {
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_farToNearSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
            };
        }

        public bool IsShadowPass { get; internal set; }

        //internal bool HasItemsToRender => _renderingPasses.Any(x => x.Count > 0);
        //public int NumTotalCommandsAdded { get; private set; }
        private int _numCommandsRecentlyAdded = 0;

        private readonly RenderSortNearToFar _nearToFarSorter;
        private readonly RenderSortFarToNear _farToNearSorter;
        private SortedSet<RenderCommand>[] _updatingPasses;
        private SortedSet<RenderCommand>[] _renderingPasses;

        private class RenderSortFarToNear : IComparer<RenderCommand>
        {
            int IComparer<RenderCommand>.Compare(RenderCommand x, RenderCommand y) => -x.CompareTo(y);
        }
        private class RenderSortNearToFar : IComparer<RenderCommand>
        {
            int IComparer<RenderCommand>.Compare(RenderCommand x, RenderCommand y) => x.CompareTo(y);
        }

        public void Add(RenderCommand item)
        {
            int index = (int)item.RenderPass;
            var set = _updatingPasses[index];
            //lock (set)
            //{
            set.Add(item);
            //}
            ++_numCommandsRecentlyAdded;
            //++NumTotalCommandsAdded;
        }
        internal int GetCommandsAddedCount()
        {
            int added = _numCommandsRecentlyAdded;
            _numCommandsRecentlyAdded = 0;
            return added;
        }
        internal void Render(ERenderPass pass)
        {
            var list = _renderingPasses[(int)pass];
            list.ForEach(x => x.Render(IsShadowPass));
            list.Clear();
        }
        internal void ClearRendering(ERenderPass pass)
        {
            var list = _renderingPasses[(int)pass];
            list.Clear();
        }
        internal void ClearUpdating(ERenderPass pass)
        {
            var list = _updatingPasses[(int)pass];
            list.Clear();
            _numCommandsRecentlyAdded = 0;
            //NumTotalCommandsAdded = 0;
        }
        internal void SwapBuffers()
        {
            ClearRenderList();
            THelpers.Swap(ref _updatingPasses, ref _renderingPasses);
        }
        internal void ClearRenderList()
            => _renderingPasses.ForEach(x => x.Clear());
        internal void ClearUpdateList()
            => _updatingPasses.ForEach(x => x.Clear());
    }
}