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
        private Camera _camera = new PerspectiveCamera();
        public Camera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }
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
        protected override void RecalcLocalTransform()
        {
            SetLocalTransforms(Camera.Matrix, Camera.InverseMatrix);
        }
        internal override void OriginRebased(Vec3 newOrigin)
        {
            _camera.TranslateAbsolute(-newOrigin);
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            throw new NotImplementedException();
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
