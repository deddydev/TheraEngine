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
                    
                    var attributes = node.ObjectWriter.Attributes;
                    var childElements = node.ObjectWriter.ChildElements;
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

            public override Task WriteStartElementAsync(string name)
            {
                throw new NotImplementedException();
            }

            public override Task WriteEndElementAsync()
            {
                throw new NotImplementedException();
            }

            public override Task WriteAttributeStringAsync(string name, string value)
            {
                throw new NotImplementedException();
            }

            public override Task WriteElementStringAsync(string name, string value)
            {
                throw new NotImplementedException();
            }

            protected override Task WriteAsync(MemberTreeNode node)
            {
                throw new NotImplementedException();
            }

            protected internal override bool ParseElementObject(MemberTreeNode member, out object result)
            {
                bool valid = SerializationCommon.GetString(member.Object, member.MemberInfo.VariableType, out string output);
                result = output;
                return valid;
            }
            internal protected override MemberTreeNode CreateNode(object obj, VarInfo memberInfo)
            {
                return new XMLMemberTreeNode(obj, memberInfo, this);
            }
            internal protected override MemberTreeNode CreateNode(object rootObject)
            {
                return new XMLMemberTreeNode(rootObject, this);
            }
        }
    }
}
