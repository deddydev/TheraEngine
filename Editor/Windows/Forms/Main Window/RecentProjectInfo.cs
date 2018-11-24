using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public partial class RecentProjectInfoControl : UserControl
    {
        public RecentProjectInfoControl()
        {
            InitializeComponent();
        }
        public static RecentProjectInfoControl FromFile(string path)
        {
            return new RecentProjectInfoControl();
        }
    }
}
