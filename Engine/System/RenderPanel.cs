﻿using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security.Permissions;
using CustomEngine.Input;
using CustomEngine.Rendering;
using CustomEngine.Rendering.HUD;
using CustomEngine.Rendering.DirectX;
using CustomEngine.Rendering.OpenGL;
using System.Drawing.Imaging;

namespace CustomEngine
{
    public static class ControlExtension
    {
        [ReflectionPermission(SecurityAction.Demand, MemberAccess = true)]
        public static void Reset(this Control c)
        {
            typeof(Control).InvokeMember("SetState", BindingFlags.NonPublic |
            BindingFlags.InvokeMethod | BindingFlags.Instance, null,
            c, new object[] { 0x400000, false });
        }
    }
    /// <summary>
    /// Used for rendering using any rasterizer that inherits from AbstractRenderer.
    /// </summary>
    public class RenderPanel : UserControl, IEnumerable<Viewport>
    {
        private delegate void DelOnRender(PaintEventArgs e);
        private DelOnRender OnRender;

        public static RenderPanel HoveredPanel;
        public RenderPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque |
                ControlStyles.ResizeRedraw,
                true);

            if (Engine.Settings.ShadingStyle == ShadingStyle.Deferred)
            {
                _gBuffer = new GBuffer(Width, Height);
                OnRender = OnRenderDeferred;
            }
            else
            {
                _gBuffer = null;
                OnRender = OnRenderForward;
            }

