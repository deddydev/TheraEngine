using System;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public enum ETextureType
    {
        Texture2D,
        Texture3D,
        TextureCubeMap
    }
    public class PrePushDataCallback
    {
        public bool ShouldPush { get; set; } = true;
        public bool AllowPostPushCallback { get; set; } = true;
    }
    public class PreBindCallback
    {
        public bool ShouldBind { get; set; } = true;
    }
    public delegate void DelPrePushData(PrePushDataCallback callback);
    public delegate void DelPreBind(PreBindCallback callback);
    public abstract class BaseRenderTexture : BaseRenderObject, IDisposable
    {
        public event DelPreBind PreBind;
        public event DelPrePushData PrePushData;
        public event Action PostPushData;

        protected bool OnPreBind()
        {
            PreBindCallback callback = new PreBindCallback();
            PreBind?.Invoke(callback);
            return callback.ShouldBind;
        }
        protected void OnPrePushData(out bool shouldPush, out bool allowPostPushCallback)
        {
            PrePushDataCallback callback = new PrePushDataCallback();
            PrePushData?.Invoke(callback);
            shouldPush = callback.ShouldPush;
            allowPostPushCallback = callback.AllowPostPushCallback;
        }
        protected void OnPostPushData() => PostPushData?.Invoke();

        public BaseRenderTexture() : base(EObjectType.Texture) { }
        public BaseRenderTexture(int bindingId) : base(EObjectType.Texture, bindingId) { }

        //public DataSource BitmapData { get; set; }
        public EPixelInternalFormat InternalFormat { get; set; } = EPixelInternalFormat.Rgba;
        public EPixelFormat PixelFormat { get; set; } = EPixelFormat.Rgba;
        public EPixelType PixelType { get; set; } = EPixelType.UnsignedByte;

        public abstract ETexTarget TextureTarget { get; }

        internal static T[] GenTextures<T>(int count) where T : BaseRenderTexture
            => Engine.Renderer.CreateObjects<T>(EObjectType.Texture, count);

        /// <summary>
        /// If true, this texture's data has been updated and needs to be pushed to the GPU.
        /// </summary>
        /// <returns></returns>
        public bool IsInvalidated { get; private set; } = true;
        /// <summary>
        /// Informs the renderer that this texture's data has been updated and needs to be pushed to the GPU.
        /// </summary>
        /// <returns></returns>
        public bool Invalidate() => IsInvalidated = true;

        public virtual void Bind()
        {
            if (OnPreBind())
            {
                int id = BindingId;
                if (id != NullBindingId)
                {
                    Engine.Renderer.BindTexture(TextureTarget, id);
                    if (IsInvalidated)
                    {
                        IsInvalidated = false;
                        PushData();
                    }
                }
            }
        }

        public void Clear(ColorF4 color, int level = 0)
        {
            var id = BindingId;
            if (id != NullBindingId)
            {
                Engine.Renderer.BindTexture(TextureTarget, id);
                Engine.Renderer.ClearTexImage(id, level, color);
            }
        }

        public abstract int MaxDimension { get; }

        /// <summary>
        /// Returns the level of the smallest allowed mipmap based on the maximum dimension of the base texture.
        /// </summary>
        public int SmallestMipmapLevel => //Note: 3.321928f is approx 1 / (log base 10 of 2)
            Math.Min((int)Math.Floor(Math.Log10(MaxDimension) * 3.321928f), SmallestAllowedMipmapLevel); 

        public int MinLOD { get; set; } = -1000;
        public int MaxLOD { get; set; } = 1000;
        public int LargestMipmapLevel { get; set; } = 0;
        public int SmallestAllowedMipmapLevel { get; set; } = 1000;
        public bool AutoGenerateMipmaps { get; set; } = false;

        internal void SetMipmapGenParams() => Engine.Renderer.SetMipmapParams(BindingId, MinLOD, MaxLOD, LargestMipmapLevel, SmallestAllowedMipmapLevel);
        internal void GenerateMipmaps() => Engine.Renderer.GenerateMipmap(TextureTarget);
        
        protected override int CreateObject()
            => Engine.Renderer.CreateTexture(TextureTarget);
        protected override void PostGenerated()
            => PushData();

        internal abstract void PushData();
    }
}
