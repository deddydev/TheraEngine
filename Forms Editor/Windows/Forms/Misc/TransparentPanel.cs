using System.Drawing;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public class TransparentPanel : Panel
    {
        public TransparentPanel() : base()
        {
            BackColor = Color.Transparent;
        }
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
        //        return cp;
        //    }
        //}
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    //base.OnPaintBackground(e);
        //}
    }
}
