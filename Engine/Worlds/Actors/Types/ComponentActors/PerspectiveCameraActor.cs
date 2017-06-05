using CustomEngine.Worlds.Actors;
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
            PerspectiveCamera camera = new PerspectiveCamera(1.0f, 500.0f, 30.0f, 16.0f / 9.0f);
            return new CameraComponent(camera);
        }

        public override void OnSpawned(World world)
        {
            Engine.Renderer.Scene.Add(RootComponent.Camera);
            base.OnSpawned(world);
        }

        public override void OnDespawned()
        {
            Engine.Renderer.Scene.Remove(RootComponent.Camera);
            base.OnDespawned();
        }
    }
}
