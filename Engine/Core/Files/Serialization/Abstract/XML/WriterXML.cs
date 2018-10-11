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
                XMLMemberTreeNode xmlRootNode = (XMLMemberTreeNode)root;
                _stream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.RandomAccess);
                _writer = XmlWriter.Create(_stream, _settings);
                await _writer.FlushAsync();
                _stream.Position = 0;
                await _writer.WriteStartDocumentAsync();
                await WriteElement(xmlRootNode);
                await _writer.WriteEndDocumentAsync();
                _writer.Dispose();
                _stream.Dispose();
            }
            private async Task WriteElement(XMLMemberTreeNode node)
            {
                await _writer.WriteStartElementAsync(null, node.ElementName, null);
                {
                    if (ReportProgress())
                    {
                        await _writer.WriteEndElementAsync();
                        return;
                    }

                    if (node.IsDerivedType)
                    {
                        await _writer.WriteAttributeStringAsync(null, SerializationCommon.TypeIdent, null, node.ObjectType.AssemblyQualifiedName);
                        if (ReportProgress())
                        {
                            await _writer.WriteEndElementAsync();
                            return;
                        }
                    }
                    
                    var attributes = node.Attributes;
                    var childElements = node.ChildElements;
                    string childStringData = node.ChildStringData;

                    foreach (var (Name, Value) in attributes)
                    {
                        await _writer.WriteAttributeStringAsync(null, Name, null, Value);
                        if (ReportProgress())
                        {
                            await _writer.WriteEndElementAsync();
                            return;
                        }
                    }

                    if (childStringData != null)
                    {
                        await _writer.WriteStringAsync(childStringData);
                        if (ReportProgress())
                        {
                            await _writer.WriteEndElementAsync();
                            return;
                        }
                    }
                    else
                        foreach (XMLMemberTreeNode childNode in childElements)
                            await WriteElement(childNode);
                }
                await _writer.WriteEndElementAsync();
                ReportProgress();
            }
            protected override void OnReportProgress()
                => Progress.Report((float)_stream.Position / _stream.Length);
            internal protected override MemberTreeNode CreateNode(object obj, VarInfo memberInfo)
                => new XMLMemberTreeNode(obj, memberInfo, this);
            internal protected override MemberTreeNode CreateNode(object rootObject)
                => new XMLMemberTreeNode(rootObject, this);
        }
    }
}
