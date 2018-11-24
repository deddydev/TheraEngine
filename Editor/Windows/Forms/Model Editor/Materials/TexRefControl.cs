using System;
using System.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;

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
            if (theraPropertyGrid1.TargetFileObject == texture)
                return;

            lblTexName.Text = texture?.Name;
            if (texture is TexRef2D tref2d)
            {
                if (tref2d.Mipmaps != null &&
                    tref2d.Mipmaps.Length > 0 &&
                    tref2d.Mipmaps[0] != null)
                    tref2d.Mipmaps[0].RegisterLoadEvent(OnMipLoaded);
                else
                    texThumbnail.Image = null;

                tref2d.Renamed += Tref2d_Renamed;
            }
            else
                texThumbnail.Image = null;
            theraPropertyGrid1.TargetFileObject = texture;
        }
        private void OnMipLoaded(TextureFile2D obj)
        {
            if (obj?.Bitmaps != null && obj.Bitmaps.Length > 0)
                texThumbnail.Image = obj.Bitmaps[0];
            else
                texThumbnail.Image = null;
        }
        private void Tref2d_Renamed(TheraEngine.TObject node, string oldName)
        {
            lblTexName.Text = node?.Name;
        }
    }
}
