using System;
using System.Threading;
//using SharpDX.Direct3D;
using SharpDX.DXGI;
using DX12 = SharpDX.Direct3D12;
//using Device = SharpDX.Direct3D12.Device;
//using SharpDX.Direct3D11;
using System.Drawing;
using SharpDX.Mathematics.Interop;
using SharpDX.Direct3D12;
//using SharpDX.Direct3D12;

namespace TheraEngine.Rendering.DirectX
{
    public class DXWindowContext : RenderContext
    {
        static DXWindowContext()
        {
            
        }

        private DXRenderer _renderer;

        //TODO: pass DXThreadSubContext as generic into RenderContext; use IRenderContext throughout engine
        public DX12.Device Device => ((DXThreadSubContext)_currentSubContext).Device;
        public SwapChain SwapChain => ((DXThreadSubContext)_currentSubContext).SwapChain;
        //public Texture2D TextureTarget => ((DXThreadSubContext)_currentSubContext).TextureTarget;
        //public RenderTargetView TargetView => ((DXThreadSubContext)_currentSubContext).TargetView;

        protected class DXThreadSubContext : ThreadSubContext
        {
            private VSyncMode _vsyncMode = VSyncMode.Adaptive;

            public const int FrameCount = 2;

            public int FrameIndex { get; private set; }
            public DX12.Device Device { get; private set; }
            public SwapChain3 SwapChain { get; private set; }
            public CommandQueue CommandQueue { get; private set; }
            public CommandAllocator CommandAllocator { get; private set; }
            public RootSignature RootSignature { get; private set; }
            public DescriptorHeap RenderTargetViewHeap { get; private set; }
            public PipelineState PipelineState { get; private set; }
            public GraphicsCommandList CommandList { get; private set; }
            public int RtvDescriptorSize { get; private set; }

            private readonly DX12.Resource[] RenderTargets = new DX12.Resource[FrameCount];

            public DXThreadSubContext(IntPtr controlHandle, Thread thread)
                : base(controlHandle, thread) { }

            public override void Generate()
            {
                //ModeDescription modeDesc = new ModeDescription(0, 0, new Rational(60, 1), Format.R16G16B16A16_Float);
                //SampleDescription smplDesc = new SampleDescription(1, 0);
                //SwapChainDescription swapChainDesc = new SwapChainDescription()
                //{
                //    BufferCount = 1,
                //    ModeDescription = modeDesc,
                //    SampleDescription = smplDesc,
                //    Usage = Usage.RenderTargetOutput,
                //    SwapEffect = SwapEffect.Discard,
                //    OutputHandle = _controlHandle,
                //    Flags = SwapChainFlags.None,
                //    IsWindowed = true,
                //};

                //DeviceCreationFlags flags = DeviceCreationFlags.Debug | DeviceCreationFlags.Debuggable;
                //Device.CreateWithSwapChain(DriverType.Hardware, flags, swapChainDesc, out Device d, out SwapChain sc);

                //Device = d;
                //SwapChain = sc;

                //TextureTarget = SwapChain.GetBackBuffer<Texture2D>(0);
                //TargetView = new RenderTargetView(Device, TextureTarget);

#if DEBUG
                // Enable the D3D12 debug layer.
                DebugInterface.Get().EnableDebugLayer();
#endif
                
                Device = new DX12.Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
                using (var factory = new Factory4())
                {
                    // Describe and create the command queue.
                    var queueDesc = new CommandQueueDescription(CommandListType.Direct);
                    CommandQueue = Device.CreateCommandQueue(queueDesc);

                    // Describe and create the swap chain.
                    var swapChainDesc = new SwapChainDescription()
                    {
                        BufferCount = FrameCount,
                        ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                        Usage = Usage.RenderTargetOutput,
                        SwapEffect = SwapEffect.FlipDiscard,
                        OutputHandle = _controlHandle,
                        //Flags = SwapChainFlags.None,
                        SampleDescription = new SampleDescription(1, 0),
                        IsWindowed = true
                    };

                    var tempSwapChain = new SwapChain(factory, CommandQueue, swapChainDesc);
                    SwapChain = tempSwapChain.QueryInterface<SwapChain3>();
                    tempSwapChain.Dispose();
                    FrameIndex = SwapChain.CurrentBackBufferIndex;
                }

                // Create descriptor heaps.
                // Describe and create a render target view (RTV) descriptor heap.
                var rtvHeapDesc = new DescriptorHeapDescription()
                {
                    DescriptorCount = FrameCount,
                    Flags = DescriptorHeapFlags.None,
                    Type = DescriptorHeapType.RenderTargetView
                };
                DescriptorHeap renderTargetViewHeap = Device.CreateDescriptorHeap(rtvHeapDesc);
                RtvDescriptorSize = Device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

                // Create frame resources.
                var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
                for (int n = 0; n < 2; ++n)
                {
                    RenderTargets[n] = SwapChain.GetBackBuffer<DX12.Resource>(n);
                    Device.CreateRenderTargetView(RenderTargets[n], null, rtvHandle);
                    rtvHandle += RtvDescriptorSize;
                }

                CommandAllocator = Device.CreateCommandAllocator(CommandListType.Direct);

                var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout);
                RootSignature = Device.CreateRootSignature(rootSignatureDesc.Serialize());
                
