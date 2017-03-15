using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using CustomEngine.Input;
using CustomEngine.Rendering;
using CustomEngine.Rendering.HUD;
using CustomEngine.Rendering.DirectX;
using CustomEngine.Rendering.OpenGL;
using System.Windows;
using System.Windows.Controls;

namespace CustomEngine
{
    /// <summary>
    /// Used for rendering using any rasterizer that inherits from AbstractRenderer.
    /// </summary>
    public class RenderPanelWPF : UserControl, IEnumerable<Viewport>
    {
        public RenderPanelWPF()
        {
            //if (_context == null)
                CreateContext();
            AddViewport(new LocalPlayerController());
        }

        public static RenderPanel HoveredPanel;

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
                Refresh();
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
            //if (_updateCounter > 0)
            //    return;

            if (_context == null || _context.IsContextDisposed())
                base.OnPaint(e);
            else if (Monitor.TryEnter(_context))
            {
                //PushUpdate();
                try
                {
                    _context.Capture();
                    OnRender(e);
                    _context.Swap();
                    //_context.ErrorCheck();
                    //_context.Flush();
                }
                finally
                {
                    Monitor.Exit(_context);
                    //PopUpdate();
                }
            }
        }
        //protected override void OnMouseEnter(EventArgs e)
        //{
        //    base.OnMouseEnter(e);
        //    if (HoveredPanel != this)
        //        HoveredPanel = this;
        //}
        //protected override void OnMouseLeave(EventArgs e)
        //{
        //    base.OnMouseLeave(e);
        //    if (HoveredPanel == this)
        //        HoveredPanel = null;
        //}
        protected virtual void OnRender(PaintEventArgs e)
        {
            _context.BeginDraw();
            foreach (Viewport v in _viewports)
                v.Render(Engine.Renderer.Scene);
            _globalHud?.Render();
            _context.EndDraw();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (Viewport v in _viewports)
                v.Resize(Width, Height);
            //Rectangle region = new Rectangle(0, 0, Width, Height);
            //Engine.Renderer.PopRenderArea();
            //Engine.Renderer.PushRenderArea(region);
            //Engine.Renderer.CropRenderArea(region);
        }
        //protected override void OnLoad(EventArgs e)
        //{
        //    if (_context == null)
        //        SetRenderLibrary();
        //    base.OnLoad(e);
        //}
        //protected override void OnHandleCreated(EventArgs e)
        //{
        //    if (_context == null)
        //        SetRenderLibrary();
        //    _context.ContextChanged += OnContextChanged;
        //    _context.ResetOccured += OnReset;
        //}
        //protected override void DestroyHandle()
        //{
        //    DisposeContext();
        //    base.DestroyHandle();
        //}
        //protected override void OnHandleDestroyed(EventArgs e)
        //{
        //    DisposeContext();
        //    base.OnHandleDestroyed(e);
        //}
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
        protected override void Dispose(bool disposing)
        {
            DisposeContext();
            base.Dispose(disposing);
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

        public IEnumerator<Viewport> GetEnumerator()
            => ((IEnumerable<Viewport>)_viewports).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<Viewport>)_viewports).GetEnumerator();

        public void AttachToEngine()
        {
            Redraw = new Action(Refresh);
            _attachedToEngine = true;
            _context?.Initialize();
            Engine.RegisterRenderTick(RenderTick);
        }
        public void DetachFromEngine()
        {
            Engine.UnregisterRenderTick(RenderTick);
            DisposeContext();
            _attachedToEngine = false;
        }
        private Action Redraw;
        private double _time = 0.0;
        public void RenderTick(object sender, FrameEventArgs e)
        {
            _time += e.Time;
            //Invalidate();
            if (_time > 0.05)
            {
                BeginInvoke(Redraw);
                _time = 0.0;
            }
            //Thread.Sleep(40);
        }
    }
}
