using System;
using System.Windows.Controls;
//using SlimDX.Direct3D11;
//using SlimDX.DXGI;
//using SlimDX;
//using Device = SlimDX.Direct3D11.Device;
using System.Threading;

namespace TheraEngine.Rendering.DirectX
{
    public class DXWindowContext : RenderContext
    {
        //private Device _device;
        //private PresentFlags _presentParameters;
        //private static SwapChain _swapChain;
        //private static RenderTargetView _renderTarget;
        //private static Texture2D _resource;
        //private static DepthStencilView _depthStencil;

        public DXWindowContext(RenderPanel c) : base(c)
        {

        }

        public void Init()
        {
            //// Create swap chain description
            //var swapChainDesc = new SwapChainDescription()
            //{
            //    BufferCount = 2,
            //    Usage = Usage.RenderTargetOutput,
            //    OutputHandle = _control.Handle,
            //    IsWindowed = true,
            //    ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
            //    SampleDescription = new SampleDescription(1, 0),
            //    Flags = SwapChainFlags.AllowModeSwitch,
            //    SwapEffect = SwapEffect.Discard
            //};

            //_device = Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out _device, out _swapChain);
        }

        internal override AbstractRenderer GetRendererInstance()
        {
            return new DXRenderer();
        }

        public override bool IsCurrent()
        {
            throw new NotImplementedException();
        }

        public override bool IsContextDisposed()
        {
            throw new NotImplementedException();
        }

        public override void SetCurrent(bool current)
        {
            throw new NotImplementedException();
        }

        protected override void OnSwapBuffers()
        {
            //_swapChain.Present(0, PresentFlags.None);
        }

        protected override void OnUpdated()
        {
            throw new NotImplementedException();
        }

        public override void ErrorCheck()
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void BeginDraw()
        {
            //_renderTarget.BeginDraw();
            //_renderTarget.Transform = Matrix3x2.Identity;
            //_renderTarget.Clear(Color.White);
        }

        public override void EndDraw()
        {
            //_renderTarget.EndDraw();
        }

        private void CreateDepthStencilBuffer(int width, int height)
        {
            //Texture2D DSTexture = new Texture2D(
            //    _device,
            //    new Texture2DDescription()
            //    {
            //        ArraySize = 1,
            //        MipLevels = 1,
            //        Format = Format.D32_Float,
            //        Width = width,
            //        Height = height,
            //        BindFlags = BindFlags.DepthStencil,
            //        CpuAccessFlags = CpuAccessFlags.None,
            //        SampleDescription = new SampleDescription(1, 0),
            //        Usage = ResourceUsage.Default
            //    }
            //);

            //_depthStencil = new DepthStencilView(
            //   _device,
            //   DSTexture,
            //   new DepthStencilViewDescription()
            //   {
            //       ArraySize = 0,
            //       FirstArraySlice = 0,
            //       MipSlice = 0,
            //       Format = Format.D32_Float,
            //       Dimension = DepthStencilViewDimension.Texture2D
            //   }
            //);

            ////_context.OutputMerger.DepthStencilState = DepthStencilState.FromDescription(
            ////    _device,
            ////    new DepthStencilStateDescription()
            ////    {
            ////        DepthComparison = Comparison.Always,
            ////        DepthWriteMask = DepthWriteMask.All,
            ////        IsDepthEnabled = true,
            ////        IsStencilEnabled = false
            ////    }
            ////);

            ////_context.OutputMerger.SetTargets(_depthStencil, _renderTarget);

            //DepthStencilStateDescription dssd = new DepthStencilStateDescription
            //{
            //    IsDepthEnabled = true,
            //    IsStencilEnabled = false,
            //    DepthWriteMask = DepthWriteMask.All,
            //    DepthComparison = Comparison.Less,
            //};

            //DepthStencilState depthStencilStateNormal;
            //depthStencilStateNormal = DepthStencilState.FromDescription(DeviceManager.Instance.device, dssd);
            //DeviceManager.Instance.context.OutputMerger.DepthStencilState = depthStencilStateNormal;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (!_disposedValue)
            //{
            //    if (disposing)
            //    {
            //        _renderTarget.Dispose();
            //        _swapChain.Dispose();
            //        _device.Dispose();
            //    }
            //    _disposedValue = true;
            //}
        }
        internal override void OnResized(object sender, EventArgs e)
        {
            //if (_renderTarget != null)
            //    _renderTarget.Dispose();
            //if (_resource != null)
            //    _resource.Dispose();
            //if (_depthStencil != null)
            //    _depthStencil.Dispose();

            //_swapChain.ResizeBuffers(2, (int)_control.Width, (int)_control.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);

            //_resource = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(_swapChain, 0);
            //_renderTarget = new RenderTargetView(_device, _resource);
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        protected override ThreadSubContext CreateSubContext(Thread thread)
        {
            throw new NotImplementedException();
        }
    }
}
