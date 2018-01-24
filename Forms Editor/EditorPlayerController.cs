using System.Collections.Generic;
using TheraEngine.Input;
using TheraEngine.Actors;

namespace TheraEditor
{
    public class EditorPlayerController : LocalPlayerController
    {
        public EditorPlayerController(LocalPlayerIndex index, Queue<IPawn> possessionQueue = null) : base(index, possessionQueue)
        {
            SetViewportCamera = false;
            SetViewportHUD = false;
        }
        public EditorPlayerController(LocalPlayerIndex index) : base(index)
        {
            SetViewportCamera = false;
            SetViewportHUD = false;
        }

        //protected override void RegisterInput(InputInterface input)
        //{
        //    input.RegisterButtonEvent(EKey.B, ButtonInputType.Pressed, ToggleBones, InputPauseType.TickAlways);
        //    input.RegisterButtonEvent(EKey.C, ButtonInputType.Pressed, ToggleCameras, InputPauseType.TickAlways);
        //    base.RegisterInput(input);
        //}

        //private void ToggleBones()
        //{
        //    Engine.Settings.RenderSkeletons = !Engine.Settings.RenderSkeletons;

        //    TestCharacter t = Engine.World.State.GetSpawnedActorsOfType<TestCharacter>().ToArray()[0];
        //    SkeletalMeshComponent skm = t.RootComponent.ChildComponents[0] as SkeletalMeshComponent;

        //    if (skm.SkeletonRef.IsLoaded)
        //    {
        //        if (Engine.Settings.RenderSkeletons)
        //            Engine.Scene.Add(skm.SkeletonRef.File);
        //        else
        //            Engine.Scene.Remove(skm.SkeletonRef.File);
        //    }
        //}
        //private void ToggleCameras()
        //{
        //    Engine.Settings.RenderCameraFrustums = !Engine.Settings.RenderCameraFrustums;

        //    TestCharacter t = Engine.World.StateRef.File.GetSpawnedActorsOfType<TestCharacter>().ToArray()[0];
        //    CameraComponent c = t.RootComponent.ChildComponents[1].ChildComponents[0].ChildComponents[0] as CameraComponent;

        //    if (c.CameraRef.IsLoaded)
        //    {
        //        if (Engine.Settings.RenderCameraFrustums)
        //            Engine.Scene.Add(c.CameraRef.File);
        //        else
        //            Engine.Scene.Remove(c.CameraRef.File);
        //    }
        //}
    }
}
