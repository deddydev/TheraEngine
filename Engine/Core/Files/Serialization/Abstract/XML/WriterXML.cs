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
        private class WriterXML : AbstractWriter
        {
            private FileStream _stream;
            private XmlWriter _writer;

            private readonly XmlWriterSettings _settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                Async = true,
            };

            public WriterXML(TSerializer owner, MemberTreeNode rootNode, XmlWriterSettings settings) : base(owner, rootNode)
            {
                if (settings != null)
                    _settings = settings;
            }
            
            public override async Task Start()
            {
                _stream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan);
                _writer = XmlWriter.Create(_stream, _settings);
                _writer.Flush();
                _stream.Position = 0;
                await _writer.WriteStartDocumentAsync();
            }
            public override async Task Finish()
            {
                await _writer.WriteEndDocumentAsync();
                _writer.Dispose();
                _stream.Dispose();
            }
            protected override void OnReportProgress()
            {
                Progress.Report((float)_stream.Position / _stream.Length);
            }
            public override async Task WriteStartElementAsync(string name)
            {
                await _writer.WriteStartElementAsync(null, name, null);
            }
            public override async Task WriteEndElementAsync()
            {
                await _writer.WriteEndElementAsync();
            }
            public override async Task WriteAttributeStringAsync(string name, string value)
            {
                await _writer.WriteAttributeStringAsync(null, name, null, value);
            }
            public override async Task WriteElementStringAsync(string name, string value)
            {
                await _writer.WriteElementStringAsync(null, name, null, value);
            }
            protected override async Task WriteAsync(MemberTreeNode node)
            {
                await _writer.WriteEndElementAsync();
            }
        }
    }
}
