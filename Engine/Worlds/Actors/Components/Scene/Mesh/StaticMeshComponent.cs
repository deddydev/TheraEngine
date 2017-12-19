using TheraEngine.Rendering.Models;
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
        public StaticMeshComponent(StaticModel m, TRigidBodyConstructionInfo info) 
            : this(m, Vec3.Zero, Rotator.GetZero(), Vec3.One, info) { }
        public StaticMeshComponent(
            StaticModel m,
            Vec3 translation,
            Rotator rotation,
            Vec3 scale,
            TRigidBodyConstructionInfo info)
        {
            SetTRS(translation, rotation, scale);
            Model = m;

            if (info == null)
                _rigidBodyCollision = null;
            else
            {
                info.CollisionShape = m.Collision;
                info.InitialWorldTransform = WorldMatrix;
                _rigidBodyCollision = TRigidBody.New(this, info);
                WorldTransformChanged += StaticMeshComponent_WorldTransformChanged;
                _rigidBodyCollision.TransformChanged += _rigidBodyCollision_TransformChanged; 
            }
        }

        private void _rigidBodyCollision_TransformChanged(Matrix4 transform)
            => WorldMatrix = _rigidBodyCollision.WorldTransform;
        private void StaticMeshComponent_WorldTransformChanged()
            => _rigidBodyCollision.WorldTransform = WorldMatrix;

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
        private RenderableMesh[] _meshes;

        private GlobalFileRef<StaticModel> _model;

        [TSerialize("RigidBodyCollision")]
        private TRigidBody _rigidBodyCollision;
        [TSerialize("Sockets")]
        private Dictionary<string, MeshSocket> _sockets = new Dictionary<string, MeshSocket>();

        [Category("Static Mesh Component")]
        [TSerialize]
        public GlobalFileRef<StaticModel> Model
        {
            get => _model;
            set
            {
                if (_model == value)
                    return;
                _model = value;
                if (_model != null)
                {
                    if (_model.IsLoaded || IsSpawned)
                    {
                        StaticModel model = _model.GetInstance();

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
                }
                else
                {
                    if (_meshes != null)
                    {
                        foreach (RenderableMesh mesh in _meshes)
                            mesh.Visible = false;
                        _meshes = null;
                    }
                }
            }
        }

        [Category("Static Mesh Component")]
        public TRigidBody RigidBodyCollision => _rigidBodyCollision;

        [Category("Static Mesh Component")]
        public RenderableMesh[] Meshes => _meshes;
        
        public override void OnSpawned()
        {
            foreach (RenderableMesh m in _meshes)
                m.Visible = m.Mesh.VisibleByDefault;
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            foreach (RenderableMesh m in _meshes)
                m.Visible = false;
            base.OnDespawned();
        }
        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            Translation.Raw -= newOrigin;
        }
    }
}
