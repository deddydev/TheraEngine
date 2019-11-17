using System.Windows.Forms;

namespace TheraEditor.Wrappers
{
    public class ThirdPartyFileWrapper : UnknownFileWrapper
    {
        public ThirdPartyFileWrapper() : base() { }
        public ThirdPartyFileWrapper(TheraMenu menu) : base(menu) { }
        public ThirdPartyFileWrapper(string path) : base(path)
        {

        }
    }
}
