using System;
using System.Drawing;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Worlds.Actors.Components.Scene;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors.Types
{
    public class SkylightActor : Actor<PositionComponent>
    {
        TextureCubemap _skyTexture;
        
        public SkylightActor()
        {
            
        }
        
        public void Capture() => Capture(OwningWorld.Settings.Bounds.HalfExtents.LengthFast);
        public void Capture(float distance)
        {
            Viewport v = new Viewport(_skyTexture.Width, _skyTexture.Height); 
            foreach (CameraComponent comp in RootComponent.ChildComponents)
            {
                PerspectiveCamera cam = (PerspectiveCamera)comp.Camera;

                //TODO: render to each of the sky texture's sides
                Engine.Scene.Render(cam, cam.Frustum, v, false);
            }
        }

        protected override PositionComponent OnConstruct()
        {
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(0.0f,   0.0f, 0.0f, RotationOrder.YPR), //forward
                new Rotator(0.0f, 180.0f, 0.0f, RotationOrder.YPR), //backward
                new Rotator(0.0f, -90.0f, 0.0f, RotationOrder.YPR), //left
                new Rotator(0.0f,  90.0f, 0.0f, RotationOrder.YPR), //right
                new Rotator(90.0f,  0.0f, 0.0f, RotationOrder.YPR), //up
                new Rotator(-90.0f, 0.0f, 0.0f, RotationOrder.YPR), //down
            };
            PositionComponent pos = new PositionComponent();
            for (int i = 0; i < 6; ++i)
            {
                CameraComponent cam = new CameraComponent(new PerspectiveCamera(
                    Vec3.Zero, rotations[i], 1.0f, 10000.0f, 90.0f, 1.0f));
                pos.ChildComponents.Add(cam);
            }
            return pos;
        }
    }
}
