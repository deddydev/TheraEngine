using System.Windows.Forms;

namespace TheraEditor.Wrappers
{
    public class ThirdPartyFileWrapper : UnknownFileWrapper
    {
        public ThirdPartyFileWrapper() : base() { }
        public ThirdPartyFileWrapper(ContextMenuStrip menu) : base(menu) { }
        public ThirdPartyFileWrapper(string path) : base(path)
        {

        }
    }
}
