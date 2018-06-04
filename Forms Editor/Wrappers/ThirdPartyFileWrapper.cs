using System;
using System.Diagnostics;
using System.IO;
using TheraEngine.Files;

namespace TheraEditor.Wrappers
{
    public class ThirdPartyFileWrapper : GenericFileWrapper
    {
        public ThirdPartyFileWrapper() : base() { }
        public ThirdPartyFileWrapper(string path) : base(path)
        {

        }
    }
}
