using TheraEngine.Rendering.Models;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Files;

namespace TheraEngine.Worlds.Actors
{
    public partial class SkeletalMeshComponent : TRSComponent, IPreRenderNeeded
    {
        public SkeletalMeshComponent(SkeletalMesh m, Skeleton skeleton)
        {
            Skeleton = skeleton;
            Model = m;
        }
        public SkeletalMeshComponent() { }

        private SingleFileRef<SkeletalMesh> _model;
        private SingleFileRef<Skeleton> _skeleton;

        //For internal runtime use
        private RenderableMesh[] _meshes;

        [Serialize]
        public SingleFileRef<SkeletalMesh> Model
        {
            get => _model;
            set
            {
                if (_model == value)
                    return;
                _model = value;
                var skel = _skeleton;
                _skeleton = null;
                if (_model != null)
                {
                    if (_model.IsLoaded || IsSpawned)
                    {
                        SkeletalMesh model = _model.GetInstance();
                        _meshes = new RenderableMesh[model.RigidChildren.Count + model.SoftChildren.Count];
                        for (int i = 0; i < model.RigidChildren.Count; ++i)
                        {
                            RenderableMesh m = new RenderableMesh(model.RigidChildren[i], skel, this);
                            m.Visible = IsSpawned && m.Mesh.VisibleByDefault;
                            _meshes[i] = m;
                        }
                        for (int i = 0; i < model.SoftChildren.Count; ++i)
                        {
                            RenderableMesh m = new RenderableMesh(model.SoftChildren[i], skel, this);
                            m.Visible = IsSpawned && m.Mesh.VisibleByDefault;
                            _meshes[model.RigidChildren.Count + i] = m;
                        }
                    }
                }
                else
                {
                    if (_meshes != null)
                    {
                        //foreach (RenderableMesh mesh in _meshes)
                            
                    }
                }
                _skeleton = skel;
            }
        }
        [Serialize]
        public SingleFileRef<Skeleton> Skeleton
        {
            get => _skeleton;
            set
            {
                if (value == _skeleton)
                    return;
                if (_skeleton != null)
                    _skeleton.File.OwningComponent = null;
                _skeleton = value;
                if (_skeleton != null)
                {
                    _skeleton.File.OwningComponent = this;
                    if (IsSpawned && Engine.Settings.RenderSkeletons && _skeleton != null)
                        Engine.Scene.Add(_skeleton.File);
                }
                if (_meshes != null)
                    foreach (RenderableMesh m in _meshes)
                        m.Skeleton = _skeleton;
            }
        }
        
        public RenderableMesh[] Meshes => _meshes;
        
        public void SetAllSimulatingPhysics(bool doSimulation)
        {
            foreach (Bone b in _skeleton.File)
                if (b.PhysicsDriver != null)
                    b.PhysicsDriver.SimulatingPhysics = doSimulation;
        }
        public override void OnSpawned()
        {
            if (_meshes != null)
                foreach (RenderableMesh m in _meshes)
                    m.Visible = m.Mesh.VisibleByDefault;

            if (Engine.Settings.RenderSkeletons && _skeleton != null)
                Engine.Scene.Add(_skeleton.File);

            //RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);

            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (_meshes != null)
                foreach (RenderableMesh m in _meshes)
                    m.Visible = false;

            base.OnDespawned();

            if (Engine.Settings.RenderSkeletons && _skeleton != null)
                Engine.Scene.Remove(_skeleton.File);

            //UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
        }
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _skeleton?.File?.WorldMatrixChanged();
        }

        //private void Tick(float delta) => PreRender();

        public void PreRender()
        {
            _skeleton?.File?.UpdateBones(AbstractRenderer.CurrentCamera);
        }
    }
}