            PointToClientDelegate = new DelPointConvert(PointToClient);
            PointToScreenDelegate = new DelPointConvert(PointToScreen);
            CreateContext();
            AddViewport(new LocalPlayerController());
        }
        protected override void Dispose(bool disposing)
        {
            DisposeContext();
            base.Dispose(disposing);
        }

        public new Point PointToClient(Point p)
        {
            p = base.PointToClient(p);
            p.Y = Height - p.Y;
            return p;
        }
        public new Point PointToScreen(Point p)
        {
            p.Y = Height - p.Y;
            return base.PointToScreen(p);
        }
        
        internal delegate Point DelPointConvert(Point p);
        internal DelPointConvert PointToClientDelegate;
        internal DelPointConvert PointToScreenDelegate;

        private bool _hasAnyForward = true;
        private GBuffer _gBuffer;
        internal RenderContext _context;
        protected int _updateCounter;
        private HudManager _globalHud;
        public List<Viewport> _viewports = new List<Viewport>();
        private ColorF4 _backColor = Color.Lavender;
        private bool _attachedToEngine = false;

        public HudManager GlobalHud
        {
            get => _globalHud;
            set => _globalHud = value;
        }
        public new ColorF4 BackColor
        {
            get => _backColor;
            set => _backColor = value;
        }
        
        /// <summary>
        /// Disables rendering until PopUpdate is called, unless there are other update calls on the stack.
        /// </summary>
        public void PushUpdate()
        {
            ++_updateCounter;
        }
        /// <summary>
        /// Ends the last PushUpdate call. Rendering may not resume unless the update stack is empty.
        /// </summary>
        public void PopUpdate()
        {
            if ((_updateCounter = Math.Max(_updateCounter - 1, 0)) == 0)
                Invalidate();
        }
        public void CaptureContext()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CaptureContext));
                return;
            }
            _context?.Capture();
        }
        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_updateCounter > 0)
                return;

            if (_context == null || _context.IsContextDisposed())
                base.OnPaint(e);
            else if (Monitor.TryEnter(_context))
            {
                try
                {
                    _context.Capture();
                    OnRender(e);
                    _context.Swap();
                    //_context.ErrorCheck();
                    //_context.Flush();
                }
                finally { Monitor.Exit(_context); }
            }
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (HoveredPanel != this)
                HoveredPanel = this;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (HoveredPanel == this)
                HoveredPanel = null;
        }
        protected virtual void OnRenderDeferred(PaintEventArgs e)
        {
            _gBuffer.Bind(FramebufferType.ReadWrite);
            _context.BeginDraw();

            foreach (Viewport v in _viewports)
                v.RenderDeferred(Engine.Renderer.Scene);

            //Write to default frame buffer
            Engine.Renderer.BindFrameBuffer(FramebufferType.ReadWrite, 0);
            Engine.Renderer.Clear(BufferClear.Color);
            _gBuffer.Render();

            if (_hasAnyForward)
            {
                //Copy depth from GBuffer to main frame buffer
                Engine.Renderer.BlitFrameBuffer(
                    _gBuffer.BindingId, 0,
                    0, 0, Width, Height, 0, 0, Width, Height, 
                    AbstractRenderer.EClearBufferMask.DepthBufferBit, 
                    AbstractRenderer.EBlitFramebufferFilter.Nearest);
                
                foreach (Viewport v in _viewports)
                    v.RenderForward(Engine.Renderer.Scene);
            }

            _globalHud?.Render();
            _context.EndDraw();
        }
        protected virtual void OnRenderForward(PaintEventArgs e)
        {
            _context.BeginDraw();
            foreach (Viewport v in _viewports)
                v.RenderForward(Engine.Renderer.Scene);
            _globalHud?.Render();
            _context.EndDraw();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _gBuffer?.Resize(Width, Height);
            foreach (Viewport v in _viewports)
                v.Resize(Width, Height);
            //Rectangle region = new Rectangle(0, 0, Width, Height);
            //Engine.Renderer.PopRenderArea();
            //Engine.Renderer.PushRenderArea(region);
            //Engine.Renderer.CropRenderArea(region);
        }
        protected virtual void OnReset(object sender, EventArgs e)
        {
            _context?.Initialize();
        }
        protected virtual void OnContextChanged(bool isNowCurrent)
        {
            //Don't update anything if this context has just been released
            if (isNowCurrent)
                OnResize(EventArgs.Empty);

            //_currentPanel = isNowCurrent ? this : null;
        }
        public void CreateContext()
        {
            switch (Engine.RenderLibrary)
            {
                case RenderLibrary.OpenGL:
                    if (_context is GLWindowContext)
                        return;
                    _context?.Dispose();
                    _context = new GLWindowContext(this);
                    break;
                case RenderLibrary.Direct3D11:
                    if (_context is DXWindowContext)
                        return;
                    _context?.Dispose();
                    _context = new DXWindowContext(this);
                    break;
                default:
                    return;
            }
            if (_context != null)
            {
                _context.ContextChanged += OnContextChanged;
                _context.ResetOccured += OnReset;
                _context.Capture(true);
                _context.Initialize();
            }
        }
        private void DisposeContext()
        {
            if (_context != null)
            {
                Resize -= _context.OnResized;
                _context.Unbind();
                _context.Dispose();
                _context = null;
            }
        }
        public void AddViewport(LocalPlayerController owner)
        {
            _viewports.Add(new Viewport(owner, this, _viewports.Count));
            for (int i = 0; i < _viewports.Count; ++i)
                _viewports[i].ViewportCountChanged(i, _viewports.Count, Engine.TwoPlayerPref, Engine.ThreePlayerPref);
        }
        public Viewport GetViewport(int index)
            => index >= 0 && index < _viewports.Count ? _viewports[index] : null;
        public void RemoveViewport(LocalPlayerController owner)
        {
            if (_viewports.Contains(owner.Viewport))
            {
                _viewports.Remove(owner.Viewport);
                for (int i = 0; i < _viewports.Count; ++i)
                    _viewports[i].ViewportCountChanged(i, _viewports.Count, Engine.TwoPlayerPref, Engine.ThreePlayerPref);
            }
        }

        public void BeginTick()
        {
            _attachedToEngine = true;
            Engine.RegisterRenderTick(RenderTick);
        }
        public void EndTick()
        {
            Engine.UnregisterRenderTick(RenderTick);
            _attachedToEngine = false;
        }
        public void RenderTick(object sender, FrameEventArgs e)
        {
            Invalidate();
        }

        public IEnumerator<Viewport> GetEnumerator()
            => ((IEnumerable<Viewport>)_viewports).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<Viewport>)_viewports).GetEnumerator();
    }
}
