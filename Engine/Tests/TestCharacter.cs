using CustomEngine.Files;
using CustomEngine.Rendering.Animation;
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
            //Mesh = new SingleFileRef<SkeletalMesh>(Engine.ContentFolderAbs + MeshName + "." + GetFileHeader(typeof(SkeletalMesh)).GetProperExtension(FileFormat.XML));

            //string desktop = Environment.MachineName == "DAVID-DESKTOP" ? "X:\\Desktop\\" : "C:\\Users\\David\\Desktop\\";
            //Collada.ImportOptions options = new Collada.ImportOptions();
            //options.InitialTransform.Scale = new Vec3(1.0f.InchesToMeters());
            Collada.Scene scene = TestWorld.ColladaScene;// Collada.Import(desktop + "TEST.DAE", options, false, true);
            Mesh = scene.SkeletalModel;
            //Skeleton = new SingleFileRef<Skeleton>(Engine.ContentFolderAbs + SkelName + "." + GetFileHeader(typeof(Skeleton)).GetProperExtension(FileFormat.XML));

            PropAnimFloat elbowAnim = new PropAnimFloat(360, true, true);
            FloatKeyframe f1 = new FloatKeyframe(0.0f, 0.0f, PlanarInterpType.CubicHermite);
            FloatKeyframe s1 = new FloatKeyframe(180.0f, -145.0f, PlanarInterpType.CubicHermite);
            FloatKeyframe l1 = new FloatKeyframe(360.0f, 0.0f, PlanarInterpType.CubicHermite);
            f1.Link(s1).Link(l1);
            elbowAnim.Keyframes.Add(f1);
            scene.Skeleton["LElbow"]?.FrameState.AddAnimation(new AnimationContainer("Yaw", false, elbowAnim), true);

            PropAnimFloat legAnim = new PropAnimFloat(360, true, true);
            FloatKeyframe f2 = new FloatKeyframe(0.0f, -180.0f, PlanarInterpType.CubicHermite);
            FloatKeyframe s2 = new FloatKeyframe(180.0f, -90.0f, PlanarInterpType.CubicHermite);
            FloatKeyframe l2 = new FloatKeyframe(360.0f, -180.0f, PlanarInterpType.CubicHermite);
            f2.Link(s2).Link(l2);
            legAnim.Keyframes.Add(f2);
            scene.Skeleton["LLeg"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, legAnim), true);

            PropAnimFloat kneeAnim = new PropAnimFloat(360, true, true);
            FloatKeyframe f3 = new FloatKeyframe(0.0f, -180.0f, PlanarInterpType.CubicHermite);
            FloatKeyframe s3 = new FloatKeyframe(180.0f, -90.0f, PlanarInterpType.CubicHermite);
            FloatKeyframe l3 = new FloatKeyframe(360.0f, -180.0f, PlanarInterpType.CubicHermite);
            f3.Link(s3).Link(l3);
            kneeAnim.Keyframes.Add(f3);
            scene.Skeleton["LKnee"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, kneeAnim), true);

            Skeleton = scene.Skeleton;

            base.PreConstruct();
        }
    }
}