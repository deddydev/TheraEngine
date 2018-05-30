using TheraEngine.Animation;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableAnimationEditor : DockContent
    {
        public DockableAnimationEditor()
        {
            InitializeComponent();
        }
        public void SetAnimation(SkeletalAnimation anim)
            => meshControl1.SetAnimation(anim);

        private void DockableAnimationEditor_Load(object sender, System.EventArgs e)
        {

        }
    }
}
