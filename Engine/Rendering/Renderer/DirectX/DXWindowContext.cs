using System;
using System.Threading;
using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;

namespace TheraEngine.Rendering.DirectX
{
    public class DXWindowContext : RenderContext
    {
        static DXWindowContext()
        {
            
        }

        private DXRenderer _renderer;

        protected class DXThreadSubContext : ThreadSubContext
        {
            public const int FrameCount = 2;

            private VSyncMode _vsyncMode = VSyncMode.Adaptive;
            private SharpDX.Direct3D12.Device _device;
            private CommandQueue _commandQueue;
            private SwapChain3 _swapChain;
            private int _frameIndex;
#if DEBUG
            private static bool _hasPrintedInfo = false;
#endif
            
            public DXThreadSubContext(IntPtr controlHandle, Thread thread)
                : base(controlHandle, thread) { }

            public override void Generate()
            {
#if DEBUG
                DebugInterface.Get().EnableDebugLayer();
#endif
                _device = new SharpDX.Direct3D12.Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
                using (var factory = new Factory4())
                {
                    // Describe and create the command queue.
                    CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
                    _commandQueue = _device.CreateCommandQueue(queueDesc);

                    // Describe and create the swap chain.
                    SwapChainDescription swapChainDesc = new SwapChainDescription()
                    {
                        BufferCount = FrameCount,
                        ModeDescription = new ModeDescription(1, 1, new Rational(60, 1), Format.R16G16B16A16_Float),
                        Usage = Usage.RenderTargetOutput,
                        SwapEffect = SwapEffect.FlipDiscard,
                        OutputHandle = _controlHandle,
                        //Flags = SwapChainFlags.None,
                        SampleDescription = new SampleDescription(1, 0),
                        IsWindowed = true
                    };

                    SwapChain tempSwapChain = new SwapChain(factory, _commandQueue, swapChainDesc);
                    _swapChain = tempSwapChain.QueryInterface<SwapChain3>();
                    tempSwapChain.Dispose();
                    _frameIndex = _swapChain.CurrentBackBufferIndex;
                }

                // Create descriptor heaps.
                // Describe and create a render target view (RTV) descriptor heap.
                //DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription()
                //{
                //    DescriptorCount = FrameCount,
                //    Flags = DescriptorHeapFlags.None,
                //    Type = DescriptorHeapType.RenderTargetView
                //};

                //renderTargetViewHeap = _device.CreateDescriptorHeap(rtvHeapDesc);

                //rtvDescriptorSize = _device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

                //// Create frame resources.
                //CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
                //for (int n = 0; n < FrameCount; n++)
                //{
                //    renderTargets[n] = _swapChain.GetBackBuffer<Resource>(n);
                //    _device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
                //    rtvHandle += rtvDescriptorSize;
                //}

                //commandAllocator = _device.CreateCommandAllocator(CommandListType.Direct);
            }

            internal override void VsyncChanged(VSyncMode vsyncMode)
            {
                _vsyncMode = vsyncMode;

                //if (_context == null)
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

            }

            public override bool IsContextDisposed()
                => false;

            public override bool IsCurrent()
            {
                if (!IsContextDisposed())
                    return false;
                return false;
            }

            public override void OnSwapBuffers()
            {
                //try
                //{
                //    if (!IsContextDisposed())
                //        _context.SwapBuffers();
                //}
                //catch (Exception ex)
                //{
                //    Engine.LogException(ex);
                //}
            }

            public override void OnResized(Vec2 size)
            {
                //_context?.Update(WindowInfo);
            }

            public override void SetCurrent(bool current)
            {
                //try
                //{
                //    if (!IsContextDisposed() && IsCurrent() != current)
                //        _context.MakeCurrent(current ? WindowInfo : null);
                //}
                //catch (Exception ex)
                //{
                //    Engine.LogException(ex);
                //}
            }
        }
        protected override ThreadSubContext CreateSubContext(Thread thread)
        {
            IntPtr handle;
            if (_control.InvokeRequired)
                handle = (IntPtr)_control.Invoke(new Func<IntPtr>(() => _control.Handle));
            else
                handle = _control.Handle;
            return new DXThreadSubContext(handle, thread);
        }

        public DXWindowContext(BaseRenderPanel c) : base(c)
        {

        }
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

        internal override AbstractRenderer GetRendererInstance()
            => _renderer ?? (_renderer = new DXRenderer());

        public override void ErrorCheck()
        {
            GetCurrentSubContext();
            //ErrorCode code = GL.GetError();
            //if (code != ErrorCode.NoError && _control != null)
            //    _control.Reset();
        }

        public unsafe override void Initialize()
        {
            GetCurrentSubContext();
            
            //Set up constant rendering parameters
        }
        public unsafe override void BeginDraw()
        {
            GetCurrentSubContext();


        }
        public override void EndDraw()
        {

        }
        public override void Flush()
        {
            GetCurrentSubContext();
            //GL.Flush();
        }
    }
}