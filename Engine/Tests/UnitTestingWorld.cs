using Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.ComponentActors.Shapes;
using TheraEngine.Animation;
using TheraEngine.Components;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Components.Scene.Volumes;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Physics;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using TheraEngine.ThirdParty;
using TheraEngine.Worlds;

namespace TheraEngine.Tests
{
    public class UnitTestingWorld : World
    {
        private Random _rand = new Random();
        private void RigidBodyCollision_Collided1(TCollisionObject @this, TCollisionObject other, TContactInfo info, bool thisIsA)
        {
            ShaderVec3 color = (ShaderVec3)((StaticMeshComponent)@this.Owner).ModelRef.File.RigidChildren[0].LODs[0].MaterialRef.File.Parameters[0];
            color.Value = new Vec3((float)_rand.NextDouble(), (float)_rand.NextDouble(), (float)_rand.NextDouble());
            //_collideSound.Play(_param);
        }
        private float _time = 0.0f;
        private FastNoise _noise;
        private LandscapeComponent _landscape;
        private int _landscapeWH;
        Stopwatch _watch = new Stopwatch();
        private unsafe void TickLandscape(float delta)
        {
            //_watch.Reset();
            //_watch.Start();
            _time += delta * 5.0f;
            float* data = (float*)_landscape.DataSource.Address;
            //float temp;
            Parallel.For(0, _landscapeWH * _landscapeWH, i =>
            {
                *(data + i) = _noise.GetPerlin(i % _landscapeWH, i / _landscapeWH, _time) * 50.0f;
            });
            //for (int r = 0; r < _landscapeWH; ++r)
            //    for (int x = 0; x < _landscapeWH; ++x)
            //    {
            //        temp = _noise.GetPerlin(x, r, _time) * 100.0f;
            //        *data++ = temp;
            //    }
            //_watch.Stop();
            //long loopMs = _watch.ElapsedMilliseconds;
            //_watch.Reset();
            //_watch.Start();
            _landscape.HeightDataChanged();
            //_watch.Stop();
            //long meshMs = _watch.ElapsedMilliseconds;
            //Engine.PrintLine($"{loopMs} loop, {meshMs} update");
        }
        protected override async void OnBeginPlay()
        {
            bool testDeferredDecal = true;
            bool testShapeTracer = false;
            bool testLandscape = true;
            bool createWalls = false;
            int pointLights = 0;
            int dirLights = 1;
            int spotLights = 0;

            float margin = 2.0f;
            float radius = 1.0f;
            ColorF4 sphereColor = Color.Red;
            ColorF4 boxColor = Color.Blue;
            ColorF4 floorColor = Color.Gray;
            float diam = radius * 2.0f;
            float originDist = diam + margin;
            BoundingBox bounds = new BoundingBox(1000.0f);

            List<BaseActor> actors = new List<BaseActor>();
            BaseActor actor;

            #region Meshes
            int count = 2;
            int y = 0;

            Random rand = new Random((int)DateTime.Now.Ticks);
            int maxVel = 50;
            int maxVelMod = maxVel * 100;
            int halfMax = maxVelMod / 2;
            for (int x = -count; x <= count; ++x)
                for (int z = -count; z <= count; ++z)
                {
                    float xV = ((x + count) / (float)count * 0.5f).ClampMin(0.0f);
                    float zV = ((z + count) / (float)count * 0.5f).ClampMin(0.0f);
                    TMaterial mat = TMaterial.CreateLitColorMaterial(new ColorF4(xV, zV, 0.0f, 1.0f));
                    //mat.RenderParams.StencilTest = Editor.EditorState.OutlinePassStencil;
                    //mat.Requirements = TMaterial.UniformRequirements.NeedsCamera;
                    //mat.AddShader(Engine.LoadEngineShader("VisualizeNormal.gs", EShaderMode.Geometry));
                    mat.Parameter<ShaderFloat>("Roughness").Value = xV;
                    mat.Parameter<ShaderFloat>("Metallic").Value = zV;
                    TRigidBodyConstructionInfo cinfo = new TRigidBodyConstructionInfo()
                    {
                        Mass = 100.0f,
                        Friction = 0.5f,
                        Restitution = 0.5f,
                        LinearDamping = 0.1f,
                        AngularDamping = 0.1f,
                        UseMotionState = true,
                        RollingFriction = 0.1f,
                        SleepingEnabled = true,
                        SimulatePhysics = true,
                        CollisionEnabled = true,
                        DeactivationTime = 0.1f,
                        //CcdMotionThreshold = 0.4f,
                        CustomMaterialCallback = true,
                        //ContactProcessingThreshold = 3.0f,
                        LinearSleepingThreshold = 0.0f,
                        AngularSleepingThreshold = 0.0f,
                        //CcdSweptSphereRadius = radius * 0.95f,
                        CollisionGroup = (ushort)ETheraCollisionGroup.DynamicWorld,
                        CollidesWith = (ushort)(ETheraCollisionGroup.StaticWorld | ETheraCollisionGroup.DynamicWorld),
                    };
                    Actor<StaticMeshComponent> sphere = //((x ^ z) & 1) == 0 ?
                        //(Actor<StaticMeshComponent>)new ConeActor("TestCone" + (y++).ToString(), radius, radius * 2.0f, new Vec3(x * originDist, 0.0f, z * originDist), Rotator.GetZero(), mat, cinfo, 20) :
                        new SphereActor("TestSphere" + (y++).ToString(), radius, new Vec3(x * originDist, 0.0f, z * originDist), Quat.Identity, mat, cinfo, 20);

                    //sphere.RootComponent.RigidBodyCollision.AngularVelocity = new Vec3(
                    //    rand.Next(-halfMax, halfMax) / maxVel,
                    //    rand.Next(-halfMax, halfMax) / maxVel,
                    //    rand.Next(-halfMax, halfMax) / maxVel);
                    //sphere.RootComponent.RigidBodyCollision.LinearVelocity = new Vec3(
                    //    rand.Next(-halfMax, halfMax) / maxVel,
                    //    rand.Next(-halfMax, halfMax) / maxVel,
                    //    rand.Next(-halfMax, halfMax) / maxVel);
                    //sphere.RootComponent.RigidBodyCollision.Collided += RigidBodyCollision_Collided1;
                    //foreach (var mesh in sphere.RootComponent.Meshes)
                    //    foreach (var lod in mesh.LODs)
                    //    {
                    //        StencilTest st = lod.Manager.Material.RenderParams.StencilTest;

                    //        st.Enabled = ERenderParamUsage.Enabled;

                    //        st.FrontFace.BothFailOp = EStencilOp.Keep;
                    //        st.FrontFace.StencilPassDepthFailOp = EStencilOp.Keep;
                    //        st.FrontFace.BothPassOp = EStencilOp.Replace;
                    //        st.FrontFace.Func = EComparison.Always;
                    //        st.FrontFace.Ref = rand.Next(0, byte.MaxValue);
                    //        st.FrontFace.ReadMask = 0xFF;
                    //        st.FrontFace.WriteMask = 0xFF;

                    //        st.BackFace.BothFailOp = EStencilOp.Keep;
                    //        st.BackFace.StencilPassDepthFailOp = EStencilOp.Keep;
                    //        st.BackFace.BothPassOp = EStencilOp.Replace;
                    //        st.BackFace.Func = EComparison.Always;
                    //        st.BackFace.Ref = rand.Next(0, byte.MaxValue);
                    //        st.BackFace.ReadMask = 0xFF;
                    //        st.BackFace.WriteMask = 0xFF;
                    //    }

                    actors.Add(sphere);
                }

            //Create walls
            if (createWalls)
            {
                Quat[] rotations =
                {
                    Quat.Euler(0.0f, 0.0f, 0.0f),
                    Quat.Euler(90.0f, 0.0f, 0.0f),
                    Quat.Euler(180.0f, 0.0f, 0.0f),
                    Quat.Euler(-90.0f, 0.0f, 0.0f),
                    Quat.Euler(90.0f, 90.0f, 0.0f),
                    Quat.Euler(90.0f, -90.0f, 0.0f),
                };

                for (int i = 0; i < 6; ++i)
                {
                    Quat rotation = rotations[i];
                    actor = new BoxActor("Wall" + i,
                        new Vec3(100.0f, 0.5f, 100.0f),
                        new Vec3(0.0f, -100.0f, 0.0f) * rotation,
                        rotation, 
                        TMaterial.CreateLitColorMaterial(floorColor),
                        new TRigidBodyConstructionInfo()
                        {
                            UseMotionState = false,
                            SimulatePhysics = false,
                            CollisionEnabled = true,
                            CollidesWith = (ushort)(~ETheraCollisionGroup.StaticWorld & ETheraCollisionGroup.All),
                            CollisionGroup = (ushort)ETheraCollisionGroup.StaticWorld,
                            Restitution = 0.5f,
                            Mass = 50.0f,
                        });
                    actors.Add(actor);
                }
            }
            #endregion

            #region Lights
            //Create world lights
            if (dirLights > 0)
            {
                float lightAngle = 360.0f / dirLights;
                for (int i = 0; i < dirLights; ++i)
                {
                    Actor<DirectionalLightComponent> dirlight = new Actor<DirectionalLightComponent>();
                    DirectionalLightComponent dir = dirlight.RootComponent;
                    dir.LightColor = (ColorF3)Color.White;
                    dir.DiffuseIntensity = 1.0f;
                    dir.Scale = 500.0f;
                    dir.Transform.Rotation.Value = Quat.Euler(-20.0f, lightAngle * i, 0.0f);
                    actors.Add(dirlight);
                }
            }
            //Create point lights
            //if (pointLights > 0)
            //{
            //    float lightAngle = 360.0f / pointLights * TMath.DegToRadMultf;
            //    float lightPosRadius = 50.0f;
            //    float upTrans = 20.0f;
            //    for (int i = 0; i < pointLights; i++)
            //    {
            //        PointLightComponent comp = new PointLightComponent(200.0f, 2.0f, (ColorF3)Color.White, 2000.0f)
            //        {
            //            Translation = new Vec3(
            //                TMath.Cosf(i * lightAngle) * lightPosRadius,
            //                upTrans,
            //                TMath.Sinf(i * lightAngle) * lightPosRadius),
            //            //LightColor = new ColorF3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()),
            //        };
            //        Actor<PointLightComponent> pointLight = new Actor<PointLightComponent>(comp);
            //        actors.Add(pointLight);
            //    }
            //}
            //Create spot light
            //if (spotLights > 0)
            //{
            //    float lightAngle = 360.0f / spotLights;
            //    float lightAngleRad = lightAngle * TMath.DegToRadMultf;
            //    float lightPosRadius = 50.0f;
            //    float upTrans = 70.0f;
            //    for (int i = 0; i < spotLights; ++i)
            //    {
            //        SpotLightComponent spot = new SpotLightComponent()
            //        {
            //            LightColor = (ColorF3)Color.White,
            //            Distance = 500.0f,
            //            Brightness = 100.0f,
            //            Exponent = 2.0f,
            //            OuterCutoffAngleDegrees = 50.0f,
            //            InnerCutoffAngleDegrees = 30.0f,
            //            Translation = new Vec3(
            //                TMath.Cosf(i * lightAngleRad) * lightPosRadius,
            //                upTrans,
            //                TMath.Sinf(i * lightAngleRad) * lightPosRadius)
            //        };
            //        spot.Rotation.Pitch = -60.0f;
            //        spot.Rotation.Yaw = lightAngle * ((spotLights - i) - spotLights + 1);
            //        Actor<SpotLightComponent> spotlight = new Actor<SpotLightComponent>(spot);
            //        actors.Add(spotlight);
            //    }
            //}
            #endregion

            #region Landscape
            if (testLandscape)
            {
                int wh = 401;
                _landscapeWH = wh;
                FastNoise noise = new FastNoise();
                _noise = noise;
                //noise.SetFrequency(10.0f);
                DataSource source = new DataSource(wh * wh * 4);
                unsafe
                {
                    float* data = (float*)source.Address;
                    float temp;
                    for (int r = 0; r < wh; ++r)
                        for (int x = 0; x < wh; ++x)
                        {
                            temp = noise.GetPerlin(x, r, 0.0f) * 50.0f;
                            *data++ = temp;
                        }
                }
                TRigidBodyConstructionInfo landscapeInfo = new TRigidBodyConstructionInfo()
                {
                    CollidesWith = (ushort)(ETheraCollisionGroup.All & ~ETheraCollisionGroup.StaticWorld),
                    CollisionGroup = (ushort)ETheraCollisionGroup.StaticWorld,
                    IsKinematic = true,
                };

                Actor<LandscapeComponent> landscape = new Actor<LandscapeComponent>();
                //TextureFile2D f = Load<TextureFile2D>("");
                //Bitmap bmp = f.Bitmaps[0];
                //BitmapData d = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                //DataSource source = new DataSource(d.Scan0, d.Width * d.Height * d.Stride, true);
                landscape.RootComponent.GenerateHeightFieldCollision(
                    source, wh, wh, -100.0f, 100.0f,
                    TCollisionHeightField.EHeightValueType.Single,
                    landscapeInfo);
                TMaterial mat = TMaterial.CreateLitColorMaterial(Color.LightBlue);
                //mat.Requirements = TMaterial.UniformRequirements.NeedsCamera;
                //mat.AddShader(Engine.LoadEngineShader("VisualizeNormal.gs", EShaderMode.Geometry));
                mat.Parameter<ShaderFloat>("Roughness").Value = 1.0f;
                mat.Parameter<ShaderFloat>("Metallic").Value = 0.0f;
                landscape.RootComponent.GenerateHeightFieldMesh(mat, 2);
                landscape.RootComponent.Transform.Translation.Y -= 25.0f;
                _landscape = landscape.RootComponent;
                actors.Add(landscape);
            }
            #endregion

            #region Shape Tracer
            if (testShapeTracer)
            {
                actors.Add(new SphereTraceActor());
            }
            #endregion

            #region Camera Shake
            //Create camera shake test
            TransformComponent posComp = new TransformComponent(new TTransform(new Vec3(0.0f, 50.0f, 0.0f), Quat.Identity, Vec3.One));
            ScreenShake3DComponent shakeComp = new ScreenShake3DComponent()
            {
                MaxTrauma = 100.0f,
                TraumaDecrementPerSecond = 0.0f,
                Trauma = 40.0f,
            };
            CameraComponent camComp = new CameraComponent(new PerspectiveCamera(0.1f, 2000.0f, 45.0f, 1.0f));
            posComp.ChildSockets.Add(shakeComp);
            shakeComp.ChildSockets.Add(camComp);
            Actor<TransformComponent> testScreenshake = new Actor<TransformComponent>(posComp);
            actors.Add(testScreenshake);
            #endregion

            #region Skybox
            TextureFile2D skyTex = await Engine.Files.LoadEngineTexture2DAsync("modelviewerbg2.png");
            SkyboxActor skyboxActor = new SkyboxActor(skyTex, 1000.0f);
            actors.Add(skyboxActor);
            #endregion

            Actor<TriggerVolumeComponent> triggerVolumeActor = new Actor<TriggerVolumeComponent>();
            triggerVolumeActor.RootComponent.Transform.Translation.Y -= 30.0f;
            triggerVolumeActor.RootComponent.Shape.HalfExtents.Value = new Vec3(10.0f, 2.0f, 10.0f);
            actors.Add(triggerVolumeActor);

            #region Decal
            if (testDeferredDecal)
            {
                Actor<DecalComponent> decal = new Actor<DecalComponent>();
                TextureFile2D decalTex = await Engine.Files.LoadEngineTexture2DAsync("decal guide.png");
                decal.RootComponent.Material = DecalComponent.CreateDefaultMaterial(decalTex);

                var bmp = decalTex.GetBitmap();
                float maxDim = Math.Max(bmp.Width, bmp.Height);
                decal.RootComponent.Shape.HalfExtents = new Vec3(
                    bmp.Width / maxDim * 0.5f,
                    1.0f, 
                    bmp.Height / maxDim * 0.5f);

                actors.Add(decal);
            }
            #endregion

            IBLProbeGridActor iblProbes = new IBLProbeGridActor();
            //iblProbes.RootComponent.Translation.Y += 3.0f;
            //Random random = new Random();
            //for (int i = 0; i < 10; ++i)
            //{
            //    iblProbes.AddProbe(new Vec3(
            //        ((float)random.NextDouble() - 0.5f) * 200.0f,
            //        ((float)random.NextDouble() - 0.5f) * 200.0f,
            //        ((float)random.NextDouble() - 0.5f) * 200.0f));
            //}
            //iblProbes.AddProbe(new Vec3(50.0f, 0.0f, 0.0f));
            //iblProbes.AddProbe(new Vec3(-51.0f, 0.0f, 0.0f));
            iblProbes.AddProbe(new Vec3(0.0f, 100.0f, 0.0f));
            //iblProbes.AddProbe(new Vec3(0.0f, -53.0f, 0.0f));
            //iblProbes.AddProbe(new Vec3(10.0f, 0.0f, 54.0f));
            //iblProbes.AddProbe(new Vec3(0.0f, 0.0f, -55.0f));
            //iblProbes.AddProbe(new Vec3(0.0f, -70.0f, 154.0f));
            //iblProbes.AddProbe(new Vec3(0.0f, 60.0f, -155.0f));
            //iblProbes.SetFrequencies(BoundingBox.FromHalfExtentsTranslation(100.0f, Vec3.Zero), new Vec3(0.02f));
            actors.Add(iblProbes);

            Settings = new WorldSettings("UnitTestingWorld", new Map(true, Vec3.Zero, actors))
            {
                Bounds = bounds,
                OriginRebaseRadius = 100.0f,
                EnableOriginRebasing = false,
            };

            base.OnBeginPlay();

            //RegisterTick(ETickGroup.PrePhysics, ETickOrder.Timers, TickLandscape, EInputPauseType.TickOnlyWhenUnpaused);
        }
    }

