using TheraEngine.Rendering.Textures;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    //[EditorFor(typeof(ITextureFile))]
    public partial class TextureViewerForm : TheraForm, IDockPanelOwner
    {
        DockPanel IDockPanelOwner.DockPanelRef => dockPanel1;

        public TextureViewerForm()
        {
            InitializeComponent();
            dockPanel1.Theme = new TheraEditorTheme();
            FormTitle2.MouseDown += TitleBar_MouseDown;
            FormTitle2.MouseUp += (s, e) => 
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right && 
                    FormTitle.ClientRectangle.Contains(e.Location))
                    ShowSystemMenu(MouseButtons);
            };
        }
        public TextureViewerForm(ITextureFile texture) : this()
        {
            Texture = texture;
        }
        private ITextureFile _texture;
        public ITextureFile Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                if (value != null)
                    FormTitle2.Text = value.Name + " [" + value.FilePath + "]";
                else
                    FormTitle2.Text = string.Empty;
                if (Texture is TextureFile2D t2d)
                    pictureBox1.Image = t2d.Bitmaps[0];
            }
        }
    }
}
