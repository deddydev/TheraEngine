using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEngine.Animation;
using TheraEngine.Components.Logic.Animation;
using TheraEngine.Files;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableAnimationList : DockContent
    {
        public DockableAnimationList()
        {
            InitializeComponent();
        }
        public void LoadAnimation(SkeletalAnimation anim)
        {
            listBox1.Items.Add(anim);
        }
        public void LoadAnimations(params SkeletalAnimation[] anims)
        {
            listBox1.Items.AddRange(anims);
        }
        public void LoadAnimations(IEnumerable<SkeletalAnimation> anims)
        {
            listBox1.Items.AddRange(anims.ToArray());
        }
        public void ClearAnimations()
        {
            listBox1.Items.Clear();
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!(DockPanel.FindForm() is ModelEditorForm form))
                return;
            if (!(listBox1.SelectedItem is SkeletalAnimation anim))
                return;
            AnimStateMachineComponent machine = form.TargetActor.GetLogicComponent<AnimStateMachineComponent>();
            machine.InitialState = new AnimState(new GlobalFileRef<SkelAnimPoseGenBase>(new SkelAnimDirectPose(anim)));
            theraPropertyGrid1.TargetFileObject = anim;
        }

        private async void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.GetFilter<SkeletalAnimation>(),
                Multiselect = true
            })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (string animPath in ofd.FileNames)
                        LoadAnimation(await TFileObject.LoadAsync<SkeletalAnimation>(animPath));
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }

        private void closeAllToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }
    }
}
