using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;
using System.ComponentModel;
using BulletSharp;
using System.Collections.Generic;

namespace CustomEngine.Worlds.Actors
{
    public class StaticMeshSocket : ISocket
    {
        public StaticMeshSocket(FrameState transform, IActor owner)
        {
            _owner = owner;
            _transform = transform;
            _childComponents = new MonitoredList<SceneComponent>();
            _childComponents.Added          += _children_Added;
            _childComponents.AddedRange     += _children_AddedRange;
            _childComponents.Inserted       += _children_Inserted;
            _childComponents.InsertedRange  += _children_InsertedRange;
            _childComponents.Removed        += _children_Removed;
            _childComponents.RemovedRange   += _children_RemovedRange;
        }

        private IActor _owner;
        private FrameState _transform = FrameState.Identity;
        private MonitoredList<SceneComponent> _childComponents;

        public Matrix4 WorldMatrix => _transform.Matrix;
        public Matrix4 InverseWorldMatrix => _transform.InverseMatrix;
        public MonitoredList<SceneComponent> ChildComponents => _childComponents;

        private void _children_RemovedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = null;
                item.Owner = null;
                item.RecalcGlobalTransform();
            }
            //_owner?.GenerateSceneComponentCache();
        }
        private void _children_Removed(SceneComponent item)
        {
            item._parent = null;
            item.Owner = null;
            item.RecalcGlobalTransform();
            //_owner?.GenerateSceneComponentCache();
        }
        private void _children_InsertedRange(IEnumerable<SceneComponent> items, int index)
            => _children_AddedRange(items);
        private void _children_Inserted(SceneComponent item, int index)
            => _children_Added(item);
        private void _children_AddedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = this;
                item.Owner = _owner;
                item.RecalcGlobalTransform();
            }
            //_owner?.GenerateSceneComponentCache();
        }
        private void _children_Added(SceneComponent item)
        {
            item._parent = this;
            item.Owner = _owner;
            item.RecalcGlobalTransform();
            //_owner?.GenerateSceneComponentCache();
        }
    }
    public partial class StaticMeshComponent : TRSComponent, IPhysicsDrivable
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
            Model = m;
            SetTRS(translation, rotation, scale);

            if (info == null)
                _physicsDriver = null;
            else
            {
                //info.InitialWorldTransform = WorldMatrix;
                info.CollisionShape = m.CullingVolume.GetCollisionShape();
                info.MotionState = new DefaultMotionState(WorldMatrix);
                _physicsDriver = new PhysicsDriver(this, info, _physicsDriver_TransformChanged);
            }
        }

        public StaticMeshSocket this[string socketName]
        {
            get => _sockets.ContainsKey(socketName) ? _sockets[socketName] : null;
            set
            {
                if (_sockets.ContainsKey(socketName))
                    _sockets[socketName] = value;
                else
                    _sockets.Add(socketName, value);
            }
        }

        private Dictionary<string, StaticMeshSocket> _sockets = new Dictionary<string, StaticMeshSocket>();

        private void _physicsDriver_TransformChanged(Matrix4 worldMatrix)
            => WorldMatrix = worldMatrix;
        
        internal override void RecalcGlobalTransform()
        {
            if (_physicsDriver == null || !_physicsDriver.SimulatingPhysics)
            {
                base.RecalcGlobalTransform();
                foreach (RenderableMesh m in _meshes)
                    m.CullingVolume.SetTransform(WorldMatrix);
            }
        }

        private StaticMesh _model;
        private PhysicsDriver _physicsDriver;
        internal RenderableMesh[] _meshes;

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
        public PhysicsDriver PhysicsDriver => _physicsDriver;

        public override void OnSpawned()
        {
            _physicsDriver?.OnSpawned();
            foreach (RenderableMesh m in _meshes)
                m.Visible = m.Mesh.VisibleByDefault;
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            _physicsDriver?.OnDespawned();
            foreach (RenderableMesh m in _meshes)
                m.Visible = false;
            base.OnDespawned();
        }
        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            Translation -= newOrigin;
        }
    }
}
