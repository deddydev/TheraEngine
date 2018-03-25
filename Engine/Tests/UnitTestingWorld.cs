using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Actors.Types.ComponentActors.Shapes;
using TheraEngine.Worlds.Maps;
using TheraEngine.Rendering;
using TheraEngine.Actors.Types.Lights;
using TheraEngine.Actors.Types;
using TheraEngine.Rendering.Textures;
using TheraEngine.Files;
using System.Drawing.Imaging;
using TheraEngine.Core.Memory;
using TheraEngine.ThirdParty;

namespace TheraEngine.Tests
{
    public class UnitTestingWorld : World
    {
        protected unsafe internal override void OnLoaded()
        {
            int pointLights = 1;
            int dirLights = 1;
            int spotLights = 3;

            float margin = 2.0f;
            float radius = 1.0f;
            ColorF4 sphereColor = Color.Red;
            ColorF4 boxColor = Color.Blue;
            ColorF4 floorColor = Color.Gray;
            float diam = radius * 2.0f;
            float originDist = diam + margin;
            BoundingBox bounds = new BoundingBox(1000.0f);

            List<IActor> actors = new List<IActor>();
            IActor actor;

            #region Meshes
            int count = 4;
            int y = 0;

            Random rand = new Random(800);
            int maxVel = 50;
            int maxVelMod = maxVel * 100;

            //Create spheres
            for (int x = -count; x <= count; ++x)
                for (int z = -count; z <= count; ++z)
                {
                    TMaterial mat = TMaterial.CreateLitColorMaterial(sphereColor);
                    mat.Parameter<ShaderFloat>("Roughness").Value = ((x + count) / (float)count * 0.5f).ClampMin(0.0f);
                    mat.Parameter<ShaderFloat>("Metallic").Value = ((z + count) / (float)count * 0.5f).ClampMin(0.0f);
                    BoxActor sphere = new BoxActor("TestBox" + (y++).ToString(), radius, new Vec3(x * originDist, 0.0f, z * originDist), Rotator.GetZero(),
                        mat, new TRigidBodyConstructionInfo()
                        {
                            UseMotionState = true,
                            SimulatePhysics = true,
                            CollisionEnabled = true,
                            CollidesWith = (ushort)(TCollisionGroup.StaticWorld /*| TCollisionGroup.DynamicWorld*/),
                            CollisionGroup = (ushort)TCollisionGroup.DynamicWorld,
                            Restitution = 0.8f,
                            Mass = 100.0f,
                        });
                    sphere.RootComponent.RigidBodyCollision.AngularVelocity = new Vec3(
                        rand.Next(0, maxVelMod) / 100.0f,
                        rand.Next(0, maxVelMod) / 100.0f,
                        rand.Next(0, maxVelMod) / 100.0f);
                    sphere.RootComponent.RigidBodyCollision.LinearVelocity = new Vec3(
                        rand.Next(0, maxVelMod) / 100.0f,
                        rand.Next(0, maxVelMod) / 100.0f,
                        rand.Next(0, maxVelMod) / 100.0f);
                    actors.Add(sphere);
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

            Rotator[] rotations = 
            {
                new Rotator(0.0f, 0.0f, 0.0f),
                new Rotator(90.0f, 0.0f, 0.0f),
                new Rotator(180.0f, 0.0f, 0.0f),
                new Rotator(270.0f, 0.0f, 0.0f),
                new Rotator(90.0f, 90.0f, 0.0f),
                new Rotator(90.0f, -90.0f, 0.0f),
            };

            //Create walls
            //for (int i = 0; i < 6; ++i)
            //{
            //    Rotator r = rotations[i];
            //    actor = new BoxActor("Wall" + i,
            //        new Vec3(100.0f, 0.5f, 100.0f), Vec3.TransformPosition(new Vec3(0.0f, -100.0f, 0.0f), r.GetMatrix()),
            //        r, TMaterial.CreateLitColorMaterial(floorColor), new TRigidBodyConstructionInfo()
            //        {
            //            UseMotionState = false,
            //            SimulatePhysics = false,
            //            CollisionEnabled = true,
            //            CollidesWith = (ushort)TCollisionGroup.All,
            //            CollisionGroup = (ushort)TCollisionGroup.StaticWorld,
            //            Restitution = 0.8f,
            //            Mass = 50.0f,
            //        });
            //    actors.Add(actor);
            //}
            #endregion

            #region Lights
            //Create world lights
            if (dirLights > 0)
            {
                float lightAngle = 360.0f / dirLights;
                for (int i = 0; i < dirLights; ++i)
                {
                    DirectionalLightActor dirlight = new DirectionalLightActor();
                    DirectionalLightComponent dir = dirlight.RootComponent;
                    dir.LightColor = (ColorF3)Color.White;
                    dir.DiffuseIntensity = 1.0f;
                    dir.AmbientIntensity = 0.0f;
                    dir.WorldRadius = bounds.HalfExtents.LengthFast;
                    dir.Rotation.Pitch = -35.0f;
                    dir.Rotation.Yaw = lightAngle * i;
                    actors.Add(dirlight);
                }
            }
            //Create point lights
            if (pointLights > 0)
            {
                float lightAngle = 360.0f / pointLights * TMath.DegToRadMultf;
                float lightPosRadius = 50.0f;
                float upTrans = 20.0f;
                for (int i = 0; i < pointLights; i++)
                {
                    PointLightComponent comp = new PointLightComponent(400.0f, 5.0f, (ColorF3)Color.White, 2000.0f, 0.0f)
                    {
                        Translation = new Vec3(
                            TMath.Cosf(i * lightAngle) * lightPosRadius,
                            upTrans,
                            TMath.Sinf(i * lightAngle) * lightPosRadius)
                    };
                    Actor<PointLightComponent> pointLight = new Actor<PointLightComponent>(comp);
                    actors.Add(pointLight);
                }
            }
            //Create spot light
            if (spotLights > 0)
            {
                float lightAngle = 360.0f / spotLights;
                float lightAngleRad = lightAngle * TMath.DegToRadMultf;
                float lightPosRadius = 50.0f;
                float upTrans = 70.0f;
                for (int i = 0; i < spotLights; ++i)
                {
                    SpotLightComponent spot = new SpotLightComponent()
                    {
                        LightColor = (ColorF3)Color.White,
                        AmbientIntensity = 0.0f,
                        Distance = 2000.0f,
                        Brightness = 100.0f,
                        Exponent = 2.0f,
                        OuterCutoffAngleDegrees = 50.0f,
                        InnerCutoffAngleDegrees = 30.0f,
                        Translation = new Vec3(
                            TMath.Cosf(i * lightAngleRad) * lightPosRadius,
                            upTrans,
                            TMath.Sinf(i * lightAngleRad) * lightPosRadius)
                    };
                    spot.Rotation.Pitch = -60.0f;
                    spot.Rotation.Yaw = lightAngle * ((spotLights - i) - spotLights + 1);
                    Actor<SpotLightComponent> spotlight = new Actor<SpotLightComponent>(spot);
                    actors.Add(spotlight);
                }
            }
            #endregion

            int wh = 65;
            FastNoise noise = new FastNoise((int)DateTime.Now.Ticks);
            //noise.SetFrequency(1.0f);
            DataSource source = new DataSource(wh * wh * 4);
            float* data = (float*)source.Address;
            float temp;
            for (int r = 0; r < wh; ++r)
                for (int x = 0; x < wh; ++x)
                {
                    temp = noise.GetPerlin(x, r) * 50.0f;
                    *data++ = temp;
                }

            TRigidBodyConstructionInfo landscapeInfo = new TRigidBodyConstructionInfo()
            {
                CollidesWith = (ushort)TCollisionGroup.All,
                CollisionGroup = (ushort)TCollisionGroup.StaticWorld,
            };

            Actor<LandscapeComponent> landscape = new Actor<LandscapeComponent>();
            //TextureFile2D f = Load<TextureFile2D>("");
            //Bitmap bmp = f.Bitmaps[0];
            //BitmapData d = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            //DataSource source = new DataSource(d.Scan0, d.Width * d.Height * d.Stride, true);
            landscape.RootComponent.GenerateHeightFieldCollision(
                source, wh, wh, 0.0f, 50.0f,
                TCollisionHeightField.EHeightValueType.Single,
                landscapeInfo);
            //landscape.RootComponent.Translation.Y -= 50.0f;
            actors.Add(landscape);

            //Create shape tracer
            //actor = new SphereTraceActor();
            //actors.Add(actor);

            //float rotationsPerSecond = 0.1f, testRadius = 30.0f, testHeight = 20.0f;
            //PropAnimMethod<Vec3> animMethod = new PropAnimMethod<Vec3>(
            //    1.0f / rotationsPerSecond, true, second =>
            //{
            //    float theta = (rotationsPerSecond * second).RemapToRange(0.0f, 1.0f) * 360.0f;
            //    //float mult = 1.5f - 4.0f * TMath.Cosdf(theta);
            //    Vec2 coord = TMath.PolarToCartesianDeg(theta, testRadius/* * mult*/);
            //    return new Vec3(coord.X, testHeight, -coord.Y);
            //});
            //AnimationContainer anim = new AnimationContainer(
            //    "RotationTrace", "Translation.Raw", false, animMethod);
            //actor.RootComponent.AddAnimation(anim, true, false, 
            //    ETickGroup.PostPhysics, ETickOrder.Animation, Input.Devices.EInputPauseType.TickAlways);

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

            Settings = new WorldSettings("UnitTestingWorld", new Map(new MapSettings(true, Vec3.Zero, actors)))
            {
                Bounds = bounds,
                OriginRebaseBounds = new BoundingBox(50.0f),
                EnableOriginRebasing = false,
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
            = new RenderInfo3D(ERenderPass3D.OpaqueForward, null, false, false);

        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        public override void OnSpawnedPostComponentSetup()
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            RootComponent.WorldTransformChanged += RootComponent_WorldTransformChanged;
            RootComponent.Rotation.Pitch = -90.0f;
            OwningWorld.Scene.Add(this);
        }

        private void RootComponent_WorldTransformChanged()
        {
            _direction = Vec3.TransformVector(new Vec3(0.0f, 0.0f, -_testDistance), RootComponent.Rotation.GetMatrix());
            _endTraceTransform = _direction.AsTranslationMatrix() * RootComponent.WorldMatrix;
        }

        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            OwningWorld.Scene.Remove(this);
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
