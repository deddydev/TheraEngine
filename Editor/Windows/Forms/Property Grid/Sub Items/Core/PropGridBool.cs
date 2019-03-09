using System;
using TheraEngine.Animation;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(bool))]
    public partial class PropGridBool : PropGridItem
    {
        public PropGridBool() => InitializeComponent();

        public override bool CanAnimate => true;
        protected override BasePropAnim CreateAnimation() => new PropAnimBool(0.0f, true, true);
        protected override bool UpdateDisplayInternal(object value)
        {
            chkValue.Checked = (bool)value;
            chkValue.Enabled = IsEditable();
            return false;
        }
        private void chkValue_CheckedChanged(object sender, EventArgs e)
            => UpdateValue(chkValue.Checked, true);
    }
}
