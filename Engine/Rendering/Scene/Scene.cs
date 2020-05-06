using Extensions;
using System.Collections.Generic;
using TheraEngine.Core;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public delegate void DelRender(RenderPasses renderingPasses, ICamera camera, Viewport viewport, FrameBuffer target);
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
        void PreRenderUpdate(RenderPasses passes, IVolume collectionVolume, ICamera camera);

        void PreRenderSwap();
        void PreRender(Viewport viewport, ICamera camera);

        void AddPreRenderedObject(IPreRendered obj);
        void RemovePreRenderedObject(IPreRendered obj);

        void RegenerateTree();
        void GlobalRender();
        void GlobalCollectVisible();
        void GlobalSwap();

        void RenderPipeline(RenderPasses renderPasses, ICamera camera, Viewport viewport, FrameBuffer frameBuffer);
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
        public void PreRenderUpdate(RenderPasses passes, IVolume collectionVolume, ICamera camera)
        {
            CollectVisible(passes, collectionVolume, camera, false);

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
        public abstract void GlobalRender();
        /// <summary>
        /// Occurs before any individual viewport processing.
        /// </summary>
        public abstract void GlobalCollectVisible();
        /// <summary>
        /// Swaps all render pass buffers and processes render trees.
        /// Occurs before any individual viewport processing.
        /// </summary>
        public abstract void GlobalSwap();

        public void RenderPipeline(RenderPasses renderPasses, ICamera camera, Viewport viewport, FrameBuffer frameBuffer)
        {

        }

        //public abstract IEnumerator<IRenderable> GetEnumerator();
        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
