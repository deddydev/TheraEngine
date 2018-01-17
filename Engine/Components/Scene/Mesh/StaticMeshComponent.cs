﻿using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Rendering;
using System.ComponentModel;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;
using TheraEngine.Files;
using TheraEngine.Physics;

namespace TheraEngine.Worlds.Actors.Components.Scene.Mesh
{
    public partial class StaticMeshComponent : TRSComponent, IRigidCollidable, IMeshSocketOwner
    {
        public StaticMeshComponent() : this(null, null) { }
        public StaticMeshComponent(GlobalFileRef<StaticModel> model) : this(model, null) { }
        public StaticMeshComponent(GlobalFileRef<StaticModel> model, TRigidBodyConstructionInfo info) 
            : this(model, Vec3.Zero, Rotator.GetZero(), Vec3.One, info) { }
        public StaticMeshComponent(
            GlobalFileRef<StaticModel> model,
            Vec3 translation,
            Rotator rotation,
            Vec3 scale,
            TRigidBodyConstructionInfo info) : base(translation, rotation, scale, true)
        {
            _modelRef = model ?? new GlobalFileRef<StaticModel>();
            _modelRef.RegisterLoadEvent(ModelLoaded);

            if (info == null)
                _rigidBodyCollision = null;
            else
            {
                info.CollisionShape = model.File.Collision;
                info.InitialWorldTransform = WorldMatrix;
                _rigidBodyCollision = TRigidBody.New(this, info);
                _rigidBodyCollision.TransformChanged += RigidBodyTransformUpdated;

                //WorldTransformChanged += ThisTransformUpdated;
                //ThisTransformUpdated();
            }

            RecalcLocalTransform();
        }

        private void RigidBodyTransformUpdated(Matrix4 transform)
            => WorldMatrix = _rigidBodyCollision.WorldTransform;
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

        //internal override void RecalcGlobalTransform()
        //{
        //    base.RecalcGlobalTransform();
        //    if (_meshes != null)
        //        foreach (RenderableMesh m in _meshes)
        //            m.CullingVolume.SetTransform(WorldMatrix);
        //}

        //For internal runtime use
        private RenderableMesh[] _meshes = null;

        private GlobalFileRef<StaticModel> _modelRef;

        [TSerialize("RigidBodyCollision")]
        private TRigidBody _rigidBodyCollision = null;
        [TSerialize("Sockets")]
        private Dictionary<string, MeshSocket> _sockets = new Dictionary<string, MeshSocket>();

        [Category("Static Mesh Component")]
        [TSerialize]
        public GlobalFileRef<StaticModel> ModelRef
        {
            get => _modelRef;
            set
            {
                if (_modelRef == value)
                    return;
                if (_meshes != null)
                {
                    if (IsSpawned)
                        foreach (RenderableMesh mesh in _meshes)
                            mesh.Visible = false;
                    _meshes = null;
                }
                _modelRef.UnregisterLoadEvent(ModelLoaded);
                _modelRef = value ?? new GlobalFileRef<StaticModel>();
                _modelRef.RegisterLoadEvent(ModelLoaded);
            }
        }

        [Category("Static Mesh Component")]
        public TRigidBody RigidBodyCollision => _rigidBodyCollision;

        [Category("Static Mesh Component")]
        public RenderableMesh[] Meshes => _meshes;
        
        //TODO: make lod loading async
        private void ModelLoaded(StaticModel model)
        {
            _meshes = new RenderableMesh[model.RigidChildren.Count + model.SoftChildren.Count];
            for (int i = 0; i < model.RigidChildren.Count; ++i)
            {
                RenderableMesh m = new RenderableMesh(model.RigidChildren[i], this);
                m.Visible = IsSpawned && m.Mesh.VisibleByDefault;
                _meshes[i] = m;
            }
            for (int i = 0; i < model.SoftChildren.Count; ++i)
            {
                RenderableMesh m = new RenderableMesh(model.SoftChildren[i], this);
                m.Visible = IsSpawned && m.Mesh.VisibleByDefault;
                _meshes[model.RigidChildren.Count + i] = m;
            }
        }
        public override void OnSpawned()
        {
            if (_meshes == null)
            {
                if (_modelRef.IsLoaded)
                {
                    ModelLoaded(_modelRef.File);
                }
                else
                {
                    //TODO: make async, try to load and display lowest lods first
                    StaticModel m = _modelRef.GetInstance();
                    if (m != null)
                        ModelLoaded(m);
                    else
                    {
                        base.OnSpawned();
                        return;
                    }
                }
            }
            else
            {
                foreach (RenderableMesh m in _meshes)
                    m.Visible = m.Mesh.VisibleByDefault;
            }
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            foreach (RenderableMesh m in _meshes)
                m.Visible = false;
            base.OnDespawned();
        }
    }
}
