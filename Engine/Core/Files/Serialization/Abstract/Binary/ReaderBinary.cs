using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Files;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TDeserializer
    {
        private class ReaderBinary : AbstractReader
        {
            public ReaderBinary(TDeserializer owner) : base(owner)
            {

            }
        }
    }
}
