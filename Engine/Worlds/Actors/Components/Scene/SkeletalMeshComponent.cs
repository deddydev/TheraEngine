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
        public SkeletalMeshComponent(SkeletalMesh m, Skeleton skeleton)
        {
            _skeleton = skeleton;
            Model = m;
        }
        public SkeletalMeshComponent()
        {

        }
        
        private SkeletalMesh _model;
        private Skeleton _skeleton;
        internal RenderableMesh[] _meshes;

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
                        _meshes[i] = new RenderableMesh(_model.RigidChildren[i], _skeleton, this);
                    for (int i = 0; i < _model.SoftChildren.Count; ++i)
                        _meshes[_model.RigidChildren.Count + i] = new RenderableMesh(_model.SoftChildren[i], _skeleton, this);
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
                foreach (RenderableMesh m in _meshes)
                    m.Skeleton = _skeleton;
            }
        }
        public void SetAllSimulatingPhysics(bool doSimulation)
        {
            foreach (Bone b in _skeleton)
                if (b.PhysicsDriver != null)
                    b.PhysicsDriver.SimulatingPhysics = doSimulation;
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
        internal class RenderableMesh : IRenderable
        {
            public RenderableMesh(ISkeletalMesh mesh, Skeleton skeleton, SceneComponent component)
            {
                _mesh = mesh;
                _component = component;
                Skeleton = skeleton;
                Visible = false;
                IsRendering = true;
            }

            private bool _isVisible, _isRendering;
            private SceneComponent _component;
            private ISkeletalMesh _mesh;
            private RenderOctree.Node _renderNode;
            private Bone _singleBind;
            private Skeleton _skeleton;
            private Shape _cullingVolume;
            
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
            }
            public bool IsRendering
            {
                get { return _isRendering; }
                set { _isRendering = value; }
            }
            public ISkeletalMesh Mesh
            {
                get { return _mesh; }
                set { _mesh = value; }
            }
            public RenderOctree.Node RenderNode
            {
                get { return _renderNode; }
                set { _renderNode = value; }
            }
            public Skeleton Skeleton
            {
                get { return _skeleton; }
                set
                {
                    _skeleton = value;

                    //TODO: support multi-bone influence as single bind as well
                    _singleBind = _mesh.SingleBindName != null && _skeleton != null ? _skeleton.GetBone(_mesh.SingleBindName) : null;
                }
            }
            public Shape CullingVolume { get { return _cullingVolume; } }

            //IsRendering is checked by the octree before calling
            public void Render()
            {
                if (Visible)
                {
                    Matrix4 world, invWorld;
                    if (_singleBind != null)
                    {
                        world = _singleBind.WorldMatrix;
                        invWorld = _singleBind.InverseWorldMatrix;
                    }
                    else
                    {
                        world = _component.WorldMatrix;
                        invWorld = _component.InverseWorldMatrix;
                    }
                    _mesh.PrimitiveManager.Render(world, invWorld.Transposed());
                }
            }
        }
    }
}
