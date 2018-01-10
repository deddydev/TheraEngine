using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Animation;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Components.Scene.Lights;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;
using TheraEngine.Worlds.Actors.Types.ComponentActors.Shapes;
using TheraEngine.Worlds.Maps;

namespace TheraEngine.Tests
{
    public class UnitTestingWorld : World
    {
        protected internal override void OnLoaded()
        {
            float margin = 5.0f;
            float radius = 5.0f;
            ColorF4 sphereColor = Color.Red;
            ColorF4 boxColor = Color.Blue;
            ColorF4 floorColor = Color.Gray;
            float diam = radius * 2.0f;
            float originDist = diam + margin;

            List<IActor> actors = new List<IActor>();
            IActor actor;

            int count = 5;
            int y = 0;

            //Create spheres
            for (int x = -count; x <= count; ++x)
                for (int z = -count; z <= count; ++z)
                {
                    TMaterial mat = TMaterial.CreateLitColorMaterial(sphereColor);
                    mat.Parameter<ShaderFloat>("Roughness").Value = ((x + count) / (float)count * 0.5f).ClampMin(0.0f);
                    mat.Parameter<ShaderFloat>("Metallic").Value = ((z + count) / (float)count * 0.5f).ClampMin(0.0f);
                    actor = new SphereActor("TestSphere" + (y++).ToString(), radius, new Vec3(x * originDist, 0.0f, z * originDist), Rotator.GetZero(),
                        mat, new TRigidBodyConstructionInfo() { UseMotionState = true, });
                    actors.Add(actor);
                }

            //Create boxes
            //for (int i = -5; i < 5; ++i)
            //{
            //    actor = new BoxActor("TestBox", radius, new Vec3(i * originDist, 0.0f, originDist), new Rotator(0.0f, 0.0f, i * 30.0f, RotationOrder.YPR),
            //        TMaterial.CreateLitColorMaterial(boxColor), new TRigidBodyConstructionInfo()
            //        {
            //            //UseMotionState = false,
            //        });
            //    actors.Add(actor);
            //}

            //Create floor
            actor = new BoxActor("Floor",
                new Vec3(500.0f, 0.5f, 500.0f), new Vec3(0.0f, -30.0f, 0.0f),
                Rotator.GetZero(), TMaterial.CreateLitColorMaterial(floorColor));
            actors.Add(actor);

            //Create shape tracer


            actor = new SphereTraceActor();
            actors.Add(actor);
            float rotationsPerSecond = 0.1f, testRadius = 30.0f, testHeight = 20.0f;
            PropAnimMethod<Vec3> animMethod = new PropAnimMethod<Vec3>(
                1.0f / rotationsPerSecond, true, second =>
            {
                float theta = (rotationsPerSecond * second).RemapToRange(0.0f, 1.0f) * 360.0f;
                //float mult = 1.5f - 4.0f * TMath.Cosdf(theta);
                Vec2 coord = TMath.PolarToCartesianDeg(theta, testRadius/* * mult*/);
                return new Vec3(coord.X, testHeight, -coord.Y);
            });
            AnimationContainer anim = new AnimationContainer(
                "RotationTrace", "Translation.Raw", false, animMethod);
            actor.RootComponent.AddAnimation(anim, true, false, 
                ETickGroup.PostPhysics, ETickOrder.Animation, Input.Devices.EInputPauseType.TickAlways);

            //Create world light
            //Actor<DirectionalLightComponent> dirlight = new Actor<DirectionalLightComponent>();
            //dirlight.RootComponent.LightColor = (ColorF3)Color.Beige;
            //dirlight.RootComponent.Rotation.Pitch = -35;
            //dirlight.RootComponent.AmbientIntensity = 0.01f;
            //actors.Add(dirlight);

            //Create spot light
            //Actor<SpotLightComponent> spotlight = new Actor<SpotLightComponent>();
            //spotlight.RootComponent.LightColor = (ColorF3)Color.Beige;
            //spotlight.RootComponent.Rotation.Pitch = -90;
            //spotlight.RootComponent.AmbientIntensity = 0.01f;
            //spotlight.RootComponent.Translation.Raw = Vec3.Up * 70.0f;
            //spotlight.RootComponent.Distance = 200.0f;
            //spotlight.RootComponent.Brightness = 100.0f;
            //spotlight.RootComponent.OuterCutoffAngleDegrees = 50.0f;
            //spotlight.RootComponent.InnerCutoffAngleDegrees = 30.0f;
            //actors.Add(spotlight);

            //Create camera shake test
            //PositionComponent posComp = new PositionComponent(new Vec3(0.0f, 50.0f, 0.0f));
            //ScreenShake3DComponent shakeComp = new ScreenShake3DComponent()
            //{
            //    MaxTrauma = 100.0f,
            //    TraumaDecrementPerSecond = 0.0f,
            //    Trauma = 40.0f,
            //};
            //CameraComponent camComp = new CameraComponent(new PerspectiveCamera(0.1f, 2000.0f, 45.0f, 1.0f));
            //posComp.ChildComponents.Add(shakeComp);
            //shakeComp.ChildComponents.Add(camComp);
            //Actor<PositionComponent> testScreenshake = new Actor<PositionComponent>(posComp);
            //actors.Add(testScreenshake);

            //Create point lights
            int lightCount = 5;
            float lightAngle = 360.0f / lightCount * TMath.DegToRadMultf;
            float lightPosRadius = 50.0f;
            float upTrans = 20.0f;
            for (int i = 0; i < lightCount; i++)
            {
                Actor<PointLightComponent> pointLight = new Actor<PointLightComponent>();
                pointLight.RootComponent.Radius = 200.0f;
                pointLight.RootComponent.DiffuseIntensity = 2000.0f;
                pointLight.RootComponent.AmbientIntensity = 0.0f;
                pointLight.RootComponent.Translation = new Vec3(
                    TMath.Cosf(i * lightAngle) * lightPosRadius,
                    upTrans,
                    TMath.Sinf(i * lightAngle) * lightPosRadius);
                actors.Add(pointLight);
            }

            Settings = new WorldSettings("UnitTestingWorld", new Map(new MapSettings(true, Vec3.Zero, actors)))
            {
                Bounds = new BoundingBox(500.0f),
                OriginRebaseBounds = new BoundingBox(50.0f),
            };
        }
    }

