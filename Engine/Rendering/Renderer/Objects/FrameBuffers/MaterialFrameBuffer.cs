using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class MaterialFrameBuffer : FrameBuffer
    {
        public MaterialFrameBuffer() { }
        public MaterialFrameBuffer(TMaterial m) => Material = m;
        
        private TMaterial _material;
        public TMaterial Material
        {
            get => _material;
            set
            {
                if (_material == value)
                    return;
                _material = value;
                SetRenderTargets(_material);
            }
        }
    }
}
