using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.ComponentActors.Shapes;
using TheraEngine.Actors.Types.Lights;
using TheraEngine.Animation;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using TheraEngine.ThirdParty;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Maps;

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

        public override async void BeginPlay()
        {
            bool testLandscape = true;
            bool createWalls = true;
            int pointLights = 2;
            int dirLights = 0;
            int spotLights = 0;

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

            Random rand = new Random((int)DateTime.Now.Ticks);
            int maxVel = 50;
            int maxVelMod = maxVel * 100;
            int halfMax = maxVelMod / 2;
            for (int x = -count; x <= count; ++x)
                for (int z = -count; z <= count; ++z)
                {
                    float xV = ((x + count) / (float)count * 0.5f).ClampMin(0.0f);
                    float zV = ((z + count) / (float)count * 0.5f).ClampMin(0.0f);
                    TMaterial mat = TMaterial.CreateLitColorMaterial(sphereColor/*new ColorF4(xV, zV, 0.0f, 1.0f)*/);
                    mat.RenderParams.StencilTest = Editor.EditorState.OutlinePassStencil;
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
                        SimulatePhysics = true,
                        CollisionEnabled = true,
                        SleepingEnabled = true,
                        DeactivationTime = 0.1f,
                        //CcdMotionThreshold = 0.4f,
                        CustomMaterialCallback = true,
                        //ContactProcessingThreshold = 3.0f,
                        LinearSleepingThreshold = 0.0f,
                        AngularSleepingThreshold = 0.0f,
                        //CcdSweptSphereRadius = radius * 0.95f,
                        CollisionGroup = (ushort)TCollisionGroup.DynamicWorld,
                        CollidesWith = (ushort)(TCollisionGroup.StaticWorld | TCollisionGroup.DynamicWorld),
                    };
                    Actor<StaticMeshComponent> sphere = ((x ^ z) & 1) == 0 ?
                        (Actor<StaticMeshComponent>)new ConeActor("TestCone" + (y++).ToString(), radius, radius * 2.0f, new Vec3(x * originDist, 0.0f, z * originDist), Rotator.GetZero(), mat, cinfo) :
                        new SphereActor("TestSphere" + (y++).ToString(), radius, new Vec3(x * originDist, 0.0f, z * originDist), Rotator.GetZero(), mat, cinfo);

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
                Rotator[] rotations =
                {
                    new Rotator(0.0f, 0.0f, 0.0f),
                    new Rotator(90.0f, 0.0f, 0.0f),
                    new Rotator(180.0f, 0.0f, 0.0f),
                    new Rotator(270.0f, 0.0f, 0.0f),
                    new Rotator(90.0f, 90.0f, 0.0f),
                    new Rotator(90.0f, -90.0f, 0.0f),
                };

                for (int i = 0; i < 1; ++i)
                {
                    Rotator r = rotations[i];
                    actor = new BoxActor("Wall" + i,
                        new Vec3(100.0f, 0.5f, 100.0f), Vec3.TransformPosition(new Vec3(0.0f, -100.0f, 0.0f), r.GetMatrix()),
                        r, TMaterial.CreateLitColorMaterial(floorColor), new TRigidBodyConstructionInfo()
                        {
                            UseMotionState = false,
                            SimulatePhysics = false,
                            CollisionEnabled = true,
                            CollidesWith = (ushort)(~TCollisionGroup.StaticWorld & TCollisionGroup.All),
                            CollisionGroup = (ushort)TCollisionGroup.StaticWorld,
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
                    DirectionalLightActor dirlight = new DirectionalLightActor();
                    DirectionalLightComponent dir = dirlight.RootComponent;
                    dir.LightColor = (ColorF3)Color.White;
                    dir.DiffuseIntensity = 1.0f;
                    dir.Scale = 500.0f;
                    dir.Rotation.Pitch = -20.0f;
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
                    PointLightComponent comp = new PointLightComponent(200.0f, 2.0f, (ColorF3)Color.White, 2000.0f)
                    {
                        Translation = new Vec3(
                            TMath.Cosf(i * lightAngle) * lightPosRadius,
                            upTrans,
                            TMath.Sinf(i * lightAngle) * lightPosRadius),
                        //LightColor = new ColorF3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()),
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
                        Distance = 500.0f,
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

            if (testLandscape)
            {
                int wh = 401;
                FastNoise noise = new FastNoise();
                //noise.SetFrequency(10.0f);
                DataSource source = new DataSource(wh * wh * 4);
                unsafe
                {
                    float* data = (float*)source.Address;
                    float temp;
                    for (int r = 0; r < wh; ++r)
                        for (int x = 0; x < wh; ++x)
                        {
                            temp = noise.GetCubic(x, r) * 50.0f;
                            *data++ = temp;
                        }
                }
                TRigidBodyConstructionInfo landscapeInfo = new TRigidBodyConstructionInfo()
                {
                    CollidesWith = (ushort)(TCollisionGroup.All & ~TCollisionGroup.StaticWorld),
                    CollisionGroup = (ushort)TCollisionGroup.StaticWorld,
                    IsKinematic = true,
                };

                Actor<LandscapeComponent> landscape = new Actor<LandscapeComponent>();
                //TextureFile2D f = Load<TextureFile2D>("");
                //Bitmap bmp = f.Bitmaps[0];
                //BitmapData d = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                //DataSource source = new DataSource(d.Scan0, d.Width * d.Height * d.Stride, true);
                landscape.RootComponent.GenerateHeightFieldCollision(
                    source, wh, wh, -50.0f, 50.0f,
                    TCollisionHeightField.EHeightValueType.Single,
                    landscapeInfo);
                TMaterial mat = TMaterial.CreateLitColorMaterial(Color.LightBlue);
                //mat.Requirements = TMaterial.UniformRequirements.NeedsCamera;
                //mat.AddShader(Engine.LoadEngineShader("VisualizeNormal.gs", EShaderMode.Geometry));
                mat.Parameter<ShaderFloat>("Roughness").Value = 1.0f;
                mat.Parameter<ShaderFloat>("Metallic").Value = 0.0f;
                landscape.RootComponent.GenerateHeightFieldMesh(mat, 10);
                landscape.RootComponent.Translation.Y -= 20.0f;
                actors.Add(landscape);
            }

            //Create shape tracer
            //actor = new SphereTraceActor();
            //actors.Add(actor);

            //float rotationsPerSecond = 0.2f, testRadius = 15.0f, testHeight = 20.0f;
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

            Vec3 max = 1000.0f;
            Vec3 min = -max;
            TextureFile2D skyTex = await Engine.LoadEngineTexture2DAsync("modelviewerbg1.png");
            StaticModel skybox = new StaticModel("Skybox");
            TexRef2D texRef = new TexRef2D("SkyboxTexture", skyTex)
            {
                MagFilter = ETexMagFilter.Nearest,
                MinFilter = ETexMinFilter.Nearest
            };
            StaticRigidSubMesh mesh = new StaticRigidSubMesh("Mesh", true,
                BoundingBox.FromMinMax(min, max),
                BoundingBox.SolidMesh(min, max, true,
                skyTex.Bitmaps[0].Width > skyTex.Bitmaps[0].Height ?
                    BoundingBox.ECubemapTextureUVs.WidthLarger :
                    BoundingBox.ECubemapTextureUVs.HeightLarger),
                TMaterial.CreateUnlitTextureMaterialForward(texRef, new RenderingParameters()
                {
                    DepthTest = new DepthTest()
                    {
                        Enabled = ERenderParamUsage.Enabled,
                        UpdateDepth = false,
                        Function = EComparison.Less
                    }
                }));
            mesh.RenderInfo.RenderPass = ERenderPass.Background;
            skybox.RigidChildren.Add(mesh);
            Actor<StaticMeshComponent> skyboxActor = new Actor<StaticMeshComponent>();
            skyboxActor.RootComponent.ModelRef = skybox;
            actors.Add(skyboxActor);

            IBLProbeGridActor iblProbes = new IBLProbeGridActor();
            iblProbes.RootComponent.Translation.Y += 3.0f;
            iblProbes.SetFrequencies(BoundingBox.FromHalfExtentsTranslation(100.0f, Vec3.Zero), new Vec3(0.02f));
            actors.Add(iblProbes);

            Settings = new WorldSettings("UnitTestingWorld", new Map(new MapSettings(true, Vec3.Zero, actors)))
            {
                Bounds = bounds,
                OriginRebaseBounds = new BoundingBox(50.0f),
                EnableOriginRebasing = false,
            };

            base.BeginPlay();

            iblProbes.InitAndCaptureAll(512);
        }
    }

    public class SphereTraceActor : Actor<TRComponent>, I3DRenderable
    {
        public SphereTraceActor()
        {
            _renderCommand = new RenderCommandDebug3D(Render);
            _sphere = TCollisionSphere.New(2.0f);
            _shapeCast = new ShapeTraceClosest(_sphere, Matrix4.Identity, Matrix4.Identity,
                (ushort)(TCollisionGroup.DynamicWorld), (ushort)(TCollisionGroup.StaticWorld | TCollisionGroup.DynamicWorld));
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
                RootComponent_WorldTransformChanged();
            }
        }
        public float Radius
        {
            get => _sphere.Radius;
            set => _sphere.Radius = value;
        }

        public RenderInfo3D RenderInfo { get; } 
            = new RenderInfo3D(ERenderPass.OpaqueForward, false, false);

        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        public bool Visible { get; set; } = true;
        public bool VisibleInEditorOnly { get; set; } = false;
        public bool HiddenFromOwner { get; set; } = false;
        public bool VisibleToOwnerOnly { get; set; } = false;

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
            _shapeCast.Start = RootComponent.WorldMatrix;
            _shapeCast.End = _endTraceTransform;
            if (_hasHit = _shapeCast.Trace())
            {
                _hitPoint = _shapeCast.HitPointWorld;
                _hitNormal = _shapeCast.HitNormalWorld;
                _drawPoint = RootComponent.Translation + _direction * _shapeCast.HitFraction;
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

        private RenderCommandDebug3D _renderCommand;
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_renderCommand, RenderInfo.RenderPass);
        }
    }
}
