using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer
    {
        public class WriterXML : AbstractWriter
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

            public WriterXML(
                TSerializer owner,
                TFileObject rootFileObject,
                string filePath,
                ESerializeFlags flags,
                IProgress<float> progress,
                CancellationToken cancel,
                XmlWriterSettings settings)
                : base(owner, rootFileObject, filePath, flags, progress, cancel)
            {
                if (settings != null)
                    _settings = settings;
            }
            protected internal override async Task WriteTree(MemberTreeNode root)
            {
                _stream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan);
                _writer = XmlWriter.Create(_stream, _settings);
                await _writer.FlushAsync();
                _stream.Position = 0;
                await _writer.WriteStartDocumentAsync();
                await WriteElement(root, null);
                await _writer.WriteEndDocumentAsync();
                _writer.Dispose();
                _stream.Dispose();
            }
            private async Task WriteElement(MemberTreeNode node, string elementName)
            {
                await _writer.WriteStartElementAsync(null, elementName ?? node.ElementName, null);
                {
                    if (node.WriteAssemblyType)
                        await _writer.WriteAttributeStringAsync(null, SerializationCommon.TypeIdent, null, node.ObjectType.AssemblyQualifiedName);
                    
                    (string, object)[] attributes = node.ObjectWriter.Attributes;
                    MemberTreeNode[] childElements = node.ObjectWriter.ChildElements;
                    object serializableChildData = node.ObjectWriter.SingleSerializableChildData;

                    foreach (var attr in attributes)
                    {

                    }
                    if (serializableChildData != null)
                    {

                    }
                    else
                    {
                        foreach (MemberTreeNode childNode in childElements)
                        {

                        }
                    }
                }
                await _writer.WriteEndElementAsync();
            }
            protected override void OnReportProgress()
            {
                Progress.Report((float)_stream.Position / _stream.Length);
            }
        }
    }
}
