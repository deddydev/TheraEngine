using TheraEngine.Rendering.Cameras;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering
{
    public delegate void DelRender(Camera camera, Viewport viewport, MaterialFrameBuffer target);
    /// <summary>
    /// Use for calculating something right before *anything* in the scene is rendered.
    /// Generally used for setting up data for a collection of sub-renderables just before they are rendered separately.
    /// </summary>
    public interface IPreRendered
    {
        void PreRender(Camera camera);
    }
    public abstract class Scene
    {
        private static List<TMaterial> _activeMaterials = new List<TMaterial>();
        private static Queue<int> _removedIds = new Queue<int>();
        protected List<IPreRendered> _preRenderList = new List<IPreRendered>();

        public DelRender Render { get; protected set; }
        public abstract int Count { get; }

        public Scene()
        {

        }
        
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
        
        public abstract void CollectVisibleRenderables(Frustum frustum, bool shadowPass);
    }
}
