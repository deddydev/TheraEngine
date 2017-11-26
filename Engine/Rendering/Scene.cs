using TheraEngine.Rendering.Cameras;
using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;
using TheraEngine.Rendering.Particles;
using TheraEngine.Worlds.Actors.Components.Scene;
using TheraEngine.Worlds.Actors.Components.Scene.Lights;
using System.Drawing;
using TheraEngine.Files;
using TheraEngine.Rendering.Textures;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering
{
    public delegate void DelRender(Camera camera, Frustum frustum, Viewport viewport, bool shadowPass);
    /// <summary>
    /// Use for calculating something right before *anything* in the scene is rendered.
    /// Generally used for setting up data for a collection of sub-renderables just before they are rendered separately.
    /// </summary>
    public interface IPreRenderNeeded
    {
        void PreRender();
    }
    public abstract class Scene
    {
        private static List<Material> _activeMaterials = new List<Material>();
        private static Queue<int> _removedIds = new Queue<int>();
        protected List<IPreRenderNeeded> _preRenderList = new List<IPreRenderNeeded>();

        public DelRender Render { get; protected set; }
        
        public Scene()
        {

        }
        
        public int AddActiveMaterial(Material material)
        {
            int id = _removedIds.Count > 0 ? _removedIds.Dequeue() : _activeMaterials.Count;
            _activeMaterials.Add(material);
            return id;
        }
        public void RemoveActiveMaterial(Material material)
        {
            _removedIds.Enqueue(material.UniqueID);
            _activeMaterials.RemoveAt(material.UniqueID);
        }

        public void AddPreRenderedObject(IPreRenderNeeded obj)
        {
            if (obj == null)
                return;
            if (!_preRenderList.Contains(obj))
                _preRenderList.Add(obj);
        }
        public void RemovePreRenderedObject(IPreRenderNeeded obj)
        {
            if (obj == null)
                return;
            if (_preRenderList.Contains(obj))
                _preRenderList.Remove(obj);
        }
    }
}
