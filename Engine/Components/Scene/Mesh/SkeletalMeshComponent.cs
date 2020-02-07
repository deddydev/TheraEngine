using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;

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
        public MeshSocket FindOrCreateSocket(string socketName, ITransform transform)
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
        public void AddToSocket(string socketName, ISceneComponent component)
            => FindOrCreateSocket(socketName).ChildComponents.Add(component);
        public void AddRangeToSocket(string socketName, IEnumerable<ISceneComponent> components)
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
                
                if (Meshes != null)
                {
                    foreach (SkeletalRenderableMesh mesh in Meshes)
                    {
                        mesh.RenderInfo.UnlinkScene();
                        mesh.RenderInfo.Visible = false;
                    }
                    Meshes = null;
                }

                if (_modelRef != null)
                {
                    _modelRef.Loaded -= OnModelLoaded;
                    _modelRef.Unloaded -= OnModelUnloaded;
                }

                _modelRef = value ?? new GlobalFileRef<SkeletalModel>();

                _modelRef.Loaded += OnModelLoaded;
                _modelRef.Unloaded += OnModelUnloaded;
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
                    _skeletonRef.Loaded -= (SkeletonLoaded);
                    _skeletonRef.Unloaded -= (SkeletonUnloaded);

                    if (_skeletonRef.IsLoaded)
                        _skeletonRef.File.OwningComponent = null;
                }

                _skeletonRef = value ?? new LocalFileRef<Skeleton>();
                _skeletonRef.Loaded += (SkeletonLoaded);
                _skeletonRef.Unloaded += (SkeletonUnloaded);
            }
        }

        private void SkeletonUnloaded(Skeleton skel)
        {
            if (IsSpawned)
                TargetSkeleton?.RenderInfo?.UnlinkScene();
        }
        private void SkeletonLoaded(Skeleton skel)
        {
            if (ModelRef is null || !ModelRef.IsLoaded)
                return;

            if (IsSpawned)
            {
                MakeMeshes(ModelRef.File, skel);
                TargetSkeleton?.RenderInfo?.LinkScene(TargetSkeleton, OwningScene3D);
            }
        }
        private void OnModelUnloaded(SkeletalModel model)
        {
            if (model is null)
                return;

            model.RigidChildren.PostAnythingAdded -= RigidChildren_PostAnythingAdded;
            model.RigidChildren.PostAnythingRemoved -= RigidChildren_PostAnythingRemoved;
            model.SoftChildren.PostAnythingAdded -= SoftChildren_PostAnythingAdded;
            model.SoftChildren.PostAnythingRemoved -= SoftChildren_PostAnythingRemoved;

            foreach (var mesh in Meshes)
                mesh?.RenderInfo?.UnlinkScene();
            Meshes.Clear();
        }
        private async void OnModelLoaded(SkeletalModel model)
        {
            if (model is null)
                return;

            //Engine.PrintLine("Skeletal Model : OnModelLoaded");

            model.RigidChildren.PostAnythingAdded += RigidChildren_PostAnythingAdded;
            model.RigidChildren.PostAnythingRemoved += RigidChildren_PostAnythingRemoved;
            model.SoftChildren.PostAnythingAdded += SoftChildren_PostAnythingAdded;
            model.SoftChildren.PostAnythingRemoved += SoftChildren_PostAnythingRemoved;

            Skeleton skelOverride = null;
            if (SkeletonOverrideRef != null)
                skelOverride = await SkeletonOverrideRef.GetInstanceAsync();

            MakeMeshes(model, skelOverride);
        }

        private async void MakeMeshes(SkeletalModel model, Skeleton skeletonOverride)
        {
            if (Meshes != null)
                foreach (SkeletalRenderableMesh m in Meshes)
                {
                    m.RenderInfo.UnlinkScene();
                    m.RenderInfo.Visible = false;
                    m.Destroy();
                }
            
            if (model is null)
                return;

            TargetSkeleton = skeletonOverride ?? await model.SkeletonRef?.GetInstanceAsync();

            Meshes = new List<SkeletalRenderableMesh>();

            for (int i = 0; i < model.RigidChildren.Count; ++i)
                RigidChildren_PostAnythingAdded(model.RigidChildren[i]);
            
            for (int i = 0; i < model.SoftChildren.Count; ++i)
                SoftChildren_PostAnythingAdded(model.SoftChildren[i]);

            if (TargetSkeleton != null)
            {
                TargetSkeleton.OwningComponent = this;

                if (IsSpawned)
                {
                    AbstractPhysicsWorld world = OwningWorld.PhysicsWorld3D;
                    foreach (IBone b in TargetSkeleton.PhysicsDrivableBones)
                    {
                        TConstraint constraint = b.ParentPhysicsConstraint;
                        if (constraint != null)
                            world.AddConstraint(constraint);
                        TRigidBody body = b.RigidBodyCollision;
                        if (body != null)
                            world.AddCollisionObject(body);
                    }
                }
            }
        }

        private void RigidChildren_PostAnythingAdded(SkeletalRigidSubMesh item)
            => AddRenderMesh(item);
        private void RigidChildren_PostAnythingRemoved(SkeletalRigidSubMesh item)
            => RemoveRenderMesh(item);
        private void SoftChildren_PostAnythingAdded(SkeletalSoftSubMesh item)
            => AddRenderMesh(item);
        private void SoftChildren_PostAnythingRemoved(SkeletalSoftSubMesh item)
            => RemoveRenderMesh(item);

        private void AddRenderMesh(ISkeletalSubMesh subMesh)
        {
            //Engine.PrintLine("Skeletal Model : AddRenderMesh");

            SkeletalRenderableMesh renderMesh = new SkeletalRenderableMesh(subMesh, TargetSkeleton, this);
            if (IsSpawned)
                renderMesh.RenderInfo.LinkScene(renderMesh, OwningScene3D);
            Meshes.Add(renderMesh);
        }
        private void RemoveRenderMesh(ISkeletalSubMesh subMesh)
        {
            //Engine.PrintLine("Skeletal Model : RemoveRenderMesh");

            int match = Meshes.FindIndex(x => x.Mesh == subMesh);
            if (Meshes.IndexInRange(match))
            {
                Meshes[match]?.RenderInfo?.UnlinkScene();
                Meshes.RemoveAt(match);
            }
        }

        [Category("Skeletal Mesh Component")]
        public List<SkeletalRenderableMesh> Meshes { get; private set; }
        [Browsable(false)]
        public bool PreRenderEnabled { get; set; } = true;
        [Browsable(false)]
        public Skeleton TargetSkeleton { get; private set; }

        public void SetAllSimulatingPhysics(bool doSimulation)
        {
            foreach (Bone b in TargetSkeleton)
                if (b.RigidBodyCollision != null)
                    b.RigidBodyCollision.SimulatingPhysics = doSimulation;
        }
        public override async void OnSpawned()
        {
            if (Meshes is null)
            {
                if (!_modelRef.IsLoaded)
                    await _modelRef.GetInstanceAsync();
                else
                    OnModelLoaded(_modelRef.File);
            }

            if (Meshes != null)
                foreach (BaseRenderableMesh3D m in Meshes)
                    m.RenderInfo.LinkScene(m, OwningScene3D);

            TargetSkeleton?.RenderInfo?.LinkScene(TargetSkeleton, OwningScene3D);

            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (Meshes != null)
            {
                foreach (BaseRenderableMesh3D m in Meshes)
                {
                    m.RenderInfo.UnlinkScene();
                    m.Destroy();
                }
                Meshes = null;
            }

            if (TargetSkeleton != null)
            {
                AbstractPhysicsWorld world = OwningWorld.PhysicsWorld3D;
                foreach (IBone b in TargetSkeleton.PhysicsDrivableBones)
                {
                    TConstraint constraint = b.ParentPhysicsConstraint;
                    if (constraint != null)
                        world.RemoveConstraint(constraint);
                    TRigidBody body = b.RigidBodyCollision;
                    if (body != null)
                        world.RemoveCollisionObject(body);
                }
                TargetSkeleton.RenderInfo.UnlinkScene();
            }

            base.OnDespawned();
        }
        public override void RecalcWorldTransform(bool recalcChildWorldTransformsNow = true)
        {
            base.RecalcWorldTransform(recalcChildWorldTransformsNow);
            TargetSkeleton?.WorldMatrixChanged();
        }
        
        public void PreRenderUpdate(ICamera camera)
        {
            //_targetSkeleton?.UpdateBones(camera, Matrix4.Identity, Matrix4.Identity);
        }
        public void PreRenderSwap()
        {
            TargetSkeleton?.SwapBuffers();
        }
        public void PreRender(Viewport viewport, ICamera camera)
        {
            TargetSkeleton?.UpdateBones(camera, Matrix4.Identity, Matrix4.Identity);
        }

#if EDITOR
        protected internal override void OnHighlightChanged(bool highlighted)
        {
            base.OnHighlightChanged(highlighted);

            if (Meshes != null)
                foreach (SkeletalRenderableMesh m in Meshes)
                    foreach (var lod in m.LODs)
                        Editor.EditorState.RegisterHighlightedMaterial(lod.Manager.Material, highlighted, OwningScene);
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (Meshes != null)
                foreach (SkeletalRenderableMesh m in Meshes)
                {
                    var cull = m?.RenderInfo?.CullingVolume;
                    if (cull != null)
                        cull.RenderInfo.Visible = selected;
                    
                    //Editor.EditorState.RegisterSelectedMesh(m, selected, OwningScene);
                }
        }
#endif
    }
}
