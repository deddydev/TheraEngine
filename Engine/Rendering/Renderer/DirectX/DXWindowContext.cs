using System;
using System.Threading;
using SharpDX.DXGI;
using DX11 = SharpDX.Direct3D11;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;

namespace TheraEngine.Rendering.DirectX
{
    public class DXWindowContext : RenderContext
    {
        static DXWindowContext()
        {
            
        }

        public override AbstractRenderer Renderer { get; } = new DXRenderer();

        //TODO: pass DXThreadSubContext as generic into RenderContext; use IRenderContext throughout engine
        public DX11.Device Device => ((DXThreadSubContext)_currentSubContext).Device;
        public SwapChain SwapChain => ((DXThreadSubContext)_currentSubContext).SwapChain;
        public RasterizerState RasterState => ((DXThreadSubContext)_currentSubContext).RasterState;
        public DeviceContext DeviceContext => ((DXThreadSubContext)_currentSubContext).DeviceContext;
        public Texture2D DepthStencilBuffer => ((DXThreadSubContext)_currentSubContext).DepthStencilBuffer;
        public RenderTargetView RenderTargetView => ((DXThreadSubContext)_currentSubContext).RenderTargetView;
        public DepthStencilState DepthStencilState => ((DXThreadSubContext)_currentSubContext).DepthStencilState;
        public DepthStencilView DepthStencilView => ((DXThreadSubContext)_currentSubContext).DepthStencilView;

        protected class DXThreadSubContext : ThreadSubContext
        {
            private EVSyncMode _vsyncMode = EVSyncMode.Adaptive;
            
            public RasterizerState RasterState { get; private set; }
            public DX11.Device Device { get; private set; }
            public DeviceContext DeviceContext { get; private set; }
            public SwapChain SwapChain { get; private set; }
            public Texture2D DepthStencilBuffer { get; private set; }
            public RenderTargetView RenderTargetView { get; private set; }
            public DepthStencilState DepthStencilState { get; private set; }
            public DepthStencilView DepthStencilView { get; private set; }

            public DXThreadSubContext(IntPtr controlHandle, Thread thread)
                : base(controlHandle, thread) { }

            public override void Generate()
            {
                var swapChainDesc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(Size.X, Size.Y, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    SwapEffect = SwapEffect.Discard,
                    OutputHandle = _controlHandle,
                    SampleDescription = new SampleDescription(1, 0),
                    IsWindowed = true
                };

                DX11.Device.CreateWithSwapChain(
                    DriverType.Hardware,
                    DeviceCreationFlags.None, 
                    swapChainDesc,
                    out DX11.Device device,
                    out SwapChain swapChain);

                Device = device;
                SwapChain = swapChain;
                DeviceContext = device.ImmediateContext;

                var backBuffer = DX11.Resource.FromSwapChain<Texture2D>(SwapChain, 0);
                RenderTargetView = new RenderTargetView(device, backBuffer);
                backBuffer.Dispose();

                // Initialize and set up the description of the depth buffer.
                var depthBufferDesc = new Texture2DDescription()
                {
                    Width = Size.X,
                    Height = Size.Y,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.D24_UNorm_S8_UInt,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                };
                DepthStencilBuffer = new Texture2D(device, depthBufferDesc);
                
                var depthStencilDesc = new DepthStencilStateDescription()
                {
                    IsDepthEnabled = true,
                    DepthWriteMask = DepthWriteMask.All,
                    DepthComparison = Comparison.Less,
                    IsStencilEnabled = true,
                    StencilReadMask = 0xFF,
                    StencilWriteMask = 0xFF,
                    FrontFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Increment,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    },
                    BackFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Decrement,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    }
                };
                DepthStencilState = new DepthStencilState(Device, depthStencilDesc);
                DeviceContext.OutputMerger.SetDepthStencilState(DepthStencilState, 1);
                
