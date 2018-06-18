using System.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms
{
    public partial class TexRefControl : UserControl
    {
        public TexRefControl()
        {
            InitializeComponent();
        }
        public void SetTexRef(BaseTexRef texture)
        {
            lblTexName.Text = texture?.Name;
            if (texture is TexRef2D tref2d)
            {
                if (tref2d.Mipmaps != null &&
                    tref2d.Mipmaps.Length > 0 &&
                    tref2d.Mipmaps[0] != null &&
                    tref2d.Mipmaps[0].File != null &&
                    tref2d.Mipmaps[0].File.Bitmaps != null &&
                    tref2d.Mipmaps[0].File.Bitmaps.Length > 0)
                    texThumbnail.Image = tref2d.Mipmaps[0].File.Bitmaps[0];
            }
            theraPropertyGrid1.TargetFileObject = texture;
        }
    }
}