    public class SphereTraceActor : Actor<TRComponent>, I3DRenderable
    {
        private Vec3 _direction;
        private Matrix4 _endTraceTransform = Matrix4.Identity;
        private TCollisionSphere _sphere = TCollisionSphere.New(3.0f);
        private bool _hasHit = false;
        private Vec3 _hitPoint, _hitNormal, _drawPoint;
        private float _testDistance = 40.0f;

        public float TestDistance
        {
            get => _testDistance;
            set
            {
                _testDistance = value;
                RootComponent_WorldTransformChanged();
            }
        }
        public float Radius
        {
            get => _sphere.Radius;
            set => _sphere.Radius = value;
        }

        public RenderInfo3D RenderInfo { get; } 
            = new RenderInfo3D(Rendering.ERenderPass3D.OpaqueForward, null, false, false);

        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        public override void OnSpawnedPostComponentSetup(World world)
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            RootComponent.WorldTransformChanged += RootComponent_WorldTransformChanged;
            RootComponent.Rotation.Pitch = -90.0f;
            Engine.Scene.Add(this);
        }

        private void RootComponent_WorldTransformChanged()
        {
            _direction = Vec3.TransformVector(new Vec3(0.0f, 0.0f, -_testDistance), RootComponent.Rotation.GetMatrix());
            _endTraceTransform = _direction.AsTranslationMatrix() * RootComponent.WorldMatrix;
        }

        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            Engine.Scene.Remove(this);
        }

        private void Tick(float delta)
        {
            ShapeTraceClosest t = new ShapeTraceClosest(_sphere, RootComponent.WorldMatrix, _endTraceTransform, 0, 0xFFFF);
            if (_hasHit = t.Trace())
            {
                _hitPoint = t.HitPointWorld;
                _hitNormal = t.HitNormalWorld;
                _drawPoint = RootComponent.Translation + _direction * t.HitFraction;
            }
            else
            {
                _drawPoint = RootComponent.Translation + _direction;
            }
        }

        public void Render()
        {
            ColorF4 color = _hasHit ? Color.LimeGreen : Color.Red;
            Engine.Renderer.RenderLine(RootComponent.Translation, _drawPoint, color);
            Engine.Renderer.RenderSphere(_drawPoint, Radius, false, color);

            if (!_hasHit)
                return;
            
            Engine.Renderer.RenderLine(_hitPoint, _hitPoint + (_hitNormal * Radius), Color.Orange);
        }
    }
}
