﻿using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Rendering;
using System.ComponentModel;
using BulletSharp;
using System.Collections.Generic;

namespace TheraEngine.Worlds.Actors
{
    public class StaticMeshSocket : ISocket
    {
        public StaticMeshSocket(FrameState transform, IActor owner)
        {
            _owner = owner;
            _transform = transform;
            _childComponents = new MonitoredList<SceneComponent>();
            _childComponents.PostAdded          += _children_Added;
            _childComponents.PostAddedRange     += _children_AddedRange;
            _childComponents.PostInserted       += _children_Inserted;
            _childComponents.PostInsertedRange  += _children_InsertedRange;
            _childComponents.PostRemoved        += _children_Removed;
            _childComponents.PostRemovedRange   += _children_RemovedRange;
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
                item.OwningActor = null;
                item.RecalcGlobalTransform();
            }
            //_owner?.GenerateSceneComponentCache();
        }
        private void _children_Removed(SceneComponent item)
        {
            item._parent = null;
            item.OwningActor = null;
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
                item.OwningActor = _owner;
                item.RecalcGlobalTransform();
            }
            //_owner?.GenerateSceneComponentCache();
        }
        private void _children_Added(SceneComponent item)
        {
            item._parent = this;
            item.OwningActor = _owner;
            item.RecalcGlobalTransform();
            //_owner?.GenerateSceneComponentCache();
        }
        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
            }
        }
        public void HandleTranslation(Vec3 delta)
        {
            _transform.Translation += delta;
        }
        public void HandleScale(Vec3 delta)
        {
            _transform.Scale += delta;
        }
        public void HandleRotation(Quat delta)
        {
            _transform.Quaternion *= delta;
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
