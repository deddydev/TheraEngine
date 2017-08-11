using System;
using TheraEngine.Files;
using TheraEngine.Animation;
using TheraEngine.Rendering.Models;
using TheraEngine.Worlds.Actors;

namespace TheraEngine.Tests
{
    public class TestCharacter : CharacterPawn
    {
        public const string MeshName = "TESTMESH";
        public const string SkelName = "TESTSKEL";
        protected override void PreConstruct()
        {
            string desktop = Environment.MachineName == "DAVID-DESKTOP" ? "X:\\Desktop\\" : "C:\\Users\\David\\Desktop\\";
            //string desktop = Engine.StartupPath;
            ModelImportOptions options = new ModelImportOptions()
            {
                ImportAnimations = false,
                InitialTransform = new FrameState(Vec3.Zero, Quat.Identity, new Vec3(1.0f), TransformOrder.TRS),
            };
            ModelScene scene = Collada.Import(desktop + "TEST.DAE", options);
            if (scene != null)
            {
                PropAnimFloat elbowAnim = new PropAnimFloat(360, true, true);
                FloatKeyframe f1 = new FloatKeyframe(0.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe s1 = new FloatKeyframe(180.0f, -145.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe l1 = new FloatKeyframe(360.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
                elbowAnim.Keyframes.Add(f1);
                elbowAnim.Keyframes.Add(s1);
                elbowAnim.Keyframes.Add(l1);
                scene.Skeleton["LElbow"]?.FrameState.AddAnimation(new AnimationContainer("Yaw", false, elbowAnim), true);

                PropAnimFloat legAnim = new PropAnimFloat(360, true, true);
                FloatKeyframe f2 = new FloatKeyframe(0.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe s2 = new FloatKeyframe(180.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe l2 = new FloatKeyframe(360.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                legAnim.Keyframes.Add(f2);
                legAnim.Keyframes.Add(s2);
                legAnim.Keyframes.Add(l2);
                scene.Skeleton["LLeg"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, legAnim), true);

                PropAnimFloat kneeAnim = new PropAnimFloat(360, true, true);
                FloatKeyframe f3 = new FloatKeyframe(0.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe s3 = new FloatKeyframe(180.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe l3 = new FloatKeyframe(360.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                kneeAnim.Keyframes.Add(f3);
                kneeAnim.Keyframes.Add(s3);
                kneeAnim.Keyframes.Add(l3);
                scene.Skeleton["LKnee"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, kneeAnim), true);
                
                scene.Skeleton.Export(desktop, "TESTSKEL", FileFormat.XML);
                Skeleton newSkel = Import<Skeleton>(scene.Skeleton.FilePath);

                Mesh = scene.SkeletalModel;
                //Mesh = new SingleFileRef<SkeletalMesh>(Engine.ContentFolderAbs + MeshName + "." + GetFileHeader(typeof(SkeletalMesh)).GetProperExtension(FileFormat.XML));
                Skeleton = scene.Skeleton;
                //Skeleton = new SingleFileRef<Skeleton>(Engine.ContentFolderAbs + SkelName + "." + GetFileHeader(typeof(Skeleton)).GetProperExtension(FileFormat.XML));
            }

            base.PreConstruct();
        }
    }
}