using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer
    {
        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into XML.</param>
        /// <param name="filePath">The path of the file to write.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        public async Task SerializeXMLAsync(
            object fileObject,
            string filePath,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            Writer = new WriterXML(this, fileObject, filePath, flags, progress, cancel, null);

            await Writer.CreateTreeAsync();
            await Writer.WriteTreeAsync();

            Engine.PrintLine("Serialized XML file to {0}", filePath);
        }
        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into XML.</param>
        /// <param name="targetDirectoryPath">The path to a directory to write the file in.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        public async Task SerializeXMLAsync(
            object fileObject,
            string targetDirectoryPath,
            string fileName,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            string filePath = TFileObject.GetFilePath(targetDirectoryPath, fileName, EProprietaryFileFormat.XML, fileObject.GetType());
            Writer = new WriterXML(this, fileObject, filePath, flags, progress, cancel, null);

            await Writer.CreateTreeAsync();
            await Writer.WriteTreeAsync();

            Engine.PrintLine("Serialized XML file to {0}", filePath);
        }
        public class WriterXML : AbstractWriter<XMLMemberTreeNode>
        {
            private FileStream _stream;
            private XmlWriter _writer;

            public override EProprietaryFileFormatFlag Format => EProprietaryFileFormatFlag.XML;

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
                object rootFileObject,
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
            protected internal override async Task WriteTreeAsync()
            {
                using (_stream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.RandomAccess))
                using (_writer = XmlWriter.Create(_stream, _settings))
                {
                    await _writer.FlushAsync();
                    _stream.Position = 0;

                    await _writer.WriteStartDocumentAsync();
                    await WriteElementAsync(RootNode);
                    await _writer.WriteEndDocumentAsync();
                }
            }
            private async Task WriteElementAsync(XMLMemberTreeNode node)
            {
                await _writer.WriteStartElementAsync(null, SerializationCommon.FixElementName(node.ElementName), null);
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
                    
                    List<XMLAttribute> attributes = node.Attributes;
                    List<XMLMemberTreeNode> childElements = node.ChildElements;
                    string childStringData = node.ElementString;

                    foreach (XMLAttribute attribute in attributes)
                    {
                        await _writer.WriteAttributeStringAsync(null, attribute.Name, null, attribute.Value);
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
                        {
                            await WriteElementAsync(childNode);
                            if (ReportProgress())
                            {
                                await _writer.WriteEndElementAsync();
                                return;
                            }
                        }
                }
                await _writer.WriteEndElementAsync();
            }
            
            public override XMLMemberTreeNode CreateNode(XMLMemberTreeNode parent, MemberInfo memberInfo)
                => new XMLMemberTreeNode(parent, memberInfo);
            public override XMLMemberTreeNode CreateNode(object root)
                => new XMLMemberTreeNode(root);
        }
    }
}