                var depthStencilViewDesc = new DepthStencilViewDescription()
                {
                    Format = Format.D24_UNorm_S8_UInt,
                    Dimension = DepthStencilViewDimension.Texture2D,
                    Texture2D = new DepthStencilViewDescription.Texture2DResource()
                    {
                        MipSlice = 0
                    }
                };
                DepthStencilView = new DepthStencilView(Device, DepthStencilBuffer, depthStencilViewDesc);
                DeviceContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);
                
                var rasterDesc = new RasterizerStateDescription()
                {
                    IsAntialiasedLineEnabled = false,
                    CullMode = CullMode.Back,
                    DepthBias = 0,
                    DepthBiasClamp = 0.0f,
                    IsDepthClipEnabled = true,
                    FillMode = FillMode.Solid,
                    IsFrontCounterClockwise = false,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = 0.0f
                };
                RasterState = new RasterizerState(Device, rasterDesc);

                DeviceContext.Rasterizer.State = RasterState;
                DeviceContext.Rasterizer.SetViewport(0, 0, Size.X, Size.Y, 0, 1);
            }
            
            internal override void VsyncChanged(EVSyncMode vsyncMode)
            {
                _vsyncMode = vsyncMode;

                //if (_context is null)
                //    return;
                //switch (vsyncMode)
                //{
                //    case VSyncMode.Disabled:
                //        _context.SwapInterval = 0;
                //        break;
                //    case VSyncMode.Enabled:
                //        _context.SwapInterval = 1;
                //        break;
                //    case VSyncMode.Adaptive:
                //        _context.SwapInterval = -1;
                //        break;
                //}
            }

            public override void Dispose()
            {
                Device?.Dispose();
                SwapChain?.Dispose();
                //TextureTarget?.Dispose();
                //TargetView?.Dispose();
            }

            public override bool IsContextDisposed()
                => Device is null || Device.IsDisposed;

            public override bool IsCurrent()
            {
                if (!IsContextDisposed())
                    return false;
                return false;
            }

            public override void OnSwapBuffers()
            {
                SwapChain.Present(1, PresentFlags.None);
                //FrameIndex = SwapChain.CurrentBackBufferIndex;
            }

            public override void OnResized(IVec2 size)
            {
                base.OnResized(size);
                //_context?.Update(WindowInfo);
            }

            public override void SetCurrent(bool current)
            {
                //try
                //{
                //    if (!IsContextDisposed() && IsCurrent() != current)
                //    {

                //    }
                //}
                //catch (Exception ex)
                //{
                //    Engine.LogException(ex);
                //}
            }
        }
        protected override ThreadSubContext CreateSubContext(IntPtr handle, Thread thread)
            => new DXThreadSubContext(handle, thread);
        
        public DXWindowContext(IntPtr handle) : base(handle) { }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (ThreadSubContext c in _subContexts.Values)
                        c.Dispose();
                    _subContexts.Clear();
                    _currentSubContext = null;
                }
                _disposedValue = true;
            }
        }

        public override void ErrorCheck()
        {
            GetCurrentSubContext();
            //ErrorCode code = GL.GetError();
            //if (code != ErrorCode.NoError && _control != null)
            //    _control.Reset();
        }

        public unsafe override void Initialize()
        {
            IsInitialized = true;
            GetCurrentSubContext();
            
            //Set up constant rendering parameters
        }
        public unsafe override void BeginDraw()
        {
            GetCurrentSubContext();

            //Device.ImmediateContext.OutputMerger.SetRenderTargets(TargetView);
            //Device.ImmediateContext.ClearRenderTargetView(TargetView, new RawColor4(0.2f, 0.4f, 0.5f, 1.0f));
        }
        public override void EndDraw()
        {

        }
        public override void Flush()
        {
            GetCurrentSubContext();
            //GL.Flush();
        }

        internal override void PreRender()
        {

        }

        internal override void PostRender()
        {

        }
    }
}