﻿using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;
using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public enum RenderPass
    {
        OpaqueDeferred,
        OpaqueForward,
        TransparentForward
    }
    internal class RenderPasses
    {
        private Deque<I3DRenderable> _opaqueDeferred = new Deque<I3DRenderable>();
        private Deque<I3DRenderable> _opaqueForward = new Deque<I3DRenderable>();
        private Deque<I3DRenderable> _transparentForward = new Deque<I3DRenderable>();

        public Deque<I3DRenderable> OpaqueDeferred => _opaqueDeferred;
        public Deque<I3DRenderable> OpaqueForward => _opaqueForward;
        public Deque<I3DRenderable> TransparentForward => _transparentForward;

        public void Render(RenderPass pass)
        {
            switch (pass)
            {
                case RenderPass.OpaqueDeferred:
                    foreach (I3DRenderable r in OpaqueDeferred)
                        r.Render();
                    OpaqueDeferred.Clear();
                    break;
                case RenderPass.OpaqueForward:
                    foreach (I3DRenderable r in OpaqueForward)
                        r.Render();
                    OpaqueForward.Clear();
                    break;
                case RenderPass.TransparentForward:
                    foreach (I3DRenderable r in TransparentForward)
                        r.Render();
                    TransparentForward.Clear();
                    break;
            }
        }
    }
    /// <summary>
    /// Processes all scene information that will be sent to the renderer.
    /// </summary>
    public class SceneProcessor
    {
        private RenderPasses _passes;
        private RenderOctree _renderTree;
        private LightManager _lightManager;
        private static List<Material> _activeMaterials = new List<Material>();
        private static Queue<int> _removedIds = new Queue<int>();

        public RenderOctree RenderTree => _renderTree;
        public LightManager Lights => _lightManager;
        internal RenderPasses RenderPasses => _passes;

        /// <summary>
        /// Call this to only enable visibility for items visible from the given camera.
        /// </summary>
        /// <param name="camera">The camera viewpoint.</param>
        /// <param name="resetVisibility">If true, changes all visible objects back to invisible before testing for visibility again.</param>
        /// <param name="cullOffscreen">If true, will set all offscreen items to invisible.</param>
        /// <param name="renderOctree">If true, will render the subdivisions of the octree.</param>
        public void Cull(Camera camera, bool resetVisibility = true, bool cullOffscreen = true, bool renderOctree = false)
        {
            AbstractRenderer.CurrentCamera = camera;
            _renderTree.Cull(camera, true, true, _passes, Engine.Settings.RenderOctree);
            AbstractRenderer.CurrentCamera = null;
        }
        public void Render(Camera camera, RenderPass pass)
        {
            if (_renderTree == null || camera == null)
                return;

            AbstractRenderer.CurrentCamera = camera;
            _passes.Render(pass);
            AbstractRenderer.CurrentCamera = null;
        }
        public void Add(I3DRenderable obj)
        {
            _renderTree?.Add(obj);
        }
        public void Remove(I3DRenderable obj)
        {
            _renderTree?.Remove(obj);
        }
        internal void WorldChanged()
        {
            if (Engine.World == null)
            {
                _renderTree = null;
                _lightManager = null;
                _passes = null;
                return;
            }

            _renderTree = new RenderOctree(Engine.World.Settings.Bounds);
            _lightManager = new LightManager();
            _passes = new RenderPasses();
        }
        internal int AddActiveMaterial(Material material)
        {
            int id = _removedIds.Count > 0 ? _removedIds.Dequeue() : _activeMaterials.Count;
            _activeMaterials.Add(material);
            return id;
        }
        internal void RemoveActiveMaterial(Material material)
        {
            _removedIds.Enqueue(material.BindingId);
            _activeMaterials.RemoveAt(material.BindingId);
        }
    }
}
