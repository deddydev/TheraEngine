using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public enum TextureType
    {
        Texture2D,
        Texture3D,
        TextureCubeMap
    }
    public abstract class BaseRenderTexture : BaseRenderState, IDisposable
    {
        public event Action PrePushData;
        public event Action PostPushData;

        protected void OnPrePushData() => PrePushData?.Invoke();
        protected void OnPostPushData() => PostPushData?.Invoke();

        public BaseRenderTexture() : base(EObjectType.Texture) { }
        public BaseRenderTexture(int bindingId) : base(EObjectType.Texture, bindingId) { }

        public EPixelInternalFormat InternalFormat { get; set; } = EPixelInternalFormat.Rgba;
        public EPixelFormat PixelFormat { get; set; } = EPixelFormat.Rgba;
        public EPixelType PixelType { get; set; } = EPixelType.UnsignedByte;

        public abstract ETexTarget TextureTarget { get; }

        public static T[] GenTextures<T>(int count) where T : BaseRenderTexture
            => Engine.Renderer.CreateObjects<T>(EObjectType.Texture, count);

        public void Bind()
            => Engine.Renderer.BindTexture(TextureTarget, BindingId);
        public void Clear(ColorF4 clearColor, int level = 0)
            => Engine.Renderer.ClearTexImage(BindingId, level, clearColor);

        protected override int CreateObject()
            => Engine.Renderer.CreateTextures(TextureTarget, 1)[0];
        protected override void OnGenerated()
            => PushData();

        public abstract void PushData();
    }
}
