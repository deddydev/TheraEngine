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
        private class ReaderXML : AbstractReader<XMLMemberTreeNode>
        {
            private FileStream _stream;
            private XmlReader _reader;

            private readonly XmlReaderSettings _settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                Async = true,
            };
            
            public ReaderXML(TDeserializer owner, XmlReaderSettings settings) : base(owner)
            {
                if (settings != null)
                    _settings = settings;
            }
            
            public override async Task WriteTree()
            {

            }
            public override async Task Finish()
            {
                _reader.Dispose();
                _stream.Dispose();
            }
            protected override void OnReportProgress()
            {
                Progress.Report((float)_stream.Position / _stream.Length);
            }
        }
    }
}
