using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheraEngine.Core;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public delegate void DelRender(RenderPasses renderingPasses, Camera camera, Viewport viewport, FrameBuffer target);
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
        /// Use for any opaque objects that you need special lighting for (or no lighting at all).
        /// </summary>
        OpaqueForward,
        /// <summary>
        /// Use for all objects that use alpha translucency! Material.HasTransparency will help you determine this.
        /// </summary>
        TransparentForward,
        /// <summary>
        /// Renders on top of everything that has been previously rendered.
        /// </summary>
        OnTopForward,
    }
    public class RenderPasses
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
                new SortedSet<RenderCommand>(_farToNearSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
            };
            _renderingPasses = new SortedSet<RenderCommand>[]
            {
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
                new SortedSet<RenderCommand>(_farToNearSorter),
                new SortedSet<RenderCommand>(_nearToFarSorter),
            };
        }

        public bool ShadowPass { get; internal set; }
        public bool HasItemsToRender => _renderingPasses.Any(x => x.Count > 0);

        private readonly RenderSortNearToFar _nearToFarSorter;
        private readonly RenderSortFarToNear _farToNearSorter;
        private SortedSet<RenderCommand>[] _updatingPasses;
        private SortedSet<RenderCommand>[] _renderingPasses;

        public class RenderSortFarToNear : IComparer<RenderCommand>
        {
            int IComparer<RenderCommand>.Compare(RenderCommand x, RenderCommand y) => -x.CompareTo(y);
        }
        public class RenderSortNearToFar : IComparer<RenderCommand>
        {
            int IComparer<RenderCommand>.Compare(RenderCommand x, RenderCommand y) => x.CompareTo(y);
        }

        public void Render(ERenderPass pass)
        {
            var list = _renderingPasses[(int)pass];
            list.ForEach(x => x.Render());
            list.Clear();
        }
        public void ClearRendering(ERenderPass pass)
        {
            var list = _renderingPasses[(int)pass];
            list.Clear();
        }
        public void ClearUpdating(ERenderPass pass)
        {
            var list = _updatingPasses[(int)pass];
            list.Clear();
        }

        public void Add(RenderCommand item, ERenderPass pass)
            => _updatingPasses[(int)pass].Add(item);

        public void SwapBuffers()
            => THelpers.Swap(ref _updatingPasses, ref _renderingPasses);

        public void ClearRenderList()
        {
            _renderingPasses.ForEach(x => x.Clear());
        }
        public void ClearUpdateList()
        {
            _updatingPasses.ForEach(x => x.Clear());
        }
    }
    /// <summary>
    /// Use for calculating something right before *anything* in the scene is rendered.
    /// Generally used for setting up data for a collection of sub-renderables just before they are rendered separately.
    /// </summary>
    public interface IPreRendered
    {
        void PreRenderUpdate(Camera camera);
        void PreRenderSwap();
        void PreRender(Viewport viewport, Camera camera);
    }
    public abstract class BaseScene
    {
        private static List<TMaterial> _activeMaterials = new List<TMaterial>();
        private static Queue<int> _removedIds = new Queue<int>();
        protected List<IPreRendered> _preRenderList = new List<IPreRendered>();

        public DelRender Render { get; protected set; }
        public abstract int Count { get; }

        //public int AddActiveMaterial(TMaterial material)
        //{
        //    int id = _removedIds.Count > 0 ? _removedIds.Dequeue() : _activeMaterials.Count;
        //    _activeMaterials.Add(material);
        //    return id;
        //}
        //public void RemoveActiveMaterial(TMaterial material)
        //{
        //    _removedIds.Enqueue(material.UniqueID);
        //    _activeMaterials.RemoveAt(material.UniqueID);
        //}
        public abstract void CollectVisible(RenderPasses passes, IVolume collectionVolume, Camera camera, bool shadowPass);
        public void Update(RenderPasses passes, IVolume collectionVolume, Camera camera)
        {
            CollectVisible(passes, collectionVolume, camera, false);
            PreRenderUpdate(camera);
        }
        public async void PreRenderUpdate(Camera camera)
        {
            //TODO: prerender on own consistent animation thread
            ParallelLoopResult result = await Task.Run(() => Parallel.ForEach(_preRenderList, p => { p.PreRenderUpdate(camera); }));
            //foreach (IPreRendered p in _preRenderList)
            //    p.PreRenderUpdate(camera);
        }
        public void PreRenderSwap()
        {
            foreach (IPreRendered p in _preRenderList)
                p.PreRenderSwap();
        }
        public void PreRender(Viewport viewport, Camera camera)
        {
            foreach (IPreRendered p in _preRenderList)
                p.PreRender(viewport, camera);
        }
        public void AddPreRenderedObject(IPreRendered obj)
        {
            if (obj == null)
                return;
            if (!_preRenderList.Contains(obj))
                _preRenderList.Add(obj);
        }
        public void RemovePreRenderedObject(IPreRendered obj)
        {
            if (obj == null)
                return;
            if (_preRenderList.Contains(obj))
                _preRenderList.Remove(obj);
        }
    }
}
