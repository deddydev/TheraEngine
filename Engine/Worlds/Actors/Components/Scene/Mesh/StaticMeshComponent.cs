using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Rendering;
using System.ComponentModel;
using BulletSharp;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;

namespace TheraEngine.Worlds.Actors.Components.Scene.Mesh
{
    public partial class StaticMeshComponent : TRSComponent, IPhysicsDrivable, IMeshSocketOwner
    {
        public StaticMeshComponent(StaticMesh m, PhysicsConstructionInfo info) 
            : this(m, Vec3.Zero, Rotator.GetZero(), Vec3.One, info) { }
        public StaticMeshComponent(
            StaticMesh m,
            Vec3 translation,
            Rotator rotation,
            Vec3 scale,
            PhysicsConstructionInfo info)
        {
            SetTRS(translation, rotation, scale);
            Model = m;

            if (info == null)
                _physicsDriver = null;
            else
            {
                //info.InitialWorldTransform = WorldMatrix;
                info.CollisionShape = m.Collision;
                info.MotionState = new DefaultMotionState(WorldMatrix);
                _physicsDriver = new PhysicsDriver(this, info, _physicsDriver_TransformChanged);
                WorldTransformChanged += StaticMeshComponent_WorldTransformChanged;
            }
        }

        private void StaticMeshComponent_WorldTransformChanged()
        {
            _physicsDriver.SetPhysicsTransform(WorldMatrix);
        }

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

        private void _physicsDriver_TransformChanged(Matrix4 worldMatrix)
            => WorldMatrix = worldMatrix;
        
        //internal override void RecalcGlobalTransform()
        //{
        //    base.RecalcGlobalTransform();
        //    if (_meshes != null)
        //        foreach (RenderableMesh m in _meshes)
        //            m.CullingVolume.SetTransform(WorldMatrix);
        //}

        private StaticMesh _model;
        private PhysicsDriver _physicsDriver;
        private RenderableMesh[] _meshes;
        private Dictionary<string, MeshSocket> _sockets = new Dictionary<string, MeshSocket>();

        public StaticMesh Model
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
                        _meshes[i] = new RenderableMesh(_model.RigidChildren[i], this);
                    for (int i = 0; i < _model.SoftChildren.Count; ++i)
                        _meshes[_model.RigidChildren.Count + i] = new RenderableMesh(_model.SoftChildren[i], this);
                }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PhysicsDriver PhysicsDriver => _physicsDriver;
        
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
