using System;
using System.Threading.Tasks;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Animation;
using TheraEngine.Components.Logic.Animation;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Tests
{
    public class TestCharacter : CharacterPawn
    {
        private async void SceneImported(Task<Collada.Data> task)
        {
            var scene = task.Result;
            if (scene != null)
            {
                var mesh = scene.Models[0].SkeletalModel;
                var skeleton = scene.Models[0].Skeleton;

                //PropAnimFloat elbowAnim = new PropAnimFloat(360, 60.0f, true, true);
                //FloatKeyframe f1 = new FloatKeyframe(0, 60.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
                //FloatKeyframe s1 = new FloatKeyframe(180, 60.0f, -145.0f, 0.0f, PlanarInterpType.CubicHermite);
                //FloatKeyframe l1 = new FloatKeyframe(360, 60.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
                //elbowAnim.Keyframes.Add(f1);
                //elbowAnim.Keyframes.Add(s1);
                //elbowAnim.Keyframes.Add(l1);
                //skeleton["LElbow"]?.FrameState.AddAnimation(new AnimationContainer("Yaw", false, elbowAnim), true);

                //PropAnimFloat legAnim = new PropAnimFloat(360, 60.0f, true, true);
                //FloatKeyframe f2 = new FloatKeyframe(0, 60.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                //FloatKeyframe s2 = new FloatKeyframe(180, 60.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
                //FloatKeyframe l2 = new FloatKeyframe(360, 60.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                //legAnim.Keyframes.Add(f2);
                //legAnim.Keyframes.Add(s2);
                //legAnim.Keyframes.Add(l2);
                //Bone RLeg = skeleton["RLeg"];
                //RLeg?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, legAnim), true);

                //PropAnimFloat kneeAnim = new PropAnimFloat(360, 60.0f, true, true);
                //FloatKeyframe f3 = new FloatKeyframe(0, 60.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                //FloatKeyframe s3 = new FloatKeyframe(180, 60.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
                //FloatKeyframe l3 = new FloatKeyframe(360, 60.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                //kneeAnim.Keyframes.Add(f3);
                //kneeAnim.Keyframes.Add(s3);
                //kneeAnim.Keyframes.Add(l3);
                //skeleton["LKnee"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, kneeAnim), true);

                _meshComp.SkeletonOverrideRef = skeleton;
                _meshComp.ModelRef = mesh;

                string animPath = //TestDefaults.DesktopPath + "test_anim.dae";
                TestDefaults.GoogleDrivePath + "Thera\\Assets\\Characters\\Carly\\Animations\\Carly_Idle.dae";
                SkeletalAnimation m = await Read3rdPartyAsync<SkeletalAnimation>(animPath);
                //if (scene.ModelAnimations != null && scene.ModelAnimations.Count > 0)
                {
                    _animationStateMachine.Skeleton = _meshComp.SkeletonOverride;
                    _animationStateMachine.InitialState = new AnimState(/*scene.ModelAnimations[0]*/new Files.GlobalFileRef<SkelAnimPoseGenBase>(new SkelAnimDirectPose(m)));
                }
            }
        }
        protected override void PreConstruct()
        {
            //string desktop = Engine.StartupPath;
            ColladaImportOptions options = new ColladaImportOptions()
            {
                IgnoreFlags =
                Collada.EIgnoreFlags.Extra |
                Collada.EIgnoreFlags.Lights |
                Collada.EIgnoreFlags.Cameras |
                Collada.EIgnoreFlags.Animations,
                InitialTransform = new Transform(Vec3.Zero, Quat.Identity, new Vec3(0.45f), TransformOrder.TRS),
            };
            
            Task.Run(() => Collada.ImportAsync(TestDefaults.DesktopPath + "test.dae", options)).ContinueWith(task => SceneImported(task));
            base.PreConstruct();
        }
    }
}