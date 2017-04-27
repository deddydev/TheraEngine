﻿using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Models;
using System;

namespace CustomEngine.Worlds.Actors
{
    public partial class SkeletalMeshComponent : TRSComponent
    {
        public SkeletalMeshComponent(SkeletalMesh m, Skeleton skeleton)
        {
            Skeleton = skeleton;
            Model = m;
        }
        public SkeletalMeshComponent()
        {

        }

        private SkeletalMesh _model;
        private Skeleton _skeleton;
        //For internal runtime use
        internal RenderableMesh[] _meshes;

        [Serialize]
        public SkeletalMesh Model
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
                        _meshes[i] = new RenderableMesh(_model.RigidChildren[i], _skeleton, this);
                    for (int i = 0; i < _model.SoftChildren.Count; ++i)
                        _meshes[_model.RigidChildren.Count + i] = new RenderableMesh(_model.SoftChildren[i], _skeleton, this);
                }
            }
        }
        [Serialize]
        public Skeleton Skeleton
        {
            get => _skeleton;
            set
            {
                if (value == _skeleton)
                    return;
                if (_skeleton != null)
                    _skeleton.OwningComponent = null;
                _skeleton = value;
                if (_skeleton != null)
                    _skeleton.OwningComponent = this;
                if (_meshes != null)
                    foreach (RenderableMesh m in _meshes)
                        m.Skeleton = _skeleton;
            }
        }
        public void SetAllSimulatingPhysics(bool doSimulation)
        {
            foreach (Bone b in _skeleton)
                if (b.PhysicsDriver != null)
                    b.PhysicsDriver.SimulatingPhysics = doSimulation;
        }
        public override void OnSpawned()
        {
            foreach (RenderableMesh m in _meshes)
                m.Visible = m.Mesh.VisibleByDefault;

            if (Engine.Settings.RenderSkeletons)
                Engine.Renderer.Scene.AddRenderable(_skeleton);

            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            foreach (RenderableMesh m in _meshes)
                m.Visible = false;

            base.OnDespawned();

            if (Engine.Settings.RenderSkeletons)
                Engine.Renderer.Scene.RemoveRenderable(_skeleton);
        }
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _skeleton?.WorldMatrixChanged();
        }
    }
}
