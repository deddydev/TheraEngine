using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms
{
    public partial class MaterialPreviewControl : UserControl
    {
        private float _cameraFovY = 45.0f;
        public float CameraFovY
        {
            get => _cameraFovY;
            set
            {
                _cameraFovY = value;
                if (basicRenderPanel1.Camera is PerspectiveCamera cam)
                {
                    cam.VerticalFieldOfView = _cameraFovY;
                    float camDist = 1.0f / TMath.Tandf(_cameraFovY * 0.5f);
                    cam.LocalPoint.Z = camDist;
                }
            }
        }

        public MaterialPreviewControl()
        {
            InitializeComponent();
            basicRenderPanel1.MouseDown += BasicRenderPanel1_MouseDown;
            basicRenderPanel1.MouseUp += BasicRenderPanel1_MouseUp;
            basicRenderPanel1.MouseMove += BasicRenderPanel1_MouseMove;
            //pnlMatInfo.MouseEnter += PnlMatInfo_MouseEnter;
            //pnlMatInfo.MouseLeave += PnlMatInfo_MouseLeave;
            //pnlMatInfo.MouseDown += PnlMatInfo_MouseDown;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            float camDist = 1.0f / TMath.Tandf(_cameraFovY * 0.5f);
            PerspectiveCamera c = new PerspectiveCamera(
                new Vec3(0.0f, 0.0f, camDist), Rotator.GetZero(), 0.1f, 100.0f, _cameraFovY, 1.0f);
            c.PostProcessRef.File.ColorGrading.AutoExposure = true;
            c.PostProcessRef.File.ColorGrading.Exposure = 1.0f;
            basicRenderPanel1.Camera = c;
            if (_light == null)
            {
                _light = new DirectionalLightComponent();
                _light.SetShadowMapResolution(128, 128);
                _light.DiffuseIntensity = 2000.0f;
                _light.LightColor = (ColorF3)Color.White;
                _light.Scale = 1.0f;
                _light.Rotation.Yaw = 0.0f;
                _light.Rotation.Pitch = 0.0f;
                basicRenderPanel1.Scene.Lights.Add(_light);
                basicRenderPanel1.PreRendered += BasicRenderPanel1_PreRendered;
                basicRenderPanel1.RegisterTick();
                RedrawPreview();
            }
        }

        private int _prevX, _prevY;
        private bool _dragging = false;
        private void BasicRenderPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                int dx = e.X - _prevX;
                int dy = e.Y - _prevY;
                _prevX = e.X;
                _prevY = e.Y;
                
                if (dx == 0 && dy == 0)
                    return;

                //up is negative, left is negative

                Quat lightRotation = _light.Rotation.GetMatrix().ExtractRotation(false);

                Vec2 vec = new Vec2(dx, dy);
                Vec3 axis = Vec3.Zero;

                if (dx == 0)
                {
                    if (dy < 0)
                        axis = -Vec3.UnitX;
                    else
                        axis = Vec3.UnitX;
                }
                else if (dy == 0)
                {
                    if (dx < 0)
                        axis = -Vec3.UnitY;
                    else
                        axis = Vec3.UnitY;
                }
                //else
                //{
                //    axis = new Vec3(1.0f / vec, 0.0f).Normalized();
                //}
                float vecLen = vec.Length;
                Quat rot = Quat.FromAxisAngleDeg(axis, vecLen);
                lightRotation = rot * lightRotation;
                Rotator r = lightRotation.ToYawPitchRoll();
                _light.Rotation.SetRotations(r);

                RedrawPreview();
            }
        }

        private void BasicRenderPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void BasicRenderPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            _prevX = e.X;
            _prevY = e.Y;
            _dragging = true;
        }
        
        private void BasicRenderPanel1_PreRendered()
        {
            _light.RenderShadowMap(basicRenderPanel1.Scene);
        }
        
        private DirectionalLightComponent _light;
        private MeshRenderable _spherePrim;
        private TMaterial _material;

        public TMaterial Material
        {
            get => _material;
            set
            {
                _material = value;

                if (Engine.DesignMode)
                    return;
                
                if (_material != null)
                {
                    if (_spherePrim == null)
                    {
                        basicRenderPanel1.Scene.Clear(BoundingBox.FromHalfExtentsTranslation(5.0f, 0.0f));
                        basicRenderPanel1.Scene.Lights.Add(_light);
                        _spherePrim = new MeshRenderable( //0.8f instead of 1.0f for border padding
                            new PrimitiveManager(Sphere.SolidMesh(Vec3.Zero, 0.8f, 30), _material));
                        basicRenderPanel1.Scene.Add(_spherePrim);

                        IBLProbeGridActor probes = new IBLProbeGridActor();
                        probes.AddProbe(Vec3.Zero);
                        //probes.SetFrequencies(BoundingBox.FromHalfExtentsTranslation(100.0f, Vec3.Zero), new Vec3(0.02f));
                        probes.SceneComponentCache.ForEach(x => x.OwningScene = basicRenderPanel1.Scene);
                        probes.InitAndCaptureAll(128);
                        basicRenderPanel1.Scene.IBLProbeActor = probes;
                    }
                    else
                        _spherePrim.Material = _material;
                }

                RedrawPreview();
            }
        }

        private void RedrawPreview()
        {
            //basicRenderPanel1.UpdateTick(null, null);
            //basicRenderPanel1.SwapBuffers();
            //basicRenderPanel1.Invalidate();
        }
    }
}
