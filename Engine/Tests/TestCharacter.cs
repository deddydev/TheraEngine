using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors;
using System;

namespace CustomEngine.Tests
{
    public class TestCharacter : CharacterPawn
    {
        public const string MeshName = "TESTMESH";
        public const string SkelName = "TESTSKEL";
        protected override void PreConstruct()
        {
            Collada.ImportOptions options = new Collada.ImportOptions();
            options.InitialTransform.Scale = new Vec3(1.0f.InchesToMeters());
            string desktop = Environment.MachineName == "DAVID-DESKTOP" ? "X:\\Desktop\\" : "C:\\Users\\David\\Desktop\\";
            //Collada.Scene scene = Collada.Import(desktop + "TEST.DAE", options, false, true);
            Mesh = new SingleFileRef<SkeletalMesh>(Engine.ContentFolderAbs + MeshName + "." + GetFileHeader(typeof(SkeletalMesh)).GetProperExtension(FileFormat.XML));
            //Mesh = TestWorld.ColladaScene._skeletalModel;
            Skeleton = new SingleFileRef<Skeleton>(Engine.ContentFolderAbs + SkelName + "." + GetFileHeader(typeof(Skeleton)).GetProperExtension(FileFormat.XML));
            //Skeleton = TestWorld.ColladaScene._skeleton;
            base.PreConstruct();
        }
    }
}