using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;
using System.ComponentModel;

namespace CustomEngine.Worlds.Actors.Components
{
    public class SkeletalMeshComponent : TRSComponent
    {
        public SkeletalMeshComponent(SkeletalMesh m, bool visibleByDefault)
        {
            Model = m;
            _visibleByDefault = visibleByDefault;
        }
        public SkeletalMeshComponent(bool visibleByDefault)
        {
            _visibleByDefault = visibleByDefault;
        }
        public SkeletalMeshComponent()
        {
            _visibleByDefault = true;
        }

        private bool _visible, _visibleByDefault;
        private SkeletalMesh _model;
        internal SkeletalMesh Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;
                SkeletalMesh oldModel = _model;
                _model = value;
                if (oldModel != null)
                    oldModel.LinkedComponent = null;
                if (_model != null)
                    _model.LinkedComponent = this;
            }
        }
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;

                _visible = value;
                foreach (SkeletalRigidSubMesh m in Model.RigidChildren)
                    if (_visible)
                        Engine.Renderer.Scene.AddRenderable(m);
                    else
                        Engine.Renderer.Scene.RemoveRenderable(m);
                foreach (SkeletalSoftSubMesh m in Model.SoftChildren)
                    if (_visible)
                        Engine.Renderer.Scene.AddRenderable(m);
                    else
                        Engine.Renderer.Scene.RemoveRenderable(m);
            }
        }
        public override void OnSpawned()
        {
            base.OnSpawned();
            Visible = _visibleByDefault;
            Model?.OnSpawned();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            Visible = false;
            Model?.OnDespawned();
        }
    }
}
