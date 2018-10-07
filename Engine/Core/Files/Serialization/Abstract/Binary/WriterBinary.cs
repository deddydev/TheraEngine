using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer
    {
        private class WriterBinary : AbstractWriter
        {
            public WriterBinary(TSerializer owner) : base(owner)
            {

            }
        }
    }
}
