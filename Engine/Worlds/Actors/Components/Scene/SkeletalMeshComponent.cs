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
        public SkeletalMeshComponent(SkeletalMesh m) { Model = m; }

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
                foreach (SkeletalSubMesh m in Model)
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
