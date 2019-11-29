using Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public delegate void DelRender(RenderPasses renderingPasses, ICamera camera, Viewport viewport, FrameBuffer target);
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
        /// Use for all objects that use alpha translucency! Material.HasTransparency will help you determine this.
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

        public bool ShadowPass { get; internal set; }
        public bool HasItemsToRender => _renderingPasses.Any(x => x.Count > 0);
        //public int NumTotalCommandsAdded { get; private set; }
        private int _numCommandsRecentlyAdded = 0;

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

        public void Add(RenderCommand item)
        {
            int index = (int)item.RenderPass;
            var set = _updatingPasses[index];
            set.Add(item);
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
            list.ForEach(x => x.Render(ShadowPass));
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
    /// <summary>
    /// Use for calculating something right before *anything* in the scene is rendered.
    /// Generally used for setting up data for a collection of sub-renderables just before they are rendered separately.
    /// </summary>
    public interface IPreRendered
    {
        bool PreRenderEnabled { get; }
        void PreRenderUpdate(ICamera camera);
        void PreRenderSwap();
        void PreRender(Viewport viewport, ICamera camera);
    }
    public interface IScene
    {
        DelRender Render { get; }

        void CollectVisible(RenderPasses passes, IVolume collectionVolume, ICamera camera, bool shadowPass);
        void Update(RenderPasses passes, IVolume collectionVolume, ICamera camera);

        void PreRenderUpdate(ICamera camera);
        void PreRenderSwap();
        void PreRender(Viewport viewport, ICamera camera);

        void AddPreRenderedObject(IPreRendered obj);
        void RemovePreRenderedObject(IPreRendered obj);

        void RegenerateTree();
        void GlobalPreRender();
        void GlobalUpdate();
        void GlobalSwap();
    }
    public abstract class BaseScene : TObjectSlim, IScene
    {
        /// <summary>
        /// Call this method to render the scene.
        /// </summary>
        public DelRender Render { get; protected set; }
        /// <summary>
        /// The number of renderable items in the scene.
        /// </summary>
        //public abstract int Count { get; }

        protected List<IPreRendered> _preRenderList = new List<IPreRendered>();
        protected List<IPreRendered> _preRenderAddWaitList = new List<IPreRendered>();
        protected List<IPreRendered> _preRenderRemoveWaitList = new List<IPreRendered>();
        
        //private static List<TMaterial> _activeMaterials = new List<TMaterial>();
        //private static Queue<int> _removedIds = new Queue<int>();

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
        public abstract void CollectVisible(RenderPasses passes, IVolume collectionVolume, ICamera camera, bool shadowPass);
        /// <summary>
        /// Populates the given RenderPasses object with all renderables 
        /// having culling volumes that reside within the collectionVolume.
        /// </summary>
        /// <param name="passes"></param>
        /// <param name="collectionVolume"></param>
        /// <param name="camera"></param>
        public void Update(RenderPasses passes, IVolume collectionVolume, ICamera camera)
        {
            CollectVisible(passes, collectionVolume, camera, false);
            PreRenderUpdate(camera);
        }
        
        public void PreRenderUpdate(ICamera camera)
        {
            //TODO: prerender on own consistent animation thread
            //ParallelLoopResult result = await Task.Run(() => Parallel.ForEach(_preRenderList, p => { p.PreRenderUpdate(camera); }));
            foreach (IPreRendered p in _preRenderList)
                if (p.PreRenderEnabled)
                    p.PreRenderUpdate(camera);
        }
        public void PreRenderSwap()
        {
            foreach (IPreRendered p in _preRenderRemoveWaitList)
                _preRenderList.Remove(p);
            foreach (IPreRendered p in _preRenderAddWaitList)
                _preRenderList.Add(p);

            _preRenderRemoveWaitList.Clear();
            _preRenderAddWaitList.Clear();

            foreach (IPreRendered p in _preRenderList)
                if (p.PreRenderEnabled)
                    p.PreRenderSwap();
        }
        public void PreRender(Viewport viewport, ICamera camera)
        {
            foreach (IPreRendered p in _preRenderList)
                if (p.PreRenderEnabled)
                    p.PreRender(viewport, camera);
        }
        public void AddPreRenderedObject(IPreRendered obj)
        {
            if (obj is null)
                return;
            if (!_preRenderList.Contains(obj))
                _preRenderAddWaitList.Add(obj);
        }
        public void RemovePreRenderedObject(IPreRendered obj)
        {
            if (obj is null)
                return;
            if (_preRenderList.Contains(obj))
                _preRenderRemoveWaitList.Add(obj);
        }
        
        public abstract void RegenerateTree();
        /// <summary>
        /// Occurs before any individual viewport processing.
        /// </summary>
        public abstract void GlobalPreRender();
        /// <summary>
        /// Occurs before any individual viewport processing.
        /// </summary>
        public abstract void GlobalUpdate();
        /// <summary>
        /// Occurs before any individual viewport processing.
        /// </summary>
        public abstract void GlobalSwap();
        
        //public abstract IEnumerator<IRenderable> GetEnumerator();
        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
