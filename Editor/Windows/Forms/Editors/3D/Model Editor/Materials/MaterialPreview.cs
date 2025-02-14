﻿using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds;

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
            //    if (basicRenderPanel1.Camera is PerspectiveCamera cam)
            //    {
            //        cam.VerticalFieldOfView = _cameraFovY;
            //        float camDist = 1.0f / TMath.Tandf(_cameraFovY * 0.5f);
            //        cam.LocalPoint.Z = camDist;
            //    }
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

            PerspectiveCamera cam = new PerspectiveCamera(
                new Vec3(0.0f, 0.0f, camDist), Rotator.GetZero(), 0.1f, 100.0f, _cameraFovY, 1.0f);

            cam.PostProcessRef.File.ColorGrading.AutoExposure = false;
            cam.PostProcessRef.File.ColorGrading.Exposure = 1.0f;

            //basicRenderPanel1.Camera = cam;

            if (_light is null)
            {
                _light = new DirectionalLightComponent();
                _light.SetShadowMapResolution(128, 128);
                _light.DiffuseIntensity = 2000.0f;
                _light.LightColor = (ColorF3)Color.White;
                _light.Scale = 1.0f;
                _light.Rotation = Quat.Identity;
                //_light.CastsShadows = false;
            }

            WorldSettings settings = new WorldSettings
            {
                Bounds = BoundingBox.FromHalfExtentsTranslation(5.0f, 0.0f),

            };
            //basicRenderPanel1.World.Settings = settings;
            //basicRenderPanel1.World.BeginPlay();

            //basicRenderPanel1.RegisterTick();
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

                _light.Rotation *= Quat.FromAxisAngleDeg(axis, vec.Length);
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
        
        private DirectionalLightComponent _light;
        private DirectMesh _spherePrim;
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
                    if (_spherePrim is null)
                    {
                        //basicRenderPanel1.World.Clear(BoundingBoxStruct.FromHalfExtentsTranslation(5.0f, 0.0f));
                        //basicRenderPanel1.World.Lights.Add(_light);
                        //_spherePrim = new MeshRenderable( //0.8f instead of 1.0f for border padding
                        //    new PrimitiveManager(Sphere.SolidMesh(Vec3.Zero, 0.8f, 30), _material));
                        //basicRenderPanel1.World.Add(_spherePrim);

                        //IBLProbeGridActor probes = new IBLProbeGridActor();
                        //probes.AddProbe(Vec3.Zero);
                        ////probes.SetFrequencies(BoundingBox.FromHalfExtentsTranslation(100.0f, Vec3.Zero), new Vec3(0.02f));
                        //probes.OwningScene = basicRenderPanel1.World;
                        //probes.InitAndCaptureAll(128);
                        //basicRenderPanel1.World.IBLProbeActor = probes;
                    }
                    else
                        _spherePrim.Material = _material;
                }
            }
        }
    }
}
