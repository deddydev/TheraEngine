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
                if (_material != null)
                {
                    int w = -1;
                    int h = -1;
                    int tw = -1;
                    int th = -1;
                    foreach (var tex in _material.Textures)
                    {
                        if (tex?.FrameBufferAttachment is null)
                            continue;

                        if (tex is TexRef2D tref)
                        {
                            tw = tref.Width;
                            th = tref.Height;
                        }
                        else if (tex is TexRefView2D vref)
                        {
                            tw = vref.Width;
                            th = vref.Height;
                        }
                        if (w < 0)
                            w = tw;
                        else if (w != tw)
                        {
                            Engine.LogWarning($"FBO texture widths are not all the same.");
                        }
                        if (h < 0)
                            h = th;
                        else if (h != th)
                        {
                            Engine.LogWarning($"FBO texture heights are not all the same.");
                        }
                    }
                }
            }
        }
    }
}
