﻿using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Scene.Mesh
{
    public partial class StaticMeshComponent : TransformComponent, IRigidBodyCollidable, IMeshSocketOwner
    {
        public StaticMeshComponent()
            : this(null, null) { }
        public StaticMeshComponent(GlobalFileRef<StaticModel> model)
            : this(model, null) { }
        public StaticMeshComponent(GlobalFileRef<StaticModel> model, TRigidBodyConstructionInfo info)
            : this(model, TTransform.GetIdentity(), info) { }
        public StaticMeshComponent(GlobalFileRef<StaticModel> model, TTransform transform, TRigidBodyConstructionInfo info)
            : base(transform, true)
        {
            _modelRef = model ?? new GlobalFileRef<StaticModel>();
            _modelRef.Loaded += OnModelLoaded;

            if (info is null)
                RigidBodyCollision = null;
            else
            {
                info.CollisionShape = model.File.CollisionShape;
                info.InitialWorldTransform = WorldMatrix;
                RigidBodyCollision = TRigidBody.New(info);

                //WorldTransformChanged += ThisTransformUpdated;
                //ThisTransformUpdated();
            }

            RecalcLocalTransform();
        }

        public event Action ModelLoaded;

        private void RigidBodyTransformUpdated(Matrix4 transform)
        {
            _readingPhysicsTransform = true;
            WorldMatrix.Value = _rigidBodyCollision.InterpolationWorldTransform;
            _readingPhysicsTransform = false;
        }
        //private void ThisTransformUpdated()
        //    => _rigidBodyCollision.WorldTransform = WorldMatrix;

        #region IMeshSocketOwner interface
        public MeshSocket this[string socketName]
            => _sockets.ContainsKey(socketName) ? _sockets[socketName] : null;
        public MeshSocket FindOrCreateSocket(string socketName)
        {
            if (_sockets.ContainsKey(socketName))
                return _sockets[socketName];
            else
            {
                MeshSocket socket = new MeshSocket(TTransform.GetIdentity(), this, OwningActor);
                _sockets.Add(socketName, socket);
                return socket;
            }
        }
        public MeshSocket FindOrCreateSocket(string socketName, TTransform transform)
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

        private GlobalFileRef<StaticModel> _modelRef;
        
        private TRigidBody _rigidBodyCollision = null;
        [TSerialize("Sockets")]
        private Dictionary<string, MeshSocket> _sockets = new Dictionary<string, MeshSocket>();

        /// <summary>
        /// Retrieves the model. 
        /// May load synchronously if not currently loaded.
        /// </summary>
        [Browsable(false)]
        public StaticModel Model => ModelRef.File;

        [Category("Static Mesh Component")]
        [TSerialize]
        public GlobalFileRef<StaticModel> ModelRef
        {
            get => _modelRef;
            set
            {
                if (_modelRef == value)
                    return;

                if (Meshes != null)
                {
                    if (IsSpawned)
                        foreach (StaticRenderableMesh mesh in Meshes)
                            mesh.RenderInfo.IsVisible = false;
                    Meshes = null;
                }

                if (_modelRef != null)
                {
                    _modelRef.Loaded -= OnModelLoaded;
                    _modelRef.Unloaded -= OnModelUnloaded;
                }

                _modelRef = value ?? new GlobalFileRef<StaticModel>();

                _modelRef.Loaded += OnModelLoaded;
                _modelRef.Unloaded += OnModelUnloaded;
            }
        }

        [TSerialize]
        [Category("Static Mesh Component")]
        public TRigidBody RigidBodyCollision
        {
            get => _rigidBodyCollision;
            set
            {
                if (_rigidBodyCollision == value)
                    return;
                if (_rigidBodyCollision != null)
                {
                    if (IsSpawned)
                        OwningWorld.PhysicsWorld3D?.RemoveCollisionObject(_rigidBodyCollision);

                    _rigidBodyCollision.Owner = null;
                    _rigidBodyCollision.TransformChanged -= RigidBodyTransformUpdated;
                }
                _rigidBodyCollision = value;
                if (_rigidBodyCollision != null)
                {
                    _rigidBodyCollision.Owner = this;
                    _rigidBodyCollision.TransformChanged += RigidBodyTransformUpdated;

                    if (IsSpawned)
                        OwningWorld.PhysicsWorld3D?.AddCollisionObject(_rigidBodyCollision);
                }
            }
        }


        [Category("Static Mesh Component")]
        public List<StaticRenderableMesh> Meshes { get; private set; } = null;

        TRigidBody IRigidBodyCollidable.RigidBodyCollision => RigidBodyCollision;
        Matrix4 ICollidable.CollidableWorldMatrix
        {
            get => WorldMatrix.Value;
            set => WorldMatrix.Value = value;
        }

        private void OnModelUnloaded(StaticModel model)
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
        private void OnModelLoaded(StaticModel model)
        {
            if (model is null)
                return;

            //Engine.PrintLine("Static Model : OnModelLoaded");

            model.RigidChildren.PostAnythingAdded += RigidChildren_PostAnythingAdded;
            model.RigidChildren.PostAnythingRemoved += RigidChildren_PostAnythingRemoved;
            model.SoftChildren.PostAnythingAdded += SoftChildren_PostAnythingAdded;
            model.SoftChildren.PostAnythingRemoved += SoftChildren_PostAnythingRemoved;

            Meshes = new List<StaticRenderableMesh>(model.RigidChildren.Count + model.SoftChildren.Count);

            for (int i = 0; i < model.RigidChildren.Count; ++i)
                RigidChildren_PostAnythingAdded(model.RigidChildren[i]);
            for (int i = 0; i < model.SoftChildren.Count; ++i)
                SoftChildren_PostAnythingAdded(model.SoftChildren[i]);

            ModelLoaded?.Invoke();
        }

        private void RigidChildren_PostAnythingAdded(StaticRigidSubMesh item)
            => AddRenderMesh(item);
        private void RigidChildren_PostAnythingRemoved(StaticRigidSubMesh item)
            => RemoveRenderMesh(item);
        private void SoftChildren_PostAnythingAdded(StaticSoftSubMesh item)
            => AddRenderMesh(item);
        private void SoftChildren_PostAnythingRemoved(StaticSoftSubMesh item)
            => RemoveRenderMesh(item);

        private void AddRenderMesh(IStaticSubMesh subMesh)
        {
            //Engine.PrintLine("Static Model : AddRenderMesh");

            StaticRenderableMesh m = new StaticRenderableMesh(subMesh, this);
            if (IsSpawned)
                m.RenderInfo.LinkScene(m, OwningScene3D);
            Meshes.Add(m);
        }
        private void RemoveRenderMesh(IStaticSubMesh subMesh)
        {
            //Engine.PrintLine("Static Model : RemoveRenderMesh");

            int match = Meshes.FindIndex(x => x.Mesh == subMesh);
            if (Meshes.IndexInRange(match))
            {
                Meshes[match]?.RenderInfo?.UnlinkScene();
                Meshes.RemoveAt(match);
            }
        }

        protected override async void OnSpawned()
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
            
            base.OnSpawned();
        }
        protected override void OnDespawned()
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

            base.OnDespawned();
        }

#if EDITOR
        protected internal override void OnHighlightChanged(bool highlighted)
        {
            base.OnHighlightChanged(highlighted);

            if (OwningScene is null)
                return;

            foreach (StaticRenderableMesh m in Meshes)
                foreach (var lod in m.LODs)
                    Editor.EditorState.RegisterHighlightedMaterial(lod.Manager.Material, highlighted, OwningScene);
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (Meshes != null)
                foreach (StaticRenderableMesh m in Meshes)
                {
                    var cull = m?.RenderInfo?.CullingVolume;
                    if (cull != null)
                        cull.RenderInfo.IsVisible = selected;

                    //Editor.EditorState.RegisterSelectedMesh(m, selected, OwningScene);
                }
        }
#endif

        public MeshSocket FindOrCreateSocket(string socketName, ITransform transform) => throw new NotImplementedException();
        public void AddToSocket(string socketName, ISceneComponent component) => throw new NotImplementedException();
        public void AddRangeToSocket(string socketName, IEnumerable<ISceneComponent> components) => throw new NotImplementedException();
    }
}