    public class SphereTraceActor : Actor<TransformComponent>, I3DRenderable
    {
        public SphereTraceActor()
        {
            _renderCommand = new RenderCommandMethod3D(ERenderPass.OpaqueForward, Render);
            _sphere = TCollisionSphere.New(2.0f);
            _shapeCast = new ShapeTraceClosest(_sphere, Matrix4.Identity, Matrix4.Identity,
                (ushort)(ETheraCollisionGroup.DynamicWorld), (ushort)(ETheraCollisionGroup.StaticWorld | ETheraCollisionGroup.DynamicWorld));
        }

        private Vec3 _direction;
        private Matrix4 _endTraceTransform = Matrix4.Identity;
        private TCollisionSphere _sphere;
        private bool _hasHit = false;
        private Vec3 _hitPoint, _hitNormal, _drawPoint;
        private float _testDistance = 40.0f;
        private ShapeTraceClosest _shapeCast;

        public float TestDistance
        {
            get => _testDistance;
            set
            {
                _testDistance = value;
                RootComponent_WorldTransformChanged(null);
            }
        }
        public float Radius
        {
            get => _sphere.Radius;
            set => _sphere.Radius = value;
        }

        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, true);
        
        private PropAnimMethod<Vec3> _methodAnim;
        private AnimationTree _animTree;
        private float _rotationsPerSecond = 0.2f;
        public float RotationsPerSecond
        {
            get => _rotationsPerSecond;
            set
            {
                _rotationsPerSecond = value;
                float length = 1.0f / RotationsPerSecond;
                _methodAnim?.SetLength(length, false);
                _animTree?.SetLength(length, false);
            }
        }
        public float TestRadius { get; set; } = 15.0f;
        public float TestHeight { get; set; } = 20.0f;
        
