using TheraEngine.Rendering.Models;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Files;
using System.Collections.Generic;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using System;

namespace TheraEngine.Components.Scene.Mesh
{
    public partial class SkeletalMeshComponent : TRSComponent, IPreRendered, IMeshSocketOwner
    {
        public SkeletalMeshComponent(GlobalFileRef<SkeletalModel> mesh, LocalFileRef<Skeleton> skeleton)
        {
            if (skeleton != null)
            {
                _skeletonRef = skeleton;
                if (skeleton.IsLoaded)
                    _skeletonRef_Loaded(skeleton);
            }
            else
            {
                _skeletonRef = new LocalFileRef<Skeleton>();
            }
            _skeletonRef.RegisterLoadEvent(_skeletonRef_Loaded);

            ModelRef = mesh;
        }
        public SkeletalMeshComponent()
        {
            SkeletonRef = new LocalFileRef<Skeleton>();
            ModelRef = new GlobalFileRef<SkeletalModel>();
        }

        private LocalFileRef<Skeleton> _skeletonRef;
        private GlobalFileRef<SkeletalModel> _modelRef;

        //TODO: figure out how to serialize sockets and refer to what's attached to them in the current state
        private Dictionary<string, MeshSocket> _sockets = new Dictionary<string, MeshSocket>();

        //For internal runtime use
        private SkeletalRenderableMesh[] _meshes;

        #region IMeshSocketOwner interface
        public MeshSocket this[string socketName] 
            => _sockets.ContainsKey(socketName) ? _sockets[socketName] : null;
        public MeshSocket FindOrCreateSocket(string socketName)
        {
            if (_sockets.ContainsKey(socketName))
                return _sockets[socketName];
            else
            {
                MeshSocket socket = new MeshSocket(Transform.GetIdentity(), this, OwningActor);
                _sockets.Add(socketName, socket);
                return socket;
            }
        }
        public MeshSocket FindOrCreateSocket(string socketName, Transform transform)
        {
            if (_sockets.ContainsKey(socketName))
            {
                MeshSocket socket = _sockets[socketName];
                socket.Transform = transform;
                return socket;
            }
            else
            {
                MeshSocket socket = new MeshSocket(transform, this, OwningActor);
                _sockets.Add(socketName, socket);
                return socket;
            }
        }
        public void DeleteSocket(string socketName)
        {
            if (_sockets.ContainsKey(socketName))
                _sockets.Remove(socketName);
        }
        public void AddToSocket(string socketName, SceneComponent component)
            => FindOrCreateSocket(socketName).ChildComponents.Add(component);
        public void AddRangeToSocket(string socketName, IEnumerable<SceneComponent> components)
            => FindOrCreateSocket(socketName).ChildComponents.AddRange(components);
        #endregion

        /// <summary>
        /// Retrieves the model. 
        /// May load synchronously if not currently loaded.
        /// </summary>
        [Browsable(false)]
        public SkeletalModel Model => ModelRef.File;

        [TSerialize]
        [Category("Skeletal Mesh Component")]
        public GlobalFileRef<SkeletalModel> ModelRef
        {
            get => _modelRef;
            set
            {
                if (_modelRef == value)
                    return;

                if (_modelRef != null)
                {
                    _modelRef.UnregisterLoadEvent(_modelRef_Loaded);
                }

                if (_meshes != null)
                {
                    foreach (SkeletalRenderableMesh mesh in _meshes)
                        mesh.Visible = false;
                    _meshes = null;
                }

                _modelRef = value ?? new GlobalFileRef<SkeletalModel>();
                _modelRef.RegisterLoadEvent(_modelRef_Loaded);
                if (_modelRef.IsLoaded || IsSpawned)
                    _modelRef_Loaded(_modelRef.File);
            }
        }

        /// <summary>
        /// Retrieves the skeleton. 
        /// May load synchronously if not currently loaded.
        /// </summary>
        [Browsable(false)]
        public Skeleton Skeleton => SkeletonRef.File;