                var psoDesc = new GraphicsPipelineStateDescription()
                {
                    InputLayout = new InputLayoutDescription(new InputElement[0]),
                    RootSignature = RootSignature,
                    VertexShader = null,
                    PixelShader = null,
                    RasterizerState = RasterizerStateDescription.Default(),
                    BlendState = BlendStateDescription.Default(),
                    DepthStencilFormat = Format.D32_Float,
                    DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
                    SampleMask = int.MaxValue,
                    PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                    RenderTargetCount = 1,
                    Flags = PipelineStateFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                    StreamOutput = new StreamOutputDescription()
                };
                psoDesc.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;

                PipelineState = Device.CreateGraphicsPipelineState(psoDesc);

                // Create the command list.
                CommandList = Device.CreateCommandList(CommandListType.Direct, CommandAllocator, PipelineState);
            }

            public void BeginPopulateCommandList()
            {
                // Command list allocators can only be reset when the associated 
                // command lists have finished execution on the GPU; apps should use 
                // fences to determine GPU execution progress.
                CommandAllocator.Reset();

                // However, when ExecuteCommandList() is called on a particular command 
                // list, that command list can then be reset at any time and must be before 
                // re-recording.
                CommandList.Reset(CommandAllocator, PipelineState);

                // Set necessary state.
                CommandList.SetGraphicsRootSignature(RootSignature);
                //CommandList.SetViewport(viewport);
                //CommandList.SetScissorRectangles(scissorRect);

                // Indicate that the back buffer will be used as a render target.
                CommandList.ResourceBarrierTransition(RenderTargets[FrameIndex], ResourceStates.Present, ResourceStates.RenderTarget);

                var rtvHandle = RenderTargetViewHeap.CPUDescriptorHandleForHeapStart;
                rtvHandle += FrameIndex * RtvDescriptorSize;
                CommandList.SetRenderTargets(rtvHandle, null);

                // Record commands.
                CommandList.ClearRenderTargetView(rtvHandle, new RawColor4(0, 0.2F, 0.4f, 1), 0, null);

                //CommandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
                //CommandList.SetVertexBuffer(0, vertexBufferView);
                //CommandList.DrawInstanced(3, 1, 0, 0);

                // Indicate that the back buffer will now be used to present.
                CommandList.ResourceBarrierTransition(RenderTargets[FrameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

                CommandList.Close();
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
                Device?.Dispose();
                SwapChain?.Dispose();
                //TextureTarget?.Dispose();
                //TargetView?.Dispose();
            }

            public override bool IsContextDisposed()
                => Device == null || Device.IsDisposed;

            public override bool IsCurrent()
            {
                if (!IsContextDisposed())
                    return false;
                return false;
            }

            public override void OnSwapBuffers()
            {
                SwapChain.Present(1, PresentFlags.None);
                FrameIndex = SwapChain.CurrentBackBufferIndex;
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
        protected override ThreadSubContext CreateSubContext(IntPtr handle, Thread thread)
            => new DXThreadSubContext(handle, thread);
        
        public DXWindowContext(BaseRenderPanel c) : base(c) { }

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