using System.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms
{
    public partial class MaterialEditor : UserControl
    {
        private MaterialEditorHud _hud;

        public MaterialEditor()
        {
            InitializeComponent();
            renderPanel1.GlobalHud = _hud = new MaterialEditorHud(renderPanel1.ClientSize);
        }

        public Material Material
        {
            get => _hud.TargetMaterial;
            set => _hud.TargetMaterial = value;
        }
    }
}
