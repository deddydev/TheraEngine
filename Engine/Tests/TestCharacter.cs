using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors;
using System;

namespace CustomEngine.Tests
{
    public class TestCharacter : CharacterPawn
    {
        protected override void PreConstruct()
        {
            Collada.ImportOptions options = new Collada.ImportOptions();
            options.InitialTransform.Scale = new Vec3(1.0f.InchesToMeters());
            string desktop = Environment.MachineName == "DAVID-DESKTOP" ?
               "X:\\Desktop\\" :
               "C:\\Users\\David\\Desktop\\";
            Collada.Scene scene = Collada.Import(desktop + "TEST.DAE", options, false, true);
            Mesh = scene._skeletalModel;
            Skeleton = scene._skeleton;
            base.PreConstruct();
        }
    }
}