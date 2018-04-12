using System;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public enum TextureType
    {
        Texture2D,
        Texture3D,
        TextureCubeMap
    }
    public class PrePushDataCallback
    {
        public bool ShouldPush { get; set; } = true;
    }
    public delegate void DelPrePushData(PrePushDataCallback callback);
    public abstract class BaseRenderTexture : BaseRenderState, IDisposable
    {
        public event DelPrePushData PrePushData;
        public event Action PostPushData;

        protected bool OnPrePushData()
        {
            PrePushDataCallback callback = new PrePushDataCallback();
            PrePushData?.Invoke(callback);
            return callback.ShouldPush;
        }
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
            => Engine.Renderer.CreateTexture(TextureTarget);
        protected override void PostGenerated()
            => PushData();

        public abstract void PushData();
    }
}