        [TSerialize]
        [Category("Skeletal Mesh Component")]
        public LocalFileRef<Skeleton> SkeletonRef
        {
            get => _skeletonRef;
            set
            {
                if (_skeletonRef == value)
                    return;

                if (_skeletonRef != null)
                {
                    _skeletonRef.UnregisterLoadEvent(_skeletonRef_Loaded);

                    if (_skeletonRef.IsLoaded)
                        _skeletonRef.File.OwningComponent = null;
                }

                _skeletonRef = value ?? new LocalFileRef<Skeleton>();
                _skeletonRef.RegisterLoadEvent(_skeletonRef_Loaded);
            }
        }

        private void _skeletonRef_Loaded(Skeleton skel)
        {
            skel.OwningComponent = this;
            if (_meshes != null)
                foreach (SkeletalRenderableMesh m in _meshes)
                    m.Skeleton = skel;
        }
        private void _modelRef_Loaded(SkeletalModel model)
        {
            _meshes = new SkeletalRenderableMesh[model.RigidChildren.Count + model.SoftChildren.Count];
            for (int i = 0; i < model.RigidChildren.Count; ++i)
            {
                SkeletalRenderableMesh mesh = new SkeletalRenderableMesh(model.RigidChildren[i], Skeleton, this);
                if (IsSpawned)
                    mesh.Visible = mesh.Mesh.VisibleByDefault;
                _meshes[i] = mesh;
            }
            for (int i = 0; i < model.SoftChildren.Count; ++i)
            {
                SkeletalRenderableMesh mesh = new SkeletalRenderableMesh(model.SoftChildren[i], Skeleton, this);
                if (IsSpawned)
                    mesh.Visible = mesh.Mesh.VisibleByDefault;
                _meshes[model.RigidChildren.Count + i] = mesh;
            }
        }

        [Category("Skeletal Mesh Component")]
        public SkeletalRenderableMesh[] Meshes => _meshes;
        
        public void SetAllSimulatingPhysics(bool doSimulation)
        {
            foreach (Bone b in Skeleton)
                if (b.RigidBodyCollision != null)
                    b.RigidBodyCollision.SimulatingPhysics = doSimulation;
        }
        public override void OnSpawned()
        {
            if (_meshes == null)
            {
                SkeletonRef.GetInstance();
                ModelRef.GetInstance();
            }

            if (_meshes != null)
                foreach (SkeletalRenderableMesh m in _meshes)
                    m.Visible = m.Mesh.VisibleByDefault;
            //OwningScene.Add(Skeleton);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (_meshes != null)
                foreach (SkeletalRenderableMesh m in _meshes)
                    m.Visible = false;
            //OwningScene.Remove(Skeleton);
            base.OnDespawned();
        }
        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            Skeleton?.WorldMatrixChanged();
        }

        //private void Tick(float delta) => PreRender();

        public void PreRenderUpdate(Camera camera)
        {
            Skeleton?.UpdateBones(camera, WorldMatrix, InverseWorldMatrix);
        }
        public void PreRenderSwap()
        {
            Skeleton?.SwapBuffers();
        }

        protected internal override void OnHighlightChanged(bool highlighted)
        {
            base.OnHighlightChanged(highlighted);

            foreach (SkeletalRenderableMesh m in Meshes)
            {
                foreach (var lod in m.LODs)
                {
                    Editor.EditorState.RegisterHighlightedMaterial(lod.Manager.Material, highlighted, OwningScene);
                }
            }
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            base.OnSelectedChanged(selected);

            if (OwningScene == null)
                return;

            if (Meshes != null)
                foreach (SkeletalRenderableMesh m in Meshes)
                {
                    if (m?.CullingVolume != null)
                    {
                        if (selected)
                        {
                            OwningScene.Add(m.CullingVolume);
                        }
                        else
                        {
                            OwningScene.Remove(m.CullingVolume);
                        }
                    }
                    //Editor.EditorState.RegisterSelectedMesh(m, selected, OwningScene);
                }
        }
    }
}
