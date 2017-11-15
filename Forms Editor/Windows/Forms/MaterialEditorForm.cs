using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms
{
    public partial class MaterialEditorForm : TheraForm
    {
        public MaterialEditorForm()
        {
            InitializeComponent();
        }
        
        public Material Material
        {
            get => materialEditor1.Material;
            set => materialEditor1.Material = value;
        }
    }
}
