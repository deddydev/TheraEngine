using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds;
using CustomEngine.Rendering.Cameras;
using System;

namespace CustomEngine.Worlds.Actors.Types
{
    public class PerspectiveCameraActor : Actor<CameraComponent>
    {
        public PerspectiveCameraActor() : base() { }

        public PerspectiveCamera Camera
        {
            get => (PerspectiveCamera)RootComponent.Camera;
            set => RootComponent.Camera = value;
        }
        
        protected override CameraComponent OnConstruct()
        {
            PerspectiveCamera camera = new PerspectiveCamera(1.0f, 500.0f, 30.0f);
            return new CameraComponent(camera);
        }

        public override void OnSpawned(World world)
        {
            //Engine.Renderer.Scene.AddRenderable(_cameraComponent.Camera);
            base.OnSpawned(world);
        }

        public override void OnDespawned()
        {
            //Engine.Renderer.Scene.RemoveRenderable(_cameraComponent.Camera);
            base.OnDespawned();
        }
    }
}
