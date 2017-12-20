﻿using TheraEngine.Rendering.Models;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Files;
using System.Collections.Generic;
using System;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;

namespace TheraEngine.Worlds.Actors.Components.Scene.Mesh
{
    public partial class SkeletalMeshComponent : TRSComponent, IPreRenderNeeded, IMeshSocketOwner
    {
        public SkeletalMeshComponent(SkeletalMesh mesh, Skeleton skeleton)
        {
            _skeletonRef = skeleton ?? new LocalFileRef<Skeleton>();
            _skeletonRef.Loaded += _skeletonRef_Loaded;

            ModelRef = mesh;
        }
        public SkeletalMeshComponent()
        {
            _skeletonRef = new LocalFileRef<Skeleton>();
            _skeletonRef.Loaded += _skeletonRef_Loaded;

            ModelRef = new GlobalFileRef<SkeletalMesh>();
        }

        private LocalFileRef<Skeleton> _skeletonRef;
        private GlobalFileRef<SkeletalMesh> _modelRef;

        //TODO: figure out how to serialize sockets and refer to what's attached to them in the current state
        private Dictionary<string, MeshSocket> _sockets = new Dictionary<string, MeshSocket>();

        //For internal runtime use
        private RenderableMesh[] _meshes;

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
        public SkeletalMesh Model => ModelRef.File;

        [TSerialize]
        [Category("Skeletal Mesh Component")]
        public GlobalFileRef<SkeletalMesh> ModelRef
        {
            get => _modelRef;
            set
            {
                if (_modelRef == value)
                    return;
                
                _modelRef = value ?? new GlobalFileRef<SkeletalMesh>();

                if (_meshes != null)
                {
                    foreach (RenderableMesh mesh in _meshes)
                        mesh.Visible = false;
                    _meshes = null;
                }

                if (_modelRef.IsLoaded || IsSpawned)
                {
                    _meshes = new RenderableMesh[Model.RigidChildren.Count + Model.SoftChildren.Count];
                    for (int i = 0; i < Model.RigidChildren.Count; ++i)
                    {
                        RenderableMesh mesh = new RenderableMesh(Model.RigidChildren[i], Skeleton, this);
                        mesh.Visible = IsSpawned && mesh.Mesh.VisibleByDefault;
                        _meshes[i] = mesh;
                    }
                    for (int i = 0; i < Model.SoftChildren.Count; ++i)
                    {
                        RenderableMesh mesh = new RenderableMesh(Model.SoftChildren[i], Skeleton, this);
                        mesh.Visible = IsSpawned && mesh.Mesh.VisibleByDefault;
                        _meshes[Model.RigidChildren.Count + i] = mesh;
                    }
                }
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

                _skeletonRef.Loaded -= _skeletonRef_Loaded;

                if (_skeletonRef.IsLoaded)
                    _skeletonRef.File.OwningComponent = null;                

                _skeletonRef = value ?? new LocalFileRef<Skeleton>();
                _skeletonRef.Loaded += _skeletonRef_Loaded;
            }
        }

        private void _skeletonRef_Loaded(Skeleton skel)
        {
            skel.OwningComponent = this;
            if (_meshes != null)
                foreach (RenderableMesh m in _meshes)
                    m.Skeleton = skel;
        }
        
        [Category("Skeletal Mesh Component")]
        public RenderableMesh[] Meshes => _meshes;
        
        public void SetAllSimulatingPhysics(bool doSimulation)
        {
            foreach (Bone b in Skeleton)
                if (b.RigidBodyCollision != null)
                    b.RigidBodyCollision.SimulatingPhysics = doSimulation;
        }
        public override void OnSpawned()
        {
            if (_meshes != null)
                foreach (RenderableMesh m in _meshes)
                    m.Visible = m.Mesh.VisibleByDefault;
            
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (_meshes != null)
                foreach (RenderableMesh m in _meshes)
                    m.Visible = false;

            base.OnDespawned();
        }
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            Skeleton.WorldMatrixChanged();
        }

        //private void Tick(float delta) => PreRender();

        public void PreRender()
        {
            Skeleton.UpdateBones(AbstractRenderer.CurrentCamera);
        }
    }
}
