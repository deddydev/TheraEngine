﻿using System;
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

                PropAnimFloat elbowAnim = new PropAnimFloat(360, 60.0f, true, true);
                FloatKeyframe f1 = new FloatKeyframe(0, 60.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe s1 = new FloatKeyframe(180, 60.0f, -145.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe l1 = new FloatKeyframe(360, 60.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
                elbowAnim.Keyframes.Add(f1);
                elbowAnim.Keyframes.Add(s1);
                elbowAnim.Keyframes.Add(l1);
                skeleton["LElbow"]?.FrameState.AddAnimation(new AnimationContainer("Yaw", false, elbowAnim), true);

                PropAnimFloat legAnim = new PropAnimFloat(360, 60.0f, true, true);
                FloatKeyframe f2 = new FloatKeyframe(0, 60.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe s2 = new FloatKeyframe(180, 60.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe l2 = new FloatKeyframe(360, 60.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                legAnim.Keyframes.Add(f2);
                legAnim.Keyframes.Add(s2);
                legAnim.Keyframes.Add(l2);
                skeleton["LLeg"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, legAnim), true);

                PropAnimFloat kneeAnim = new PropAnimFloat(360, 60.0f, true, true);
                FloatKeyframe f3 = new FloatKeyframe(0, 60.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe s3 = new FloatKeyframe(180, 60.0f, -90.0f, 0.0f, PlanarInterpType.CubicHermite);
                FloatKeyframe l3 = new FloatKeyframe(360, 60.0f, -180.0f, 0.0f, PlanarInterpType.CubicHermite);
                kneeAnim.Keyframes.Add(f3);
                kneeAnim.Keyframes.Add(s3);
                kneeAnim.Keyframes.Add(l3);
                skeleton["LKnee"]?.FrameState.AddAnimation(new AnimationContainer("Pitch", false, kneeAnim), true);

                _meshComp.Skeleton = skeleton;
                _meshComp.Model = mesh;
            }

            //string animPath = TestDefaults.DesktopPath + "test_anim.dae";
            ////googleDrive + "Thera\\Assets\\Characters\\Carly\\Animations\\Carly_Idle.dae";

            //ModelAnimation m = FromThirdParty<ModelAnimation>(animPath);
            //_animationStateMachine.Skeleton = Skeleton;
            //_animationStateMachine.InitialState = new AnimState(m);
        }
        protected override void PreConstruct()
        {
            //string desktop = Engine.StartupPath;
            ModelImportOptions options = new ModelImportOptions()
            {
                ImportModels = true,
                ImportAnimations = false,
                InitialTransform = new FrameState(Vec3.Zero, Quat.Identity, new Vec3(1.0f), TransformOrder.TRS),
            };

            Task.Factory.StartNew(() => 
            Collada.Import(TestDefaults.DesktopPath + "TEST.DAE", options), 
            TaskCreationOptions.LongRunning).
            ContinueWith(task => SceneImported(task));
            
            base.PreConstruct();
        }
    }
}