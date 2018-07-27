﻿using System;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Rendering.Models.Materials;
using System.Drawing;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Shapes;
using System.ComponentModel;

namespace TheraEngine.Components.Scene
{
    public class DecalComponent : BoxComponent, IPreRendered
    {
        public TMaterial Material { get; set; }
        
        internal Matrix4 DecalRenderMatrix { get; private set; }
        internal Matrix4 InverseDecalRenderMatrix { get; private set; }
        private FrameBuffer RenderFBO { get; set; }
        private QuadFrameBuffer QuadFBO { get; set; }
        internal TexRef2D AlbedoOpacity { get; private set; }
        internal TexRef2D Normal { get; private set; }
        internal TexRef2D RMSI { get; private set; }
        private BoundingRectangle ViewRect { get; set; }
        [Category("Decal")]
        public bool PreRenderEnabled { get; set; } = true;

        public DecalComponent() : base() { }
        public DecalComponent(Vec3 halfExtents) : base(halfExtents, null) { }

        [Category("Decal")]
        public override Vec3 HalfExtents
        {
            get => base.HalfExtents;
            set
            {
                base.HalfExtents = value;
                Vec3 halfExtents = Box.HalfExtents.Raw;
                DecalRenderMatrix = WorldMatrix * halfExtents.AsScaleMatrix();
                InverseDecalRenderMatrix = (1.0f / halfExtents).AsScaleMatrix() * InverseWorldMatrix;
            }
        }

        protected override void OnWorldTransformChanged()
        {
            Vec3 halfExtents = Box.HalfExtents.Raw;
            DecalRenderMatrix = WorldMatrix * halfExtents.AsScaleMatrix();
            InverseDecalRenderMatrix = (1.0f / halfExtents).AsScaleMatrix() * InverseWorldMatrix;
            base.OnWorldTransformChanged();
        }
        public void Initialize(int width, int height)
        {
            ViewRect = new BoundingRectangle(0, 0, width, height);
            QuadFBO = new QuadFrameBuffer(Material);
            AlbedoOpacity = TexRef2D.CreateFrameBufferTexture("DecalAlbedoOpacity", width, height,
                EPixelInternalFormat.Rgba8, EPixelFormat.Rgba, EPixelType.UnsignedByte);
            Normal = TexRef2D.CreateFrameBufferTexture("DecalNormal", width, height,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat);
            RMSI = TexRef2D.CreateFrameBufferTexture("DecalRMSI", width, height,
                EPixelInternalFormat.Rgba8, EPixelFormat.Rgba, EPixelType.UnsignedByte);
            RenderFBO = new FrameBuffer();
            RenderFBO.SetRenderTargets(
                (AlbedoOpacity, EFramebufferAttachment.ColorAttachment0, 0, -1),
                (Normal, EFramebufferAttachment.ColorAttachment1, 0, -1),
                (RMSI, EFramebufferAttachment.ColorAttachment2, 0, -1));
        }
        public override void OnSpawned()
        {
            Material = TMaterial.CreateLitColorMaterial(Color.Orange, true);
            Initialize(128, 128);
            OwningScene.Decals.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            OwningScene.Decals.Add(this);
            base.OnDespawned();
        }

        public void PreRenderUpdate(Camera camera)
        {

        }
        public void PreRenderSwap()
        {

        }
        public void PreRender(Viewport viewport, Camera camera)
        {
            RenderFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.PushRenderArea(ViewRect);
            Engine.Renderer.Clear(EFBOTextureType.Color);
            QuadFBO.RenderFullscreen();
            Engine.Renderer.PopRenderArea();
            RenderFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
    }
}
