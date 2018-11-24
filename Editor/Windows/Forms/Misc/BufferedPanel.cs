namespace System.Windows.Forms
{
    public class BufferedPanel : Panel
    {
        protected override void OnPaintBackground(PaintEventArgs e) { }
        public BufferedPanel() => SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.Opaque |
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.AllPaintingInWmPaint, 
                true);
    }
}
