using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TheraEngine.Core.Files;

namespace TheraEditor.Wrappers
{
    public class ThirdPartyFileWrapper : GenericFileWrapper
    {
        public ThirdPartyFileWrapper() : base() { }
        public ThirdPartyFileWrapper(ContextMenuStrip menu) : base(menu) { }
        public ThirdPartyFileWrapper(string path) : base(path)
        {

        }
    }
}
