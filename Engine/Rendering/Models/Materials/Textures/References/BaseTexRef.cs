using TheraEngine.Files;
using TheraEngine.Rendering.Textures;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.Models.Materials
{
    public abstract class BaseTextureReference : FileObject
    {
        public abstract BaseRenderTexture GetTextureGeneric();
        public abstract Task<BaseRenderTexture> GetTextureGenericAsync();
    }
}
