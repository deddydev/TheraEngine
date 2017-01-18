using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;
using System.ComponentModel;

namespace CustomEngine.Worlds.Actors.Components
{
    public class SkeletalMeshComponent : TRSComponent
    {
        public SkeletalMeshComponent(SkeletalMesh m)
        {
            Model = m;
        }
        public SkeletalMeshComponent()
        {

        }
        
        private SkeletalMesh _model;
        private Skeleton _skeleton;
        protected RenderableMesh[] _meshes;

        public SkeletalMesh Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;
                _model = value;
                if (_model != null)
                {
                    _meshes = new RenderableMesh[_model.RigidChildren.Count + _model.SoftChildren.Count];
                    for (int i = 0; i < _model.RigidChildren.Count; ++i)
                        _meshes[i] = new RenderableMesh(_model.RigidChildren[i], this);
                    for (int i = 0; i < _model.SoftChildren.Count; ++i)
                        _meshes[_model.RigidChildren.Count + i] = new RenderableMesh(_model.SoftChildren[i], this);
                }
            }
        }
        public Skeleton Skeleton
        {
            get { return _skeleton; }
            set
            {
                if (value == _skeleton)
                    return;
                _skeleton = value;
            }
        }
        public override void OnSpawned()
        {
            base.OnSpawned();
            foreach (RenderableMesh m in _meshes)
                m.Visible = m.Mesh.VisibleByDefault;
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            foreach (RenderableMesh m in _meshes)
                m.Visible = false;
        }
        internal class RenderableMesh : IMesh
        {
            public RenderableMesh(IMesh mesh, SceneComponent component)
            {
                _mesh = mesh;
                _component = component;
                Visible = false;
                IsRendering = true;
            }

            private bool _isVisible, _isRendering;
            private SceneComponent _component;
            private IMesh _mesh;
            private RenderOctree.OctreeNode _renderNode;
            private Bone _singleBind;

            public bool Visible
            {
                get { return _isVisible; }
                set
                {
                    _isVisible = value;
                    if (_isVisible)
                        Engine.Renderer.Scene.AddRenderable(this);
                    else
                        Engine.Renderer.Scene.RemoveRenderable(this);
                }
            }
            public Bone SingleBind
            {
                get { return _singleBind; }
                set { _singleBind = value; }
            }
            public bool IsRendering
            {
                get { return _isRendering; }
                set { _isRendering = value; }
            }
            public IMesh Mesh
            {
                get { return _mesh; }
                set { _mesh = value; }
            }
            public Shape CullingVolume { get { return _mesh.CullingVolume.TransformedBy(_component.WorldMatrix); } }
            public RenderOctree.OctreeNode RenderNode
            {
                get { return _renderNode; }
                set { _renderNode = value; }
            }
            public void Render()
            {
                if (Visible && IsRendering)
                    _mesh.PrimitiveManager.Render(SingleBind.WorldMatrix, SingleBind.InverseWorldMatrix.Transposed());
            }
        }
    }
}
