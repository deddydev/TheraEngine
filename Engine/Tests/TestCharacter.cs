using System;
using TheraEngine.Files;
using TheraEngine.Animation;
using TheraEngine.Rendering.Models;
using TheraEngine.Worlds.Actors;
using System.Threading.Tasks;

namespace TheraEngine.Tests
{
    public class TestCharacter : CharacterPawn
    {
        public const string MeshName = "TESTMESH";
        public const string SkelName = "TESTSKEL";
        private void SceneImported(Task<Collada.Data> task)
        {
            var scene = task.Result;
            if (scene != null)
            {
                var mesh = scene.Models[0].SkeletalModel;
                var skeleton = scene.Models[0].Skeleton;

                PropAnimFloat elbowAnim = new PropAnimFloat(360, true, true);
                FloatKeyframe f1 = new FloatKeyframe(0.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe s1 = new FloatKeyframe(180.0f, -145.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe l1 = new FloatKeyframe(360.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
                elbowAnim.Keyframes.Add(f1);
                elbowAnim.Keyframes.Add(s1);
                elbowAnim.Keyframes.Add(l1);
                skeleton["LElbow"]?.FrameState.AddAnimation(new AnimationContainer("Yaw", false, elbowAnim), true);

                PropAnimFloat legAnim = new PropAnimFloat(360, true, true);
                FloatKeyframe f2 = new FloatKeyframe(0.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe s2 = new FloatKeyframe(180.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe l2 = new FloatKeyframe(360.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                legAnim.Keyframes.Add(f2);
                legAnim.Keyframes.Add(s2);
                legAnim.Keyframes.Add(l2);
                skeleton["LLeg"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, legAnim), true);

                PropAnimFloat kneeAnim = new PropAnimFloat(360, true, true);
                FloatKeyframe f3 = new FloatKeyframe(0.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe s3 = new FloatKeyframe(180.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe l3 = new FloatKeyframe(360.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                kneeAnim.Keyframes.Add(f3);
                kneeAnim.Keyframes.Add(s3);
                kneeAnim.Keyframes.Add(l3);
                skeleton["LKnee"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, kneeAnim), true);

                Skeleton = skeleton;
                Mesh = mesh;
            }
        }
        protected override void PreConstruct()
        {
            string desktop = Environment.MachineName == "DAVID-DESKTOP" ?
                "X:\\Desktop\\" :
                "C:\\Users\\David\\Desktop\\";
            string googleDrive = Environment.MachineName == "DAVID-DESKTOP" ?
                "X:\\Cloud Storage\\Google Drive\\TheraDev\\" :
                "C:\\Users\\David\\Google Drive\\TheraDev\\";

            //string desktop = Engine.StartupPath;
            ModelImportOptions options = new ModelImportOptions()
            {
                ImportModels = true,
                ImportAnimations = false,
                InitialTransform = new FrameState(Vec3.Zero, Quat.Identity, new Vec3(1.0f), TransformOrder.TRS),
            };

            var task = Task.Factory.StartNew(() => Collada.Import(desktop + "TEST.DAE", options), TaskCreationOptions.LongRunning).ContinueWith(tsk => SceneImported(tsk));

            //Task<Collada.Data> import = Collada.Import(desktop + "TEST.DAE", options);
            //Collada.Data scene = await import;
            //if (scene != null)
            //{
            //    //PropAnimFloat elbowAnim = new PropAnimFloat(360, true, true);
            //    //FloatKeyframe f1 = new FloatKeyframe(0.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
            //    //FloatKeyframe s1 = new FloatKeyframe(180.0f, -145.0f, 0.0f, PlanarInterpType.CubicHermite);
            //    //FloatKeyframe l1 = new FloatKeyframe(360.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
            //    //elbowAnim.Keyframes.Add(f1);
            //    //elbowAnim.Keyframes.Add(s1);
            //    //elbowAnim.Keyframes.Add(l1);
            //    //scene.Skeleton["LElbow"]?.FrameState.AddAnimation(new AnimationContainer("Yaw", false, elbowAnim), true);

            //    //PropAnimFloat legAnim = new PropAnimFloat(360, true, true);
            //    //FloatKeyframe f2 = new FloatKeyframe(0.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
            //    //FloatKeyframe s2 = new FloatKeyframe(180.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
            //    //FloatKeyframe l2 = new FloatKeyframe(360.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
            //    //legAnim.Keyframes.Add(f2);
            //    //legAnim.Keyframes.Add(s2);
            //    //legAnim.Keyframes.Add(l2);
            //    //scene.Skeleton["LLeg"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, legAnim), true);

            //    //PropAnimFloat kneeAnim = new PropAnimFloat(360, true, true);
            //    //FloatKeyframe f3 = new FloatKeyframe(0.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
            //    //FloatKeyframe s3 = new FloatKeyframe(180.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
            //    //FloatKeyframe l3 = new FloatKeyframe(360.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
            //    //kneeAnim.Keyframes.Add(f3);
            //    //kneeAnim.Keyframes.Add(s3);
            //    //kneeAnim.Keyframes.Add(l3);
            //    //scene.Skeleton["LKnee"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, kneeAnim), true);

            //    Mesh = scene.Models[0].SkeletalModel;
            //    //Mesh = new SingleFileRef<SkeletalMesh>(Engine.ContentFolderAbs + MeshName + "." + GetFileHeader(typeof(SkeletalMesh)).GetProperExtension(FileFormat.XML));
            //    Skeleton = scene.Models[0].Skeleton;
            //    //Skeleton = new SingleFileRef<Skeleton>(Engine.ContentFolderAbs + SkelName + "." + GetFileHeader(typeof(Skeleton)).GetProperExtension(FileFormat.XML));
            //}

            base.PreConstruct();

            //string animPath = desktop + "test_anim.dae";//googleDrive + "Thera\\Assets\\Characters\\Carly\\Animations\\Carly_Idle.dae";

            //ModelAnimation m = FromThirdParty<ModelAnimation>(animPath);
            //_animationStateMachine.Skeleton = Skeleton;
            //_animationStateMachine.InitialState = new AnimState(m);
        }
    }
}