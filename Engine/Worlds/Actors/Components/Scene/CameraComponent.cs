using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;
using System.Collections.Generic;
using System;
using CustomEngine.Files;
using System.IO;
using System.Xml;

namespace CustomEngine.Worlds.Actors.Components
{
    public class CameraComponent : SceneComponent
    {
        public CameraComponent()
        {
            _camera = new PerspectiveCamera();
            _camera.TransformChanged += RecalcGlobalTransform;
        }
        public CameraComponent(bool orthographic)
        {
            _camera = orthographic ? (Camera)new OrthographicCamera() : new PerspectiveCamera();
            _camera.TransformChanged += RecalcGlobalTransform;
        }
        public CameraComponent(Camera camera)
        {
            _camera = camera;
            _camera.TransformChanged += RecalcGlobalTransform;
        }

        bool _updatingTransform = false;
        private Camera _camera = new PerspectiveCamera();

        public Camera Camera
        {
            get => _camera;
            set => _camera = value;
        }
        protected override void GenerateChildCache(List<SceneComponent> cache)
        {
            base.GenerateChildCache(cache);
            if (Owner is IPawn p && p.CurrentCameraComponent == null)
                p.CurrentCameraComponent = this;
        }
        public void SetCurrentForController(LocalPlayerController controller)
        {
            if (controller != null)
                controller.CurrentCamera = _camera;
        }
        public void SetCurrentForOwner()
        {
            if (Owner is IPawn pawn && pawn.Controller is LocalPlayerController controller)
                controller.CurrentCamera = _camera;
        }
        public void SetCurrentForPawn(IPawn pawn)
        {
            if (pawn != null)
                pawn.CurrentCameraComponent = this;
        }
        public override Matrix4 WorldMatrix
        {
            get => base.WorldMatrix;
            protected set => base.WorldMatrix = value;
        }
        public override Matrix4 InverseWorldMatrix
        {
            get => base.InverseWorldMatrix;
            set => base.InverseWorldMatrix = value;
        }

        //protected override void RecalcLocalTransform()
        //{
        //    if (_updatingTransform)
        //        return;
        //    _updatingTransform = true;
        //    SetLocalTransforms(Camera.Matrix, Camera.InverseMatrix);
        //    _updatingTransform = false;
        //}
        //internal override void RecalcGlobalTransform()
        //{
        //    if (!_simulatingPhysics)
        //    {
        //        _worldTransform = GetParentMatrix() * Camera.Matrix;
        //        if (_ancestorSimulatingPhysics == null)
        //            _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();
        //    }
        //    foreach (SceneComponent c in _children)
        //        c.RecalcGlobalTransform();
        //    OnWorldTransformChanged();
        //}

        internal override void OriginRebased(Vec3 newOrigin)
        {
            _camera.TranslateAbsolute(-newOrigin);
        }
        public override void OnSpawned()
        {
            if (Engine.Settings.RenderCameraFrustums)
                Engine.Renderer.Scene.AddRenderable(_camera);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (Engine.Settings.RenderCameraFrustums)
                Engine.Renderer.Scene.RemoveRenderable(_camera);
            base.OnDespawned();
        }
    }
}