        private Vec3 AnimTick(float second)
        {
            float theta = (RotationsPerSecond * second).RemapToRange(0.0f, 1.0f) * 360.0f;
            //float mult = 1.5f - 4.0f * TMath.Cosdf(theta);
            Vec2 coord = TMath.PolarToCartesianDeg(theta, TestRadius/* * mult*/);
            return new Vec3(coord.X, TestHeight, -coord.Y);
        }
        protected override void OnSpawnedPreComponentSpawn()
        {
            float length = 1.0f / RotationsPerSecond;
            _methodAnim = new PropAnimMethod<Vec3>(length, true, AnimTick);
            _animTree = new AnimationTree("RotationTrace", "Translation.Raw", _methodAnim);
            RootComponent.Animations = new EventList<AnimationTree>();
            _animTree.Group = ETickGroup.PrePhysics;
            _animTree.Order = ETickOrder.Animation;
            _animTree.PausedBehavior = EInputPauseType.TickAlways;
            _animTree.BeginOnSpawn = true;
            _animTree.TickSelf = true;
            RootComponent.Animations.Add(_animTree);
        }
        protected override void OnSpawnedPostComponentSpawn()
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            RootComponent.WorldTransformChanged += RootComponent_WorldTransformChanged;
            //RootComponent.Transform.Rotation.Pitch = -90.0f;
        }
        protected override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
        }

        private void RootComponent_WorldTransformChanged(ISceneComponent comp)
        {
            //_direction = Vec3.TransformVector(new Vec3(0.0f, 0.0f, -_testDistance), RootComponent.Transform.Rotation.GetMatrix());
            _endTraceTransform = _direction.AsTranslationMatrix() * RootComponent.WorldMatrix.Value;
        }

        private void Tick(float delta)
        {
            _shapeCast.Start = RootComponent.WorldMatrix.Value;
            _shapeCast.End = _endTraceTransform;
            if (_hasHit = _shapeCast.Trace(OwningWorld))
            {
                _hitPoint = _shapeCast.HitPointWorld;
                _hitNormal = _shapeCast.HitNormalWorld;
                _drawPoint = RootComponent.Transform.Translation + _direction * _shapeCast.HitFraction;
            }
            else
            {
                _drawPoint = RootComponent.Transform.Translation + _direction;
            }
        }

        public void Render(bool shadowPass)
        {
            ColorF4 color = _hasHit ? Color.LimeGreen : Color.Red;
            Engine.Renderer.RenderLine(RootComponent.Transform.Translation, _drawPoint, color);
            Engine.Renderer.RenderSphere(_drawPoint, Radius, false, color);

            if (!_hasHit)
                return;
            
            Engine.Renderer.RenderLine(_hitPoint, _hitPoint + (_hitNormal * Radius), Color.Orange);
        }

        private readonly RenderCommandMethod3D _renderCommand;
        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            passes.Add(_renderCommand);
        }
    }
}
