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
            SkeletonOverrideRef = skeleton;
            ModelRef = mesh;
        }
        public SkeletalMeshComponent()
        {
            SkeletonOverrideRef = new LocalFileRef<Skeleton>();
            ModelRef = new GlobalFileRef<SkeletalModel>();
        }

        private LocalFileRef<Skeleton> _skeletonRef;
        private GlobalFileRef<SkeletalModel> _modelRef;

        //TODO: figure out how to serialize sockets and refer to what's attached to them in the current state
        private Dictionary<string, MeshSocket> _sockets = new Dictionary<string, MeshSocket>();

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

                if (Meshes != null)
                {
                    foreach (SkeletalRenderableMesh mesh in Meshes)
                        mesh.RenderInfo.Visible = false;
                    Meshes = null;
                }

                _modelRef = value ?? new GlobalFileRef<SkeletalModel>();
                _modelRef.RegisterLoadEvent(_modelRef_Loaded);
            }
        }

        /// <summary>
        /// Retrieves the skeleton. 
        /// May load synchronously if not currently loaded.
        /// </summary>
        [Browsable(false)]
        public Skeleton SkeletonOverride => SkeletonOverrideRef?.File;

        [TSerialize]
        [Category("Skeletal Mesh Component")]
        public LocalFileRef<Skeleton> SkeletonOverrideRef
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
            if (ModelRef != null && !ModelRef.IsLoaded)
                return;

            //_targetSkeleton = SkeletonOverride ?? ModelRef.File.SkeletonRef.File;
            //_targetSkeleton.OwningComponent = this;

            //if (Meshes != null)
            //    foreach (SkeletalRenderableMesh m in Meshes)
            //        m.Skeleton = _targetSkeleton;

            if (IsSpawned)
                MakeMeshes();
        }
        private void _modelRef_Loaded(SkeletalModel model)
        {
            //_targetSkeleton = SkeletonOverride ?? model.SkeletonRef.File;
            //_targetSkeleton.OwningComponent = this;

            if (IsSpawned)
                MakeMeshes();
        }

        private async void MakeMeshes()
        {
            if (Meshes != null)
                foreach (SkeletalRenderableMesh m in Meshes)
                {
                    m.RenderInfo.Visible = false;
                    m.Destroy();
                }

            if (ModelRef == null)
                return;

            SkeletalModel model = await ModelRef.GetInstanceAsync();
            if (model == null)
                return;

            _targetSkeleton = null;
            if (SkeletonOverrideRef != null)
                _targetSkeleton = await SkeletonOverrideRef.GetInstanceAsync();

            if (_targetSkeleton == null && model.SkeletonRef != null)
                _targetSkeleton = await model.SkeletonRef.GetInstanceAsync();
            
            if (_targetSkeleton == null || model == null)
                return;

            _targetSkeleton.OwningComponent = this;

            Meshes = new SkeletalRenderableMesh[model.RigidChildren.Count + model.SoftChildren.Count];
            for (int i = 0; i < model.RigidChildren.Count; ++i)
            {
                SkeletalRenderableMesh mesh = new SkeletalRenderableMesh(model.RigidChildren[i], _targetSkeleton, this);
                mesh.RenderInfo.LinkScene(mesh, OwningScene3D);
                Meshes[i] = mesh;
            }
            for (int i = 0; i < model.SoftChildren.Count; ++i)
            {
                SkeletalRenderableMesh mesh = new SkeletalRenderableMesh(model.SoftChildren[i], _targetSkeleton, this);
                mesh.RenderInfo.LinkScene(mesh, OwningScene3D);
                Meshes[model.RigidChildren.Count + i] = mesh;
            }
        }

        [Category("Skeletal Mesh Component")]
        public SkeletalRenderableMesh[] Meshes { get; private set; }
        [Browsable(false)]
        public bool PreRenderEnabled { get; set; } = true;

        private Skeleton _targetSkeleton;

        public void SetAllSimulatingPhysics(bool doSimulation)
        {
            foreach (Bone b in _targetSkeleton)
                if (b.RigidBodyCollision != null)
                    b.RigidBodyCollision.SimulatingPhysics = doSimulation;
        }
        public override void OnSpawned()
        {
            MakeMeshes();
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (Meshes != null)
                foreach (SkeletalRenderableMesh m in Meshes)
                {
                    m.RenderInfo.Visible = false;
                    m.Destroy();
                }
            
            base.OnDespawned();
        }
        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            _targetSkeleton?.WorldMatrixChanged();
        }
        
        public void PreRenderUpdate(Camera camera)
        {
            //_targetSkeleton?.UpdateBones(camera, Matrix4.Identity, Matrix4.Identity);
        }
        public void PreRenderSwap()
        {
            _targetSkeleton?.SwapBuffers();
        }
        public void PreRender(Viewport viewport, Camera camera)
        {
            _targetSkeleton?.UpdateBones(camera, Matrix4.Identity, Matrix4.Identity);
        }

        protected internal override void OnHighlightChanged(bool highlighted)
        {
            base.OnHighlightChanged(highlighted);

            if (Meshes != null)
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
            if (Meshes != null)
                foreach (SkeletalRenderableMesh m in Meshes)
                {
                    var cull = m?.CullingVolume;
                    if (cull != null)
                        cull.RenderInfo.Visible = selected;
                    
                    //Editor.EditorState.RegisterSelectedMesh(m, selected, OwningScene);
                }
        }
    }
}
