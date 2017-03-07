using CustomEngine.Rendering.Models;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;
using System.ComponentModel;

namespace CustomEngine.Worlds.Actors.Components
{
    public partial class SkeletalMeshComponent : TRSComponent
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
            get => _model;
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
            get => _skeleton;
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

            if (Engine.Settings.RenderSkeletons)
                Engine.Renderer.Scene.AddRenderable(_skeleton);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            foreach (RenderableMesh m in _meshes)
                m.Visible = false;

            if (Engine.Settings.RenderSkeletons)
                Engine.Renderer.Scene.RemoveRenderable(_skeleton);
        }
    }
}
